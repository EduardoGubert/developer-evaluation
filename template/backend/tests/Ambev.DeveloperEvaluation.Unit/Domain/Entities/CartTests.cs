using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Cart entity class.
/// Tests cover cart operations, product management, and validation scenarios.
/// </summary>
public class CartTests
{
    /// <summary>
    /// Tests that a cart can be created with valid data.
    /// </summary>
    [Fact(DisplayName = "Cart should be created with valid data")]
    public void Given_ValidData_When_CreatingCart_Then_CartShouldBeCreated()
    {
        // Arrange & Act
        var cart = CartTestData.GenerateValidCart();

        // Assert
        Assert.NotEqual(Guid.Empty, cart.Id);
        Assert.NotEqual(Guid.Empty, cart.UserId);
        Assert.NotEmpty(cart.Products);
    }

    /// <summary>
    /// Tests that adding a product to a cart adds the product correctly.
    /// </summary>
    [Fact(DisplayName = "Adding product should add to cart")]
    public void Given_Cart_When_AddingProduct_Then_ProductShouldBeAdded()
    {
        // Arrange
        var cart = new Cart
        {
            UserId = CartTestData.GenerateValidUserId(),
            Date = DateTime.UtcNow
        };
        var productId = CartTestData.GenerateValidProductId();
        var quantity = 5;

        // Act
        var item = cart.AddProduct(productId, quantity);

        // Assert
        Assert.Single(cart.Products);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(quantity, item.Quantity);
    }

    /// <summary>
    /// Tests that adding an existing product increases the quantity.
    /// </summary>
    [Fact(DisplayName = "Adding existing product should increase quantity")]
    public void Given_CartWithProduct_When_AddingSameProduct_Then_QuantityShouldIncrease()
    {
        // Arrange
        var cart = new Cart
        {
            UserId = CartTestData.GenerateValidUserId(),
            Date = DateTime.UtcNow
        };
        var productId = CartTestData.GenerateValidProductId();

        // Act
        cart.AddProduct(productId, 3);
        cart.AddProduct(productId, 2);

        // Assert
        Assert.Single(cart.Products);
        Assert.Equal(5, cart.Products.First().Quantity);
    }

    /// <summary>
    /// Tests that removing a product from the cart removes it correctly.
    /// </summary>
    [Fact(DisplayName = "Removing product should remove from cart")]
    public void Given_CartWithProducts_When_RemovingProduct_Then_ProductShouldBeRemoved()
    {
        // Arrange
        var cart = new Cart
        {
            UserId = CartTestData.GenerateValidUserId(),
            Date = DateTime.UtcNow
        };
        var productId1 = CartTestData.GenerateValidProductId();
        var productId2 = CartTestData.GenerateValidProductId();
        cart.AddProduct(productId1, 3);
        cart.AddProduct(productId2, 2);

        // Act
        var result = cart.RemoveProduct(productId1);

        // Assert
        Assert.True(result);
        Assert.Single(cart.Products);
        Assert.Equal(productId2, cart.Products.First().ProductId);
    }

    /// <summary>
    /// Tests that removing a non-existent product returns false.
    /// </summary>
    [Fact(DisplayName = "Removing non-existent product should return false")]
    public void Given_Cart_When_RemovingNonExistentProduct_Then_ShouldReturnFalse()
    {
        // Arrange
        var cart = new Cart
        {
            UserId = CartTestData.GenerateValidUserId(),
            Date = DateTime.UtcNow
        };
        cart.AddProduct(CartTestData.GenerateValidProductId(), 3);

        // Act
        var result = cart.RemoveProduct(Guid.NewGuid());

        // Assert
        Assert.False(result);
        Assert.Single(cart.Products);
    }

    /// <summary>
    /// Tests that updating a cart changes its properties.
    /// </summary>
    [Fact(DisplayName = "Updating cart should change its properties")]
    public void Given_Cart_When_Updating_Then_PropertiesShouldBeChanged()
    {
        // Arrange
        var cart = CartTestData.GenerateValidCart();
        var newUserId = Guid.NewGuid();
        var newDate = DateTime.UtcNow.AddDays(1);

        // Act
        cart.Update(newUserId, newDate);

        // Assert
        Assert.Equal(newUserId, cart.UserId);
        Assert.Equal(newDate, cart.Date);
        Assert.NotNull(cart.UpdatedAt);
    }

    /// <summary>
    /// Tests that validation passes for valid cart data.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for valid cart data")]
    public void Given_ValidCartData_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var cart = CartTestData.GenerateValidCart();

        // Act
        var result = cart.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when user ID is empty.
    /// </summary>
    [Fact(DisplayName = "Validation should fail when user ID is empty")]
    public void Given_EmptyUserId_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var cart = new Cart
        {
            UserId = Guid.Empty,
            Date = DateTime.UtcNow
        };
        cart.AddProduct(CartTestData.GenerateValidProductId(), 1);

        // Act
        var result = cart.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>
    /// Tests that CreatedAt is set automatically.
    /// </summary>
    [Fact(DisplayName = "CreatedAt should be set on initialization")]
    public void Given_NewCart_When_Created_Then_CreatedAtShouldBeSet()
    {
        // Arrange & Act
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);
        var cart = new Cart();
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(cart.CreatedAt >= beforeCreation);
        Assert.True(cart.CreatedAt <= afterCreation);
    }
}
