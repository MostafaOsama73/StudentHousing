using Business.Interfaces;
using Business.Mappers;
using Business.Models.Settings;
using Business.Services;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Presentation.Helpers;

public class ContainerConfiguration
{
    public static void ConfigureServices(IServiceCollection services , IConfiguration configuration)
    {
        #region JWT Settings
        // Add Identity
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<StudentHousingDBContext>()
        .AddDefaultTokenProviders();

        // JwtSettings is still needed by TokenService even though we don't use JWT auth here
        var jwtSettings = new JwtSettings();
        configuration.GetSection("JwtSettings").Bind(jwtSettings);
        services.AddSingleton(jwtSettings);

        // Add AutoMapper
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        // Configure the Identity Cookie paths
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

        // Add Authorization
        services.AddAuthorization(options =>
        {
            options.AddPolicy("StudentOnly", policy =>
                policy.RequireRole("Student"));

            options.AddPolicy("LandLordOnly", policy =>
                policy.RequireRole("LandLord"));

            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));
        });
        #endregion

        #region Repositories
        // Add Repositories
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ILandLordRepository, LandLordRepository>();
        services.AddScoped<IHousingUnitRepository, HousingUnitRepository>();
        #endregion

        #region Services
        // Add Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        #endregion


    }

}
