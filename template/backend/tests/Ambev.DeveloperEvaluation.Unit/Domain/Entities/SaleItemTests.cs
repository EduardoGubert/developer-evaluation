using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the SaleItem entity class.
/// Tests cover discount calculation based on quantity rules:
/// - Less than 4 items: 0% discount
/// - 4 to 9 items: 10% discount
/// - 10 to 20 items: 20% discount
/// - More than 20 items: Not allowed
/// </summary>
public class SaleItemTests
{
    /// <summary>
    /// Tests that items with quantity less than 4 have no discount.
    /// </summary>
    [Theory(DisplayName = "Items with quantity less than 4 should have no discount")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Given_QuantityLessThan4_When_CalculatingDiscount_Then_DiscountShouldBeZero(int quantity)
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = SaleTestData.GenerateValidProductName(),
            Quantity = quantity,
            UnitPrice = 100m
        };

        // Act
        item.CalculateDiscount();

        // Assert
        Assert.Equal(0m, item.Discount);
        Assert.Equal(quantity * 100m, item.TotalAmount);
    }

    /// <summary>
    /// Tests that items with quantity between 4 and 9 have 10% discount.
    /// </summary>
    [Theory(DisplayName = "Items with quantity 4-9 should have 10% discount")]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void Given_QuantityBetween4And9_When_CalculatingDiscount_Then_DiscountShouldBe10Percent(int quantity)
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = SaleTestData.GenerateValidProductName(),
            Quantity = quantity,
            UnitPrice = 100m
        };

        // Act
        item.CalculateDiscount();

        // Assert
        Assert.Equal(0.10m, item.Discount);
        var expectedTotal = (quantity * 100m) * 0.90m; // 10% off
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    /// <summary>
    /// Tests that items with quantity between 10 and 20 have 20% discount.
    /// </summary>
    [Theory(DisplayName = "Items with quantity 10-20 should have 20% discount")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void Given_QuantityBetween10And20_When_CalculatingDiscount_Then_DiscountShouldBe20Percent(int quantity)
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = SaleTestData.GenerateValidProductName(),
            Quantity = quantity,
            UnitPrice = 100m
        };

        // Act
        item.CalculateDiscount();

        // Assert
        Assert.Equal(0.20m, item.Discount);
        var expectedTotal = (quantity * 100m) * 0.80m; // 20% off
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    /// <summary>
    /// Tests that items with quantity greater than 20 throw an exception.
    /// </summary>
    [Theory(DisplayName = "Items with quantity greater than 20 should throw exception")]
    [InlineData(21)]
    [InlineData(50)]
    [InlineData(100)]
    public void Given_QuantityGreaterThan20_When_CalculatingDiscount_Then_ShouldThrowException(int quantity)
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = SaleTestData.GenerateValidProductName(),
            Quantity = quantity,
            UnitPrice = 100m
        };

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => item.CalculateDiscount());
        Assert.Contains("20", exception.Message);
    }

    /// <summary>
    /// Tests that cancelling an item sets the IsCancelled flag.
    /// </summary>
    [Fact(DisplayName = "Item should be marked as cancelled when cancelled")]
    public void Given_ActiveItem_When_Cancelled_Then_ShouldBeMarkedAsCancelled()
    {
        // Arrange
        var item = SaleTestData.GenerateValidSaleItem();
        Assert.False(item.IsCancelled);

        // Act
        item.Cancel();

        // Assert
        Assert.True(item.IsCancelled);
    }

    /// <summary>
    /// Tests that total amount is correctly calculated.
    /// </summary>
    [Fact(DisplayName = "Total amount should be correctly calculated")]
    public void Given_ItemWithPriceAndQuantity_When_CalculatingTotal_Then_ShouldBeCorrect()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = SaleTestData.GenerateValidProductName(),
            Quantity = 5,
            UnitPrice = 50m
        };

        // Act
        item.CalculateDiscount();

        // Assert
        // 5 items * 50 = 250, with 10% discount = 225
        Assert.Equal(225m, item.TotalAmount);
    }

    /// <summary>
    /// Tests that update method recalculates discount and total.
    /// </summary>
    [Fact(DisplayName = "Update should recalculate discount and total")]
    public void Given_ExistingItem_When_Updated_Then_ShouldRecalculate()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = SaleTestData.GenerateValidProductName(),
            Quantity = 2,
            UnitPrice = 100m
        };
        item.CalculateDiscount();
        Assert.Equal(0m, item.Discount);

        // Act
        item.Update(5, 100m);

        // Assert
        Assert.Equal(0.10m, item.Discount);
        Assert.Equal(450m, item.TotalAmount); // 5 * 100 * 0.90
    }

    /// <summary>
    /// Tests the boundary case at exactly 4 items (start of 10% discount).
    /// </summary>
    [Fact(DisplayName = "Exactly 4 items should get 10% discount")]
    public void Given_Exactly4Items_When_CalculatingDiscount_Then_ShouldGet10PercentDiscount()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = SaleTestData.GenerateValidProductName(),
            Quantity = 4,
            UnitPrice = 100m
        };

        // Act
        item.CalculateDiscount();

        // Assert
        Assert.Equal(0.10m, item.Discount);
        Assert.Equal(360m, item.TotalAmount); // 4 * 100 * 0.90 = 360
    }

    /// <summary>
    /// Tests the boundary case at exactly 10 items (start of 20% discount).
    /// </summary>
    [Fact(DisplayName = "Exactly 10 items should get 20% discount")]
    public void Given_Exactly10Items_When_CalculatingDiscount_Then_ShouldGet20PercentDiscount()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = SaleTestData.GenerateValidProductName(),
            Quantity = 10,
            UnitPrice = 100m
        };

        // Act
        item.CalculateDiscount();

        // Assert
        Assert.Equal(0.20m, item.Discount);
        Assert.Equal(800m, item.TotalAmount); // 10 * 100 * 0.80 = 800
    }

    /// <summary>
    /// Tests the boundary case at exactly 20 items (last valid quantity).
    /// </summary>
    [Fact(DisplayName = "Exactly 20 items should get 20% discount")]
    public void Given_Exactly20Items_When_CalculatingDiscount_Then_ShouldGet20PercentDiscount()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = SaleTestData.GenerateValidProductName(),
            Quantity = 20,
            UnitPrice = 100m
        };

        // Act
        item.CalculateDiscount();

        // Assert
        Assert.Equal(0.20m, item.Discount);
        Assert.Equal(1600m, item.TotalAmount); // 20 * 100 * 0.80 = 1600
    }
}
