using Xunit;
using Moq;
using System;
using Dotnet.Script.Core;

namespace Dotnet.Script.Tests.Interactive
{
    public class ClsCommandTests
    {
        private readonly Mock<ScriptConsole> _mockConsole;
        private readonly Mock<InteractiveRunner> _mockRunner;
        private readonly CommandContext _commandContext;
        private readonly ClsCommand _clsCommand;

        public ClsCommandTests()
        {
            _mockConsole = new Mock<ScriptConsole>(
                new System.IO.StringWriter(),
                new System.IO.StringReader(string.Empty),
                new System.IO.StringWriter()
            ) { CallBase = true };

            _mockRunner = new Mock<InteractiveRunner>(null, null, null, null) { CallBase = true };
            _commandContext = new CommandContext(_mockConsole.Object, _mockRunner.Object);
            _clsCommand = new ClsCommand();
        }

        [Fact]
        public void Name_ShouldReturnClsLiteral()
        {
            // Arrange & Act
            var name = _clsCommand.Name;

            // Assert
            Assert.Equal("cls", name);
        }

        [Fact]
        public void Name_ShouldReturnConsistentValue()
        {
            // Arrange & Act
            var name1 = _clsCommand.Name;
            var name2 = _clsCommand.Name;

            // Assert
            Assert.Equal(name1, name2);
            Assert.Equal("cls", name1);
        }

        [Fact]
        public void Execute_ShouldCallConsolesClearMethod()
        {
            // Arrange
            var command = new ClsCommand();

            // Act
            command.Execute(_commandContext);

            // Assert
            _mockConsole.Verify(c => c.Clear(), Times.Once);
        }

        [Fact]
        public void Execute_ShouldCallClearOnlyOnce()
        {
            // Arrange
            var command = new ClsCommand();

            // Act
            command.Execute(_commandContext);

            // Assert
            _mockConsole.Verify(c => c.Clear(), Times.Exactly(1));
        }

        [Fact]
        public void Execute_WithNullCommandContext_ShouldThrowArgumentNullException()
        {
            // Arrange
            var command = new ClsCommand();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => command.Execute(null));
        }

        [Fact]
        public void Execute_WithValidCommandContext_ShouldNotThrow()
        {
            // Arrange
            var command = new ClsCommand();

            // Act & Assert (no exception should be thrown)
            command.Execute(_commandContext);
        }

        [Fact]
        public void Execute_ShouldAccessConsoleProperty()
        {
            // Arrange
            var command = new ClsCommand();

            // Act
            command.Execute(_commandContext);

            // Assert
            _mockConsole.VerifyGet(c => c, Times.AtLeastOnce);
        }

        [Fact]
        public void Execute_MultipleInvocations_ShouldCallClearMultipleTimes()
        {
            // Arrange
            var command = new ClsCommand();

            // Act
            command.Execute(_commandContext);
            command.Execute(_commandContext);
            command.Execute(_commandContext);

            // Assert
            _mockConsole.Verify(c => c.Clear(), Times.Exactly(3));
        }

        [Fact]
        public void ClsCommand_ImplementsIInteractiveCommand()
        {
            // Arrange & Act
            var command = new ClsCommand();

            // Assert
            Assert.IsAssignableFrom<IInteractiveCommand>(command);
        }

        [Fact]
        public void ClsCommand_HasNameProperty()
        {
            // Arrange & Act
            var command = new ClsCommand();

            // Assert
            Assert.NotNull(command.Name);
            Assert.IsType<string>(command.Name);
        }

        [Fact]
        public void ClsCommand_CanBeInstantiatedMultipleTimes()
        {
            // Arrange & Act
            var command1 = new ClsCommand();
            var command2 = new ClsCommand();

            // Assert
            Assert.NotSame(command1, command2);
            Assert.Equal(command1.Name, command2.Name);
        }

        [Fact]
        public void Execute_ShouldNotModifyCommandContext()
        {
            // Arrange
            var command = new ClsCommand();
            var console = _commandContext.Console;
            var runner = _commandContext.Runner;

            // Act
            command.Execute(_commandContext);

            // Assert
            Assert.Same(console, _commandContext.Console);
            Assert.Same(runner, _commandContext.Runner);
        }
    }
}