namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;

/// <summary>
/// Represents geolocation coordinates in the request.
/// </summary>
public class CreateUserGeolocationRequest
{
    public string Lat { get; set; } = string.Empty;
    public string Long { get; set; } = string.Empty;
}
