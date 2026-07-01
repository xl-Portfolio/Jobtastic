using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Jobtastic.Controllers
{
    [Authorize]

    public class JobPostingController : Controller
    {

        private readonly ApplicationDbContext _context;
        private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private bool IsAuthorized(JobPosting job) => job.OwnerID == UserId || User.IsInRole("Admin");

        public JobPostingController(ApplicationDbContext context)
        {
            _context = context;
        }
        private async Task<JobPosting?> GetJobByIdAsync(int id)
        {
            return await _context.Postings.SingleOrDefaultAsync(x => x.ID == id);
        }
        public async Task<IActionResult> Index()
        {
            var allJobs = await _context.Postings
                .Where(x => x.OwnerID == UserId)
                .Include(j => j.Company)
                .ToListAsync();
            return View(allJobs);
        }

        public async Task<IActionResult> Form(int id) //View
        {
            if (id == 0)
                return View();
            var job = await GetJobByIdAsync(id);
            if (job == null)
                return NotFound();
            if (!IsAuthorized(job))
                return Unauthorized();
            return View(job);
        }
        public async Task<IActionResult> CreateEditJob(JobPosting job, IFormFile file) //Interaktion Speichern+Ändern
        {
            //Uploaddate und Expirydate mit IsOnline verknüpfen
            //Admin und Owner muss Owner irgendwo ändern können
            

            if (job.ID == 0)
            {
                job.OwnerID = UserId;
                //job.Company = ??
                await _context.Postings.AddAsync(job);
            }
            else
            {
                var postingById = await GetJobByIdAsync(job.ID);
                if (postingById == null)
                {
                    return NotFound();
                }
                if (!IsAuthorized(postingById))
                    return Unauthorized();

                postingById.JobTitle = job.JobTitle;
                postingById.Experience = job.Experience;
                postingById.StartDate = job.StartDate;
                postingById.Header = job.Header;
                postingById.JobDescription = job.JobDescription;
                postingById.JobLocation = job.JobLocation;
                postingById.AnnualSalary = job.AnnualSalary;
                postingById.Fulltime = job.Fulltime;
                postingById.VolumeHours = job.VolumeHours;
                postingById.Mode = job.Mode;
                postingById.IsOnline = job.IsOnline;
                postingById.UploadDate = System.DateTime.Now;
                postingById.ExpiryDate = postingById.UploadDate.AddMonths(6);
                    
                    
                    //CompanyId (FK) ??

                
                
            }
            //if (file != null) //Bild speichern
            //{
            //    using (var memoryStream = new MemoryStream()) //Bild als bytearray speichern in db
            //    {
            //        file.CopyTo(memoryStream);
            //        var byteArray = memoryStream.ToArray();
            //        job.CompanyImage = byteArray; //muss in db angelegt werden (Logo?)
            //    }
            //}
            //else { return NotFound(); }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}