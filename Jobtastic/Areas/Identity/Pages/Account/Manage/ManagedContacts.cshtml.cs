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
    public class ManagedContactsModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public InputModel Input { get; set; }
        public List<JobContact> Contacts { get; private set; }
        public List<Company> OwnedCompanies { get; private set; }

        [BindProperty]
        public int ContactId { get; set; }

        public ManagedContactsModel(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Vorname ist erforderlich.")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "Nachname ist erforderlich.")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "E-Mail ist erforderlich.")]
            [EmailAddress(ErrorMessage = "Bitte eine gültige E-Mail-Adresse angeben.")]
            public string Email { get; set; }
            [Phone(ErrorMessage = "Bitte eine gültige Telefonnummer angeben.")]
            public string? Phone { get; set; }
            public string? Department { get; set; }
            [Url(ErrorMessage = "Profilbild muss eine gültige URL sein (z. B. https://...).")]
            public string? ProfileImagePath { get; set; }
            public int CompanyID { get; set; }
        }

        private async Task<User?> GetUserWithContactsAsync()
        {
            var userId = _userManager.GetUserId(User);
            return await _context.Users
                .Include(u => u.Contacts).ThenInclude(c => c.Company)
                .Include(u => u.Companies)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        private IEnumerable<string> GetModelErrors() =>
            ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        private JsonResult ErrorJson(int statusCode, params string[] errors) =>
            new(new { success = false, errors }) { StatusCode = statusCode };

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await GetUserWithContactsAsync();
            if (user == null)
                return NotFound();

            Contacts = user.Contacts.ToList();
            OwnedCompanies = user.Companies.OrderBy(c => c.Name).ToList();
            Input = new InputModel();
            return Page();
        }

        // AJAX: Erfolg -> HTML-Fragment (Partial), Fehler -> JSON
        public async Task<IActionResult> OnPostAddContactAsync()
        {
            var user = await GetUserWithContactsAsync();
            if (user == null)
                return ErrorJson(StatusCodes.Status404NotFound, "Benutzer nicht gefunden.");

            if (!ModelState.IsValid)
                return ErrorJson(StatusCodes.Status400BadRequest, GetModelErrors().ToArray());

            var company = user.Companies.FirstOrDefault(c => c.ID == Input.CompanyID);
            if (company == null)
                return ErrorJson(StatusCodes.Status400BadRequest, "Firma gehört nicht zu deinen Mandaten.");

            var contact = new JobContact
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                Phone = Input.Phone,
                Department = Input.Department,
                ProfileImagePath = Input.ProfileImagePath,
                Company = company,
                UserID = user.Id
            };
            _context.Contacts.Add(contact);

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException)
            {
                return ErrorJson(StatusCodes.Status500InternalServerError, "Speichern fehlgeschlagen.");
            }

            return Partial("_ContactItem", contact);
        }

        public async Task<IActionResult> OnPostEditContactAsync()
        {
            var user = await GetUserWithContactsAsync();
            if (user == null)
                return ErrorJson(StatusCodes.Status404NotFound, "Benutzer nicht gefunden.");

            if (!ModelState.IsValid)
                return ErrorJson(StatusCodes.Status400BadRequest, GetModelErrors().ToArray());

            var contact = user.Contacts.FirstOrDefault(c => c.ID == ContactId);
            if (contact == null)
                return ErrorJson(StatusCodes.Status404NotFound, "Kontakt nicht gefunden oder keine Berechtigung.");

            contact.FirstName = Input.FirstName;
            contact.LastName = Input.LastName;
            contact.Email = Input.Email;
            contact.Phone = Input.Phone;
            contact.Department = Input.Department;
            contact.ProfileImagePath = Input.ProfileImagePath;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException)
            {
                return ErrorJson(StatusCodes.Status500InternalServerError, "Speichern fehlgeschlagen.");
            }

            return Partial("_ContactItem", contact);
        }

        public async Task<IActionResult> OnPostDeleteContactAsync()
        {
            var user = await GetUserWithContactsAsync();
            if (user == null)
                return ErrorJson(StatusCodes.Status404NotFound, "Benutzer nicht gefunden.");

            var contact = user.Contacts.FirstOrDefault(c => c.ID == ContactId);
            if (contact == null)
                return ErrorJson(StatusCodes.Status404NotFound, "Kontakt nicht gefunden oder keine Berechtigung.");

            var hasPostings = await _context.Postings.AnyAsync(p => p.ContactID == contact.ID);
            if (hasPostings)
                return ErrorJson(StatusCodes.Status400BadRequest,
                    "Dieser Kontakt kann nicht gelöscht werden, solange ihm noch Stellenanzeigen zugeordnet sind.");

            _context.Contacts.Remove(contact);

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException)
            {
                return ErrorJson(StatusCodes.Status500InternalServerError, "Löschen fehlgeschlagen.");
            }

            return new JsonResult(new { success = true, id = contact.ID });
        }
    }
}