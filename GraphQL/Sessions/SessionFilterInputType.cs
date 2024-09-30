using ConferencePlanner.GraphQL.Data;
using HotChocolate.Data.Filters;

namespace ConferencePlanner.GraphQL.Sessions;

public sealed class SessionFilterInputType : FilterInputType<Session>
{
    // Expose the specific fields that we're interested in the filter middleware
    protected override void Configure(IFilterInputTypeDescriptor<Session> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(s => s.Title);
        descriptor.Field(s => s.Abstract);
        descriptor.Field(s => s.StartTime);
        descriptor.Field(s => s.EndTime);
    }
}