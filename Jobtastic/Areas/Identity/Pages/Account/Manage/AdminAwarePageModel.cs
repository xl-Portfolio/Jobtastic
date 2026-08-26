using Jobtastic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Jobtastic.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    /// Base for account-management pages that can target either the caller's own
    /// account or, for admins, another account via a `userId` parameter.
    /// </summary>
    public abstract class AdminAwarePageModel : PageModel
    {
        protected readonly UserManager<User> UserManager;

        protected AdminAwarePageModel(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        protected string? CallerId => UserManager.GetUserId(User);

        /// <summary>Id of the account this request resolved to.</summary>
        public string? TargetUserId { get; private set; }

        /// <summary>True when managing an account other than the caller's own.</summary>
        public bool IsActingOnBehalfOfOther { get; private set; }

        /// <summary>
        /// Resolves the target account: the caller by default, or
        /// the account named by <paramref name="userId"/> for admins. Non-admins targeting
        /// someone else get 403, not 404 (this is a role check, not a lookup).
        /// <paramref name="include"/> adds the caller's own EF includes.
        /// </summary>
        protected async Task<(User? User, IActionResult? Error)> ResolveTargetUserAsync(
            string? userId, Func<IQueryable<User>, IQueryable<User>> include)
        {
            var callerId = CallerId;
            var targetId = string.IsNullOrEmpty(userId) ? callerId : userId;

            if (targetId != callerId && !User.IsInRole("Admin"))
                return (null, Forbid());

            var user = await include(UserManager.Users).FirstOrDefaultAsync(u => u.Id == targetId);
            if (user == null)
                return (null, NotFound());

            TargetUserId = targetId;
            IsActingOnBehalfOfOther = targetId != callerId;
            return (user, null);
        }
    }
}
