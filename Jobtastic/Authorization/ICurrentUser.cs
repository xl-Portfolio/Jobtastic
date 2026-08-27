namespace Jobtastic.Authorization
{
    /// <summary>
    /// Abstracts over the HttpContext to keep implementations testable. Provides the
    /// current user and their roles.
    /// </summary>
    public interface ICurrentUser
    {
        string? Id { get; }
        bool IsAdmin { get; }
        bool IsAuthenticated { get; }
    }
}
