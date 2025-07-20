```csharp
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using Microsoft.CodeAnalysis;
using DotNetScript.Core;

namespace DotNetScript.Tests
{
    public class AssemblyResolverTests
    {
        private readonly AssemblyResolver _resolver;

        public AssemblyResolverTests()
        {
            _resolver = new AssemblyResolver();
        }

        [Fact]
        public void GetReferences_Default_ReturnsSystemReferences()
        {
            // Act
            var references = _resolver.GetReferences();

            // Assert
            Assert.NotEmpty(references);
            Assert.Contains(references, r => r.Display.Contains("System.Runtime"));
            Assert.Contains(references, r => r.Display.Contains("System.Console"));
        }

        [Fact]
        public void GetReferences_IncludesNetCoreApp_ReturnsNetCoreReferences()
        {
            // Act
            var references = _resolver.GetReferences();

            // Assert
            Assert.Contains(references, r => r.Display.Contains("netcoreapp") || r.Display.Contains("net6.0") || r.Display.Contains("net7.0"));
        }

        [Fact]
        public void AddReference_ValidAssembly_AddsToReferences()
        {
            // Arrange
            var assembly = typeof(System.Linq.Enumerable).Assembly;

            // Act
            _resolver.AddReference(assembly);
            var references = _resolver.GetReferences();

            // Assert
            Assert.Contains(references, r => r.Display.Contains("System.Linq"));
        }

        [Fact]
        public void AddReference_NullAssembly_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _resolver.AddReference(null));
        }

        [Fact]
        public void AddReferenceByName_ValidAssemblyName_AddsToReferences()
        {
            // Arrange
            var assemblyName = "System.Text.Json";

            // Act
            _resolver.AddReferenceByName(assemblyName);
            var references = _resolver.GetReferences();

            // Assert
            Assert.Contains(references, r => r.Display.Contains("System.Text.Json"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddReferenceByName_EmptyOrNullName_ThrowsArgumentException(string assemblyName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _resolver.AddReferenceByName(assemblyName));
        }

        [Fact]
        public void AddReferenceByName_NonExistentAssembly_ThrowsFileNotFoundException()
        {
            // Arrange
            var assemblyName = "NonExistent.Assembly";

            // Act & Assert
            Assert.Throws<System.IO.FileNotFoundException>(() => _resolver.AddReferenceByName(assemblyName));
        }

        [Fact]
        public void GetUsings_Default_ReturnsCommonUsings()
        {
            // Act
            var usings = _resolver.GetUsings();

            // Assert
            Assert.Contains("System", usings);
            Assert.Contains("System.Collections.Generic", usings);
            Assert.Contains("System.Linq", usings);
            Assert.Contains("System.Threading.Tasks", usings);
        }

        [Fact]
        public void AddUsing_ValidNamespace_AddsToUsings()
        {
            // Arrange
            var namespaceName = "System.Text.Json";

            // Act
            _resolver.AddUsing(namespaceName);
            var usings = _resolver.GetUsings();

            // Assert
            Assert.Contains(namespaceName, usings);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddUsing_EmptyOrNullNamespace_ThrowsArgumentException(string namespaceName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _resolver.AddUsing(namespaceName));
        }

        [Fact]
        public void Reset_ClearsCustomReferencesAndUsings()
        {
            // Arrange
            _resolver.AddReferenceByName("System.Text.Json");
            _resolver.AddUsing("System.Text.Json");

            // Act
            _resolver.Reset();
            var references = _resolver.GetReferences();
            var usings = _resolver.GetUsings();

            // Assert
            Assert.DoesNotContain(references, r => r.Display.Contains("System.Text.Json"));
            Assert.DoesNotContain("System.Text.Json", usings);
        }
    }
}
```