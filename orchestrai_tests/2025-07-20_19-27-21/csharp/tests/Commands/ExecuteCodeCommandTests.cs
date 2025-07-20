```csharp
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteCodeCommandTests
    {
        private readonly Mock<ILogger<ExecuteCodeCommand>> _mockLogger;
        private readonly Mock<IScriptExecutor> _mockScriptExecutor;
        private readonly ExecuteCodeCommand _command;

        public ExecuteCodeCommandTests()
        {
            _mockLogger = new Mock<ILogger<ExecuteCodeCommand>>();
            _mockScriptExecutor = new Mock<IScriptExecutor>();
            _command = new ExecuteCodeCommand(_mockScriptExecutor.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidCode_ExecutesSuccessfully()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            _mockScriptExecutor
                .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<string>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
            _mockScriptExecutor.Verify(x => x.ExecuteAsync(
                options.Code, 
                It.IsAny<string[]>(), 
                options.WorkingDirectory), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithEmptyCode_ThrowsArgumentException()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = string.Empty,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _command.ExecuteAsync(options));
        }

        [Fact]
        public async Task ExecuteAsync_WithScriptExecutionFailure_ReturnsErrorCode()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "throw new Exception(\"Test error\");",
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            _mockScriptExecutor
                .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithReferences_PassesReferencesToExecutor()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                WorkingDirectory = Directory.GetCurrentDirectory(),
                References = new[] { "System.dll", "System.Core.dll" }
            };

            _mockScriptExecutor
                .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<string>()))
                .ReturnsAsync(0);

            // Act
            await _command.ExecuteAsync(options);

            // Assert
            _mockScriptExecutor.Verify(x => x.ExecuteAsync(
                options.Code,
                options.References,
                options.WorkingDirectory), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_LogsExecutionStart()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            _mockScriptExecutor
                .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<string>()))
                .ReturnsAsync(0);

            // Act
            await _command.ExecuteAsync(options);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Executing code")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
```