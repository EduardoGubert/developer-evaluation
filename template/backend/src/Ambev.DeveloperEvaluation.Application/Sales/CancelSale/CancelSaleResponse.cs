namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Response model for CancelSale operation.
/// </summary>
public class CancelSaleResponse
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
    /// The sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;
}
