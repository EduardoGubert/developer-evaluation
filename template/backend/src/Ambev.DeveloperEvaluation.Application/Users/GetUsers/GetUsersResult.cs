namespace Ambev.DeveloperEvaluation.Application.Users.GetUsers;

/// <summary>
/// Result for GetUsers operation containing paginated user data.
/// </summary>
public class GetUsersResult
{
    /// <summary>
    /// Gets or sets the list of users.
    /// </summary>
    public List<GetUsersItemResult> Data { get; set; } = new();

    /// <summary>
    /// Gets or sets the total number of items.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages.
    /// </summary>
    public int TotalPages { get; set; }
}
