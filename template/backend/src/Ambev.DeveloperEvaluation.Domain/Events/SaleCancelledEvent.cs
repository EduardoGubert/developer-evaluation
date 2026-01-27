namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when a sale is cancelled.
/// </summary>
public class SaleCancelledEvent
{
    /// <summary>
    /// Gets the identifier of the sale that was cancelled.
    /// </summary>
    public Guid SaleId { get; }

    /// <summary>
    /// Gets the sale number of the cancelled sale.
    /// </summary>
    public string SaleNumber { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; }

    /// <summary>
    /// Initializes a new instance of the SaleCancelledEvent class.
    /// </summary>
    /// <param name="saleId">The identifier of the cancelled sale.</param>
    /// <param name="saleNumber">The sale number.</param>
    public SaleCancelledEvent(Guid saleId, string saleNumber)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        OccurredAt = DateTime.UtcNow;
    }
}
