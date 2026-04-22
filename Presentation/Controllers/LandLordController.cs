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

[Authorize(Roles = "LandLord")]
public class LandLordController : Controller
{
    private readonly ILandLordRepository _landLordRepository;
    private readonly IHousingUnitRepository _housingUnitRepository;
    private readonly UserManager<User> _userManager;

    public LandLordController(
        ILandLordRepository landLordRepository,
        IHousingUnitRepository housingUnitRepository,
        UserManager<User> userManager)
    {
        _landLordRepository = landLordRepository;
        _housingUnitRepository = housingUnitRepository;
        _userManager = userManager;
    }

    // GET: LandLord/Profile
    [HttpGet]
    [Authorize(Roles = "LandLord")]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var landlord = await _landLordRepository.GetAll()
            .FirstOrDefaultAsync(l => l.UserId == user.Id);

        if (landlord == null)
        {
            return NotFound();
        }

        var viewModel = new LandLordProfileViewModel
        {
            LandLordId = landlord.LandLordId,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfileImage = user.ProfileImage,
            Status = user.Status,
            // Educational/verification data
            CompanyName = landlord.CompanyName,
            NationalId = landlord.NationalId,
            PropertyOwnerShipProof = landlord.PropertyOwnerShipProof
        };

        return View(viewModel);
    }

    // POST: LandLord/Profile
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "LandLord")]
    public async Task<IActionResult> Profile(LandLordProfileViewModel viewModel)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var landlord = await _landLordRepository.GetAll()
            .FirstOrDefaultAsync(l => l.UserId == user.Id);

        if (landlord == null)
        {
            return NotFound();
        }

        // Check if Educational/Verification Data is being edited
        bool verificationDataChanged = 
            landlord.NationalId != viewModel.NationalId ||
            landlord.CompanyName != viewModel.CompanyName ||
            landlord.PropertyOwnerShipProof != viewModel.PropertyOwnerShipProof;

        // Update user personal data (always allowed)
        user.Email = viewModel.Email;
        user.PhoneNumber = viewModel.PhoneNumber;
        user.ProfileImage = viewModel.ProfileImage;

        // Update landlord data
        landlord.CompanyName = viewModel.CompanyName;
        landlord.NationalId = viewModel.NationalId;
        landlord.PropertyOwnerShipProof = viewModel.PropertyOwnerShipProof;

        // If Verification Data changed, set status to Pending
        if (verificationDataChanged)
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

        await _landLordRepository.Update(landlord);
        await _landLordRepository.CommitAsync();

        if (verificationDataChanged)
        {
            TempData["Message"] = "Profile updated. Since you changed verification data, your account is now pending approval.";
        }
        else
        {
            TempData["Message"] = "Profile updated successfully.";
        }

        return RedirectToAction(nameof(Profile));
    }

    // GET: LandLord/MyUnits
    [HttpGet]
    [Authorize(Roles = "LandLord")]
    public async Task<IActionResult> MyUnits()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var landlord = await _landLordRepository.GetAll()
            .FirstOrDefaultAsync(l => l.UserId == user.Id);

        if (landlord == null)
        {
            return NotFound();
        }

        var units = await _housingUnitRepository.GetAll()
            .Where(u => u.LandLordId == landlord.LandLordId)
            .ToListAsync();

        return View(units);
    }

    // GET: LandLord/EditUnit/5
    [HttpGet]
    [Authorize(Roles = "LandLord")]
    public async Task<IActionResult> EditUnit(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var unit = await _housingUnitRepository.GetAsync(id);
        if (unit == null)
        {
            return NotFound();
        }

        // Check if the unit belongs to this landlord
        var landlord = await _landLordRepository.GetAll()
            .FirstOrDefaultAsync(l => l.UserId == user.Id);
        
        if (landlord == null || unit.LandLordId != landlord.LandLordId)
        {
            return Forbid();
        }

        return View(unit);
    }

    // POST: LandLord/EditUnit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "LandLord")]
    public async Task<IActionResult> EditUnit(HousingUnit unit)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var existingUnit = await _housingUnitRepository.GetAsync(unit.HousingUnitId);
        if (existingUnit == null)
        {
            return NotFound();
        }

        // Check if the unit belongs to this landlord
        var landlord = await _landLordRepository.GetAll()
            .FirstOrDefaultAsync(l => l.UserId == user.Id);
        
        if (landlord == null || existingUnit.LandLordId != landlord.LandLordId)
        {
            return Forbid();
        }

        // Check if unit information is being edited (title, description, price, etc.)
        bool unitInfoChanged = 
            existingUnit.Title != unit.Title ||
            existingUnit.Description != unit.Description ||
            existingUnit.Price != unit.Price ||
            existingUnit.Address != unit.Address ||
            existingUnit.City != unit.City ||
            existingUnit.Area != unit.Area ||
            existingUnit.Rules != unit.Rules;

        // Update unit
        existingUnit.Title = unit.Title;
        existingUnit.Description = unit.Description;
        existingUnit.Price = unit.Price;
        existingUnit.Address = unit.Address;
        existingUnit.City = unit.City;
        existingUnit.Area = unit.Area;
        existingUnit.Rules = unit.Rules;
        existingUnit.UnitImageUrl = unit.UnitImageUrl;

        // If unit information changed, set landlord status to Pending
        if (unitInfoChanged)
        {
            user.Status = UserStatus.Pending;
            await _userManager.UpdateAsync(user);
        }

        await _housingUnitRepository.Update(existingUnit);
        await _housingUnitRepository.CommitAsync();

        if (unitInfoChanged)
        {
            TempData["Message"] = "Unit updated. Since you changed unit information, your account is now pending approval.";
        }
        else
        {
            TempData["Message"] = "Unit updated successfully.";
        }

        return RedirectToAction(nameof(MyUnits));
    }
}
