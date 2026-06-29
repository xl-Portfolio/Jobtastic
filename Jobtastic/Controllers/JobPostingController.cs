using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobtastic.Controllers
{
    [Authorize]

    public class JobPostingController : Controller
    {

        private readonly ApplicationDbContext _context;

        public JobPostingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var AllJobs = await _context.Postings.ToListAsync();
            return View(AllJobs);
        }

        public async Task<IActionResult> Form(int iD) //Ergänzen um Ownerprüfung
        {
            if (iD != 0)
            {
                var particularJob = await _context.Postings.SingleOrDefaultAsync(x => x.ID == iD);
                return View(particularJob);
            }
            else return View();
        }
        //public IActionResult CreateEditJob(JobPosting job, IFormFile file)
        //{
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