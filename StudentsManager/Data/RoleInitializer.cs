using Microsoft.AspNetCore.Identity;
using StudentsManager.Models;

namespace StudentsManager.Data;

public static class RoleInitializer
{
    public static async Task InitializeAsync(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "TEACHER", "STUDENT", "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist) await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var adminUser = await userManager.FindByEmailAsync("admin@admin.com");
        if (adminUser == null)
        {
            var newUser = new ApplicationUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                FirstName = "admin",
                LastName = "admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newUser, "AdminPassword123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(newUser, "Admin");
        }
    }
}