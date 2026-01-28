using Ambev.DeveloperEvaluation.Application.Products.GetCategories;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the GetCategoriesHandler class.
/// </summary>
public class GetCategoriesHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly GetCategoriesHandler _handler;

    public GetCategoriesHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _handler = new GetCategoriesHandler(_productRepository);
    }

    /// <summary>
    /// Tests that categories are returned successfully.
    /// </summary>
    [Fact(DisplayName = "Given categories exist When getting categories Then returns all categories")]
    public async Task Handle_CategoriesExist_ReturnsAllCategories()
    {
        // Given
        var categories = new List<string> { "Electronics", "Clothing", "Books", "Food" };
        var command = new GetCategoriesCommand();

        _productRepository.GetAllCategoriesAsync(Arg.Any<CancellationToken>())
            .Returns(categories);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Categories.Should().HaveCount(4);
        result.Categories.Should().BeEquivalentTo(categories);
        await _productRepository.Received(1).GetAllCategoriesAsync(Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an empty list is returned when no categories exist.
    /// </summary>
    [Fact(DisplayName = "Given no categories exist When getting categories Then returns empty list")]
    public async Task Handle_NoCategoriesExist_ReturnsEmptyList()
    {
        // Given
        var command = new GetCategoriesCommand();

        _productRepository.GetAllCategoriesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<string>());

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Categories.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that the repository method is called correctly.
    /// </summary>
    [Fact(DisplayName = "Given request When getting categories Then repository is called")]
    public async Task Handle_Request_CallsRepository()
    {
        // Given
        var command = new GetCategoriesCommand();

        _productRepository.GetAllCategoriesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<string> { "Test" });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).GetAllCategoriesAsync(Arg.Any<CancellationToken>());
    }
}
