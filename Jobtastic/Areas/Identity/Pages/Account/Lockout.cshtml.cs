using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jobtastic.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Shown when a sign-in attempt is rejected because the account is locked.
    /// Reached via the redirect in <see cref="LoginModel"/>.
    /// </summary>
    [AllowAnonymous]
    public class LockoutModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
