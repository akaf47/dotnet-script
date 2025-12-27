using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteLibraryCommandTests
    {
        private readonly ExecuteLibraryCommand _command;
        private readonly Mock<ILibraryExecutor> _mockLibraryExecutor;
        private readonly Mock<ILogger> _mockLogger;

        public ExecuteLibraryCommandTests()
        {
            _mockLibraryExecutor = new Mock<ILibraryExecutor>();
            _mockLogger = new Mock<ILogger>();
            _command = new ExecuteLibraryCommand(_mockLibraryExecutor.Object, _mockLogger.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidDependencies_ShouldInitializeSuccessfully()
        {
            // Arrange & Act
            var command = new ExecuteLibraryCommand(_mockLibraryExecutor.Object, _mockLogger.Object);

            // Assert
            Assert.NotNull(command);
        }

        [Fact]
        public void Constructor_WithNullLibraryExecutor_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ExecuteLibraryCommand(null, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ExecuteLibraryCommand(_mockLibraryExecutor.Object, null));
        }

        [Fact]
        public void Constructor_WithBothNullDependencies_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ExecuteLibraryCommand(null, null));
        }

        #endregion

        #region Execute Method Tests

        [Fact]
        public async Task Execute_WithValidOptions_ShouldCallLibraryExecutor()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string> { "arg1", "arg2" }
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            _mockLibraryExecutor.Verify(x => x.ExecuteAsync("path/to/library.dll", "Namespace.ClassName.MethodName", It.Is<List<string>>(l => l.Count == 2)), Times.Once);
        }

        [Fact]
        public async Task Execute_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }

        [Fact]
        public async Task Execute_WithEmptyLibraryPath_ShouldThrowArgumentException()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _command.Execute(options));
        }

        [Fact]
        public async Task Execute_WithNullLibraryPath_ShouldThrowArgumentException()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = null,
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _command.Execute(options));
        }

        [Fact]
        public async Task Execute_WithWhitespaceLibraryPath_ShouldThrowArgumentException()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "   ",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _command.Execute(options));
        }

        [Fact]
        public async Task Execute_WithEmptyEntryPoint_ShouldThrowArgumentException()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "",
                Arguments = new List<string>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _command.Execute(options));
        }

        [Fact]
        public async Task Execute_WithNullEntryPoint_ShouldThrowArgumentException()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = null,
                Arguments = new List<string>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _command.Execute(options));
        }

        [Fact]
        public async Task Execute_WithWhitespaceEntryPoint_ShouldThrowArgumentException()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "   ",
                Arguments = new List<string>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _command.Execute(options));
        }

        [Fact]
        public async Task Execute_WithNullArgumentsList_ShouldExecuteWithEmptyArguments()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = null
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            _mockLibraryExecutor.Verify(x => x.ExecuteAsync("path/to/library.dll", "Namespace.ClassName.MethodName", It.IsAny<List<string>>()), Times.Once);
        }

        [Fact]
        public async Task Execute_WithEmptyArgumentsList_ShouldExecuteWithEmptyArguments()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            _mockLibraryExecutor.Verify(x => x.ExecuteAsync("path/to/library.dll", "Namespace.ClassName.MethodName", It.Is<List<string>>(l => l.Count == 0)), Times.Once);
        }

        [Fact]
        public async Task Execute_WithMultipleArguments_ShouldPassAllArgumentsToExecutor()
        {
            // Arrange
            var arguments = new List<string> { "arg1", "arg2", "arg3", "arg4" };
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = arguments
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            _mockLibraryExecutor.Verify(x => x.ExecuteAsync("path/to/library.dll", "Namespace.ClassName.MethodName", 
                It.Is<List<string>>(l => l.Count == 4 && l[0] == "arg1" && l[3] == "arg4")), Times.Once);
        }

        [Fact]
        public async Task Execute_WhenExecutorReturnsZero_ShouldReturnZero()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_WhenExecutorReturnsPositiveExitCode_ShouldReturnSameExitCode()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Execute_WhenExecutorReturnsNegativeExitCode_ShouldReturnSameExitCode()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(-1);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public async Task Execute_WhenExecutorReturnsLargeExitCode_ShouldReturnSameExitCode()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(256);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(256, result);
        }

        #endregion

        #region Execute With Exception Handling Tests

        [Fact]
        public async Task Execute_WhenExecutorThrowsException_ShouldLogErrorAndRethrow()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            var exception = new InvalidOperationException("Library not found");
            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ThrowsAsync(exception);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _command.Execute(options));
        }

        [Fact]
        public async Task Execute_WhenExecutorThrowsArgumentException_ShouldRethrow()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            var exception = new ArgumentException("Invalid entry point format");
            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ThrowsAsync(exception);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _command.Execute(options));
        }

        [Fact]
        public async Task Execute_WhenExecutorThrowsFileNotFoundException_ShouldRethrow()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/nonexistent/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            var exception = new System.IO.FileNotFoundException("Library file not found");
            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ThrowsAsync(exception);

            // Act & Assert
            await Assert.ThrowsAsync<System.IO.FileNotFoundException>(() => _command.Execute(options));
        }

        #endregion

        #region Entry Point Format Tests

        [Fact]
        public async Task Execute_WithInvalidEntryPointFormat_ShouldThrowArgumentException()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "InvalidFormat",  // Should have at least one dot
                Arguments = new List<string>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _command.Execute(options));
        }

        [Fact]
        public async Task Execute_WithEntryPointWithTwoDots_ShouldExecuteSuccessfully()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_WithEntryPointWithMultipleDots_ShouldExecuteSuccessfully()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "My.Namespace.Sub.ClassName.MethodName",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region Logging Tests

        [Fact]
        public async Task Execute_ShouldLogExecutionStart()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            await _command.Execute(options);

            // Assert
            _mockLogger.Verify(x => x.LogInformation(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Execute_ShouldLogExecutionEnd()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            await _command.Execute(options);

            // Assert
            _mockLogger.Verify(x => x.LogInformation(It.IsAny<string>()), Times.AtLeastOnce);
        }

        #endregion

        #region Library Path Format Tests

        [Fact]
        public async Task Execute_WithAbsoluteLibraryPath_ShouldExecuteSuccessfully()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "/usr/lib/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_WithRelativeLibraryPath_ShouldExecuteSuccessfully()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_WithWindowsAbsoluteLibraryPath_ShouldExecuteSuccessfully()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "C:\\lib\\library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region Arguments Special Cases

        [Fact]
        public async Task Execute_WithArgumentsContainingEmptyStrings_ShouldPassToExecutor()
        {
            // Arrange
            var arguments = new List<string> { "arg1", "", "arg3" };
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = arguments
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            _mockLibraryExecutor.Verify(x => x.ExecuteAsync("path/to/library.dll", "Namespace.ClassName.MethodName", 
                It.Is<List<string>>(l => l.Count == 3 && l[1] == "")), Times.Once);
        }

        [Fact]
        public async Task Execute_WithArgumentsContainingSpecialCharacters_ShouldPassToExecutor()
        {
            // Arrange
            var arguments = new List<string> { "arg with spaces", "arg\"with\"quotes", "arg=value" };
            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = arguments
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_WithLargeNumberOfArguments_ShouldPassAllToExecutor()
        {
            // Arrange
            var arguments = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                arguments.Add($"arg{i}");
            }

            var options = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library.dll",
                EntryPoint = "Namespace.ClassName.MethodName",
                Arguments = arguments
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            _mockLibraryExecutor.Verify(x => x.ExecuteAsync("path/to/library.dll", "Namespace.ClassName.MethodName", 
                It.Is<List<string>>(l => l.Count == 100)), Times.Once);
        }

        #endregion

        #region Concurrent Execution Tests

        [Fact]
        public async Task Execute_WithMultipleConcurrentExecutions_ShouldExecuteAllSuccessfully()
        {
            // Arrange
            var options1 = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library1.dll",
                EntryPoint = "Namespace.ClassName.Method1",
                Arguments = new List<string>()
            };

            var options2 = new ExecuteLibraryCommandOptions
            {
                LibraryPath = "path/to/library2.dll",
                EntryPoint = "Namespace.ClassName.Method2",
                Arguments = new List<string>()
            };

            _mockLibraryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(0);

            // Act
            var task1 = _command.Execute(options1);
            var task2 = _command.Execute(options2);

            var results = await Task.WhenAll(task1, task2);

            // Assert
            Assert.All(results, r => Assert.Equal(0, r));
            _mockLibraryExecutor.Verify(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()), Times.Exactly(2));
        }

        #endregion
    }
}