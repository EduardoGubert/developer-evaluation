using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the GetCartHandler class.
/// </summary>
public class GetCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly GetCartHandler _handler;

    public GetCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetCartHandler(_cartRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid cart ID returns the cart successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid cart ID When getting cart Then returns cart successfully")]
    public async Task Handle_ValidId_ReturnsCartSuccessfully()
    {
        // Given
        var cart = CartTestData.GenerateValidCart();
        var command = new GetCartCommand(cart.Id);

        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>())
            .Returns(cart);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(cart.Id);
        result.UserId.Should().Be(cart.UserId);
        result.Products.Should().HaveCount(cart.Products.Count);
        await _cartRepository.Received(1).GetByIdAsync(cart.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a non-existent cart ID throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent cart ID When getting cart Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentId_ThrowsKeyNotFoundException()
    {
        // Given
        var cartId = Guid.NewGuid();
        var command = new GetCartCommand(cartId);

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns((Cart?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{cartId}*not found*");
    }

    /// <summary>
    /// Tests that the cart products are mapped correctly.
    /// </summary>
    [Fact(DisplayName = "Given cart with products When getting cart Then returns cart with products")]
    public async Task Handle_CartWithProducts_ReturnsCartWithProducts()
    {
        // Given
        var cart = CartTestData.GenerateValidCart();
        var command = new GetCartCommand(cart.Id);

        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>())
            .Returns(cart);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Products.Should().NotBeEmpty();
        result.Products.Should().AllSatisfy(p =>
        {
            p.ProductId.Should().NotBeEmpty();
            p.Quantity.Should().BeGreaterThan(0);
        });
    }
}
