using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteScriptCommandTests
    {
        private readonly Mock<ILogger<ExecuteScriptCommand>> _loggerMock;
        private readonly ExecuteScriptCommand _command;

        public ExecuteScriptCommandTests()
        {
            _loggerMock = new Mock<ILogger<ExecuteScriptCommand>>();
            _command = new ExecuteScriptCommand(_loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidScriptFile_ReturnsSuccess()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "Console.WriteLine(\"Hello World\");");
            
            var options = new ExecuteScriptCommandOptions
            {
                File = tempFile,
                Configuration = "Release"
            };

            try
            {
                // Act
                var result = await _command.ExecuteAsync(options);

                // Assert
                Assert.Equal(0, result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task ExecuteAsync_WithNonExistentFile_ReturnsError()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions
            {
                File = "nonexistent.csx",
                Configuration = "Release"
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithArguments_PassesArgumentsToScript()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "Console.WriteLine(Args[0]);");
            
            var options = new ExecuteScriptCommandOptions
            {
                File = tempFile,
                Configuration = "Release",
                Arguments = new[] { "test-arg" }
            };

            try
            {
                // Act
                var result = await _command.ExecuteAsync(options);

                // Assert
                Assert.Equal(0, result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}