using Baubit.Reflection.Reasons;
using Baubit.Traceability;
using FluentResults;
using System;

namespace Baubit.Reflection
{
    public sealed class TypeResolver
    {
        public static Result<Type> TryResolveType(string assemblyQualifiedName)
        {
            return Result.Try(() => Type.GetType(assemblyQualifiedName))
                         .Bind(type => Result.FailIf(type == null, new Error(string.Empty))
                                             .AddReasonIfFailed(new TypeNotDefined(assemblyQualifiedName))
                                             .Bind(() => Result.Ok(type)));
        }
    }
}
