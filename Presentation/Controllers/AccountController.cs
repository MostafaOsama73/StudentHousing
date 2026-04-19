using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

/// <summary>
/// Account controller for handling authentication views
/// Displays forms for login and registration
/// </summary>
public class AccountController : Controller
{
    /// <summary>
    /// Displays login form
    /// </summary>
    [HttpGet]
    public IActionResult Login()
    {
        return View();
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
    /// Displays landlord registration form
    /// </summary>
    [HttpGet]
    public IActionResult RegisterLandlord()
    {
        return View();
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
}
