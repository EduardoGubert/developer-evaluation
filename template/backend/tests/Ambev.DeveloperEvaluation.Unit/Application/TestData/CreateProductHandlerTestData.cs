using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data for CreateProductHandler tests.
/// </summary>
public static class CreateProductHandlerTestData
{
    private static readonly Faker<CreateProductCommand> CreateProductCommandFaker = new Faker<CreateProductCommand>()
        .RuleFor(c => c.Title, f => f.Commerce.ProductName())
        .RuleFor(c => c.Price, f => decimal.Parse(f.Commerce.Price(10, 1000)))
        .RuleFor(c => c.Description, f => f.Commerce.ProductDescription())
        .RuleFor(c => c.Category, f => f.Commerce.Categories(1)[0])
        .RuleFor(c => c.Image, f => f.Image.PicsumUrl())
        .RuleFor(c => c.Rating, f => new CreateProductRatingCommand
        {
            Rate = f.Random.Decimal(0, 5),
            Count = f.Random.Int(0, 1000)
        });

    /// <summary>
    /// Generates a valid CreateProductCommand with random data.
    /// </summary>
    public static CreateProductCommand GenerateValidCommand()
    {
        return CreateProductCommandFaker.Generate();
    }

    /// <summary>
    /// Generates an invalid CreateProductCommand with empty title.
    /// </summary>
    public static CreateProductCommand GenerateInvalidCommandWithEmptyTitle()
    {
        var command = CreateProductCommandFaker.Generate();
        command.Title = "";
        return command;
    }

    /// <summary>
    /// Generates an invalid CreateProductCommand with negative price.
    /// </summary>
    public static CreateProductCommand GenerateInvalidCommandWithNegativePrice()
    {
        var command = CreateProductCommandFaker.Generate();
        command.Price = -10m;
        return command;
    }
}
