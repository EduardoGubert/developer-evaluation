namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Response model for CancelSaleItem operation.
/// </summary>
public class CancelSaleItemResponse
{
    /// <summary>
    /// Indicates whether the cancel operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The sale identifier.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// The item identifier.
    /// </summary>
    public Guid ItemId { get; set; }

    /// <summary>
    /// The product name of the cancelled item.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
}
