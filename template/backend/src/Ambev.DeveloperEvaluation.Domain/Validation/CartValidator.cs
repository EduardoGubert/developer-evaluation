using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for the Cart entity.
/// </summary>
public class CartValidator : AbstractValidator<Cart>
{
    public CartValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User is required.");

        RuleFor(c => c.Date)
            .NotEmpty().WithMessage("Date is required.");
    }
}
