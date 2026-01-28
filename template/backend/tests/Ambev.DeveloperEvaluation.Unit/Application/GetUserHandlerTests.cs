using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the GetUserHandler class.
/// </summary>
public class GetUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly GetUserHandler _handler;

    public GetUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetUserHandler(_userRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid user ID returns the user successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid user ID When getting user Then returns user successfully")]
    public async Task Handle_ValidId_ReturnsUserSuccessfully()
    {
        // Given
        var user = UserTestData.GenerateValidUser();
        user.Id = Guid.NewGuid(); // Set a valid ID for testing
        var command = new GetUserCommand(user.Id);
        var expectedResult = new GetUserResult
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            Phone = user.Phone
        };

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);
        _mapper.Map<GetUserResult>(user).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        await _userRepository.Received(1).GetByIdAsync(user.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a non-existent user ID throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent user ID When getting user Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentId_ThrowsKeyNotFoundException()
    {
        // Given
        var nonExistentId = Guid.NewGuid();
        var command = new GetUserCommand(nonExistentId);

        _userRepository.GetByIdAsync(nonExistentId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{nonExistentId}*not found*");
    }

    /// <summary>
    /// Tests that an empty GUID throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given empty GUID When getting user Then throws ValidationException")]
    public async Task Handle_EmptyGuid_ThrowsValidationException()
    {
        // Given
        var command = new GetUserCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
