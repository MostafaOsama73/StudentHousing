using Business.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Business.Models.Responses;
using Business.Models.Settings;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Business.Services;

/// <summary>
/// Service for JWT token generation and validation
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<TokenService> _logger;
    private readonly SymmetricSecurityKey _securityKey;

    public TokenService(JwtSettings jwtSettings, ILogger<TokenService> logger)
    {
        _jwtSettings = jwtSettings;
        _logger = logger;
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
    }

    /// <summary>
    /// Generates JWT access token with user claims
    /// </summary>
    public async Task<string> GenerateAccessTokenAsync(User user, Student? student = null, LandLord? landlord = null)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim("PhoneNumber", user.PhoneNumber ?? string.Empty)
            };

            // Add role claims
            if (student != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Student"));
                claims.Add(new Claim("StudentId", student.StudentId.ToString()));
            }

            if (landlord != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, "LandLord"));
                claims.Add(new Claim("LandLordId", landlord.LandLordId.ToString()));
            }

            var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.WriteToken(token);

            _logger.LogInformation($"Access token generated for user: {user.Id}");
            return accessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating access token: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Generates a refresh token (random string)
    /// </summary>
    public async Task<string> GenerateRefreshTokenAsync()
    {
        var randomNumber = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    /// <summary>
    /// Validates JWT token signature and expiration
    /// </summary>
    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _securityKey,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Token validation failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets ClaimsPrincipal from an expired token (for refresh token flow)
    /// </summary>
    public async Task<ClaimsPrincipal?> GetPrincipalFromExpiredTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _securityKey,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false, // We want to get principal even if expired
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting principal from expired token: {ex.Message}");
            return null;
        }
    }
}
