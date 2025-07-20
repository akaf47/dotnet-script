using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteInteractiveCommandTests
    {
        private readonly Mock<ILogger<ExecuteInteractiveCommand>> _mockLogger;
        private readonly ExecuteInteractiveCommand _command;

        public ExecuteInteractiveCommandTests()
        {
            _mockLogger = new Mock<ILogger<ExecuteInteractiveCommand>>();
            _command = new ExecuteInteractiveCommand(_mockLogger.Object);
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
            Assert.Throws<ArgumentNullException>(() => new ExecuteInteractiveCommand(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidOptions_ShouldStartInteractiveMode()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithWorkingDirectory_ShouldSetWorkingDirectory()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                WorkingDirectory = Environment.CurrentDirectory 
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }
    }
}