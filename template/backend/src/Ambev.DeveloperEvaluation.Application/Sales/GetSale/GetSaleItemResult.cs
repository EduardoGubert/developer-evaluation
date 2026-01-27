namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Response model for a sale item.
/// </summary>
public class GetSaleItemResult
{
    /// <summary>
    /// The item identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The product identifier.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// The product name.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// The quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// The discount percentage.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// The total amount for this item.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Whether the item is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }
}
