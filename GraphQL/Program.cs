using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    // We did this so that it can be injected into resolvers.
    .AddDbContext<ApplicationDbContext>(
        options => options.UseNpgsql("Host=127.0.0.1;Username=graphql_workshop;Password=secret"))

    // The following code adds a GraphQL server configuration to the dependency injection container.
    .AddGraphQLServer()

    // Enabiling the Relay server specification called Object Identification.
    .AddGlobalObjectIdentification()

    // Enabiling the GraphQL mutations conventions 
    // Hot Chocolate has built-in conventions for mutations to minimize boilerplate code. 
    // Instead of manually creating payload types, Hot Chocolate can generate these types for us automatically.
    .AddMutationConventions()

    // This will add a cursor paging provider that uses native keyset pagination.
    .AddDbContextCursorPagingProvider()
    
    // Add paging arguments to the schema configuration
    // This will make paging arguments available to resolver methods.
    .AddPagingArguments()

    // Add filtering and sorting conventions to the schema configuration
    .AddFiltering()
    .AddSorting()

    // Add Redis subscriptions to the GraphQL configuration
    .AddRedisSubscriptions(_ => ConnectionMultiplexer.Connect("127.0.0.1:6379")) // With AddRedisSubscriptions(...) we've added a Redis pub/sub system for GraphQL subscriptions to our schema.

    // This registers all types in the assembly using a source generator (HotChocolate.Types.Analyzers).
    .AddGraphQLTypes();

var app = builder.Build();

// Adding the web sockets to the request pipeline
// The order of the middlewares is important
// Put it before the GraphQL middleware in the request pipeline
// we've enabled our server to handle websocket requests.
app.UseWebSockets(); // => With app.UseWebSockets() we've enabled our server to handle websocket requests.

// Adds a GraphQL endpoint to the endpoint configurations
// Configure the GraphQL middleware so that the server knows how to execute GraphQL requests.
app.MapGraphQL();

await app.RunWithGraphQLCommandsAsync(args);