```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace DotNetScript.Tests
{
    public class ScriptHostTests
    {
        private readonly Mock<ILogger<ScriptHost>> _mockLogger;
        private readonly Mock<IScriptRunner> _mockScriptRunner;
        private readonly Mock<IScriptCompiler> _mockCompiler;
        private readonly ScriptHost _scriptHost;

        public ScriptHostTests()
        {
            _mockLogger = new Mock<ILogger<ScriptHost>>();
            _mockScriptRunner = new Mock<IScriptRunner>();
            _mockCompiler = new Mock<IScriptCompiler>();
            _scriptHost = new ScriptHost(_mockLogger.Object, _mockScriptRunner.Object, _mockCompiler.Object);
        }

        [Fact]
        public async Task RunScriptAsync_WithValidScript_ShouldExecuteSuccessfully()
        {
            // Arrange
            var scriptPath = "test.csx";
            var expectedResult = new ScriptExecutionResult { Success = true, Output = "Hello World" };
            _mockScriptRunner.Setup(x => x.ExecuteAsync(scriptPath, It.IsAny<string[]>()))
                            .ReturnsAsync(expectedResult);

            // Act
            var result = await _scriptHost.RunScriptAsync(scriptPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Output.Should().Be("Hello World");
            _mockScriptRunner.Verify(x => x.ExecuteAsync(scriptPath, It.IsAny<string[]>()), Times.Once);
        }

        [Fact]
        public async Task RunScriptAsync_WithArguments_ShouldPassArgumentsToRunner()
        {
            // Arrange
            var scriptPath = "test.csx";
            var arguments = new[] { "arg1", "arg2" };
            var expectedResult = new ScriptExecutionResult { Success = true };
            _mockScriptRunner.Setup(x => x.ExecuteAsync(scriptPath, arguments))
                            .ReturnsAsync(expectedResult);

            // Act
            var result = await _scriptHost.RunScriptAsync(scriptPath, arguments);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockScriptRunner.Verify(x => x.ExecuteAsync(scriptPath, arguments), Times.Once);
        }

        [Fact]
        public async Task RunScriptAsync_WhenRunnerThrows_ShouldHandleException()
        {
            // Arrange
            var scriptPath = "test.csx";
            _mockScriptRunner.Setup(x => x.ExecuteAsync(scriptPath, It.IsAny<string[]>()))
                            .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act
            var result = await _scriptHost.RunScriptAsync(scriptPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Error.Should().Contain("Test exception");
        }

        [Fact]
        public void CompileScript_WithValidCode_ShouldReturnCompilationResult()
        {
            // Arrange
            var code = "Console.WriteLine(\"Hello\");";
            var expectedResult = new CompilationResult { Success = true };
            _mockCompiler.Setup(x => x.Compile(code))
                        .Returns(expectedResult);

            // Act
            var result = _scriptHost.CompileScript(code);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockCompiler.Verify(x => x.Compile(code), Times.Once);
        }

        [Fact]
        public void GetAvailableGlobals_ShouldReturnGlobalVariables()
        {
            // Act
            var globals = _scriptHost.GetAvailableGlobals();

            // Assert
            globals.Should().NotBeNull();
            globals.Should().ContainKey("Args");
            globals.Should().ContainKey("Console");
        }

        [Fact]
        public void SetGlobal_WithValidKeyValue_ShouldAddToGlobals()
        {
            // Arrange
            var key = "TestVariable";
            var value = "TestValue";

            // Act
            _scriptHost.SetGlobal(key, value);
            var globals = _scriptHost.GetAvailableGlobals();

            // Assert
            globals.Should().ContainKey(key);
            globals[key].Should().Be(value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void SetGlobal_WithInvalidKey_ShouldThrowArgumentException(string key)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _scriptHost.SetGlobal(key, "value"));
        }
    }
}
```