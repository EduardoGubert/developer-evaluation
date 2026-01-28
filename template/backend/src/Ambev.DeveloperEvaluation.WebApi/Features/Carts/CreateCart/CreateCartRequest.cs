namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;

/// <summary>
/// Request model for creating a new cart.
/// </summary>
public class CreateCartRequest
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
    public List<CreateCartProductRequest> Products { get; set; } = new();
}

/// <summary>
/// Product item in the cart creation request.
/// </summary>
public class CreateCartProductRequest
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
