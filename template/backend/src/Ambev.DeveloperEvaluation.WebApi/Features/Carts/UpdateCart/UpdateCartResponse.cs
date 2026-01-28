namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

/// <summary>
/// Response model for cart update.
/// </summary>
public class UpdateCartResponse
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
    public List<UpdateCartProductResponse> Products { get; set; } = new();
}

/// <summary>
/// Product item in the cart update response.
/// </summary>
public class UpdateCartProductResponse
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
