using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Jobtastic.Areas.Identity.Pages.Account.Manage
{
    public class CompanyMandateModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public InputModel Input { get; set; }
        public List<Company> Companies { get; private set; }

        [BindProperty]
        public int CompanyId { get; set; }


        public CompanyMandateModel(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public class InputModel
        {
            [Required]
            public string Name { get; set; }
            public string? Description { get; set; }
            public string? LogoImageSource { get; set; }
            public string? WebsiteURL { get; set; }
            public int? NumberEmployees { get; set; }
        }
        private async Task<User> GetCompaniesbyUserAsync()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.Companies)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await GetCompaniesbyUserAsync();
            if (user == null)
                return NotFound();

            Companies = user.Companies.ToList();

            Input = new InputModel();

            return Page();
        }
        public async Task<IActionResult> OnPostAddMandateAsync()
        {
            var user = await GetCompaniesbyUserAsync();
            if (user == null) 
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var company = new Company
            {
                Name = Input.Name,
                Description = Input.Description,
                LogoImageSource = Input.LogoImageSource,
                WebsiteURL = Input.WebsiteURL,
                NumberEmployees = Input.NumberEmployees
            };

            user.Companies.Add(company);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return RedirectToPage();

            ModelState.AddModelError("", "Speichern fehlgeschlagen.");
            return Page();
        }
        public async Task<IActionResult> OnPostEditMandateAsync()
        {
            var user = await GetCompaniesbyUserAsync();
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var company = user.Companies.FirstOrDefault(c => c.ID == CompanyId);
            if (company == null)
                return NotFound();

            company.Name = Input.Name;
            company.Description = Input.Description;
            company.LogoImageSource = Input.LogoImageSource;
            company.WebsiteURL = Input.WebsiteURL;
            company.NumberEmployees = Input.NumberEmployees;

            var result = await _context.SaveChangesAsync();
            if (result > 0) 
                return RedirectToPage();

            ModelState.AddModelError("", "Speichern fehlgeschlagen.");
            return Page();
        }
    }
}
