using Jobtastic.Authorization;
using Jobtastic.Data;
using Jobtastic.Models;
using Jobtastic.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Jobtastic.Services
{
    public class ApiJobpostingService
    {
        public readonly ApplicationDbContext _context;
        public ApiJobpostingService(ApplicationDbContext context)
        {
            _context = context;
        }
        private static readonly Expression<Func<JobPosting, JobPostingDto>> ToDto = p => new JobPostingDto
        {
            ID = p.ID,
            JobTitle = p.JobTitle,
            Header = p.Header,
            JobDescription = p.JobDescription,
            JobLocation = p.JobLocation,
            AnnualSalary = p.AnnualSalary,
            Fulltime = p.Fulltime,
            VolumeHours = p.VolumeHours,
            Mode = p.Mode,
            Experience = p.Experience,
            StartDate = p.StartDate,
            Company = new CompanyDto
            {
                ID = p.Company.ID,
                Name = p.Company.Name,
                WebsiteURL = p.Company.WebsiteURL
            },
            Contact = p.Contact == null ? null : new JobContactDto
            {
                ID = p.Contact.ID,
                FirstName = p.Contact.FirstName,
                LastName = p.Contact.LastName,
                Email = p.Contact.Email,
                Phone = p.Contact.Phone
            }
        };
        public async Task<List<JobPostingDto>> GetAllPostings() =>
            await _context.Postings
                .PubliclyVisible()
                .Select(ToDto)
                .ToListAsync();
        public async Task<JobPostingDto?> GetPostingById(int id) =>
            await _context.Postings
                .PubliclyVisible()
                .Where(p => p.ID == id)
                .Select(ToDto)
                .SingleOrDefaultAsync();
    }
}
