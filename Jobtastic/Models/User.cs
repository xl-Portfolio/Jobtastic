using Microsoft.AspNetCore.Identity;

namespace Jobtastic.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ProfileImagePath { get; set; }
        public int? CompanyID { get; set; } //FK
        public Company? Company { get; set; }
        public List<JobContact> Contacts { get; set; }
        public List<JobPosting> Postings { get; set; }


    }
}
