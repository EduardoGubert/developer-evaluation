namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Response model for GetSales operation.
/// </summary>
public class GetSalesResult
{
    /// <summary>
    /// The list of sales.
    /// </summary>
    public List<GetSalesItemResult> Data { get; set; } = new();

    /// <summary>
    /// The total number of items.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// The current page number.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages { get; set; }
}
