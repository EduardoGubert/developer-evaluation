using Ambev.DeveloperEvaluation.Application.Products.GetProductsByCategory;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the GetProductsByCategoryHandler class.
/// </summary>
public class GetProductsByCategoryHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly GetProductsByCategoryHandler _handler;

    public GetProductsByCategoryHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetProductsByCategoryHandler(_productRepository, _mapper);
    }

    /// <summary>
    /// Tests that products are returned for a valid category.
    /// </summary>
    [Fact(DisplayName = "Given valid category When getting products Then returns products in category")]
    public async Task Handle_ValidCategory_ReturnsProductsInCategory()
    {
        // Given
        var category = "Electronics";
        var products = new List<Product>
        {
            ProductTestData.GenerateValidProduct(),
            ProductTestData.GenerateValidProduct()
        };
        products.ForEach(p => p.Category = category);

        var command = new GetProductsByCategoryCommand
        {
            Category = category,
            Page = 1,
            Size = 10
        };

        _productRepository.GetByCategoryAsync(category, 1, 10, null, Arg.Any<CancellationToken>())
            .Returns((products, 2));

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.TotalItems.Should().Be(2);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(1);
        result.Data.Should().AllSatisfy(p => p.Category.Should().Be(category));
    }

    /// <summary>
    /// Tests that empty list is returned when no products in category.
    /// </summary>
    [Fact(DisplayName = "Given category with no products When getting products Then returns empty list")]
    public async Task Handle_CategoryWithNoProducts_ReturnsEmptyList()
    {
        // Given
        var category = "NonExistentCategory";
        var command = new GetProductsByCategoryCommand
        {
            Category = category,
            Page = 1,
            Size = 10
        };

        _productRepository.GetByCategoryAsync(category, 1, 10, null, Arg.Any<CancellationToken>())
            .Returns((new List<Product>(), 0));

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    /// <summary>
    /// Tests that pagination works correctly.
    /// </summary>
    [Fact(DisplayName = "Given products in category When getting with pagination Then returns paged results")]
    public async Task Handle_WithPagination_ReturnsPagedResults()
    {
        // Given
        var category = "Electronics";
        var products = new List<Product> { ProductTestData.GenerateValidProduct() };
        products[0].Category = category;

        var command = new GetProductsByCategoryCommand
        {
            Category = category,
            Page = 2,
            Size = 5
        };

        _productRepository.GetByCategoryAsync(category, 2, 5, null, Arg.Any<CancellationToken>())
            .Returns((products, 10));

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.TotalItems.Should().Be(10);
        result.CurrentPage.Should().Be(2);
        result.TotalPages.Should().Be(2);
    }

    /// <summary>
    /// Tests that ordering is passed to the repository.
    /// </summary>
    [Fact(DisplayName = "Given category and ordering When getting products Then repository receives order parameter")]
    public async Task Handle_WithOrdering_PassesOrderToRepository()
    {
        // Given
        var category = "Electronics";
        var command = new GetProductsByCategoryCommand
        {
            Category = category,
            Page = 1,
            Size = 10,
            Order = "title asc"
        };

        _productRepository.GetByCategoryAsync(category, 1, 10, "title asc", Arg.Any<CancellationToken>())
            .Returns((new List<Product>(), 0));

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).GetByCategoryAsync(category, 1, 10, "title asc", Arg.Any<CancellationToken>());
    }
}
