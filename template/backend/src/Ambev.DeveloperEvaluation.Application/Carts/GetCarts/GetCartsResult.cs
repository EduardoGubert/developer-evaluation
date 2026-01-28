namespace Ambev.DeveloperEvaluation.Application.Carts.GetCarts;

/// <summary>
/// Result of the GetCarts operation.
/// </summary>
public class GetCartsResult
{
    public List<GetCartsItemResult> Data { get; set; } = new();
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class GetCartsItemResult
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public List<GetCartsProductResult> Products { get; set; } = new();
}

public class GetCartsProductResult
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
