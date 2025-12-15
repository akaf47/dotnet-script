```csharp
using System;
using System.IO;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Environment;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet.Script.Tests
{
    [Collection("IntegrationTests")]
    public class CachedRestorerTests
    {
        private static readonly string[] NoPackageSources = Array.Empty<string>();

        public CachedRestorerTests(ITestOutputHelper testOutputHelper) => testOutputHelper.Capture();

        [Fact]
        public void ShouldUseCacheWhenAllPackagesArePinned()
        {
            // Arrange
            var restorerMock = new Mock<IRestorer>();
            var cachedRestorer = new CachedRestorer(restorerMock.Object, TestOutputHelper.CreateTestLogFactory());
            using var projectFolder = new DisposableFolder();
            var pathToProjectFile = Path.Combine(projectFolder.Path, "script.csproj");
            var pathToCachedProjectFile = Path.Combine(projectFolder.Path, $"script.csproj.cache");
            var projectFile = new ProjectFile();
            projectFile.PackageReferences.Add(new PackageReference("SomePackage", "1.2.3"));
            projectFile.packageReferences.Add(new PackageReference("AnotherPackage", "3.2.1"));
            projectFile.Save(pathToProjectFile);
            var projectFileInfo = new ProjectFileInfo(pathToProjectFile, string.Empty);

            // Act
            cachedRestorer.Restore(projectFileInfo, NoPackageSources);

            // Assert
            restorerMock.Verify(m => m.Restore(projectFileInfo, NoPackageSources), Times.Once);
            Assert.Contains(pathToCachedProjectFile, Directory.GetFiles(projectFolder.Path));
            restorerMock.Reset();

            cachedRestorer.Restore(projectFileInfo, NoPackageSources);
            restorerMock.Verify(m => m.Restore(projectFileInfo, NoPackageSources), Times.Never);
            Assert.Contains(pathToCachedProjectFile, Directory.GetFiles(projectFolder.Path));
        }
        
        [Fact]
        public void ShouldNotUseCacheWhenPackagesAreNotPinned()
        {
            // Arrange
            var restorerMock = new Mock<IRestorer>();
            var cachedRestorer = new CachedRestorer(restorerMock.Object, TestOutputHelper.CreateTestLogFactory());
            using var projectFolder = new DisposableFolder();
            var projectFile = new ProjectFile();
            var pathToProjectFile = Path.Combine(projectFolder.Path, "script.csproj");
            var pathToCachedProjectFile = Path.Combine(projectFolder.Path, $"script.csproj.cache");
            projectFile.PackageReferences.Add(new PackageReference("SomePackage", "1.2.3"));
            projectFile.PackageReferences.Add(new PackageReference("AnotherPackage", "3.2"));
            projectFile.Save(pathToProjectFile);
            var projectFileInfo = new ProjectFileInfo(pathToProjectFile, string.Empty);

            // Act
            cachedRestorer.Restore(projectFileInfo, NoPackageSources);

            // Assert
            restorerMock.Verify(m => m.Restore(projectFileInfo, NoPackageSources), Times.Once);
            Assert.DoesNotContain(pathToCachedProjectFile, Directory.GetFiles(projectFolder.Path));
            restorerMock.Reset();

            cachedRestorer.Restore(projectFileInfo, NoPackageSources);
            restorerMock.Verify(m => m.Restore(projectFileInfo, NoPackageSources), Times.Once);
            Assert.DoesNotContain(pathToCachedProjectFile, Directory.GetFiles(projectFolder.Path));
        }

        [Fact]
        public void ShouldNotCacheWhenProjectFilesAreNotEqual()
        {
            // Arrange
            var restorerMock = new Mock<IRestorer>();
            var cachedRestorer = new CachedRestorer(restorerMock.Object, TestOutputHelper.CreateTestLogFactory());
            using var projectFolder = new DisposableFolder();
            var projectFile = new ProjectFile();
            var pathToProjectFile = Path.Combine(projectFolder.Path, "script.csproj");
            var pathToCachedProjectFile = Path.Combine(projectFolder.Path, $"script.csproj.cache");
            projectFile.PackageReferences.Add(new PackageReference("SomePackage", "1.2.3"));
            projectFile.PackageReferences.Add(new PackageReference("AnotherPackage", "1.2.3"));
            projectFile.Save(pathToProjectFile);
            var projectFileInfo = new ProjectFileInfo(pathToProjectFile, string.Empty);

            // Act
            cachedRestorer.Restore(projectFileInfo, NoPackageSources);

            // Assert
            restorerMock.Verify(m => m.Restore(projectFileInfo, NoPackageSources), Times.Once);
            Assert.Contains(pathToCachedProjectFile, Directory.GetFiles(projectFolder.Path));
            restorerMock.Reset();

            projectFile.PackageReferences.Add(new PackageReference("YetAnotherPackage", "1.2.3"));
            projectFile.Save(pathToProjectFile);
            cachedRestorer.Restore(projectFileInfo, NoPackageSources);

            restorerMock.Verify(m => m.Restore(projectFileInfo, NoPackageSources), Times.Once);
            Assert.Contains(pathToCachedProjectFile, Directory.GetFiles(projectFolder.Path));
        }
    }
}
```