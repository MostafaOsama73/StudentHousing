using Business.Interfaces;
using Business.Models.Requests;
using Business.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Shared.Enums;

namespace Presentation.Controllers;

/// <summary>
/// Account controller for handling authentication views
/// Displays forms for login and registration
/// </summary>
public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(IAuthService authService, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _authService = authService;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    /// <summary>
    /// Displays login form
    /// </summary>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Processes login form submission
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest request, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            System.Diagnostics.Debug.WriteLine($"ModelState invalid: {string.Join(", ", errors)}");
            return View(request);
        }

        try
        {
            System.Diagnostics.Debug.WriteLine($"Login attempt for email: {request.Email}");

            // Sign in the user with ASP.NET Identity using password
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                System.Diagnostics.Debug.WriteLine("User not found");
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(request);
            }

            System.Diagnostics.Debug.WriteLine($"User found: {user.Email}, ID: {user.Id}, Status: {user.Status}");

            if (user.IsDeleted)
            {
                System.Diagnostics.Debug.WriteLine("User is deleted");
                ModelState.AddModelError(string.Empty, "Your account has been deleted.");
                return View(request);
            }

            if (user.Status == UserStatus.Rejected)
            {
                System.Diagnostics.Debug.WriteLine("User status is Rejected");
                ModelState.AddModelError(string.Empty, "Your account has been rejected. Please contact administrator.");
                return View(request);
            }

            var signInResult = await _signInManager.PasswordSignInAsync(
                user,
                request.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            System.Diagnostics.Debug.WriteLine($"SignIn result: {signInResult}, Succeeded: {signInResult.Succeeded}, IsLockedOut: {signInResult.IsLockedOut}, RequiresTwoFactor: {signInResult.RequiresTwoFactor}");

            if (signInResult.Succeeded)
            {
                System.Diagnostics.Debug.WriteLine($"User signed in successfully: {user.Email}");

                try
                {
                    // Get user roles from Identity
                    var userRoles = await _userManager.GetRolesAsync(user);

                    // Log the roles for debugging
                    System.Diagnostics.Debug.WriteLine($"User roles: {string.Join(", ", userRoles)}");
                    System.Diagnostics.Debug.WriteLine($"User status: {user.Status}");

                    if (userRoles.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("User has no roles");
                        ModelState.AddModelError(string.Empty, "Your account has no assigned roles. Please contact the administrator.");
                        await _signInManager.SignOutAsync();
                        return View(request);
                    }

                    // Check approval status for non-admin users
                    if (user.Status != UserStatus.Approved && !userRoles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
                    {
                        System.Diagnostics.Debug.WriteLine($"User is not approved. Status: {user.Status}");
                        if (user.Status == UserStatus.Pending)
                        {
                            System.Diagnostics.Debug.WriteLine("Redirecting to PendingVerification page");
                            return RedirectToAction("PendingVerification", "Account");
                        }
                        else if (user.Status == UserStatus.Rejected)
                        {
                            System.Diagnostics.Debug.WriteLine("User is rejected");
                            await _signInManager.SignOutAsync();
                            ModelState.AddModelError(string.Empty, "Your account has been rejected. Please contact the administrator.");
                            return View(request);
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Checking for Admin role in: {string.Join(", ", userRoles)}");
                    if (userRoles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
                    {
                        System.Diagnostics.Debug.WriteLine("Redirecting to Admin Dashboard");
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (userRoles.Any(r => r.Equals("Student", StringComparison.OrdinalIgnoreCase)))
                    {
                        System.Diagnostics.Debug.WriteLine("Redirecting to Student HousingUnits");
                        return returnUrl != null ? LocalRedirect(returnUrl) : RedirectToAction("HousingUnits", "Student");
                    }
                    else if (userRoles.Any(r => r.Equals("LandLord", StringComparison.OrdinalIgnoreCase)))
                    {
                        System.Diagnostics.Debug.WriteLine("Redirecting to Landlord Profile");
                        return RedirectToAction("Profile", "LandLord");
                    }

                    System.Diagnostics.Debug.WriteLine("No role matched, redirecting to Home");
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error during redirect: {ex.Message}");
                    ModelState.AddModelError(string.Empty, $"Error during redirect: {ex.Message}");
                    await _signInManager.SignOutAsync();
                    return View(request);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("SignIn failed");
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(request);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            ModelState.AddModelError(string.Empty, $"Login error: {ex.Message}");
            return View(request);
        }
    }

    /// <summary>
    /// Displays registration choice page
    /// </summary>
    [HttpGet]
    public IActionResult RegisterChoice()
    {
        return View();
    }

    /// <summary>
    /// Displays student registration form
    /// </summary>
    [HttpGet]
    public IActionResult RegisterStudent()
    {
        return View();
    }

    /// <summary>
    /// Processes student registration
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterStudent(StudentRegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var result = await _authService.RegisterStudentAsync(request);
            if (result.Success)
            {
                // Store JWT token in session (with null checks)
                if (result.Token != null)
                {
                    HttpContext.Session.SetString("AccessToken", result.Token.AccessToken ?? string.Empty);
                    HttpContext.Session.SetString("RefreshToken", result.Token.RefreshToken ?? string.Empty);
                }
                if (result.User != null)
                {
                    HttpContext.Session.SetString("User", System.Text.Json.JsonSerializer.Serialize(result.User));
                }

                // Sign in the user with ASP.NET Identity
                if (!string.IsNullOrEmpty(request.Email))
                {
                    var user = await _userManager.FindByEmailAsync(request.Email);
                    if (user != null)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                }

                return RedirectToAction("HousingUnits", "Student");
            }
            ModelState.AddModelError(string.Empty, result.Message ?? "Registration failed");
            return View(request);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }

    /// <summary>
    /// Displays landlord registration form
    /// </summary>
    [HttpGet]
    public IActionResult RegisterLandlord()
    {
        return View();
    }

    /// <summary>
    /// Processes landlord registration
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterLandlord(LandLordRegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var result = await _authService.RegisterLandLordAsync(request);
            if (result.Success)
            {
                // Store JWT token in session (with null checks)
                if (result.Token != null)
                {
                    HttpContext.Session.SetString("AccessToken", result.Token.AccessToken ?? string.Empty);
                    HttpContext.Session.SetString("RefreshToken", result.Token.RefreshToken ?? string.Empty);
                }
                if (result.User != null)
                {
                    HttpContext.Session.SetString("User", System.Text.Json.JsonSerializer.Serialize(result.User));
                }

                // Sign in the user with ASP.NET Identity
                if (!string.IsNullOrEmpty(request.Email))
                {
                    var user = await _userManager.FindByEmailAsync(request.Email);
                    if (user != null)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                }

                return RedirectToAction("PendingVerification", "Account");
            }
            ModelState.AddModelError(string.Empty, result.Message ?? "Registration failed");
            return View(request);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }

    /// <summary>
    /// Displays admin registration form
    /// </summary>
    [HttpGet]
    public IActionResult RegisterAdmin()
    {
        return View();
    }

    /// <summary>
    /// Processes admin registration
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterAdmin(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var result = await _authService.RegisterAdminAsync(request);
            if (result.Success)
            {
                // Sign in the user with ASP.NET Identity
                if (!string.IsNullOrEmpty(request.Email))
                {
                    var user = await _userManager.FindByEmailAsync(request.Email);
                    if (user != null)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                }

                return RedirectToAction("Dashboard", "Admin");
            }
            ModelState.AddModelError(string.Empty, result.Message ?? "Registration failed");
            return View(request);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }

    /// <summary>
    /// Displays access denied page
    /// </summary>
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    /// <summary>
    /// Displays pending verification page for landlords
    /// </summary>
    [HttpGet]
    public IActionResult PendingVerification()
    {
        return View();
    }

    /// <summary>
    /// Processes logout request
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
