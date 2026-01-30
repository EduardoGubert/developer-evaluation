using System.Net;
using Ambev.DeveloperEvaluation.Integration.Fixtures;
using Ambev.DeveloperEvaluation.Integration.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

/// <summary>
/// Integration tests for deleting sales via the API.
/// </summary>
public class DeleteSaleIntegrationTests : IntegrationTestBase
{
    private const string BaseUrl = "/api/sales";

    public DeleteSaleIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    /// <summary>
    /// Tests that deleting a sale removes it from the database.
    /// </summary>
    [Fact(DisplayName = "DELETE /api/sales/{id} - Should delete sale")]
    public async Task DeleteSale_ShouldRemoveSale()
    {
        // Arrange - Create a sale
        var createRequest = SaleIntegrationTestData.CreateSaleRequest(quantity: 5, unitPrice: 100m);
        var createResponse = await PostAsJsonAsync(BaseUrl, createRequest);
        var createdSale = (await DeserializeResponseAsync<ApiResponseWithData<SaleTestResponse>>(createResponse))!.Data!;

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{createdSale.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await DeserializeResponseAsync<ApiResponse>(response);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();

        // Verify sale is deleted
        var getResponse = await Client.GetAsync($"{BaseUrl}/{createdSale.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that deleting a non-existent sale returns 404.
    /// </summary>
    [Fact(DisplayName = "DELETE /api/sales/{id} - Should return 404 when not found")]
    public async Task DeleteSale_WhenNotExists_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
