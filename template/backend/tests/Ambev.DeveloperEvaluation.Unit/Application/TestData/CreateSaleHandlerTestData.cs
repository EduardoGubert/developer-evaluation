using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data for CreateSaleHandler tests.
/// </summary>
public static class CreateSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid CreateSaleCommand instances.
    /// </summary>
    private static readonly Faker<CreateSaleCommand> CreateSaleCommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(c => c.SaleNumber, f => $"SALE-{f.Random.Number(10000, 99999)}")
        .RuleFor(c => c.SaleDate, f => f.Date.Recent(30))
        .RuleFor(c => c.CustomerId, f => Guid.NewGuid())
        .RuleFor(c => c.CustomerName, f => f.Person.FullName)
        .RuleFor(c => c.BranchId, f => Guid.NewGuid())
        .RuleFor(c => c.BranchName, f => f.Company.CompanyName())
        .RuleFor(c => c.Items, f => GenerateValidItems(f.Random.Number(1, 3)));

    /// <summary>
    /// Generates a valid CreateSaleCommand with randomized data.
    /// </summary>
    /// <returns>A valid CreateSaleCommand instance.</returns>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return CreateSaleCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a list of valid CreateSaleItemCommand instances.
    /// </summary>
    /// <param name="count">The number of items to generate.</param>
    /// <returns>A list of valid CreateSaleItemCommand instances.</returns>
    public static List<CreateSaleItemCommand> GenerateValidItems(int count)
    {
        var faker = new Faker();
        var items = new List<CreateSaleItemCommand>();

        for (int i = 0; i < count; i++)
        {
            items.Add(new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                ProductName = faker.Commerce.ProductName(),
                Quantity = faker.Random.Number(1, 10),
                UnitPrice = decimal.Parse(faker.Commerce.Price(10, 100))
            });
        }

        return items;
    }

    /// <summary>
    /// Generates a CreateSaleCommand with an item that has invalid quantity (more than 20).
    /// </summary>
    /// <returns>A CreateSaleCommand with invalid item quantity.</returns>
    public static CreateSaleCommand GenerateCommandWithInvalidQuantity()
    {
        var command = CreateSaleCommandFaker.Generate();
        command.Items = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                ProductName = new Faker().Commerce.ProductName(),
                Quantity = 25, // Invalid: more than 20
                UnitPrice = 100m
            }
        };
        return command;
    }

    /// <summary>
    /// Generates a CreateSaleCommand with items that qualify for 10% discount.
    /// </summary>
    /// <returns>A CreateSaleCommand with items for 10% discount.</returns>
    public static CreateSaleCommand GenerateCommandWith10PercentDiscount()
    {
        var faker = new Faker();
        var command = CreateSaleCommandFaker.Generate();
        command.Items = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                ProductName = faker.Commerce.ProductName(),
                Quantity = 5, // Qualifies for 10% discount
                UnitPrice = 100m
            }
        };
        return command;
    }

    /// <summary>
    /// Generates a CreateSaleCommand with items that qualify for 20% discount.
    /// </summary>
    /// <returns>A CreateSaleCommand with items for 20% discount.</returns>
    public static CreateSaleCommand GenerateCommandWith20PercentDiscount()
    {
        var faker = new Faker();
        var command = CreateSaleCommandFaker.Generate();
        command.Items = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                ProductName = faker.Commerce.ProductName(),
                Quantity = 15, // Qualifies for 20% discount
                UnitPrice = 100m
            }
        };
        return command;
    }
}
