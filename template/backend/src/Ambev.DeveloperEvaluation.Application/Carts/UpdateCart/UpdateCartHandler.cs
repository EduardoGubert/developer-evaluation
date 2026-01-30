using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

/// <summary>
/// Handler for processing UpdateCartCommand requests.
/// </summary>
public class UpdateCartHandler : IRequestHandler<UpdateCartCommand, UpdateCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public UpdateCartHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<UpdateCartResult> Handle(UpdateCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingCart = await _cartRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingCart == null)
            throw new KeyNotFoundException($"Cart with ID {command.Id} not found");

        existingCart.Update(command.UserId, command.Date);

        var newItems = new List<CartItem>();

        foreach (var product in command.Products)
        {
            var existingItem = existingCart.Products.FirstOrDefault(p => p.ProductId == product.ProductId);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(product.Quantity);
            }
            else
            {
                var newItem = existingCart.AddProduct(product.ProductId, product.Quantity);
                newItems.Add(newItem);
            }
        }
        
        foreach (var newItem in newItems)
        {
            await _cartRepository.CreateItemAsync(newItem, cancellationToken);
        }
                
        var updatedCart = await _cartRepository.UpdateAsync(existingCart, cancellationToken);

        return new UpdateCartResult
        {
            Id = updatedCart.Id,
            UserId = updatedCart.UserId,
            Date = updatedCart.Date,
            Products = updatedCart.Products.Select(p => new UpdateCartProductResult
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            }).ToList()
        };
    }
}
