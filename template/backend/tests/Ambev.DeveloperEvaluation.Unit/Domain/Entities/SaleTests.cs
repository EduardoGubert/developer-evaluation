using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover sale operations, cancellation, and validation scenarios.
/// </summary>
public class SaleTests
{
    /// <summary>
    /// Tests that a sale can be created with valid data.
    /// </summary>
    [Fact(DisplayName = "Sale should be created with valid data")]
    public void Given_ValidData_When_CreatingSale_Then_SaleShouldBeCreated()
    {
        // Arrange & Act
        var sale = SaleTestData.GenerateValidSale();

        // Assert
        Assert.NotEqual(Guid.Empty, sale.Id);
        Assert.NotEmpty(sale.SaleNumber);
        Assert.NotEmpty(sale.CustomerName);
        Assert.NotEmpty(sale.BranchName);
        Assert.NotEmpty(sale.Items);
        Assert.True(sale.TotalAmount > 0);
    }

    /// <summary>
    /// Tests that adding an item to a sale updates the total amount.
    /// </summary>
    [Fact(DisplayName = "Adding item should update total amount")]
    public void Given_Sale_When_AddingItem_Then_TotalAmountShouldBeUpdated()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = SaleTestData.GenerateValidSaleNumber(),
            CustomerId = Guid.NewGuid(),
            CustomerName = SaleTestData.GenerateValidCustomerName(),
            BranchId = Guid.NewGuid(),
            BranchName = SaleTestData.GenerateValidBranchName()
        };

        // Act
        sale.AddItem(Guid.NewGuid(), SaleTestData.GenerateValidProductName(), 2, 100m);

        // Assert
        Assert.Single(sale.Items);
        Assert.Equal(200m, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that cancelling a sale sets the IsCancelled flag.
    /// </summary>
    [Fact(DisplayName = "Sale should be marked as cancelled when cancelled")]
    public void Given_ActiveSale_When_Cancelled_Then_ShouldBeMarkedAsCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        Assert.False(sale.IsCancelled);

        // Act
        sale.Cancel();

        // Assert
        Assert.True(sale.IsCancelled);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that cancelling an item updates the total amount.
    /// </summary>
    [Fact(DisplayName = "Cancelling item should update total amount")]
    public void Given_SaleWithItems_When_CancellingItem_Then_TotalAmountShouldBeUpdated()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = SaleTestData.GenerateValidSaleNumber(),
            CustomerId = Guid.NewGuid(),
            CustomerName = SaleTestData.GenerateValidCustomerName(),
            BranchId = Guid.NewGuid(),
            BranchName = SaleTestData.GenerateValidBranchName()
        };
        sale.AddItem(Guid.NewGuid(), "Product 1", 2, 100m);
        sale.AddItem(Guid.NewGuid(), "Product 2", 3, 50m);

        var initialTotal = sale.TotalAmount;
        var itemToCancel = sale.Items.First();

        // Act
        sale.CancelItem(itemToCancel.Id);

        // Assert
        Assert.True(itemToCancel.IsCancelled);
        Assert.True(sale.TotalAmount < initialTotal);
    }

    /// <summary>
    /// Tests that removing an item updates the total amount.
    /// </summary>
    [Fact(DisplayName = "Removing item should update total amount")]
    public void Given_SaleWithItems_When_RemovingItem_Then_TotalAmountShouldBeUpdated()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = SaleTestData.GenerateValidSaleNumber(),
            CustomerId = Guid.NewGuid(),
            CustomerName = SaleTestData.GenerateValidCustomerName(),
            BranchId = Guid.NewGuid(),
            BranchName = SaleTestData.GenerateValidBranchName()
        };
        sale.AddItem(Guid.NewGuid(), "Product 1", 2, 100m);
        sale.AddItem(Guid.NewGuid(), "Product 2", 3, 50m);

        var itemToRemove = sale.Items.First();

        // Act
        var result = sale.RemoveItem(itemToRemove.Id);

        // Assert
        Assert.True(result);
        Assert.Single(sale.Items);
    }

    /// <summary>
    /// Tests that validation passes for valid sale data.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for valid sale data")]
    public void Given_ValidSaleData_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var result = sale.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when sale number is empty.
    /// </summary>
    [Fact(DisplayName = "Validation should fail when sale number is empty")]
    public void Given_EmptySaleNumber_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "",
            CustomerId = Guid.NewGuid(),
            CustomerName = SaleTestData.GenerateValidCustomerName(),
            BranchId = Guid.NewGuid(),
            BranchName = SaleTestData.GenerateValidBranchName()
        };
        sale.AddItem(Guid.NewGuid(), "Product", 1, 100m);

        // Act
        var result = sale.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when sale has no items.
    /// </summary>
    [Fact(DisplayName = "Validation should fail when sale has no items")]
    public void Given_SaleWithNoItems_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = SaleTestData.GenerateValidSaleNumber(),
            CustomerId = Guid.NewGuid(),
            CustomerName = SaleTestData.GenerateValidCustomerName(),
            BranchId = Guid.NewGuid(),
            BranchName = SaleTestData.GenerateValidBranchName()
        };

        // Act
        var result = sale.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }
}
