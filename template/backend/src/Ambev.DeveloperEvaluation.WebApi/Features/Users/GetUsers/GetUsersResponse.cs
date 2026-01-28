namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUsers;

/// <summary>
/// API response model for GetUsers operation with pagination
/// </summary>
public class GetUsersResponse
{
    /// <summary>
    /// The list of users
    /// </summary>
    public List<GetUsersItemResponse> Data { get; set; } = new();

    /// <summary>
    /// Total number of items
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }
}
