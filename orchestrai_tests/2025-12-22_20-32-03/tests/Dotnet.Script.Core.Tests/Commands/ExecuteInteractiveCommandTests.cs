using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteInteractiveCommandTests
    {
        private readonly Mock<IScriptExecutor> _mockExecutor;
        private readonly Mock<IConsoleService> _mockConsole;
        private ExecuteInteractiveCommand _command;

        public ExecuteInteractiveCommandTests()
        {
            _mockExecutor = new Mock<IScriptExecutor>();
            _mockConsole = new Mock<IConsoleService>();
            _command = new ExecuteInteractiveCommand(_mockExecutor.Object, _mockConsole.Object);
        }

        #region Constructor Tests
        
        [Fact]
        public void Constructor_WithValidParameters_InitializesSuccessfully()
        {
            // Arrange & Act
            var command = new ExecuteInteractiveCommand(_mockExecutor.Object, _mockConsole.Object);

            // Assert
            Assert.NotNull(command);
        }

        [Fact]
        public void Constructor_WithNullExecutor_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new ExecuteInteractiveCommand(null, _mockConsole.Object));
        }

        [Fact]
        public void Constructor_WithNullConsole_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new ExecuteInteractiveCommand(_mockExecutor.Object, null));
        }

        [Fact]
        public void Constructor_WithBothParametersNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new ExecuteInteractiveCommand(null, null));
        }

        #endregion

        #region Execute Method Tests

        [Fact]
        public async Task ExecuteAsync_WithValidOptions_ExecutesSuccessfully()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>()
            };
            _mockExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
            _mockExecutor.Verify(x => x.ExecuteAsync("test.csx", It.IsAny<List<string>>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithEmptyScriptPath_ThrowsArgumentException()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = string.Empty,
                Arguments = new List<string>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _command.ExecuteAsync(options));
        }

        [Fact]
        public async Task ExecuteAsync_WithNullScriptPath_ThrowsArgumentNullException()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = null,
                Arguments = new List<string>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(options));
        }

        [Fact]
        public async Task ExecuteAsync_WithNullArguments_ExecutesWithEmptyArguments()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = null
            };
            _mockExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithEmptyArgumentsList_ExecutesSuccessfully()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>()
            };
            _mockExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
            _mockExecutor.Verify(x => x.ExecuteAsync("test.csx", It.IsAny<List<string>>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WithMultipleArguments_PassesAllArgumentsToExecutor()
        {
            // Arrange
            var arguments = new List<string> { "arg1", "arg2", "arg3" };
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = arguments
            };
            _mockExecutor.Setup(x => x.ExecuteAsync("test.csx", arguments))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
            _mockExecutor.Verify(x => x.ExecuteAsync("test.csx", arguments), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenExecutorReturnsNonZeroExitCode_ReturnsExitCode()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>()
            };
            _mockExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task ExecuteAsync_WhenExecutorThrowsException_PropagatesException()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>()
            };
            _mockExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ThrowsAsync(new InvalidOperationException("Execution failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _command.ExecuteAsync(options));
        }

        [Fact]
        public async Task ExecuteAsync_WhenExecutorThrowsTimeoutException_PropagatesTimeoutException()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>()
            };
            _mockExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ThrowsAsync(new TimeoutException("Execution timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TimeoutException>(() => _command.ExecuteAsync(options));
        }

        [Fact]
        public async Task ExecuteAsync_WithWhitespaceOnlyScriptPath_ThrowsArgumentException()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "   ",
                Arguments = new List<string>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _command.ExecuteAsync(options));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidPathAndEnableDebug_PassesDebugFlagCorrectly()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>(),
                EnableDebug = true
            };
            _mockExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidPathAndDisableDebug_ExecutesSuccessfully()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>(),
                EnableDebug = false
            };
            _mockExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region ExecuteInteractive Method Tests

        [Fact]
        public async Task ExecuteInteractiveAsync_WithValidOptions_EntersInteractiveMode()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>()
            };
            _mockConsole.Setup(x => x.ReadLineAsync())
                .ReturnsAsync("exit");
            _mockExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteInteractiveAsync(options);

            // Assert
            Assert.Equal(0, result);
            _mockConsole.Verify(x => x.ReadLineAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteInteractiveAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteInteractiveAsync(null));
        }

        [Fact]
        public async Task ExecuteInteractiveAsync_WhenUserEntersExit_TerminatesInteractiveMode()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>()
            };
            _mockConsole.Setup(x => x.ReadLineAsync())
                .ReturnsAsync("exit");
            _mockExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteInteractiveAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteInteractiveAsync_WhenUserEntersQuit_TerminatesInteractiveMode()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>()
            };
            _mockConsole.Setup(x => x.ReadLineAsync())
                .ReturnsAsync("quit");

            // Act
            var result = await _command.ExecuteInteractiveAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteInteractiveAsync_WhenUserEntersValidCommand_ExecutesCommand()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>()
            };
            var callCount = 0;
            _mockConsole.Setup(x => x.ReadLineAsync())
                .Returns(() =>
                {
                    callCount++;
                    return callCount == 1 ? Task.FromResult("some-code") : Task.FromResult("exit");
                });
            _mockExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteInteractiveAsync(options);

            // Assert
            Assert.Equal(0, result);
            _mockExecutor.Verify(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteInteractiveAsync_WhenExecutionFails_ReturnsNonZeroExitCode()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>()
            };
            var callCount = 0;
            _mockConsole.Setup(x => x.ReadLineAsync())
                .Returns(() =>
                {
                    callCount++;
                    return callCount == 1 ? Task.FromResult("some-code") : Task.FromResult("exit");
                });
            _mockExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            // Act
            var result = await _command.ExecuteInteractiveAsync(options);

            // Assert
            Assert.Equal(1, result);
        }

        #endregion

        #region ValidateOptions Method Tests

        [Fact]
        public void ValidateOptions_WithValidOptions_ReturnsTrue()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = "test.csx",
                Arguments = new List<string>()
            };

            // Act
            var result = _command.ValidateOptions(options);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateOptions_WithNullOptions_ReturnsFalse()
        {
            // Act
            var result = _command.ValidateOptions(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateOptions_WithEmptyScriptPath_ReturnsFalse()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = string.Empty,
                Arguments = new List<string>()
            };

            // Act
            var result = _command.ValidateOptions(options);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateOptions_WithNullScriptPath_ReturnsFalse()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                ScriptPath = null,
                Arguments = new List<string>()
            };

            // Act
            var result = _command.ValidateOptions(options);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region DisplayPrompt Method Tests

        [Fact]
        public void DisplayPrompt_WritesToConsole()
        {
            // Arrange
            var prompt = "script>";

            // Act
            _command.DisplayPrompt(prompt);

            // Assert
            _mockConsole.Verify(x => x.Write(prompt), Times.Once);
        }

        [Fact]
        public void DisplayPrompt_WithNullPrompt_WritesNullToConsole()
        {
            // Act
            _command.DisplayPrompt(null);

            // Assert
            _mockConsole.Verify(x => x.Write(null), Times.Once);
        }

        [Fact]
        public void DisplayPrompt_WithEmptyPrompt_WritesEmptyStringToConsole()
        {
            // Act
            _command.DisplayPrompt(string.Empty);

            // Assert
            _mockConsole.Verify(x => x.Write(string.Empty), Times.Once);
        }

        #endregion

        #region DisplayWelcome Method Tests

        [Fact]
        public void DisplayWelcome_WritesToConsole()
        {
            // Act
            _command.DisplayWelcome();

            // Assert
            _mockConsole.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        #endregion
    }
}