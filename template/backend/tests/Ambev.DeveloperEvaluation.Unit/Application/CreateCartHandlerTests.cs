using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the CreateCartHandler class.
/// </summary>
public class CreateCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly CreateCartHandler _handler;

    public CreateCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateCartHandler(_cartRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid cart creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid cart data When creating cart Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateCartHandlerTestData.GenerateValidCommand();
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Date = command.Date
        };

        foreach (var product in command.Products)
        {
            cart.AddProduct(product.ProductId, product.Quantity);
        }

        _cartRepository.CreateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
            .Returns(cart);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(cart.Id);
        result.UserId.Should().Be(command.UserId);
        result.Products.Should().HaveCount(command.Products.Count);
        await _cartRepository.Received(1).CreateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid cart creation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid cart data When creating cart Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateCartCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that creating a cart with empty user ID throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given empty user ID When creating cart Then throws validation exception")]
    public async Task Handle_EmptyUserId_ThrowsValidationException()
    {
        // Given
        var command = CreateCartHandlerTestData.GenerateInvalidCommandWithEmptyUserId();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that creating a cart with no products throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given no products When creating cart Then throws validation exception")]
    public async Task Handle_NoProducts_ThrowsValidationException()
    {
        // Given
        var command = CreateCartHandlerTestData.GenerateInvalidCommandWithNoProducts();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that creating a cart with invalid product quantity throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid product quantity When creating cart Then throws validation exception")]
    public async Task Handle_InvalidQuantity_ThrowsValidationException()
    {
        // Given
        var command = CreateCartHandlerTestData.GenerateInvalidCommandWithInvalidQuantity();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that products are added to the cart correctly.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then products are added to cart")]
    public async Task Handle_ValidRequest_AddsProductsToCart()
    {
        // Given
        var command = CreateCartHandlerTestData.GenerateValidCommand();
        Cart? capturedCart = null;

        _cartRepository.CreateAsync(Arg.Do<Cart>(c => capturedCart = c), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cart>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedCart.Should().NotBeNull();
        capturedCart!.Products.Should().HaveCount(command.Products.Count);
    }
}
