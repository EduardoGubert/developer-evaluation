namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;

/// <summary>
/// Represents geolocation coordinates in the response.
/// </summary>
public class CreateUserGeolocationResponse
{
    public string Lat { get; set; } = string.Empty;
    public string Long { get; set; } = string.Empty;
}
