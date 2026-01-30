using Ambev.DeveloperEvaluation.Application.Carts.GetCarts;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the GetCartsHandler class.
/// </summary>
public class GetCartsHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly GetCartsHandler _handler;

    public GetCartsHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetCartsHandler(_cartRepository, _mapper);
    }

    /// <summary>
    /// Tests that carts are returned with correct pagination.
    /// </summary>
    [Fact(DisplayName = "Given carts exist When getting carts Then returns paginated list")]
    public async Task Handle_CartsExist_ReturnsPaginatedList()
    {
        // Given
        var carts = new List<Cart>
        {
            CartTestData.GenerateValidCart(),
            CartTestData.GenerateValidCart()
        };
        var command = new GetCartsCommand { Page = 1, Size = 10 };

        _cartRepository.GetAllAsync(1, 10, null, Arg.Any<Dictionary<string, string>?>(), Arg.Any<CancellationToken>())
            .Returns((carts, 2));

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.TotalItems.Should().Be(2);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    /// <summary>
    /// Tests that an empty list is returned when no carts exist.
    /// </summary>
    [Fact(DisplayName = "Given no carts exist When getting carts Then returns empty list")]
    public async Task Handle_NoCartsExist_ReturnsEmptyList()
    {
        // Given
        var command = new GetCartsCommand { Page = 1, Size = 10 };

        _cartRepository.GetAllAsync(1, 10, null, Arg.Any<Dictionary<string, string>?>(), Arg.Any<CancellationToken>())
            .Returns((new List<Cart>(), 0));

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    /// <summary>
    /// Tests that pagination calculates total pages correctly.
    /// </summary>
    [Fact(DisplayName = "Given many carts When getting with pagination Then calculates total pages correctly")]
    public async Task Handle_ManyCartsWithPagination_CalculatesTotalPagesCorrectly()
    {
        // Given
        var carts = new List<Cart> { CartTestData.GenerateValidCart() };
        var command = new GetCartsCommand { Page = 2, Size = 5 };

        _cartRepository.GetAllAsync(2, 5, null, Arg.Any<Dictionary<string, string>?>(), Arg.Any<CancellationToken>())
            .Returns((carts, 12));

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.TotalItems.Should().Be(12);
        result.CurrentPage.Should().Be(2);
        result.TotalPages.Should().Be(3); // 12 items / 5 per page = 3 pages
    }

    /// <summary>
    /// Tests that ordering parameter is passed to repository.
    /// </summary>
    [Fact(DisplayName = "Given order parameter When getting carts Then repository receives order")]
    public async Task Handle_WithOrdering_PassesOrderToRepository()
    {
        // Given
        var command = new GetCartsCommand
        {
            Page = 1,
            Size = 10,
            Order = "date desc"
        };

        _cartRepository.GetAllAsync(1, 10, "date desc", Arg.Any<Dictionary<string, string>?>(), Arg.Any<CancellationToken>())
            .Returns((new List<Cart>(), 0));

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _cartRepository.Received(1).GetAllAsync(1, 10, "date desc", Arg.Any<Dictionary<string, string>?>(), Arg.Any<CancellationToken>());
    }
}
