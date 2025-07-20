using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptCompilerTests : IDisposable
    {
        private readonly Mock<ILogger<ScriptCompiler>> _mockLogger;
        private readonly ScriptCompiler _compiler;
        private readonly string _tempDirectory;

        public ScriptCompilerTests()
        {
            _mockLogger = new Mock<ILogger<ScriptCompiler>>();
            _compiler = new ScriptCompiler(_mockLogger.Object);
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [Fact]
        public void Constructor_WithValidLogger_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Assert.NotNull(_compiler);
        }

        [Theory]
        [InlineData("Console.WriteLine(\"Hello World\");")]
        [InlineData("var x = 5; Console.WriteLine(x);")]
        [InlineData("return 42;")]
        public async Task CompileAsync_WithValidCode_ShouldCompileSuccessfully(string code)
        {
            // Act
            var result = await _compiler.CompileAsync(code);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Theory]
        [InlineData("invalid syntax")]
        [InlineData("Console.WriteLine(")]
        [InlineData("var x = ;")]
        public async Task CompileAsync_WithInvalidCode_ShouldReturnCompilationErrors(string code)
        {
            // Act
            var result = await _compiler.CompileAsync(code);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task CompileAsync_WithNullCode_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _compiler.CompileAsync(null));
        }

        [Fact]
        public async Task CompileAsync_WithEmptyCode_ShouldReturnEmptyCompilation()
        {
            // Act
            var result = await _compiler.CompileAsync(string.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task CompileScriptFileAsync_WithValidFile_ShouldCompileSuccessfully()
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "test.csx");
            await File.WriteAllTextAsync(scriptPath, "Console.WriteLine(\"Hello from file\");");

            // Act
            var result = await _compiler.CompileScriptFileAsync(scriptPath);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task CompileScriptFileAsync_WithNonExistentFile_ShouldThrowFileNotFoundException()
        {
            // Arrange
            var scriptPath = "nonexistent.csx";

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _compiler.CompileScriptFileAsync(scriptPath));
        }

        [Fact]
        public async Task CompileWithReferencesAsync_WithValidReferences_ShouldCompileSuccessfully()
        {
            // Arrange
            var code = "using System.Text.Json; var json = JsonSerializer.Serialize(new { test = \"value\" });";
            var references = new[] { "System.Text.Json" };

            // Act
            var result = await _compiler.CompileWithReferencesAsync(code, references);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task CompileWithReferencesAsync_WithInvalidReferences_ShouldReturnCompilationErrors()
        {
            // Arrange
            var code = "using NonExistent.Library; var obj = new SomeClass();";
            var references = new[] { "NonExistent.Library" };

            // Act
            var result = await _compiler.CompileWithReferencesAsync(code, references);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
    }
}