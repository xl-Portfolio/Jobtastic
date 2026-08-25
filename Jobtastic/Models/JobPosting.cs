using System.ComponentModel.DataAnnotations;
using Jobtastic.Enums;

namespace Jobtastic.Models
{
    public class JobPosting
    {
        public int ID { get; set; }
        public int CompanyID { get; set; } //(FK)
        public Company Company { get; set; }
        public string JobTitle { get; set; }
        [MaxLength(60)]
        public string Header { get; set; }
        [MaxLength(5000)]
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
