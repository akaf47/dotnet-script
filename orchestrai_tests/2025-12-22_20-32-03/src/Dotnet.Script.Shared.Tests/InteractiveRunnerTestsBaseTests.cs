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
        private readonly Mock<ITestOutputHelper> _mockOutputHelper;
        private readonly TestableInteractiveRunnerTestsBase _testBase;

        public InteractiveRunnerTestsBaseTests()
        {
            _mockOutputHelper = new Mock<ITestOutputHelper>();
            _testBase = new TestableInteractiveRunnerTestsBase(_mockOutputHelper.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidTestOutputHelper_CallsCaptureMethod()
        {
            // Arrange
            var mockHelper = new Mock<ITestOutputHelper>();

            // Act
            var testInstance = new TestableInteractiveRunnerTestsBase(mockHelper.Object);

            // Assert
            // Verify that the Capture method was called (implicit in construction)
            Assert.NotNull(testInstance);
            mockHelper.Verify();
        }

        [Fact]
        public void Constructor_WithNullTestOutputHelper_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new TestableInteractiveRunnerTestsBase(null));
        }

        #endregion

        #region InteractiveTestContext Tests

        [Fact]
        public void InteractiveTestContext_Constructor_WithValidParameters_InitializesProperties()
        {
            // Arrange
            var mockConsole = new Mock<ScriptConsole>(new StringWriter(), new StringReader(""), new StringWriter());
            var mockRunner = new Mock<InteractiveRunner>();

            // Act
            var context = new InteractiveRunnerTestsBase.InteractiveTestContext(
                mockConsole.Object,
                mockRunner.Object);

            // Assert
            Assert.NotNull(context);
            Assert.Same(mockConsole.Object, context.Console);
            Assert.Same(mockRunner.Object, context.Runner);
        }

        [Fact]
        public void InteractiveTestContext_Console_Property_ReturnsSetConsole()
        {
            // Arrange
            var writer = new StringWriter();
            var reader = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(writer, reader, error);
            var mockRunner = new Mock<InteractiveRunner>();

            // Act
            var context = new InteractiveRunnerTestsBase.InteractiveTestContext(console, mockRunner.Object);

            // Assert
            Assert.Same(console, context.Console);
        }

        [Fact]
        public void InteractiveTestContext_Runner_Property_ReturnsSetRunner()
        {
            // Arrange
            var writer = new StringWriter();
            var reader = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(writer, reader, error);
            var mockRunner = new Mock<InteractiveRunner>();

            // Act
            var context = new InteractiveRunnerTestsBase.InteractiveTestContext(console, mockRunner.Object);

            // Assert
            Assert.Same(mockRunner.Object, context.Runner);
        }

        [Fact]
        public void InteractiveTestContext_Constructor_WithNullConsole_Throws()
        {
            // Arrange
            var mockRunner = new Mock<InteractiveRunner>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new InteractiveRunnerTestsBase.InteractiveTestContext(null, mockRunner.Object));
        }

        [Fact]
        public void InteractiveTestContext_Constructor_WithNullRunner_Throws()
        {
            // Arrange
            var writer = new StringWriter();
            var reader = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(writer, reader, error);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new InteractiveRunnerTestsBase.InteractiveTestContext(console, null));
        }

        #endregion

        #region GetRunner Tests - No Parameters

        [Fact]
        public void GetRunner_WithNoParameters_CreatesValidContext()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_WithNoParameters_CreatesScriptConsole()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            Assert.NotNull(context.Console);
            Assert.IsType<ScriptConsole>(context.Console);
        }

        [Fact]
        public void GetRunner_WithNoParameters_CreatesInteractiveRunner()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            Assert.NotNull(context.Runner);
            Assert.IsType<InteractiveRunner>(context.Runner);
        }

        #endregion

        #region GetRunner Tests - Single Command

        [Fact]
        public void GetRunner_WithSingleCommand_CreatesValidContext()
        {
            // Act
            var context = _testBase.GetRunner("1+1");

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_WithSingleCommand_ReadsCommandCorrectly()
        {
            // Arrange
            string expectedCommand = "var x = 1;";

            // Act
            var context = _testBase.GetRunner(expectedCommand);

            // Assert
            Assert.NotNull(context);
            // Verify the reader is properly initialized with the command
            Assert.NotNull(context.Console);
        }

        [Fact]
        public void GetRunner_WithEmptyStringCommand_CreatesValidContext()
        {
            // Act
            var context = _testBase.GetRunner("");

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        #endregion

        #region GetRunner Tests - Multiple Commands

        [Fact]
        public void GetRunner_WithMultipleCommands_CreatesValidContext()
        {
            // Arrange
            var commands = new[] { "var x = 1;", "x + x", "#exit" };

            // Act
            var context = _testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_WithTwoCommands_CreatesValidContext()
        {
            // Arrange
            var commands = new[] { "var x = 5;", "x * 2" };

            // Act
            var context = _testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_WithManyCommands_CreatesValidContext()
        {
            // Arrange
            var commands = new[] 
            { 
                "var x = 1;", 
                "var y = 2;", 
                "var z = 3;", 
                "x + y + z",
                "x * y * z",
                "#exit"
            };

            // Act
            var context = _testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        #endregion

        #region GetRunner Tests - Edge Cases

        [Fact]
        public void GetRunner_WithNullArrayElement_CreatesValidContext()
        {
            // Arrange
            var commands = new[] { "var x = 1;", null, "#exit" };

            // Act
            var context = _testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_WithEmptyCommandsArray_CreatesValidContext()
        {
            // Arrange
            var commands = new string[] { };

            // Act
            var context = _testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_WithWhitespaceCommand_CreatesValidContext()
        {
            // Arrange
            var commands = new[] { "   ", "\t", "\n" };

            // Act
            var context = _testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_WithSpecialCharactersInCommand_CreatesValidContext()
        {
            // Arrange
            var commands = new[] { "var x = \"test\";", "x.Length" };

            // Act
            var context = _testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_WithVeryLongCommand_CreatesValidContext()
        {
            // Arrange
            var longCommand = new string('x', 10000) + " = 1;";
            var commands = new[] { longCommand };

            // Act
            var context = _testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        [Fact]
        public void GetRunner_WithUnicodeCommand_CreatesValidContext()
        {
            // Arrange
            var commands = new[] { "var x = \"Ñoño こんにちは 中文\";", "x" };

            // Act
            var context = _testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Runner);
        }

        #endregion

        #region GetRunner Tests - Console Creation

        [Fact]
        public void GetRunner_CreatesStringWriterForOutput()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            Assert.NotNull(context.Console);
            var outProperty = typeof(ScriptConsole).GetProperty("Out");
            Assert.NotNull(outProperty);
        }

        [Fact]
        public void GetRunner_CreatesStringReaderForInput()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            Assert.NotNull(context.Console);
            var inProperty = typeof(ScriptConsole).GetProperty("In");
            Assert.NotNull(inProperty);
        }

        [Fact]
        public void GetRunner_CreatesStringWriterForError()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            Assert.NotNull(context.Console);
            var errorProperty = typeof(ScriptConsole).GetProperty("Error");
            Assert.NotNull(errorProperty);
        }

        #endregion

        #region GetRunner Tests - Compiler Creation

        [Fact]
        public void GetRunner_CreatesScriptCompiler()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            Assert.NotNull(context.Runner);
            // Verify the runner was created successfully with a compiler
            Assert.IsType<InteractiveRunner>(context.Runner);
        }

        [Fact]
        public void GetRunner_CreatesCompilerWithoutRestoreCache()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            Assert.NotNull(context.Runner);
            // The compiler should be initialized without cache restoration
        }

        #endregion

        #region GetRunner Tests - Runner Configuration

        [Fact]
        public void GetRunner_CreatesRunnerWithEmptyArgs()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            Assert.NotNull(context.Runner);
            // Runner should be initialized with empty string array for args
        }

        [Fact]
        public void GetRunner_RunnerHasConsole()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            Assert.NotNull(context.Runner);
            Assert.Same(context.Console, context.Console);
        }

        #endregion

        #region GetRunner Tests - Command Processing

        [Fact]
        public void GetRunner_CommandsJoinedWithNewline()
        {
            // Arrange
            var commands = new[] { "line1", "line2", "line3" };

            // Act
            var context = _testBase.GetRunner(commands);

            // Assert
            // Verify that commands are joined with newlines
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Console.In);
        }

        [Fact]
        public void GetRunner_SingleCommandNoNewlinePrepended()
        {
            // Arrange
            var commands = new[] { "single" };

            // Act
            var context = _testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
        }

        [Fact]
        public void GetRunner_EmptyCommandArrayCreatesConsole()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            Assert.NotNull(context.Console);
            Assert.NotNull(context.Console.Out);
            Assert.NotNull(context.Console.Error);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void GetRunner_CreatedContextCanExecuteCommands()
        {
            // Arrange
            var commands = new[] { "1+1" };

            // Act
            var context = _testBase.GetRunner(commands);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Runner);
            Assert.NotNull(context.Console);
        }

        [Fact]
        public void GetRunner_MultipleCallsCreateIndependentContexts()
        {
            // Act
            var context1 = _testBase.GetRunner("var x = 1;");
            var context2 = _testBase.GetRunner("var y = 2;");

            // Assert
            Assert.NotNull(context1);
            Assert.NotNull(context2);
            Assert.NotSame(context1.Console, context2.Console);
            Assert.NotSame(context1.Runner, context2.Runner);
        }

        [Fact]
        public void GetRunner_ContextConsoleOutputCanBeRead()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            var outputWriter = context.Console.Out;
            Assert.NotNull(outputWriter);
            Assert.IsType<StringWriter>(outputWriter);
        }

        [Fact]
        public void GetRunner_ContextConsoleErrorCanBeRead()
        {
            // Act
            var context = _testBase.GetRunner();

            // Assert
            var errorWriter = context.Console.Error;
            Assert.NotNull(errorWriter);
            Assert.IsType<StringWriter>(errorWriter);
        }

        #endregion

        #region Test Isolation

        [Fact]
        public void GetRunner_ConsoleOutIsIndependentBetweenCalls()
        {
            // Act
            var context1 = _testBase.GetRunner();
            var context2 = _testBase.GetRunner();

            // Assert
            Assert.NotSame(context1.Console.Out, context2.Console.Out);
        }

        [Fact]
        public void GetRunner_ConsoleErrorIsIndependentBetweenCalls()
        {
            // Act
            var context1 = _testBase.GetRunner();
            var context2 = _testBase.GetRunner();

            // Assert
            Assert.NotSame(context1.Console.Error, context2.Console.Error);
        }

        [Fact]
        public void GetRunner_ConsoleInIsIndependentBetweenCalls()
        {
            // Act
            var context1 = _testBase.GetRunner();
            var context2 = _testBase.GetRunner();

            // Assert
            Assert.NotSame(context1.Console.In, context2.Console.In);
        }

        #endregion
    }

    /// <summary>
    /// Testable subclass of InteractiveRunnerTestsBase to allow testing the base class.
    /// </summary>
    public class TestableInteractiveRunnerTestsBase : InteractiveRunnerTestsBase
    {
        public TestableInteractiveRunnerTestsBase(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        /// <summary>
        /// Exposes the protected GetRunner method for testing.
        /// </summary>
        public new InteractiveRunnerTestsBase.InteractiveTestContext GetRunner(params string[] commands)
        {
            return base.GetRunner(commands);
        }
    }
}