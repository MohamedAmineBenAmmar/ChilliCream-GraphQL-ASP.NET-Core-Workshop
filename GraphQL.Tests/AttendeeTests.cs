using ConferencePlanner.GraphQL.Data;
using CookieCrumble;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace GraphQL.Tests;

public sealed class AttendeeTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16.3")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.4")
        .Build();

    private IRequestExecutor _requestExecutor = null!;

    public async Task InitializeAsync()
    {
        // Start test containers.
        await Task.WhenAll(_postgreSqlContainer.StartAsync(), _redisContainer.StartAsync());

        // Build request executor.
        _requestExecutor = await new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(_postgreSqlContainer.GetConnectionString()))
            .AddGraphQLServer()
            .AddGlobalObjectIdentification()
            .AddMutationConventions()
            .AddDbContextCursorPagingProvider()
            .AddPagingArguments()
            .AddFiltering()
            .AddSorting()
            .AddRedisSubscriptions(
                _ => ConnectionMultiplexer.Connect(_redisContainer.GetConnectionString()))
            .AddGraphQLTypes()
            .BuildRequestExecutorAsync();

        // Create database.
        var dbContext = _requestExecutor.Services
            .GetApplicationServices()
            .GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async Task RegisterAttendee()
    {
        // Arrange & act
        var result = await _requestExecutor.ExecuteAsync(
            """
            mutation RegisterAttendee {
                registerAttendee(
                    input: {
                        firstName: "Michael"
                        lastName: "Staib"
                        username: "michael"
                        emailAddress: "michael@chillicream.com"
                    }
                ) {
                    attendee {
                        id
                    }
                }
            }
            """);

        // Assert
        result.MatchSnapshot(extension: ".json");
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
    }
}

// Explination: In the above test, we use Testcontainers for PostgreSQL and Redis, for realistic integration testing, as opposed to using in-memory providers.
// To execute against a schema we can call BuildRequestExecutorAsync on the service collection to get an IRequestExecutor. 
// After executing the mutation, we snapshot the result object, and as with the previous test, subsequent test runs will compare our snapshot file.