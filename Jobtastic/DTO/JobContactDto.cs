using Jobtastic.Models;

namespace Jobtastic.DTO
{
    public class JobContactDto
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
    }
}
