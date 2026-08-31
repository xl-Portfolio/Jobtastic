using Jobtastic.Authorization;
using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.EntityFrameworkCore;

namespace Jobtastic.Services
{
    public class PostingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUser _me;
        public PostingService(ApplicationDbContext context, ICurrentUser me)
        {
            _context = context;
            _me = me;
        }
        public bool IsAuthorized(JobPosting job) => _me.IsAdmin || (_me.Id is not null && job.OwnerID == _me.Id);
        public bool IsAuthorized(User user) => _me.IsAdmin || (_me.Id is not null && user.Id == _me.Id);
        public async Task<bool> ProfileIsComplete()
        {
            var user = await _context.Users
                .Include(u => u.Companies)
                .SingleOrDefaultAsync(u => u.Id == _me.Id);
            return user?.Companies.Any() == true;
        }
        public async Task<List<JobContact>> GetContactsAsync()
        {
            var user = await _context.Users
                .Include(u => u.Contacts)
                .SingleOrDefaultAsync(u => u.Id == _me.Id);
            return user!.Contacts.ToList();
        }
        public async Task<List<Company>> GetMandatesAsync()
        {
            var user = await _context.Users
                .Include(u => u.Companies)
                .SingleOrDefaultAsync(u => u.Id == _me.Id);
            return user!.Companies.ToList();
        }
        /// <summary>
        /// Checks against the posting's owner, not the caller.
        /// </summary>
        public async Task<bool> OwnerHoldsMandate(string? ownerId, int companyId) =>
            ownerId is not null &&
            await _context.Users.AnyAsync(u => u.Id == ownerId && u.Companies.Any(c => c.ID == companyId));

        /// <summary>
        /// Like <see cref="OwnerHoldsMandate"/>, the assigned contact must
        /// belong to the owner and to the same company.
        /// </summary>
        public async Task<bool> OwnerHoldsContact(string? ownerId, int contactId, int companyId) =>
            ownerId is not null &&
            await _context.Contacts.AnyAsync(c => c.ID == contactId && c.CompanyID == companyId && c.UserID == ownerId);

        /// <summary>
        /// Marks postings in the given scope offline once their expiry date has
        /// passed.
        /// </summary>
        private async Task ExpireStalePostings(IQueryable<JobPosting> scope)
        {
            var stale = await scope.Expired().ToListAsync();
            if (stale.Count == 0)
                return;

            foreach (var posting in stale)
                posting.IsOnline = false;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Load a specific post for editing. The scope is limited to the current user and admins. 
        /// Returns null if not found or not authorized.
        /// </summary>
        public async Task<JobPosting?> GetJobById(int id)
        {
            var scope = _context.Postings.ManageableBy(_me);
            await ExpireStalePostings(scope.Where(x => x.ID == id));
            return await scope.SingleOrDefaultAsync(x => x.ID == id);
        }

        /// <summary>
        /// Detail view: publicly visible posts and owned posts (preview for owners and admins). 
        /// Otherwise null.
        /// </summary>
        public async Task<JobPosting?> GetJobDetailsById(int id)
        {
            var jobDetails = await _context.Postings
                .Include(j => j.Company)
                .Include(j => j.Contact)
                .SingleOrDefaultAsync(x => x.ID == id);

            if (jobDetails == null)
                return null;

            return jobDetails.IsPubliclyVisible() || IsAuthorized(jobDetails) ? jobDetails : null;
        }
        public async Task<List<JobPosting>> GetOwnedPostings()
        {
            var scope = _context.Postings.Where(x => x.OwnerID == _me.Id);
            await ExpireStalePostings(scope);

            var jobList = await scope
                .Include(j => j.Company)
                .ToListAsync();
            return jobList;
        }
        public async Task<List<JobPosting>> GetAllActivePostingsAsync()
        {
            var jobList = await _context.Postings
                .PubliclyVisible()
                .Include(j => j.Company)
                .ToListAsync();
            return jobList;
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
                OwnerID = _me.Id
            };

            if (job.IsOnline)
            {
                job.UploadDate = DateTime.Now;
                job.ExpiryDate = job.StartDate.AddMonths(3);
            }

            await _context.Postings.AddAsync(job);
            var entitiesCreated = await _context.SaveChangesAsync();
            return entitiesCreated >= 1;
        }
        /// <summary>
        /// Permanently removes a posting.
        /// </summary>
        public async Task<bool> DeleteJob_Successfully(JobPosting job)
        {
            _context.Postings.Remove(job);
            var entitiesDeleted = await _context.SaveChangesAsync();
            return entitiesDeleted >= 1;
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
                dbJob.ExpiryDate = dbJob.StartDate.AddMonths(3);
            }

            var entitiesChanged = await _context.SaveChangesAsync();
            return entitiesChanged >= 1;
        }
    }
}
