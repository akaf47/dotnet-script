using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Dotnet.Script.DependencyModel.Environment;

[TestFixture]
public class ProjectFileTests
{
    [Test]
    public void Constructor_WithEmptyXml_CreatesEmptyProjectFile()
    {
        // Arrange
        var xmlContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
            <ItemGroup></ItemGroup>
        </Project>";

        // Act
        var projectFile = new ProjectFile(xmlContent);

        // Assert
        Assert.AreEqual(0, projectFile.PackageReferences.Count);
        Assert.AreEqual(0, projectFile.AssemblyReferences.Count);
        Assert.AreEqual("Microsoft.NET.Sdk", projectFile.Sdk);
        Assert.AreEqual(ScriptEnvironment.Default.TargetFramework, projectFile.TargetFramework);
    }

    [Test]
    public void Constructor_WithPackageReferences_PopulatesPackageReferences()
    {
        // Arrange
        var xmlContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
            <ItemGroup>
                <PackageReference Include=""Newtonsoft.Json"" Version=""12.0.3"" />
                <PackageReference Include=""System.Text.Json"" Version=""5.0.0"" />
            </ItemGroup>
        </Project>";

        // Act
        var projectFile = new ProjectFile(xmlContent);

        // Assert
        Assert.AreEqual(2, projectFile.PackageReferences.Count);
        Assert.IsTrue(projectFile.PackageReferences.Contains(new PackageReference("Newtonsoft.Json", "12.0.3")));
        Assert.IsTrue(projectFile.PackageReferences.Contains(new PackageReference("System.Text.Json", "5.0.0")));
    }

    [Test]
    public void Save_CreatesProjectFileWithCorrectContent()
    {
        // Arrange
        var projectFile = new ProjectFile();
        projectFile.PackageReferences.Add(new PackageReference("Newtonsoft.Json", "12.0.3"));
        projectFile.AssemblyReferences.Add(new AssemblyReference("SomeAssembly.dll"));
        var tempFile = Path.GetTempFileName() + ".csproj";

        try 
        {
            // Act
            projectFile.Save(tempFile);

            // Assert
            var savedContent = File.ReadAllText(tempFile);
            Assert.IsTrue(savedContent.Contains("<PackageReference Include=\"Newtonsoft.Json\" Version=\"12.0.3\" />"));
            Assert.IsTrue(savedContent.Contains("<Reference Include=\"SomeAssembly.dll\" />"));
        }
        finally 
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public void Equals_WithSameContent_ReturnsTrue()
    {
        // Arrange
        var projectFile1 = new ProjectFile();
        projectFile1.PackageReferences.Add(new PackageReference("Newtonsoft.Json", "12.0.3"));
        
        var projectFile2 = new ProjectFile();
        projectFile2.PackageReferences.Add(new PackageReference("Newtonsoft.Json", "12.0.3"));

        // Act & Assert
        Assert.AreEqual(projectFile1, projectFile2);
        Assert.IsTrue(projectFile1.Equals(projectFile2));
    }

    [Test]
    public void Equals_WithDifferentContent_ReturnsFalse()
    {
        // Arrange
        var projectFile1 = new ProjectFile();
        projectFile1.PackageReferences.Add(new PackageReference("Newtonsoft.Json", "12.0.3"));
        
        var projectFile2 = new ProjectFile();
        projectFile2.PackageReferences.Add(new PackageReference("System.Text.Json", "5.0.0"));

        // Act & Assert
        Assert.AreNotEqual(projectFile1, projectFile2);
        Assert.IsFalse(projectFile1.Equals(projectFile2));
    }

    [Test]
    public void IsCacheable_WithPinnedVersions_ReturnsTrue()
    {
        // Arrange
        var projectFile = new ProjectFile();
        projectFile.PackageReferences.Add(new PackageReference("Newtonsoft.Json", "12.0.3"));

        // Act & Assert
        Assert.IsTrue(projectFile.IsCacheable);
    }
}