using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Extensions;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;

namespace ConferencePlanner.GraphQL.Tracks;

[ObjectType<Track>]
public static partial class TrackType
{
    // An example that demonstrates the use of the UseUpperCase middleware with the GraphQL extension class
    static partial void Configure(IObjectTypeDescriptor<Track> descriptor)
    {
        descriptor
            .Field(t => t.Name)
            .UseUpperCase(); // We used this syntax style because we already created an extension method to IObjectFieldDescriptor in the ObjectFieldDescriptorExtensions that we created
    }

    // An example of the usage of the UsePaging without modifying the IEnumerable to IQueryable
    // Cache optimization:
    // There is one caveat in our implementation with the TrackType. Since we are using a DataLoader within our resolver and first fetch the list of IDs, 
    // we'll essentially always fetch everything and slice in memory. In an actual project this can be split into two actions by moving the DataLoader part into a middleware
    //  and first paging on the ID queryable.
    //  Also, one could implement a special IPagingHandler that uses the DataLoader and applies paging logic.
    // [UsePaging]
    // public static async Task<IEnumerable<Session>> GetSessionsAsync(
    //     [Parent] Track track,
    //     ISessionsByTrackIdDataLoader sessionsByTrackId,
    //     CancellationToken cancellationToken)
    // {
    //     return await sessionsByTrackId.LoadRequiredAsync(track.Id, cancellationToken);
    // }

    // Example of the method GetSessionsAsync after adding the keyset pagination update
    // Explination: Here, we apply the [UsePaging] attribute, and forward the paging arguments to the DataLoader. We also convert the returned Page<T> to a Connection<T>.
    [UsePaging]
    public static async Task<Connection<Session>> GetSessionsAsync(
    [Parent] Track track,
    ISessionsByTrackIdDataLoader sessionsByTrackId,
    PagingArguments pagingArguments,
    CancellationToken cancellationToken)
    {
        return await sessionsByTrackId
            .WithPagingArguments(pagingArguments)
            .LoadAsync(track.Id, cancellationToken)
            .ToConnectionAsync();
    }
}