using Jobtastic.Enums;

namespace Jobtastic.Models
{
    /// <summary>
    /// One row of the admin posting overview. Flattened like
    /// <see cref="AdminUserListModel"/> so the list comes from a single query.
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

        /// <summary>
        /// A posting that was never published has no upload date, so that check comes
        /// first: without it a draft would read as expired, its expiry being unset and
        /// therefore in the past. After that the expiry decides, because the periodic
        /// sweep clears IsOnline once a posting runs out - "offline" is then reserved
        /// for the one case a person caused: taken down while still valid.
        /// </summary>
        public PostingStatus Status =>
            UploadDate == default ? PostingStatus.Draft
            : ExpiryDate <= DateTime.Now ? PostingStatus.Expired
            : IsOnline ? PostingStatus.Online
            : PostingStatus.Offline;
    }
}
