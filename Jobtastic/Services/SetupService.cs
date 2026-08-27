using Jobtastic.Identity;
using Jobtastic.Models;
using Microsoft.AspNetCore.Identity;

namespace Jobtastic.Services
{
    public class SetupService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager; 

        public SetupService(RoleManager<IdentityRole> roleManager,
                             UserManager<User> userManager) 
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task SeedRolesAsync()
        {
            string[] roles = RoleNames.All;

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        public async Task AssignRoleAsync(User user, string role)
        {
            if (!await _userManager.IsInRoleAsync(user, role))
                await _userManager.AddToRoleAsync(user, role);
        }
    }
}
