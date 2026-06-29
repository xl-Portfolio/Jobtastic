namespace Jobtastic.Models
{
    public enum Mode { OnSite, FullRemote, Hybrid }
    public enum Experience { Intern, Entry, Junior, Professionel, Senior }
    public class JobPosting
    {
        public int ID { get; set; }
        public int CompanyID { get; set; } //(FK)
        public Company Company { get; set; }
        public string JobTitle { get; set; }
        public string Header { get; set; }
        public string JobDescription { get; set; }
        public string JobLocation { get; set; } 
        public double AnnualSalary { get; set; }
        public Boolean Fulltime { get; set; }
        public double? VolumeHours { get; set; }
        public Mode Mode { get; set; }
        public Experience Experience {  get; set; } 
        public DateTime StartDate { get; set; }
        public bool IsOnline { get; set; }
        public int Klicks { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? OwnerID { get; set; } //FK
        public User? Owner { get; set; }


    }
}
