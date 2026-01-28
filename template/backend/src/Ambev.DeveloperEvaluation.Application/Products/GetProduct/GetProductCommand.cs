using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct;

/// <summary>
/// Command for retrieving a product by ID.
/// </summary>
public class GetProductCommand : IRequest<GetProductResult>
{
    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public Guid Id { get; set; }

    public GetProductCommand(Guid id)
    {
        Id = id;
    }
}
