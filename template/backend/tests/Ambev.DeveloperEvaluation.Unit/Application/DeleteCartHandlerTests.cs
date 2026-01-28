using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the DeleteCartHandler class.
/// </summary>
public class DeleteCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly DeleteCartHandler _handler;

    public DeleteCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _handler = new DeleteCartHandler(_cartRepository);
    }

    /// <summary>
    /// Tests that a valid cart ID deletes the cart successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid cart ID When deleting cart Then returns success")]
    public async Task Handle_ValidId_DeletesSuccessfully()
    {
        // Given
        var cartId = Guid.NewGuid();
        var command = new DeleteCartCommand(cartId);

        _cartRepository.DeleteAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(true);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("deleted successfully");
        await _cartRepository.Received(1).DeleteAsync(cartId, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a non-existent cart ID throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent cart ID When deleting cart Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentId_ThrowsKeyNotFoundException()
    {
        // Given
        var cartId = Guid.NewGuid();
        var command = new DeleteCartCommand(cartId);

        _cartRepository.DeleteAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(false);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{cartId}*not found*");
    }

    /// <summary>
    /// Tests that the repository is called with the correct ID.
    /// </summary>
    [Fact(DisplayName = "Given cart ID When deleting cart Then repository receives correct ID")]
    public async Task Handle_ValidId_RepositoryReceivesCorrectId()
    {
        // Given
        var cartId = Guid.NewGuid();
        var command = new DeleteCartCommand(cartId);

        _cartRepository.DeleteAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(true);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _cartRepository.Received(1).DeleteAsync(cartId, Arg.Any<CancellationToken>());
    }
}
