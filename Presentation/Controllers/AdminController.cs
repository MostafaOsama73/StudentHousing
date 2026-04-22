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
        user.PhoneNumber = viewModel.PhoneNumber;
        user.ProfileImage = viewModel.ProfileImage;

        await _userManager.UpdateAsync(user);

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

    #endregion

    #region Landlord Management

    // GET: Admin/Landlords
    public async Task<IActionResult> Landlords(
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null)
    {
        var landlords = _landLordRepository.GetAll(asNoTracking: true);
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            landlords = landlords.Where(l =>
                l.CompanyName.Contains(search) ||
                l.NationalId.Contains(search));
        }

        var totalCount = landlords.Count();
        var pagedLandlords = landlords
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var viewModel = new AdminLandlordViewModel
        {
            Landlords = pagedLandlords,
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
