using Domain.Entities;

namespace Business.Interfaces;

/// <summary>
/// Interface for JWT token generation and validation
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the user
    /// </summary>
    Task<string> GenerateAccessTokenAsync(User user, Student? student = null, LandLord? landlord = null);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    Task<string> GenerateRefreshTokenAsync();

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// Gets principal from expired token
    /// </summary>
    Task<System.Security.Claims.ClaimsPrincipal?> GetPrincipalFromExpiredTokenAsync(string token);
}
