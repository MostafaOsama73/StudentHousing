using Business.Models.Requests;
using FluentValidation;

namespace StudentHousingAPI.Validators;

public class LandLordRegisterValidator : AbstractValidator<LandLordRegisterRequest>
{
    public LandLordRegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.");
        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("National ID is required.");
        RuleFor(x => x.PropertyOwnerShipProof)
            .NotEmpty().WithMessage("Property ownership proof is required.");
        RuleFor(x => x.ProfileImage)
            .NotEmpty().WithMessage("Profile image is required.");
    }
}
