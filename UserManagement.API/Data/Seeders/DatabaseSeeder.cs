using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using UserManagement.API.Models.Entities;
using UserManagement.API.Models.Enums;

namespace UserManagement.API.Data.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Seed Roles
            string[] defaultRoles = { "SuperAdmin", "Admin", "User" };
            foreach (var roleName in defaultRoles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var description = roleName switch
                    {
                        "SuperAdmin" => "Full system access bypassing permission checks.",
                        "Admin" => "Administrative user with configurable permissions.",
                        "User" => "Standard application user.",
                        _ => null
                    };

                    await roleManager.CreateAsync(new ApplicationRole(roleName, description));
                }
            }

            // 2. Assign Permission Claims to "Admin" role
            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole != null)
            {
                var existingClaims = await roleManager.GetClaimsAsync(adminRole);
                var allPermissions = SystemPermissions.GetAllPermissions();

                foreach (var permission in allPermissions)
                {
                    if (!existingClaims.Any(c => c.Type == "permission" && c.Value == permission))
                    {
                        await roleManager.AddClaimAsync(adminRole, new Claim("permission", permission));
                    }
                }
            }

            // 3. Seed SuperAdmin User
            var superAdminEmail = "admin@example.com";
            var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
            if (superAdminUser == null)
            {
                superAdminUser = new ApplicationUser
                {
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(superAdminUser, "Admin@123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
                }
            }

            // 4. Seed Standard Admin User
            var adminEmail = "manager@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Manager",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 5. Seed Standard User
            var standardEmail = "user@example.com";
            var standardUser = await userManager.FindByEmailAsync(standardEmail);
            if (standardUser == null)
            {
                standardUser = new ApplicationUser
                {
                    UserName = standardEmail,
                    Email = standardEmail,
                    FirstName = "Standard",
                    LastName = "User",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(standardUser, "User@123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(standardUser, "User");
                }
            }
        }
    }
}
