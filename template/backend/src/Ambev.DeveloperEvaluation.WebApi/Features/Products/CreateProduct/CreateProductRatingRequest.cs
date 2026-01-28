namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

/// <summary>
/// Rating information for creating a product.
/// </summary>
public class CreateProductRatingRequest
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
