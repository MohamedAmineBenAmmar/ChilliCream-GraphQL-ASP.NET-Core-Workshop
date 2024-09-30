using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Sessions;

[QueryType]
public static class SessionQueries
{
    // 1st version
    // public static async Task<IEnumerable<Session>> GetSessionsAsync(
    //     ApplicationDbContext dbContext,
    //     CancellationToken cancellationToken)
    // {
    //     return await dbContext.Sessions.AsNoTracking().ToListAsync(cancellationToken);
    // }

    // 2sec version enhanced with pagination
    // [UsePaging]
    // public static IQueryable<Session> GetSessions(ApplicationDbContext dbContext)
    // {
    //     return dbContext.Sessions.AsNoTracking().OrderBy(s => s.Title);
    // }

    // 3rd version enhanced with filtering and sorting
    // Note: Filtering, like paging, is a middleware that can be applied on IQueryable. 
    // As mentioned in the middleware session, order is important with middleware. This means that our paging middleware has to execute last.
    // Note: By default, the filter middleware would infer a filter type that exposes all the fields of the entity. 
    // In our case, it would be better to be explicit, by specifying exactly which fields our users can filter by.
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Session> GetSessions(ApplicationDbContext dbContext)
    {
        // Note: In order to use keyset pagination, we must always include a unique column in the ORDER BY clause (in this case, we also order by the primary key Id)
        return dbContext.Sessions.AsNoTracking().OrderBy(s => s.Title).ThenBy(s => s.Id);
    }

    [NodeResolver]
    public static async Task<Session?> GetSessionByIdAsync(
        int id,
        ISessionByIdDataLoader sessionById,
        CancellationToken cancellationToken)
    {
        return await sessionById.LoadAsync(id, cancellationToken);
    }

    public static async Task<IEnumerable<Session>> GetSessionsByIdAsync(
        [ID<Session>] int[] ids,
        ISessionByIdDataLoader sessionById,
        CancellationToken cancellationToken)
    {
        return await sessionById.LoadRequiredAsync(ids, cancellationToken);
    }
}