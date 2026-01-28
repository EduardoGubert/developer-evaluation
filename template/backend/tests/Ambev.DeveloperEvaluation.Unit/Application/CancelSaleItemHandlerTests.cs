using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the CancelSaleItemHandler class.
/// </summary>
public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventStore _eventStore;
    private readonly ILogger<CancelSaleItemHandler> _logger;
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventStore = Substitute.For<IEventStore>();
        _logger = Substitute.For<ILogger<CancelSaleItemHandler>>();
        _handler = new CancelSaleItemHandler(_saleRepository, _eventStore, _logger);
    }

    /// <summary>
    /// Tests that a valid item cancellation returns success.
    /// </summary>
    [Fact(DisplayName = "Given valid sale and item IDs When cancelling item Then returns success")]
    public async Task Handle_ValidIds_CancelsItemSuccessfully()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();
        var itemId = sale.Items.First().Id;
        var command = new CancelSaleItemCommand(sale.Id, itemId);

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.SaleId.Should().Be(sale.Id);
        result.ItemId.Should().Be(itemId);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that cancelling an item from a non-existent sale throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When cancelling item Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var command = new CancelSaleItemCommand(saleId, itemId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{saleId}*not found*");
    }

    /// <summary>
    /// Tests that cancelling an item from a cancelled sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given cancelled sale When cancelling item Then throws InvalidOperationException")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();
        var itemId = sale.Items.First().Id;
        var command = new CancelSaleItemCommand(sale.Id, itemId);

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*cancelled sale*");
    }

    /// <summary>
    /// Tests that cancelling a non-existent item throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent item ID When cancelling item Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentItem_ThrowsKeyNotFoundException()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();
        var nonExistentItemId = Guid.NewGuid();
        var command = new CancelSaleItemCommand(sale.Id, nonExistentItemId);

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{nonExistentItemId}*not found*");
    }

    /// <summary>
    /// Tests that empty sale GUID throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given empty sale GUID When cancelling item Then throws ValidationException")]
    public async Task Handle_EmptySaleGuid_ThrowsValidationException()
    {
        // Given
        var command = new CancelSaleItemCommand(Guid.Empty, Guid.NewGuid());

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that empty item GUID throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given empty item GUID When cancelling item Then throws ValidationException")]
    public async Task Handle_EmptyItemGuid_ThrowsValidationException()
    {
        // Given
        var command = new CancelSaleItemCommand(Guid.NewGuid(), Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
