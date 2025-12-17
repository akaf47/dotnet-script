using System.Linq;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.CodeAnalysis.Text;
using Moq;
using Xunit;

public class ScriptCompilationContextTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var mockScript = new Mock<Script<int>>().Object;
        var sourceText = SourceText.From("test code");
        var mockLoader = new Mock<InteractiveAssemblyLoader>().Object;
        var scriptOptions = ScriptOptions.Default;
        var runtimeDependencies = new RuntimeDependency[0];
        var diagnostics = new[]
        {
            Diagnostic.Create(DiagnosticDescriptor.Create("ID1", "Title", "Message", "Category", 
                DiagnosticSeverity.Warning, true), Location.None),
            Diagnostic.Create(DiagnosticDescriptor.Create("ID2", "Title", "Message", "Category", 
                DiagnosticSeverity.Error, true), Location.None)
        };

        // Act
        var context = new ScriptCompilationContext<int>(
            mockScript, sourceText, mockLoader, scriptOptions, 
            runtimeDependencies, diagnostics);

        // Assert
        Assert.Equal(mockScript, context.Script);
        Assert.Equal(sourceText, context.SourceText);
        Assert.Equal(mockLoader, context.Loader);
        Assert.Equal(scriptOptions, context.ScriptOptions);
        Assert.Equal(runtimeDependencies, context.RuntimeDependencies);
    }

    [Fact]
    public void Warnings_ShouldFilterWarningDiagnostics()
    {
        // Arrange
        var mockScript = new Mock<Script<int>>().Object;
        var sourceText = SourceText.From("test code");
        var mockLoader = new Mock<InteractiveAssemblyLoader>().Object;
        var scriptOptions = ScriptOptions.Default;
        var runtimeDependencies = new RuntimeDependency[0];
        var diagnostics = new[]
        {
            Diagnostic.Create(DiagnosticDescriptor.Create("ID1", "Title", "Message", "Category", 
                DiagnosticSeverity.Warning, true), Location.None),
            Diagnostic.Create(DiagnosticDescriptor.Create("ID2", "Title", "Message", "Category", 
                DiagnosticSeverity.Error, true), Location.None)
        };

        // Act
        var context = new ScriptCompilationContext<int>(
            mockScript, sourceText, mockLoader, scriptOptions, 
            runtimeDependencies, diagnostics);

        // Assert
        Assert.Single(context.Warnings);
        Assert.Equal(DiagnosticSeverity.Warning, context.Warnings[0].Severity);
    }

    [Fact]
    public void Errors_ShouldFilterErrorDiagnostics()
    {
        // Arrange
        var mockScript = new Mock<Script<int>>().Object;
        var sourceText = SourceText.From("test code");
        var mockLoader = new Mock<InteractiveAssemblyLoader>().Object;
        var scriptOptions = ScriptOptions.Default;
        var runtimeDependencies = new RuntimeDependency[0];
        var diagnostics = new[]
        {
            Diagnostic.Create(DiagnosticDescriptor.Create("ID1", "Title", "Message", "Category", 
                DiagnosticSeverity.Warning, true), Location.None),
            Diagnostic.Create(DiagnosticDescriptor.Create("ID2", "Title", "Message", "Category", 
                DiagnosticSeverity.Error, true), Location.None)
        };

        // Act
        var context = new ScriptCompilationContext<int>(
            mockScript, sourceText, mockLoader, scriptOptions, 
            runtimeDependencies, diagnostics);

        // Assert
        Assert.Single(context.Errors);
        Assert.Equal(DiagnosticSeverity.Error, context.Errors[0].Severity);
    }
}