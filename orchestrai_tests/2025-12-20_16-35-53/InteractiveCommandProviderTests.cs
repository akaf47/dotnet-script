using System;
using NUnit.Framework;
using Dotnet.Script.Core;

[TestFixture]
public class InteractiveCommandProviderTests
{
    private InteractiveCommandProvider _commandProvider;

    [SetUp]
    public void Setup()
    {
        _commandProvider = new InteractiveCommandProvider();
    }

    [Test]
    public void TryProvideCommand_ValidCommand_ShouldReturnTrue()
    {
        // Arrange
        string commandInput = "#reset";

        // Act
        bool result = _commandProvider.TryProvideCommand(commandInput, out IInteractiveCommand command);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(command);
        Assert.AreEqual("reset", command.Name);
    }

    [Test]
    public void TryProvideCommand_CompilerDirective_ShouldReturnFalse()
    {
        // Arrange
        string[] compilerDirectives = { "#r System.dll", "#load script.csx" };

        // Act & Assert
        foreach (string directive in compilerDirectives)
        {
            bool result = _commandProvider.TryProvideCommand(directive, out IInteractiveCommand command);
            Assert.IsFalse(result);
            Assert.IsNull(command);
        }
    }

    [Test]
    public void TryProvideCommand_InvalidCommand_ShouldReturnFalse()
    {
        // Arrange
        string[] invalidCommands = { "#unknown", "#  ", null };

        // Act & Assert
        foreach (string command in invalidCommands)
        {
            bool result = _commandProvider.TryProvideCommand(command, out IInteractiveCommand parsedCommand);
            Assert.IsFalse(result);
            Assert.IsNull(parsedCommand);
        }
    }

    [Test]
    public void TryProvideCommand_ExitCommand_ShouldBeRecognized()
    {
        // Arrange
        string exitCommand = "#exit";

        // Act
        bool result = _commandProvider.TryProvideCommand(exitCommand, out IInteractiveCommand command);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(command);
        Assert.AreEqual("exit", command.Name);
    }

    [Test]
    public void TryProvideCommand_ClsCommand_ShouldBeRecognized()
    {
        // Arrange
        string clsCommand = "#cls";

        // Act
        bool result = _commandProvider.TryProvideCommand(clsCommand, out IInteractiveCommand command);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(command);
        Assert.AreEqual("cls", command.Name);
    }
}