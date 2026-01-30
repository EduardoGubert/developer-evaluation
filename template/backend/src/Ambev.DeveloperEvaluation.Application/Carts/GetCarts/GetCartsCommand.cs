using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCarts;

/// <summary>
/// Command for retrieving a paginated list of carts.
/// </summary>
public class GetCartsCommand : IRequest<GetCartsResult>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
    public Dictionary<string, string> Filters { get; set; } = new();
}
