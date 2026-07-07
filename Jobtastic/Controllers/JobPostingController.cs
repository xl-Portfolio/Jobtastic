using Jobtastic.Data;
using Jobtastic.Models;
using Jobtastic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Jobtastic.Controllers
{
    [Authorize]

    public class JobPostingController : Controller
    {
        private readonly PostingService _postingService;
        
        public JobPostingController(PostingService service)
        {
            _postingService = service;
        }
        public async Task<IActionResult> Index()
        {
            var jobs = await _postingService.GetOwnedPostings();
            return View(jobs);
        }
        public async Task<IActionResult> Form(int id)
        {
            if (id == 0)
                return View();
            var job = await _postingService.GetJobById(id);
            if (job == null)
                return NotFound();
            if (!_postingService.IsAuthorized(job))
                return Unauthorized();
            return View(job);
        }

        public async Task<IActionResult> CreateEditJob(JobPosting job, IFormFile file) //Interaktion Speichern+Ändern
        {
            if (job.ID == 0)
            {
                var jobAdded = await _postingService.AddJob_Successfully(job, file);
                if (!jobAdded)
                    return BadRequest();
            }
            else
            {
                var postingById = await _postingService.GetJobById(job.ID);
                if (postingById == null)
                {
                    return NotFound();
                }
                if (!_postingService.IsAuthorized(postingById))
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
            



            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Settings(User user)
        {
            var userData = await _context.Users.SingleOrDefaultAsync(x => x.Id == user.Id);
            if (userData == null)
                return NotFound();
            if (!IsAuthorized(user))
                return Unauthorized();
            return View(userData);

        }
        public async Task<IActionResult> EditAccount(User user)
        {
            var userData = await _context.Users.SingleOrDefaultAsync(x => x.Id == user.Id);
            await _context.SaveChangesAsync();
            return RedirectToAction("Settings");
        }

    }
}