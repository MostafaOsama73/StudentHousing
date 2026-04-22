using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Models;
using Shared.Enums;
using System;
using System.Threading.Tasks;

namespace Presentation.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly IStudentRepository _studentRepository;
    private readonly IHousingUnitRepository _housingUnitRepository;
    private readonly UserManager<User> _userManager;

    public StudentController(IStudentRepository studentRepository, IHousingUnitRepository housingUnitRepository, UserManager<User> userManager)
    {
        _studentRepository = studentRepository;
        _housingUnitRepository = housingUnitRepository;
        _userManager = userManager;
    }

    // GET: Student
    [HttpGet]
    public async Task<IActionResult> Index(
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null,
        string? sortBy = null,
        bool ascending = true)
    {
        var (students, totalCount) = await _studentRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            search,
            sortBy,
            ascending);

        var viewModel = new StudentIndexViewModel
        {
            Students = students,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Search = search,
            SortBy = sortBy,
            Ascending = ascending
        };

        return View(viewModel);
    }

    // GET: Student/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var student = await _studentRepository.GetAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // GET: Student/Profile
    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var student = await _studentRepository.GetAll()
            .FirstOrDefaultAsync(s => s.UserId == user.Id);

        if (student == null)
        {
            return NotFound();
        }

        var viewModel = new StudentProfileViewModel
        {
            StudentId = student.StudentId,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            Address = student.Address,
            City = student.City,
            PreferredArea = student.PreferredArea,
            NationalId = student.NationalId,
            ProfileImage = user.ProfileImage,
            Status = user.Status
        };

        return View(viewModel);
    }

    // POST: Student/Profile
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Profile(StudentProfileViewModel viewModel)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var student = await _studentRepository.GetAll()
            .FirstOrDefaultAsync(s => s.UserId == user.Id);

        if (student == null)
        {
            return NotFound();
        }

        // Check if Educational Data is being edited (NationalId, PreferredArea, etc.)
        bool educationalDataChanged = 
            student.NationalId != viewModel.NationalId ||
            student.PreferredArea != viewModel.PreferredArea ||
            student.City != viewModel.City;

        // Check if Email is being changed
        bool emailChanged = user.Email != viewModel.Email;

        // Update user personal data (always allowed)
        user.UserName = viewModel.UserName;
        user.Email = viewModel.Email;
        user.PhoneNumber = viewModel.PhoneNumber;
        user.ProfileImage = viewModel.ProfileImage;

        // Update student data
        student.DateOfBirth = viewModel.DateOfBirth;
        student.Gender = viewModel.Gender;
        student.Address = viewModel.Address;
        student.City = viewModel.City;
        student.PreferredArea = viewModel.PreferredArea;
        student.NationalId = viewModel.NationalId;

        // If Educational Data changed, set status to Pending
        if (educationalDataChanged)
        {
            user.Status = UserStatus.Pending;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            ModelState.AddModelError(string.Empty, $"Failed to update profile: {errors}");
            return View(viewModel);
        }

        await _studentRepository.Update(student);
        await _studentRepository.CommitAsync();

        if (educationalDataChanged)
        {
            TempData["Message"] = "Profile updated. Since you changed educational data, your account is now pending approval.";
        }
        else
        {
            TempData["Message"] = "Profile updated successfully.";
        }

        return RedirectToAction(nameof(Profile));
    }

    // GET: Student/HousingUnits
    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> HousingUnits(
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null,
        string? city = null,
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        var (housingUnits, totalCount) = await _housingUnitRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            search,
            city,
            minPrice,
            maxPrice);

        var viewModel = new StudentHousingUnitViewModel
        {
            HousingUnits = housingUnits,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Search = search,
            City = city,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        };

        return View(viewModel);
    }

    // GET: Student/HousingUnitDetails/5
    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> HousingUnitDetails(Guid id)
    {
        var housingUnit = await _housingUnitRepository.GetAsync(id);
        if (housingUnit == null)
        {
            return NotFound();
        }

        return View(housingUnit);
    }

    // GET: Student/BookHousingUnit/5
    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> BookHousingUnit(Guid id)
    {
        var housingUnit = await _housingUnitRepository.GetAsync(id);
        if (housingUnit == null)
        {
            return NotFound();
        }

        var viewModel = new BookingViewModel
        {
            HousingUnitId = housingUnit.HousingUnitId,
            HousingUnit = housingUnit
        };

        return View(viewModel);
    }

    // POST: Student/BookHousingUnit
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> BookHousingUnit(BookingViewModel viewModel)
    {
        // Check if user's account is approved
        var user = await _userManager.GetUserAsync(User);
        if (user == null || user.Status != UserStatus.Approved)
        {
            ModelState.AddModelError(string.Empty, "Your account is still pending approval. You cannot book housing units until your account is approved.");
            return View(viewModel);
        }

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        // TODO: Implement booking logic
        // This would create a Booking entity and save it to the database
        // For now, we'll just redirect to the HousingUnits list

        return RedirectToAction(nameof(HousingUnits));
    }
}
