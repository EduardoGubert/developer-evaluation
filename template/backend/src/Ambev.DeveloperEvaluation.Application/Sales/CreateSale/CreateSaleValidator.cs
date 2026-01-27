using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleCommand that defines validation rules for sale creation.
/// </summary>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(sale => sale.SaleNumber)
            .NotEmpty()
            .WithMessage("Sale number is required.");

        RuleFor(sale => sale.SaleDate)
            .NotEmpty()
            .WithMessage("Sale date is required.");

        RuleFor(sale => sale.CustomerId)
            .NotEmpty()
            .WithMessage("Customer is required.");

        RuleFor(sale => sale.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(200)
            .WithMessage("Customer name cannot be longer than 200 characters.");

        RuleFor(sale => sale.BranchId)
            .NotEmpty()
            .WithMessage("Branch is required.");

        RuleFor(sale => sale.BranchName)
            .NotEmpty()
            .WithMessage("Branch name is required.")
            .MaximumLength(200)
            .WithMessage("Branch name cannot be longer than 200 characters.");

        RuleFor(sale => sale.Items)
            .NotEmpty()
            .WithMessage("Sale must have at least one item.");

        RuleForEach(sale => sale.Items)
            .SetValidator(new CreateSaleItemCommandValidator());
    }
}

/// <summary>
/// Validator for CreateSaleItemCommand.
/// </summary>
public class CreateSaleItemCommandValidator : AbstractValidator<CreateSaleItemCommand>
{
    public CreateSaleItemCommandValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty()
            .WithMessage("Product is required.");

        RuleFor(item => item.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(200)
            .WithMessage("Product name cannot be longer than 200 characters.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(20)
            .WithMessage("Cannot sell more than 20 identical items.");

        RuleFor(item => item.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than 0.");
    }
}
