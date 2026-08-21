using System.ComponentModel.DataAnnotations;

namespace Jobtastic.Enums
{
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
}
