using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

/// <summary>
/// Command for updating an existing user.
/// </summary>
public class UpdateUserCommand : IRequest<UpdateUserResult>
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password (optional for update).
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public UpdateUserNameDto Name { get; set; } = new();

    /// <summary>
    /// Gets or sets the address of the user.
    /// </summary>
    public UpdateUserAddressDto Address { get; set; } = new();

    /// <summary>
    /// Gets or sets the user status.
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the user role.
    /// </summary>
    public UserRole Role { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new UpdateUserCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}
