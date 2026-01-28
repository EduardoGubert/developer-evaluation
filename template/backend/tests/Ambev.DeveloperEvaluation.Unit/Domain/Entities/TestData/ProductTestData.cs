using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for Product entity tests.
/// </summary>
public static class ProductTestData
{
    private static readonly Faker<Product> ProductFaker = new Faker<Product>()
        .RuleFor(p => p.Id, f => Guid.NewGuid())
        .RuleFor(p => p.Title, f => f.Commerce.ProductName())
        .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(10, 1000)))
        .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
        .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
        .RuleFor(p => p.Image, f => f.Image.PicsumUrl())
        .RuleFor(p => p.RatingRate, f => f.Random.Decimal(0, 5))
        .RuleFor(p => p.RatingCount, f => f.Random.Int(0, 1000))
        .RuleFor(p => p.CreatedAt, f => DateTime.UtcNow);

    /// <summary>
    /// Generates a valid Product instance with random data.
    /// </summary>
    public static Product GenerateValidProduct()
    {
        return ProductFaker.Generate();
    }

    /// <summary>
    /// Generates a valid product title.
    /// </summary>
    public static string GenerateValidTitle()
    {
        return new Faker().Commerce.ProductName();
    }

    /// <summary>
    /// Generates a valid product description.
    /// </summary>
    public static string GenerateValidDescription()
    {
        return new Faker().Commerce.ProductDescription();
    }

    /// <summary>
    /// Generates a valid product category.
    /// </summary>
    public static string GenerateValidCategory()
    {
        return new Faker().Commerce.Categories(1)[0];
    }

    /// <summary>
    /// Generates a valid price.
    /// </summary>
    public static decimal GenerateValidPrice()
    {
        return decimal.Parse(new Faker().Commerce.Price(10, 1000));
    }

    /// <summary>
    /// Generates a valid image URL.
    /// </summary>
    public static string GenerateValidImage()
    {
        return new Faker().Image.PicsumUrl();
    }
}
