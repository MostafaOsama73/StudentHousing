using Business.Mappers;
using Business.Models.Settings;
using Business.Services;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        // Configure JWT Settings
        var jwtSettings = new JwtSettings();
        configuration.GetSection("JwtSettings").Bind(jwtSettings);
        services.AddSingleton(jwtSettings);

        // Add AutoMapper
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        // Add Authentication (Cookies + JWT for MVC)
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
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
        #endregion

        #region Services
        // Add Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        #endregion


    }

}
