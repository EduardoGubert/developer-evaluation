namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCarts;

/// <summary>
/// Response model for getting paginated carts.
/// </summary>
public class GetCartsResponse
{
    /// <summary>
    /// Gets or sets the list of carts.
    /// </summary>
    public List<GetCartsItemResponse> Data { get; set; } = new();

    /// <summary>
    /// Gets or sets the total number of items.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages.
    /// </summary>
    public int TotalPages { get; set; }
}

/// <summary>
/// Cart item in the get carts response.
/// </summary>
public class GetCartsItemResponse
{
    /// <summary>
    /// Gets or sets the cart ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the products.
    /// </summary>
    public List<GetCartsProductResponse> Products { get; set; } = new();
}

/// <summary>
/// Product item in the get carts response.
/// </summary>
public class GetCartsProductResponse
{
    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }
}
