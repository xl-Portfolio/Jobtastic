using Microsoft.AspNetCore.Identity;

namespace Jobtastic.Models
{
    //public enum UserType
    //{
    //    CompanyContact, JobApplicant
    //}
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ProfileImagePath { get; set; }


        //public UserType UserType { get; set; }

        //public CompanyContact? CompanyContact { get; set; }
        //public JobApplicant? JobApplicant { get; set; }
    }
}
