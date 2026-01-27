namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

/// <summary>
/// Result for UpdateUser operation.
/// </summary>
public class UpdateUserResult
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's name.
    /// </summary>
    public UpdateUserNameResult Name { get; set; } = new();

    /// <summary>
    /// Gets or sets the user's address.
    /// </summary>
    public UpdateUserAddressResult Address { get; set; } = new();

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user role.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user status.
    /// </summary>
    public string Status { get; set; } = string.Empty;
}
