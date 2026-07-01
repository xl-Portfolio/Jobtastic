using Microsoft.AspNetCore.Identity;

namespace Jobtastic.Models
{
    /// <summary>
    /// Systemkonto
    /// </summary>
    public class User : IdentityUser 
    {
        public int? CompanyID { get; set; } //FK
        public Company? Company { get; set; }
        public List<JobContact> Contacts { get; set; }
        public List<JobPosting> Postings { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
