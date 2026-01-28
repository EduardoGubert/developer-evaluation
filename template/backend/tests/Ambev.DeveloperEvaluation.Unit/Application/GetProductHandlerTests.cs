using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the GetProductHandler class.
/// </summary>
public class GetProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly GetProductHandler _handler;

    public GetProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetProductHandler(_productRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid product ID returns the product successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid product ID When getting product Then returns product successfully")]
    public async Task Handle_ValidId_ReturnsProductSuccessfully()
    {
        // Given
        var product = ProductTestData.GenerateValidProduct();
        var command = new GetProductCommand(product.Id);

        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.Title.Should().Be(product.Title);
        result.Price.Should().Be(product.Price);
        await _productRepository.Received(1).GetByIdAsync(product.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a non-existent product ID throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent product ID When getting product Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentId_ThrowsKeyNotFoundException()
    {
        // Given
        var nonExistentId = Guid.NewGuid();
        var command = new GetProductCommand(nonExistentId);

        _productRepository.GetByIdAsync(nonExistentId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{nonExistentId}*not found*");
    }

    /// <summary>
    /// Tests that the result includes rating information.
    /// </summary>
    [Fact(DisplayName = "Given valid product When getting product Then result includes rating")]
    public async Task Handle_ValidProduct_ReturnsProductWithRating()
    {
        // Given
        var product = ProductTestData.GenerateValidProduct();
        var command = new GetProductCommand(product.Id);

        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Rating.Should().NotBeNull();
        result.Rating.Rate.Should().Be(product.RatingRate);
        result.Rating.Count.Should().Be(product.RatingCount);
    }
}
