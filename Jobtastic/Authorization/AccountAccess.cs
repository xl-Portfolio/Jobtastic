namespace Jobtastic.Authorization
{
    public static class AccountAccess
    {
        public static bool MayTarget(string? callerId, string? requestedUserId, bool isAdmin) =>
            string.IsNullOrEmpty(requestedUserId) || requestedUserId == callerId || isAdmin;
    }
}