using Ambev.DeveloperEvaluation.Application.Carts.CheckoutCart;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data for CheckoutCartHandler tests.
/// </summary>
public static class CheckoutCartHandlerTestData
{
    private static readonly Faker Faker = new Faker();

    /// <summary>
    /// Generates a valid CheckoutCartCommand with random data.
    /// </summary>
    public static CheckoutCartCommand GenerateValidCommand()
    {
        return new CheckoutCartCommand
        {
            CartId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            BranchName = Faker.Company.CompanyName()
        };
    }

    /// <summary>
    /// Generates an invalid CheckoutCartCommand with empty CartId.
    /// </summary>
    public static CheckoutCartCommand GenerateInvalidCommandWithEmptyCartId()
    {
        var command = GenerateValidCommand();
        command.CartId = Guid.Empty;
        return command;
    }

    /// <summary>
    /// Generates an invalid CheckoutCartCommand with empty BranchId.
    /// </summary>
    public static CheckoutCartCommand GenerateInvalidCommandWithEmptyBranchId()
    {
        var command = GenerateValidCommand();
        command.BranchId = Guid.Empty;
        return command;
    }

    /// <summary>
    /// Generates an invalid CheckoutCartCommand with empty BranchName.
    /// </summary>
    public static CheckoutCartCommand GenerateInvalidCommandWithEmptyBranchName()
    {
        var command = GenerateValidCommand();
        command.BranchName = string.Empty;
        return command;
    }

    /// <summary>
    /// Generates an invalid CheckoutCartCommand with BranchName exceeding max length.
    /// </summary>
    public static CheckoutCartCommand GenerateInvalidCommandWithBranchNameTooLong()
    {
        var command = GenerateValidCommand();
        command.BranchName = new string('A', 201);
        return command;
    }
}
