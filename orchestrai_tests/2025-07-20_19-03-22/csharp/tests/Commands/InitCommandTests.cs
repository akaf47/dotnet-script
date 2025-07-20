using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class InitCommandTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly InitCommand _command;
        private readonly string _tempDirectory;

        public InitCommandTests()
        {
            _mockLogger = new Mock<ILogger>();
            _command = new InitCommand(_mockLogger.Object);
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [Fact]
        public async Task Execute_WithValidDirectory_ShouldCreateScriptFile()
        {
            // Arrange
            var options = new InitCommandOptions
            {
                Name = "test-script",
                Directory = _tempDirectory
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            Assert.True(File.Exists(Path.Combine(_tempDirectory, "test-script.csx")));
        }

        [Fact]
        public async Task Execute_WithoutName_ShouldCreateMainScript()
        {
            // Arrange
            var options = new InitCommandOptions
            {
                Directory = _tempDirectory
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            Assert.True(File.Exists(Path.Combine(_tempDirectory, "main.csx")));
        }

        [Fact]
        public async Task Execute_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }

        [Fact]
        public async Task Execute_WithExistingFile_ShouldNotOverwrite()
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "existing.csx");
            var originalContent = "// Original content";
            File.WriteAllText(scriptPath, originalContent);

            var options = new InitCommandOptions
            {
                Name = "existing",
                Directory = _tempDirectory
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            var content = File.ReadAllText(scriptPath);
            Assert.Equal(originalContent, content);
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