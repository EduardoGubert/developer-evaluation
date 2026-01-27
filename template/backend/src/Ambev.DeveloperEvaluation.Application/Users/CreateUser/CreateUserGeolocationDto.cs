namespace Ambev.DeveloperEvaluation.Application.Users.CreateUser;

/// <summary>
/// Represents geolocation coordinates.
/// </summary>
public class CreateUserGeolocationDto
{
    public string Lat { get; set; } = string.Empty;
    public string Long { get; set; } = string.Empty;
}
