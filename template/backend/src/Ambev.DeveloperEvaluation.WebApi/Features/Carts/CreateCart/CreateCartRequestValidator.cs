using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;

/// <summary>
/// Validator for CreateCartRequest.
/// </summary>
public class CreateCartRequestValidator : AbstractValidator<CreateCartRequest>
{
    public CreateCartRequestValidator()
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
            .SetValidator(new CreateCartProductRequestValidator());
    }
}

/// <summary>
/// Validator for CreateCartProductRequest.
/// </summary>
public class CreateCartProductRequestValidator : AbstractValidator<CreateCartProductRequest>
{
    public CreateCartProductRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");
    }
}
