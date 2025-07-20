using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptExecutorTests
    {
        private readonly Mock<ILogger<ScriptExecutor>> _mockLogger;
        private readonly Mock<IScriptCompiler> _mockCompiler;
        private readonly ScriptExecutor _executor;

        public ScriptExecutorTests()
        {
            _mockLogger = new Mock<ILogger<ScriptExecutor>>();
            _mockCompiler = new Mock<IScriptCompiler>();
            _executor = new ScriptExecutor(_mockLogger.Object, _mockCompiler.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Assert.NotNull(_executor);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ScriptExecutor(null, _mockCompiler.Object));
        }

        [Fact]
        public void Constructor_WithNullCompiler_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ScriptExecutor(_mockLogger.Object, null));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidCode_ShouldExecuteSuccessfully()
        {
            // Arrange
            var code = "Console.WriteLine(\"Hello World\");";
            var mockResult = new Mock<ICompilationResult>();
            mockResult.Setup(r => r.Success).Returns(true);
            _mockCompiler.Setup(c => c.CompileAsync(code)).ReturnsAsync(mockResult.Object);

            // Act
            var result = await _executor.ExecuteAsync(code);

            // Assert
            Assert.Equal(0, result.ExitCode);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ExecuteAsync_WithCompilationError_ShouldReturnError()
        {
            // Arrange
            var code = "invalid syntax";
            var mockResult = new Mock<ICompilationResult>();
            mockResult.Setup(r => r.Success).Returns(false);
            mockResult.Setup(r => r.Errors).Returns(new[] { "Compilation error" });
            _mockCompiler.Setup(c => c.CompileAsync(code)).ReturnsAsync(mockResult.Object);

            // Act
            var result = await _executor.ExecuteAsync(code);

            // Assert
            Assert.NotEqual(0, result.ExitCode);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullCode_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _executor.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteWithArgsAsync_WithValidCodeAndArgs_ShouldExecuteSuccessfully()
        {
            // Arrange
            var code = "Console.WriteLine($\"Args: {string.Join(\", \", Args)}\");";
            var args = new[] { "arg1", "arg2" };
            var mockResult = new Mock<ICompilationResult>();
            mockResult.Setup(r => r.Success).Returns(true);
            _mockCompiler.Setup(c => c.CompileAsync(It.IsAny<string>())).ReturnsAsync(mockResult.Object);

            // Act
            var result = await _executor.ExecuteWithArgsAsync(code, args);

            // Assert
            Assert.Equal(0, result.ExitCode);
            Assert.True(result.Success);
        }

        [Theory]
        [InlineData(new string[0])]
        [InlineData(new[] { "single" })]
        [InlineData(new[] { "arg1", "arg2", "arg3" })]
        public async Task ExecuteWithArgsAsync_WithDifferentArgCounts_ShouldHandleCorrectly(string[] args)
        {
            // Arrange
            var code = "Console.WriteLine(Args.Count);";
            var mockResult = new Mock<ICompilationResult>();
            mockResult.Setup(r => r.Success).Returns(true);
            _mockCompiler.Setup(c => c.CompileAsync(It.IsAny<string>())).ReturnsAsync(mockResult.Object);

            // Act
            var result = await _executor.ExecuteWithArgsAsync(code, args);

            // Assert
            Assert.Equal(0, result.ExitCode);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ExecuteFileAsync_WithValidScriptPath_ShouldExecuteSuccessfully()
        {
            // Arrange
            var scriptPath = "test.csx";
            var mockResult = new Mock<ICompilationResult>();
            mockResult.Setup(r => r.Success).Returns(true);
            _mockCompiler.Setup(c => c.CompileScriptFileAsync(scriptPath)).ReturnsAsync(mockResult.Object);

            // Act
            var result = await _executor.ExecuteFileAsync(scriptPath);

            // Assert
            Assert.Equal(0, result.ExitCode);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ExecuteFileAsync_WithNullPath_Shoul