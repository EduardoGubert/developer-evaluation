using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Command for retrieving a paginated list of sales.
/// </summary>
public record GetSalesCommand : IRequest<GetSalesResult>
{
    /// <summary>
    /// The page number (1-based).
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int Size { get; init; } = 10;

    /// <summary>
    /// The ordering string (e.g., "saleDate desc, saleNumber asc").
    /// </summary>
    public string? Order { get; init; }
}
