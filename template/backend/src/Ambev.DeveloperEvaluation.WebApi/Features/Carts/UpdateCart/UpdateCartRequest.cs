namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

/// <summary>
/// Request model for updating a cart.
/// </summary>
public class UpdateCartRequest
{
    /// <summary>
    /// Gets or sets the user ID who owns the cart.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the date of the cart.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the products in the cart.
    /// </summary>
    public List<UpdateCartProductRequest> Products { get; set; } = new();
}

/// <summary>
/// Product item in the cart update request.
/// </summary>
public class UpdateCartProductRequest
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
