using Business.Interfaces;
using Business.Models.Requests;
using Business.Models.Responses;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace StudentHousingAPI.Controllers;

/// <summary>
/// Authentication controller for JWT token management
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.Login(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Register a new student
    /// </summary>
    /// <param name="request">Student registration details</param>
    /// <returns>JWT token and student information</returns>
    [HttpPost("register-student")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterStudent([FromBody] StudentRegisterRequest request)
    {
        var response = await _authService.RegisterStudentAsync(request);
        return response.Success ? CreatedAtAction(nameof(Login), response) : BadRequest(response);
    }

    /// <summary>
    /// Register a new landlord
    /// </summary>
    /// <param name="request">Landlord registration details</param>
    /// <returns>JWT token and landlord information</returns>
    [HttpPost("register-landlord")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterLandLord([FromBody] LandLordRegisterRequest request)
    {
        var response = await _authService.RegisterLandLordAsync(request);
        return response.Success ? CreatedAtAction(nameof(Login), response) : BadRequest(response);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Token refresh request</param>
    /// <returns>New JWT token</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request);
        return response.Success ? Ok(response) : Unauthorized(response);
    }

    /// <summary>
    /// Logout user
    /// </summary>
    /// <param name="request">Logout request with token</param>
    /// <returns>Logout confirmation</returns>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var result = await _authService.LogoutAsync(request.Token);
        
        return Ok(new ApiResponse<string>
        {
            Success = result,
            Message = result ? "Logout successful" : "Logout failed"
        });
    }
}
