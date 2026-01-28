using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

/// <summary>
/// Handler for processing CreateProductCommand requests.
/// </summary>
public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CreateProductHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateProductCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var product = new Product
        {
            Title = command.Title,
            Price = command.Price,
            Description = command.Description,
            Category = command.Category,
            Image = command.Image,
            RatingRate = command.Rating.Rate,
            RatingCount = command.Rating.Count
        };

        var createdProduct = await _productRepository.CreateAsync(product, cancellationToken);

        var result = new CreateProductResult
        {
            Id = createdProduct.Id,
            Title = createdProduct.Title,
            Price = createdProduct.Price,
            Description = createdProduct.Description,
            Category = createdProduct.Category,
            Image = createdProduct.Image,
            Rating = new CreateProductRatingResult
            {
                Rate = createdProduct.RatingRate,
                Count = createdProduct.RatingCount
            }
        };

        return result;
    }
}
