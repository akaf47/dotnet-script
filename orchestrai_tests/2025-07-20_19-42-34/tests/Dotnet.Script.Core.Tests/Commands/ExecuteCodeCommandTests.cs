using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteCodeCommandTests
    {
        private readonly Mock<ILogger<ExecuteCodeCommand>> _mockLogger;
        private readonly ExecuteCodeCommand _command;

        public ExecuteCodeCommandTests()
        {
            _mockLogger = new Mock<ILogger<ExecuteCodeCommand>>();
            _command = new ExecuteCodeCommand(_mockLogger.Object);
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
            Assert.Throws<ArgumentNullException>(() => new ExecuteCodeCommand(null));
        }

        [Theory]
        [InlineData("Console.WriteLine(\"Hello World\");")]
        [InlineData("var x = 5; Console.WriteLine(x);")]
        [InlineData("System.DateTime.Now.ToString();")]
        public async Task ExecuteAsync_WithValidCode_ShouldExecuteSuccessfully(string code)
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions { Code = code };

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
        public async Task ExecuteAsync_WithEmptyCode_ShouldReturnError()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions { Code = "" };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidCode_ShouldReturnError()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions { Code = "invalid syntax here" };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }
    }
}