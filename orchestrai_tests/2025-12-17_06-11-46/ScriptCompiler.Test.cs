using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting;
using Moq;
using Xunit;

public class ScriptCompilerTests
{
    private ScriptCompiler CreateCompiler(LogFactory logFactory = null, bool useRestoreCache = true)
    {
        logFactory ??= _ => _ => { };
        return new ScriptCompiler(logFactory, useRestoreCache);
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var logFactory = new Mock<LogFactory>().Object;

        // Act
        var compiler = CreateCompiler(logFactory);

        // Assert
        Assert.NotNull(compiler);
        Assert.NotNull(compiler.RuntimeDependencyResolver);
    }

    [Fact]
    public void ImportedNamespaces_ShouldContainStandardNamespaces()
    {
        // Arrange
        var compiler = CreateCompiler();

        // Act
        var namespaces = compiler.ImportedNamespaces;

        // Assert
        Assert.Contains("System", namespaces);
        Assert.Contains("System.Linq", namespaces);
        Assert.Contains("System.Threading.Tasks", namespaces);
    }

    [Fact]
    public void CreateScriptOptions_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange
        var compiler = CreateCompiler();
        var dependencies = new List<RuntimeDependency>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            compiler.CreateScriptOptions(null, dependencies));
    }

    [Fact]
    public void CreateScriptOptions_ShouldConfigureOptionsCorrectly()
    {
        // Arrange
        var compiler = CreateCompiler();
        var context = new ScriptContext(SourceText.From("test code"), ".");
        var dependencies = new List<RuntimeDependency>();

        // Act
        var options = compiler.CreateScriptOptions(context, dependencies);

        // Assert
        Assert.NotNull(options);
        Assert.Contains(options.Imports, ns => ns == "System");
        Assert.Equal(Encoding.UTF8, options.FileEncoding);
    }

    [Fact]
    public void CreateCompilationContext_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange
        var compiler = CreateCompiler();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            compiler.CreateCompilationContext<object, object>(null));
    }

    [Fact]
    public void CreateScriptDependenciesMap_ShouldPickHighestVersion()
    {
        // Arrange
        var dependencies = new[]
        {
            new RuntimeDependency("test", new[]
            {
                new RuntimeAssembly(new AssemblyName("Test, Version=1.0.0.0"), "/path1"),
                new RuntimeAssembly(new AssemblyName("Test, Version=2.0.0.0"), "/path2")
            })
        };

        // Act
        var map = ScriptCompiler.CreateScriptDependenciesMap(dependencies);

        // Assert
        Assert.Single(map);
        Assert.Equal("2.0.0.0", map["Test"].Name.Version.ToString());
    }

    [Fact]
    public void GetRuntimeDependencies_ForScript_ShouldResolveCorrectly()
    {
        // Arrange
        var compiler = CreateCompiler();
        var context = new ScriptContext(SourceText.From("test"), "/test/path.csx") 
        { 
            ScriptMode = ScriptMode.Script 
        };

        // Act
        var method = typeof(ScriptCompiler)
            .GetMethod("GetRuntimeDependencies", BindingFlags.NonPublic | BindingFlags.Instance);
        var dependencies = method.Invoke(compiler, new object[] { context }) as RuntimeDependency[];

        // Assert
        Assert.NotNull(dependencies);
    }

    [Fact]
    public void GetScriptCode_WithRawCode_ShouldInjectNewLines()
    {
        // Arrange
        var compiler = CreateCompiler();
        var context = new ScriptContext(SourceText.From("#define TEST\ncode"), null);

        // Act
        var method = typeof(ScriptCompiler)
            .GetMethod("GetScriptCode", BindingFlags.NonPublic | BindingFlags.Instance);
        var code = method.Invoke(compiler, new object[] { context }) as string;

        // Assert
        Assert.Contains(Environment.NewLine, code);
    }
}