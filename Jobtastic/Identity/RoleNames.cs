namespace Jobtastic.Identity
{
    /// <summary>
    /// Role names, ordered from highest privilege to lowest.
    ///
    /// Roles are stored additively: every account holds <see cref="User"/>, an admin
    /// additionally holds <see cref="Admin"/>, and the founding account additionally
    /// holds <see cref="Owner"/>. The UI presents only the highest one, so accounts
    /// read as if they had exactly one role.
    /// </summary>
    public static class RoleNames
    {
        /// <summary>
        /// The founding account. Never granted or revoked through the UI - it exists
        /// so that an admin cannot strip or lock out the account that set the system
        /// up, not even one the owner promoted themselves.
        /// </summary>
        public const string Owner = "Owner";

        public const string Admin = "Admin";
        public const string User = "User";

        /// <summary>Seeded at startup, highest privilege first.</summary>
        public static readonly string[] All = { Owner, Admin, User };
    }
}
