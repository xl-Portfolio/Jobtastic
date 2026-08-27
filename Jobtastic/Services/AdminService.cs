using Jobtastic.Data;
using Jobtastic.Identity;
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
        /// Every posting in the system with its owner, for the admin overview. Unlike
        /// the recruiter's own list this deliberately bypasses the ownership scope -
        /// the caller is already restricted to admins at controller level.
        /// </summary>
        public async Task<List<AdminPostingListModel>> GetPostingOverviewAsync() =>
            await _context.Postings
                .OrderByDescending(p => p.UploadDate)
                .ThenBy(p => p.JobTitle)
                .Select(p => new AdminPostingListModel
                {
                    Id = p.ID,
                    JobTitle = p.JobTitle,
                    CompanyName = p.Company.Name,
                    JobLocation = p.JobLocation,
                    OwnerEmail = p.Owner == null ? null : p.Owner.Email,
                    IsOnline = p.IsOnline,
                    UploadDate = p.UploadDate,
                    ExpiryDate = p.ExpiryDate,
                    StartDate = p.StartDate,
                    Klicks = p.Klicks
                })
                .ToListAsync();

        /// <summary>
        /// Every company with the counts that show how entangled it is - a company with
        /// postings or contacts cannot be removed by dropping the last mandate.
        /// </summary>
        public async Task<List<AdminCompanyListModel>> GetCompanyOverviewAsync() =>
            await _context.Companies
                .OrderBy(c => c.Name)
                .Select(c => new AdminCompanyListModel
                {
                    Id = c.ID,
                    Name = c.Name,
                    Description = c.Description,
                    WebsiteURL = c.WebsiteURL,
                    MandateHolderCount = c.Users.Count,
                    ContactCount = c.Contacts.Count,
                    PostingCount = c.Postings.Count
                })
                .ToListAsync();

        /// <summary>
        /// Every contact across all accounts, with the account that maintains it.
        /// </summary>
        public async Task<List<AdminContactListModel>> GetContactOverviewAsync() =>
            await _context.Contacts
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .Select(c => new AdminContactListModel
                {
                    Id = c.ID,
                    FullName = c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    Department = c.Department,
                    CompanyName = c.Company.Name,
                    OwnerEmail = c.User == null ? null : c.User.Email,
                    PostingCount = c.Postings.Count
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
