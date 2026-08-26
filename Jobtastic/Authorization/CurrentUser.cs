using System.Security.Claims;

namespace Jobtastic.Authorization
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _accessor;
        public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

        private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;
        public string? Id => Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        public bool IsAdmin => Principal?.IsInRole("Admin") ?? false;
        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;
    }
}
