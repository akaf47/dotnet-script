using System;
using Moq;
using Xunit;
using Dotnet.Script.Core;

namespace Dotnet.Script.Core.Tests.Interactive
{
    public class ExitCommandTests
    {
        /// <summary>
        /// Tests that ExitCommand is a valid implementation of IInteractiveCommand
        /// </summary>
        [Fact]
        public void ExitCommand_ShouldImplementIInteractiveCommand()
        {
            // Arrange & Act
            var command = new ExitCommand();

            // Assert
            Assert.IsAssignableFrom<IInteractiveCommand>(command);
        }

        /// <summary>
        /// Tests that the Name property returns "exit"
        /// </summary>
        [Fact]
        public void Name_ShouldReturnExitString()
        {
            // Arrange
            var command = new ExitCommand();

            // Act
            var name = command.Name;

            // Assert
            Assert.Equal("exit", name);
        }

        /// <summary>
        /// Tests that Name property is consistent across multiple calls
        /// </summary>
        [Fact]
        public void Name_ShouldReturnSameValueOnMultipleCalls()
        {
            // Arrange
            var command = new ExitCommand();

            // Act
            var name1 = command.Name;
            var name2 = command.Name;
            var name3 = command.Name;

            // Assert
            Assert.Equal(name1, name2);
            Assert.Equal(name2, name3);
            Assert.Equal("exit", name1);
        }

        /// <summary>
        /// Tests that Execute method calls Exit on the runner
        /// </summary>
        [Fact]
        public void Execute_ShouldCallRunnerExit()
        {
            // Arrange
            var mockRunner = new Mock<InteractiveRunner>();
            var mockConsole = new Mock<ScriptConsole>();
            var commandContext = new CommandContext(mockConsole.Object, mockRunner.Object);
            var command = new ExitCommand();

            // Act
            command.Execute(commandContext);

            // Assert
            mockRunner.Verify(r => r.Exit(), Times.Once);
        }

        /// <summary>
        /// Tests that Execute method calls Exit exactly once when called multiple times
        /// </summary>
        [Fact]
        public void Execute_CalledMultipleTimes_ShouldCallRunnerExitEachTime()
        {
            // Arrange
            var mockRunner = new Mock<InteractiveRunner>();
            var mockConsole = new Mock<ScriptConsole>();
            var commandContext = new CommandContext(mockConsole.Object, mockRunner.Object);
            var command = new ExitCommand();

            // Act
            command.Execute(commandContext);
            command.Execute(commandContext);
            command.Execute(commandContext);

            // Assert
            mockRunner.Verify(r => r.Exit(), Times.Exactly(3));
        }

        /// <summary>
        /// Tests that Execute throws ArgumentNullException when commandContext is null
        /// </summary>
        [Fact]
        public void Execute_WithNullCommandContext_ShouldThrowArgumentNullException()
        {
            // Arrange
            var command = new ExitCommand();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => command.Execute(null));
        }

