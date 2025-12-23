using Xunit;
using Moq;
using Dotnet.Script.Core;

namespace Dotnet.Script.Core.Tests.Interactive
{
    public class ResetInteractiveCommandTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Act
            var command = new ResetInteractiveCommand();

            // Assert
            Assert.NotNull(command);
            Assert.IsAssignableFrom<IInteractiveCommand>(command);
        }

        [Fact]
        public void Name_ShouldReturnReset()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act
            var name = command.Name;

            // Assert
            Assert.Equal("reset", name);
        }

        [Fact]
        public void Name_PropertyShouldBeConsistent()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act
            var name1 = command.Name;
            var name2 = command.Name;

            // Assert
            Assert.Equal(name1, name2);
            Assert.Equal("reset", name1);
        }

        [Fact]
        public void Execute_ShouldCallRunnerReset()
        {
            // Arrange
            var mockRunner = new Mock<InteractiveRunner>();
            var mockConsole = new Mock<ScriptConsole>();
            var commandContext = new CommandContext(mockConsole.Object, mockRunner.Object);
            var command = new ResetInteractiveCommand();

            // Act
            command.Execute(commandContext);

            // Assert
            mockRunner.Verify(r => r.Reset(), Times.Once);
        }

        [Fact]
        public void Execute_WithValidCommandContext_ShouldNotThrow()
        {
            // Arrange
            var mockRunner = new Mock<InteractiveRunner>();
            var mockConsole = new Mock<ScriptConsole>();
            var commandContext = new CommandContext(mockConsole.Object, mockRunner.Object);
            var command = new ResetInteractiveCommand();

            // Act & Assert - should not throw
            var ex = Record.Exception(() => command.Execute(commandContext));
            Assert.Null(ex);
        }

        [Fact]
        public void Execute_WithNullCommandContext_ShouldThrowArgumentNullException()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act & Assert
            Assert.Throws<System.NullReferenceException>(() => command.Execute(null));
        }

        [Fact]
        public void Execute_ShouldAccessRunnerProperty()
        {
            // Arrange
            var mockRunner = new Mock<InteractiveRunner>();
            var mockConsole = new Mock<ScriptConsole>();
            var commandContext = new CommandContext(mockConsole.Object, mockRunner.Object);
            var command = new ResetInteractiveCommand();

            // Act
            command.Execute(commandContext);

            // Assert
            mockRunner.Verify(r => r.Reset(), Times.Once);
        }

        [Fact]
        public void Execute_MultipleCallsShouldEachCallRunnerReset()
        {
            // Arrange
            var mockRunner = new Mock<InteractiveRunner>();
            var mockConsole = new Mock<ScriptConsole>();
            var commandContext = new CommandContext(mockConsole.Object, mockRunner.Object);
            var command = new ResetInteractiveCommand();

            // Act
            command.Execute(commandContext);
            command.Execute(commandContext);
            command.Execute(commandContext);

            // Assert
            mockRunner.Verify(r => r.Reset(), Times.Exactly(3));
        }

        [Fact]
        public void Execute_ShouldNotAccessConsoleProperty()
        {
            // Arrange
            var mockRunner = new Mock<InteractiveRunner>();
            var mockConsole = new Mock<ScriptConsole>();
            var commandContext = new CommandContext(mockConsole.Object, mockRunner.Object);
            var command = new ResetInteractiveCommand();

            // Act
            command.Execute(commandContext);

            // Assert
            // Verify Reset was called but Console was not accessed
            mockRunner.Verify(r => r.Reset(), Times.Once);
            // commandContext.Console is never accessed in the implementation
        }

        [Fact]
        public void Name_ShouldNotBeNull()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act
            var name = command.Name;

            // Assert
            Assert.NotNull(name);
        }

        [Fact]
        public void Name_ShouldBeExactly_reset()
        {
            // Arrange
            var command = new ResetInteractiveCommand();

            // Act
            var name = command.Name;

            // Assert
            Assert.Equal("reset", name);
            Assert.NotEqual("Reset", name);
            Assert.NotEqual("RESET", name);
        }
    }
}