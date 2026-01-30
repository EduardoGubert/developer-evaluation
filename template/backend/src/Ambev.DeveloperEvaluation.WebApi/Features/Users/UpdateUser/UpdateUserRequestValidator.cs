using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;

/// <summary>
/// Validator for UpdateUserRequest that defines validation rules for user update.
/// </summary>
public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateUserRequestValidator with defined validation rules.
    /// </summary>
    public UpdateUserRequestValidator()
    {
        RuleFor(user => user.Email).SetValidator(new EmailValidator());
        RuleFor(user => user.Username).NotEmpty().Length(3, 50);
        RuleFor(user => user.Password)
            .SetValidator(new PasswordValidator()!)
            .When(user => !string.IsNullOrEmpty(user.Password));
        RuleFor(user => user.Phone).Matches(@"^\+?[1-9]\d{1,14}$");
        RuleFor(user => user.Status).NotEqual(UserStatus.Unknown);
        RuleFor(user => user.Role).NotEqual(UserRole.None);
               
        RuleFor(user => user.Name).NotNull().WithMessage("Name is required.");
        RuleFor(user => user.Name.Firstname)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");
        RuleFor(user => user.Name.Lastname)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");
                
        RuleFor(user => user.Address).NotNull().WithMessage("Address is required.");
        RuleFor(user => user.Address.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");
        RuleFor(user => user.Address.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(200).WithMessage("Street cannot exceed 200 characters.");
        RuleFor(user => user.Address.Number)
            .GreaterThan(0).WithMessage("Address number must be greater than 0.");
        RuleFor(user => user.Address.Zipcode)
            .NotEmpty().WithMessage("Zipcode is required.")
            .MaximumLength(20).WithMessage("Zipcode cannot exceed 20 characters.");
    }
}
