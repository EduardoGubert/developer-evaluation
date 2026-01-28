namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

/// <summary>
/// Rating information in the UpdateProduct response.
/// </summary>
public class UpdateProductRatingResponse
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
