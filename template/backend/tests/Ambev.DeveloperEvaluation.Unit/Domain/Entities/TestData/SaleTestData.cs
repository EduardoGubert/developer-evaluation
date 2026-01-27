using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for Sale and SaleItem entities using the Bogus library.
/// </summary>
public static class SaleTestData
{
    /// <summary>
    /// Configures the Faker to generate valid SaleItem entities.
    /// </summary>
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.Id, f => Guid.NewGuid())
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Number(1, 20))
        .RuleFor(i => i.UnitPrice, f => decimal.Parse(f.Commerce.Price(10, 1000)));

    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// </summary>
    /// <returns>A valid Sale entity.</returns>
    public static Sale GenerateValidSale()
    {
        var faker = new Faker();
        var sale = new Sale
        {
            SaleNumber = $"SALE-{faker.Random.Number(10000, 99999)}",
            SaleDate = faker.Date.Recent(30),
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Person.FullName,
            BranchId = Guid.NewGuid(),
            BranchName = faker.Company.CompanyName()
        };

        var itemCount = faker.Random.Number(1, 3);
        for (int i = 0; i < itemCount; i++)
        {
            sale.AddItem(
                Guid.NewGuid(),
                faker.Commerce.ProductName(),
                faker.Random.Number(1, 5),
                decimal.Parse(faker.Commerce.Price(10, 100)));
        }

        return sale;
    }

    /// <summary>
    /// Generates a valid SaleItem entity.
    /// </summary>
    /// <returns>A valid SaleItem entity.</returns>
    public static SaleItem GenerateValidSaleItem()
    {
        var item = SaleItemFaker.Generate();
        item.CalculateDiscount();
        return item;
    }

    /// <summary>
    /// Generates a SaleItem with quantity that qualifies for 10% discount (4-9 items).
    /// </summary>
    /// <returns>A SaleItem with 10% discount.</returns>
    public static SaleItem GenerateItemWithTenPercentDiscount()
    {
        var faker = new Faker();
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = faker.Commerce.ProductName(),
            Quantity = faker.Random.Number(4, 9),
            UnitPrice = 100m
        };
        item.CalculateDiscount();
        return item;
    }

    /// <summary>
    /// Generates a SaleItem with quantity that qualifies for 20% discount (10-20 items).
    /// </summary>
    /// <returns>A SaleItem with 20% discount.</returns>
    public static SaleItem GenerateItemWithTwentyPercentDiscount()
    {
        var faker = new Faker();
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = faker.Commerce.ProductName(),
            Quantity = faker.Random.Number(10, 20),
            UnitPrice = 100m
        };
        item.CalculateDiscount();
        return item;
    }

    /// <summary>
    /// Generates a SaleItem with quantity that does not qualify for discount (1-3 items).
    /// </summary>
    /// <returns>A SaleItem without discount.</returns>
    public static SaleItem GenerateItemWithNoDiscount()
    {
        var faker = new Faker();
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = faker.Commerce.ProductName(),
            Quantity = faker.Random.Number(1, 3),
            UnitPrice = 100m
        };
        item.CalculateDiscount();
        return item;
    }

    /// <summary>
    /// Generates a SaleItem with invalid quantity (more than 20 items).
    /// </summary>
    /// <returns>A SaleItem with invalid quantity.</returns>
    public static SaleItem GenerateItemWithInvalidQuantity()
    {
        var faker = new Faker();
        return new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = faker.Commerce.ProductName(),
            Quantity = faker.Random.Number(21, 50),
            UnitPrice = 100m
        };
    }

    /// <summary>
    /// Generates a valid sale number.
    /// </summary>
    /// <returns>A valid sale number.</returns>
    public static string GenerateValidSaleNumber()
    {
        return $"SALE-{new Faker().Random.Number(10000, 99999)}";
    }

    /// <summary>
    /// Generates a valid customer name.
    /// </summary>
    /// <returns>A valid customer name.</returns>
    public static string GenerateValidCustomerName()
    {
        return new Faker().Person.FullName;
    }

    /// <summary>
    /// Generates a valid branch name.
    /// </summary>
    /// <returns>A valid branch name.</returns>
    public static string GenerateValidBranchName()
    {
        return new Faker().Company.CompanyName();
    }

    /// <summary>
    /// Generates a valid product name.
    /// </summary>
    /// <returns>A valid product name.</returns>
    public static string GenerateValidProductName()
    {
        return new Faker().Commerce.ProductName();
    }
}
