using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

/// <summary>
/// Handler for processing UpdateProductCommand requests.
/// </summary>
public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateProductHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateProductCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingProduct = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingProduct == null)
            throw new KeyNotFoundException($"Product with ID {command.Id} not found");

        existingProduct.Update(
            command.Title,
            command.Price,
            command.Description,
            command.Category,
            command.Image,
            command.Rating.Rate,
            command.Rating.Count);

        var updatedProduct = await _productRepository.UpdateAsync(existingProduct, cancellationToken);

        return new UpdateProductResult
        {
            Id = updatedProduct.Id,
            Title = updatedProduct.Title,
            Price = updatedProduct.Price,
            Description = updatedProduct.Description,
            Category = updatedProduct.Category,
            Image = updatedProduct.Image,
            Rating = new UpdateProductRatingResult
            {
                Rate = updatedProduct.RatingRate,
                Count = updatedProduct.RatingCount
            }
        };
    }
}
