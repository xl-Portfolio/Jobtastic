using Jobtastic.Data;
using Jobtastic.Models;
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

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
