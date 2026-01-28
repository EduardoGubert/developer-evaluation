namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProducts;

/// <summary>
/// Rating information in the GetProducts response.
/// </summary>
public class GetProductsRatingResponse
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
