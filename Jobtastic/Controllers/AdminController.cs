using Jobtastic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobtastic.Controllers
{
    /// <summary>
    /// Administration area. Gated at controller level, so every action added here
    /// is admin-only by default rather than by remembering to annotate it.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Users() => View(await _adminService.GetUserOverviewAsync());
    }
}
