namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUsers;

/// <summary>
/// Represents a request to get a paginated list of users.
/// </summary>
public class GetUsersRequest
{
    /// <summary>
    /// Gets or sets the page number (default: 1).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page (default: 10).
    /// </summary>
    public int Size { get; set; } = 10;

    /// <summary>
    /// Gets or sets the ordering (e.g., "username asc", "email desc").
    /// </summary>
    public string? Order { get; set; }
}
