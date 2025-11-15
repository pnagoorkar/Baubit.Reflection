using Baubit.Reflection.Reasons;

namespace Baubit.Reflection.Test.TypeResolver
{
    public class Test
    {
        [Fact]
        public void TryResolveType_WithValidSystemType_ShouldReturnSuccess()
        {
            // Arrange
            var assemblyQualifiedName = typeof(string).AssemblyQualifiedName!;

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(typeof(string), result.Value);
        }

        [Fact]
        public void TryResolveType_WithValidCustomType_ShouldReturnSuccess()
        {
            // Arrange
            var assemblyQualifiedName = typeof(Test).AssemblyQualifiedName!;

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(typeof(Test), result.Value);
        }

        [Fact]
        public void TryResolveType_WithInvalidTypeName_ShouldReturnFailure()
        {
            // Arrange
            var assemblyQualifiedName = "NonExistent.Type.Name, NonExistent.Assembly";

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public void TryResolveType_WithInvalidTypeName_ShouldContainTypeNotDefinedReason()
        {
            // Arrange
            var assemblyQualifiedName = "NonExistent.Type.Name, NonExistent.Assembly";

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Reasons, r => r is TypeNotDefined);
        }

        [Fact]
        public void TryResolveType_WithInvalidTypeName_ReasonShouldContainTypeName()
        {
            // Arrange
            var assemblyQualifiedName = "NonExistent.Type.Name, NonExistent.Assembly";

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsFailed);
            var typeNotDefinedReason = result.Reasons.OfType<TypeNotDefined>().FirstOrDefault();
            Assert.NotNull(typeNotDefinedReason);
            Assert.Contains(assemblyQualifiedName, typeNotDefinedReason!.Message);
        }

        [Fact]
        public void TryResolveType_WithGenericType_ShouldReturnSuccess()
        {
            // Arrange
            var assemblyQualifiedName = typeof(List<int>).AssemblyQualifiedName!;

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(typeof(List<int>), result.Value);
        }

        [Fact]
        public void TryResolveType_WithComplexGenericType_ShouldReturnSuccess()
        {
            // Arrange
            var assemblyQualifiedName = typeof(Dictionary<string, List<int>>).AssemblyQualifiedName!;

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(typeof(Dictionary<string, List<int>>), result.Value);
        }

        [Fact]
        public void TryResolveType_WithArrayType_ShouldReturnSuccess()
        {
            // Arrange
            var assemblyQualifiedName = typeof(int[]).AssemblyQualifiedName!;

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(typeof(int[]), result.Value);
        }

        [Fact]
        public void TryResolveType_WithNestedType_ShouldReturnSuccess()
        {
            // Arrange
            var assemblyQualifiedName = typeof(OuterTestClass.InnerTestClass).AssemblyQualifiedName!;

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(typeof(OuterTestClass.InnerTestClass), result.Value);
        }

        [Fact]
        public void TryResolveType_WithEmptyString_ShouldReturnFailure()
        {
            // Arrange
            var assemblyQualifiedName = string.Empty;

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public void TryResolveType_WithPartialTypeName_ShouldReturnSuccess()
        {
            // Arrange - using just the type name without assembly qualification
            var typeName = "System.String";

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(typeName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(typeof(string), result.Value);
        }

        [Fact]
        public void TryResolveType_WithMalformedTypeName_ShouldReturnFailure()
        {
            // Arrange
            var assemblyQualifiedName = "This.Is.Not.A.Valid.Type.Name.At.All";

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Reasons, r => r is TypeNotDefined);
        }

        [Fact]
        public void TryResolveType_WithNullableType_ShouldReturnSuccess()
        {
            // Arrange
            var assemblyQualifiedName = typeof(int?).AssemblyQualifiedName!;

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(typeof(int?), result.Value);
        }

        [Fact]
        public void TryResolveType_WithInterfaceType_ShouldReturnSuccess()
        {
            // Arrange
            var assemblyQualifiedName = typeof(IEnumerable<int>).AssemblyQualifiedName!;

            // Act
            var result = Baubit.Reflection.TypeResolver.TryResolveType(assemblyQualifiedName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(typeof(IEnumerable<int>), result.Value);
        }

        // Helper class for nested type testing
        public class OuterTestClass
        {
            public class InnerTestClass
            {
            }
        }
    }
}
