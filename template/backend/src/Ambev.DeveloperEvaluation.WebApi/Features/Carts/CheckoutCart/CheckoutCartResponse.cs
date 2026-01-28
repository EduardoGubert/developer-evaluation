namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CheckoutCart;

/// <summary>
/// Response model for cart checkout.
/// </summary>
public class CheckoutCartResponse
{
    /// <summary>
    /// The created sale ID.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// The generated sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// The total amount after discounts.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// The sale items with discount details.
    /// </summary>
    public List<CheckoutCartItemResponse> Items { get; set; } = new();
}

/// <summary>
/// Response for each item in the checkout.
/// </summary>
public class CheckoutCartItemResponse
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
}
