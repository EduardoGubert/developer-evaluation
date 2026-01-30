using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.RemoveCartItem;

/// <summary>
/// Command for removing a product from a cart.
/// </summary>
public record RemoveCartItemCommand : IRequest<RemoveCartItemResponse>
{
    /// <summary>
    /// The unique identifier of the cart.
    /// </summary>
    public Guid CartId { get; }

    /// <summary>
    /// The unique identifier of the product to remove.
    /// </summary>
    public Guid ProductId { get; }

    public RemoveCartItemCommand(Guid cartId, Guid productId)
    {
        CartId = cartId;
        ProductId = productId;
    }
}
