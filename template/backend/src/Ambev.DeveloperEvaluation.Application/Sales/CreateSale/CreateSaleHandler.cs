using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests.
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventStore _eventStore;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IEventStore eventStore,
        IMapper mapper,
        ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _eventStore = eventStore;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale != null)
            throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");

        var sale = _mapper.Map<Sale>(command);

        foreach (var itemCommand in command.Items)
        {
            sale.AddItem(
                itemCommand.ProductId,
                itemCommand.ProductName,
                itemCommand.Quantity,
                itemCommand.UnitPrice);
        }

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
                
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
                ItemCount = createdSale.Items.Count
            },
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "SaleCreated event published. SaleId: {SaleId}, SaleNumber: {SaleNumber}, TotalAmount: {TotalAmount}",
            createdSale.Id,
            createdSale.SaleNumber,
            createdSale.TotalAmount);

        var result = _mapper.Map<CreateSaleResult>(createdSale);
        return result;
    }
}
