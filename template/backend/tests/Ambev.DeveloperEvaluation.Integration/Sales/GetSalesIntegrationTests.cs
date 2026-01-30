using System.Net;
using Ambev.DeveloperEvaluation.Integration.Fixtures;
using Ambev.DeveloperEvaluation.Integration.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

/// <summary>
/// Integration tests for listing sales with pagination via the API.
/// </summary>
public class GetSalesIntegrationTests : IntegrationTestBase
{
    private const string BaseUrl = "/api/sales";

    public GetSalesIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    /// <summary>
    /// Tests that listing sales returns paginated results.
    /// </summary>
    [Fact(DisplayName = "GET /api/sales - Should return paginated sales")]
    public async Task GetSales_ShouldReturnPaginatedResults()
    {
        // Arrange - Create multiple sales
        for (int i = 0; i < 3; i++)
        {
            var request = SaleIntegrationTestData.CreateSaleRequest(quantity: 2);
            await PostAsJsonAsync(BaseUrl, request);
        }

        // Act
        var response = await Client.GetAsync($"{BaseUrl}?_page=1&_size=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await DeserializeResponseAsync<ApiResponseWithData<GetSalesTestResponse>>(response);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Data.Should().NotBeEmpty();
        result.Data.TotalItems.Should().BeGreaterOrEqualTo(3);
        result.Data.CurrentPage.Should().Be(1);
    }

    /// <summary>
    /// Tests that pagination parameters work correctly.
    /// </summary>
    [Fact(DisplayName = "GET /api/sales - Pagination should limit results")]
    public async Task GetSales_WithPagination_ShouldLimitResults()
    {
        // Arrange - Create 5 sales
        for (int i = 0; i < 5; i++)
        {
            var request = SaleIntegrationTestData.CreateSaleRequest(quantity: 2);
            await PostAsJsonAsync(BaseUrl, request);
        }

        // Act - Get only 2 per page
        var response = await Client.GetAsync($"{BaseUrl}?_page=1&_size=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await DeserializeResponseAsync<ApiResponseWithData<GetSalesTestResponse>>(response);
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
        result.Data!.Data.Count.Should().BeLessOrEqualTo(2);
    }

    /// <summary>
    /// Tests that ordering parameter works correctly.
    /// </summary>
    [Fact(DisplayName = "GET /api/sales - Should support ordering")]
    public async Task GetSales_WithOrdering_ShouldReturnOrderedResults()
    {
        // Arrange
        var request = SaleIntegrationTestData.CreateSaleRequest(quantity: 2);
        await PostAsJsonAsync(BaseUrl, request);

        // Act
        var response = await Client.GetAsync($"{BaseUrl}?_page=1&_size=10&_order=saleDate desc");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await DeserializeResponseAsync<ApiResponseWithData<GetSalesTestResponse>>(response);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }
}
