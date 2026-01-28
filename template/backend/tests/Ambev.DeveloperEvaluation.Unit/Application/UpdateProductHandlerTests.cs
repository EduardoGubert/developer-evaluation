using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the UpdateProductHandler class.
/// </summary>
public class UpdateProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateProductHandler(_productRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid update command returns the updated product.
    /// </summary>
    [Fact(DisplayName = "Given valid update command When updating product Then returns updated product")]
    public async Task Handle_ValidCommand_ReturnsUpdatedProduct()
    {
        // Given
        var existingProduct = ProductTestData.GenerateValidProduct();
        var command = new UpdateProductCommand
        {
            Id = existingProduct.Id,
            Title = "Updated Product",
            Price = 199.99m,
            Description = "Updated description",
            Category = "Updated Category",
            Image = "https://example.com/updated.jpg",
            Rating = new UpdateProductRatingCommand { Rate = 4.5m, Count = 100 }
        };

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);
        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(existingProduct.Id);
        await _productRepository.Received(1).UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that updating a non-existent product throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent product ID When updating product Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentProduct_ThrowsKeyNotFoundException()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Title = "Product",
            Price = 99.99m,
            Description = "Description",
            Category = "Category",
            Image = "https://example.com/image.jpg",
            Rating = new UpdateProductRatingCommand { Rate = 4.0m, Count = 50 }
        };

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{command.Id}*not found*");
    }

    /// <summary>
    /// Tests that invalid command data throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid command When updating product Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Given
        var command = new UpdateProductCommand(); // Empty command will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that the product is updated with the correct values.
    /// </summary>
    [Fact(DisplayName = "Given valid command When updating product Then product receives correct values")]
    public async Task Handle_ValidCommand_ProductReceivesCorrectValues()
    {
        // Given
        var existingProduct = ProductTestData.GenerateValidProduct();
        var command = new UpdateProductCommand
        {
            Id = existingProduct.Id,
            Title = "New Title",
            Price = 299.99m,
            Description = "New Description",
            Category = "New Category",
            Image = "https://example.com/new.jpg",
            Rating = new UpdateProductRatingCommand { Rate = 5.0m, Count = 200 }
        };

        Product? capturedProduct = null;
        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);
        _productRepository.UpdateAsync(Arg.Do<Product>(p => capturedProduct = p), Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedProduct.Should().NotBeNull();
        capturedProduct!.Title.Should().Be(command.Title);
        capturedProduct.Price.Should().Be(command.Price);
    }
}
