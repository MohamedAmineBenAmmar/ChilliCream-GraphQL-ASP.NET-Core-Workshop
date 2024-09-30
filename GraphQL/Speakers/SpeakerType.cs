using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Speakers;

// This class represents a type extension for the Speaker class where we are going to bind the created field to the SessionSpeakers field that holds the join objects
[ObjectType<Speaker>]
public static partial class SpeakerType
{
    // In this type extension, we replace the existing sessionSpeakers field (property SessionSpeakers), with a new field named sessions (method GetSessionsAsync), 
    // using the [BindMember] attribute. The new field exposes the sessions associated with the speaker.
    [BindMember(nameof(Speaker.SessionSpeakers))]
    public static async Task<IEnumerable<Session>> GetSessionsAsync(
        [Parent] Speaker speaker,
        ISessionsBySpeakerIdDataLoader sessionsBySpeakerId,
        CancellationToken cancellationToken)
    {
        return await sessionsBySpeakerId.LoadRequiredAsync(speaker.Id, cancellationToken);
    }
}