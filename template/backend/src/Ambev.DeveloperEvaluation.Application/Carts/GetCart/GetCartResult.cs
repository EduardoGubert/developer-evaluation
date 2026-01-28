namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

/// <summary>
/// Result of the GetCart operation.
/// </summary>
public class GetCartResult
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public List<GetCartProductResult> Products { get; set; } = new();
}

public class GetCartProductResult
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
