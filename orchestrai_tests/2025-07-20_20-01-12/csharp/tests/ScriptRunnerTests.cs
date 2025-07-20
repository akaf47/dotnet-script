```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using DotNetScript.Core;
using DotNetScript.Models;

namespace DotNetScript.Tests
{
    public class ScriptRunnerTests
    {
        private readonly Mock<ILogger<ScriptRunner>> _mockLogger;
        private readonly Mock<IScriptCompiler> _mockCompiler;
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly ScriptRunner _scriptRunner;

        public ScriptRunnerTests()
        {
            _mockLogger = new Mock<ILogger<ScriptRunner>>();
            _mockCompiler = new Mock<IScriptCompiler>();
            _mockFileSystem = new Mock<IFileSystem>();
            _scriptRunner = new ScriptRunner(_mockLogger.Object, _mockCompiler.Object, _mockFileSystem.Object);
        }

        [Fact]
        public async Task RunScriptAsync_ValidScript_ReturnsSuccess()
        {
            // Arrange
            var scriptPath = "test.csx";
            var scriptContent = "Console.WriteLine(\"Hello World\");";
            var expectedResult = new ScriptResult { Success = true, Output = "Hello World" };

            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ReturnsAsync(scriptContent);
            _mockCompiler.Setup(x => x.CompileAndRunAsync(scriptContent))
                        .ReturnsAsync(expectedResult);

            // Act
            var result = await _scriptRunner.RunScriptAsync(scriptPath);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Hello World", result.Output);
            _mockFileSystem.Verify(x => x.ReadAllTextAsync(scriptPath), Times.Once);
            _mockCompiler.Verify(x => x.CompileAndRunAsync(scriptContent), Times.Once);
        }

        [Fact]
        public async Task RunScriptAsync_FileNotFound_ThrowsFileNotFoundException()
        {
            // Arrange
            var scriptPath = "nonexistent.csx";
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ThrowsAsync(new FileNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _scriptRunner.RunScriptAsync(scriptPath));
        }

        [Fact]
        public async Task RunScriptAsync_CompilationError_ReturnsFailure()
        {
            // Arrange
            var scriptPath = "invalid.csx";
            var scriptContent = "invalid C# code";
            var expectedResult = new ScriptResult { Success = false, Error = "Compilation failed" };

            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ReturnsAsync(scriptContent);
            _mockCompiler.Setup(x => x.CompileAndRunAsync(scriptContent))
                        .ReturnsAsync(expectedResult);

            // Act
            var result = await _scriptRunner.RunScriptAsync(scriptPath);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Compilation failed", result.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task RunScriptAsync_EmptyOrNullPath_ThrowsArgumentException(string scriptPath)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _scriptRunner.RunScriptAsync(scriptPath));
        }

        [Fact]
        public async Task RunScriptWithArgsAsync_ValidScriptWithArgs_ReturnsSuccess()
        {
            // Arrange
            var scriptPath = "test.csx";
            var args = new[] { "arg1", "arg2" };
            var scriptContent = "Console.WriteLine(Args[0]);";
            var expectedResult = new ScriptResult { Success = true, Output = "arg1" };

            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ReturnsAsync(scriptContent);
            _mockCompiler.Setup(x => x.CompileAndRunAsync(scriptContent, args))
                        .ReturnsAsync(expectedResult);

            // Act
            var result = await _scriptRunner.RunScriptWithArgsAsync(scriptPath, args);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("arg1", result.Output);
        }
    }
}
```