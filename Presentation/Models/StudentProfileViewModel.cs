using Domain.Entities;
using Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class StudentProfileViewModel
{
    public Guid StudentId { get; set; }

    [Required]
    [StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public Gender Gender { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? PreferredArea { get; set; }

    public string? NationalId { get; set; }

    public string? ProfileImage { get; set; }

    public UserStatus Status { get; set; }
}
