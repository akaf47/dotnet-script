using System;
using Xunit;
using Dotnet.Script.DependencyModel.ProjectSystem;

namespace Dotnet.Script.DependencyModel.Tests.ProjectSystem
{
    public class ProjectFileInfoTests
    {
        [Fact]
        public void Constructor_WithValidPaths_StoresPathValues()
        {
            var projectPath = "/path/to/project.csproj";
            var nugetConfigPath = "/path/to/NuGet.Config";

            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            Assert.Equal(projectPath, projectFileInfo.Path);
            Assert.Equal(nugetConfigPath, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithEmptyStrings_StoresEmptyValues()
        {
            var projectPath = "";
            var nugetConfigPath = "";

            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            Assert.Equal("", projectFileInfo.Path);
            Assert.Equal("", projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithNullPath_StoresNullPath()
        {
            var projectFileInfo = new ProjectFileInfo(null, "config.path");

            Assert.Null(projectFileInfo.Path);
            Assert.Equal("config.path", projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithNullNuGetConfigFile_StoresNullConfigFile()
        {
            var projectFileInfo = new ProjectFileInfo("project.path", null);

            Assert.Equal("project.path", projectFileInfo.Path);
            Assert.Null(projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithBothNullValues_StoresBothNull()
        {
            var projectFileInfo = new ProjectFileInfo(null, null);

            Assert.Null(projectFileInfo.Path);
            Assert.Null(projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Path_Property_IsReadOnly()
        {
            var projectFileInfo = new ProjectFileInfo("path1", "config1");

            Assert.Equal("path1", projectFileInfo.Path);

            var property = typeof(ProjectFileInfo).GetProperty("Path");
            Assert.NotNull(property);
            Assert.False(property.CanWrite);
        }

        [Fact]
        public void NuGetConfigFile_Property_IsReadOnly()
        {
            var projectFileInfo = new ProjectFileInfo("path1", "config1");

            Assert.Equal("config1", projectFileInfo.NuGetConfigFile);

            var property = typeof(ProjectFileInfo).GetProperty("NuGetConfigFile");
            Assert.NotNull(property);
            Assert.False(property.CanWrite);
        }

        [Fact]
        public void Constructor_WithWhitespaceStrings_StoresWhitespaceValues()
        {
            var projectPath = "   ";
            var nugetConfigPath = "\t\n";

            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            Assert.Equal("   ", projectFileInfo.Path);
            Assert.Equal("\t\n", projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithComplexPaths_StoresComplexValues()
        {
            var projectPath = "C:\\Users\\Admin\\Documents\\Project\\bin\\Debug\\net6.0\\MyProject.csproj";
            var nugetConfigPath = "C:\\Users\\Admin\\AppData\\Roaming\\NuGet\\NuGet.Config";

            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            Assert.Equal(projectPath, projectFileInfo.Path);
            Assert.Equal(nugetConfigPath, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithUriPaths_StoresUriValues()
        {
            var projectPath = "https://example.com/project.csproj";
            var nugetConfigPath = "https://example.com/NuGet.Config";

            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            Assert.Equal(projectPath, projectFileInfo.Path);
            Assert.Equal(nugetConfigPath, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithSpecialCharacterPaths_StoresSpecialCharacterValues()
        {
            var projectPath = "/path/to/project-file_v2.1.0.csproj";
            var nugetConfigPath = "/path/to/NuGet@Config#1.xml";

            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            Assert.Equal(projectPath, projectFileInfo.Path);
            Assert.Equal(nugetConfigPath, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_WithRelativePaths_StoresRelativeValues()
        {
            var projectPath = "../project.csproj";
            var nugetConfigPath = "../../NuGet.Config";

            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            Assert.Equal(projectPath, projectFileInfo.Path);
            Assert.Equal(nugetConfigPath, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_MultipleInstances_HasIndependentState()
        {
            var info1 = new ProjectFileInfo("path1", "config1");
            var info2 = new ProjectFileInfo("path2", "config2");

            Assert.Equal("path1", info1.Path);
            Assert.Equal("config1", info1.NuGetConfigFile);
            Assert.Equal("path2", info2.Path);
            Assert.Equal("config2", info2.NuGetConfigFile);
        }

        [Fact]
        public void Path_Property_CanBeReadMultipleTimes()
        {
            var projectPath = "/path/to/project.csproj";
            var projectFileInfo = new ProjectFileInfo(projectPath, "config");

            var path1 = projectFileInfo.Path;
            var path2 = projectFileInfo.Path;
            var path3 = projectFileInfo.Path;

            Assert.Equal(path1, path2);
            Assert.Equal(path2, path3);
        }

        [Fact]
        public void NuGetConfigFile_Property_CanBeReadMultipleTimes()
        {
            var nugetConfigPath = "/path/to/NuGet.Config";
            var projectFileInfo = new ProjectFileInfo("path", nugetConfigPath);

            var config1 = projectFileInfo.NuGetConfigFile;
            var config2 = projectFileInfo.NuGetConfigFile;
            var config3 = projectFileInfo.NuGetConfigFile;

            Assert.Equal(config1, config2);
            Assert.Equal(config2, config3);
        }

        [Fact]
        public void Constructor_WithVeryLongPaths_StoresLongValues()
        {
            var longPath = new string('a', 1000);
            var longConfig = new string('b', 1000);

            var projectFileInfo = new ProjectFileInfo(longPath, longConfig);

            Assert.Equal(longPath, projectFileInfo.Path);
            Assert.Equal(longConfig, projectFileInfo.NuGetConfigFile);
            Assert.Equal(1000, projectFileInfo.Path.Length);
            Assert.Equal(1000, projectFileInfo.NuGetConfigFile.Length);
        }

        [Fact]
        public void Constructor_WithSamePathForBoth_StoresIdenticalValues()
        {
            var samePath = "/same/path";

            var projectFileInfo = new ProjectFileInfo(samePath, samePath);

            Assert.Equal(samePath, projectFileInfo.Path);
            Assert.Equal(samePath, projectFileInfo.NuGetConfigFile);
            Assert.Same(projectFileInfo.Path, projectFileInfo.NuGetConfigFile);
        }

        [Fact]
        public void Constructor_Properties_AreInitializedInConstructor()
        {
            var projectPath = "project.csproj";
            var nugetConfigPath = "nuget.config";

            var projectFileInfo = new ProjectFileInfo(projectPath, nugetConfigPath);

            Assert.NotNull(projectFileInfo);
            var properties = typeof(ProjectFileInfo).GetProperties();
            Assert.Equal(2, properties.Length);
        }
    }
}