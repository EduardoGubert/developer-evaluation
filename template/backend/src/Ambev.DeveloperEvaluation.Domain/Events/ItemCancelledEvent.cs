namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when a sale item is cancelled.
/// </summary>
public class ItemCancelledEvent
{
    /// <summary>
    /// Gets the identifier of the sale containing the cancelled item.
    /// </summary>
    public Guid SaleId { get; }

    /// <summary>
    /// Gets the identifier of the cancelled item.
    /// </summary>
    public Guid ItemId { get; }

    /// <summary>
    /// Gets the product name of the cancelled item.
    /// </summary>
    public string ProductName { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; }

    /// <summary>
    /// Initializes a new instance of the ItemCancelledEvent class.
    /// </summary>
    /// <param name="saleId">The identifier of the sale.</param>
    /// <param name="itemId">The identifier of the cancelled item.</param>
    /// <param name="productName">The product name of the cancelled item.</param>
    public ItemCancelledEvent(Guid saleId, Guid itemId, string productName)
    {
        SaleId = saleId;
        ItemId = itemId;
        ProductName = productName;
        OccurredAt = DateTime.UtcNow;
    }
}
