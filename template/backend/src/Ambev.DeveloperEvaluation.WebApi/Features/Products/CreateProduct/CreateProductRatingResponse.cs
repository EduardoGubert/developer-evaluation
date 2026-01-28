namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

/// <summary>
/// Rating information in the CreateProduct response.
/// </summary>
public class CreateProductRatingResponse
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
