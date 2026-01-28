using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the DeleteProductHandler class.
/// </summary>
public class DeleteProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly DeleteProductHandler _handler;

    public DeleteProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _handler = new DeleteProductHandler(_productRepository);
    }

    /// <summary>
    /// Tests that a valid product ID deletes the product successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid product ID When deleting product Then returns success")]
    public async Task Handle_ValidId_DeletesSuccessfully()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);

        _productRepository.DeleteAsync(productId, Arg.Any<CancellationToken>())
            .Returns(true);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("deleted successfully");
        await _productRepository.Received(1).DeleteAsync(productId, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a non-existent product ID throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent product ID When deleting product Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentId_ThrowsKeyNotFoundException()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);

        _productRepository.DeleteAsync(productId, Arg.Any<CancellationToken>())
            .Returns(false);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{productId}*not found*");
    }

    /// <summary>
    /// Tests that the repository is called with the correct ID.
    /// </summary>
    [Fact(DisplayName = "Given product ID When deleting product Then repository receives correct ID")]
    public async Task Handle_ValidId_RepositoryReceivesCorrectId()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);

        _productRepository.DeleteAsync(productId, Arg.Any<CancellationToken>())
            .Returns(true);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).DeleteAsync(productId, Arg.Any<CancellationToken>());
    }
}
