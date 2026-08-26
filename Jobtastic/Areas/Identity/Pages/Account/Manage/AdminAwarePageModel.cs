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
        public string? TargetUserId { get; private set; }
        public string? TargetUserName { get; private set; }
        public bool IsActingOnBehalfOfOther { get; private set; }

        /// <summary>
        /// Name for page headings: the managed account while acting on behalf of
        /// someone, otherwise the caller's own.
        /// </summary>
        public string? DisplayUserName => IsActingOnBehalfOfOther ? TargetUserName : User.Identity?.Name;

        /// <summary>
        /// Value to append as userId on links and form posts so the managed account
        /// survives navigation. Null on one's own account, which keeps those URLs clean.
        /// </summary>
        public string? TargetUserIdForLinks => IsActingOnBehalfOfOther ? TargetUserId : null;

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
            TargetUserName = user.UserName;
            IsActingOnBehalfOfOther = targetId != callerId;
            return (user, null);
        }
    }
}
