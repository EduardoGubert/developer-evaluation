namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Response model for UpdateSale operation.
/// </summary>
public class UpdateSaleResult
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
    /// The total amount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// The sale items.
    /// </summary>
    public List<UpdateSaleItemResult> Items { get; set; } = new();
}

/// <summary>
/// Response model for a sale item in the update response.
/// </summary>
public class UpdateSaleItemResult
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
}
