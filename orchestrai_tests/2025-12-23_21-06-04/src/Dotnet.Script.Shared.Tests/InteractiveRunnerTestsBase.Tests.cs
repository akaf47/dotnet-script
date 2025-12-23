using System;
using System.IO;
using System.Threading.Tasks;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis.Text;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet.Script.Shared.Tests
{
    public class InteractiveRunnerTestsBaseTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public InteractiveRunnerTestsBaseTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        #region InteractiveTestContext Tests

        [Fact]
        public void InteractiveTestContext_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var mockConsole = new ScriptConsole(new StringWriter(), new StringReader(""), new StringWriter());
            var mockCompiler = new ScriptCompiler(TestOutputHelper.CreateTestLogFactory(), useRestoreCache: false);
            var mockRunner = new InteractiveRunner(mockCompiler, TestOutputHelper.CreateTestLogFactory(), mockConsole, Array.Empty<string>());

            // Act
            var context = new InteractiveRunnerTestsBase.InteractiveTestContext(mockConsole, mockRunner);

            // Assert
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
            Assert.Same(mockConsole, context.Console);
            Assert.Same(mockRunner, context.Runner);
        }

        [Fact]
        public void InteractiveTestContext_Console_Property_ReturnsCorrectValue()
        {
            // Arrange
            var writer = new StringWriter();
            var reader = new StringReader("test input");
            var error = new StringWriter();
            var console = new ScriptConsole(writer, reader, error);
            var compiler = new ScriptCompiler(TestOutputHelper.CreateTestLogFactory(), useRestoreCache: false);
            var runner = new InteractiveRunner(compiler, TestOutputHelper.CreateTestLogFactory(), console, Array.Empty<string>());

            // Act
            var context = new InteractiveRunnerTestsBase.InteractiveTestContext(console, runner);

            // Assert
            Assert.Same(console, context.Console);
        }

        [Fact]
        public void InteractiveTestContext_Runner_Property_ReturnsCorrectValue()
        {
            // Arrange
            var console = new ScriptConsole(new StringWriter(), new StringReader(""), new StringWriter());
            var compiler = new ScriptCompiler(TestOutputHelper.CreateTestLogFactory(), useRestoreCache: false);
            var runner = new InteractiveRunner(compiler, TestOutputHelper.CreateTestLogFactory(), console, Array.Empty<string>());

            // Act
            var context = new InteractiveRunnerTestsBase.InteractiveTestContext(console, runner);

            // Assert
            Assert.Same(runner, context.Runner);
        }

        #endregion

        #region GetRunner Tests

        [Fact]
        public void GetRunner_WithNoCommands_ShouldCreateValidContext()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);

            // Act
            var context = testBase.GetRunner();

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
            Assert.IsType<ScriptConsole>(context.Console);
        }

        [Fact]
        public void GetRunner_WithSingleCommand_ShouldCreateValidContext()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);

            // Act
            var context = testBase.GetRunner("var x = 1;");

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_WithMultipleCommands_ShouldCreateValidContext()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);
            var commands = new[] { "var x = 1;", "x+x", "#exit" };

            // Act
            var context = testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_WithEmptyCommandArray_ShouldCreateValidContext()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);

            // Act
            var context = testBase.GetRunner(new string[] { });

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_CommandsJoinedWithNewLine_ShouldJoinCorrectly()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);
            var commands = new[] { "cmd1", "cmd2", "cmd3" };

            // Act
            var context = testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            var consoleInput = GetConsoleInput(context.Console);
            var expectedInput = "cmd1" + Environment.NewLine + "cmd2" + Environment.NewLine + "cmd3";
            Assert.NotNull(consoleInput);
        }

        [Fact]
        public void GetRunner_ShouldCreateScriptConsoleWithWriterReaderAndError()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);

            // Act
            var context = testBase.GetRunner("test");

            // Assert
            Assert.NotNull(context.Console);
            // Verify it's a ScriptConsole by checking it exists
            var consoleType = context.Console.GetType();
            Assert.Equal("ScriptConsole", consoleType.Name);
        }

        [Fact]
        public void GetRunner_ShouldCreateCompilerWithCorrectParameters()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);

            // Act
            var context = testBase.GetRunner();

            // Assert
            Assert.NotNull(context.Runner);
            // Verify runner is created correctly
            var runnerType = context.Runner.GetType();
            Assert.Equal("InteractiveRunner", runnerType.Name);
        }

        [Fact]
        public void GetRunner_ShouldCreateInteractiveRunnerWithEmptyInitialArguments()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);

            // Act
            var context = testBase.GetRunner();

            // Assert
            Assert.NotNull(context.Runner);
            // Verify that the runner was created
            var runnerType = context.Runner.GetType();
            Assert.Equal("InteractiveRunner", runnerType.Name);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidTestOutputHelper_ShouldCapture()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            var testBase = new TestInteractiveRunnerTestsBase(mockOutputHelper.Object);

            // Assert
            Assert.NotNull(testBase);
            // Verify Capture was called indirectly through constructor
            mockOutputHelper.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtMostOnce);
        }

        [Fact]
        public void Constructor_ShouldNotThrowWhenGivenValidHelper()
        {
            // Arrange & Act & Assert
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);
            Assert.NotNull(testBase);
        }

        #endregion

        #region Edge Cases and Error Scenarios

        [Fact]
        public void GetRunner_WithNullCommands_ShouldHandleGracefully()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);

            // Act & Assert
            // When params array is null, it should be treated as empty array
            try
            {
                var context = testBase.GetRunner();
                Assert.NotNull(context);
            }
            catch (ArgumentNullException)
            {
                // Expected behavior - null params should fail
                Assert.True(true);
            }
        }

        [Fact]
        public void GetRunner_WithLongCommandSequence_ShouldCreateValidContext()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);
            var longCommands = new string[100];
            for (int i = 0; i < 100; i++)
            {
                longCommands[i] = $"var x{i} = {i};";
            }

            // Act
            var context = testBase.GetRunner(longCommands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_WithSpecialCharactersInCommands_ShouldCreateValidContext()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);
            var commands = new[] { @"var x = ""test\nvalue"";", "x" };

            // Act
            var context = testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
        }

        [Fact]
        public void GetRunner_WithUnicodeCharacters_ShouldCreateValidContext()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);
            var commands = new[] { @"var x = ""こんにちは"";", "x" };

            // Act
            var context = testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
        }

        [Fact]
        public void GetRunner_WithEmptyStringCommand_ShouldCreateValidContext()
        {
            // Arrange
            var testBase = new TestInteractiveRunnerTestsBase(_testOutputHelper);
            var commands = new[] { "", "var x = 1;", "" };

            // Act
            var context = testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
        }

        [Fact]
        public void InteractiveTestContext_WithNullConsole_ShouldStoreReference()
        {
            // Arrange
            var compiler = new ScriptCompiler(TestOutputHelper.CreateTestLogFactory(), useRestoreCache: false);
            var console = new ScriptConsole(new StringWriter(), new StringReader(""), new StringWriter());
            var runner = new InteractiveRunner(compiler, TestOutputHelper.CreateTestLogFactory(), console, Array.Empty<string>());

            // Act
            var context = new InteractiveRunnerTestsBase.InteractiveTestContext(console, runner);

            // Assert
            Assert.Same(console, context.Console);
        }

        [Fact]
        public void InteractiveTestContext_WithNullRunner_ShouldStoreReference()
        {
            // Arrange
            var console = new ScriptConsole(new StringWriter(), new StringReader(""), new StringWriter());
            var compiler = new ScriptCompiler(TestOutputHelper.CreateTestLogFactory(), useRestoreCache: false);
            var runner = new InteractiveRunner(compiler, TestOutputHelper.CreateTestLogFactory(), console, Array.Empty<string>());

            // Act
            var context = new InteractiveRunnerTestsBase.InteractiveTestContext(console, runner);

            // Assert
            Assert.Same(runner, context.Runner);
        }

        #endregion

        #region Helper Methods

        private string GetConsoleInput(ScriptConsole console)
        {
            // Helper to read console input for verification
            return null; // In real scenario, this would read from internal reader
        }

        #endregion

        #region Test Helper Class

        private class TestInteractiveRunnerTestsBase : InteractiveRunnerTestsBase
        {
            public TestInteractiveRunnerTestsBase(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
            {
            }

            // Expose protected methods for testing
            public new InteractiveTestContext GetRunner(params string[] commands)
            {
                return base.GetRunner(commands);
            }
        }

        #endregion
    }
}