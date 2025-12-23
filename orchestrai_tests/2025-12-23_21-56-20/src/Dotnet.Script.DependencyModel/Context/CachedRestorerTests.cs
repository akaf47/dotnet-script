using System;
using System.IO;
using Xunit;
using Moq;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Environment;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.ProjectSystem;

namespace Dotnet.Script.DependencyModel.Tests.Context
{
    public class CachedRestorerTests : IDisposable
    {
        private readonly Mock<IRestorer> _mockRestorer;
        private readonly Mock<LogFactory> _mockLogFactory;
        private readonly Mock<Logger> _mockLogger;
        private readonly CachedRestorer _sut;
        private readonly string _testDirectory;

        public CachedRestorerTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            _mockRestorer = new Mock<IRestorer>();
            _mockLogFactory = new Mock<LogFactory>();
            _mockLogger = new Mock<Logger>();

            // Setup log factory to return mock logger
            _mockLogFactory.Setup(f => f(It.IsAny<Type>())).Returns(_mockLogger.Object);

            _sut = new CachedRestorer(_mockRestorer.Object, _mockLogFactory.Object);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_testDirectory))
                {
                    Directory.Delete(_testDirectory, true);
                }
            }
            catch { }
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var restorer = new CachedRestorer(_mockRestorer.Object, _mockLogFactory.Object);

            // Assert
            Assert.NotNull(restorer);
        }

        [Fact]
        public void Constructor_WithNullRestorer_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CachedRestorer(null, _mockLogFactory.Object));
        }

        [Fact]
        public void Constructor_WithNullLogFactory_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CachedRestorer(_mockRestorer.Object, null));
        }

        #endregion

        #region CanRestore Property Tests

        [Fact]
        public void CanRestore_ShouldReturnTrueWhenUnderlyingRestorerCanRestore()
        {
            // Arrange
            _mockRestorer.Setup(r => r.CanRestore).Returns(true);

            // Act
            var result = _sut.CanRestore;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanRestore_ShouldReturnFalseWhenUnderlyingRestorerCannotRestore()
        {
            // Arrange
            _mockRestorer.Setup(r => r.CanRestore).Returns(false);

            // Act
            var result = _sut.CanRestore;

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Restore Method Tests - Cache Hit Scenarios

        [Fact]
        public void Restore_WhenCacheFileExistsAndProjectFilesAreIdentical_ShouldSkipRestore()
        {
            // Arrange
            var projectFilePath = Path.Combine(_testDirectory, "project.csproj");
            var cacheFilePath = $"{projectFilePath}.cache";
            var projectContent = CreateValidProjectFileContent();
            
            File.WriteAllText(projectFilePath, projectContent);
            File.WriteAllText(cacheFilePath, projectContent);

            var projectFileInfo = new ProjectFileInfo(projectFilePath, null);
            var packageSources = new string[] { };

            // Act
            _sut.Restore(projectFileInfo, packageSources);

            // Assert
            _mockRestorer.Verify(r => r.Restore(It.IsAny<ProjectFileInfo>(), It.IsAny<string[]>()), Times.Never);
            _mockLogger.Verify(l => l(LogLevel.Debug, It.Is<string>(s => s.Contains("Skipping restore")), null), Times.Once);
        }

        #endregion

        #region Restore Method Tests - Cache Miss Scenarios

        [Fact]
        public void Restore_WhenCacheFileDoesNotExist_ShouldRestoreAndCache()
        {
            // Arrange
            var projectFilePath = Path.Combine(_testDirectory, "project.csproj");
            var cacheFilePath = $"{projectFilePath}.cache";
            var projectContent = CreateValidProjectFileContent();
            
            File.WriteAllText(projectFilePath, projectContent);

            var projectFileInfo = new ProjectFileInfo(projectFilePath, null);
            var packageSources = new string[] { "https://api.nuget.org/v3/index.json" };

            // Setup restorer to succeed
            _mockRestorer.Setup(r => r.Restore(It.IsAny<ProjectFileInfo>(), It.IsAny<string[]>()));

            // Act
            _sut.Restore(projectFileInfo, packageSources);

            // Assert
            _mockRestorer.Verify(r => r.Restore(projectFileInfo, packageSources), Times.Once);
            Assert.True(File.Exists(cacheFilePath), "Cache file should be created after successful restore");
            _mockLogger.Verify(l => l(LogLevel.Debug, It.Is<string>(s => s.Contains("Caching project file")), null), Times.Once);
        }

        [Fact]
        public void Restore_WhenCacheFileExistsButProjectFilesAreDifferent_ShouldDeleteCacheAndRestore()
        {
            // Arrange
            var projectFilePath = Path.Combine(_testDirectory, "project.csproj");
            var cacheFilePath = $"{projectFilePath}.cache";
            var oldProjectContent = CreateValidProjectFileContent("OldSdk");
            var newProjectContent = CreateValidProjectFileContent("NewSdk");
            
            File.WriteAllText(projectFilePath, newProjectContent);
            File.WriteAllText(cacheFilePath, oldProjectContent);

            var projectFileInfo = new ProjectFileInfo(projectFilePath, null);
            var packageSources = new string[] { };

            _mockRestorer.Setup(r => r.Restore(It.IsAny<ProjectFileInfo>(), It.IsAny<string[]>()));

            // Act
            _sut.Restore(projectFileInfo, packageSources);

            // Assert
            _mockRestorer.Verify(r => r.Restore(projectFileInfo, packageSources), Times.Once);
            _mockLogger.Verify(l => l(LogLevel.Debug, It.Is<string>(s => s.Contains("Cache miss")), null), Times.Once);
            Assert.True(File.Exists(cacheFilePath), "Cache file should be recreated after restore");
        }

        #endregion

        #region Restore Method Tests - Non-Cacheable Projects

        [Fact]
        public void Restore_WhenProjectIsNotCacheable_ShouldRestoreButNotCache()
        {
            // Arrange
            var projectFilePath = Path.Combine(_testDirectory, "project.csproj");
            var cacheFilePath = $"{projectFilePath}.cache";
            var projectContent = CreateValidProjectFileContentWithUnpinnedVersion();
            
            File.WriteAllText(projectFilePath, projectContent);

            var projectFileInfo = new ProjectFileInfo(projectFilePath, null);
            var packageSources = new string[] { };

            _mockRestorer.Setup(r => r.Restore(It.IsAny<ProjectFileInfo>(), It.IsAny<string[]>()));

            // Act
            _sut.Restore(projectFileInfo, packageSources);

            // Assert
            _mockRestorer.Verify(r => r.Restore(projectFileInfo, packageSources), Times.Once);
            Assert.False(File.Exists(cacheFilePath), "Cache file should not be created for non-cacheable projects");
            _mockLogger.Verify(l => l(LogLevel.Warning, It.Is<string>(s => s.Contains("Unable to cache")), null), Times.Once);
        }

        #endregion

        #region Restore Method Tests - Multiple PackageSources

        [Fact]
        public void Restore_WithMultiplePackageSources_ShouldRestoreWithAllSources()
        {
            // Arrange
            var projectFilePath = Path.Combine(_testDirectory, "project.csproj");
            var projectContent = CreateValidProjectFileContent();
            
            File.WriteAllText(projectFilePath, projectContent);

            var projectFileInfo = new ProjectFileInfo(projectFilePath, null);
            var packageSources = new string[] 
            { 
                "https://api.nuget.org/v3/index.json",
                "https://custom.nuget.org/v3/index.json"
            };

            _mockRestorer.Setup(r => r.Restore(It.IsAny<ProjectFileInfo>(), It.IsAny<string[]>()));

            // Act
            _sut.Restore(projectFileInfo, packageSources);

            // Assert
            _mockRestorer.Verify(r => r.Restore(projectFileInfo, packageSources), Times.Once);
        }

        #endregion

        #region Restore Method Tests - With NuGet Config File

        [Fact]
        public void Restore_WithNuGetConfigFile_ShouldRestoreWithConfig()
        {
            // Arrange
            var projectFilePath = Path.Combine(_testDirectory, "project.csproj");
            var configFilePath = Path.Combine(_testDirectory, "nuget.config");
            var projectContent = CreateValidProjectFileContent();
            
            File.WriteAllText(projectFilePath, projectContent);
            File.WriteAllText(configFilePath, "<configuration/>");

            var projectFileInfo = new ProjectFileInfo(projectFilePath, configFilePath);
            var packageSources = new string[] { };

            _mockRestorer.Setup(r => r.Restore(It.IsAny<ProjectFileInfo>(), It.IsAny<string[]>()));

            // Act
            _sut.Restore(projectFileInfo, packageSources);

            // Assert
            _mockRestorer.Verify(r => r.Restore(projectFileInfo, packageSources), Times.Once);
        }

        #endregion

        #region Restore Method Tests - Edge Cases

        [Fact]
        public void Restore_WithEmptyPackageSources_ShouldRestoreWithoutSources()
        {
            // Arrange
            var projectFilePath = Path.Combine(_testDirectory, "project.csproj");
            var projectContent = CreateValidProjectFileContent();
            
            File.WriteAllText(projectFilePath, projectContent);

            var projectFileInfo = new ProjectFileInfo(projectFilePath, null);
            var packageSources = new string[] { };

            _mockRestorer.Setup(r => r.Restore(It.IsAny<ProjectFileInfo>(), It.IsAny<string[]>()));

            // Act
            _sut.Restore(projectFileInfo, packageSources);

            // Assert
            _mockRestorer.Verify(r => r.Restore(projectFileInfo, packageSources), Times.Once);
        }

        [Fact]
        public void Restore_WithNullPackageSources_ShouldThrowArgumentNullException()
        {
            // Arrange
            var projectFilePath = Path.Combine(_testDirectory, "project.csproj");
            var projectContent = CreateValidProjectFileContent();
            
            File.WriteAllText(projectFilePath, projectContent);

            var projectFileInfo = new ProjectFileInfo(projectFilePath, null);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _sut.Restore(projectFileInfo, null));
        }

        [Fact]
        public void Restore_WithNullProjectFileInfo_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _sut.Restore(null, new string[] { }));
        }

        #endregion

        #region Restore Method Tests - Restore Failures

        [Fact]
        public void Restore_WhenRestoreFails_ShouldPropagateException()
        {
            // Arrange
            var projectFilePath = Path.Combine(_testDirectory, "project.csproj");
            var projectContent = CreateValidProjectFileContent();
            
            File.WriteAllText(projectFilePath, projectContent);

            var projectFileInfo = new ProjectFileInfo(projectFilePath, null);
            var packageSources = new string[] { };

            var expectedException = new Exception("Restore failed");
            _mockRestorer.Setup(r => r.Restore(It.IsAny<ProjectFileInfo>(), It.IsAny<string[]>()))
                .Throws(expectedException);

            // Act & Assert
            var actualException = Assert.Throws<Exception>(() => _sut.Restore(projectFileInfo, packageSources));
            Assert.Equal(expectedException.Message, actualException.Message);
        }

        #endregion

        #region Restore Method Tests - Local Nested Directories

        [Fact]
        public void Restore_WithProjectFileInNestedDirectory_ShouldCreateCacheInSameDirectory()
        {
            // Arrange
            var nestedDirectory = Path.Combine(_testDirectory, "nested", "deeply", "folder");
            Directory.CreateDirectory(nestedDirectory);
            
            var projectFilePath = Path.Combine(nestedDirectory, "project.csproj");
            var cacheFilePath = $"{projectFilePath}.cache";
            var projectContent = CreateValidProjectFileContent();
            
            File.WriteAllText(projectFilePath, projectContent);

            var projectFileInfo = new ProjectFileInfo(projectFilePath, null);
            var packageSources = new string[] { };

            _mockRestorer.Setup(r => r.Restore(It.IsAny<ProjectFileInfo>(), It.IsAny<string[]>()));

            // Act
            _sut.Restore(projectFileInfo, packageSources);

            // Assert
            Assert.True(File.Exists(cacheFilePath));
        }

        #endregion

        #region Helper Methods

        private string CreateValidProjectFileContent(string sdk = "Microsoft.NET.Sdk")
        {
            return $@"<Project Sdk=""{sdk}"">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Newtonsoft.Json"" Version=""13.0.1"" />
  </ItemGroup>
</Project>";
        }

        private string CreateValidProjectFileContentWithUnpinnedVersion()
        {
            return @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Newtonsoft.Json"" Version=""*"" />
  </ItemGroup>
</Project>";
        }

        #endregion
    }
}