using System.ComponentModel.DataAnnotations;

namespace Jobtastic.Enums
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
}
