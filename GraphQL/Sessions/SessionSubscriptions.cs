using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Sessions;

[SubscriptionType]
public static class SessionSubscriptions
{
    // The [Subscribe] attribute tells the schema builder that this resolver method needs to be hooked up to the pub/sub system. This means that in the background, 
    // the resolver compiler will create a so-called subscribe resolver that handles subscribing to the pub/sub system.
    [Subscribe]
    // The [Topic] attribute can be put on the method or a parameter of the method and will infer the pub/sub topic for this subscription.
    // With GraphQL a subscription stream can be infinite or finite. A finite stream will automatically complete whenever the server chooses to complete the topic (ITopicEventSender.CompleteAsync).
    // This example demonstrates an example of a static topic
    [Topic]
    // The [EventMessage] attribute marks the parameter where the execution engine will inject the message payload of the pub/sub system.k    
    public static async Task<Session> OnSessionScheduledAsync(
        [EventMessage] int sessionId,
        ISessionByIdDataLoader sessionById,
        CancellationToken cancellationToken)
    {
        return await sessionById.LoadRequiredAsync(sessionId, cancellationToken);
    }
}

// Note: We still need an event trigger
// A mutation is going to trigger this subscription