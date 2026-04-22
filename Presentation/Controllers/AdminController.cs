using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Presentation.Models;

namespace Presentation.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IStudentRepository _studentRepository;
    private readonly ILandLordRepository _landLordRepository;
    private readonly UserManager<User> _userManager;

    public AdminController(
        IStudentRepository studentRepository,
        ILandLordRepository landLordRepository,
        UserManager<User> userManager)
    {
        _studentRepository = studentRepository;
        _landLordRepository = landLordRepository;
        _userManager = userManager;
    }

    // GET: Admin/Dashboard
    public IActionResult Dashboard()
    {
        return View();
    }

    // GET: Admin/Profile
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var viewModel = new AdminProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfileImage = user.ProfileImage,
            Status = user.Status
        };

        return View(viewModel);
    }

    // POST: Admin/Profile
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Profile(AdminProfileViewModel viewModel)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Admin can edit everything without restrictions
        user.UserName = viewModel.UserName;
        user.Email = viewModel.Email;
        user.PhoneNumber = viewModel.PhoneNumber;
        user.ProfileImage = viewModel.ProfileImage;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            ModelState.AddModelError(string.Empty, $"Failed to update profile: {errors}");
            return View(viewModel);
        }

        TempData["Message"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    #region Student Management

    // GET: Admin/Students
    public async Task<IActionResult> Students(
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null)
    {
        var (students, totalCount) = await _studentRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            search);

        // Load User navigation properties for each student to access Status and Email
        foreach (var student in students)
        {
            if (!string.IsNullOrEmpty(student.UserId))
            {
                student.User = await _userManager.FindByIdAsync(student.UserId);
            }
        }

        var viewModel = new AdminStudentViewModel
        {
            Students = students,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Search = search
        };

        return View(viewModel);
    }

    // GET: Admin/StudentDetails/5
    public async Task<IActionResult> StudentDetails(Guid id)
    {
        var student = await _studentRepository.GetAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // GET: Admin/DeleteStudent/5
    public async Task<IActionResult> DeleteStudent(Guid id)
    {
        var student = await _studentRepository.GetAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // POST: Admin/DeleteStudent/5
    [HttpPost, ActionName("DeleteStudent")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStudentConfirmed(Guid id)
    {
        var student = await _studentRepository.GetAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        await _studentRepository.Delete(student);
        await _studentRepository.CommitAsync();
        return RedirectToAction(nameof(Students));
    }

    // GET: Admin/ActivateStudent/5
    public async Task<IActionResult> ActivateStudent(Guid id)
    {
        var student = await _studentRepository.GetAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(student.UserId);
        if (user != null)
        {
            user.IsActive = true;
            await _userManager.UpdateAsync(user);
        }

        return RedirectToAction(nameof(Students));
    }

    // GET: Admin/DeactivateStudent/5
    public async Task<IActionResult> DeactivateStudent(Guid id)
    {
        var student = await _studentRepository.GetAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(student.UserId);
        if (user != null)
        {
            user.IsActive = false;
            await _userManager.UpdateAsync(user);
        }

        return RedirectToAction(nameof(Students));
    }

    // GET: Admin/ApproveStudent/5
    public async Task<IActionResult> ApproveStudent(Guid id)
    {
        var student = await _studentRepository.GetAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(student.UserId);
        if (user != null)
        {
            user.Status = Shared.Enums.UserStatus.Approved;
            await _userManager.UpdateAsync(user);
        }

        return RedirectToAction(nameof(Students));
    }

    // GET: Admin/RejectStudent/5
    public async Task<IActionResult> RejectStudent(Guid id)
    {
        var student = await _studentRepository.GetAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(student.UserId);
        if (user != null)
        {
            user.Status = Shared.Enums.UserStatus.Rejected;
            await _userManager.UpdateAsync(user);
        }

        return RedirectToAction(nameof(Students));
    }

    #endregion

    #region Landlord Management

    // GET: Admin/Landlords
    public async Task<IActionResult> Landlords(
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null)
    {
        var landlords = _landLordRepository.GetAll(asNoTracking: true);

        // Apply search filter if provided
        if (!string.IsNullOrEmpty(search))
        {
            landlords = landlords.Where(l =>
                l.CompanyName.Contains(search) ||
                l.NationalId.Contains(search));
        }

        var totalCount = landlords.Count();

        // Apply pagination
        landlords = landlords.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        // Load User navigation properties for each landlord to access Status and Email
        foreach (var landlord in landlords)
        {
            if (!string.IsNullOrEmpty(landlord.UserId))
            {
                landlord.User = await _userManager.FindByIdAsync(landlord.UserId);
            }
        }

        var viewModel = new AdminLandlordViewModel
        {
            Landlords = landlords,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Search = search
        };

        return View(viewModel);
    }

    // GET: Admin/LandlordDetails/5
    public async Task<IActionResult> LandlordDetails(Guid id)
    {
        var landlord = await _landLordRepository.GetAsync(id);
        if (landlord == null)
        {
            return NotFound();
        }

        return View(landlord);
    }

    // GET: Admin/ApproveLandlord/5
    public async Task<IActionResult> ApproveLandlord(Guid id)
    {
        var landlord = await _landLordRepository.GetAsync(id);
        if (landlord == null)
        {
            return NotFound();
        }

        landlord.VerificationStatus = "Approved";
        var user = await _userManager.FindByIdAsync(landlord.UserId);
        if (user != null)
        {
            user.Status = Shared.Enums.UserStatus.Approved;
            user.IsActive = true;
            await _userManager.UpdateAsync(user);
        }

        await _landLordRepository.Update(landlord);
        await _landLordRepository.CommitAsync();

        return RedirectToAction(nameof(Landlords));
    }

    // GET: Admin/RejectLandlord/5
    public async Task<IActionResult> RejectLandlord(Guid id)
    {
        var landlord = await _landLordRepository.GetAsync(id);
        if (landlord == null)
        {
            return NotFound();
        }

        landlord.VerificationStatus = "Rejected";
        var user = await _userManager.FindByIdAsync(landlord.UserId);
        if (user != null)
        {
            user.Status = Shared.Enums.UserStatus.Rejected;
            user.IsActive = false;
            await _userManager.UpdateAsync(user);
        }

        await _landLordRepository.Update(landlord);
        await _landLordRepository.CommitAsync();

        return RedirectToAction(nameof(Landlords));
    }

    // GET: Admin/DeleteLandlord/5
    public async Task<IActionResult> DeleteLandlord(Guid id)
    {
        var landlord = await _landLordRepository.GetAsync(id);
        if (landlord == null)
        {
            return NotFound();
        }

        return View(landlord);
    }

    // POST: Admin/DeleteLandlord/5
    [HttpPost, ActionName("DeleteLandlord")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLandlordConfirmed(Guid id)
    {
        var landlord = await _landLordRepository.GetAsync(id);
        if (landlord == null)
        {
            return NotFound();
        }

        await _landLordRepository.Delete(landlord);
        await _landLordRepository.CommitAsync();
        return RedirectToAction(nameof(Landlords));
    }

    #endregion
}
