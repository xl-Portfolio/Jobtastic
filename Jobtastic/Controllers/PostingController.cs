using Jobtastic.Authorization;
using Jobtastic.Models;
using Jobtastic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobtastic.Controllers
{
    [Authorize]
    public class PostingController : Controller
    {
        private readonly PostingService _postingService;
        private readonly ICurrentUser _me;

        public PostingController(PostingService service, ICurrentUser me)
        {
            _postingService = service;
            _me = me;
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
        /// <summary>
        /// Renders the form again with the submitted values and the validation errors
        /// still in ModelState.
        /// </summary>
        private async Task<IActionResult> RedisplayFormAsync(JobPostingInputModel input)
        {
            ViewBag.Mandates = await _postingService.GetMandatesAsync();
            ViewBag.Contacts = await _postingService.GetContactsAsync();
            return View("Form", input);
        }

        /// <summary>
        /// Deletes a posting for good. GetJobById is scoped to what the caller may
        /// manage, so a foreign id is indistinguishable from a missing one.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _postingService.GetJobById(id);
            if (job == null)
                return Json(new { success = false, errors = new[] { "Inserat nicht gefunden." } });

            if (!await _postingService.DeleteJob_Successfully(job))
                return Json(new { success = false, errors = new[] { "Löschen fehlgeschlagen." } });

            return Json(new { success = true, id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditJob(JobPostingInputModel input)
        {
            // Plain form post, so errors belong back in the form rather than in a raw
            // JSON response. The dropdowns are populated per request and have to be
            // restored before re-rendering.
            if (!ModelState.IsValid)
                return await RedisplayFormAsync(input);

            if (input.ID == 0)
            {
                if (!await _postingService.OwnerHoldsMandate(_me.Id, input.CompanyID))
                    return Unauthorized();
                if (input.ContactID.HasValue && !await _postingService.OwnerHoldsContact(_me.Id, input.ContactID.Value, input.CompanyID))
                    return Unauthorized();

                var jobAdded = await _postingService.AddJob_Successfully(input);
                if (!jobAdded)
                    return BadRequest();
            }
            else
            {
                var postingById = await _postingService.GetJobById(input.ID);
                if (postingById == null)
                    return NotFound();
                if (!await _postingService.OwnerHoldsMandate(postingById.OwnerID, input.CompanyID))
                    return Unauthorized();
                if (input.ContactID.HasValue && !await _postingService.OwnerHoldsContact(postingById.OwnerID, input.ContactID.Value, input.CompanyID))
                    return Unauthorized();

                var jobEdited = await _postingService.EditJob_Successfully(input, postingById);
                if (!jobEdited)
                    return BadRequest();
            }
            return RedirectToAction("Index");
        }
    }
}