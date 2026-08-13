namespace Jobtastic.Models
{
    /// <summary>
    /// fachlicher Ansprechpartner
    /// JobContact = reine Datenstruktur (Appendix / Value-ähnliches Entity), kein “Domänen-Akteur”
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