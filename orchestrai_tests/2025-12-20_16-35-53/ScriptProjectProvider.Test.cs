using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Environment;
using Moq;

[TestFixture]
public class ScriptProjectProviderTests
{
    private Mock<LogFactory> _mockLogFactory;
    private Mock<Logger> _mockLogger;
    private ScriptProjectProvider _scriptProjectProvider;

    [SetUp]
    public void Setup()
    {
        _mockLogFactory = new Mock<LogFactory>();
        _mockLogger = new Mock<Logger>();
        _mockLogFactory.Setup(x => x.CreateLogger<ScriptProjectProvider>()).Returns(_mockLogger.Object);
        _scriptProjectProvider = new ScriptProjectProvider(_mockLogFactory.Object);
    }

    [Test]
    public void CreateProjectForRepl_WithCode_CreatesProjectFile()
    {
        // Arrange
        string code = @"#r ""nuget:Newtonsoft.Json,12.0.3""";
        string targetDirectory = Path.GetTempPath();

        // Act
        var projectFileInfo = _scriptProjectProvider.CreateProjectForRepl(code, targetDirectory);

        try 
        {
            // Assert
            Assert.IsNotNull(projectFileInfo);
            Assert.IsTrue(File.Exists(projectFileInfo.ProjectFilePath));
            var content = File.ReadAllText(projectFileInfo.ProjectFilePath);
            Assert.IsTrue(content.Contains("<PackageReference Include=\"Newtonsoft.Json\" Version=\"12.0.3\" />"));
        }
        finally 
        {
            if (File.Exists(projectFileInfo.ProjectFilePath))
                File.Delete(projectFileInfo.ProjectFilePath);
        }
    }

    [Test]
    public void CreateProjectForRepl_WithMultipleCodeReferences_CreatesProjectFileWithAllReferences()
    {
        // Arrange
        string code = @"#r ""nuget:Newtonsoft.Json,12.0.3""
                        #load ""nuget:Microsoft.Extensions.Logging,5.0.0""";
        string targetDirectory = Path.GetTempPath();

        // Act
        var projectFileInfo = _scriptProjectProvider.CreateProjectForRepl(code, targetDirectory);

        try 
        {
            // Assert
            Assert.IsNotNull(projectFileInfo);
            Assert.IsTrue(File.Exists(projectFileInfo.ProjectFilePath));
            var content = File.ReadAllText(projectFileInfo.ProjectFilePath);
            Assert.IsTrue(content.Contains("<PackageReference Include=\"Newtonsoft.Json\" Version=\"12.0.3\" />"));
            Assert.IsTrue(content.Contains("<PackageReference Include=\"Microsoft.Extensions.Logging\" Version=\"5.0.0\" />"));
        }
        finally 
        {
            if (File.Exists(projectFileInfo.ProjectFilePath))
                File.Delete(projectFileInfo.ProjectFilePath);
        }
    }

    [Test]
    public void CreateProject_WithScriptFiles_CreatesProjectFile()
    {
        // Arrange
        string targetDirectory = Path.GetTempPath();
        var tempFile = Path.Combine(targetDirectory, "script.csx");
        File.WriteAllText(tempFile, @"#r ""nuget:Newtonsoft.Json,12.0.3""");

        try 
        {
            // Act
            var projectFileInfo = _scriptProjectProvider.CreateProject(targetDirectory);

            // Assert
            Assert.IsNotNull(projectFileInfo);
            Assert.IsTrue(File.Exists(projectFileInfo.ProjectFilePath));
            var content = File.ReadAllText(projectFileInfo.ProjectFilePath);
            Assert.IsTrue(content.Contains("<PackageReference Include=\"Newtonsoft.Json\" Version=\"12.0.3\" />"));
        }
        finally 
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Test]
    public void CreateProject_WithNoScriptFiles_ReturnsNull()
    {
        // Arrange
        string targetDirectory = Path.GetTempPath();

        // Act
        var projectFileInfo = _scriptProjectProvider.CreateProject(targetDirectory);

        // Assert
        Assert.IsNull(projectFileInfo);
    }

    [Test]
    public void CreateProjectForScriptFile_CreatesProjectFile()
    {
        // Arrange
        var tempFile = Path.GetTempFileName() + ".csx";
        File.WriteAllText(tempFile, @"#r ""nuget:Newtonsoft.Json,12.0.3""");

        try 
        {
            // Act
            var projectFileInfo = _scriptProjectProvider.CreateProjectForScriptFile(tempFile);

            // Assert
            Assert.IsNotNull(projectFileInfo);
            Assert.IsTrue(File.Exists(projectFileInfo.ProjectFilePath));
            var content = File.ReadAllText(projectFileInfo.ProjectFilePath);
            Assert.IsTrue(content.Contains("<PackageReference Include=\"Newtonsoft.Json\" Version=\"12.0.3\" />"));
        }
        finally 
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);

            var projectFilePath = Path.Combine(Path.GetDirectoryName(tempFile), "script.csproj");
            if (File.Exists(projectFilePath))
                File.Delete(projectFilePath);
        }
    }

    [Test]
    public void GetPathToProjectFile_GeneratesCorrectPath()
    {
        // Arrange
        string targetDirectory = Path.GetTempPath();
        string targetFramework = "net5.0";

        // Act
        var path = ScriptProjectProvider.GetPathToProjectFile(targetDirectory, targetFramework);

        // Assert
        Assert.IsTrue(path.Contains(targetDirectory));
        Assert.IsTrue(path.Contains(targetFramework));
        Assert.IsTrue(path.EndsWith("script.csproj"));
    }

    [Test]
    public void GetPathToProjectFile_WithCustomProjectName_UsesCustomName()
    {
        // Arrange
        string targetDirectory = Path.GetTempPath();
        string targetFramework = "net5.0";
        string projectName = "customproject";

        // Act
        var path = ScriptProjectProvider.GetPathToProjectFile(targetDirectory, targetFramework, projectName);

        // Assert
        Assert.IsTrue(path.Contains(targetDirectory));
        Assert.IsTrue(path.Contains(targetFramework));
        Assert.IsTrue(path.EndsWith($"{projectName}.csproj"));
    }
}