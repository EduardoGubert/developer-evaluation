using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the GetSaleHandler class.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid sale ID returns the sale successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When getting sale Then returns sale successfully")]
    public async Task Handle_ValidId_ReturnsSaleSuccessfully()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();
        var command = new GetSaleCommand(sale.Id);
        var expectedResult = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            BranchId = sale.BranchId,
            BranchName = sale.BranchName,
            TotalAmount = sale.TotalAmount,
            IsCancelled = sale.IsCancelled
        };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        result.SaleNumber.Should().Be(sale.SaleNumber);
        await _saleRepository.Received(1).GetByIdAsync(sale.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a non-existent sale ID throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When getting sale Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentId_ThrowsKeyNotFoundException()
    {
        // Given
        var nonExistentId = Guid.NewGuid();
        var command = new GetSaleCommand(nonExistentId);

        _saleRepository.GetByIdAsync(nonExistentId, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{nonExistentId}*not found*");
    }

    /// <summary>
    /// Tests that an empty GUID throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given empty GUID When getting sale Then throws ValidationException")]
    public async Task Handle_EmptyGuid_ThrowsValidationException()
    {
        // Given
        var command = new GetSaleCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
