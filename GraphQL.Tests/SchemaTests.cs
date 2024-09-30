using ConferencePlanner.GraphQL.Data;
using Microsoft.Extensions.DependencyInjection;
using CookieCrumble;
using HotChocolate.Execution;

namespace GraphQL.Tests;

public sealed class SchemaTests
{
    [Fact]
    public async Task SchemaChanged()
    {
        // Arrange & act
        var schema = await new ServiceCollection()
            .AddDbContext<ApplicationDbContext>()
            .AddGraphQLServer()
            .AddGlobalObjectIdentification()
            .AddMutationConventions()
            .AddDbContextCursorPagingProvider()
            .AddPagingArguments()
            .AddFiltering()
            .AddSorting()
            .AddInMemorySubscriptions()
            .AddGraphQLTypes()
            .BuildSchemaAsync();

        // Assert
        schema.MatchSnapshot();
    }
}
// Explination: The above test takes the service collection and builds a schema from it. 
// We call MatchSnapshot to create a snapshot of the GraphQL SDL representation of the schema, which is compared in subsequent test runs.