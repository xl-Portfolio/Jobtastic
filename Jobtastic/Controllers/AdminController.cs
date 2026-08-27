using Jobtastic.Authorization;
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
        private readonly ICurrentUser _me;

        public AdminController(AdminService adminService, ICurrentUser me)
        {
            _adminService = adminService;
            _me = me;
        }

        public async Task<IActionResult> Users()
        {
            ViewBag.CurrentUserId = _me.Id;
            return View(await _adminService.GetUserOverviewAsync());
        }

        /// <summary>
        /// <paramref name="owner"/> preselects the table filter, so the posting count
        /// in the user overview can link straight to that account's postings.
        /// </summary>
        public async Task<IActionResult> Postings(string? owner)
        {
            ViewBag.OwnerFilter = owner;
            return View(await _adminService.GetPostingOverviewAsync());
        }

        [HttpPost]
        public async Task<IActionResult> SetLocked(string userId, bool locked)
        {
            // Unlocking is always safe; only locking needs the guards.
            if (locked && !AccountAccess.MayLock(_me.Id, userId, await _adminService.IsOwnerAsync(userId)))
                return Error("Dieses Konto kann nicht gesperrt werden.");

            var state = await _adminService.SetLockedAsync(userId, locked);
            if (state == null)
                return Error("Benutzer nicht gefunden.", StatusCodes.Status404NotFound);

            return Json(new { success = true, locked = state.Value });
        }

        [HttpPost]
        public async Task<IActionResult> SetAdminRole(string userId, bool isAdmin)
        {
            // Granting is always safe; only revoking could strand the system.
            if (!isAdmin && !AccountAccess.MayRevokeAdmin(_me.Id, userId, await _adminService.IsOwnerAsync(userId)))
                return Error("Diesem Konto kann die Admin-Rolle nicht entzogen werden.");

            var state = await _adminService.SetAdminRoleAsync(userId, isAdmin);
            if (state == null)
                return Error("Rollenänderung fehlgeschlagen.");

            return Json(new { success = true, isAdmin = state.Value });
        }

        private JsonResult Error(string message, int statusCode = StatusCodes.Status400BadRequest) =>
            new(new { success = false, errors = new[] { message } }) { StatusCode = statusCode };
    }
}
