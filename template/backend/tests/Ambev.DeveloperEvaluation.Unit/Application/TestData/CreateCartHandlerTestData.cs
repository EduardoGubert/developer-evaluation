using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data for CreateCartHandler tests.
/// </summary>
public static class CreateCartHandlerTestData
{
    private static readonly Faker Faker = new Faker();

    /// <summary>
    /// Generates a valid CreateCartCommand with random data.
    /// </summary>
    public static CreateCartCommand GenerateValidCommand()
    {
        return new CreateCartCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = new List<CreateCartProductCommand>
            {
                new CreateCartProductCommand
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = Faker.Random.Int(1, 10)
                },
                new CreateCartProductCommand
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = Faker.Random.Int(1, 10)
                }
            }
        };
    }

    /// <summary>
    /// Generates an invalid CreateCartCommand with empty user ID.
    /// </summary>
    public static CreateCartCommand GenerateInvalidCommandWithEmptyUserId()
    {
        var command = GenerateValidCommand();
        command.UserId = Guid.Empty;
        return command;
    }

    /// <summary>
    /// Generates an invalid CreateCartCommand with no products.
    /// </summary>
    public static CreateCartCommand GenerateInvalidCommandWithNoProducts()
    {
        return new CreateCartCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = new List<CreateCartProductCommand>()
        };
    }

    /// <summary>
    /// Generates an invalid CreateCartCommand with invalid product quantity.
    /// </summary>
    public static CreateCartCommand GenerateInvalidCommandWithInvalidQuantity()
    {
        return new CreateCartCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = new List<CreateCartProductCommand>
            {
                new CreateCartProductCommand
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 0 // Invalid quantity
                }
            }
        };
    }
}
