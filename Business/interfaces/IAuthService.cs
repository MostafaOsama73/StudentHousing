using Business.Models.Requests;
using Business.Models.Responses;

namespace Business.Interfaces;

/// <summary>
/// Interface for authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates user and returns JWT token
    /// </summary>
    Task<AuthResponse> Login(LoginRequest request);

    /// <summary>
    /// Registers a new student user
    /// </summary>
    Task<AuthResponse> RegisterStudentAsync(StudentRegisterRequest request);

    /// <summary>
    /// Registers a new landlord user
    /// </summary>
    Task<AuthResponse> RegisterLandLordAsync(LandLordRegisterRequest request);

    /// <summary>
    /// Registers a new admin user
    /// </summary>
    Task<AuthResponse> RegisterAdminAsync(RegisterRequest request);

    /// <summary>
    /// Refreshes expired access token
    /// </summary>
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>
    /// Logs out user (invalidates token)
    /// </summary>
    Task<bool> LogoutAsync(string token);
}
