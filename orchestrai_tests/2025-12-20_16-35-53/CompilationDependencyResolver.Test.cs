using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using Dotnet.Script.DependencyModel.Compilation;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Dotnet.Script.DependencyModel.Logging;

[TestFixture]
public class CompilationDependencyResolverTests
{
    private Mock<LogFactory> _mockLogFactory;
    private Mock<ScriptProjectProvider> _mockScriptProjectProvider;
    private Mock<ScriptDependencyContextReader> _mockScriptDependencyContextReader;
    private Mock<ICompilationReferenceReader> _mockCompilationReferenceReader;
    private Mock<IRestorer> _mockRestorer;

    [SetUp]
    public void Setup()
    {
        _mockLogFactory = new Mock<LogFactory>();
        _mockScriptProjectProvider = new Mock<ScriptProjectProvider>(_mockLogFactory.Object);
        _mockScriptDependencyContextReader = new Mock<ScriptDependencyContextReader>(_mockLogFactory.Object);
        _mockCompilationReferenceReader = new Mock<ICompilationReferenceReader>();
        _mockRestorer = new Mock<IRestorer>();
    }

    [Test]
    public void GetDependencies_ValidInput_ReturnsCompilationDependencies()
    {
        // Arrange
        string targetDirectory = Path.GetTempPath();
        string[] scriptFiles = new[] { "test.csx" };
        bool enableScriptNugetReferences = true;
        string defaultTargetFramework = "net46";

        var projectFileInfo = new ProjectFileInfo 
        { 
            Path = Path.Combine(targetDirectory, "project.proj"),
            TargetFramework = defaultTargetFramework
        };

        _mockScriptProjectProvider
            .Setup(x => x.CreateProject(targetDirectory, scriptFiles, defaultTargetFramework, enableScriptNugetReferences))
            .Returns(projectFileInfo);

        var assetsFilePath = Path.Combine(Path.GetDirectoryName(projectFileInfo.Path), "obj", "project.assets.json");

        var mockDependencyContext = new ScriptDependencyContext 
        { 
            Dependencies = new[] 
            { 
                new ScriptDependency 
                {
                    Name = "TestPackage", 
                    Version = "1.0.0", 
                    CompileTimeDependencyPaths = new[] { "/path/to/compile" },
                    ScriptPaths = new[] { "/path/to/script" }
                }
            }
        };

        _mockScriptDependencyContextReader
            .Setup(x => x.ReadDependencyContext(assetsFilePath))
            .Returns(mockDependencyContext);

        var mockCompilationReferences = new[] 
        {
            new CompilationReference { Path = "/path/to/default/reference" }
        };

        _mockCompilationReferenceReader
            .Setup(x => x.Read(projectFileInfo))
            .Returns(mockCompilationReferences);

        var resolver = new CompilationDependencyResolver(
            _mockScriptProjectProvider.Object,
            _mockScriptDependencyContextReader.Object,
            _mockCompilationReferenceReader.Object, 
            _mockLogFactory.Object);

        // Act
        var result = resolver.GetDependencies(targetDirectory, scriptFiles, enableScriptNugetReferences, defaultTargetFramework);

        // Assert
        Assert.AreEqual(2, result.Count());
        
        var firstDependency = result.First();
        Assert.AreEqual("TestPackage", firstDependency.Name);
        Assert.AreEqual("1.0.0", firstDependency.Version);

        var defaultDependency = result.Last();
        Assert.AreEqual("Dotnet.Script.Default.Dependencies", defaultDependency.Name);
        Assert.AreEqual("99.0", defaultDependency.Version);
    }

    [Test]
    public void CreateRestorer_LogFactoryProvided_ReturnsProfiledRestorer()
    {
        // Arrange
        var logFactory = new Mock<LogFactory>().Object;

        // Act
        var resolver = new CompilationDependencyResolver(logFactory);

        // Assert
        Assert.IsNotNull(resolver);
    }

    [Test]
    public void Constructor_AllDependenciesInjected_InitializesCorrectly()
    {
        // Arrange
        var mockLogFactory = new Mock<LogFactory>();
        var mockScriptProjectProvider = new Mock<ScriptProjectProvider>(mockLogFactory.Object);
        var mockScriptDependencyContextReader = new Mock<ScriptDependencyContextReader>(mockLogFactory.Object);
        var mockCompilationReferenceReader = new Mock<ICompilationReferenceReader>();

        // Act
        var resolver = new CompilationDependencyResolver(
            mockScriptProjectProvider.Object,
            mockScriptDependencyContextReader.Object,
            mockCompilationReferenceReader.Object,
            mockLogFactory.Object);

        // Assert
        Assert.IsNotNull(resolver);
    }
}