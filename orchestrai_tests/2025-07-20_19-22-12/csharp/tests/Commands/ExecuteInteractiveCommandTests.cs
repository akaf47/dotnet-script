using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteInteractiveCommandTests
    {
        private readonly Mock<ILogger<ExecuteInteractiveCommand>> _loggerMock;
        private readonly ExecuteInteractiveCommand _command;

        public ExecuteInteractiveCommandTests()
        {
            _loggerMock = new Mock<ILogger<ExecuteInteractiveCommand>>();
            _command = new ExecuteInteractiveCommand(_loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidOptions_ReturnsSuccess()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions
            {
                Configuration = "Release"
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithDebugMode_ReturnsSuccess()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions
            {
                Configuration = "Debug",
                Debug = true
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }
    }
}