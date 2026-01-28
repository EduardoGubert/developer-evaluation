using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

/// <summary>
/// Command for retrieving a cart by ID.
/// </summary>
public class GetCartCommand : IRequest<GetCartResult>
{
    public Guid Id { get; set; }

    public GetCartCommand(Guid id)
    {
        Id = id;
    }
}
