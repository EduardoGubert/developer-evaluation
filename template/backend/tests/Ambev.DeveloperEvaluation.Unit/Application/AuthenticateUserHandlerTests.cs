using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the AuthenticateUserHandler class.
/// </summary>
public class AuthenticateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly AuthenticateUserHandler _handler;

    public AuthenticateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        _handler = new AuthenticateUserHandler(_userRepository, _passwordHasher, _jwtTokenGenerator);
    }

    /// <summary>
    /// Tests that valid credentials return a token successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid credentials When authenticating Then returns token")]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        // Given
        var user = UserTestData.GenerateValidUser();
        user.Status = UserStatus.Active;
        var command = new AuthenticateUserCommand
        {
            Email = user.Email,
            Password = "ValidPassword123"
        };
        var expectedToken = "generated.jwt.token";

        _userRepository.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password)
            .Returns(true);
        _jwtTokenGenerator.GenerateToken(user)
            .Returns(expectedToken);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Token.Should().Be(expectedToken);
        result.Email.Should().Be(user.Email);
        result.Name.Should().Be(user.Username);
    }

    /// <summary>
    /// Tests that a non-existent email throws UnauthorizedAccessException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent email When authenticating Then throws UnauthorizedAccessException")]
    public async Task Handle_NonExistentEmail_ThrowsUnauthorizedAccessException()
    {
        // Given
        var command = new AuthenticateUserCommand
        {
            Email = "nonexistent@test.com",
            Password = "SomePassword123"
        };

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Invalid credentials*");
    }

    /// <summary>
    /// Tests that wrong password throws UnauthorizedAccessException.
    /// </summary>
    [Fact(DisplayName = "Given wrong password When authenticating Then throws UnauthorizedAccessException")]
    public async Task Handle_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        // Given
        var user = UserTestData.GenerateValidUser();
        user.Status = UserStatus.Active;
        var command = new AuthenticateUserCommand
        {
            Email = user.Email,
            Password = "WrongPassword123"
        };

        _userRepository.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password)
            .Returns(false);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Invalid credentials*");
    }

    /// <summary>
    /// Tests that an inactive user throws UnauthorizedAccessException.
    /// </summary>
    [Fact(DisplayName = "Given inactive user When authenticating Then throws UnauthorizedAccessException")]
    public async Task Handle_InactiveUser_ThrowsUnauthorizedAccessException()
    {
        // Given
        var user = UserTestData.GenerateValidUser();
        user.Status = UserStatus.Inactive;
        var command = new AuthenticateUserCommand
        {
            Email = user.Email,
            Password = "ValidPassword123"
        };

        _userRepository.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password)
            .Returns(true);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*not active*");
    }

    /// <summary>
    /// Tests that a suspended user throws UnauthorizedAccessException.
    /// </summary>
    [Fact(DisplayName = "Given suspended user When authenticating Then throws UnauthorizedAccessException")]
    public async Task Handle_SuspendedUser_ThrowsUnauthorizedAccessException()
    {
        // Given
        var user = UserTestData.GenerateValidUser();
        user.Status = UserStatus.Suspended;
        var command = new AuthenticateUserCommand
        {
            Email = user.Email,
            Password = "ValidPassword123"
        };

        _userRepository.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password)
            .Returns(true);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*not active*");
    }
}
