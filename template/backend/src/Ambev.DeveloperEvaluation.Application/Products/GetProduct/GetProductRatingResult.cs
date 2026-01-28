namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct;

/// <summary>
/// Rating information in the GetProduct result.
/// </summary>
public class GetProductRatingResult
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
