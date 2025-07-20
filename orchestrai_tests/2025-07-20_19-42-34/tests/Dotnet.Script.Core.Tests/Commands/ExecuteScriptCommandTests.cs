using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteScriptCommandTests : IDisposable
    {
        private readonly Mock<ILogger<ExecuteScriptCommand>> _mockLogger;
        private readonly ExecuteScriptCommand _command;
        private readonly string _tempDirectory;

        public ExecuteScriptCommandTests()
        {
            _mockLogger = new Mock<ILogger<ExecuteScriptCommand>>();
            _command = new ExecuteScriptCommand(_mockLogger.Object);
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [Fact]
        public void Constructor_WithValidLogger_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Assert.NotNull(_command);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecuteScriptCommand(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidScriptFile_ShouldExecuteSuccessfully()
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "test.csx");
            await File.WriteAllTextAsync(scriptPath, "Console.WriteLine(\"Hello from script\");");
            var options = new ExecuteScriptCommandOptions { ScriptPath = scriptPath };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNonExistentFile_ShouldReturnError()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions { ScriptPath = "nonexistent.csx" };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidScript_ShouldReturnError()
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "invalid.csx");
            await File.WriteAllTextAsync(scriptPath, "invalid C# syntax here");
            var options = new ExecuteScriptCommandOptions { ScriptPath = scriptPath };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Theory]
        [InlineData("arg1")]
        [InlineData("arg1", "arg2")]
        [InlineData("arg1", "arg2", "arg3")]
        public async Task ExecuteAsync_WithArguments_ShouldPassArgumentsToScript(params string[] args)
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "args.csx");
            await File.WriteAllTextAsync(scriptPath, "Console.WriteLine($\"Args count: {Args.Count}\");");
            var options = new ExecuteScriptCommandOptions 
            { 
                ScriptPath = scriptPath,
                Arguments = args
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
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