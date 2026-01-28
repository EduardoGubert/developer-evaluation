using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handler for processing CancelSaleCommand requests.
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventStore _eventStore;
    private readonly ILogger<CancelSaleHandler> _logger;

    public CancelSaleHandler(
        ISaleRepository saleRepository,
        IEventStore eventStore,
        ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _eventStore = eventStore;
        _logger = logger;
    }

    public async Task<CancelSaleResponse> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            throw new ValidationException("Sale ID is required.");

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Sale is already cancelled");

        sale.Cancel();
        await _saleRepository.UpdateAsync(sale, cancellationToken);
                
        await _eventStore.AppendAsync(
            eventType: "SaleCancelled",
            aggregateId: sale.Id.ToString(),
            aggregateType: "Sale",
            data: new
            {
                sale.SaleNumber,
                sale.TotalAmount,
                sale.CustomerId,
                sale.CustomerName,
                CancelledAt = DateTime.UtcNow
            },
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "SaleCancelled event published. SaleId: {SaleId}, SaleNumber: {SaleNumber}",
            sale.Id,
            sale.SaleNumber);

        return new CancelSaleResponse
        {
            Success = true,
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber
        };
    }
}
