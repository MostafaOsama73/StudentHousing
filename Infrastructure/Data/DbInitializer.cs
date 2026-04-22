using Domain.Entities;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Enums;

namespace Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<StudentHousingDBContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed roles
            await SeedRolesAsync(roleManager);

            // Seed admin user
            await SeedAdminUserAsync(userManager, roleManager);
        }
        catch (Exception ex)
        {
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DbInitializer");
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Admin", "Student", "LandLord" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Check if admin user already exists
        var adminUser = await userManager.FindByEmailAsync("admin@studenthousing.com");

        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = "admin@studenthousing.com",
                Email = "admin@studenthousing.com",
                PhoneNumber = "+1234567890",
                IsActive = true,
                IsDeleted = false,
                Status = UserStatus.Approved
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");

            if (result.Succeeded)
            {
                // Assign Admin role
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ApplicationException($"Failed to create admin user: {errors}");
            }
        }
        else if (adminUser.Status != UserStatus.Approved)
        {
            // Update status if admin user exists but is not approved
            adminUser.Status = UserStatus.Approved;
            await userManager.UpdateAsync(adminUser);
        }
    }
}
