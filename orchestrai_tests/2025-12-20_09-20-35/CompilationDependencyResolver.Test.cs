using NUnit.Framework;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Dotnet.Script.DependencyModel.Compilation;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Dotnet.Script.DependencyModel.Logging;

[TestFixture]
public class CompilationDependencyResolverTests
{
    private Mock<LogFactory> _mockLogFactory;
    private Mock<ScriptProjectProvider> _mockScriptProjectProvider;
    private Mock<ScriptDependencyContextReader> _mockDependencyContextReader;
    private Mock<ICompilationReferenceReader> _mockCompilationReferenceReader;

    [SetUp]
    public void Setup()
    {
        _mockLogFactory = new Mock<LogFactory>();
        _mockScriptProjectProvider = new Mock<ScriptProjectProvider>(_mockLogFactory.Object);
        _mockDependencyContextReader = new Mock<ScriptDependencyContextReader>(_mockLogFactory.Object);
        _mockCompilationReferenceReader = new Mock<ICompilationReferenceReader>();
    }

    [Test]
    public void Constructor_WithLogFactory_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var resolver = new CompilationDependencyResolver(_mockLogFactory.Object);

        // Assert
        Assert.IsNotNull(resolver);
    }

    [Test]
    public void Constructor_WithDependencies_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var resolver = new CompilationDependencyResolver(
            _mockScriptProjectProvider.Object, 
            _mockDependencyContextReader.Object, 
            _mockCompilationReferenceReader.Object, 
            _mockLogFactory.Object);

        // Assert
        Assert.IsNotNull(resolver);
    }

    [Test]
    public void GetDependencies_WithValidInput_ShouldReturnCompilationDependencies()
    {
        // Arrange
        var targetDirectory = Path.GetTempPath();
        var scriptFiles = new[] { Path.Combine(targetDirectory, "test.csx") };
        
        var projectFileInfo = new ProjectFileInfo("path/to/project.csproj", "path/to/nuget.config");
        _mockScriptProjectProvider
            .Setup(x => x.CreateProject(targetDirectory, scriptFiles, "net46", true))
            .Returns(projectFileInfo);

        var dependencyContext = new ScriptDependencyContext(new[]
        {
            new ScriptDependency("TestPackage", "1.0.0", new[] { "path/to/compile" }, new[] { "path/to/script" })
        });
        _mockDependencyContextReader
            .Setup(x => x.ReadDependencyContext(It.IsAny<string>()))
            .Returns(dependencyContext);

        var compilationReferences = new[]
        {
            new CompilationReference("path/to/reference")
        };
        _mockCompilationReferenceReader
            .Setup(x => x.Read(projectFileInfo))
            .Returns(compilationReferences);

        var resolver = new CompilationDependencyResolver(
            _mockScriptProjectProvider.Object, 
            _mockDependencyContextReader.Object, 
            _mockCompilationReferenceReader.Object, 
            _mockLogFactory.Object);

        // Act
        var dependencies = resolver.GetDependencies(targetDirectory, scriptFiles, true).ToList();

        // Assert
        Assert.AreEqual(2, dependencies.Count);
        Assert.AreEqual("TestPackage", dependencies[0].Name);
        Assert.AreEqual("1.0.0", dependencies[0].Version);
        Assert.AreEqual("Dotnet.Script.Default.Dependencies", dependencies[1].Name);
    }

    [Test]
    public void GetDependencies_WithEmptyScriptFiles_ShouldHandleEmptyInput()
    {
        // Arrange
        var targetDirectory = Path.GetTempPath();
        var scriptFiles = Array.Empty<string>();
        
        var resolver = new CompilationDependencyResolver(_mockLogFactory.Object);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            resolver.GetDependencies(targetDirectory, scriptFiles, true).ToList());
    }
}