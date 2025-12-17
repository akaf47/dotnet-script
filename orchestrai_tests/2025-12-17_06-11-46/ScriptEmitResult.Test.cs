using System.Linq;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using System.IO;
using Xunit;

public class ScriptEmitResultTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var peStream = new MemoryStream();
        var directiveReferences = new[] { MetadataReference.CreateFromFile("test.dll") };
        var runtimeDependencies = new[] 
        { 
            new RuntimeDependency("test", new[] { new RuntimeAssembly(new System.Reflection.AssemblyName("Test"), "/path") }) 
        };

        // Act
        var result = new ScriptEmitResult(peStream, directiveReferences, runtimeDependencies);

        // Assert
        Assert.Equal(peStream, result.PeStream);
        Assert.Equal(runtimeDependencies, result.RuntimeDependencies);
        Assert.Single(result.DirectiveReferences);
        Assert.True(result.Success);
    }

    [Fact]
    public void Error_ShouldCreateResultWithDiagnostics()
    {
        // Arrange
        var diagnostics = new[]
        {
            Diagnostic.Create(DiagnosticDescriptor.Create("ID", "Title", "Message", "Category", 
                DiagnosticSeverity.Error, true), Location.None)
        };

        // Act
        var result = ScriptEmitResult.Error(diagnostics);

        // Assert
        Assert.Single(result.Diagnostics);
        Assert.False(result.Success);
        Assert.Equal(DiagnosticSeverity.Error, result.Diagnostics[0].Severity);
    }

    [Fact]
    public void Success_ShouldBeFalseWhenErrorDiagnosticsPresent()
    {
        // Arrange
        var diagnostics = new[]
        {
            Diagnostic.Create(DiagnosticDescriptor.Create("ID", "Title", "Message", "Category", 
                DiagnosticSeverity.Error, true), Location.None)
        };

        // Act
        var result = ScriptEmitResult.Error(diagnostics);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Success_ShouldBeTrueWhenNoDiagnostics()
    {
        // Arrange
        var peStream = new MemoryStream();
        var directiveReferences = new[] { MetadataReference.CreateFromFile("test.dll") };
        var runtimeDependencies = new[] 
        { 
            new RuntimeDependency("test", new[] { new RuntimeAssembly(new System.Reflection.AssemblyName("Test"), "/path") }) 
        };

        // Act
        var result = new ScriptEmitResult(peStream, directiveReferences, runtimeDependencies);

        // Assert
        Assert.True(result.Success);
    }
}