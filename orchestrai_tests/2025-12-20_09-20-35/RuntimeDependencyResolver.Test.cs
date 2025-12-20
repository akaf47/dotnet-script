using NUnit.Framework;
using Moq;
using System.IO;
using System.Linq;
using Dotnet.Script.DependencyModel.Runtime;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Environment;

[TestFixture]
public class RuntimeDependencyResolverTests
{
    private Mock<LogFactory> _mockLogFactory;
    private Mock<ScriptProjectProvider> _mockScriptProjectProvider;
    private RuntimeDependencyResolver _resolver;

    [SetUp]
    public void Setup()
    {
        _mockLogFactory = new Mock<LogFactory>();
        _mockScriptProjectProvider = new Mock<ScriptProjectProvider>(_mockLogFactory.Object);
        _resolver = new RuntimeDependencyResolver(_mockScriptProjectProvider.Object, _mockLogFactory.Object, false);
    }

    [Test]
    public void Constructor_WithLogFactory_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var resolver = new RuntimeDependencyResolver(_mockLogFactory.Object, false);

        // Assert
        Assert.IsNotNull(resolver);
    }

    [Test]
    public void GetDependencies_WithValidScriptFile_ShouldReturnRuntimeDependencies()
    {
        // Arrange
        var scriptFile = Path.Combine(Path.GetTempPath(), "test.csx");
        var projectFileInfo = new ProjectFileInfo("path/to/project.csproj", "path/to/nuget.config");
        
        _mockScriptProjectProvider
            .Setup(x => x.CreateProjectForScriptFile(scriptFile))
            .Returns(projectFileInfo);

        // Act
        var dependencies = _resolver.GetDependencies(scriptFile, new[] { "nuget.org" }).ToList();

        // Assert
        Assert.IsNotNull(dependencies);
    }

    [Test]
    public void GetDependenciesForLibrary_WithValidLibraryPath_ShouldReturnRuntimeDependencies()
    {
        // Arrange
        var libraryPath = Path.Combine(Path.GetTempPath(), "library.dll");

        // Act
        var dependencies = _resolver.GetDependenciesForLibrary(libraryPath).ToList();

        // Assert
        Assert.IsNotNull(dependencies);
    }

    [Test]
    public void GetDependenciesForCode_WithValidInput_ShouldReturnRuntimeDependencies()
    {
        // Arrange
        var targetDirectory = Path.GetTempPath();
        var code = "var x = 42;";
        var projectFileInfo = new ProjectFileInfo("path/to/project.csproj", "path/to/nuget.config");

        _mockScriptProjectProvider
            .Setup(x => x.CreateProjectForRepl(code, It.IsAny<string>(), ScriptEnvironment.Default.TargetFramework))
            .Returns(projectFileInfo);

        // Act
        var dependencies = _resolver.GetDependenciesForCode(targetDirectory, ScriptMode.Repl, new[] { "nuget.org" }, code).ToList();

        // Assert
        Assert.IsNotNull(dependencies);
    }
}