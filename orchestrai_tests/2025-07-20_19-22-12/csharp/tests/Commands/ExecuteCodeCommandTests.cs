using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteCodeCommandTests
    {
        private readonly Mock<ILogger<ExecuteCodeCommand>> _loggerMock;
        private readonly ExecuteCodeCommand _command;

        public ExecuteCodeCommandTests()
        {
            _loggerMock = new Mock<ILogger<ExecuteCodeCommand>>();
            _command = new ExecuteCodeCommand(_loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidCode_ReturnsSuccess()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                Configuration = "Release"
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidCode_ReturnsError()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "invalid C# code syntax error",
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
        public async Task ExecuteAsync_WithEmptyCode_ReturnsSuccess()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = string.Empty,
                Configuration = "Release"
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }
    }
}