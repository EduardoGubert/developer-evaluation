using Ambev.DeveloperEvaluation.Application.Users.GetUsers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the GetUsersHandler class.
/// </summary>
public class GetUsersHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly GetUsersHandler _handler;

    public GetUsersHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetUsersHandler(_userRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid request returns a paginated list of users.
    /// </summary>
    [Fact(DisplayName = "Given valid request When getting users Then returns paginated list")]
    public async Task Handle_ValidRequest_ReturnsPaginatedList()
    {
        // Given
        var users = new List<User>
        {
            UserTestData.GenerateValidUser(),
            UserTestData.GenerateValidUser(),
            UserTestData.GenerateValidUser()
        };
        var command = new GetUsersCommand { Page = 1, Size = 10 };
        var mappedItems = users.Select(u => new GetUsersItemResult
        {
            Id = u.Id,
            Email = u.Email,
            Username = u.Username
        }).ToList();

        _userRepository.GetAllAsync(1, 10, null, Arg.Any<CancellationToken>(), Arg.Any<Dictionary<string, string>?>())
            .Returns((users.AsEnumerable(), 3));
        _mapper.Map<List<GetUsersItemResult>>(Arg.Any<List<User>>())
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
    [Fact(DisplayName = "Given no users exist When getting users Then returns empty list")]
    public async Task Handle_NoUsersExist_ReturnsEmptyList()
    {
        // Given
        var command = new GetUsersCommand { Page = 1, Size = 10 };

        _userRepository.GetAllAsync(1, 10, null, Arg.Any<CancellationToken>(), Arg.Any<Dictionary<string, string>?>())
            .Returns((Enumerable.Empty<User>(), 0));
        _mapper.Map<List<GetUsersItemResult>>(Arg.Any<List<User>>())
            .Returns(new List<GetUsersItemResult>());

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
    [Fact(DisplayName = "Given multiple pages When getting users Then calculates pagination correctly")]
    public async Task Handle_MultiplePagesExist_CalculatesPaginationCorrectly()
    {
        // Given
        var users = new List<User> { UserTestData.GenerateValidUser() };
        var command = new GetUsersCommand { Page = 2, Size = 5 };
        var mappedItems = new List<GetUsersItemResult>
        {
            new GetUsersItemResult { Id = users[0].Id }
        };

        _userRepository.GetAllAsync(2, 5, null, Arg.Any<CancellationToken>(), Arg.Any<Dictionary<string, string>?>())
            .Returns((users.AsEnumerable(), 12)); // 12 total items, 5 per page = 3 pages
        _mapper.Map<List<GetUsersItemResult>>(Arg.Any<List<User>>())
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
    [Fact(DisplayName = "Given order parameter When getting users Then passes order to repository")]
    public async Task Handle_WithOrderParameter_PassesOrderToRepository()
    {
        // Given
        var users = new List<User> { UserTestData.GenerateValidUser() };
        var command = new GetUsersCommand { Page = 1, Size = 10, Order = "username desc" };

        _userRepository.GetAllAsync(1, 10, "username desc", Arg.Any<CancellationToken>(), Arg.Any<Dictionary<string, string>?>())
            .Returns((users.AsEnumerable(), 1));
        _mapper.Map<List<GetUsersItemResult>>(Arg.Any<List<User>>())
            .Returns(new List<GetUsersItemResult>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _userRepository.Received(1).GetAllAsync(1, 10, "username desc", Arg.Any<CancellationToken>(), Arg.Any<Dictionary<string, string>?>());
    }
}
