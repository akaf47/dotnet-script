using NUnit.Framework;
using Moq;
using System.IO;
using System.Linq;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Environment;

[TestFixture]
public class ScriptProjectProviderTests
{
    private Mock<LogFactory> _mockLogFactory;
    private Mock<Logger> _mockLogger;
    private ScriptProjectProvider _provider;

    [SetUp]
    public void Setup()
    {
        _mockLogFactory = new Mock<LogFactory>();
        _mockLogger = new Mock<Logger>();
        _mockLogFactory.Setup(x => x.CreateLogger<ScriptProjectProvider>()).Returns(_mockLogger.Object);
        _provider = new ScriptProjectProvider(_mockLogFactory.Object);
    }

    [Test]
    public void CreateProjectForRepl_WithValidCode_ShouldCreateProjectFile()
    {
        // Arrange
        var code = @"#r ""nuget:SomePackage""
                     var x = 42;";
        var targetDirectory = Path.GetTempPath();

        // Act
        var projectFileInfo = _provider.CreateProjectForRepl(code, targetDirectory);

        // Assert
        Assert.IsNotNull(projectFileInfo);
        Assert.IsTrue(File.Exists(projectFileInfo.Path));
        Assert.IsTrue(projectFileInfo.Path.Contains("interactive"));
    }

    [Test]
    public void CreateProject_WithNoScriptFiles_ShouldReturnNull()
    {
        // Arrange
        var targetDirectory = Path.GetTempPath();

        // Act
        var projectFileInfo = _provider.CreateProject(targetDirectory, Enumerable.Empty<string>());

        // Assert
        Assert.IsNull(projectFileInfo);
    }

    [Test]
    public void CreateProjectForScriptFile_WithValidScriptFile_ShouldCreateProjectFile()
    {
        // Arrange
        var scriptFile = Path.Combine(Path.GetTempPath(), "test.csx");
        File.WriteAllText(scriptFile, "#r \"nuget:SomePackage\"");

        // Act
        var projectFileInfo = _provider.CreateProjectForScriptFile(scriptFile);

        // Assert
        Assert.IsNotNull(projectFileInfo);
        Assert.IsTrue(File.Exists(projectFileInfo.Path));
    }

    [Test]
    public void GetPathToProjectFile_WithValidInput_ShouldGenerateCorrectPath()
    {
        // Arrange
        var targetDirectory = Path.GetTempPath();
        var targetFramework = "net46";

        // Act
        var projectFilePath = ScriptProjectProvider.GetPathToProjectFile(targetDirectory, targetFramework);

        // Assert
        Assert.IsTrue(projectFilePath.EndsWith("script.csproj"));
        Assert.IsTrue(projectFilePath.Contains(targetFramework));
    }

    [Test]
    public void CreateProjectFileFromScriptFiles_WithValidFiles_ShouldCreateProjectFile()
    {
        // Arrange
        var csxFiles = new[] { Path.Combine(Path.GetTempPath(), "test.csx") };
        File.WriteAllText(csxFiles[0], "#r \"nuget:SomePackage\"");
        var targetFramework = "net46";

        // Act
        var projectFile = _provider.CreateProjectFileFromScriptFiles(targetFramework, csxFiles);

        // Assert
        Assert.IsNotNull(projectFile);
        Assert.AreEqual(targetFramework, projectFile.TargetFramework);
    }
}