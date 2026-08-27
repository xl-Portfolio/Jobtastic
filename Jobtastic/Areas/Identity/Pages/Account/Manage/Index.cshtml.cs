// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Jobtastic.Models;

namespace Jobtastic.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : AdminAwarePageModel
    {
        [BindProperty]
        public InputModel Input {  get; set; }

        [BindProperty]
        public PasswordInputModel PasswordInput { get; set; }

        /// <summary>Roles of the displayed account, so an admin profile is recognizable as one.</summary>
        public IList<string> TargetUserRoles { get; private set; } = new List<string>();

        public IndexModel(UserManager<User> userManager) : base(userManager)
        {
        }
        public class InputModel
        {
            [Required(ErrorMessage = "E-Mail ist erforderlich.")]
            [EmailAddress(ErrorMessage = "Bitte eine gültige E-Mail-Adresse angeben.")]
            public string Email { get; set; }
            [Phone(ErrorMessage = "Bitte eine gültige Telefonnummer angeben.")]
            public string? PhoneNumber { get; set; }
        }
        public class PasswordInputModel
        {
            [Required(ErrorMessage = "Bitte das aktuelle Passwort angeben.")]
            public string Password { get; set; }
            [Required(ErrorMessage = "Bitte ein neues Passwort angeben.")]
            public string NewPassword { get; set; }
            [Required(ErrorMessage = "Bitte das neue Passwort wiederholen.")]
            [Compare("NewPassword", ErrorMessage = "Die Passwörter stimmen nicht überein.")]
            public string ConfirmedPassword { get; set; }
        }
        /// <summary>
        /// The page hosts two independent forms that both bind to this model, so a
        /// post from one carries no values for the other's [Required] fields and
        /// would fail validation on its behalf. Discards the incoming validation
        /// state and re-runs it against the submitted sub-model only.
        /// </summary>
        private bool ValidateOnly<TModel>(TModel model, string prefix) where TModel : class
        {
            ModelState.Clear();
            return TryValidateModel(model, prefix);
        }

        // Both forms are submitted by fetch, so failures answer with the same JSON
        // shape the mandate and contact pages use and are shown as a dialog. Returning
        // BadRequest(ModelState) instead would replace the page with raw JSON.
        private IEnumerable<string> GetModelErrors() =>
            ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);

        private JsonResult ErrorJson(params string[] errors) =>
            new(new { success = false, errors }) { StatusCode = StatusCodes.Status400BadRequest };

        private JsonResult ErrorJson(IEnumerable<IdentityError> errors) =>
            ErrorJson(errors.Select(e => e.Description).ToArray());

        public async Task<IActionResult> OnGetAsync(string? userId)
        {
            var (user, error) = await ResolveTargetUserAsync(userId, users => users);
            if (error != null)
                return error;

            TargetUserRoles = await UserManager.GetRolesAsync(user!);

            Input = new InputModel
            {
                Email = user!.Email!,
                PhoneNumber = user.PhoneNumber,
            };
            PasswordInput = new PasswordInputModel
            {

            };

            return Page();
        }
        public async Task<IActionResult> OnPostEditDataAsync(string? userId)
        {
            var (user, error) = await ResolveTargetUserAsync(userId, users => users);
            if (error != null)
                return error;

            if (!ValidateOnly(Input, nameof(Input)))
                return ErrorJson(GetModelErrors().ToArray());

            var result = await UserManager.SetEmailAsync(user!, Input.Email);
            if (!result.Succeeded)
                return ErrorJson(result.Errors);

            // The sign-in form matches against UserName, and registration seeds it
            // with the email. Without this the account would keep logging in under
            // the old address after an email change.
            result = await UserManager.SetUserNameAsync(user!, Input.Email);
            if (!result.Succeeded)
                return ErrorJson(result.Errors);

            result = await UserManager.SetPhoneNumberAsync(user!, Input.PhoneNumber);
            if (!result.Succeeded)
                return ErrorJson(result.Errors);

            return new JsonResult(new { success = true, email = Input.Email, phoneNumber = Input.PhoneNumber ?? "" });
        }

        /// <summary>
        /// Self-service only. Changing a password requires the current one, which an
        /// admin cannot supply for someone else's account, so this handler always
        /// resolves the caller and ignores any userId - the form is not rendered
        /// while managing another account.
        /// </summary>
        public async Task<IActionResult> OnPostEditPasswordAsync()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
                return ErrorJson("Benutzer nicht gefunden.");

            if (!ValidateOnly(PasswordInput, nameof(PasswordInput)))
                return ErrorJson(GetModelErrors().ToArray());

            var result = await UserManager.ChangePasswordAsync(
                user,
                PasswordInput.Password,
                PasswordInput.NewPassword);
            if (!result.Succeeded)
                return ErrorJson(result.Errors);

            return new JsonResult(new { success = true });
        }
    }
}
