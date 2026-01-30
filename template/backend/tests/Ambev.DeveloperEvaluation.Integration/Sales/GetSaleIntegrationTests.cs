using System.Net;
using Ambev.DeveloperEvaluation.Integration.Fixtures;
using Ambev.DeveloperEvaluation.Integration.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

/// <summary>
/// Integration tests for retrieving sales via the API.
/// </summary>
public class GetSaleIntegrationTests : IntegrationTestBase
{
    private const string BaseUrl = "/api/sales";

    public GetSaleIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    /// <summary>
    /// Tests that getting a sale by ID returns the correct sale.
    /// </summary>
    [Fact(DisplayName = "GET /api/sales/{id} - Should return sale when exists")]
    public async Task GetSale_WhenExists_ShouldReturnSale()
    {
        // Arrange - Create a sale first
        var createRequest = SaleIntegrationTestData.CreateSaleRequest(quantity: 5, unitPrice: 100m);
        var createResponse = await PostAsJsonAsync(BaseUrl, createRequest);
        var createdSale = (await DeserializeResponseAsync<ApiResponseWithData<SaleTestResponse>>(createResponse))!.Data!;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{createdSale.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await DeserializeResponseAsync<ApiResponseWithData<SaleTestResponse>>(response);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        
        var sale = result.Data!;
        sale.Id.Should().Be(createdSale.Id);
        sale.SaleNumber.Should().Be(createdSale.SaleNumber);
        sale.CustomerId.Should().Be(createdSale.CustomerId);
        sale.CustomerName.Should().Be(createdSale.CustomerName);
        sale.BranchId.Should().Be(createdSale.BranchId);
        sale.BranchName.Should().Be(createdSale.BranchName);
        sale.TotalAmount.Should().Be(createdSale.TotalAmount);
        sale.Items.Should().HaveCount(1);
    }

    /// <summary>
    /// Tests that getting a non-existent sale returns 404.
    /// </summary>
    [Fact(DisplayName = "GET /api/sales/{id} - Should return 404 when not found")]
    public async Task GetSale_WhenNotExists_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
