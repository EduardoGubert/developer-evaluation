namespace Ambev.DeveloperEvaluation.Application.Users.CreateUser;

/// <summary>
/// Represents a user's address in the result.
/// </summary>
public class CreateUserAddressResult
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public CreateUserGeolocationResult Geolocation { get; set; } = new();
}
