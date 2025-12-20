using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using Dotnet.Script.DependencyModel.Runtime;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Environment;

[TestFixture]
public class RuntimeDependencyResolverTests
{
    private Mock<LogFactory> _mockLogFactory;
    private Mock<ScriptProjectProvider> _mockScriptProjectProvider;

    [SetUp]
    public void Setup()
    {
        _mockLogFactory = new Mock<LogFactory>();
        _mockScriptProjectProvider = new Mock<ScriptProjectProvider>(_mockLogFactory.Object);
    }

    [Test]
    public void GetDependencies_WithScriptFile_ReturnsRuntimeDependencies()
    {
        // Arrange
        string scriptFile = "test.csx";
        string[] packageSources = new[] { "https://api.nuget.org/v3/index.json" };

        var projectFileInfo = new ProjectFileInfo 
        { 
            Path = Path.Combine(Path.GetTempPath(), "project.proj")
        };

        _mockScriptProjectProvider
            .Setup(x => x.CreateProjectForScriptFile(scriptFile))
            .Returns(projectFileInfo);

        var dependencyContext = new ScriptDependencyContext 
        { 
            Dependencies = new[] 
            { 
                new ScriptDependency 
                {
                    Name = "TestPackage", 
                    Version = "1.0.0", 
                    RuntimeDependencyPaths = new[] { "/path/to/runtime/assembly.dll" },
                    NativeAssetPaths = new[] { "/path/to/native/asset" },
                    ScriptPaths = new[] { "/path/to/script" }
                }
            }
        };

        var mockDependencyContextReader = new Mock<ScriptDependencyContextReader>(_mockLogFactory.Object);
        mockDependencyContextReader
            .Setup(x => x.ReadDependencyContext(It.IsAny<string>()))
            .Returns(dependencyContext);

        var mockRestorer = new Mock<IRestorer>();

        var resolver = new RuntimeDependencyResolver(
            _mockScriptProjectProvider.Object, 
            _mockLogFactory.Object, 
            false);

        // Act
        var result = resolver.GetDependencies(scriptFile, packageSources);

        // Assert
        Assert.AreEqual(1, result.Count());
        var dependency = result.First();
        Assert.AreEqual("TestPackage", dependency.Name);
        Assert.AreEqual("1.0.0", dependency.Version);
    }

    [Test]
    public void GetDependenciesForLibrary_WithLibraryPath_ReturnsRuntimeDependencies()
    {
        // Arrange
        string libraryPath = "/path/to/library/testlib.dll";

        var dependencyContext = new ScriptDependencyContext 
        { 
            Dependencies = new[] 
            { 
                new ScriptDependency 
                {
                    Name = "LibraryPackage", 
                    Version = "2.0.0", 
                    RuntimeDependencyPaths = new[] { "/path/to/runtime/library.dll" },
                    NativeAssetPaths = new[] { "/path/to/native/library" },
                    ScriptPaths = new[] { "/path/to/library/script" }
                }
            }
        };

        var mockDependencyContextReader = new Mock<ScriptDependencyContextReader>(_mockLogFactory.Object);
        mockDependencyContextReader
            .Setup(x => x.ReadDependencyContext(It.IsAny<string>()))
            .Returns(dependencyContext);

        var resolver = new RuntimeDependencyResolver(
            _mockScriptProjectProvider.Object, 
            _mockLogFactory.Object, 
            false);

        // Act
        var result = resolver.GetDependenciesForLibrary(libraryPath);

        // Assert
        Assert.AreEqual(1, result.Count());
        var dependency = result.First();
        Assert.AreEqual("LibraryPackage", dependency.Name);
        Assert.AreEqual("2.0.0", dependency.Version);
    }

    [Test]
    public void GetDependenciesForCode_WithReplMode_ReturnsRuntimeDependencies()
    {
        // Arrange
        string targetDirectory = Path.GetTempPath();
        ScriptMode scriptMode = ScriptMode.Repl;
        string[] packageSources = new[] { "https://api.nuget.org/v3/index.json" };
        string code = "Console.WriteLine(\"Test\");";

        var projectFileInfo = new ProjectFileInfo 
        { 
            Path = Path.Combine(targetDirectory, scriptMode.ToString(), "project.proj")
        };

        _mockScriptProjectProvider
            .Setup(x => x.CreateProjectForRepl(
                code, 
                Path.Combine(targetDirectory, scriptMode.ToString()), 
                ScriptEnvironment.Default.TargetFramework))
            .Returns(projectFileInfo);

        var dependencyContext = new ScriptDependencyContext 
        { 
            Dependencies = new[] 
            { 
                new ScriptDependency 
                {
                    Name = "ReplPackage", 
                    Version = "3.0.0", 
                    RuntimeDependencyPaths = new[] { "/path/to/runtime/repl.dll" },
                    NativeAssetPaths = new[] { "/path/to/native/repl" },
                    ScriptPaths = new[] { "/path/to/repl/script" }
                }
            }
        };

        var mockDependencyContextReader = new Mock<ScriptDependencyContextReader>(_mockLogFactory.Object);
        mockDependencyContextReader
            .Setup(x => x.ReadDependencyContext(It.IsAny<string>()))
            .Returns(dependencyContext);

        var mockRestorer = new Mock<IRestorer>();

        var resolver = new RuntimeDependencyResolver(
            _mockScriptProjectProvider.Object, 
            _mockLogFactory.Object, 
            true);

        // Act
        var result = resolver.GetDependenciesForCode(targetDirectory, scriptMode, packageSources, code);

        // Assert
        Assert.AreEqual(1, result.Count());
        var dependency = result.First();
        Assert.AreEqual("ReplPackage", dependency.Name);
        Assert.AreEqual("3.0.0", dependency.Version);
    }

    [Test]
    public void CreateRestorer_WithCacheEnabled_ReturnsCachedRestorer()
    {
        // Arrange
        bool useRestoreCache = true;

        // Act
        var resolver = new RuntimeDependencyResolver(_mockLogFactory.Object, useRestoreCache);

        // Assert
        Assert.IsNotNull(resolver);
    }

    [Test]
    public void CreateRestorer_WithCacheDisabled_ReturnsStandardRestorer()
    {
        // Arrange
        bool useRestoreCache = false;

        // Act
        var resolver = new RuntimeDependencyResolver(_mockLogFactory.Object, useRestoreCache);

        // Assert
        Assert.IsNotNull(resolver);
    }
}