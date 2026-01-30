namespace Ambev.DeveloperEvaluation.Integration.Fixtures;

/// <summary>
/// Standard API response wrapper for integration tests.
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Standard API response wrapper with data for integration tests.
/// </summary>
public class ApiResponseWithData<T> : ApiResponse
{
    public T? Data { get; set; }
}

/// <summary>
/// Paginated response for list endpoints.
/// </summary>
public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
