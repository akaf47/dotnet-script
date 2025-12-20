using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Logging;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Moq;
using NUnit.Framework;

[TestFixture]
public class InteractiveRunnerTests
{
    private Mock<ScriptCompiler> _mockScriptCompiler;
    private Mock<LogFactory> _mockLogFactory;
    private Mock<ScriptConsole> _mockConsole;
    private Mock<TextWriter> _mockTextWriter;
    private InteractiveRunner _runner;

    [SetUp]
    public void Setup()
    {
        _mockTextWriter = new Mock<TextWriter>();
        _mockConsole = new Mock<ScriptConsole>();
        _mockConsole.Setup(c => c.Out).Returns(_mockTextWriter.Object);
        _mockLogFactory = new Mock<LogFactory>();
        _mockScriptCompiler = new Mock<ScriptCompiler>();

        _runner = new InteractiveRunner(_mockScriptCompiler.Object, _mockLogFactory.Object, _mockConsole.Object, null);
    }

    [Test]
    public async Task Execute_FirstScript_ShouldRunSuccessfully()
    {
        // Arrange
        var mockCompilationContext = new Mock<ScriptCompilationContext<object, InteractiveScriptGlobals>>();
        var mockScript = new Mock<Script<object>>();
        var mockScriptState = new Mock<ScriptState<object>>();

        _mockScriptCompiler
            .Setup(c => c.CreateCompilationContext<object, InteractiveScriptGlobals>(It.IsAny<ScriptContext>()))
            .Returns(mockCompilationContext.Object);

        mockCompilationContext.Setup(cc => cc.Script).Returns(mockScript.Object);
        mockScript
            .Setup(s => s.RunAsync(It.IsAny<InteractiveScriptGlobals>(), It.IsAny<Func<Exception, bool>>()))
            .ReturnsAsync(mockScriptState.Object);

        // Act
        await _runner.Execute("Console.WriteLine('test');");

        // Assert
        _mockScriptCompiler.Verify(c => c.CreateCompilationContext<object, InteractiveScriptGlobals>(It.IsAny<ScriptContext>()), Times.Once);
    }

    [Test]
    public async Task Execute_SubsequentScript_ShouldContinueExecution()
    {
        // Arrange
        var mockCompilationContext = new Mock<ScriptCompilationContext<object, InteractiveScriptGlobals>>();
        var mockScript = new Mock<Script<object>>();
        var mockScriptState = new Mock<ScriptState<object>>();

        _mockScriptCompiler
            .Setup(c => c.CreateCompilationContext<object, InteractiveScriptGlobals>(It.IsAny<ScriptContext>()))
            .Returns(mockCompilationContext.Object);

        mockCompilationContext.Setup(cc => cc.Script).Returns(mockScript.Object);
        mockScript
            .Setup(s => s.RunAsync(It.IsAny<InteractiveScriptGlobals>(), It.IsAny<Func<Exception, bool>>()))
            .ReturnsAsync(mockScriptState.Object);

        // First execution
        await _runner.Execute("int x = 5;");

        // Act
        await _runner.Execute("x * 2;");

        // Assert
        _mockScriptCompiler.Verify(c => c.CreateCompilationContext<object, InteractiveScriptGlobals>(It.IsAny<ScriptContext>()), Times.Once);
    }

    [Test]
    public void Exit_ShouldSetShouldExitToTrue()
    {
        // Act
        _runner.Exit();

        // Use reflection to verify private field
        var field = typeof(InteractiveRunner).GetField("_shouldExit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var value = field.GetValue(_runner);

        // Assert
        Assert.IsTrue((bool)value);
    }

    [Test]
    public void Reset_ShouldClearScriptState()
    {
        // Act
        _runner.Reset();

        // Use reflection to verify private fields
        var stateField = typeof(InteractiveRunner).GetField("_scriptState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var optionsField = typeof(InteractiveRunner).GetField("_scriptOptions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Assert
        Assert.IsNull(stateField.GetValue(_runner));
        Assert.IsNull(optionsField.GetValue(_runner));
    }

    [Test]
    public async Task RunLoopWithSeed_ShouldExecuteInitialScriptAndStartLoop()
    {
        // Arrange
        var mockContext = new Mock<ScriptContext>();
        var mockCompilationContext = new Mock<ScriptCompilationContext<object, InteractiveScriptGlobals>>();
        var mockScript = new Mock<Script<object>>();

        _mockScriptCompiler
            .Setup(c => c.CreateCompilationContext<object, InteractiveScriptGlobals>(It.IsAny<ScriptContext>()))
            .Returns(mockCompilationContext.Object);

        mockCompilationContext.Setup(cc => cc.Script).Returns(mockScript.Object);

        // Act & Assert
        await _runner.RunLoopWithSeed(mockContext.Object);

        // Verify interactions
        _mockScriptCompiler.Verify(c => c.CreateCompilationContext<object, InteractiveScriptGlobals>(It.IsAny<ScriptContext>()), Times.Once);
    }
}