namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;

/// <summary>
/// Response model for cart creation.
/// </summary>
public class CreateCartResponse
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
    public List<CreateCartProductResponse> Products { get; set; } = new();
}

/// <summary>
/// Product item in the cart creation response.
/// </summary>
public class CreateCartProductResponse
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
