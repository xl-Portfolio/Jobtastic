using Jobtastic.Data;
using Jobtastic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Jobtastic.Areas.Identity.Pages.Account.Manage
{
    public class CompanyMandateModel : AdminAwarePageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public InputModel Input { get; set; }
        public List<Company> Companies { get; private set; }

        [BindProperty]
        public int CompanyId { get; set; }
        public int? ExistingCompanyId { get; set; }
        [BindProperty]
        public bool ForceCreate { get; set; }


        public CompanyMandateModel(UserManager<User> userManager, ApplicationDbContext context) : base(userManager)
        {
            _context = context;
        }
        public class InputModel
        {
            [Required(ErrorMessage = "Firmenname ist erforderlich.")]
            public string Name { get; set; }
            public string? Description { get; set; }
            [Url(ErrorMessage = "Logo-URL muss eine gültige URL sein (z. B. https://...).")]
            public string? LogoImageSource { get; set; }
            [Url(ErrorMessage = "Webseite muss eine gültige URL sein (z. B. https://...).")]
            public string? WebsiteURL { get; set; }
        }
        private static IQueryable<User> WithCompanies(IQueryable<User> users) => users.Include(u => u.Companies);

        /// <summary>
        /// Literalzeichen, die als SQL-Wildcards fungieren, werden escaped.
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        private static string EscapeLikeTerm(string term) =>
            term.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

        private IEnumerable<string> GetModelErrors() =>
            ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        private JsonResult ErrorJson(int statusCode, params string[] errors) =>
            new(new { success = false, errors }) { StatusCode = statusCode };
        public async Task<IActionResult> OnGetAsync(string? userId)
        {
            var (user, error) = await ResolveTargetUserAsync(userId, WithCompanies);
            if (error != null)
                return error;

            Companies = user!.Companies.ToList();
            Input = new InputModel();
            return Page();
        }
        public async Task<IActionResult> OnPostAddMandateAsync(string? userId)
        {
            var (user, error) = await ResolveTargetUserAsync(userId, WithCompanies);
            if (error != null)
                return error;

            if (ExistingCompanyId.HasValue && ExistingCompanyId.Value > 0)
            {
                var existing = await _context.Companies.FirstOrDefaultAsync(c => c.ID == ExistingCompanyId.Value);
                if (existing == null)
                    return ErrorJson(StatusCodes.Status404NotFound, "Firma nicht gefunden.");

                if (user!.Companies.Any(c => c.ID == existing.ID))
                    return ErrorJson(StatusCodes.Status400BadRequest, "Du hast dieses Mandat bereits.");

                user.Companies.Add(existing);

                try { await _context.SaveChangesAsync(); }
                catch (DbUpdateException)
                {
                    return ErrorJson(StatusCodes.Status500InternalServerError, "Speichern fehlgeschlagen.");
                }

                return Partial("_MandateItem", existing);
            }

            if (!ModelState.IsValid)
                return ErrorJson(StatusCodes.Status400BadRequest, GetModelErrors().ToArray());

            if (!ForceCreate)
            {
                var name = Input.Name.Trim();
                var escapedName = EscapeLikeTerm(name);

                var candidates = await _context.Companies
                    .Where(c => EF.Functions.Like(c.Name, "%" + escapedName + "%", "\\")
                             || EF.Functions.Like(name, "%" + c.Name.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%", "\\"))
                    .OrderBy(c => c.Name)
                    .Take(5)
                    .ToListAsync();

                if (candidates.Any())
                {
                    var ownedCandidate = candidates.FirstOrDefault(c => user!.Companies.Any(uc => uc.ID == c.ID));
                    if (ownedCandidate != null)
                        return ErrorJson(StatusCodes.Status400BadRequest, "Du hast dieses Mandat bereits.");

                    return new JsonResult(new
                    {
                        success = false,
                        conflict = true,
                        candidates = candidates.Select(c => new { id = c.ID, name = c.Name })
                    })
                    { StatusCode = StatusCodes.Status409Conflict };
                }
            }

            var company = new Company
            {
                Name = Input.Name,
                Description = Input.Description,
                LogoImageSource = Input.LogoImageSource,
                WebsiteURL = Input.WebsiteURL
            };
            user!.Companies.Add(company);

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException)
            {
                return ErrorJson(StatusCodes.Status500InternalServerError, "Speichern fehlgeschlagen.");
            }

            return Partial("_MandateItem", company);
        }
        public async Task<IActionResult> OnPostEditMandateAsync(string? userId)
        {
            var (user, error) = await ResolveTargetUserAsync(userId, WithCompanies);
            if (error != null)
                return error;

            if (!ModelState.IsValid)
                return ErrorJson(StatusCodes.Status400BadRequest, GetModelErrors().ToArray());

            var company = user!.Companies.FirstOrDefault(c => c.ID == CompanyId);
            if (company == null)
                return ErrorJson(StatusCodes.Status404NotFound, "Firma nicht gefunden oder keine Berechtigung.");

            var newName = Input.Name.Trim();
            var nameChanged = !string.Equals(company.Name, newName, StringComparison.OrdinalIgnoreCase);

            if (nameChanged && !ForceCreate)
            {
                var escapedNewName = EscapeLikeTerm(newName);

                var candidates = await _context.Companies
                    .Where(c => c.ID != company.ID &&
                        (EF.Functions.Like(c.Name, "%" + escapedNewName + "%", "\\")
                         || EF.Functions.Like(newName, "%" + c.Name.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%", "\\")))
                    .OrderBy(c => c.Name)
                    .Take(5)
                    .ToListAsync();

                if (candidates.Any())
                {
                    return new JsonResult(new
                    {
                        success = false,
                        conflict = true,
                        candidates = candidates.Select(c => new { id = c.ID, name = c.Name })
                    })
                    { StatusCode = StatusCodes.Status409Conflict };
                }
            }

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
                return ErrorJson(StatusCodes.Status500InternalServerError, "Speichern fehlgeschlagen.");
            }

            return Partial("_MandateItem", company);
        }
        public async Task<IActionResult> OnPostDeleteMandateAsync(string? userId)
        {
            var (user, error) = await ResolveTargetUserAsync(userId, WithCompanies);
            if (error != null)
                return error;

            var company = user!.Companies.FirstOrDefault(c => c.ID == CompanyId);
            if (company == null)
                return ErrorJson(StatusCodes.Status404NotFound, "Firma nicht gefunden oder keine Berechtigung.");

            var hasContacts = await _context.Contacts.AnyAsync(c => c.CompanyID == company.ID);
            if (hasContacts)
                return ErrorJson(StatusCodes.Status400BadRequest,
                    "Diese Firma kann nicht gel�scht werden, solange ihr noch Kontakte zugeordnet sind.");

            var hasPostings = await _context.Postings.AnyAsync(p => p.CompanyID == company.ID);
            if (hasPostings)
                return ErrorJson(StatusCodes.Status400BadRequest,
                    "Diese Firma kann nicht gel�scht werden, solange ihr noch Stellenanzeigen zugeordnet sind.");

            user.Companies.Remove(company);
            await _context.Entry(company).Collection(c => c.Users).LoadAsync();
            if (!company.Users.Any())
            {
                _context.Companies.Remove(company);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return ErrorJson(StatusCodes.Status500InternalServerError, "L�schen fehlgeschlagen.");
            }

            return new JsonResult(new { success = true, id = company.ID });
        }
        public async Task<IActionResult> OnGetSearchCompaniesAsync(string? term, string? userId)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Trim().Length < 2)
                return new JsonResult(new { companies = Array.Empty<object>() });

            var (user, error) = await ResolveTargetUserAsync(userId, WithCompanies);
            if (error != null)
                return error;

            term = term.Trim();
            var existingIds = user!.Companies.Select(c => c.ID).ToList();

            var matches = await _context.Companies
                .Where(c => EF.Functions.Like(c.Name, $"%{term}%") && !existingIds.Contains(c.ID))
                .OrderBy(c => c.Name)
                .Take(8)
                .Select(c => new
                {
                    id = c.ID,
                    name = c.Name,
                    logoImageSource = c.LogoImageSource,
                    websiteURL = c.WebsiteURL,
                    description = c.Description
                })
                .ToListAsync();

            return new JsonResult(new { companies = matches });
        }
    }
}
