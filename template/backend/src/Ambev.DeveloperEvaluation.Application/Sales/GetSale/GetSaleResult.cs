namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Response model for GetSale operation.
/// </summary>
public class GetSaleResult
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

    /// <summary>
    /// The sale items.
    /// </summary>
    public List<GetSaleItemResult> Items { get; set; } = new();

    /// <summary>
    /// The date when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date when the sale was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

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
