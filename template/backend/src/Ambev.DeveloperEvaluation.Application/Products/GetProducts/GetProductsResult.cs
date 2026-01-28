namespace Ambev.DeveloperEvaluation.Application.Products.GetProducts;

/// <summary>
/// Result of the GetProducts operation.
/// </summary>
public class GetProductsResult
{
    public List<GetProductsItemResult> Data { get; set; } = new();
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
