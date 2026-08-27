using Jobtastic.Enums;

namespace Jobtastic.Models
{
    /// <summary>
    /// One row of the admin posting overview. Flattened so the list comes from a single query.
    /// </summary>
    public class AdminPostingListModel
    {
        public int Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string JobLocation { get; set; } = string.Empty;

        /// <summary>Null for a posting whose owner account no longer exists.</summary>
        public string? OwnerEmail { get; set; }

        public bool IsOnline { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime StartDate { get; set; }
        public int Klicks { get; set; }
        public PostingStatus Status =>
            UploadDate == default ? PostingStatus.Draft
            : ExpiryDate <= DateTime.Now ? PostingStatus.Expired
            : IsOnline ? PostingStatus.Online
            : PostingStatus.Offline;
    }
}
