namespace Ambev.DeveloperEvaluation.Application.Users.GetUser;

/// <summary>
/// Represents a user's address in the result.
/// </summary>
public class GetUserAddressResult
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public GetUserGeolocationResult Geolocation { get; set; } = new();
}
