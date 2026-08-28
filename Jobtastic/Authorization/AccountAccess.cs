namespace Jobtastic.Authorization
{
    public static class AccountAccess
    {
        public static bool MayTarget(string? callerId, string? requestedUserId, bool isAdmin) =>
            string.IsNullOrEmpty(requestedUserId) || requestedUserId == callerId || isAdmin;

        /// <summary>
        /// An admin may lock any account except their own and the owner.
        /// </summary>
        public static bool MayLock(string? callerId, string? targetUserId, bool targetIsOwner) =>
            !string.IsNullOrEmpty(targetUserId) && targetUserId != callerId && !targetIsOwner;

        /// <summary>
        /// An admin may not drop their own Admin role.
        /// </summary>
        public static bool MayRevokeAdmin(string? callerId, string? targetUserId, bool targetIsOwner) =>
            !string.IsNullOrEmpty(targetUserId) && targetUserId != callerId && !targetIsOwner;
    }
}