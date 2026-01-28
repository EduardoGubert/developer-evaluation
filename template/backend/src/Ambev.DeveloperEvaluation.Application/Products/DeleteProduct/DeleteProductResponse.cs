namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;

/// <summary>
/// Response of the DeleteProduct operation.
/// </summary>
public class DeleteProductResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
