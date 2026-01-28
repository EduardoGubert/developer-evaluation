namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CheckoutCart;

/// <summary>
/// Request model for cart checkout.
/// </summary>
public class CheckoutCartRequest
{
    /// <summary>
    /// The branch ID where the sale is made.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// The branch name where the sale is made.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;
}
