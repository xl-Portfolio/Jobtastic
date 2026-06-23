namespace Jobtastic.Models
{
    public class JobApplicant
    {
        public int ID { get; set; }
        public string UserID { get; set; } //FK
        public User User { get; set; }
        public string? ProfileInfo { get; set; }
        public List<Application> Applications { get; set; }
    }
}