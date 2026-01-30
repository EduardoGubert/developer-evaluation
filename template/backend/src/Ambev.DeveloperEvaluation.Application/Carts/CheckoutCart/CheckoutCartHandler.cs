using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Carts.CheckoutCart;

/// <summary>
/// Handler for checking out a cart and creating a sale.
/// Converts cart items into a sale with automatic discount calculation.
/// </summary>
public class CheckoutCartHandler : IRequestHandler<CheckoutCartCommand, CheckoutCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IEventStore _eventStore;
    private readonly ILogger<CheckoutCartHandler> _logger;

    public CheckoutCartHandler(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IUserRepository userRepository,
        ISaleRepository saleRepository,
        IEventStore eventStore,
        ILogger<CheckoutCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _saleRepository = saleRepository;
        _eventStore = eventStore;
        _logger = logger;
    }

    public async Task<CheckoutCartResult> Handle(CheckoutCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new CheckoutCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // 1. Get the cart
        var cart = await _cartRepository.GetByIdAsync(command.CartId, cancellationToken);
        if (cart == null)
            throw new KeyNotFoundException($"Cart with ID {command.CartId} not found.");

        if (cart.Products == null || cart.Products.Count == 0)
            throw new InvalidOperationException("Cannot checkout an empty cart.");

        // 2. Get the user for CustomerName
        var user = await _userRepository.GetByIdAsync(cart.UserId, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {cart.UserId} not found.");

        // 3. Create the sale
        var sale = new Sale
        {
            CustomerId = cart.UserId,
            CustomerName = $"{user.Firstname} {user.Lastname}",
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            SaleNumber = GenerateSaleNumber()
        };

        // 4. Batch load all products at once to avoid N+1 queries
        var productIds = cart.Products.Select(p => p.ProductId).ToList();
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
        var productDict = products.ToDictionary(p => p.Id);

        // 5. For each cart item, add to sale using cached product details
        foreach (var cartItem in cart.Products)
        {
            if (!productDict.TryGetValue(cartItem.ProductId, out var product))
                throw new KeyNotFoundException($"Product with ID {cartItem.ProductId} not found.");

            sale.AddItem(
                cartItem.ProductId,
                product.Title,
                cartItem.Quantity,
                product.Price);
        }

        // 6. Save the sale
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        // 7. Publish SaleCreated event
        await _eventStore.AppendAsync(
            eventType: "SaleCreated",
            aggregateId: createdSale.Id.ToString(),
            aggregateType: "Sale",
            data: new
            {
                createdSale.SaleNumber,
                createdSale.TotalAmount,
                createdSale.CustomerId,
                createdSale.CustomerName,
                createdSale.BranchId,
                createdSale.BranchName,
                ItemCount = createdSale.Items.Count,
                Source = "CartCheckout",
                CartId = cart.Id
            },
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Cart checked out. CartId: {CartId}, SaleId: {SaleId}, SaleNumber: {SaleNumber}, TotalAmount: {TotalAmount}",
            cart.Id, createdSale.Id, createdSale.SaleNumber, createdSale.TotalAmount);

        // 8. Delete the cart (consumed)
        await _cartRepository.DeleteAsync(cart.Id, cancellationToken);

        // 9. Build result
        return new CheckoutCartResult
        {
            SaleId = createdSale.Id,
            SaleNumber = createdSale.SaleNumber,
            TotalAmount = createdSale.TotalAmount,
            Items = createdSale.Items.Select(i => new CheckoutCartItemResult
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                TotalAmount = i.TotalAmount
            }).ToList()
        };
    }

    private static string GenerateSaleNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = Guid.NewGuid().ToString("N")[..6].ToUpper();
        return $"SALE-{timestamp}-{random}";
    }
}
