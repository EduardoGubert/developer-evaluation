using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Common.Security;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

/// <summary>
/// Handler for processing UpdateUserCommand requests
/// </summary>
public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initializes a new instance of UpdateUserHandler
    /// </summary>
    /// <param name="userRepository">The user repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="passwordHasher">The password hasher</param>
    public UpdateUserHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Handles the UpdateUserCommand request
    /// </summary>
    /// <param name="command">The UpdateUser command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated user details</returns>
    public async Task<UpdateUserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateUserCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingUser = await _userRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingUser == null)
            throw new KeyNotFoundException($"User with ID {command.Id} not found");

        // Check if email is being changed and if it's already taken
        if (existingUser.Email != command.Email)
        {
            var userWithEmail = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (userWithEmail != null)
                throw new InvalidOperationException($"User with email {command.Email} already exists");
        }

        // Update user properties
        existingUser.Username = command.Username;
        existingUser.Email = command.Email;
        existingUser.Phone = command.Phone;
        existingUser.Status = command.Status;
        existingUser.Role = command.Role;
        existingUser.UpdatedAt = DateTime.UtcNow;

        // Update name properties
        existingUser.Firstname = command.Name.Firstname;
        existingUser.Lastname = command.Name.Lastname;

        // Update address properties
        existingUser.City = command.Address.City;
        existingUser.Street = command.Address.Street;
        existingUser.Number = command.Address.Number;
        existingUser.Zipcode = command.Address.Zipcode;
        existingUser.Latitude = command.Address.Geolocation.Lat;
        existingUser.Longitude = command.Address.Geolocation.Long;

        // Update password only if provided
        if (!string.IsNullOrEmpty(command.Password))
        {
            existingUser.Password = _passwordHasher.HashPassword(command.Password);
        }

        var updatedUser = await _userRepository.UpdateAsync(existingUser, cancellationToken);
        var result = _mapper.Map<UpdateUserResult>(updatedUser);
        return result;
    }
}
