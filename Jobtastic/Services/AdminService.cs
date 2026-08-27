using Jobtastic.Authorization;
using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jobtastic.Services
{
    /// <summary>
    /// Read and write operations reserved for administrators. Callers are expected
    /// to be role-gated already; this service does not re-check the caller's role.
    /// </summary>
    public class AdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// All accounts with their roles and related-record counts, for the admin
        /// overview.
        /// </summary>
        public async Task<List<AdminUserListModel>> GetUserOverviewAsync() =>
            await _context.Users
                .OrderBy(u => u.Email)
                .Select(u => new AdminUserListModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt,
                    LockoutEnd = u.LockoutEnd,
                    MandateCount = u.Companies.Count,
                    ContactCount = u.Contacts.Count,
                    PostingCount = u.Postings.Count,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .ToList()
                })
                .ToListAsync();

        /// <summary>
        /// Locks or unlocks an account. Refreshing the security stamp invalidates any
        /// session the account already holds, so a lock takes effect within the
        /// configured validation interval instead of only at the next sign-in.
        /// Returns the resulting locked state, or null if the account is unknown.
        /// </summary>
        public async Task<bool?> SetLockedAsync(string userId, bool locked)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, locked ? DateTimeOffset.MaxValue : null);
            await _userManager.UpdateSecurityStampAsync(user);

            return locked;
        }

        /// <summary>
        /// Grants or revokes the Admin role and refreshes the security stamp, so the
        /// change reaches an already signed-in account without a re-login.
        /// Returns the resulting admin state, or null if the account is unknown.
        /// </summary>
        public async Task<bool?> SetAdminRoleAsync(string userId, bool isAdmin)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var result = isAdmin
                ? await _userManager.AddToRoleAsync(user, RoleNames.Admin)
                : await _userManager.RemoveFromRoleAsync(user, RoleNames.Admin);

            if (!result.Succeeded)
                return null;

            await _userManager.UpdateSecurityStampAsync(user);

            return isAdmin;
        }

        /// <summary>
        /// Whether the account is the founding one, which is exempt from being locked
        /// or stripped of its Admin role.
        /// </summary>
        public async Task<bool> IsOwnerAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, RoleNames.Owner);
        }
    }
}
