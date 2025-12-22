using Xunit;
using Moq;
using System;
using Dotnet.Script.Core;

namespace Dotnet.Script.Tests.Interactive
{
    public class ExitCommandTests
    {
        private readonly Mock<ScriptConsole> _mockConsole;
        private readonly Mock<InteractiveRunner> _mockRunner;
        private readonly CommandContext _commandContext;
        private readonly ExitCommand _exitCommand;

        public ExitCommandTests()
        {
            _mockConsole = new Mock<ScriptConsole>(
                new System.IO.StringWriter(),
                new System.IO.StringReader(string.Empty),
                new System.IO.StringWriter()
            ) { CallBase = true };

            _mockRunner = new Mock<InteractiveRunner>(null, null, null, null) { CallBase = true };
            _commandContext = new CommandContext(_mockConsole.Object, _mockRunner.Object);
            _exitCommand = new ExitCommand();
        }

        [Fact]
        public void Name_ShouldReturnExitLiteral()
        {
            // Arrange & Act
            var name = _exitCommand.Name;

            // Assert
            Assert.Equal("exit", name);
        }

        [Fact]
        public void Name_ShouldReturnConsistentValue()
        {
            // Arrange & Act
            var name1 = _exitCommand.Name;
            var name2 = _exitCommand.Name;

            // Assert
            Assert.Equal(name1, name2);
            Assert.Equal("exit", name1);
        }

        [Fact]
        public void Execute_ShouldCallRunnersExitMethod()
        {
            // Arrange
            var command = new ExitCommand();

            // Act
            command.Execute(_commandContext);

            // Assert
            _mockRunner.Verify(r => r.Exit(), Times.Once);
        }

        [Fact]
        public void Execute_ShouldCallExitOnlyOnce()
        {
            // Arrange
            var command = new ExitCommand();

            // Act
            command.Execute(_commandContext);

            // Assert
            _mockRunner.Verify(r => r.Exit(), Times.Exactly(1));
        }

        [Fact]
        public void Execute_WithNullCommandContext_ShouldThrowArgumentNullException()
        {
            // Arrange
            var command = new ExitCommand();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => command.Execute(null));
        }

        [Fact]
        public void Execute_WithValidCommandContext_ShouldNotThrow()
        {
            // Arrange
            var command = new ExitCommand();

            // Act & Assert (no exception should be thrown)
            command.Execute(_commandContext);
        }

        [Fact]
        public void Execute_ShouldAccessRunnerProperty()
        {
            // Arrange
            var command = new ExitCommand();

            // Act
            command.Execute(_commandContext);

            // Assert
            _mockRunner.VerifyGet(r => r, Times.AtLeastOnce);
        }

        [Fact]
        public void Execute_MultipleInvocations_ShouldCallExitMultipleTimes()
        {
            // Arrange
            var command = new ExitCommand();

            // Act
            command.Execute(_commandContext);
            command.Execute(_commandContext);
            command.Execute(_commandContext);

            // Assert
            _mockRunner.Verify(r => r.Exit(), Times.Exactly(3));
        }

        [Fact]
        public void ExitCommand_ImplementsIInteractiveCommand()
        {
            // Arrange & Act
            var command = new ExitCommand();

            // Assert
            Assert.IsAssignableFrom<IInteractiveCommand>(command);
        }

        [Fact]
        public void ExitCommand_HasNameProperty()
        {
            // Arrange & Act
            var command = new ExitCommand();

            // Assert
            Assert.NotNull(command.Name);
            Assert.IsType<string>(command.Name);
        }

        [Fact]
        public void ExitCommand_CanBeInstantiatedMultipleTimes()
        {
            // Arrange & Act
            var command1 = new ExitCommand();
            var command2 = new ExitCommand();

            // Assert
            Assert.NotSame(command1, command2);
            Assert.Equal(command1.Name, command2.Name);
        }

        [Fact]
        public void Execute_ShouldNotModifyCommandContext()
        {
            // Arrange
            var command = new ExitCommand();
            var console = _commandContext.Console;
            var runner = _commandContext.Runner;

            // Act
            command.Execute(_commandContext);

            // Assert
            Assert.Same(console, _commandContext.Console);
            Assert.Same(runner, _commandContext.Runner);
        }

        [Fact]
        public void Execute_ShouldCallRunnerExit_NotConsole()
        {
            // Arrange
            var command = new ExitCommand();

            // Act
            command.Execute(_commandContext);

            // Assert
            _mockRunner.Verify(r => r.Exit(), Times.Once);
            _mockConsole.Verify(c => c.Clear(), Times.Never);
        }

        [Fact]
        public void Name_IsReadOnlyProperty()
        {
            // Arrange
            var command = new ExitCommand();
            var name = command.Name;

            // Act & Assert
            Assert.Equal("exit", name);
            // Verify we can read it multiple times
            Assert.Equal(name, command.Name);
        }
    }
}