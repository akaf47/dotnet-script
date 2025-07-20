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
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IScriptConsole> _mockConsole;
        private readonly ExecuteInteractiveCommand _command;

        public ExecuteInteractiveCommandTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockConsole = new Mock<IScriptConsole>();
            _command = new ExecuteInteractiveCommand(_mockLogger.Object, _mockConsole.Object);
        }

        [Fact]
        public async Task Execute_WithValidOptions_ShouldReturnSuccess()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions
            {
                Configuration = "Release"
            };

            // Mock console input to exit immediately
            _mockConsole.Setup(c => c.ReadLine()).Returns("#exit");

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }

        [Fact]
        public async Task Execute_WithDebugConfiguration_ShouldUseDebugMode()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions
            {
                Configuration = "Debug"
            };

            _mockConsole.Setup(c => c.ReadLine()).Returns("#exit");

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecuteInteractiveCommand(null, _mockConsole.Object));
        }

        [Fact]
        public void Constructor_WithNullConsole_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecuteInteractiveCommand(_mockLogger.Object, null));
        }
    }
}