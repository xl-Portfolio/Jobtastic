using Jobtastic.Data;
using Jobtastic.Models;
using Jobtastic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Jobtastic.Controllers
{
    [Authorize]
    public class PostingController : Controller
    {
        private readonly PostingService _postingService;
        
        public PostingController(PostingService service)
        {
            _postingService = service;
        }
        public async Task<IActionResult> Index()
        {
            var jobs = await _postingService.GetOwnedPostings();
            ViewBag.HasMandate = await _postingService.ProfileIsComplete();
            return View(jobs);
        }
        public async Task<IActionResult> Form(int id)
        {
            if (!await _postingService.ProfileIsComplete())
                return RedirectToAction("Index");
            ViewBag.Mandates = await _postingService.GetMandatesAsync();
            ViewBag.Contacts = await _postingService.GetContactsAsync();
            if (id == 0)
                return View(new JobPostingInputModel { Fulltime = true, VolumeHours = 40, StartDate = DateTime.Today });
            var job = await _postingService.GetJobById(id);
            if (job == null)
                return NotFound();
            if (!_postingService.IsAuthorized(job))
                return Unauthorized();

            var input = new JobPostingInputModel
            {
                ID = job.ID,
                CompanyID = job.CompanyID,
                ContactID = job.ContactID,
                JobTitle = job.JobTitle,
                Header = job.Header,
                JobDescription = job.JobDescription,
                JobLocation = job.JobLocation,
                AnnualSalary = job.AnnualSalary,
                Fulltime = job.Fulltime,
                VolumeHours = job.VolumeHours,
                Mode = job.Mode,
                Experience = job.Experience,
                StartDate = job.StartDate,
                IsOnline = job.IsOnline
            };
            return View(input);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEditJob(JobPostingInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _postingService.IsAuthorizedForCompany(input.CompanyID))
                return Unauthorized();

            if (input.ContactID.HasValue && !await _postingService.IsAuthorizedForContact(input.ContactID.Value, input.CompanyID))
                return Unauthorized();

            if (input.ID == 0)
            {
                var jobAdded = await _postingService.AddJob_Successfully(input);
                if (!jobAdded)
                    return BadRequest();
            }
            else
            {
                var postingById = await _postingService.GetJobById(input.ID);
                if (postingById == null)
                    return NotFound();
                if (!_postingService.IsAuthorized(postingById))
                    return Unauthorized();
                var jobEdited = await _postingService.EditJob_Successfully(input, postingById);
                if (!jobEdited)
                    return BadRequest();
            }
            return RedirectToAction("Index");
        }

        //public async Task<IActionResult> Settings(User user)
        //{
        //    var userData = await _context.Users.SingleOrDefaultAsync(x => x.Id == user.Id);
        //    if (userData == null)
        //        return NotFound();
        //    if (!IsAuthorized(user))
        //        return Unauthorized();
        //    return View(userData);

        //}
        //public async Task<IActionResult> EditAccount(User user)
        //{
        //    var userData = await _context.Users.SingleOrDefaultAsync(x => x.Id == user.Id);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("Settings");
        //}

    }
}