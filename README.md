# Baubit.Reflection

[![CircleCI](https://dl.circleci.com/status-badge/img/circleci/TpM4QUH8Djox7cjDaNpup5/2zTgJzKbD2m3nXCf5LKvqS/tree/master.svg?style=svg)](https://dl.circleci.com/status-badge/redirect/circleci/TpM4QUH8Djox7cjDaNpup5/2zTgJzKbD2m3nXCf5LKvqS/tree/master)
[![codecov](https://codecov.io/gh/pnagoorkar/Baubit.Reflection/branch/master/graph/badge.svg)](https://codecov.io/gh/pnagoorkar/Baubit.Reflection)<br/>
[![NuGet](https://img.shields.io/nuget/v/Baubit.Reflection.svg)](https://www.nuget.org/packages/Baubit.Reflection/)
![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-512BD4?logo=dotnet&logoColor=white)<br/>
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Known Vulnerabilities](https://snyk.io/test/github/pnagoorkar/Baubit.Reflection/badge.svg)](https://snyk.io/test/github/pnagoorkar/Baubit.Reflection)

A utility library for .NET that provides enhanced reflection capabilities with functional error handling using FluentResults. This library simplifies common reflection tasks such as assembly resolution, type resolution, and assembly-qualified name formatting.

## Features

- **Assembly Management**: Advanced assembly name parsing and resolution with version-aware comparison
- **Type Resolution**: Safe type resolution with descriptive error handling
- **Assembly Name Formatting**: Generate clean, simplified assembly-qualified type names (Baubit format)
- **Resource Reading**: Read embedded resources from assemblies with error handling
- **Functional Error Handling**: All operations return `Result<T>` types for safe error handling using FluentResults

## Installation

Install the package via NuGet:

```bash
dotnet add package Baubit.Reflection
```

Or via the NuGet Package Manager:

```
Install-Package Baubit.Reflection
```

## Usage

### Assembly Name Resolution

#### Parse Assembly Names from Persistable Strings

```csharp
using Baubit.Reflection;

// Parse assembly name from "Name/Version" format
var assemblyName = AssemblyExtensions.GetAssemblyNameFromPersistableString("MyAssembly/1.2.3.4");
// assemblyName.Name = "MyAssembly"
// assemblyName.Version = Version(1, 2, 3, 4)
```

#### Resolve Loaded Assemblies

```csharp
using Baubit.Reflection;

var assemblyName = new AssemblyName("System.Runtime");
var assembly = assemblyName.TryResolveAssembly();

if (assembly != null)
{
    Console.WriteLine($"Found assembly: {assembly.FullName}");
}
```

#### Compare Assembly Names with Version-Aware Logic

```csharp
using Baubit.Reflection;

var assembly1 = new AssemblyName { Name = "MyLib", Version = new Version(1, 2, 3, 4) };
var assembly2 = new AssemblyName { Name = "MyLib", Version = new Version(1, 2, 3) };

// Compares Major, Minor, Build; ignores Revision if one is undefined
bool isSame = assembly1.IsSameAs(assembly2); // true (revision difference ignored)
```

### Type Resolution

#### Safely Resolve Types

```csharp
using Baubit.Reflection;

var assemblyQualifiedName = "System.String, System.Private.CoreLib";
var result = TypeResolver.TryResolveType(assemblyQualifiedName);

if (result.IsSuccess)
{
    Type resolvedType = result.Value;
    Console.WriteLine($"Resolved type: {resolvedType.FullName}");
}
else
{
    // Check for specific error reasons
    var typeNotDefinedReason = result.Reasons.OfType<TypeNotDefined>().FirstOrDefault();
    Console.WriteLine($"Failed: {typeNotDefinedReason?.Message}");
}
```

### Assembly-Qualified Name Formatting

#### Generate Clean Type Names (Baubit Format)

```csharp
using Baubit.Reflection;

var type = typeof(Dictionary<string, List<int>>);
var result = type.GetBaubitFormattedAssemblyQualifiedName();

if (result.IsSuccess)
{
    // Returns type name without Version, Culture, PublicKeyToken
    Console.WriteLine(result.Value);
    // Example: System.Collections.Generic.Dictionary`2[[System.String],[System.Collections.Generic.List`1[[System.Int32]]]]
}
```

### Read Embedded Resources

#### Read Assembly Resources Asynchronously

```csharp
using Baubit.Reflection;

var assembly = Assembly.GetExecutingAssembly();
var result = await assembly.ReadResource("MyNamespace.EmbeddedFile.txt");

if (result.IsSuccess)
{
    string content = result.Value;
    Console.WriteLine($"Resource content: {content}");
}
else
{
    Console.WriteLine($"Failed to read resource: {result.Errors.First().Message}");
}
```

### Stream Utilities

#### Read Streams as Strings

```csharp
using Baubit.Reflection;

using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, World!"));
var result = await stream.ReadStringAsync();

if (result.IsSuccess)
{
    Console.WriteLine(result.Value); // "Hello, World!"
}
```

## Key Classes and Methods

### `AssemblyExtensions`

| Method | Description |
|--------|-------------|
| `GetAssemblyNameFromPersistableString(string)` | Parse assembly name from "Name/Version" format |
| `TryResolveAssembly(this AssemblyName)` | Attempt to resolve an assembly from loaded assemblies |
| `IsSameAs(this AssemblyName, AssemblyName)` | Compare two assembly names with version-aware logic |
| `ReadResource(this Assembly, string)` | Read an embedded resource as a string |
| `GetBaubitFormattedAssemblyQualifiedName(this Type)` | Get simplified assembly-qualified name without version metadata |
| `ReadStringAsync(this Stream)` | Read a stream's content as a string |

### `TypeResolver`

| Method | Description |
|--------|-------------|
| `TryResolveType(string)` | Safely resolve a type from its assembly-qualified name |

### Error Reasons

| Reason | Description |
|--------|-------------|
| `TypeNotDefined` | Thrown when a type cannot be resolved from the given assembly-qualified name |

## Dependencies

- **FluentResults**: For functional error handling
- **Baubit.Traceability**: For custom error reason types
- **.NET 9**: Target framework

## Development

### Building the Project

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Creating NuGet Package

```bash
dotnet pack -c Release
```

## CI/CD

This project uses CircleCI for continuous integration and deployment:

- **Build**: Automated on every commit
- **Test**: Unit tests with code coverage reported to Codecov
- **Pack & Publish**: NuGet packages published from the master branch
- **Release**: Production releases from the release branch

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## License

See the [LICENSE](LICENSE) file for details.

## Author

Prashant Nagoorkar

## Links

- [GitHub Repository](https://github.com/pnagoorkar/Baubit.Reflection)
- [NuGet Package](https://www.nuget.org/packages/Baubit.Reflection/)
