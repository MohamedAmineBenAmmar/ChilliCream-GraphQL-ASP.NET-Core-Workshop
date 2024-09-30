using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Speakers;

[QueryType]
public static class SpeakerQueries
{
    // Example of old code without adding the pagination annotation
    // public static async Task<IEnumerable<Speaker>> GetSpeakersAsync(
    //     ApplicationDbContext dbContext,
    //     CancellationToken cancellationToken)
    // {
    //     return await dbContext.Speakers.AsNoTracking().ToListAsync(cancellationToken);
    // }

    // Example of the same resolver after adding the pagination annotation
    [UsePaging]
    public static IQueryable<Speaker> GetSpeakers(ApplicationDbContext dbContext)
    {
        // Note: In order to use keyset pagination, we must always include a unique column in the ORDER BY clause (in this case, we also order by the primary key Id)
        return dbContext.Speakers.AsNoTracking().OrderBy(s => s.Name).ThenBy(s => s.Id);
    }

    [NodeResolver]
    public static async Task<Speaker?> GetSpeakerByIdAsync(
        int id,
        ISpeakerByIdDataLoader speakerById,
        CancellationToken cancellationToken)
    {
        return await speakerById.LoadAsync(id, cancellationToken);
    }

    // Plural version of the previous quey
    public static async Task<IEnumerable<Speaker>> GetSpeakersByIdAsync(
    [ID<Speaker>] int[] ids,
    ISpeakerByIdDataLoader speakerById, 
    CancellationToken cancellationToken)
    {
        return await speakerById.LoadRequiredAsync(ids, cancellationToken); // Note that the DataLoader can also fetch multiple entities for us.
    }
}