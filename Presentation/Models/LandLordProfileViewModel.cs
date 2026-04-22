using Domain.Entities;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class LandLordProfileViewModel
{
    public Guid LandLordId { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    public string? ProfileImage { get; set; }

    public UserStatus Status { get; set; }

    // Verification data (requires approval if changed)
    public string? CompanyName { get; set; }

    public string? NationalId { get; set; }

    public string? PropertyOwnerShipProof { get; set; }
}
