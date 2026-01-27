using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the CreateSaleHandler class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _logger);
    }

    /// <summary>
    /// Tests that a valid sale creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName
        };

        foreach (var item in command.Items)
        {
            sale.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
        }

        var result = new CreateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            TotalAmount = sale.TotalAmount
        };

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createSaleResult.Should().NotBeNull();
        createSaleResult.Id.Should().Be(sale.Id);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid sale creation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateSaleCommand(); // Empty command will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that creating a sale with an existing sale number throws an exception.
    /// </summary>
    [Fact(DisplayName = "Given existing sale number When creating sale Then throws exception")]
    public async Task Handle_ExistingSaleNumber_ThrowsException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = new Sale { SaleNumber = command.SaleNumber };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*{command.SaleNumber}*already exists*");
    }

    /// <summary>
    /// Tests that items are added to the sale correctly.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then items are added to sale")]
    public async Task Handle_ValidRequest_AddsItemsToSale()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        Sale? capturedSale = null;

        var mappedSale = new Sale
        {
            SaleNumber = command.SaleNumber,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName
        };

        _mapper.Map<Sale>(command).Returns(mappedSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);
        _saleRepository.CreateAsync(Arg.Do<Sale>(s => capturedSale = s), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Sale>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedSale.Should().NotBeNull();
        capturedSale!.Items.Should().HaveCount(command.Items.Count);
    }

    /// <summary>
    /// Tests that creating a sale with item quantity over 20 throws an exception.
    /// </summary>
    [Fact(DisplayName = "Given item quantity over 20 When creating sale Then throws exception")]
    public async Task Handle_ItemQuantityOver20_ThrowsException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateCommandWithInvalidQuantity();

        var mappedSale = new Sale
        {
            SaleNumber = command.SaleNumber,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName
        };

        _mapper.Map<Sale>(command).Returns(mappedSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>()
        .WithMessage("*20*");
    }
}
