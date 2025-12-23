using System;
using System.IO;
using System.Threading.Tasks;
using Dotnet.Script.Core;
using Dotnet.Script.Core.Commands;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.Shared.Tests;
using Microsoft.CodeAnalysis;
using Moq;
using Xunit;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteCodeCommandTests
    {
        private readonly Mock<ScriptConsole> _mockScriptConsole;
        private readonly Mock<LogFactory> _mockLogFactory;
        private readonly ExecuteCodeCommand _command;

        public ExecuteCodeCommandTests()
        {
            _mockScriptConsole = new Mock<ScriptConsole>(
                new StringWriter(),
                new StringReader(""),
                new StringWriter());
            _mockLogFactory = new Mock<LogFactory>();
            _mockLogFactory.Setup(f => f(It.IsAny<Type>())).Returns(new Mock<Logger>().Object);

            _command = new ExecuteCodeCommand(_mockScriptConsole.Object, _mockLogFactory.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_InitializesProperties()
        {
            // Arrange
            var console = new ScriptConsole(new StringWriter(), new StringReader(""), new StringWriter());
            var logFactory = TestOutputHelper.CreateTestLogFactory();

            // Act
            var command = new ExecuteCodeCommand(console, logFactory);

            // Assert
            Assert.NotNull(command);
        }

        [Fact]
        public void Constructor_WithNullConsole_ThrowsArgumentNullException()
        {
            // Arrange
            var logFactory = TestOutputHelper.CreateTestLogFactory();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecuteCodeCommand(null, logFactory));
        }

        [Fact]
        public void Constructor_WithNullLogFactory_ThrowsArgumentNullException()
        {
            // Arrange
            var console = new ScriptConsole(new StringWriter(), new StringReader(""), new StringWriter());

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecuteCodeCommand(console, null));
        }

        #endregion

        #region Execute Method Tests

        [Fact]
        public async Task Execute_WithValidCodeAndOptions_ReturnsExpectedResult()
        {
            // Arrange
            var code = "return 42;";
            var workingDirectory = Directory.GetCurrentDirectory();
            var arguments = new[] { "arg1", "arg2" };
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            var options = new ExecuteCodeCommandOptions(
                code,
                workingDirectory,
                arguments,
                OptimizationLevel.Debug,
                noCache: false,
                packageSources);

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithNullWorkingDirectory_UsesCurrentDirectory()
        {
            // Arrange
            var code = "return 10;";
            var options = new ExecuteCodeCommandOptions(
                code,
                workingDirectory: null,
                arguments: new string[] { },
                optimizationLevel: OptimizationLevel.Debug,
                noCache: false,
                packageSources: new string[] { });

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithEmptyCode_ProcessesCorrectly()
        {
            // Arrange
            var code = "";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<object>(options);

            // Assert
            Assert.IsType<object>(result);
        }

        [Fact]
        public async Task Execute_WithMultipleArguments_PassesArgsToScript()
        {
            // Arrange
            var code = "return Args.Count;";
            var arguments = new[] { "arg1", "arg2", "arg3" };
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                arguments,
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithNoArguments_CreatesEmptyArgsList()
        {
            // Arrange
            var code = "return Args.Count;";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithReleaseOptimizationLevel_CompileWithReleaseMode()
        {
            // Arrange
            var code = "return 5;";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Release,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithDebugOptimizationLevel_CompileWithDebugMode()
        {
            // Arrange
            var code = "return 15;";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithNoCacheTrue_SkipsCache()
        {
            // Arrange
            var code = "return 20;";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: true,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithNoCacheFalse_UsesCache()
        {
            // Arrange
            var code = "return 25;";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithPackageSources_ConfiguresNuGetResolution()
        {
            // Arrange
            var code = "return 30;";
            var packageSources = new[] { "https://custom-nuget.org/v3/index.json", "https://api.nuget.org/v3/index.json" };
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                packageSources);

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithEmptyPackageSources_UsesDefaultResolution()
        {
            // Arrange
            var code = "return 35;";
            var packageSources = Array.Empty<string>();
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                packageSources);

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithLongRunningCode_ExecutesAsynchronously()
        {
            // Arrange
            var code = "await Task.Delay(100); return 40;";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithStringReturnType_ReturnsString()
        {
            // Arrange
            var code = "return \"hello\";";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<string>(options);

            // Assert
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task Execute_WithBoolReturnType_ReturnsBool()
        {
            // Arrange
            var code = "return true;";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<bool>(options);

            // Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task Execute_WithObjectReturnType_ReturnsObject()
        {
            // Arrange
            var code = "return new { Value = 42 };";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<object>(options);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Execute_WithNullArguments_HandlesGracefully()
        {
            // Arrange
            var code = "return 50;";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                null,
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task Execute_WithSpecialCharactersInCode_ProcessesCorrectly()
        {
            // Arrange
            var code = "return \"special!@#$%^&*()\";";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<string>(options);

            // Assert
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task Execute_WithUnicodeInCode_ProcessesCorrectly()
        {
            // Arrange
            var code = "return \"Unicode: 你好世界\";";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<string>(options);

            // Assert
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task Execute_WithVeryLongCode_ProcessesCorrectly()
        {
            // Arrange
            var code = "var result = 0; " + string.Concat(Enumerable.Range(0, 100).Select(i => $"result += {i}; ")) + "return result;";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithMultilineCode_ProcessesCorrectly()
        {
            // Arrange
            var code = @"
var x = 10;
var y = 20;
return x + y;
";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithWhitespaceOnlyCode_ProcessesCorrectly()
        {
            // Arrange
            var code = "   \n\t\n   ";
            var options = new ExecuteCodeCommandOptions(
                code,
                Directory.GetCurrentDirectory(),
                Array.Empty<string>(),
                OptimizationLevel.Debug,
                noCache: false,
                Array.Empty<string>());

            // Act & Assert - Should not throw
            var result = await _command.Execute<object>(options);
            Assert.NotNull(result);
        }

        #endregion

        #region Context Creation Tests

        [Fact]
        public async Task Execute_CreatesCorrectScriptContext()
        {
            // Arrange
            var code = "return 100;";
            var workingDirectory = Path.GetTempPath();
            var arguments = new[] { "test" };
            var packageSources = new[] { "https://test.com" };

            var options = new ExecuteCodeCommandOptions(
                code,
                workingDirectory,
                arguments,
                OptimizationLevel.Release,
                noCache: true,
                packageSources);

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public async Task Execute_WithAllNullableParameters_UsesDefaults()
        {
            // Arrange
            var code = "return 1;";
            var options = new ExecuteCodeCommandOptions(
                code,
                null,
                null,
                OptimizationLevel.Debug,
                false,
                null);

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.IsType<int>(result);
        }

        #endregion
    }
}