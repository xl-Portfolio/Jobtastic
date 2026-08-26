namespace Jobtastic.Authorization
{
    public static class AccountAccess
    {
        public static bool MayTarget(string? callerId, string? requestedUserId, bool isAdmin) =>
            string.IsNullOrEmpty(requestedUserId) || requestedUserId == callerId || isAdmin;

        /// <summary>
        /// An admin may lock any account except their own - a self-lock cannot be
        /// undone by the account that caused it - and except the owner, who would
        /// otherwise be removable by way of a lockout.
        /// </summary>
        public static bool MayLock(string? callerId, string? targetUserId, bool targetIsOwner) =>
            !string.IsNullOrEmpty(targetUserId) && targetUserId != callerId && !targetIsOwner;

        /// <summary>
        /// An admin may not drop their own Admin role. Because the acting admin always
        /// keeps theirs, this alone already guarantees at least one admin remains - no
        /// separate "last admin" headcount is needed. The owner is additionally exempt,
        /// so an admin cannot depose the account that set the system up.
        /// </summary>
        public static bool MayRevokeAdmin(string? callerId, string? targetUserId, bool targetIsOwner) =>
            !string.IsNullOrEmpty(targetUserId) && targetUserId != callerId && !targetIsOwner;
    }
}