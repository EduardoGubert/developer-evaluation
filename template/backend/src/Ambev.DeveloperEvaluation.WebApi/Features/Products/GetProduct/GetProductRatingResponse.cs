namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;

/// <summary>
/// Rating information in the GetProduct response.
/// </summary>
public class GetProductRatingResponse
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