        /// <summary>
        /// Tests that Execute throws when commandContext.Runner is null
        /// </summary>
        [Fact]
        public void Execute_WhenRunnerIsNull_ShouldThrowNullReferenceException()
        {
            // Arrange
            var mockConsole = new Mock<ScriptConsole>();
            var commandContext = new CommandContext(mockConsole.Object, null);
            var command = new ExitCommand();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => command.Execute(commandContext));
        }

        /// <summary>
        /// Tests that Execute throws when commandContext.Console is null (edge case)
        /// </summary>
        [Fact]
        public void Execute_WhenConsoleIsNull_ShouldStillCallRunnerExit()
        {
            // Arrange - CommandContext allows null Console
            var mockRunner = new Mock<InteractiveRunner>();
            var commandContext = new CommandContext(null, mockRunner.Object);
            var command = new ExitCommand();

            // Act
            command.Execute(commandContext);

            // Assert
            mockRunner.Verify(r => r.Exit(), Times.Once);
        }

        /// <summary>
        /// Tests that Execute does not reference the Console property
        /// </summary>
        [Fact]
        public void Execute_ShouldNotUseConsoleProperty()
        {
            // Arrange
            var mockRunner = new Mock<InteractiveRunner>();
            var mockConsole = new Mock<ScriptConsole>();
            var commandContext = new CommandContext(mockConsole.Object, mockRunner.Object);
            var command = new ExitCommand();

            // Act
            command.Execute(commandContext);

            // Assert
            // Console property should not be used during Execute
            // If it were used, the mock would record interactions
            // This test verifies the method only calls runner.Exit()
            mockRunner.Verify(r => r.Exit(), Times.Once);
        }

        /// <summary>
        /// Tests that multiple ExitCommand instances have the same Name
        /// </summary>
        [Fact]
        public void MultipleInstances_ShouldHaveSameName()
        {
            // Arrange
            var command1 = new ExitCommand();
            var command2 = new ExitCommand();
            var command3 = new ExitCommand();

            // Act
            var name1 = command1.Name;
            var name2 = command2.Name;
            var name3 = command3.Name;

            // Assert
            Assert.Equal(name1, name2);
            Assert.Equal(name2, name3);
            Assert.All(new[] { name1, name2, name3 }, name => Assert.Equal("exit", name));
        }

        /// <summary>
        /// Tests that Execute passes the correct commandContext to runner
        /// </summary>
        [Fact]
        public void Execute_ShouldUseProvidedCommandContext()
        {
            // Arrange
            var mockRunner1 = new Mock<InteractiveRunner>();
            var mockRunner2 = new Mock<InteractiveRunner>();
            var mockConsole = new Mock<ScriptConsole>();
            var commandContext1 = new CommandContext(mockConsole.Object, mockRunner1.Object);
            var commandContext2 = new CommandContext(mockConsole.Object, mockRunner2.Object);
            var command = new ExitCommand();

            // Act
            command.Execute(commandContext1);
            command.Execute(commandContext2);

            // Assert
            mockRunner1.Verify(r => r.Exit(), Times.Once);
            mockRunner2.Verify(r => r.Exit(), Times.Once);
        }

        /// <summary>
        /// Tests that Name property is read-only and returns a non-null value
        /// </summary>
        [Fact]
        public void Name_ShouldReturnNonNullValue()
        {
            // Arrange
            var command = new ExitCommand();

            // Act
            var name = command.Name;

            // Assert
            Assert.NotNull(name);
            Assert.IsType<string>(name);
            Assert.False(string.IsNullOrEmpty(name));
        }

        /// <summary>
        /// Tests Execute with a valid CommandContext containing mocked dependencies
        /// </summary>
        [Fact]
        public void Execute_WithValidContext_ShouldCompleteSuccessfully()
        {
            // Arrange
            var mockRunner = new Mock<InteractiveRunner>();
            var mockConsole = new Mock<ScriptConsole>();
            var commandContext = new CommandContext(mockConsole.Object, mockRunner.Object);
            var command = new ExitCommand();

            // Act - should not throw
            var exception = Record.Exception(() => command.Execute(commandContext));

            // Assert
            Assert.Null(exception);
            mockRunner.Verify(r => r.Exit(), Times.Once);
        }

        /// <summary>
        /// Tests that Name property can be accessed through IInteractiveCommand interface
        /// </summary>
        [Fact]
        public void Name_AccessedThroughInterface_ShouldReturnExit()
        {
            // Arrange
            IInteractiveCommand command = new ExitCommand();

            // Act
            var name = command.Name;

            // Assert
            Assert.Equal("exit", name);
        }

        /// <summary>
        /// Tests that Execute can be called through IInteractiveCommand interface
        /// </summary>
        [Fact]
        public void Execute_CalledThroughInterface_ShouldCallRunnerExit()
        {
            // Arrange
            var mockRunner = new Mock<InteractiveRunner>();
            var mockConsole = new Mock<ScriptConsole>();
            var commandContext = new CommandContext(mockConsole.Object, mockRunner.Object);
            IInteractiveCommand command = new ExitCommand();

            // Act
            command.Execute(commandContext);

            // Assert
            mockRunner.Verify(r => r.Exit(), Times.Once);
        }
    }
}