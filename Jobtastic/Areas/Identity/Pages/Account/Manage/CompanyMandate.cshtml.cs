using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jobtastic.Areas.Identity.Pages.Account.Manage
{
    public class CompanyMandateModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public InputModel Input { get; set; }

        public CompanyMandateModel(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public class InputModel
        {
            
            public string Name { get; set; }
            public string? Description { get; set; }
            public string? LogoImageSource { get; set; }
            public string? WebsiteURL { get; set; }
            public int? NumberEmployees { get; set; }
        }

        //public void OnGet()
        //{
        //}
    }
}
