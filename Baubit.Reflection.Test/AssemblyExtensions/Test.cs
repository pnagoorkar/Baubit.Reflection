using System.Reflection;
using System.Text;
using Xunit;

namespace Baubit.Reflection.Test.AssemblyExtensions
{
    public class Test
    {
        #region GetAssemblyNameFromPersistableString Tests

        [Fact]
        public void GetAssemblyNameFromPersistableString_WithValidString_ShouldParseCorrectly()
        {
            // Arrange
            var persistableString = "MyAssembly/1.2.3.4";

            // Act
            var assemblyName = Baubit.Reflection.AssemblyExtensions.GetAssemblyNameFromPersistableString(persistableString);

            // Assert
            Assert.Equal("MyAssembly", assemblyName.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyName.Version);
        }

        [Fact]
        public void GetAssemblyNameFromPersistableString_WithMajorMinorVersion_ShouldParseCorrectly()
        {
            // Arrange
            var persistableString = "TestAssembly/1.0";

            // Act
            var assemblyName = Baubit.Reflection.AssemblyExtensions.GetAssemblyNameFromPersistableString(persistableString);

            // Assert
            Assert.Equal("TestAssembly", assemblyName.Name);
            Assert.Equal(new Version(1, 0), assemblyName.Version);
        }

        [Fact]
        public void GetAssemblyNameFromPersistableString_WithThreePartVersion_ShouldParseCorrectly()
        {
            // Arrange
            var persistableString = "AnotherAssembly/2.1.5";

            // Act
            var assemblyName = Baubit.Reflection.AssemblyExtensions.GetAssemblyNameFromPersistableString(persistableString);

            // Assert
            Assert.Equal("AnotherAssembly", assemblyName.Name);
            Assert.Equal(new Version(2, 1, 5), assemblyName.Version);
        }

        #endregion

        #region TryResolveAssembly Tests

        [Fact]
        public void TryResolveAssembly_WithLoadedAssembly_ShouldReturnAssembly()
        {
            // Arrange
            var currentAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = currentAssembly.GetName();

            // Act
            var result = assemblyName.TryResolveAssembly();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(currentAssembly, result);
        }

        [Fact]
        public void TryResolveAssembly_WithSystemAssembly_ShouldReturnAssembly()
        {
            // Arrange
            var systemAssemblyName = typeof(string).Assembly.GetName();

            // Act
            var result = systemAssemblyName.TryResolveAssembly();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(systemAssemblyName.Name, result!.GetName().Name);
        }

