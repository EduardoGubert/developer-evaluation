using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Product entity class.
/// Tests cover product creation, update, and validation scenarios.
/// </summary>
public class ProductTests
{
    /// <summary>
    /// Tests that a product can be created with valid data.
    /// </summary>
    [Fact(DisplayName = "Product should be created with valid data")]
    public void Given_ValidData_When_CreatingProduct_Then_ProductShouldBeCreated()
    {
        // Arrange & Act
        var product = ProductTestData.GenerateValidProduct();

        // Assert
        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.NotEmpty(product.Title);
        Assert.True(product.Price > 0);
        Assert.NotEmpty(product.Description);
        Assert.NotEmpty(product.Category);
    }

    /// <summary>
    /// Tests that updating a product changes its properties.
    /// </summary>
    [Fact(DisplayName = "Updating product should change its properties")]
    public void Given_Product_When_Updating_Then_PropertiesShouldBeChanged()
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        var newTitle = "Updated Product Title";
        var newPrice = 99.99m;
        var newDescription = "Updated description";
        var newCategory = "New Category";
        var newImage = "https://example.com/new-image.jpg";
        var newRatingRate = 4.5m;
        var newRatingCount = 100;

        // Act
        product.Update(newTitle, newPrice, newDescription, newCategory, newImage, newRatingRate, newRatingCount);

        // Assert
        Assert.Equal(newTitle, product.Title);
        Assert.Equal(newPrice, product.Price);
        Assert.Equal(newDescription, product.Description);
        Assert.Equal(newCategory, product.Category);
        Assert.Equal(newImage, product.Image);
        Assert.Equal(newRatingRate, product.RatingRate);
        Assert.Equal(newRatingCount, product.RatingCount);
        Assert.NotNull(product.UpdatedAt);
    }

    /// <summary>
    /// Tests that validation passes for valid product data.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for valid product data")]
    public void Given_ValidProductData_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();

        // Act
        var result = product.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when product title is empty.
    /// </summary>
    [Fact(DisplayName = "Validation should fail when title is empty")]
    public void Given_EmptyTitle_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var product = new Product
        {
            Title = "",
            Price = 100m,
            Description = ProductTestData.GenerateValidDescription(),
            Category = ProductTestData.GenerateValidCategory(),
            Image = ProductTestData.GenerateValidImage(),
            RatingRate = 4.5m,
            RatingCount = 10
        };

        // Act
        var result = product.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when price is negative.
    /// </summary>
    [Fact(DisplayName = "Validation should fail when price is negative")]
    public void Given_NegativePrice_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var product = new Product
        {
            Title = ProductTestData.GenerateValidTitle(),
            Price = -10m,
            Description = ProductTestData.GenerateValidDescription(),
            Category = ProductTestData.GenerateValidCategory(),
            Image = ProductTestData.GenerateValidImage(),
            RatingRate = 4.5m,
            RatingCount = 10
        };

        // Act
        var result = product.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when category is empty.
    /// </summary>
    [Fact(DisplayName = "Validation should fail when category is empty")]
    public void Given_EmptyCategory_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var product = new Product
        {
            Title = ProductTestData.GenerateValidTitle(),
            Price = 100m,
            Description = ProductTestData.GenerateValidDescription(),
            Category = "",
            Image = ProductTestData.GenerateValidImage(),
            RatingRate = 4.5m,
            RatingCount = 10
        };

        // Act
        var result = product.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>
    /// Tests that CreatedAt is set automatically.
    /// </summary>
    [Fact(DisplayName = "CreatedAt should be set on initialization")]
    public void Given_NewProduct_When_Created_Then_CreatedAtShouldBeSet()
    {
        // Arrange & Act
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);
        var product = new Product();
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(product.CreatedAt >= beforeCreation);
        Assert.True(product.CreatedAt <= afterCreation);
    }
}
