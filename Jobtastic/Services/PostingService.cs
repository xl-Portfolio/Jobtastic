using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
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
        public async Task<List<JobContact>> GetContactsAsync()
        {
            var user = await _context.Users
                .Include(u => u.Contacts)
                .SingleOrDefaultAsync(u => u.Id == UserId);
            return user!.Contacts.ToList();
        }
        public async Task<bool> IsAuthorizedForContact(int contactId, int companyId)
        {
            var contacts = await GetContactsAsync();
            return contacts.Any(c => c.ID == contactId && c.CompanyID == companyId);
        }
        public async Task<List<Company>> GetMandatesAsync()
        {
            var user = await _context.Users
                .Include(u => u.Companies)
                .SingleOrDefaultAsync(u => u.Id == UserId);
            return user!.Companies.ToList();
        }
        public async Task<bool> IsAuthorizedForCompany(int companyId)
        {
            var mandates = await GetMandatesAsync();
            return mandates.Any(m => m.ID == companyId);
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
        public async Task<bool> AddJob_Successfully(JobPosting job, IFormFile file)
        {
            job.OwnerID = UserId;

            if (job.IsOnline)
            {
                job.UploadDate = DateTime.Now;
                job.ExpiryDate = job.UploadDate.AddMonths(6);
            }

            await _context.Postings.AddAsync(job);
            var entitiesCreated = await _context.SaveChangesAsync();
            return entitiesCreated >= 1;
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
            if (dbJob.IsOnline)
            {
                dbJob.UploadDate = DateTime.Now;
                dbJob.ExpiryDate = dbJob.UploadDate.AddMonths(6);
            }

            var entitiesChanged = await _context.SaveChangesAsync();
            return entitiesChanged >= 1;
        }

         
         
    }
}
