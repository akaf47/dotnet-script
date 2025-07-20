using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class InitCommandTests : IDisposable
    {
        private readonly Mock<ILogger<InitCommand>> _mockLogger;
        private readonly InitCommand _command;
        private readonly string _tempDirectory;

        public InitCommandTests()
        {
            _mockLogger = new Mock<ILogger<InitCommand>>();
            _command = new InitCommand(_mockLogger.Object);
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
            Assert.Throws<ArgumentNullException>(() => new InitCommand(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidPath_ShouldCreateScriptFile()
        {
            // Arrange
            var scriptName = "test.csx";
            var scriptPath = Path.Combine(_tempDirectory, scriptName);
            var options = new InitCommandOptions { ScriptName = scriptName, OutputPath = _tempDirectory };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
            Assert.True(File.Exists(scriptPath));
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithExistingFile_ShouldNotOverwrite()
        {
            // Arrange
            var scriptName = "existing.csx";
            var scriptPath = Path.Combine(_tempDirectory, scriptName);
            await File.WriteAllTextAsync(scriptPath, "// Existing content");
            var options = new InitCommandOptions { ScriptName = scriptName, OutputPath = _tempDirectory };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
            var content = await File.ReadAllTextAsync(scriptPath);
            Assert.Contains("// Existing content", content);
        }

        [Theory]
        [InlineData("console")]
        [InlineData("web")]
        [InlineData("library")]
        public async Task ExecuteAsync_WithTemplate_ShouldCreateTemplateBasedScript(string template)
        {
            // Arrange
            var scriptName = $"{template}.csx";
            var scriptPath = Path.Combine(_tempDirectory, scriptName);
            var options = new InitCommandOptions 
            { 
                ScriptName = scriptName, 
                OutputPath = _tempDirectory,
                Template = template
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
            Assert.True(File.Exists(scriptPath));
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