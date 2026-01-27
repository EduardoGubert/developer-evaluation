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
    private readonly ILogger<CancelSaleHandler> _logger;

    public CancelSaleHandler(
        ISaleRepository saleRepository,
        ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
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

        var saleCancelledEvent = new SaleCancelledEvent(sale.Id, sale.SaleNumber);
        _logger.LogInformation(
            "SaleCancelled event published. SaleId: {SaleId}, SaleNumber: {SaleNumber}",
            saleCancelledEvent.SaleId,
            saleCancelledEvent.SaleNumber);

        return new CancelSaleResponse
        {
            Success = true,
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber
        };
    }
}
