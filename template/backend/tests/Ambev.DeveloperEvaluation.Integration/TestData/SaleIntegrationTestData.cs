using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.TestData;

/// <summary>
/// Test data generators for Sales integration tests.
/// </summary>
public static class SaleIntegrationTestData
{
    private static int _saleNumberCounter = 1;

    /// <summary>
    /// Generates a unique sale number for tests.
    /// </summary>
    public static string GenerateUniqueSaleNumber()
    {
        return $"SALE-TEST-{Interlocked.Increment(ref _saleNumberCounter):D5}";
    }

    /// <summary>
    /// Creates a valid sale request with specified quantity for discount testing.
    /// </summary>
    public static CreateSaleTestRequest CreateSaleRequest(int quantity = 1, decimal unitPrice = 100m)
    {
        var faker = new Faker();
        return new CreateSaleTestRequest
        {
            SaleNumber = GenerateUniqueSaleNumber(),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Name.FullName(),
            BranchId = Guid.NewGuid(),
            BranchName = faker.Company.CompanyName(),
            Items = new List<CreateSaleItemTestRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = quantity,
                    UnitPrice = unitPrice
                }
            }
        };
    }

    /// <summary>
    /// Creates a sale request with multiple items.
    /// </summary>
    public static CreateSaleTestRequest CreateSaleRequestWithMultipleItems(params (int quantity, decimal unitPrice)[] items)
    {
        var faker = new Faker();
        var request = new CreateSaleTestRequest
        {
            SaleNumber = GenerateUniqueSaleNumber(),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Name.FullName(),
            BranchId = Guid.NewGuid(),
            BranchName = faker.Company.CompanyName(),
            Items = new List<CreateSaleItemTestRequest>()
        };

        foreach (var (quantity, unitPrice) in items)
        {
            request.Items.Add(new CreateSaleItemTestRequest
            {
                ProductId = Guid.NewGuid(),
                ProductName = faker.Commerce.ProductName(),
                Quantity = quantity,
                UnitPrice = unitPrice
            });
        }

        return request;
    }

    /// <summary>
    /// Creates an update sale request.
    /// </summary>
    public static UpdateSaleTestRequest CreateUpdateRequest(int quantity = 5, decimal unitPrice = 150m)
    {
        var faker = new Faker();
        return new UpdateSaleTestRequest
        {
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Name.FullName(),
            BranchId = Guid.NewGuid(),
            BranchName = faker.Company.CompanyName(),
            Items = new List<UpdateSaleItemTestRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = quantity,
                    UnitPrice = unitPrice
                }
            }
        };
    }
}

#region Request/Response DTOs

public class CreateSaleTestRequest
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public List<CreateSaleItemTestRequest> Items { get; set; } = new();
}

public class CreateSaleItemTestRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class UpdateSaleTestRequest
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public List<UpdateSaleItemTestRequest> Items { get; set; } = new();
}

public class UpdateSaleItemTestRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class SaleTestResponse
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
    public List<SaleItemTestResponse> Items { get; set; } = new();
}

public class SaleItemTestResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
}

public class GetSalesTestResponse
{
    public List<SaleTestResponse> Data { get; set; } = new();
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

#endregion
