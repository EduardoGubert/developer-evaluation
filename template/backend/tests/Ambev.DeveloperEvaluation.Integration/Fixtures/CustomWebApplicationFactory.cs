using Ambev.DeveloperEvaluation.Domain.Common.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Integration.Fixtures;

/// <summary>
/// Custom WebApplicationFactory for integration tests.
/// Replaces PostgreSQL with InMemory database and mocks external services.
/// Uses a shared database instance for all tests within the same test class.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string DatabaseName = $"IntegrationTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            services.RemoveAll(typeof(DbContextOptions<DefaultContext>));
            services.RemoveAll(typeof(DefaultContext));

            // Add InMemory database with shared name for test isolation within class
            services.AddDbContext<DefaultContext>(options =>
            {
                options.UseInMemoryDatabase(DatabaseName);
            });

            // Replace Redis cache with a mock
            services.RemoveAll(typeof(ICacheService));
            var mockCacheService = Substitute.For<ICacheService>();
            mockCacheService.GetAsync<object>(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<object?>(null));
            services.AddSingleton(mockCacheService);

            // Replace MongoDB EventStore with a mock
            services.RemoveAll(typeof(IEventStore));
            var mockEventStore = Substitute.For<IEventStore>();
            mockEventStore.AppendAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);
            services.AddSingleton(mockEventStore);

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<DefaultContext>();

            // Ensure the database is created
            db.Database.EnsureCreated();
        });

        builder.UseEnvironment("Development");
    }
}
