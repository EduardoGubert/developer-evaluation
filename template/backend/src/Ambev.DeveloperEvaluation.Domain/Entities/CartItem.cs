using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item in a shopping cart.
/// </summary>
public class CartItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the cart ID.
    /// </summary>
    public Guid CartId { get; set; }

    /// <summary>
    /// Gets or sets the cart navigation property.
    /// </summary>
    public Cart Cart { get; set; } = null!;

    /// <summary>
    /// Gets or sets the product ID (External Identity).
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Initializes a new instance of the CartItem class.
    /// </summary>
    public CartItem()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Updates the quantity.
    /// </summary>
    /// <param name="quantity">The new quantity.</param>
    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }
}
