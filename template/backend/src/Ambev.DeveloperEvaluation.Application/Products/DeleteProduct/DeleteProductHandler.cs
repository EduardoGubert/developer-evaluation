using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;

/// <summary>
/// Handler for processing DeleteProductCommand requests.
/// </summary>
public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponse>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<DeleteProductResponse> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var deleted = await _productRepository.DeleteAsync(command.Id, cancellationToken);

        if (!deleted)
            throw new KeyNotFoundException($"Product with ID {command.Id} not found");

        return new DeleteProductResponse
        {
            Success = true,
            Message = "Product deleted successfully"
        };
    }
}
