namespace Ambev.DeveloperEvaluation.Application.Users.GetUsers;

/// <summary>
/// Represents a user's address in the list result.
/// </summary>
public class GetUsersAddressResult
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public GetUsersGeolocationResult Geolocation { get; set; } = new();
}
