namespace Ambev.DeveloperEvaluation.Application.Products.GetProducts;

/// <summary>
/// Rating information in the GetProducts result.
/// </summary>
public class GetProductsRatingResult
{
    /// <summary>
    /// Gets or sets the rating rate.
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// Gets or sets the rating count.
    /// </summary>
    public int Count { get; set; }
}
