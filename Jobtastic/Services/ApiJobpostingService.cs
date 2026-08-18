using Jobtastic.Data;
using Jobtastic.Models;
using Jobtastic.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Nodes;

namespace Jobtastic.Services
{
    public class ApiJobpostingService
    {
        public readonly ApplicationDbContext _context;
        public ApiJobpostingService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<JobPostingDto>> GetAllPostings() 
        {
            var allPostings = await _context.Postings
                .Where(p => p.IsOnline)
                .Select(p => new JobPostingDto
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
                })
                .ToListAsync();
            return allPostings;
        }
        public async Task<JobPostingDto> GetPostingById(int id)
        {
            var posting = await _context.Postings
                .Where(p => p.ID == id)
                .Select(p => new JobPostingDto
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
                })
                .SingleOrDefaultAsync();
            return posting;
        }
    }
}
