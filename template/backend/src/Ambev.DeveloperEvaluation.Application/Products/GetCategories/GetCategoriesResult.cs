namespace Ambev.DeveloperEvaluation.Application.Products.GetCategories;

/// <summary>
/// Result of the GetCategories operation.
/// </summary>
public class GetCategoriesResult
{
    public List<string> Categories { get; set; } = new();
}