        [Fact]
        public void TryResolveAssembly_WithNonLoadedAssembly_ShouldReturnNull()
        {
            // Arrange
            var nonExistentAssemblyName = new AssemblyName { Name = "NonExistent.Assembly", Version = new Version(1, 0, 0, 0) };

            // Act
            var result = nonExistentAssemblyName.TryResolveAssembly();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TryResolveAssembly_WithPartialVersionMatch_ShouldResolveCorrectly()
        {
            // Arrange
            var currentAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = new AssemblyName
            {
                Name = currentAssembly.GetName().Name,
                Version = new Version(
                    currentAssembly.GetName().Version!.Major,
                    currentAssembly.GetName().Version!.Minor,
                    currentAssembly.GetName().Version!.Build
                )
            };

            // Act
            var result = assemblyName.TryResolveAssembly();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(currentAssembly, result);
        }

        #endregion

        #region IsSameAs Tests

        [Fact]
        public void IsSameAs_WithIdenticalNames_ShouldReturnTrue()
        {
            // Arrange
            var assemblyName1 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 2, 3, 4) };
            var assemblyName2 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 2, 3, 4) };

            // Act
            var result = assemblyName1.IsSameAs(assemblyName2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsSameAs_WithDifferentNames_ShouldReturnFalse()
        {
            // Arrange
            var assemblyName1 = new AssemblyName { Name = "Assembly1", Version = new Version(1, 0, 0, 0) };
            var assemblyName2 = new AssemblyName { Name = "Assembly2", Version = new Version(1, 0, 0, 0) };

            // Act
            var result = assemblyName1.IsSameAs(assemblyName2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsSameAs_WithCaseInsensitiveNames_ShouldReturnTrue()
        {
            // Arrange
            var assemblyName1 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0, 0) };
            var assemblyName2 = new AssemblyName { Name = "testassembly", Version = new Version(1, 0, 0, 0) };

            // Act
            var result = assemblyName1.IsSameAs(assemblyName2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsSameAs_WithDifferentMajorVersion_ShouldReturnFalse()
        {
            // Arrange
            var assemblyName1 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0, 0) };
            var assemblyName2 = new AssemblyName { Name = "TestAssembly", Version = new Version(2, 0, 0, 0) };

            // Act
            var result = assemblyName1.IsSameAs(assemblyName2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsSameAs_WithDifferentMinorVersion_ShouldReturnFalse()
        {
            // Arrange
            var assemblyName1 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0, 0) };
            var assemblyName2 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 1, 0, 0) };

            // Act
            var result = assemblyName1.IsSameAs(assemblyName2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsSameAs_WithDifferentBuildVersion_ShouldReturnFalse()
        {
            // Arrange
            var assemblyName1 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0, 0) };
            var assemblyName2 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 1, 0) };

            // Act
            var result = assemblyName1.IsSameAs(assemblyName2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsSameAs_WithDifferentRevision_WhenBothDefined_ShouldReturnFalse()
        {
            // Arrange
            var assemblyName1 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0, 1) };
            var assemblyName2 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0, 2) };

            // Act
            var result = assemblyName1.IsSameAs(assemblyName2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsSameAs_WithOneRevisionUndefined_ShouldIgnoreRevision()
        {
            // Arrange
            var assemblyName1 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0) }; // Revision is -1
            var assemblyName2 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0, 5) };

            // Act
            var result = assemblyName1.IsSameAs(assemblyName2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsSameAs_WithBothRevisionsUndefined_ShouldReturnTrue()
        {
            // Arrange
            var assemblyName1 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0) };
            var assemblyName2 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0) };

            // Act
            var result = assemblyName1.IsSameAs(assemblyName2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsSameAs_WithNullVersion_ShouldOnlyCheckName()
        {
            // Arrange
            var assemblyName1 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0, 0) };
            var assemblyName2 = new AssemblyName { Name = "TestAssembly", Version = null };

            // Act
            var result = assemblyName1.IsSameAs(assemblyName2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsSameAs_WithNullVersionAndDifferentNames_ShouldReturnFalse()
        {
            // Arrange
            var assemblyName1 = new AssemblyName { Name = "TestAssembly1", Version = new Version(1, 0, 0, 0) };
            var assemblyName2 = new AssemblyName { Name = "TestAssembly2", Version = null };

            // Act
            var result = assemblyName1.IsSameAs(assemblyName2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsSameAs_WithSameRevision_WhenBothDefined_ShouldReturnTrue()
        {
            // Arrange
            var assemblyName1 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0, 5) };
            var assemblyName2 = new AssemblyName { Name = "TestAssembly", Version = new Version(1, 0, 0, 5) };

            // Act
            var result = assemblyName1.IsSameAs(assemblyName2);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region ReadResource Tests

        [Fact]
        public async Task ReadResource_WithNonExistingResource_ShouldFail()
        {
            // Arrange
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "NonExistent.Resource.txt";

            // Act
            var result = await assembly.ReadResource(resourceName);

            // Assert
            Assert.True(result.IsFailed);
        }

        #endregion

        #region GetBaubitFormattedAssemblyQualifiedName Tests

        [Fact]
        public void GetBaubitFormattedAssemblyQualifiedName_WithSimpleType_ShouldRemoveVersionCulturePublicKeyToken()
        {
            // Arrange
            var type = typeof(string);

            // Act
            var result = type.GetBaubitFormattedAssemblyQualifiedName();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.DoesNotContain("Version=", result.Value);
            Assert.DoesNotContain("Culture=", result.Value);
            Assert.DoesNotContain("PublicKeyToken=", result.Value);
            Assert.Contains("System.String", result.Value);
        }

        [Fact]
        public void GetBaubitFormattedAssemblyQualifiedName_WithGenericType_ShouldRemoveVersionInfo()
        {
            // Arrange
            var type = typeof(List<int>);

            // Act
            var result = type.GetBaubitFormattedAssemblyQualifiedName();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.DoesNotContain("Version=", result.Value);
            Assert.DoesNotContain("Culture=", result.Value);
            Assert.DoesNotContain("PublicKeyToken=", result.Value);
        }

        [Fact]
        public void GetBaubitFormattedAssemblyQualifiedName_WithComplexGenericType_ShouldRemoveAllVersionInfo()
        {
            // Arrange
            var type = typeof(Dictionary<string, List<int>>);

            // Act
            var result = type.GetBaubitFormattedAssemblyQualifiedName();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.DoesNotContain("Version=", result.Value);
            Assert.DoesNotContain("Culture=", result.Value);
            Assert.DoesNotContain("PublicKeyToken=", result.Value);
        }

        [Fact]
        public void GetBaubitFormattedAssemblyQualifiedName_WithCustomType_ShouldRemoveVersionInfo()
        {
            // Arrange
            var type = typeof(Test);

            // Act
            var result = type.GetBaubitFormattedAssemblyQualifiedName();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.DoesNotContain("Version=", result.Value);
            Assert.DoesNotContain("Culture=", result.Value);
            Assert.DoesNotContain("PublicKeyToken=", result.Value);
            Assert.Contains("Baubit.Reflection.Test.AssemblyExtensions.Test", result.Value);
        }

        [Fact]
        public void GetBaubitFormattedAssemblyQualifiedName_WithArrayType_ShouldHandleCorrectly()
        {
            // Arrange
            var type = typeof(int[]);

            // Act
            var result = type.GetBaubitFormattedAssemblyQualifiedName();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.DoesNotContain("Version=", result.Value);
            Assert.DoesNotContain("Culture=", result.Value);
            Assert.DoesNotContain("PublicKeyToken=", result.Value);
        }

        [Fact]
        public void GetBaubitFormattedAssemblyQualifiedName_WithNestedType_ShouldHandleCorrectly()
        {
            // Arrange
            var type = typeof(OuterClass.InnerClass);

            // Act
            var result = type.GetBaubitFormattedAssemblyQualifiedName();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.DoesNotContain("Version=", result.Value);
            Assert.Contains("OuterClass+InnerClass", result.Value);
        }

        [Fact]
        public void GetBaubitFormattedAssemblyQualifiedName_WithNullableType_ShouldHandleCorrectly()
        {
            // Arrange
            var type = typeof(int?);

            // Act
            var result = type.GetBaubitFormattedAssemblyQualifiedName();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.DoesNotContain("Version=", result.Value);
        }

        #endregion

        #region ReadStringAsync Tests

        [Fact]
        public async Task ReadStringAsync_WithValidStream_ShouldReturnContent()
        {
            // Arrange
            var content = "Test content from stream";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            var result = await stream.ReadStringAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(content, result.Value);
        }

        [Fact]
        public async Task ReadStringAsync_WithEmptyStream_ShouldReturnEmptyString()
        {
            // Arrange
            var stream = new MemoryStream();

            // Act
            var result = await stream.ReadStringAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task ReadStringAsync_WithNullStream_ShouldFail()
        {
            // Arrange
            Stream? stream = null;

            // Act
            var result = await stream!.ReadStringAsync();

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains("null stream", result.Errors.First().Message);
        }

        [Fact]
        public async Task ReadStringAsync_WithMultilineContent_ShouldReturnFullContent()
        {
            // Arrange
            var content = "Line 1\nLine 2\nLine 3";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            var result = await stream.ReadStringAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(content, result.Value);
        }

        [Fact]
        public async Task ReadStringAsync_WithUnicodeContent_ShouldReturnCorrectContent()
        {
            // Arrange
            var content = "Hello 世界 🌍";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            var result = await stream.ReadStringAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(content, result.Value);
        }

        [Fact]
        public async Task ReadStringAsync_WithLargeContent_ShouldReadSuccessfully()
        {
            // Arrange
            var content = new string('A', 10000);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            var result = await stream.ReadStringAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(10000, result.Value.Length);
            Assert.Equal(content, result.Value);
        }

        [Fact]
        public async Task ReadStringAsync_WithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var content = "Special: \t\r\n\"'\\/@#$%";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            var result = await stream.ReadStringAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(content, result.Value);
        }

        [Fact]
        public async Task ReadStringAsync_WithDisposedStream_ShouldFail()
        {
            // Arrange
            var content = "Test content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            stream.Dispose();

            // Act
            var result = await stream.ReadStringAsync();

            // Assert
            Assert.True(result.IsFailed);
        }

        #endregion

        // Helper classes for testing
        public class OuterClass
        {
            public class InnerClass
            {
            }
        }
    }
}
