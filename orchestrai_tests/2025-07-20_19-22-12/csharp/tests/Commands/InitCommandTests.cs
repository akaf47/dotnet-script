using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class InitCommandTests
    {
        private readonly Mock<ILogger<InitCommand>> _loggerMock;
        private readonly InitCommand _command;

        public InitCommandTests()
        {
            _loggerMock = new Mock<ILogger<InitCommand>>();
            _command = new InitCommand(_loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidPath_CreatesScriptFile()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var scriptName = "test-script.csx";
            var fullPath = Path.Combine(tempDir, scriptName);
            
            var options = new InitCommandOptions
            {
                FileName = scriptName,
                WorkingDirectory = tempDir
            };

            try
            {
                // Act
                var result = await _command.ExecuteAsync(options);

                // Assert
                Assert.Equal(0, result);
                Assert.True(File.Exists(fullPath));
            }
            finally
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithExistingFile_ReturnsError()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var scriptName = "existing-script.csx";
            var fullPath = Path.Combine(tempDir, scriptName);
            
            File.WriteAllText(fullPath, "// Existing file");
            
            var options = new InitCommandOptions
            {
                FileName = scriptName,
                WorkingDirectory = tempDir
            };

            try
            {
                // Act
                var result = await _command.ExecuteAsync(options);

                // Assert
                Assert.NotEqual(0, result);
            }
            finally
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }
    }
}