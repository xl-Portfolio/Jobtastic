namespace Jobtastic.Models
{
    public class Company
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? LogoImageSource { get; set; }
        public string? WebsiteURL { get; set; }
        public List<JobPosting> Postings { get; set; } = new();
        public List<JobContact> Contacts { get; set; } = new();
        public List<User> Users { get; set; } = new();
    }
}