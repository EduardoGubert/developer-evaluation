using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for Cart entity tests.
/// </summary>
public static class CartTestData
{
    private static readonly Faker Faker = new Faker();

    /// <summary>
    /// Generates a valid Cart instance with random data.
    /// </summary>
    public static Cart GenerateValidCart()
    {
        var cart = new Cart
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow
        };

        // Add some products to the cart
        cart.AddProduct(Guid.NewGuid(), Faker.Random.Int(1, 10));
        cart.AddProduct(Guid.NewGuid(), Faker.Random.Int(1, 10));

        return cart;
    }

    /// <summary>
    /// Generates a valid user ID.
    /// </summary>
    public static Guid GenerateValidUserId()
    {
        return Guid.NewGuid();
    }

    /// <summary>
    /// Generates a valid product ID.
    /// </summary>
    public static Guid GenerateValidProductId()
    {
        return Guid.NewGuid();
    }

    /// <summary>
    /// Generates a valid quantity.
    /// </summary>
    public static int GenerateValidQuantity()
    {
        return Faker.Random.Int(1, 20);
    }

    /// <summary>
    /// Generates a valid date.
    /// </summary>
    public static DateTime GenerateValidDate()
    {
        return Faker.Date.Recent();
    }
}
