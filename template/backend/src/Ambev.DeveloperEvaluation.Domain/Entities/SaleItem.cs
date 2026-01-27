using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item within a sale, containing product information, quantity, pricing, and discount details.
/// This entity follows domain-driven design principles with business rules for quantity-based discounts.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the sale identifier this item belongs to.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the parent sale.
    /// </summary>
    public Sale Sale { get; set; } = null!;

    /// <summary>
    /// Gets or sets the external product identifier.
    /// This is an external identity reference following DDD patterns.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the denormalized product name for display purposes.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of products in this sale item.
    /// Must be between 1 and 20 items.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets the discount percentage applied to this item.
    /// Calculated based on quantity: 0% for less than 4, 10% for 4-9, 20% for 10-20 items.
    /// </summary>
    public decimal Discount { get; private set; }

    /// <summary>
    /// Gets the total amount for this item after discount.
    /// Calculated as: (UnitPrice * Quantity) - (UnitPrice * Quantity * Discount)
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Gets or sets whether this item has been cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Initializes a new instance of the SaleItem class.
    /// </summary>
    public SaleItem()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Calculates the discount based on the quantity of items.
    /// Business rules:
    /// - Less than 4 items: 0% discount
    /// - 4 to 9 items: 10% discount
    /// - 10 to 20 items: 20% discount
    /// - More than 20 items: Not allowed (throws exception)
    /// </summary>
    /// <exception cref="DomainException">Thrown when quantity exceeds 20 items.</exception>
    public void CalculateDiscount()
    {
        if (Quantity > 20)
        {
            throw new DomainException("Cannot sell more than 20 identical items.");
        }

        if (Quantity < 4)
        {
            Discount = 0;
        }
        else if (Quantity >= 4 && Quantity < 10)
        {
            Discount = 0.10m;
        }
        else
        {
            Discount = 0.20m;
        }

        CalculateTotalAmount();
    }

    /// <summary>
    /// Calculates the total amount for this item.
    /// Formula: (UnitPrice * Quantity) - (UnitPrice * Quantity * Discount)
    /// </summary>
    public void CalculateTotalAmount()
    {
        var subtotal = UnitPrice * Quantity;
        TotalAmount = subtotal - (subtotal * Discount);
    }

    /// <summary>
    /// Cancels this sale item.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
    }

    /// <summary>
    /// Updates the item with new values and recalculates discount and total.
    /// </summary>
    /// <param name="quantity">The new quantity.</param>
    /// <param name="unitPrice">The new unit price.</param>
    public void Update(int quantity, decimal unitPrice)
    {
        Quantity = quantity;
        UnitPrice = unitPrice;
        CalculateDiscount();
    }
}
