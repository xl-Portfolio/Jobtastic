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

        public async Task<IActionResult> Form(int id) 
        {
            if (id == 0)
                return View();
            var job = await GetJobByIdAsync(id);
            if (job == null)
                return NotFound();
            if (job.OwnerID != UserId && !User.IsInRole("Admin"))
                return Unauthorized();
            return View(job);
        }
        public async Task<IActionResult> CreateEditJob(JobPosting job, IFormFile file)
        {

            //Uploaddate und Expirydate mit IsOnline verknüpfen
            if (!User.IsInRole("Admin"))
                job.OwnerID = UserId; //Admin muss Owner irgendwo ändern können
   
            if (job.ID == 0)
                await _context.Postings.AddAsync(job);
            else
            {
                var postingById = await GetJobByIdAsync(job.ID);
                if (postingById == null)
                {
                    return NotFound();
                }
                postingById //.konkrete Property = job. konkrete Property
                    //für jede Property, die sinn macht
                _context.SaveChanges();
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
            return RedirectToAction("Index");
        }

        public IActionResult CreateEditJob(JobPosting job, IFormFile file)
        {
            // richtiger Benutzer
            job.OwnerName = User.Identity.Name;

            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    var byteArray = memoryStream.ToArray();
                    job.CompanyImage = byteArray;
                }
            }

            if (job == null)
            {
                return NotFound();
            }

            if (job.Id == 0)
            {
                _context.JobPostings.Add(job);
            }
            else
            {
                var jobPostingById = _context.JobPostings.SingleOrDefault(x => x.Id == job.Id);
                if (jobPostingById == null)
                {
                    return NotFound();
                }

                jobPostingById.JobTitle = job.JobTitle;
                jobPostingById.JobDescription = job.JobDescription;
                jobPostingById.CompanyName = job.CompanyName;
                jobPostingById.JobLocation = job.JobLocation;
                jobPostingById.CompanyMail = job.CompanyMail;
                jobPostingById.CompanyPhone = job.CompanyPhone;
                jobPostingById.Salary = job.Salary;
                //OwnerName nicht weil die Rolle des Besitzers würde geändert werden. 
            }

            return RedirectToAction("Index");
        }

    }
}