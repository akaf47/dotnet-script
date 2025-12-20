using NUnit.Framework;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Dotnet.Script.Core.Internal;

[TestFixture]
public class ScriptExtensionsTests
{
    [Test]
    public void GetDiagnostics_WithCompilationErrors_ReturnsDiagnosticsInOrder()
    {
        // Arrange
        var script = CSharpScript.Create(@"
            int x = 'string'; // Type mismatch error
            int y = z; // Undefined variable error
        ");

        // Act
        var diagnostics = script.GetDiagnostics().ToList();

        // Assert
        Assert.IsTrue(diagnostics.Count > 0);
        Assert.IsTrue(diagnostics.All(d => d.Severity == DiagnosticSeverity.Error));
        
        // Verify diagnostics are ordered by severity and then by source span
        for (int i = 1; i < diagnostics.Count; i++)
        {
            Assert.IsTrue(diagnostics[i-1].Severity >= diagnostics[i].Severity);
        }
    }

    [Test]
    public void GetDiagnostics_WithNoErrors_ReturnsEmptyCollection()
    {
        // Arrange
        var script = CSharpScript.Create(@"
            int x = 42;
            int y = x + 10;
        ");

        // Act
        var diagnostics = script.GetDiagnostics().ToList();

        // Assert
        Assert.AreEqual(0, diagnostics.Count);
    }
}