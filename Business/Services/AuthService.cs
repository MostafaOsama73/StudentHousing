using Business.Interfaces;
using Business.Models.Helpers;
using Business.Models.Requests;
using Business.Models.Responses;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Shared.Enums;

namespace Business.Services;

/// <summary>
/// Service for handling authentication operations
/// </summary>
public class AuthService : IAuthService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ILandLordRepository _landLordRepository;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthService> _logger;
    private readonly IMapper _mapper;

    public AuthService(
        IStudentRepository studentRepository,
        ILandLordRepository landLordRepository,
        ITokenService tokenService,
        UserManager<User> userManager,
        ILogger<AuthService> logger,
        IMapper mapper)
    {
        _studentRepository = studentRepository;
        _landLordRepository = landLordRepository;
        _tokenService = tokenService;
        _userManager = userManager;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Authenticates user with email and password
    /// </summary>
    public async Task<AuthResponse> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new ApplicationException(ErrorMessageHelper.InvalidCredentials);
    
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new ApplicationException(ErrorMessageHelper.UserNotFound);

        if (user.IsDeleted)
            throw new ApplicationException(ErrorMessageHelper.UserIsDeleted);

        if (user.Status == UserStatus.Rejected)
            throw new ApplicationException("Your account has been rejected. Please contact administrator.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid)                           
            throw new ApplicationException(ErrorMessageHelper.InvalidCredentials);
        
        var roles = await _userManager.GetRolesAsync(user);

        Student? student = null;
        LandLord? landlord = null;

        if (roles.Contains("Student"))
        {
            student = _studentRepository.GetAll(asNoTracking: true)
                .FirstOrDefault(s => s.UserId == user.Id);
        }

        if (roles.Contains("LandLord"))
        {
            landlord = _landLordRepository.GetAll(asNoTracking: true)
                .FirstOrDefault(l => l.UserId == user.Id);
        }

        var accessToken = await _tokenService.GenerateAccessTokenAsync(user, student, landlord);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

        _logger.LogInformation($"User logged in successfully: {user.Email}");

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            Token = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 18000 // 5 hours in seconds
            },
            User = new UserResponse
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToArray(),
                StudentId = student?.StudentId,
                LandLordId = landlord?.LandLordId
            }
        };
    }

    /// <summary>
    /// Registers a new student
    /// </summary>
    public async Task<AuthResponse> RegisterStudentAsync(StudentRegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ApplicationException(ErrorMessageHelper.InvalidCredentials);

        if (request.Password != request.ConfirmPassword)
            throw new ApplicationException("Passwords do not match");

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            throw new ApplicationException(ErrorMessageHelper.UserAlreadyExists);

        var user = new User
        {
            UserName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsDeleted = false,
            IsActive = true,  // Students are active by default
            Status = UserStatus.Pending,  // Students need approval by default
            ProfileImage = request.ProfileImage  // Set profile image
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new ApplicationException($"User creation failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, "Student");

        // Map request to Student entity using AutoMapper
        var student = _mapper.Map<Student>(request);
        student.StudentId = Guid.NewGuid();
        student.UserId = user.Id;

        await _studentRepository.Insert(student);
        await _studentRepository.CommitAsync();

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = await _tokenService.GenerateAccessTokenAsync(user, student);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

        _logger.LogInformation($"Student registered successfully: {user.Email}");

        return new AuthResponse
        {
            Success = true,
            Message = "Student registered successfully",
            Token = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 18000 // 5 hours in seconds
            },
            User = new UserResponse
            {
                Id = user.Id,
                FullName = request.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToArray(),
                StudentId = student.StudentId
            }
        };
    }

    /// <summary>
    /// Registers a new landlord
    /// </summary>
    public async Task<AuthResponse> RegisterLandLordAsync(LandLordRegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ApplicationException(ErrorMessageHelper.InvalidCredentials);

        if (request.Password != request.ConfirmPassword)
            throw new ApplicationException("Passwords do not match");

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            throw new ApplicationException(ErrorMessageHelper.UserAlreadyExists);

        var user = new User
        {
            UserName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsDeleted = false,
            IsActive = false,  // Landlords are inactive by default, must be activated by admin
            Status = UserStatus.Pending,  // Landlords need approval by default
            ProfileImage = request.ProfileImage  // Set profile image
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new ApplicationException($"User creation failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, "LandLord");

        // Map request to LandLord entity using AutoMapper
        var landlord = _mapper.Map<LandLord>(request);
        landlord.LandLordId = Guid.NewGuid();
        landlord.UserId = user.Id;
        landlord.VerificationStatus = "Pending";

        await _landLordRepository.Insert(landlord);
        await _landLordRepository.CommitAsync();

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = await _tokenService.GenerateAccessTokenAsync(user, landlord: landlord);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

        _logger.LogInformation($"LandLord registered successfully: {user.Email}");

        return new AuthResponse
        {
            Success = true,
            Message = "LandLord registered successfully. Pending verification.",
            Token = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 18000 // 5 hours in seconds
            },
            User = new UserResponse
            {
                Id = user.Id,
                FullName = request.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToArray(),
                LandLordId = landlord.LandLordId
            }
        };
    }

    /// <summary>
    /// Registers a new admin
    /// </summary>
    public async Task<AuthResponse> RegisterAdminAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ApplicationException(ErrorMessageHelper.InvalidCredentials);

        if (request.Password != request.ConfirmPassword)
            throw new ApplicationException("Passwords do not match");

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            throw new ApplicationException(ErrorMessageHelper.UserAlreadyExists);

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsDeleted = false,
            IsActive = true,  // Admins are active by default
            Status = UserStatus.Approved  // Admins are auto-approved
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new ApplicationException($"User creation failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, "Admin");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

        _logger.LogInformation($"Admin registered successfully: {user.Email}");

        return new AuthResponse
        {
            Success = true,
            Message = "Admin registered successfully",
            Token = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 18000 // 5 hours in seconds
            },
            User = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToArray()
            }
        };
    }

    /// <summary>
    /// Refreshes expired access token using refresh token
    /// </summary>
    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
            throw new ApplicationException("Token is required");

        var principal = await _tokenService.GetPrincipalFromExpiredTokenAsync(request.Token);
        if (principal == null)
            throw new ApplicationException("Invalid token");

        var userIdClaim = principal.FindFirst("sub") ?? principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        if (userIdClaim == null)
            throw new ApplicationException("Invalid token claims");

        var user = await _userManager.FindByIdAsync(userIdClaim.Value);
        if (user == null)
            throw new ApplicationException(ErrorMessageHelper.UserNotFound);

        if (user.IsDeleted)
            throw new ApplicationException(ErrorMessageHelper.UserIsDeleted);

        if (!user.IsActive)
            throw new ApplicationException("Your account is not activated yet. Please contact administrator.");

        var roles = await _userManager.GetRolesAsync(user);
        
        Student? student = null;
        LandLord? landlord = null;

        if (roles.Contains("Student"))
        {
            student = _studentRepository.GetAll(asNoTracking: true)
                .FirstOrDefault(s => s.UserId == user.Id);
        }

        if (roles.Contains("LandLord"))
        {
            landlord = _landLordRepository.GetAll(asNoTracking: true)
                .FirstOrDefault(l => l.UserId == user.Id);
        }

        var newAccessToken = await _tokenService.GenerateAccessTokenAsync(user, student, landlord);
        var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();

        _logger.LogInformation($"Token refreshed successfully for user: {user.Email}");

        return new AuthResponse
        {
            Success = true,
            Message = "Token refreshed successfully",
            Token = new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = 18000 // 5 hours in seconds
            }
        };
    }

    /// <summary>
    /// Logs out user
    /// </summary>
    public async Task<bool> LogoutAsync(string token)
    {
        _logger.LogInformation($"User logged out");
        return true;
    }

    /// <summary>
    /// Approves a user account
    /// </summary>
    public async Task<bool> ApproveUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"User not found: {userId}");
            return false;
        }

        user.Status = UserStatus.Approved;
        var result = await _userManager.UpdateAsync(user);
        
        if (result.Succeeded)
        {
            _logger.LogInformation($"User account approved: {user.Email}");
            return true;
        }

        _logger.LogWarning($"Failed to approve user account: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        return false;
    }

    /// <summary>
    /// Rejects a user account
    /// </summary>
    public async Task<bool> RejectUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"User not found: {userId}");
            return false;
        }

        user.Status = UserStatus.Rejected;
        var result = await _userManager.UpdateAsync(user);
        
        if (result.Succeeded)
        {
            _logger.LogInformation($"User account rejected: {user.Email}");
            return true;
        }

        _logger.LogWarning($"Failed to reject user account: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        return false;
    }
}
