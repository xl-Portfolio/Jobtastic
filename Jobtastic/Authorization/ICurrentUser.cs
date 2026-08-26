namespace Jobtastic.Authorization
{
    /// <summary>
    /// CurrentUser Interface, um den HttpContext zu kapseln und spätere Testbarkeit zu ermöglichen. 
    /// Implementierungen können den aktuellen Benutzer und seine Rollen bereitstellen.
    /// </summary>
    public interface ICurrentUser
    {
        string? Id { get; }
        bool IsAdmin { get; }
        bool IsAuthenticated { get; }
    }
}
