using Ambev.DeveloperEvaluation.Application.Carts.RemoveCartItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the RemoveCartItemHandler class.
/// </summary>
public class RemoveCartItemHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly RemoveCartItemHandler _handler;

    public RemoveCartItemHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _handler = new RemoveCartItemHandler(_cartRepository);
    }

    [Fact(DisplayName = "Given valid cart and product When removing item Then returns success")]
    public async Task Handle_ValidCartAndProduct_ReturnsSuccess()
    {
        // Given
        var cartId = Guid.NewGuid();
        var productIdA = Guid.NewGuid();
        var productIdB = Guid.NewGuid();

        var cart = new Cart { Id = cartId, UserId = Guid.NewGuid() };
        cart.AddProduct(productIdA, 2);
        cart.AddProduct(productIdB, 3);

        var command = new RemoveCartItemCommand(cartId, productIdA);

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(cart);
        _cartRepository.DeleteItemAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.CartId.Should().Be(cartId);
        result.ProductId.Should().Be(productIdA);
        await _cartRepository.Received(1).DeleteItemAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given empty cart ID When removing item Then throws ValidationException")]
    public async Task Handle_EmptyCartId_ThrowsValidationException()
    {
        // Given
        var command = new RemoveCartItemCommand(Guid.Empty, Guid.NewGuid());

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given empty product ID When removing item Then throws ValidationException")]
    public async Task Handle_EmptyProductId_ThrowsValidationException()
    {
        // Given
        var command = new RemoveCartItemCommand(Guid.NewGuid(), Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given non-existent cart When removing item Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentCart_ThrowsKeyNotFoundException()
    {
        // Given
        var cartId = Guid.NewGuid();
        var command = new RemoveCartItemCommand(cartId, Guid.NewGuid());

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns((Cart?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{cartId}*");
    }

    [Fact(DisplayName = "Given product not in cart When removing item Then throws KeyNotFoundException")]
    public async Task Handle_ProductNotInCart_ThrowsKeyNotFoundException()
    {
        // Given
        var cartId = Guid.NewGuid();
        var existingProductId = Guid.NewGuid();
        var nonExistentProductId = Guid.NewGuid();

        var cart = new Cart { Id = cartId, UserId = Guid.NewGuid() };
        cart.AddProduct(existingProductId, 2);

        var command = new RemoveCartItemCommand(cartId, nonExistentProductId);

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(cart);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{nonExistentProductId}*");
    }
}
