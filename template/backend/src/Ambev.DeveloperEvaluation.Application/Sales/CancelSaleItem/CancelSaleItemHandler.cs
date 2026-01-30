using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Handler for processing CancelSaleItemCommand requests.
/// </summary>
public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventStore _eventStore;
    private readonly ILogger<CancelSaleItemHandler> _logger;

    public CancelSaleItemHandler(
        ISaleRepository saleRepository,
        IEventStore eventStore,
        ILogger<CancelSaleItemHandler> logger)
    {
        _saleRepository = saleRepository;
        _eventStore = eventStore;
        _logger = logger;
    }

    public async Task<CancelSaleItemResponse> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        if (request.SaleId == Guid.Empty)
            throw new ValidationException("Sale ID is required.");

        if (request.ItemId == Guid.Empty)
            throw new ValidationException("Item ID is required.");

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot cancel item from a cancelled sale");

        var cancelledItem = sale.CancelItem(request.ItemId);
        if (cancelledItem == null)
            throw new KeyNotFoundException($"Item with ID {request.ItemId} not found or already cancelled");

        await _saleRepository.UpdateAsync(sale, cancellationToken);
               
        await _eventStore.AppendAsync(
            eventType: "ItemCancelled",
            aggregateId: sale.Id.ToString(),
            aggregateType: "Sale",
            data: new
            {
                sale.SaleNumber,
                ItemId = cancelledItem.Id,
                cancelledItem.ProductId,
                cancelledItem.ProductName,
                cancelledItem.Quantity,
                cancelledItem.TotalAmount,
                NewSaleTotalAmount = sale.TotalAmount,
                CancelledAt = DateTime.UtcNow
            },
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "ItemCancelled event published. SaleId: {SaleId}, ItemId: {ItemId}, ProductName: {ProductName}",
            sale.Id,
            cancelledItem.Id,
            cancelledItem.ProductName);

        return new CancelSaleItemResponse
        {
            Success = true,
            SaleId = sale.Id,
            ItemId = cancelledItem.Id,
            ProductName = cancelledItem.ProductName
        };
    }
}
