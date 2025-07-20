```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using FluentAssertions;

namespace DotNetScript.Tests
{
    public class ScriptCompilerTests
    {
        private readonly Mock<IMetadataResolver> _mockMetadataResolver;
        private readonly ScriptCompiler _compiler;

        public ScriptCompilerTests()
        {
            _mockMetadataResolver = new Mock<IMetadataResolver>();
            _compiler = new ScriptCompiler(_mockMetadataResolver.Object);
        }

        [Fact]
        public void Compile_WithValidCode_ShouldReturnSuccessfulCompilation()
        {
            // Arrange
            var code = "using System; Console.WriteLine(\"Hello World\");";
            var references = new List<MetadataReference>();
            _mockMetadataResolver.Setup(x => x.GetReferences())
                                .Returns(references);

            // Act
            var result = _compiler.Compile(code);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Assembly.Should().NotBeNull();
            result.Diagnostics.Should().BeEmpty();
        }

        [Fact]
        public void Compile_WithSyntaxError_ShouldReturnFailedCompilation()
        {
            // Arrange
            var code = "using System; Console.WriteLine(\"Hello World\""; // Missing closing parenthesis
            var references = new List<MetadataReference>();
            _mockMetadataResolver.Setup(x => x.GetReferences())
                                .Returns(references);

            // Act
            var result = _compiler.Compile(code);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Assembly.Should().BeNull();
            result.Diagnostics.Should().NotBeEmpty();
            result.Diagnostics.Should().Contain(d => d.Severity == DiagnosticSeverity.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Compile_WithEmptyOrNullCode_ShouldThrowArgumentException(string code)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _compiler.Compile(code));
        }

        [Fact]
        public void Compile_WithWarnings_ShouldReturnSuccessfulCompilationWithWarnings()
        {
            // Arrange
            var code = "using System; int unused = 5; Console.WriteLine(\"Hello World\");";
            var references = new List<MetadataReference>();
            _mockMetadataResolver.Setup(x => x.GetReferences())
                                .Returns(references);

            // Act
            var result = _compiler.Compile(code);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Assembly.Should().NotBeNull();
            result.Diagnostics.Should().Contain(d => d.Severity == DiagnosticSeverity.Warning);
        }

        [Fact]
        public void Compile_WithCustomReferences_ShouldIncludeReferences()
        {
            // Arrange
            var code = "using System; using System.Linq; var list = new[] {1,2,3}.ToList();";
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            };
            _mockMetadataResolver.Setup(x => x.GetReferences())
                                .Returns(references);

            // Act
            var result = _compiler.Compile(code);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Assembly.Should().NotBeNull();
        }
    }
}
```