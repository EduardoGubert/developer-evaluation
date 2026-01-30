using System.Net;
using System.Net.Http.Json;
using Ambev.DeveloperEvaluation.Integration.Fixtures;
using Ambev.DeveloperEvaluation.Integration.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

/// <summary>
/// Integration tests for creating sales via the API.
/// Tests cover all discount tiers and business rules.
/// </summary>
public class CreateSaleIntegrationTests : IntegrationTestBase
{
    private const string BaseUrl = "/api/sales";

    public CreateSaleIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    /// <summary>
    /// Tests that a sale with quantity less than 4 has no discount (0%).
    /// </summary>
    [Theory(DisplayName = "POST /api/sales - Quantity < 4 should have 0% discount")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task CreateSale_WithQuantityLessThan4_ShouldHaveNoDiscount(int quantity)
    {
        // Arrange
        var request = SaleIntegrationTestData.CreateSaleRequest(quantity: quantity, unitPrice: 100m);

        // Act
        var response = await PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await DeserializeResponseAsync<ApiResponseWithData<SaleTestResponse>>(response);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        
        var sale = result.Data!;
        sale.Items.Should().HaveCount(1);
        sale.Items[0].Discount.Should().Be(0m);
        sale.Items[0].TotalAmount.Should().Be(quantity * 100m);
        sale.TotalAmount.Should().Be(quantity * 100m);
    }

    /// <summary>
    /// Tests that a sale with quantity between 4 and 9 has 10% discount.
    /// </summary>
    [Theory(DisplayName = "POST /api/sales - Quantity 4-9 should have 10% discount")]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public async Task CreateSale_WithQuantityBetween4And9_ShouldHave10PercentDiscount(int quantity)
    {
        // Arrange
        var request = SaleIntegrationTestData.CreateSaleRequest(quantity: quantity, unitPrice: 100m);

        // Act
        var response = await PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await DeserializeResponseAsync<ApiResponseWithData<SaleTestResponse>>(response);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        
        var sale = result.Data!;
        sale.Items.Should().HaveCount(1);
        sale.Items[0].Discount.Should().Be(0.10m);
        
        var expectedTotal = (quantity * 100m) * 0.90m; // 10% discount
        sale.Items[0].TotalAmount.Should().Be(expectedTotal);
        sale.TotalAmount.Should().Be(expectedTotal);
    }

    /// <summary>
    /// Tests that a sale with quantity between 10 and 20 has 20% discount.
    /// </summary>
    [Theory(DisplayName = "POST /api/sales - Quantity 10-20 should have 20% discount")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public async Task CreateSale_WithQuantityBetween10And20_ShouldHave20PercentDiscount(int quantity)
    {
        // Arrange
        var request = SaleIntegrationTestData.CreateSaleRequest(quantity: quantity, unitPrice: 100m);

        // Act
        var response = await PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await DeserializeResponseAsync<ApiResponseWithData<SaleTestResponse>>(response);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        
        var sale = result.Data!;
        sale.Items.Should().HaveCount(1);
        sale.Items[0].Discount.Should().Be(0.20m);
        
        var expectedTotal = (quantity * 100m) * 0.80m; // 20% discount
        sale.Items[0].TotalAmount.Should().Be(expectedTotal);
        sale.TotalAmount.Should().Be(expectedTotal);
    }

    /// <summary>
    /// Tests that a sale with quantity greater than 20 is rejected.
    /// </summary>
    [Theory(DisplayName = "POST /api/sales - Quantity > 20 should be rejected")]
    [InlineData(21)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task CreateSale_WithQuantityGreaterThan20_ShouldReturnBadRequest(int quantity)
    {
        // Arrange
        var request = SaleIntegrationTestData.CreateSaleRequest(quantity: quantity, unitPrice: 100m);

        // Act
        var response = await PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Tests that a sale is created with all required fields populated.
    /// </summary>
    [Fact(DisplayName = "POST /api/sales - Should create sale with all required fields")]
    public async Task CreateSale_ShouldReturnSaleWithAllRequiredFields()
    {
        // Arrange
        var request = SaleIntegrationTestData.CreateSaleRequest(quantity: 5, unitPrice: 100m);

        // Act
        var response = await PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await DeserializeResponseAsync<ApiResponseWithData<SaleTestResponse>>(response);
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
        
        var sale = result.Data!;
        
        // Verify all required fields per README
        sale.Id.Should().NotBeEmpty();                    // Sale ID
        sale.SaleNumber.Should().NotBeNullOrEmpty();      // Sale number
        sale.SaleDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(5)); // Date
        sale.CustomerId.Should().NotBeEmpty();            // Customer
        sale.CustomerName.Should().NotBeNullOrEmpty();    // Customer name (denormalized)
        sale.BranchId.Should().NotBeEmpty();              // Branch
        sale.BranchName.Should().NotBeNullOrEmpty();      // Branch name (denormalized)
        sale.TotalAmount.Should().BeGreaterThan(0);       // Total sale amount
        sale.IsCancelled.Should().BeFalse();              // Cancelled status
        sale.Items.Should().NotBeEmpty();                 // Products
        
        // Verify item fields
        var item = sale.Items[0];
        item.Id.Should().NotBeEmpty();
        item.ProductId.Should().NotBeEmpty();
        item.ProductName.Should().NotBeNullOrEmpty();
        item.Quantity.Should().BeGreaterThan(0);          // Quantity
        item.UnitPrice.Should().BeGreaterThan(0);         // Unit price
        item.Discount.Should().BeGreaterThanOrEqualTo(0); // Discount
        item.TotalAmount.Should().BeGreaterThan(0);       // Total per item
        item.IsCancelled.Should().BeFalse();
    }

    /// <summary>
    /// Tests that creating a sale with duplicate sale number returns error.
    /// </summary>
    [Fact(DisplayName = "POST /api/sales - Duplicate sale number should return error")]
    public async Task CreateSale_WithDuplicateSaleNumber_ShouldReturnBadRequest()
    {
        // Arrange
        var request1 = SaleIntegrationTestData.CreateSaleRequest(quantity: 2);
        var request2 = SaleIntegrationTestData.CreateSaleRequest(quantity: 3);
        request2.SaleNumber = request1.SaleNumber; // Same sale number

        // Act
        var response1 = await PostAsJsonAsync(BaseUrl, request1);
        var response2 = await PostAsJsonAsync(BaseUrl, request2);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Tests that creating a sale with multiple items calculates total correctly.
    /// </summary>
    [Fact(DisplayName = "POST /api/sales - Multiple items should calculate total correctly")]
    public async Task CreateSale_WithMultipleItems_ShouldCalculateTotalCorrectly()
    {
        // Arrange
        var request = SaleIntegrationTestData.CreateSaleRequestWithMultipleItems(
            (3, 100m),  // 3 items @ 100 = 300 (no discount)
            (5, 50m),   // 5 items @ 50 = 250 * 0.90 = 225 (10% discount)
            (10, 20m)   // 10 items @ 20 = 200 * 0.80 = 160 (20% discount)
        );

        // Act
        var response = await PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await DeserializeResponseAsync<ApiResponseWithData<SaleTestResponse>>(response);
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
        
        var sale = result.Data!;
        sale.Items.Should().HaveCount(3);
        
        var expectedTotal = 300m + 225m + 160m; // = 685
        sale.TotalAmount.Should().Be(expectedTotal);
    }
}
