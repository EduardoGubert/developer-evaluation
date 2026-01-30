using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the UpdateCartHandler class.
/// </summary>
public class UpdateCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly UpdateCartHandler _handler;

    public UpdateCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateCartHandler(_cartRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid update returns the updated cart.
    /// </summary>
    [Fact(DisplayName = "Given valid update data When updating cart Then returns updated cart")]
    public async Task Handle_ValidUpdate_ReturnsUpdatedCart()
    {
        // Given
        var existingCart = CartTestData.GenerateValidCart();
        var newUserId = Guid.NewGuid();
        var newDate = DateTime.UtcNow.AddDays(1);
        var newProducts = new List<UpdateCartProductCommand>
        {
            new UpdateCartProductCommand { ProductId = Guid.NewGuid(), Quantity = 5 }
        };

        var command = new UpdateCartCommand
        {
            Id = existingCart.Id,
            UserId = newUserId,
            Date = newDate,
            Products = newProducts
        };

        _cartRepository.GetByIdAsync(existingCart.Id, Arg.Any<CancellationToken>())
            .Returns(existingCart);
        _cartRepository.CreateItemAsync(Arg.Any<CartItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<CartItem>());
        _cartRepository.UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Cart>());

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(existingCart.Id);
        result.UserId.Should().Be(newUserId);
        result.Products.Should().HaveCount(3); // 2 original + 1 new (merge)
        await _cartRepository.Received(1).CreateItemAsync(Arg.Any<CartItem>(), Arg.Any<CancellationToken>());
        await _cartRepository.Received(1).UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that updating a non-existent cart throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent cart ID When updating cart Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentCart_ThrowsKeyNotFoundException()
    {
        // Given
        var cartId = Guid.NewGuid();
        var command = new UpdateCartCommand
        {
            Id = cartId,
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = new List<UpdateCartProductCommand>
            {
                new UpdateCartProductCommand { ProductId = Guid.NewGuid(), Quantity = 1 }
            }
        };

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns((Cart?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{cartId}*not found*");
    }

    /// <summary>
    /// Tests that empty cart ID throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given empty cart ID When updating cart Then throws ValidationException")]
    public async Task Handle_EmptyCartId_ThrowsValidationException()
    {
        // Given
        var command = new UpdateCartCommand
        {
            Id = Guid.Empty,
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = new List<UpdateCartProductCommand>
            {
                new UpdateCartProductCommand { ProductId = Guid.NewGuid(), Quantity = 1 }
            }
        };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that new products are merged into existing cart (not replaced).
    /// </summary>
    [Fact(DisplayName = "Given new products When updating cart Then merges with existing products")]
    public async Task Handle_NewProducts_MergesWithExistingProducts()
    {
        // Given
        var existingCart = CartTestData.GenerateValidCart();
        var originalProductCount = existingCart.Products.Count; // 2
        var newProducts = new List<UpdateCartProductCommand>
        {
            new UpdateCartProductCommand { ProductId = Guid.NewGuid(), Quantity = 3 },
            new UpdateCartProductCommand { ProductId = Guid.NewGuid(), Quantity = 7 },
            new UpdateCartProductCommand { ProductId = Guid.NewGuid(), Quantity = 2 }
        };

        var command = new UpdateCartCommand
        {
            Id = existingCart.Id,
            UserId = existingCart.UserId,
            Date = existingCart.Date,
            Products = newProducts
        };

        _cartRepository.GetByIdAsync(existingCart.Id, Arg.Any<CancellationToken>())
            .Returns(existingCart);
        _cartRepository.CreateItemAsync(Arg.Any<CartItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<CartItem>());
        _cartRepository.UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Cart>());

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Products.Should().HaveCount(originalProductCount + 3); // merged
        await _cartRepository.Received(3).CreateItemAsync(Arg.Any<CartItem>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that updating an existing product updates its quantity instead of adding a duplicate.
    /// </summary>
    [Fact(DisplayName = "Given existing product When updating cart Then updates quantity")]
    public async Task Handle_ExistingProduct_UpdatesQuantity()
    {
        // Given
        var existingCart = CartTestData.GenerateValidCart();
        var existingProduct = existingCart.Products.First();
        var newQuantity = 15;

        var command = new UpdateCartCommand
        {
            Id = existingCart.Id,
            UserId = existingCart.UserId,
            Date = existingCart.Date,
            Products = new List<UpdateCartProductCommand>
            {
                new UpdateCartProductCommand { ProductId = existingProduct.ProductId, Quantity = newQuantity }
            }
        };

        _cartRepository.GetByIdAsync(existingCart.Id, Arg.Any<CancellationToken>())
            .Returns(existingCart);
        _cartRepository.UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Cart>());

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Products.Should().HaveCount(existingCart.Products.Count); // same count
        result.Products.First(p => p.ProductId == existingProduct.ProductId)
            .Quantity.Should().Be(newQuantity);
        await _cartRepository.DidNotReceive().CreateItemAsync(Arg.Any<CartItem>(), Arg.Any<CancellationToken>());
    }
}
