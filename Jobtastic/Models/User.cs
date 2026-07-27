using Microsoft.AspNetCore.Identity;

namespace Jobtastic.Models
{
    /// <summary>
    /// Systemkonto
    /// </summary>
    public class User : IdentityUser 
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<JobContact> Contacts { get; set; } = new();
        public List<JobPosting> Postings { get; set; } = new();
        public List<Company> Companies { get; set; } = new();

    }
}
