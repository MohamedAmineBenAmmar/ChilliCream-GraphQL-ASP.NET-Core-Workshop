using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Speakers;

[MutationType]
public static class SpeakerMutations
{
    // Previously we used to create the type that represents the payload thath the mutation is going to return.
    // Since we add it the mutation conventions the payload types are going to be generated automatically by HotChocolate.
    public static async Task<Speaker> AddSpeakerAsync(
        AddSpeakerInput input,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var speaker = new Speaker
        {
            Name = input.Name,
            Bio = input.Bio,
            Website = input.Website
        };

        dbContext.Speakers.Add(speaker);

        await dbContext.SaveChangesAsync(cancellationToken);

        return speaker;
    }
}