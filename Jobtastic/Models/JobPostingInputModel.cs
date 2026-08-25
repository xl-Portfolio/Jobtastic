using System.ComponentModel.DataAnnotations;
using Jobtastic.Enums;

namespace Jobtastic.Models
{
    public class JobPostingInputModel : IValidatableObject
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Firma ist erforderlich.")]
        public int CompanyID { get; set; }
        public int? ContactID { get; set; }
        [Required(ErrorMessage = "Stellenbezeichnung ist erforderlich.")]
        public string JobTitle { get; set; }
        [Required(ErrorMessage = "Header ist erforderlich.")]
        [MaxLength(60)]
        public string Header { get; set; }
        [Required(ErrorMessage = "Beschreibung ist erforderlich.")]
        [MaxLength(5000)]
        public string JobDescription { get; set; }
        [Required(ErrorMessage = "Ort ist erforderlich.")]
        public string JobLocation { get; set; }
        public double AnnualSalary { get; set; }
        public bool Fulltime { get; set; }
        public double? VolumeHours { get; set; }
        public Mode Mode { get; set; }
        public Experience Experience { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsOnline { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "Startdatum darf nicht in der Vergangenheit liegen.",
                    new[] { nameof(StartDate) });
            }
        }
    }
}
