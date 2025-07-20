```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace DotNetScript.Tests
{
    public class ScriptRunnerTests
    {
        private readonly Mock<ILogger<ScriptRunner>> _mockLogger;
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly ScriptRunner _scriptRunner;

        public ScriptRunnerTests()
        {
            _mockLogger = new Mock<ILogger<ScriptRunner>>();
            _mockFileSystem = new Mock<IFileSystem>();
            _scriptRunner = new ScriptRunner(_mockLogger.Object, _mockFileSystem.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidScript_ShouldReturnSuccess()
        {
            // Arrange
            var scriptPath = "test.csx";
            var scriptContent = "Console.WriteLine(\"Hello World\");";
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ReturnsAsync(scriptContent);

            // Act
            var result = await _scriptRunner.ExecuteAsync(scriptPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Output.Should().Contain("Hello World");
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidScript_ShouldReturnFailure()
        {
            // Arrange
            var scriptPath = "invalid.csx";
            var scriptContent = "invalid syntax here";
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ReturnsAsync(scriptContent);

            // Act
            var result = await _scriptRunner.ExecuteAsync(scriptPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Error.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_WithNonExistentFile_ShouldThrowFileNotFoundException()
        {
            // Arrange
            var scriptPath = "nonexistent.csx";
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ThrowsAsync(new FileNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _scriptRunner.ExecuteAsync(scriptPath));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task ExecuteAsync_WithInvalidPath_ShouldThrowArgumentException(string scriptPath)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _scriptRunner.ExecuteAsync(scriptPath));
        }

        [Fact]
        public async Task ExecuteAsync_WithScriptArguments_ShouldPassArgumentsCorrectly()
        {
            // Arrange
            var scriptPath = "test.csx";
            var scriptContent = "Console.WriteLine(Args[0]);";
            var arguments = new[] { "test-arg" };
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ReturnsAsync(scriptContent);

            // Act
            var result = await _scriptRunner.ExecuteAsync(scriptPath, arguments);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Output.Should().Contain("test-arg");
        }
    }
}
```