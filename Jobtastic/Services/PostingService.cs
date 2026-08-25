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
        public async Task<JobPosting?> GetJobDetailsById(int id)
        {
            return await _context.Postings
                .Include(j => j.Company)
                .Include(j => j.Contact)
                .SingleOrDefaultAsync(x => x.ID == id);
        }
        public async Task<List<JobPosting>> GetOwnedPostings()
        {
            var allJobs = await _context.Postings
                .Where(x => x.OwnerID == UserId)
                .Include(j => j.Company)
                .ToListAsync();
            return allJobs;
        }
        public async Task<bool> AddJob_Successfully(JobPostingInputModel input)
        {
            var job = new JobPosting
            {
                CompanyID = input.CompanyID,
                ContactID = input.ContactID,
                JobTitle = input.JobTitle,
                Header = input.Header,
                JobDescription = input.JobDescription,
                JobLocation = input.JobLocation,
                AnnualSalary = input.AnnualSalary,
                Fulltime = input.Fulltime,
                VolumeHours = input.VolumeHours,
                Mode = input.Mode,
                Experience = input.Experience,
                StartDate = input.StartDate,
                IsOnline = input.IsOnline,
                OwnerID = UserId
            };

            if (job.IsOnline)
            {
                job.UploadDate = DateTime.Now;
                job.ExpiryDate = job.UploadDate.AddMonths(6);
            }

            await _context.Postings.AddAsync(job);
            var entitiesCreated = await _context.SaveChangesAsync();
            return entitiesCreated >= 1;
        }
        public async Task<bool> EditJob_Successfully(JobPostingInputModel input, JobPosting dbJob)
        {
            dbJob.JobTitle = input.JobTitle;
            dbJob.Experience = input.Experience;
            dbJob.StartDate = input.StartDate;
            dbJob.Header = input.Header;
            dbJob.JobDescription = input.JobDescription;
            dbJob.JobLocation = input.JobLocation;
            dbJob.AnnualSalary = input.AnnualSalary;
            dbJob.Fulltime = input.Fulltime;
            dbJob.VolumeHours = input.VolumeHours;
            dbJob.Mode = input.Mode;
            dbJob.CompanyID = input.CompanyID;
            dbJob.ContactID = input.ContactID;

            dbJob.IsOnline = input.IsOnline;
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
