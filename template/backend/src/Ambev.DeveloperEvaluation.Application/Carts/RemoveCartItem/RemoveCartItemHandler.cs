using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Carts.RemoveCartItem;

/// <summary>
/// Handler for processing RemoveCartItemCommand requests.
/// </summary>
public class RemoveCartItemHandler : IRequestHandler<RemoveCartItemCommand, RemoveCartItemResponse>
{
    private readonly ICartRepository _cartRepository;

    public RemoveCartItemHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<RemoveCartItemResponse> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        if (request.CartId == Guid.Empty)
            throw new ValidationException("Cart ID is required.");

        if (request.ProductId == Guid.Empty)
            throw new ValidationException("Product ID is required.");

        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (cart == null)
            throw new KeyNotFoundException($"Cart with ID {request.CartId} not found");

        var cartItem = cart.Products.FirstOrDefault(p => p.ProductId == request.ProductId);
        if (cartItem == null)
            throw new KeyNotFoundException($"Product with ID {request.ProductId} not found in cart");

        cart.RemoveProduct(request.ProductId);
        await _cartRepository.DeleteItemAsync(cartItem.Id, cancellationToken);

        return new RemoveCartItemResponse
        {
            Success = true,
            CartId = cart.Id,
            ProductId = request.ProductId
        };
    }
}
