namespace ConferencePlanner.GraphQL.Extensions;

// Extension class that is going to contain an example of middleware methods to experiment with in GraphQL
public static class ObjectFieldDescriptorExtensions
{
    // Adding a UseUpper middleware
    // Explination: The above middleware first invokes the next middleware, and by doing so, gives up control and lets the rest of the pipeline do its job.
    // After next has finished executing, the middleware checks if the result is a string, and if so, it applies ToUpperInvariant on that string and writes back the updated string to context.Result.
    public static IObjectFieldDescriptor UseUpperCase(this IObjectFieldDescriptor descriptor)
    {
        return descriptor.Use(next => async context =>
        {
            await next(context);

            if (context.Result is string s)
            {
                context.Result = s.ToUpperInvariant();
            }
        });
    }
}