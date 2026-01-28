namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

/// <summary>
/// Rating information for updating a product.
/// </summary>
public class UpdateProductRatingCommand
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
