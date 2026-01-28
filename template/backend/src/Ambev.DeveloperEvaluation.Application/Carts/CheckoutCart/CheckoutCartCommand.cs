using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.CheckoutCart;

/// <summary>
/// Command for checking out a cart and creating a sale.
/// </summary>
public class CheckoutCartCommand : IRequest<CheckoutCartResult>
{
    /// <summary>
    /// The cart ID to checkout.
    /// </summary>
    public Guid CartId { get; set; }

    /// <summary>
    /// The branch ID where the sale is made.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// The branch name where the sale is made.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;
}
