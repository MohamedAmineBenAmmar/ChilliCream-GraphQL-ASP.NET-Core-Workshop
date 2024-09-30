using ConferencePlanner.GraphQL.Data;
using HotChocolate.Subscriptions;

namespace ConferencePlanner.GraphQL.Sessions;

[MutationType]
public static class SessionMutations
{
    // The Error<TError> attribute registers a middleware that will catch all exceptions of type TError 
    // on mutations and queries. By annotating the attribute the response type of the annotated resolver will be automatically extended.
    [Error<TitleEmptyException>]
    [Error<NoSpeakerException>]
    public static async Task<Session> AddSessionAsync(
        AddSessionInput input,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(input.Title))
        {
            throw new TitleEmptyException();
        }

        if (input.SpeakerIds.Count == 0)
        {
            throw new NoSpeakerException();
        }

        var session = new Session
        {
            Title = input.Title,
            Abstract = input.Abstract
        };

        foreach (var speakerId in input.SpeakerIds)
        {
            session.SessionSpeakers.Add(new SessionSpeaker
            {
                SpeakerId = speakerId
            });
        }

        dbContext.Sessions.Add(session);

        await dbContext.SaveChangesAsync(cancellationToken);

        return session;
    }

    // Example of old code before adding the GraphqQL event trigger
    // [Error<EndTimeInvalidException>]
    // [Error<SessionNotFoundException>]
    // public static async Task<Session> ScheduleSessionAsync(
    // ScheduleSessionInput input,
    // ApplicationDbContext dbContext,
    // CancellationToken cancellationToken)
    // {
    //     if (input.EndTime < input.StartTime)
    //     {
    //         throw new EndTimeInvalidException();
    //     }

    //     var session = await dbContext.Sessions.FindAsync([input.SessionId], cancellationToken);

    //     if (session is null)
    //     {
    //         throw new SessionNotFoundException();
    //     }

    //     session.TrackId = input.TrackId;
    //     session.StartTime = input.StartTime;
    //     session.EndTime = input.EndTime;

    //     await dbContext.SaveChangesAsync(cancellationToken);

    //     return session;
    // }

    // New function code after adding the GraphQL trigger
    [Error<EndTimeInvalidException>]
    [Error<SessionNotFoundException>]
    public static async Task<Session> ScheduleSessionAsync(
    ScheduleSessionInput input,
    ApplicationDbContext dbContext,
    ITopicEventSender eventSender, // Our improved resolver now injects ITopicEventSender eventSender. This gives us access to send messages to the underlying pub/sub system.
    CancellationToken cancellationToken)
    {
        if (input.EndTime < input.StartTime)
        {
            throw new EndTimeInvalidException();
        }

        var session = await dbContext.Sessions.FindAsync([input.SessionId], cancellationToken);

        if (session is null)
        {
            throw new SessionNotFoundException();
        }

        session.TrackId = input.TrackId;
        session.StartTime = input.StartTime;
        session.EndTime = input.EndTime;

        await dbContext.SaveChangesAsync(cancellationToken);

        await eventSender.SendAsync(
            nameof(SessionSubscriptions.OnSessionScheduledAsync), // Example of working with a static topic
            session.Id,
            cancellationToken);

        return session;
    }
}