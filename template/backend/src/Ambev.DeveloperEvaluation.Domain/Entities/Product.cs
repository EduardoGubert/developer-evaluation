using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a product in the catalog.
/// </summary>
public class Product : BaseEntity
{
    /// <summary>
    /// Gets or sets the product title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product category.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product image URL.
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product rating rate (0-5).
    /// </summary>
    public decimal RatingRate { get; set; }

    /// <summary>
    /// Gets or sets the product rating count.
    /// </summary>
    public int RatingCount { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the Product class.
    /// </summary>
    public Product()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the product information.
    /// </summary>
    public void Update(string title, decimal price, string description,
                       string category, string image, decimal ratingRate, int ratingCount)
    {
        Title = title;
        Price = price;
        Description = description;
        Category = category;
        Image = image;
        RatingRate = ratingRate;
        RatingCount = ratingCount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates the product entity.
    /// </summary>
    /// <returns>The validation result.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new ProductValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}
