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
        public async Task<bool> ProfileIsComplete()
        {
            var user = await _context.Users
                .Include(u => u.Companies)
                .SingleOrDefaultAsync(u => u.Id == UserId);
            return user?.Companies.Any() == true;
        }
        public async Task<List<Company>> GetCompanyMandatesAsync()
        {
            var user = await _context.Users
                .Include(u => u.Companies)
                .SingleOrDefaultAsync(u => u.Id == UserId);
            return user!.Companies.ToList();
        }

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
            //job.Company = User.
            await _context.Postings.AddAsync(job);
            var entitiesCreated = await _context.SaveChangesAsync();
            return entitiesCreated >= 1 ? true : false;
        }
        public async Task<JobPosting?> FindPosting(JobPosting job)
        {
            return await GetJobById(job.ID);
        }
        public async Task<bool> EditJob_Successfully(JobPosting formJob, IFormFile file, JobPosting dbJob)
        {
            dbJob.JobTitle = formJob.JobTitle;
            dbJob.Experience = formJob.Experience;
            dbJob.StartDate = formJob.StartDate;
            dbJob.Header = formJob.Header;
            dbJob.JobDescription = formJob.JobDescription;
            dbJob.JobLocation = formJob.JobLocation;
            dbJob.AnnualSalary = formJob.AnnualSalary;
            dbJob.Fulltime = formJob.Fulltime;
            dbJob.VolumeHours = formJob.VolumeHours;
            dbJob.Mode = formJob.Mode;
            dbJob.IsOnline = formJob.IsOnline;
            dbJob.UploadDate = DateTime.Now;
            dbJob.ExpiryDate = dbJob.UploadDate.AddMonths(6);
            //CompanyId (FK) ??
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
            var entitiesChanged = await _context.SaveChangesAsync();
            return entitiesChanged >= 1 ? true : false;
        }

         
         
    }
}
