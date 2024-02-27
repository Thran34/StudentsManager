using Microsoft.AspNetCore.Identity;
using StudentsManager.Models;

namespace StudentsManager.Data;

public static class RoleInitializer
{
    public static async Task InitializeAsync(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin")) await roleManager.CreateAsync(new IdentityRole("Admin"));
        if (!await roleManager.RoleExistsAsync("User")) await roleManager.CreateAsync(new IdentityRole("User"));

        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser == null)
        {
            var newUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newUser, "AdminPassword123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(newUser, "Admin");
        }
    }
}