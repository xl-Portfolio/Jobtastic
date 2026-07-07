using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Jobtastic.Services
{
    public class PostingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PostingService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
        private string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public bool IsAuthorized(JobPosting job) => job.OwnerID == UserId || (User?.IsInRole("Admin") ?? false);
        public bool IsAuthorized(User user) => user.Id == UserId || (User?.IsInRole("Admin") ?? false);

        public async Task<JobPosting?> GetJobById(int id)
        {
            return await _context.Postings.SingleOrDefaultAsync(x => x.ID == id);
        }
        public async Task<List<JobPosting>> GetOwnedPostings()
        {
            var allJobs = await _context.Postings
                .Where(x => x.OwnerID == UserId)
                .Include(j => j.Company)
                .ToListAsync();
            return allJobs;
        }
        public async Task<bool> AddJob_Successfully(JobPosting job, IFormFile file) //Interaktion Speichern
        {
            //Uploaddate und Expirydate mit IsOnline verknüpfen
            //Admin und Owner muss Owner irgendwo ändern können
            job.OwnerID = UserId;
            //job.Company = ??
            await _context.Postings.AddAsync(job);
            var entitiesCreated = await _context.SaveChangesAsync();
            return entitiesCreated >= 1 ? true : false;
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
            await _context.SaveChangesAsync();
            
        }
    }
}
