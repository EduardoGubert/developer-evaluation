using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

/// <summary>
/// Validator for UpdateUserCommand.
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50)
            .WithMessage("Username cannot be longer than 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Phone)
            .Matches(@"^\+[1-9]\d{10,14}$")
            .WithMessage("Phone number must start with '+' followed by 11-15 digits.");

        RuleFor(x => x.Status)
            .NotEqual(UserStatus.Unknown)
            .WithMessage("User status cannot be Unknown.");

        RuleFor(x => x.Role)
            .NotEqual(UserRole.None)
            .WithMessage("User role cannot be None.");

        // Name validation
        RuleFor(x => x.Name).NotNull().WithMessage("Name is required.");
        RuleFor(x => x.Name.Firstname)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");
        RuleFor(x => x.Name.Lastname)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

        // Address validation
        RuleFor(x => x.Address).NotNull().WithMessage("Address is required.");
        RuleFor(x => x.Address.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");
        RuleFor(x => x.Address.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(200).WithMessage("Street cannot exceed 200 characters.");
        RuleFor(x => x.Address.Number)
            .GreaterThan(0).WithMessage("Address number must be greater than 0.");
        RuleFor(x => x.Address.Zipcode)
            .NotEmpty().WithMessage("Zipcode is required.")
            .MaximumLength(20).WithMessage("Zipcode cannot exceed 20 characters.");
    }
}
