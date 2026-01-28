using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the GetSalesHandler class.
/// </summary>
public class GetSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSalesHandler _handler;

    public GetSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSalesHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid request returns a paginated list of sales.
    /// </summary>
    [Fact(DisplayName = "Given valid request When getting sales Then returns paginated list")]
    public async Task Handle_ValidRequest_ReturnsPaginatedList()
    {
        // Given
        var sales = new List<Sale>
        {
            SaleTestData.GenerateValidSale(),
            SaleTestData.GenerateValidSale(),
            SaleTestData.GenerateValidSale()
        };
        var command = new GetSalesCommand { Page = 1, Size = 10 };
        var mappedItems = sales.Select(s => new GetSalesItemResult
        {
            Id = s.Id,
            SaleNumber = s.SaleNumber,
            TotalAmount = s.TotalAmount
        }).ToList();

        _saleRepository.GetAllAsync(1, 10, null, Arg.Any<CancellationToken>())
            .Returns((sales.AsEnumerable(), 3));
        _mapper.Map<List<GetSalesItemResult>>(Arg.Any<List<Sale>>())
            .Returns(mappedItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.TotalItems.Should().Be(3);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    /// <summary>
    /// Tests that an empty result returns an empty list.
    /// </summary>
    [Fact(DisplayName = "Given no sales exist When getting sales Then returns empty list")]
    public async Task Handle_NoSalesExist_ReturnsEmptyList()
    {
        // Given
        var command = new GetSalesCommand { Page = 1, Size = 10 };

        _saleRepository.GetAllAsync(1, 10, null, Arg.Any<CancellationToken>())
            .Returns((Enumerable.Empty<Sale>(), 0));
        _mapper.Map<List<GetSalesItemResult>>(Arg.Any<List<Sale>>())
            .Returns(new List<GetSalesItemResult>());

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    /// <summary>
    /// Tests that pagination calculates correctly.
    /// </summary>
    [Fact(DisplayName = "Given multiple pages When getting sales Then calculates pagination correctly")]
    public async Task Handle_MultiplePagesExist_CalculatesPaginationCorrectly()
    {
        // Given
        var sales = new List<Sale> { SaleTestData.GenerateValidSale() };
        var command = new GetSalesCommand { Page = 2, Size = 5 };
        var mappedItems = new List<GetSalesItemResult>
        {
            new GetSalesItemResult { Id = sales[0].Id }
        };

        _saleRepository.GetAllAsync(2, 5, null, Arg.Any<CancellationToken>())
            .Returns((sales.AsEnumerable(), 12)); // 12 total items, 5 per page = 3 pages
        _mapper.Map<List<GetSalesItemResult>>(Arg.Any<List<Sale>>())
            .Returns(mappedItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.TotalItems.Should().Be(12);
        result.CurrentPage.Should().Be(2);
        result.TotalPages.Should().Be(3);
    }

    /// <summary>
    /// Tests that ordering is passed to the repository.
    /// </summary>
    [Fact(DisplayName = "Given order parameter When getting sales Then passes order to repository")]
    public async Task Handle_WithOrderParameter_PassesOrderToRepository()
    {
        // Given
        var sales = new List<Sale> { SaleTestData.GenerateValidSale() };
        var command = new GetSalesCommand { Page = 1, Size = 10, Order = "saleDate desc" };

        _saleRepository.GetAllAsync(1, 10, "saleDate desc", Arg.Any<CancellationToken>())
            .Returns((sales.AsEnumerable(), 1));
        _mapper.Map<List<GetSalesItemResult>>(Arg.Any<List<Sale>>())
            .Returns(new List<GetSalesItemResult>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetAllAsync(1, 10, "saleDate desc", Arg.Any<CancellationToken>());
    }
}
