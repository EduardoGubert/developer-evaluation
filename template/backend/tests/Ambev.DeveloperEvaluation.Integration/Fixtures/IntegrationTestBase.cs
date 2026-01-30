using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Ambev.DeveloperEvaluation.ORM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Fixtures;

/// <summary>
/// Base class for integration tests providing common functionality.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    protected readonly JsonSerializerOptions JsonOptions;

    private const string JwtSecretKey = "YourSuperSecretKeyForJwtTokenGenerationThatShouldBeAtLeast32BytesLong";

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Set default authorization header
        var token = GenerateJwtToken();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Generates a valid JWT token for authenticated requests.
    /// </summary>
    protected string GenerateJwtToken(string userId = "test-user-id", string email = "test@test.com", string role = "Admin")
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "DeveloperEvaluation",
            audience: "DeveloperEvaluation",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Gets a fresh database context for direct database operations in tests.
    /// </summary>
    protected DefaultContext GetDbContext()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<DefaultContext>();
    }

    /// <summary>
    /// Clears all sales from the database.
    /// </summary>
    protected async Task ClearSalesAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        db.SaleItems.RemoveRange(db.SaleItems);
        db.Sales.RemoveRange(db.Sales);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Posts JSON content to the specified URI.
    /// </summary>
    protected async Task<HttpResponseMessage> PostAsJsonAsync<T>(string uri, T content)
    {
        return await Client.PostAsJsonAsync(uri, content, JsonOptions);
    }

    /// <summary>
    /// Puts JSON content to the specified URI.
    /// </summary>
    protected async Task<HttpResponseMessage> PutAsJsonAsync<T>(string uri, T content)
    {
        return await Client.PutAsJsonAsync(uri, content, JsonOptions);
    }

    /// <summary>
    /// Deserializes JSON response content.
    /// </summary>
    protected async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    public void Dispose()
    {
        Client?.Dispose();
        GC.SuppressFinalize(this);
    }
}
