namespace Jobtastic.Models
{
    /// <summary>
    /// One row of the admin contact overview. Contacts are reachable per account
    /// through the user overview as well; this view adds the cross-account search.
    /// </summary>
    public class AdminContactListModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>Null for a contact whose owning account no longer exists.</summary>
        public string? OwnerEmail { get; set; }

        public int PostingCount { get; set; }
    }
}
