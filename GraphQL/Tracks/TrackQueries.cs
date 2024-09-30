using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Tracks;

[QueryType]
public static class TrackQueries
{
    // Example of applying the pagination to our resolver
    // Cursor-based paging is ideal whenever you implement infinite scrolling solutions. 
    // In contrast to offset pagination, you cannot jump to a specific page, but you can jump to a particular cursor and navigate from there.
    [UsePaging]
    // The return type mustbe IQueryable
    public static IQueryable<Track> GetTracks(ApplicationDbContext dbContext)
    {
        // Note: In order to use keyset pagination, we must always include a unique column in the ORDER BY clause (in this case, we also order by the primary key Id)
        return dbContext.Tracks.AsNoTracking().OrderBy(t => t.Name).ThenBy(t => t.Id);
    }

    [NodeResolver]
    public static async Task<Track?> GetTrackByIdAsync(
        int id,
        ITrackByIdDataLoader trackById,
        CancellationToken cancellationToken)
    {
        return await trackById.LoadAsync(id, cancellationToken);
    }

    public static async Task<IEnumerable<Track>> GetTracksByIdAsync(
        [ID<Track>] int[] ids,
        ITrackByIdDataLoader trackById,
        CancellationToken cancellationToken)
    {
        return await trackById.LoadRequiredAsync(ids, cancellationToken);
    }
}