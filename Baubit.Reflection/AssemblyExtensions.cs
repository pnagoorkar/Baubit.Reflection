using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentResults;

namespace Baubit.Reflection
{
    public static class AssemblyExtensions
    {
        public static AssemblyName GetAssemblyNameFromPersistableString(string value)
        {
            var nameParts = value.Split('/');
            return new AssemblyName { Name = nameParts[0], Version = new Version(nameParts[1]) };
        }

        public static Assembly TryResolveAssembly(this AssemblyName assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().IsSameAs(assemblyName));
        }

        public static bool IsSameAs(this AssemblyName assemblyName, AssemblyName otherAssemblyName)
        {
            bool isNameEqual = otherAssemblyName.Name.Equals(assemblyName.Name, StringComparison.OrdinalIgnoreCase);

            if (otherAssemblyName.Version == null) return isNameEqual;

            bool isVersionEqual = otherAssemblyName.Version.Major == assemblyName.Version.Major &&
                                  otherAssemblyName.Version.Minor == assemblyName.Version.Minor &&
                                  otherAssemblyName.Version.Build == assemblyName.Version.Build;

            if (otherAssemblyName.Version.Revision != -1 && assemblyName.Version.Revision != -1) // both assemblies have a Revision explicitly defined. We therefore check that they are also equal
            {
                isVersionEqual = isVersionEqual && otherAssemblyName.Version.Revision == assemblyName.Version.Revision;
            }

            return isNameEqual && isVersionEqual;
        }

        public static async Task<Result<string>> ReadResource(this Assembly assembly, string resourceName)
        {
            return await Result.Try(() => assembly.GetManifestResourceStream(resourceName))
                               .Bind(stream => stream.ReadStringAsync());
        }

        public static Result<string> GetBaubitFormattedAssemblyQualifiedName(this Type type)
        {
            return Result.Try(() => type.AssemblyQualifiedName)
                         .Bind(assemblyQualifiedName => Result.Try(() => Regex.Replace(assemblyQualifiedName, @"(,\s*Version=[^,]+|,\s*Culture=[^,]+|,\s*PublicKeyToken=[^,\]]+(?=\]))", string.Empty)))
                         .Bind(assemblyQualifiedName => Result.Try(() => assemblyQualifiedName.Substring(0, assemblyQualifiedName.LastIndexOf(','))));
        }
        
        public static async Task<Result<string>> ReadStringAsync(this Stream stream)
        {
            try
            {
                if (stream == null) return Result.Fail("Cannot read from a null stream !");
                using (var reader = new StreamReader(stream))
                {
                    var str = await reader.ReadToEndAsync();
                    return Result.Ok(str);
                }
            }
            catch (Exception exp)
            {
                return Result.Fail(new ExceptionalError(exp));
            }
        }

        /// <summary>
        /// Creates an instance of the specified type using a constructor that matches the provided parameter types.
        /// </summary>
        /// <typeparam name="T">The type to cast the created instance to.</typeparam>
        /// <param name="type">The type to instantiate.</param>
        /// <param name="paramTypes">The types of the constructor parameters.</param>
        /// <param name="paramValues">The values to pass to the constructor.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the created instance on success, 
        /// or failure information if the constructor cannot be found or invocation fails.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method uses reflection to find a public instance constructor matching the specified 
        /// parameter types and invokes it with the provided values.
        /// </para>
        /// <para>
        /// The constructor must be public and have an exact match for the parameter types.
        /// Derived types in <paramref name="paramValues"/> are allowed, but <paramref name="paramTypes"/> 
        /// must exactly match the constructor signature.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var result = typeof(MyService).CreateInstance&lt;IMyService&gt;(
        ///     new[] { typeof(IConfiguration), typeof(ILogger) },
        ///     new object[] { config, logger }
        /// );
        /// if (result.IsSuccess)
        /// {
        ///     var service = result.Value;
        /// }
        /// </code>
        /// </example>
        public static Result<T> CreateInstance<T>(this Type type, Type[] paramTypes, object[] paramValues)
        {
            return Result.Try(() => type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, paramTypes, null))
                         .Bind(ctorInfo => Result.FailIf(ctorInfo == null, "No constructor found!")
                                                 .Bind(() => Result.Try(() => (T)ctorInfo.Invoke(paramValues))));
        }
    }
}
