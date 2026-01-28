using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the CreateProductHandler class.
/// </summary>
public class CreateProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateProductHandler(_productRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid product creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When creating product Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Price = command.Price,
            Description = command.Description,
            Category = command.Category,
            Image = command.Image,
            RatingRate = command.Rating.Rate,
            RatingCount = command.Rating.Count
        };

        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(product);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.Title.Should().Be(command.Title);
        result.Price.Should().Be(command.Price);
        await _productRepository.Received(1).CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid product creation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid product data When creating product Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateProductCommand(); // Empty command will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that creating a product with empty title throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given empty title When creating product Then throws validation exception")]
    public async Task Handle_EmptyTitle_ThrowsValidationException()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateInvalidCommandWithEmptyTitle();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that creating a product with negative price throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given negative price When creating product Then throws validation exception")]
    public async Task Handle_NegativePrice_ThrowsValidationException()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateInvalidCommandWithNegativePrice();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
