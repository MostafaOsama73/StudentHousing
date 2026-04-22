namespace Business.Models.Requests;
using Shared.Enums;

/// <summary>
/// Request model for student registration
/// Extends RegisterRequest with student-specific information
/// </summary>
public class StudentRegisterRequest : RegisterRequest
{
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; } // Use enum value (Male, Female)
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PreferredArea { get; set; }
    public string? NationalId { get; set; }
    public string? ProfileImage { get; set; } // Base64 encoded image or URL
}

/// <summary>
/// Request model for updating student profile
/// </summary>
public class UpdateStudentRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PreferredArea { get; set; }
    public string? PhoneNumber { get; set; }
}

/// <summary>
/// Request model for filtering/searching students
/// </summary>
public class StudentFilterRequest
{
    public string? City { get; set; }
    public string? PreferredArea { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirthFrom { get; set; }
    public DateTime? DateOfBirthTo { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Request model for student password change
/// </summary>
public class ChangePasswordRequest
{
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
}
