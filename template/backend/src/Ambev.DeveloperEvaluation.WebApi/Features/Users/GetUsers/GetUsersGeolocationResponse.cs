namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUsers;

/// <summary>
/// Represents geolocation coordinates in the response.
/// </summary>
public class GetUsersGeolocationResponse
{
    public string Lat { get; set; } = string.Empty;
    public string Long { get; set; } = string.Empty;
}
