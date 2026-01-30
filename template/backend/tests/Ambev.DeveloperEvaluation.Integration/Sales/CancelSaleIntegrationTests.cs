using System.Net;
using Ambev.DeveloperEvaluation.Integration.Fixtures;
using Ambev.DeveloperEvaluation.Integration.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

/// <summary>
/// Integration tests for cancelling sales via the API.
/// </summary>
public class CancelSaleIntegrationTests : IntegrationTestBase
{
    private const string BaseUrl = "/api/sales";

    public CancelSaleIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    /// <summary>
    /// Tests that cancelling a sale sets IsCancelled to true.
    /// </summary>
    [Fact(DisplayName = "PATCH /api/sales/{id}/cancel - Should cancel sale")]
    public async Task CancelSale_ShouldSetIsCancelledToTrue()
    {
        // Arrange - Create a sale
        var createRequest = SaleIntegrationTestData.CreateSaleRequest(quantity: 5, unitPrice: 100m);
        var createResponse = await PostAsJsonAsync(BaseUrl, createRequest);
        var createdSale = (await DeserializeResponseAsync<ApiResponseWithData<SaleTestResponse>>(createResponse))!.Data!;
        
        createdSale.IsCancelled.Should().BeFalse();

        // Act
        var response = await Client.PatchAsync($"{BaseUrl}/{createdSale.Id}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await DeserializeResponseAsync<ApiResponseWithData<SaleTestResponse>>(response);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();

        // Verify sale is cancelled
        var getResponse = await Client.GetAsync($"{BaseUrl}/{createdSale.Id}");
        var getSale = (await DeserializeResponseAsync<ApiResponseWithData<SaleTestResponse>>(getResponse))!.Data!;
        getSale.IsCancelled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that cancelling a non-existent sale returns 404.
    /// </summary>
    [Fact(DisplayName = "PATCH /api/sales/{id}/cancel - Should return 404 when not found")]
    public async Task CancelSale_WhenNotExists_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.PatchAsync($"{BaseUrl}/{nonExistentId}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that cancelling an already cancelled sale returns error.
    /// </summary>
    [Fact(DisplayName = "PATCH /api/sales/{id}/cancel - Should return error when already cancelled")]
    public async Task CancelSale_WhenAlreadyCancelled_ShouldReturnBadRequest()
    {
        // Arrange - Create and cancel a sale
        var createRequest = SaleIntegrationTestData.CreateSaleRequest(quantity: 5, unitPrice: 100m);
        var createResponse = await PostAsJsonAsync(BaseUrl, createRequest);
        var createdSale = (await DeserializeResponseAsync<ApiResponseWithData<SaleTestResponse>>(createResponse))!.Data!;
        
        await Client.PatchAsync($"{BaseUrl}/{createdSale.Id}/cancel", null);

        // Act - Try to cancel again
        var response = await Client.PatchAsync($"{BaseUrl}/{createdSale.Id}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
