using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.AspNetCore.Http;
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
            [Required(ErrorMessage = "Firmenname ist erforderlich.")]
            public string Name { get; set; }
            public string? Description { get; set; }
            public string? LogoImageSource { get; set; }
            public string? WebsiteURL { get; set; }
        }
        private async Task<User?> GetCompaniesByUserAsync()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.Companies)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user;
        }
        private static object ToDto(Company c) => new
        {
            id = c.ID,
            name = c.Name,
            description = c.Description,
            logoImageSource = c.LogoImageSource,
            websiteURL = c.WebsiteURL,
        };

        private IEnumerable<string> GetModelErrors() =>
            ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        private JsonResult ErrorJson(int statusCode, params string[] errors) =>
            new(new { success = false, errors }) { StatusCode = statusCode };
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await GetCompaniesByUserAsync();
            if (user == null)
                return NotFound();

            Companies = user.Companies.ToList();
            Input = new InputModel();
            return Page();
        }
        // AJAX: Erfolg -> HTML-Fragment (Partial), Fehler -> JSON
        public async Task<IActionResult> OnPostAddMandateAsync()
        {
            var user = await GetCompaniesByUserAsync();
            if (user == null)
                return ErrorJson(StatusCodes.Status404NotFound, "Benutzer nicht gefunden.");

            if (!ModelState.IsValid)
                return ErrorJson(StatusCodes.Status400BadRequest, GetModelErrors().ToArray());

            var company = new Company
            {
                Name = Input.Name,
                Description = Input.Description,
                LogoImageSource = Input.LogoImageSource,
                WebsiteURL = Input.WebsiteURL
            };
            user.Companies.Add(company);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return ErrorJson(
                    StatusCodes.Status500InternalServerError,
                    "Speichern fehlgeschlagen.");
            }

            return Partial("_MandateItem", company);
        }
        public async Task<IActionResult> OnPostEditMandateAsync()
        {
            var user = await GetCompaniesByUserAsync();
            if (user == null)
                return ErrorJson(StatusCodes.Status404NotFound, "Benutzer nicht gefunden.");

            if (!ModelState.IsValid)
                return ErrorJson(StatusCodes.Status400BadRequest, GetModelErrors().ToArray());

            var company = user.Companies.FirstOrDefault(c => c.ID == CompanyId);
            if (company == null)
                return ErrorJson(StatusCodes.Status404NotFound, "Firma nicht gefunden oder keine Berechtigung.");

            company.Name = Input.Name;
            company.Description = Input.Description;
            company.LogoImageSource = Input.LogoImageSource;
            company.WebsiteURL = Input.WebsiteURL;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return ErrorJson(
                    StatusCodes.Status500InternalServerError,
                    "Speichern fehlgeschlagen.");
            }

            return Partial("_MandateItem", company);
        }
    }
}
