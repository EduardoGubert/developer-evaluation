using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

/// <summary>
/// Handler for processing DeleteCartCommand requests.
/// </summary>
public class DeleteCartHandler : IRequestHandler<DeleteCartCommand, DeleteCartResponse>
{
    private readonly ICartRepository _cartRepository;

    public DeleteCartHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<DeleteCartResponse> Handle(DeleteCartCommand command, CancellationToken cancellationToken)
    {
        var deleted = await _cartRepository.DeleteAsync(command.Id, cancellationToken);

        if (!deleted)
            throw new KeyNotFoundException($"Cart with ID {command.Id} not found");

        return new DeleteCartResponse
        {
            Success = true,
            Message = "Cart deleted successfully"
        };
    }
}
