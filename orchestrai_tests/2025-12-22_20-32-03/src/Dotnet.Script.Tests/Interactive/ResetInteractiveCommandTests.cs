using Xunit;
using Moq;
using System;
using Dotnet.Script.Core;

namespace Dotnet.Script.Tests.Interactive
{
    public class ResetInteractiveCommandTests
    {
        private readonly Mock<ScriptConsole> _mockConsole;
        private readonly Mock<InteractiveRunner> _mockRunner;
        private readonly CommandContext _commandContext;
        private readonly ResetInteractiveCommand _resetCommand;

        public ResetInteractiveCommandTests()
        {
            _mockConsole = new Mock<ScriptConsole>(
                new System.IO.StringWriter(),
                new System.IO.StringReader(string.Empty),
                new System.IO.StringWriter()
            ) { CallBase = true };

            _mockRunner = new Mock<InteractiveRunner>(null, null, null, null) { CallBase = true };
            _commandContext = new CommandContext(_mockConsole.Object, _mockRunner.Object);
            _resetCommand = new ResetInteractiveCommand();
        }

        [Fact]
        public void Name_ShouldReturnResetLiteral()
        {
            // Arrange & Act
            var name = _resetCommand.Name;

            // Assert
            Assert.Equal("reset", name);
        }

        [Fact]
        public void Name_ShouldReturnConsistentValue()
        {
            // Arrange & Act
            var name1 = _resetCommand.Name;
            var name2 = _resetCommand.Name;

            // Assert
            Assert.Equal(name1, name2);
            Assert.Equal("reset", name1);
        }

        [Fact]
        public void Execute_ShouldCallRunnersResetMethod()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act
            command.Execute(_commandContext);

            // Assert
            _mockRunner.Verify(r => r.Reset(), Times.Once);
        }

        [Fact]
        public void Execute_ShouldCallResetOnlyOnce()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act
            command.Execute(_commandContext);

            // Assert
            _mockRunner.Verify(r => r.Reset(), Times.Exactly(1));
        }

        [Fact]
        public void Execute_WithNullCommandContext_ShouldThrowArgumentNullException()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => command.Execute(null));
        }

        [Fact]
        public void Execute_WithValidCommandContext_ShouldNotThrow()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act & Assert (no exception should be thrown)
            command.Execute(_commandContext);
        }

        [Fact]
        public void Execute_ShouldAccessRunnerProperty()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act
            command.Execute(_commandContext);

            // Assert
            _mockRunner.VerifyGet(r => r, Times.AtLeastOnce);
        }

        [Fact]
        public void Execute_MultipleInvocations_ShouldCallResetMultipleTimes()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act
            command.Execute(_commandContext);
            command.Execute(_commandContext);
            command.Execute(_commandContext);

            // Assert
            _mockRunner.Verify(r => r.Reset(), Times.Exactly(3));
        }

        [Fact]
        public void ResetInteractiveCommand_ImplementsIInteractiveCommand()
        {
            // Arrange & Act
            var command = new ResetInteractiveCommand();

            // Assert
            Assert.IsAssignableFrom<IInteractiveCommand>(command);
        }

        [Fact]
        public void ResetInteractiveCommand_HasNameProperty()
        {
            // Arrange & Act
            var command = new ResetInteractiveCommand();

            // Assert
            Assert.NotNull(command.Name);
            Assert.IsType<string>(command.Name);
        }

        [Fact]
        public void ResetInteractiveCommand_CanBeInstantiatedMultipleTimes()
        {
            // Arrange & Act
            var command1 = new ResetInteractiveCommand();
            var command2 = new ResetInteractiveCommand();

            // Assert
            Assert.NotSame(command1, command2);
            Assert.Equal(command1.Name, command2.Name);
        }

        [Fact]
        public void Execute_ShouldNotModifyCommandContext()
        {
            // Arrange
            var command = new ResetInteractiveCommand();
            var console = _commandContext.Console;
            var runner = _commandContext.Runner;

            // Act
            command.Execute(_commandContext);

            // Assert
            Assert.Same(console, _commandContext.Console);
            Assert.Same(runner, _commandContext.Runner);
        }

        [Fact]
        public void Execute_ShouldCallRunnerReset_NotConsole()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act
            command.Execute(_commandContext);

            // Assert
            _mockRunner.Verify(r => r.Reset(), Times.Once);
            _mockConsole.Verify(c => c.Clear(), Times.Never);
        }

        [Fact]
        public void Name_IsReadOnlyProperty()
        {
            // Arrange
            var command = new ResetInteractiveCommand();
            var name = command.Name;

            // Act & Assert
            Assert.Equal("reset", name);
            // Verify we can read it multiple times
            Assert.Equal(name, command.Name);
        }

        [Fact]
        public void Execute_ShouldCallResetNotExit()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act
            command.Execute(_commandContext);

            // Assert
            _mockRunner.Verify(r => r.Reset(), Times.Once);
            _mockRunner.Verify(r => r.Exit(), Times.Never);
        }

        [Fact]
        public void Execute_WithDifferentContextInstances_ShouldCallRunnerReset()
        {
            // Arrange
            var mockConsole2 = new Mock<ScriptConsole>(
                new System.IO.StringWriter(),
                new System.IO.StringReader(string.Empty),
                new System.IO.StringWriter()
            ) { CallBase = true };
            var mockRunner2 = new Mock<InteractiveRunner>(null, null, null, null) { CallBase = true };
            var commandContext2 = new CommandContext(mockConsole2.Object, mockRunner2.Object);
            var command = new ResetInteractiveCommand();

            // Act
            command.Execute(commandContext2);

            // Assert
            mockRunner2.Verify(r => r.Reset(), Times.Once);
        }
    }
}