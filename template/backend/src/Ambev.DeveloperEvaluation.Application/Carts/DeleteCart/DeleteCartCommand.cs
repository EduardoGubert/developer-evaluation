using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

/// <summary>
/// Command for deleting a cart by ID.
/// </summary>
public class DeleteCartCommand : IRequest<DeleteCartResponse>
{
    public Guid Id { get; set; }

    public DeleteCartCommand(Guid id)
    {
        Id = id;
    }
}
