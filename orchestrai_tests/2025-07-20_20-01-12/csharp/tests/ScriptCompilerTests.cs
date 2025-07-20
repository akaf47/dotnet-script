```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using DotNetScript.Core;
using DotNetScript.Models;

namespace DotNetScript.Tests
{
    public class ScriptCompilerTests
    {
        private readonly Mock<IAssemblyResolver> _mockAssemblyResolver;
        private readonly ScriptCompiler _compiler;

        public ScriptCompilerTests()
        {
            _mockAssemblyResolver = new Mock<IAssemblyResolver>();
            _compiler = new ScriptCompiler(_mockAssemblyResolver.Object);
        }

        [Fact]
        public async Task CompileAndRunAsync_SimpleScript_ReturnsSuccess()
        {
            // Arrange
            var script = "return 42;";
            _mockAssemblyResolver.Setup(x => x.GetReferences())
                                .Returns(new List<MetadataReference>());

            // Act
            var result = await _compiler.CompileAndRunAsync(script);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("42", result.Output);
        }

        [Fact]
        public async Task CompileAndRunAsync_ScriptWithSyntaxError_ReturnsFailure()
        {
            // Arrange
            var script = "invalid syntax {{{";

            // Act
            var result = await _compiler.CompileAndRunAsync(script);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Contains("syntax", result.Error.ToLower());
        }

        [Fact]
        public async Task CompileAndRunAsync_ScriptWithRuntimeError_ReturnsFailure()
        {
            // Arrange
            var script = "throw new Exception(\"Runtime error\");";

            // Act
            var result = await _compiler.CompileAndRunAsync(script);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Runtime error", result.Error);
        }

        [Fact]
        public async Task CompileAndRunAsync_ScriptWithConsoleOutput_CapturesOutput()
        {
            // Arrange
            var script = "Console.WriteLine(\"Hello World\");";

            // Act
            var result = await _compiler.CompileAndRunAsync(script);

            // Assert
            Assert.True(result.Success);
            Assert.Contains("Hello World", result.Output);
        }

        [Fact]
        public async Task CompileAndRunAsync_ScriptWithImports_ReturnsSuccess()
        {
            // Arrange
            var script = @"
                using System.Linq;
                var numbers = new[] { 1, 2, 3, 4, 5 };
                return numbers.Sum();
            ";

            // Act
            var result = await _compiler.CompileAndRunAsync(script);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("15", result.Output);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task CompileAndRunAsync_EmptyOrNullScript_ThrowsArgumentException(string script)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _compiler.CompileAndRunAsync(script));
        }

        [Fact]
        public async Task CompileAndRunAsync_ScriptWithArgs_PassesArgsCorrectly()
        {
            // Arrange
            var script = "return Args[0] + Args[1];";
            var args = new[] { "Hello", " World" };

            // Act
            var result = await _compiler.CompileAndRunAsync(script, args);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Hello World", result.Output);
        }

        [Fact]
        public void ValidateScript_ValidScript_ReturnsTrue()
        {
            // Arrange
            var script = "Console.WriteLine(\"Valid\");";

            // Act
            var isValid = _compiler.ValidateScript(script);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateScript_InvalidScript_ReturnsFalse()
        {
            // Arrange
            var script = "invalid syntax {{{";

            // Act
            var isValid = _compiler.ValidateScript(script);

            // Assert
            Assert.False(isValid);
        }
    }
}
```