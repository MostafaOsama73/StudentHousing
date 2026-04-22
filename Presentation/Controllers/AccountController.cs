using Business.Interfaces;
using Business.Models.Requests;
using Business.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

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
            return View(request);
        }

        try
        {
            var result = await _authService.Login(request);
            if (result.Success)
            {
                try
                {
                    // Store JWT token in session (with null checks)
                    if (result.Token != null)
                    {
                        HttpContext.Session.SetString("AccessToken", result.Token.AccessToken ?? string.Empty);
                        HttpContext.Session.SetString("RefreshToken", result.Token.RefreshToken ?? string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error storing token in session: {ex.Message}");
                    return View(request);
                }

                try
                {
                    if (result.User != null)
                    {
                        HttpContext.Session.SetString("User", System.Text.Json.JsonSerializer.Serialize(result.User));
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error storing user in session: {ex.Message}");
                    return View(request);
                }

                try
                {
                    // Sign in the user with ASP.NET Identity
                    if (!string.IsNullOrEmpty(request.Email))
                    {
                        var user = await _userManager.FindByEmailAsync(request.Email);
                        if (user != null)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            System.Diagnostics.Debug.WriteLine($"User signed in: {user.Email}");
                            
                            // Verify authentication worked
                            if (HttpContext.User.Identity != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"IsAuthenticated: {HttpContext.User.Identity.IsAuthenticated}");
                                System.Diagnostics.Debug.WriteLine($"UserName: {HttpContext.User.Identity.Name}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("HttpContext.User.Identity is null");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("User not found after login");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error signing in user: {ex.Message}");
                    return View(request);
                }

                try
                {
                    // Redirect based on role
                    var userRoles = result.User?.Roles?.ToList() ?? new List<string>();
                    
                    // Log the roles for debugging
                    System.Diagnostics.Debug.WriteLine($"User roles: {string.Join(", ", userRoles)}");
                    
                    if (userRoles.Count == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Your account has no assigned roles. Please contact the administrator.");
                        return View(request);
                    }
                    
                    // Check if user is authenticated before redirect
                    if (HttpContext.User?.Identity?.IsAuthenticated == true)
                    {
                        System.Diagnostics.Debug.WriteLine($"User is authenticated: {HttpContext.User.Identity.Name}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("User is NOT authenticated after SignInAsync - this is the problem!");
                    }
                    
                    if (userRoles.Contains("Admin"))
                    {
                        System.Diagnostics.Debug.WriteLine("Redirecting to Admin Dashboard");
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (userRoles.Contains("Student"))
                    {
                        System.Diagnostics.Debug.WriteLine("Redirecting to Student HousingUnits");
                        return returnUrl != null ? LocalRedirect(returnUrl) : RedirectToAction("HousingUnits", "Student");
                    }
                    else if (userRoles.Contains("LandLord"))
                    {
                        System.Diagnostics.Debug.WriteLine("Redirecting to Landlord PendingVerification");
                        return RedirectToAction("PendingVerification", "Account");
                    }
                    
                    System.Diagnostics.Debug.WriteLine("No role matched, redirecting to Home");
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error during redirect: {ex.Message}");
                    return View(request);
                }
            }
            ModelState.AddModelError(string.Empty, result.Message ?? "Login failed");
            return View(request);
        }
        catch (Exception ex)
        {
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
