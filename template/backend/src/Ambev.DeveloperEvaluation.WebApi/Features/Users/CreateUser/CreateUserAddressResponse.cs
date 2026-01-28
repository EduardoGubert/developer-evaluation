namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;

/// <summary>
/// Represents a user's address in the response.
/// </summary>
public class CreateUserAddressResponse
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public CreateUserGeolocationResponse Geolocation { get; set; } = new();
}
