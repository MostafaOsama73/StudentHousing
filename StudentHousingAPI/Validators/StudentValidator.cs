using Business.Models.Requests;
using FluentValidation;

namespace StudentHousingAPI.Validators;

public class StudentRegisterValidator : AbstractValidator<StudentRegisterRequest>
{
    public StudentRegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past.");
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value.");
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.");
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");
        RuleFor(x => x.PreferredArea)
            .NotEmpty().WithMessage("Preferred area is required.");
        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("National ID is required.");
        RuleFor(x => x.ProfileImage)
            .NotEmpty().WithMessage("Profile image is required.");
    }

}