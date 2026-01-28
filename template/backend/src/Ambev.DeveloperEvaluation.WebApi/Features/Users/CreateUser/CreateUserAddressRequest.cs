namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;

/// <summary>
/// Represents a user's address in the request.
/// </summary>
public class CreateUserAddressRequest
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public CreateUserGeolocationRequest Geolocation { get; set; } = new();
}
