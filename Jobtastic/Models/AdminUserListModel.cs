using Jobtastic.Identity;

namespace Jobtastic.Models
{
    /// <summary>
    /// One row of the admin user overview. Flattened on purpose so the list can be
    /// produced by a single query instead of loading full User graphs per row
    /// </summary>
    public class AdminUserListModel
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public List<string?> Roles { get; set; } = new();
        public int MandateCount { get; set; }
        public int ContactCount { get; set; }
        public int PostingCount { get; set; }

        /// <summary>
        /// A lockout only counts while its end date is still in the future
        /// </summary>
        public bool IsLocked => LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.Now;

        public bool IsOwner => Roles.Contains(RoleNames.Owner);

        public bool IsAdmin => Roles.Contains(RoleNames.Admin);

        /// <summary>
        /// The highest role held
        /// </summary>
        public string EffectiveRole =>
            IsOwner ? RoleNames.Owner
            : IsAdmin ? RoleNames.Admin
            : RoleNames.User;
    }
}
