using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting;
using Moq;
using Xunit;

public class ScriptCompilerExtendedTests
{
    private ScriptCompiler CreateCompiler(LogFactory logFactory = null, bool useRestoreCache = true)
    {
        logFactory ??= _ => _ => { };
        return new ScriptCompiler(logFactory, useRestoreCache);
    }

    [Fact]
    public void Constructor_ShouldSetNullableErrorDiagnosticOptions()
    {
        // Arrange & Act
        var compiler = CreateCompiler();
        var diagnosticOptions = compiler.GetType()
            .GetField("SpecificDiagnosticOptions", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(compiler) as Dictionary<string, ReportDiagnostic>;

        // Assert
        for (var i = 8600; i <= 8655; i++)
        {
            Assert.Contains($"CS{i}", diagnosticOptions.Keys);
            Assert.Equal(ReportDiagnostic.Error, diagnosticOptions[$"CS{i}"]);
        }
    }

    [Fact]
    public void SuppressedDiagnosticIds_ShouldContainExpectedIds()
    {
        // Arrange
        var compiler = CreateCompiler();

        // Act
        var suppressedIds = compiler.GetType()
            .GetProperty("SuppressedDiagnosticIds", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(compiler) as IEnumerable<string>;

        // Assert
        Assert.Contains("CS1701", suppressedIds);
        Assert.Contains("CS1702", suppressedIds);
        Assert.Contains("CS1705", suppressedIds);
    }

    [Fact]
    public void CreateScriptOptions_WhenFilePathProvided_ShouldSetFilePath()
    {
        // Arrange
        var compiler = CreateCompiler();
        var context = new ScriptContext(SourceText.From("test code"), "/working/dir") 
        { 
            FilePath = "/specific/file.csx" 
        };
        var dependencies = new List<RuntimeDependency>();

        // Act
        var options = compiler.CreateScriptOptions(context, dependencies);

        // Assert
        Assert.Equal("/specific/file.csx", options.FilePath);
    }

    [Fact]
    public void CreateCompilationContext_WithOptimizationLevels_ShouldConfigureCompilation()
    {
        // Arrange
        var compiler = CreateCompiler();
        var context = new ScriptContext(SourceText.From("test"), "/working/dir") 
        { 
            OptimizationLevel = OptimizationLevel.Release 
        };

        // Act
        var compilationContext = compiler.CreateCompilationContext<object, object>(context);

        // Assert
        Assert.NotNull(compilationContext);
        Assert.NotNull(compilationContext.Script);
    }

    [Fact]
    public void CreateLoadedAssembliesMap_ShouldWorkWithMultipleAssemblies()
    {
        // Arrange & Act
        var method = typeof(ScriptCompiler)
            .GetMethod("CreateLoadedAssembliesMap", BindingFlags.NonPublic | BindingFlags.Static);
        var map = method.Invoke(null, null) as Dictionary<string, Assembly>;

        // Assert
        Assert.NotNull(map);
        Assert.True(map.Count > 0);
    }

    [Fact]
    public void MapUnresolvedAssemblyToRuntimeLibrary_WhenMatchFound_ShouldRedirectAssembly()
    {
        // Arrange
        var compiler = CreateCompiler();
        var dependencyMap = new Dictionary<string, RuntimeAssembly>
        {
            { "TestAssembly", new RuntimeAssembly(new AssemblyName("TestAssembly, Version=2.0.0.0"), "/test/path") }
        };
        var loadedAssemblyMap = new Dictionary<string, Assembly>();
        var resolveArgs = new ResolveEventArgs("TestAssembly, Version=1.0.0.0");

        // Act
        var method = typeof(ScriptCompiler)
            .GetMethod("MapUnresolvedAssemblyToRuntimeLibrary", BindingFlags.NonPublic | BindingFlags.Instance);
        var result = method.Invoke(compiler, new object[] { dependencyMap, loadedAssemblyMap, resolveArgs }) as Assembly;

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void AddScriptReferences_WithHomogeneousAssemblies_ShouldAddReferences()
    {
        // Arrange
        var compiler = CreateCompiler();
        var scriptOptions = ScriptOptions.Default;
        var loadedAssembliesMap = new Dictionary<string, Assembly>();
        var dependenciesMap = new Dictionary<string, RuntimeAssembly>
        {
            { "TestLib", new RuntimeAssembly(new AssemblyName("TestLib"), "/test/library.dll") }
        };

        // Act
        var method = typeof(ScriptCompiler)
            .GetMethod("AddScriptReferences", BindingFlags.NonPublic | BindingFlags.Instance);
        var result = method.Invoke(compiler, new object[] { scriptOptions, loadedAssembliesMap, dependenciesMap }) as ScriptOptions;

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetRuntimeDependencies_ForCodeMode_ShouldResolveCorrectly()
    {
        // Arrange
        var compiler = CreateCompiler();
        var context = new ScriptContext(SourceText.From("test code"), "/working/dir")
        {
            ScriptMode = ScriptMode.Code,
            WorkingDirectory = "/working/dir"
        };

        // Act
        var method = typeof(ScriptCompiler)
            .GetMethod("GetRuntimeDependencies", BindingFlags.NonPublic | BindingFlags.Instance);
        var dependencies = method.Invoke(compiler, new object[] { context }) as RuntimeDependency[];

        // Assert
        Assert.NotNull(dependencies);
    }
}