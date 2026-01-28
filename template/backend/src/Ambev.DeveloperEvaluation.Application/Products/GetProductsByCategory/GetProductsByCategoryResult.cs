namespace Ambev.DeveloperEvaluation.Application.Products.GetProductsByCategory;

/// <summary>
/// Result of the GetProductsByCategory operation.
/// </summary>
public class GetProductsByCategoryResult
{
    public List<GetProductsByCategoryItemResult> Data { get; set; } = new();
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class GetProductsByCategoryItemResult
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public GetProductsByCategoryRatingResult Rating { get; set; } = new();
}

public class GetProductsByCategoryRatingResult
{
    public decimal Rate { get; set; }
    public int Count { get; set; }
}
