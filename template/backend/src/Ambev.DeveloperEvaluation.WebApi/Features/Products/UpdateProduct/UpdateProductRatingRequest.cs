namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

/// <summary>
/// Rating information for updating a product.
/// </summary>
public class UpdateProductRatingRequest
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
