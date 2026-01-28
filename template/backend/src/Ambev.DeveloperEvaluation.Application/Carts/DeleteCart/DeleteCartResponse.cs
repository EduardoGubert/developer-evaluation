namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

/// <summary>
/// Response of the DeleteCart operation.
/// </summary>
public class DeleteCartResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
