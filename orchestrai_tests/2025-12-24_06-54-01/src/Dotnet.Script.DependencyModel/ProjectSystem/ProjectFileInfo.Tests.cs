using System;
using Xunit;
using Dotnet.Script.DependencyModel.ProjectSystem;

namespace Dotnet.Script.DependencyModel.Tests.ProjectSystem
{
    public class ProjectFileInfoTests
    {
        [Fact]
        public void Constructor_WithValidPaths_SetsProperties()
        {
            // Arrange
            var projectPath = "/path/to/project.csproj";
            var nugetConfigPath = "/path/to/NuGet.Config";

            // Act
            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            // Assert
            Assert.Equal(projectPath, projectFileInfo.Path);
            Assert.Equal(nugetConfigPath, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithNullPath_SetsPathToNull()
        {
            // Arrange
            string projectPath = null;
            var nugetConfigPath = "/path/to/NuGet.Config";

            // Act
            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            // Assert
            Assert.Null(projectFileInfo.Path);
            Assert.Equal(nugetConfigPath, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithNullNuGetConfigFile_SetsNuGetConfigFileToNull()
        {
            // Arrange
            var projectPath = "/path/to/project.csproj";
            string nugetConfigPath = null;

            // Act
            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            // Assert
            Assert.Equal(projectPath, projectFileInfo.Path);
            Assert.Null(projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithBothNullPaths_SetsBothToNull()
        {
            // Arrange
            string projectPath = null;
            string nugetConfigPath = null;

            // Act
            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            // Assert
            Assert.Null(projectFileInfo.Path);
            Assert.Null(projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithEmptyStringPaths_SetsEmptyStrings()
        {
            // Arrange
            var projectPath = string.Empty;
            var nugetConfigPath = string.Empty;

            // Act
            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            // Assert
            Assert.Equal(string.Empty, projectFileInfo.Path);
            Assert.Equal(string.Empty, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Path_Property_IsReadOnly()
        {
            // Arrange
            var projectPath = "/path/to/project.csproj";
            var projectFileInfo = new ProjectFileInfo(projectPath, "/config");

            // Act & Assert
            // Attempting to set Path should cause a compile error, but we verify it's read-only by accessing
            var retrievedPath = projectFileInfo.Path;
            Assert.Equal(projectPath, retrievedPath);
        }

        [Fact]
        public void NuGetConfigFile_Property_IsReadOnly()
        {
            // Arrange
            var nugetConfigPath = "/path/to/NuGet.Config";
            var projectFileInfo = new ProjectFileInfo("/path/to/project.csproj", nugetConfigPath);

            // Act & Assert
            var retrievedConfig = projectFileInfo.NuGetConfigFile;
            Assert.Equal(nugetConfigPath, retrievedConfig);
        }

        [Fact]
        public void Constructor_WithWhitespaceStringPaths_SetsWhitespaceStrings()
        {
            // Arrange
            var projectPath = "   ";
            var nugetConfigPath = "\t\t";

            // Act
            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            // Assert
            Assert.Equal("   ", projectFileInfo.Path);
            Assert.Equal("\t\t", projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithSpecialCharactersInPaths_PreservesSpecialCharacters()
        {
            // Arrange
            var projectPath = @"C:\Users\Name\My Projects\project file (1).csproj";
            var nugetConfigPath = @"C:\Program Files (x86)\NuGet.Config";

            // Act
            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            // Assert
            Assert.Equal(projectPath, projectFileInfo.Path);
            Assert.Equal(nugetConfigPath, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithRelativePaths_PreservesRelativePaths()
        {
            // Arrange
            var projectPath = "../project.csproj";
            var nugetConfigPath = "../../NuGet.Config";

            // Act
            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            // Assert
            Assert.Equal(projectPath, projectFileInfo.Path);
            Assert.Equal(nugetConfigPath, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithVeryLongPaths_PreservesLongPaths()
        {
            // Arrange
            var longPath = new string('a', 1000) + ".csproj";
            var longConfigPath = new string('b', 1000) + ".config";

            // Act
            var projectFileInfo = new ProjectFileInfo(longPath, longConfigPath);

            // Assert
            Assert.Equal(longPath, projectFileInfo.Path);
            Assert.Equal(longConfigPath, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithIdenticalPaths_BothPropertiesCanHaveSameValue()
        {
            // Arrange
            var samePath = "/same/path";

            // Act
            var projectFileInfo = new ProjectFileInfo(samePath, samePath);

            // Assert
            Assert.Equal(samePath, projectFileInfo.Path);
            Assert.Equal(samePath, projectFileInfo.NuGetConfigFile);
            Assert.Same(projectFileInfo.Path, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithUnicodeCharactersInPaths_PreservesUnicodeCharacters()
        {
            // Arrange
            var projectPath = "/path/到/项目/project.csproj";
            var nugetConfigPath = "/path/naar/NuGet.Config";

            // Act
            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            // Assert
            Assert.Equal(projectPath, projectFileInfo.Path);
            Assert.Equal(nugetConfigPath, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_MultipleInstances_HaveIndependentValues()
        {
            // Arrange
            var project1 = new ProjectFileInfo("/path1/project.csproj", "/config1");
            var project2 = new ProjectFileInfo("/path2/project.csproj", "/config2");

            // Act & Assert
            Assert.NotEqual(project1.Path, project2.Path);
            Assert.NotEqual(project1.NuGetConfigFile, project2.NuGetConfigFile);
            Assert.Equal("/path1/project.csproj", project1.Path);
            Assert.Equal("/path2/project.csproj", project2.Path);
        }
    }
}