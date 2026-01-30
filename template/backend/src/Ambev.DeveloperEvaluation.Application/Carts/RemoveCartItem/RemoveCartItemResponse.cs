namespace Ambev.DeveloperEvaluation.Application.Carts.RemoveCartItem;

/// <summary>
/// Response model for RemoveCartItem operation.
/// </summary>
public class RemoveCartItemResponse
{
    /// <summary>
    /// Indicates whether the removal was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The cart identifier.
    /// </summary>
    public Guid CartId { get; set; }

    /// <summary>
    /// The product identifier that was removed.
    /// </summary>
    public Guid ProductId { get; set; }
}
