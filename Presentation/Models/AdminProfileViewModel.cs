using Domain.Entities;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class AdminProfileViewModel
{
    [Required]
    [StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    public string? ProfileImage { get; set; }

    public UserStatus Status { get; set; }
}
