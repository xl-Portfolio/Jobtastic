using Jobtastic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jobtastic.Areas.Identity.Pages.Account.Manage
{
    public class CompanyMandateModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        [BindProperty]
        public InputModel Input { get; set; }

        public CompanyMandateModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public class InputModel
        {

        }

        //public void OnGet()
        //{
        //}
    }
}
