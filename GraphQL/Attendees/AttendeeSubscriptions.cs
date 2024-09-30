using ConferencePlanner.GraphQL.Data;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace ConferencePlanner.GraphQL.Attendees;

// Note:
// OnAttendeeCheckedIn represents our resolver like in the first subscription that we built, but now in our Subscribe attribute we are referring to a method named 
// SubscribeToOnAttendeeCheckedInAsync. So, instead of letting the system generate a subscribe resolver 
// that handles subscribing to the pub/sub system, we are creating it ourselves in order to control how it's done, or to filter out events that we don't want to pass down.


[SubscriptionType]
public static class AttendeeSubscriptions
{
    // The subscribe resolver method
    [Subscribe(With = nameof(SubscribeToOnAttendeeCheckedInAsync))] // Methods refrence
    public static SessionAttendeeCheckIn OnAttendeeCheckedIn(
        [ID<Session>] int sessionId, // The subscribe resolver has access to all of the arguments that the actual resolver has access to.
        [EventMessage] int attendeeId)
    {
        return new SessionAttendeeCheckIn(attendeeId, sessionId);
    }

    // The subscription resolver
    public static async ValueTask<ISourceStream<int>> SubscribeToOnAttendeeCheckedInAsync(
        int sessionId,
        ITopicEventReceiver eventReceiver, // The subscribe resolver is using ITopicEventReceiver to subscribe to a topic.
        CancellationToken cancellationToken)
    {
        // A subscribe resolver can return IAsyncEnumerable<T>, IEnumerable<T>, or IObservable<T> to represent the subscription stream.
        return await eventReceiver.SubscribeAsync<int>(
            $"OnAttendeeCheckedIn_{sessionId}", // The dynamic creation of a topic
            cancellationToken);
    }
}