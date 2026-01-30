using Ambev.DeveloperEvaluation.Application.Carts.CheckoutCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using ValidationException = FluentValidation.ValidationException;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the CheckoutCartHandler class.
/// Tests cover success scenarios (200/201), validation errors (400),
/// not found errors (404), and server errors (500).
/// </summary>
public class CheckoutCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IEventStore _eventStore;
    private readonly ILogger<CheckoutCartHandler> _logger;
    private readonly CheckoutCartHandler _handler;
    private readonly Faker _faker;

    public CheckoutCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventStore = Substitute.For<IEventStore>();
        _logger = Substitute.For<ILogger<CheckoutCartHandler>>();
        _handler = new CheckoutCartHandler(
            _cartRepository,
            _productRepository,
            _userRepository,
            _saleRepository,
            _eventStore,
            _logger);
        _faker = new Faker();
    }

    #region Success Tests (200/201)

    /// <summary>
    /// Tests that a valid checkout request is handled successfully and returns sale details.
    /// </summary>
    [Fact(DisplayName = "Given valid cart When checkout Then returns success with sale details")]
    public async Task Handle_ValidCart_ReturnsSuccessWithSaleDetails()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { Id = command.CartId, UserId = userId };
        cart.AddProduct(productId, 2);

        var user = new User
        {
            Id = userId,
            Firstname = _faker.Name.FirstName(),
            Lastname = _faker.Name.LastName(),
            Email = _faker.Internet.Email(),
            Username = _faker.Internet.UserName(),
            Status = UserStatus.Active
        };

        var product = new Product
        {
            Id = productId,
            Title = _faker.Commerce.ProductName(),
            Price = 100m
        };

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.SaleId.Should().NotBeEmpty();
        result.SaleNumber.Should().StartWith("SALE-");
        result.Items.Should().HaveCount(1);
        result.Items.First().ProductId.Should().Be(productId);
        result.Items.First().Quantity.Should().Be(2);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _cartRepository.Received(1).DeleteAsync(command.CartId, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that event is published when checkout is successful.
    /// </summary>
    [Fact(DisplayName = "Given successful checkout When completed Then publishes SaleCreated event")]
    public async Task Handle_SuccessfulCheckout_PublishesSaleCreatedEvent()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { Id = command.CartId, UserId = userId };
        cart.AddProduct(productId, 3);

        var user = new User
        {
            Id = userId,
            Firstname = _faker.Name.FirstName(),
            Lastname = _faker.Name.LastName()
        };

        var product = new Product { Id = productId, Title = "Test Product", Price = 50m };

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _eventStore.Received(1).AppendAsync(
            "SaleCreated",
            Arg.Any<string>(),
            "Sale",
            Arg.Any<object>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that 10% discount is applied for 4-9 items of the same product.
    /// </summary>
    [Fact(DisplayName = "Given 4-9 items When checkout Then applies 10% discount")]
    public async Task Handle_4To9Items_Applies10PercentDiscount()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { Id = command.CartId, UserId = userId };
        cart.AddProduct(productId, 5); // 5 items = 10% discount

        var user = new User { Id = userId, Firstname = "Test", Lastname = "User" };
        var product = new Product { Id = productId, Title = "Test", Price = 100m };

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });

        Sale? capturedSale = null;
        _saleRepository.CreateAsync(Arg.Do<Sale>(s => capturedSale = s), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedSale.Should().NotBeNull();
        capturedSale!.Items.First().Discount.Should().Be(0.10m);
    }

    /// <summary>
    /// Tests that 20% discount is applied for 10-20 items of the same product.
    /// </summary>
    [Fact(DisplayName = "Given 10-20 items When checkout Then applies 20% discount")]
    public async Task Handle_10To20Items_Applies20PercentDiscount()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { Id = command.CartId, UserId = userId };
        cart.AddProduct(productId, 15); // 15 items = 20% discount

        var user = new User { Id = userId, Firstname = "Test", Lastname = "User" };
        var product = new Product { Id = productId, Title = "Test", Price = 100m };

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });

        Sale? capturedSale = null;
        _saleRepository.CreateAsync(Arg.Do<Sale>(s => capturedSale = s), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedSale.Should().NotBeNull();
        capturedSale!.Items.First().Discount.Should().Be(0.20m);
    }

    /// <summary>
    /// Tests that no discount is applied for less than 4 items.
    /// </summary>
    [Fact(DisplayName = "Given less than 4 items When checkout Then no discount applied")]
    public async Task Handle_LessThan4Items_NoDiscount()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { Id = command.CartId, UserId = userId };
        cart.AddProduct(productId, 2); // 2 items = no discount

        var user = new User { Id = userId, Firstname = "Test", Lastname = "User" };
        var product = new Product { Id = productId, Title = "Test", Price = 100m };

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });

        Sale? capturedSale = null;
        _saleRepository.CreateAsync(Arg.Do<Sale>(s => capturedSale = s), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedSale.Should().NotBeNull();
        capturedSale!.Items.First().Discount.Should().Be(0m);
    }

    /// <summary>
    /// Tests that cart is deleted after successful checkout.
    /// </summary>
    [Fact(DisplayName = "Given successful checkout When completed Then deletes cart")]
    public async Task Handle_SuccessfulCheckout_DeletesCart()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { Id = command.CartId, UserId = userId };
        cart.AddProduct(productId, 1);

        var user = new User { Id = userId, Firstname = "Test", Lastname = "User" };
        var product = new Product { Id = productId, Title = "Test", Price = 10m };

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _cartRepository.Received(1).DeleteAsync(command.CartId, Arg.Any<CancellationToken>());
    }

    #endregion

    #region Validation Error Tests (400)

    /// <summary>
    /// Tests that empty CartId throws ValidationException.
    /// </summary>
    [Fact(DisplayName = "Given empty CartId When checkout Then throws ValidationException")]
    public async Task Handle_EmptyCartId_ThrowsValidationException()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateInvalidCommandWithEmptyCartId();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that empty BranchId throws ValidationException.
    /// </summary>
    [Fact(DisplayName = "Given empty BranchId When checkout Then throws ValidationException")]
    public async Task Handle_EmptyBranchId_ThrowsValidationException()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateInvalidCommandWithEmptyBranchId();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that empty BranchName throws ValidationException.
    /// </summary>
    [Fact(DisplayName = "Given empty BranchName When checkout Then throws ValidationException")]
    public async Task Handle_EmptyBranchName_ThrowsValidationException()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateInvalidCommandWithEmptyBranchName();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that BranchName exceeding max length throws ValidationException.
    /// </summary>
    [Fact(DisplayName = "Given BranchName too long When checkout Then throws ValidationException")]
    public async Task Handle_BranchNameTooLong_ThrowsValidationException()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateInvalidCommandWithBranchNameTooLong();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that empty cart throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given empty cart When checkout Then throws InvalidOperationException")]
    public async Task Handle_EmptyCart_ThrowsInvalidOperationException()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();
        var cart = new Cart { Id = command.CartId, UserId = Guid.NewGuid() };
        // Cart has no products

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*empty*");
    }

    #endregion

    #region Not Found Error Tests (404)

    /// <summary>
    /// Tests that non-existent cart throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent cart When checkout Then throws KeyNotFoundException")]
    public async Task Handle_CartNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns((Cart?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{command.CartId}*");
    }

    /// <summary>
    /// Tests that non-existent user throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent user When checkout Then throws KeyNotFoundException")]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { Id = command.CartId, UserId = userId };
        cart.AddProduct(productId, 1);

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{userId}*");
    }

    /// <summary>
    /// Tests that non-existent product throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent product When checkout Then throws KeyNotFoundException")]
    public async Task Handle_ProductNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { Id = command.CartId, UserId = userId };
        cart.AddProduct(productId, 2);

        var user = new User { Id = userId, Firstname = "Test", Lastname = "User" };

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product>()); // Product not found

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{productId}*");
    }

    #endregion

    #region Server Error Tests (500)

    /// <summary>
    /// Tests that repository exception propagates as unhandled exception.
    /// </summary>
    [Fact(DisplayName = "Given repository failure When checkout Then throws exception")]
    public async Task Handle_RepositoryFailure_ThrowsException()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { Id = command.CartId, UserId = userId };
        cart.AddProduct(productId, 1);

        var user = new User { Id = userId, Firstname = "Test", Lastname = "User" };
        var product = new Product { Id = productId, Title = "Test", Price = 10m };

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Database connection failed"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*Database connection failed*");
    }

    /// <summary>
    /// Tests that event store failure propagates as unhandled exception.
    /// </summary>
    [Fact(DisplayName = "Given event store failure When checkout Then throws exception")]
    public async Task Handle_EventStoreFailure_ThrowsException()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { Id = command.CartId, UserId = userId };
        cart.AddProduct(productId, 1);

        var user = new User { Id = userId, Firstname = "Test", Lastname = "User" };
        var product = new Product { Id = productId, Title = "Test", Price = 10m };

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());
        _eventStore.AppendAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<object>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>())
            .Throws(new Exception("MongoDB connection failed"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*MongoDB connection failed*");
    }

    #endregion

    #region Multiple Products Tests

    /// <summary>
    /// Tests checkout with multiple different products.
    /// </summary>
    [Fact(DisplayName = "Given multiple products When checkout Then creates sale with all items")]
    public async Task Handle_MultipleProducts_CreatesSaleWithAllItems()
    {
        // Given
        var command = CheckoutCartHandlerTestData.GenerateValidCommand();
        var userId = Guid.NewGuid();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var cart = new Cart { Id = command.CartId, UserId = userId };
        cart.AddProduct(productId1, 2);
        cart.AddProduct(productId2, 3);

        var user = new User { Id = userId, Firstname = "Test", Lastname = "User" };
        var product1 = new Product { Id = productId1, Title = "Product 1", Price = 100m };
        var product2 = new Product { Id = productId2, Title = "Product 2", Price = 50m };

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product1, product2 });

        Sale? capturedSale = null;
        _saleRepository.CreateAsync(Arg.Do<Sale>(s => capturedSale = s), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Items.Should().HaveCount(2);
        capturedSale.Should().NotBeNull();
        capturedSale!.Items.Should().HaveCount(2);
    }

    #endregion
}
