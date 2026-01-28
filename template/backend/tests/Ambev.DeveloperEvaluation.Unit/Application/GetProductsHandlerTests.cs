using Ambev.DeveloperEvaluation.Application.Products.GetProducts;
using Ambev.DeveloperEvaluation.Domain.Common.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the GetProductsHandler class.
/// </summary>
public class GetProductsHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly GetProductsHandler _handler;

    public GetProductsHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _cacheService = Substitute.For<ICacheService>();
        _mapper = Substitute.For<IMapper>();
        
        _cacheService.GetAsync<GetProductsResult>(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((GetProductsResult?)null);

        _handler = new GetProductsHandler(_productRepository, _cacheService, _mapper);
    }

    /// <summary>
    /// Tests that a valid request returns a paginated list of products.
    /// </summary>
    [Fact(DisplayName = "Given valid request When getting products Then returns paginated list")]
    public async Task Handle_ValidRequest_ReturnsPaginatedList()
    {
        // Given
        var products = new List<Product>
        {
            ProductTestData.GenerateValidProduct(),
            ProductTestData.GenerateValidProduct(),
            ProductTestData.GenerateValidProduct()
        };
        var command = new GetProductsCommand { Page = 1, Size = 10 };

        _productRepository.GetAllAsync(1, 10, null, Arg.Any<CancellationToken>())
            .Returns((products.AsEnumerable(), 3));

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.TotalItems.Should().Be(3);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    /// <summary>
    /// Tests that an empty result returns an empty list.
    /// </summary>
    [Fact(DisplayName = "Given no products exist When getting products Then returns empty list")]
    public async Task Handle_NoProductsExist_ReturnsEmptyList()
    {
        // Given
        var command = new GetProductsCommand { Page = 1, Size = 10 };

        _productRepository.GetAllAsync(1, 10, null, Arg.Any<CancellationToken>())
            .Returns((Enumerable.Empty<Product>(), 0));

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    /// <summary>
    /// Tests that pagination calculates correctly.
    /// </summary>
    [Fact(DisplayName = "Given multiple pages When getting products Then calculates pagination correctly")]
    public async Task Handle_MultiplePagesExist_CalculatesPaginationCorrectly()
    {
        // Given
        var products = new List<Product> { ProductTestData.GenerateValidProduct() };
        var command = new GetProductsCommand { Page = 2, Size = 5 };

        _productRepository.GetAllAsync(2, 5, null, Arg.Any<CancellationToken>())
            .Returns((products.AsEnumerable(), 12)); // 12 total items, 5 per page = 3 pages

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.TotalItems.Should().Be(12);
        result.CurrentPage.Should().Be(2);
        result.TotalPages.Should().Be(3);
    }

    /// <summary>
    /// Tests that ordering is passed to the repository.
    /// </summary>
    [Fact(DisplayName = "Given order parameter When getting products Then passes order to repository")]
    public async Task Handle_WithOrderParameter_PassesOrderToRepository()
    {
        // Given
        var products = new List<Product> { ProductTestData.GenerateValidProduct() };
        var command = new GetProductsCommand { Page = 1, Size = 10, Order = "price desc" };

        _productRepository.GetAllAsync(1, 10, "price desc", Arg.Any<CancellationToken>())
            .Returns((products.AsEnumerable(), 1));

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).GetAllAsync(1, 10, "price desc", Arg.Any<CancellationToken>());
    }
}
