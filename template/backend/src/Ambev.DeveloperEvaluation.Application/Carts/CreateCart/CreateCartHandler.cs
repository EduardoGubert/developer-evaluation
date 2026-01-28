using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

/// <summary>
/// Handler for processing CreateCartCommand requests.
/// </summary>
public class CreateCartHandler : IRequestHandler<CreateCartCommand, CreateCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public CreateCartHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<CreateCartResult> Handle(CreateCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var cart = new Cart
        {
            UserId = command.UserId,
            Date = command.Date
        };

        foreach (var product in command.Products)
        {
            cart.AddProduct(product.ProductId, product.Quantity);
        }

        var createdCart = await _cartRepository.CreateAsync(cart, cancellationToken);

        return new CreateCartResult
        {
            Id = createdCart.Id,
            UserId = createdCart.UserId,
            Date = createdCart.Date,
            Products = createdCart.Products.Select(p => new CreateCartProductResult
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            }).ToList()
        };
    }
}
