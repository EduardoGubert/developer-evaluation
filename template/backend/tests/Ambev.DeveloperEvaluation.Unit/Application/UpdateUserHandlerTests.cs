using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the UpdateUserHandler class.
/// </summary>
public class UpdateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UpdateUserHandler _handler;

    public UpdateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _handler = new UpdateUserHandler(_userRepository, _mapper, _passwordHasher);
    }

    /// <summary>
    /// Tests that a valid update command returns the updated user.
    /// </summary>
    [Fact(DisplayName = "Given valid update command When updating user Then returns updated user")]
    public async Task Handle_ValidCommand_ReturnsUpdatedUser()
    {
        // Given
        var existingUser = UserTestData.GenerateValidUser();
        existingUser.Id = Guid.NewGuid(); // Set a valid ID for testing
        var command = new UpdateUserCommand
        {
            Id = existingUser.Id,
            Username = "updateduser",
            Email = existingUser.Email, // Keep same email
            Phone = "+5511999999999",
            Status = UserStatus.Active,
            Role = UserRole.Customer,
            Name = new UpdateUserNameDto
            {
                Firstname = "Updated",
                Lastname = "User"
            },
            Address = new UpdateUserAddressDto
            {
                City = "São Paulo",
                Street = "Av. Paulista",
                Number = 1000,
                Zipcode = "01310-100",
                Geolocation = new UpdateUserGeolocationDto
                {
                    Lat = "-23.5505",
                    Long = "-46.6333"
                }
            }
        };

        var expectedResult = new UpdateUserResult
        {
            Id = existingUser.Id,
            Username = command.Username,
            Email = command.Email
        };

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingUser);
        _userRepository.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(existingUser);
        _mapper.Map<UpdateUserResult>(Arg.Any<User>())
            .Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(existingUser.Id);
        await _userRepository.Received(1).UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that updating a non-existent user throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent user ID When updating user Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentUser_ThrowsKeyNotFoundException()
    {
        // Given
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@test.com",
            Phone = "+5511999999999",
            Status = UserStatus.Active,
            Role = UserRole.Customer,
            Name = new UpdateUserNameDto
            {
                Firstname = "Test",
                Lastname = "User"
            },
            Address = new UpdateUserAddressDto
            {
                City = "City",
                Street = "Street",
                Number = 1,
                Zipcode = "12345",
                Geolocation = new UpdateUserGeolocationDto
                {
                    Lat = "0",
                    Long = "0"
                }
            }
        };

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{command.Id}*not found*");
    }

    /// <summary>
    /// Tests that changing email to an existing email throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given existing email When updating user Then throws InvalidOperationException")]
    public async Task Handle_ExistingEmail_ThrowsInvalidOperationException()
    {
        // Given
        var existingUser = UserTestData.GenerateValidUser();
        existingUser.Id = Guid.NewGuid(); // Set a valid ID for testing
        var anotherUser = UserTestData.GenerateValidUser();
        anotherUser.Id = Guid.NewGuid(); // Set a valid ID for another user
        var command = new UpdateUserCommand
        {
            Id = existingUser.Id,
            Username = "testuser",
            Email = anotherUser.Email, // Try to change to another user's email
            Phone = "+5511999999999",
            Status = UserStatus.Active,
            Role = UserRole.Customer,
            Name = new UpdateUserNameDto
            {
                Firstname = "Test",
                Lastname = "User"
            },
            Address = new UpdateUserAddressDto
            {
                City = "City",
                Street = "Street",
                Number = 1,
                Zipcode = "12345",
                Geolocation = new UpdateUserGeolocationDto
                {
                    Lat = "0",
                    Long = "0"
                }
            }
        };

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingUser);
        _userRepository.GetByEmailAsync(anotherUser.Email, Arg.Any<CancellationToken>())
            .Returns(anotherUser);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*{anotherUser.Email}*already exists*");
    }

    /// <summary>
    /// Tests that invalid command data throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid command When updating user Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Given
        var command = new UpdateUserCommand(); // Empty command will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
