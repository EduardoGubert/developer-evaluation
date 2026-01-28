using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

/// <summary>
/// Validator for UpdateCartRequest.
/// </summary>
public class UpdateCartRequestValidator : AbstractValidator<UpdateCartRequest>
{
    public UpdateCartRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required.");

        RuleFor(x => x.Products)
            .NotEmpty()
            .WithMessage("At least one product is required.");

        RuleForEach(x => x.Products)
            .SetValidator(new UpdateCartProductRequestValidator());
    }
}

/// <summary>
/// Validator for UpdateCartProductRequest.
/// </summary>
public class UpdateCartProductRequestValidator : AbstractValidator<UpdateCartProductRequest>
{
    public UpdateCartProductRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");
    }
}
