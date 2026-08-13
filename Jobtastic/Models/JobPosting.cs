using System.ComponentModel.DataAnnotations;

namespace Jobtastic.Models
{
    public enum Mode 
    {
        [Display(Name = "Vor Ort")]
        OnSite,
        [Display(Name = "Homeoffice")] 
        FullRemote,
        [Display(Name = "Hybrid")]
        Hybrid 
    }
    public enum Experience 
    {
        [Display(Name = "Praktikum")]
        Intern,
        [Display(Name = "Berufseinstieg")]
        Entry,
        [Display(Name = "Junior")]
        Junior,
        [Display(Name = "Berufserfahren")]
        Professional,
        [Display(Name = "Senior")]
        Senior 
    }
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

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public bool IsOnline { get; set; }
        public int Klicks { get; set; } = 0;
        public DateTime UploadDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? OwnerID { get; set; } //FK
        public User? Owner { get; set; }
        public int? ContactID { get; set; } //FK
        public JobContact? Contact { get; set; }


    }
}
