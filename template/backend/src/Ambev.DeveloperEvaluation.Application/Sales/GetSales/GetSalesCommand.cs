using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Command for retrieving a paginated list of sales.
/// </summary>
public class GetSalesCommand : IRequest<GetSalesResult>
{
    /// <summary>
    /// The page number (1-based).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int Size { get; set; } = 10;

    /// <summary>
    /// The ordering string (e.g., "saleDate desc, saleNumber asc").
    /// </summary>
    public string? Order { get; set; }

    /// <summary>
    /// Gets or sets the filters to apply to the query.
    /// </summary>
    public Dictionary<string, string> Filters { get; set; } = new();
}
