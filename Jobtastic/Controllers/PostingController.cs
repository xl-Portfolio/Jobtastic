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
            return View(jobs);
        }
        public async Task<IActionResult> Form(int id)
        {
            if (!await _postingService.ProfileIsComplete())
                return View("ProfileIncomplete");
            ViewBag.Mandates = await _postingService.GetMandatesAsync();
            ViewBag.Contacts = await _postingService.GetContactsAsync();
            if (id == 0)
                return View();
            var job = await _postingService.GetJobById(id);
            if (job == null)
                return NotFound();
            if (!_postingService.IsAuthorized(job))
                return Unauthorized();
            return View(job);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEditJob(JobPosting job, IFormFile file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _postingService.IsAuthorizedForCompany(job.CompanyID))
                return Unauthorized();

            if (job.ContactID.HasValue && !await _postingService.IsAuthorizedForContact(job.ContactID.Value, job.CompanyID))
                return Unauthorized();

            if (job.ID == 0)
            {
                var jobAdded = await _postingService.AddJob_Successfully(job, file);
                if (!jobAdded)
                    return BadRequest();
            }
            else
            {
                var postingById = await _postingService.FindPosting(job);
                if (postingById == null)
                    return NotFound();
                if (!_postingService.IsAuthorized(postingById)) 
                    return Unauthorized();
                var jobEdited = await _postingService.EditJob_Successfully(job, file, postingById);
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