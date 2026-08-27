namespace Jobtastic.Models
{
    /// <summary>
    /// A company's point of contact for a posting.
    /// JobContact is a pure data structure (an appendix / value-like entity), not a
    /// "domain actor".
    /// </summary>
    public class JobContact
    {
        public int ID { get; set; }
        public string? UserID { get; set; } //FK
        public User? User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ProfileImagePath { get; set; }
        public int CompanyID { get; set; } //FK
        public Company Company { get; set; }
        public string? Department { get; set; }
        public List<JobPosting> Postings { get; set; } = new();
        
    }
}