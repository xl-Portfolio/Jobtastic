namespace Jobtastic.Models
{
    public class CompanyContact
    {
        public int ID { get; set; }
        public string UserID { get; set; } //FK
        public User User { get; set; }
        public int CompanyID { get; set; } //FK
        public Company Company { get; set; }
        public string? Department { get; set; }
        public List<JobPosting> Postings { get; set; }
        
    }
}