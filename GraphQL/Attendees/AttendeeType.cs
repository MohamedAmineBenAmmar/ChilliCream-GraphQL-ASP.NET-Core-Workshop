using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Attendees;

// Extension class for the Attendee
[ObjectType<Attendee>]
public static partial class AttendeeType
{
    // The process of defining this configure method is similiar to [NodeResolver]
    static partial void Configure(IObjectTypeDescriptor<Attendee> descriptor)
    {
        descriptor
            .ImplementsNode() // marks the type as implementing the Node interface.
            .IdField(a => a.Id) // specifies the ID member of the node type.
            .ResolveNode( // specifies a delegate to resolve the node from its ID.
                async (ctx, id)
                    => await ctx.DataLoader<IAttendeeByIdDataLoader>()
                        .LoadAsync(id, ctx.RequestAborted));
    }

    [BindMember(nameof(Attendee.SessionsAttendees))]
    public static async Task<IEnumerable<Session>> GetSessionsAsync(
        [Parent] Attendee attendee,
        ISessionsByAttendeeIdDataLoader sessionsByAttendeeId,
        CancellationToken cancellationToken)
    {
        return await sessionsByAttendeeId.LoadRequiredAsync(attendee.Id, cancellationToken);
    }
}