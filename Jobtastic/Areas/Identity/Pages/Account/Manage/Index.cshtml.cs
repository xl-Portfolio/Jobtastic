// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Jobtastic.Models;

namespace Jobtastic.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        [BindProperty]
        public InputModel Input {  get; set; }

        [BindProperty]
        public PasswordInputModel PasswordInput { get; set; }
        
        public IndexModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public class InputModel
        {
            public string Email { get; set; }
            public string? PhoneNumber { get; set; }
        }
        public class PasswordInputModel
        {
            [Required]
            public string Password { get; set; }
            [Required]
            public string NewPassword { get; set; }
            [Required]
            [Compare("NewPassword")]
            public string ConfirmedPassword { get; set; }
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            Input = new InputModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
            PasswordInput = new PasswordInputModel
            {

            };

            return Page();
        }
        public async Task<IActionResult> OnPostEditDataAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var result = await _userManager.SetEmailAsync(user, Input.Email);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            result = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Page();
        }
        public async Task<IActionResult> OnPostEditPasswordAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _userManager.ChangePasswordAsync(
                user,
                PasswordInput.Password,
                PasswordInput.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Page();
        }
    }
}
