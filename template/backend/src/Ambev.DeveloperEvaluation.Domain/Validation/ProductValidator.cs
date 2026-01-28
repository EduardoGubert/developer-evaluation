using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for the Product entity.
/// </summary>
public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("Product title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(p => p.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(p => p.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters.");

        RuleFor(p => p.Image)
            .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters.");

        RuleFor(p => p.RatingRate)
            .InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5.");

        RuleFor(p => p.RatingCount)
            .GreaterThanOrEqualTo(0).WithMessage("Rating count must be non-negative.");
    }
}
