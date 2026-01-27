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

/// <summary>
/// Response model for a sale in the list.
/// </summary>
public class GetSalesItemResult
{
    /// <summary>
    /// The unique identifier of the sale.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// The sale date.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// The customer identifier.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// The customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// The branch identifier.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// The branch name.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// The total amount of the sale.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Whether the sale is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }
}
