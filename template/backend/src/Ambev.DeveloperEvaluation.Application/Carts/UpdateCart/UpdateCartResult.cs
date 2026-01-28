namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

/// <summary>
/// Result of the UpdateCart operation.
/// </summary>
public class UpdateCartResult
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public List<UpdateCartProductResult> Products { get; set; } = new();
}

public class UpdateCartProductResult
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
