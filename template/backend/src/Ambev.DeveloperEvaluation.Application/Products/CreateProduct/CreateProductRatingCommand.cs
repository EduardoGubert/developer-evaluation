namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

/// <summary>
/// Rating information for the product.
/// </summary>
public class CreateProductRatingCommand
{
    /// <summary>
    /// Gets or sets the rating rate (0-5).
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// Gets or sets the rating count.
    /// </summary>
    public int Count { get; set; }
}
