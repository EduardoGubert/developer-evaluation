using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests.
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale");

        sale.Update(
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName);

        sale.Items.Clear();

        foreach (var itemCommand in command.Items)
        {
            sale.AddItem(
                itemCommand.ProductId,
                itemCommand.ProductName,
                itemCommand.Quantity,
                itemCommand.UnitPrice);
        }

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        var saleModifiedEvent = new SaleModifiedEvent(updatedSale);
        _logger.LogInformation(
            "SaleModified event published. SaleId: {SaleId}, SaleNumber: {SaleNumber}, TotalAmount: {TotalAmount}",
            saleModifiedEvent.Sale.Id,
            saleModifiedEvent.Sale.SaleNumber,
            saleModifiedEvent.Sale.TotalAmount);

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}
