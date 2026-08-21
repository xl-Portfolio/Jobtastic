using Jobtastic.Models;
using System.ComponentModel.DataAnnotations;
using Jobtastic.Enums;

namespace Jobtastic.DTO
{
    public class JobPostingDto
    {
        public int ID { get; set; }
        public CompanyDto Company { get; set; }
        public JobContactDto? Contact { get; set; }
        public string JobTitle { get; set; }
        public string Header { get; set; }
        public string JobDescription { get; set; }
        public string JobLocation { get; set; }
        public double AnnualSalary { get; set; }
        public Boolean Fulltime { get; set; }
        public double? VolumeHours { get; set; }
        public Mode Mode { get; set; }
        public Experience Experience { get; set; }
        public DateTime StartDate { get; set; }

    }
}
