using System.Reflection;
using HotChocolate.Types.Descriptors;

namespace ConferencePlanner.GraphQL.Extensions;

// Expliniation:
// To use middleware on plain C# types, we can wrap them in so-called descriptor attributes. Descriptor attributes let us intercept the descriptors when the type is inferred. 
// For each descriptor type, there is a specific descriptor attribute base class. 
// For our case, we need to use the ObjectFieldDescriptorAttribute base class.
public sealed class UseUpperCaseAttribute : ObjectFieldDescriptorAttribute
{
    protected override void OnConfigure(
        IDescriptorContext context,
        IObjectFieldDescriptor descriptor,
        MemberInfo member)
    {
        descriptor.UseUpperCase();
    }
}

// Example of using the middleware with plain C# types
// This new attribute can now be applied to any property or method on a plain C# type.
// public sealed class Foo
// {
//     [UseUpperCase]
//     public required string Bar { get; init; }
// }