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

        public async Task<IActionResult> Index()
        {
            var AllJobs = await _context.Postings
                .Where(x => x.OwnerID == UserId)
                .Include(j => j.Company)
                .ToListAsync();
            return View(AllJobs);
        }

        public async Task<IActionResult> Form(int id) 
        {
            if (id == 0)
                return View();
            var particularJob = await _context.Postings.SingleOrDefaultAsync(x => x.ID == id);
            if (particularJob == null)
                return NotFound();
            if (particularJob.OwnerID != UserId && !User.IsInRole("Admin"))
                return Unauthorized();
            return View(particularJob);
        }
        //public IActionResult CreateEditJob(JobPosting job, IFormFile file)
        //{

        //    Uploaddate und Expirydate mit IsOnline verknüpfen
        //    job.OwnerName = User.Identity.Name; //ownername muss noch implementiert werden im model
        //    if (file != null)
        //    {
        //        using (var memoryStream = new MemoryStream()) //Bild als bytearray speichern in db
        //        {
        //            file.CopyTo(memoryStream);
        //            var byteArray = memoryStream.ToArray();
        //            job.CompanyImage = byteArray; //muss in db angelegt werden (Logo?)
        //        }
        //    }
        //    else { return NotFound(); }

        //    if (job.ID == 0)
        //    {
        //        _context.JobPostings.Add(job);
        //    }
        //    else
        //    {
        //        var jobPostingbyID = _context.JobPostings.SingleOrDefault(x => x.ID == job.ID);
        //        if (jobPostingbyID == null)
        //        {
        //            return NotFound();
        //        }
        //        jobPostingbyID //.konkrete Property = job. konkrete Property
        //            //für jede Property, die sinn macht
        //        _context.SaveChanges();
        //    }

        //    return RedirectToAction("Index");
        //}


    }
}