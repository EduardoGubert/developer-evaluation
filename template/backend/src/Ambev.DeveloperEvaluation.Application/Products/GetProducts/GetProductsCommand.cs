using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProducts;

/// <summary>
/// Command for retrieving a paginated list of products.
/// </summary>
public class GetProductsCommand : IRequest<GetProductsResult>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
    public Dictionary<string, string> Filters { get; set; } = new();
}
