using System;
using Xunit;
using Microsoft.CodeAnalysis.Text;
using Dotnet.Script.Core;
using Microsoft.CodeAnalysis;
using System.Linq;

public class ScriptContextTests
{
    [Fact]
    public void Constructor_WithAllParameters_ShouldInitializeCorrectly()
    {
        // Arrange
        var code = SourceText.From("test code");
        var workingDirectory = "/test/path";
        var args = new[] { "arg1", "arg2" };
        var filePath = "/test/script.csx";
        var optimizationLevel = OptimizationLevel.Release;
        var scriptMode = ScriptMode.Script;
        var packageSources = new[] { "source1", "source2" };

        // Act
        var context = new ScriptContext(code, workingDirectory, args, filePath, optimizationLevel, scriptMode, packageSources);

        // Assert
        Assert.Equal(code, context.Code);
        Assert.Equal(workingDirectory, context.WorkingDirectory);
        Assert.Equal(args, context.Args);
        Assert.Equal(filePath, context.FilePath);
        Assert.Equal(optimizationLevel, context.OptimizationLevel);
        Assert.Equal(ScriptMode.Script, context.ScriptMode);
        Assert.Equal(packageSources, context.PackageSources);
    }

    [Fact]
    public void Constructor_WithNullArgs_ShouldInitializeEmptyArgs()
    {
        // Arrange
        var code = SourceText.From("test code");
        var workingDirectory = "/test/path";

        // Act
        var context = new ScriptContext(code, workingDirectory, null);

        // Assert
        Assert.Empty(context.Args);
    }

    [Fact]
    public void Constructor_WithNullPackageSources_ShouldInitializeEmptyPackageSources()
    {
        // Arrange
        var code = SourceText.From("test code");
        var workingDirectory = "/test/path";

        // Act
        var context = new ScriptContext(code, workingDirectory, null);

        // Assert
        Assert.Empty(context.PackageSources);
    }

    [Fact]
    public void Constructor_WithoutFilePath_ShouldUseProvidedScriptMode()
    {
        // Arrange
        var code = SourceText.From("test code");
        var workingDirectory = "/test/path";
        var scriptMode = ScriptMode.Eval;

        // Act
        var context = new ScriptContext(code, workingDirectory, null, null, OptimizationLevel.Debug, scriptMode);

        // Assert
        Assert.Equal(scriptMode, context.ScriptMode);
    }

    [Fact]
    public void Constructor_WithFilePath_ShouldOverrideScriptModeToScript()
    {
        // Arrange
        var code = SourceText.From("test code");
        var workingDirectory = "/test/path";
        var filePath = "/test/script.csx";
        var scriptMode = ScriptMode.Eval;

        // Act
        var context = new ScriptContext(code, workingDirectory, null, filePath, OptimizationLevel.Debug, scriptMode);

        // Assert
        Assert.Equal(ScriptMode.Script, context.ScriptMode);
    }
}