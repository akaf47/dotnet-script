using System;
using System.IO;
using System.Linq;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.CodeAnalysis.Text;
using Moq;
using Xunit;

public class ScriptEmitterTests
{
    private ScriptEmitter CreateEmitter(ScriptConsole scriptConsole = null, ScriptCompiler scriptCompiler = null)
    {
        scriptConsole ??= new ScriptConsole();
        scriptCompiler ??= new Mock<ScriptCompiler>().Object;
        return new ScriptEmitter(scriptConsole, scriptCompiler);
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var mockConsole = new Mock<ScriptConsole>().Object;
        var mockCompiler = new Mock<ScriptCompiler>().Object;

        // Act
        var emitter = new ScriptEmitter(mockConsole, mockCompiler);

        // Assert
        Assert.NotNull(emitter);
    }

    [Fact]
    public void Emit_WithCompilationErrors_ShouldThrowCompilationErrorException()
    {
        // Arrange
        var mockCompiler = new Mock<ScriptCompiler>();
        var mockConsole = new Mock<ScriptConsole>().Object;
        var context = new ScriptContext(SourceText.From("test code"), ".");
        
        var mockCompilationContext = new Mock<ScriptCompilationContext<object>>();
        mockCompilationContext.Setup(x => x.Errors).Returns(new[]
        {
            Diagnostic.Create(DiagnosticDescriptor.Create("ID", "Title", "Message", "Category", 
                DiagnosticSeverity.Error, true), Location.None)
        });

        mockCompiler
            .Setup(x => x.CreateCompilationContext<object, object>(It.IsAny<ScriptContext>()))
            .Returns(mockCompilationContext.Object);

        var emitter = new ScriptEmitter(mockConsole, mockCompiler.Object);

        // Act & Assert
        Assert.Throws<CompilationErrorException>(() => 
            emitter.Emit<object, object>(context, "TestAssembly"));
    }

    [Fact]
    public void Emit_WithWarnings_ShouldWriteWarningsToConsole()
    {
        // Arrange
        var mockCompiler = new Mock<ScriptCompiler>();
        var mockConsole = new Mock<ScriptConsole>();
        var context = new ScriptContext(SourceText.From("test code"), ".");
        
        var mockCompilationContext = new Mock<ScriptCompilationContext<object>>();
        mockCompilationContext.Setup(x => x.Warnings).Returns(new[]
        {
            Diagnostic.Create(DiagnosticDescriptor.Create("ID", "Title", "Message", "Category", 
                DiagnosticSeverity.Warning, true), Location.None)
        });
        mockCompilationContext.Setup(x => x.Errors).Returns(Array.Empty<Diagnostic>());

        var mockScript = new Mock<Script<object>>();
        var mockCompilation = new Mock<Compilation>();
        mockCompilation.Setup(x => x.WithAssemblyName(It.IsAny<string>())).Returns(mockCompilation.Object);
        mockScript.Setup(x => x.GetCompilation()).Returns(mockCompilation.Object);
        mockCompilationContext.Setup(x => x.Script).Returns(mockScript.Object);

        mockCompiler
            .Setup(x => x.CreateCompilationContext<object, object>(It.IsAny<ScriptContext>()))
            .Returns(mockCompilationContext.Object);

        var emitter = new ScriptEmitter(mockConsole.Object, mockCompiler.Object);

        // Act
        emitter.Emit<object, object>(context, "TestAssembly");

        // Assert
        mockConsole.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void Emit_WithSuccessfulCompilation_ShouldReturnScriptEmitResult()
    {
        // Arrange
        var mockCompiler = new Mock<ScriptCompiler>();
        var mockConsole = new Mock<ScriptConsole>().Object;
        var context = new ScriptContext(SourceText.From("test code"), ".");
        context.OptimizationLevel = OptimizationLevel.Debug;
        
        var mockCompilationContext = new Mock<ScriptCompilationContext<object>>();
        mockCompilationContext.Setup(x => x.Warnings).Returns(Array.Empty<Diagnostic>());
        mockCompilationContext.Setup(x => x.Errors).Returns(Array.Empty<Diagnostic>());
        mockCompilationContext.Setup(x => x.RuntimeDependencies).Returns(Array.Empty<RuntimeDependency>());

        var mockScript = new Mock<Script<object>>();
        var mockCompilation = new Mock<Compilation>();
        mockCompilation.Setup(x => x.WithAssemblyName(It.IsAny<string>())).Returns(mockCompilation.Object);
        mockCompilation.Setup(x => x.Emit(It.IsAny<MemoryStream>(), It.IsAny<EmitOptions>()))
            .Returns(new EmitResult(true, ImmutableArray<Diagnostic>.Empty));
        mockScript.Setup(x => x.GetCompilation()).Returns(mockCompilation.Object);
        mockCompilationContext.Setup(x => x.Script).Returns(mockScript.Object);

        mockCompiler
            .Setup(x => x.CreateCompilationContext<object, object>(It.IsAny<ScriptContext>()))
            .Returns(mockCompilationContext.Object);

        var emitter = new ScriptEmitter(mockConsole, mockCompiler.Object);

        // Act
        var result = emitter.Emit<object, object>(context, "TestAssembly");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.PeStream);
    }
}