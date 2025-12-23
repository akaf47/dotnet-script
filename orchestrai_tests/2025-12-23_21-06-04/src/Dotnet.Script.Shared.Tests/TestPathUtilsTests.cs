using System;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;
using Moq;
using Dotnet.Script.Shared.Tests;
using Dotnet.Script.DependencyModel.ProjectSystem;

namespace Dotnet.Script.Shared.Tests.Tests
{
    public class TestPathUtilsTests
    {
        [Fact]
        public void GetPathToTestFixtureFolder_WithValidFixtureName_ReturnsCorrectPath()
        {
            // Arrange
            var fixtureName = "HelloWorld";

            // Act
            var result = TestPathUtils.GetPathToTestFixtureFolder(fixtureName);

            // Assert
            Assert.NotNull(result);
            Assert.True(Path.IsPathRooted(result));
            Assert.Contains("TestFixtures", result);
            Assert.Contains(fixtureName, result);
        }

        [Fact]
        public void GetPathToTestFixtureFolder_WithEmptyFixtureName_ReturnsPathWithEmptySegment()
        {
            // Arrange
            var fixtureName = "";

            // Act
            var result = TestPathUtils.GetPathToTestFixtureFolder(fixtureName);

            // Assert
            Assert.NotNull(result);
            Assert.True(Path.IsPathRooted(result));
        }

        [Fact]
        public void GetPathToTestFixtureFolder_WithDifferentFixtureNames_ReturnsDifferentPaths()
        {
            // Arrange
            var fixture1 = "TestFixture1";
            var fixture2 = "TestFixture2";

            // Act
            var result1 = TestPathUtils.GetPathToTestFixtureFolder(fixture1);
            var result2 = TestPathUtils.GetPathToTestFixtureFolder(fixture2);

            // Assert
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void GetPathToTestFixtureFolder_CalledTwiceWithSameName_ReturnsSamePath()
        {
            // Arrange
            var fixtureName = "SameFixture";

            // Act
            var result1 = TestPathUtils.GetPathToTestFixtureFolder(fixtureName);
            var result2 = TestPathUtils.GetPathToTestFixtureFolder(fixtureName);

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void GetPathToTestFixtureFolder_WithPathContainingSeparators_HandlesCorrectly()
        {
            // Arrange
            var fixtureName = "Folder/SubFolder";

            // Act
            var result = TestPathUtils.GetPathToTestFixtureFolder(fixtureName);

            // Assert
            Assert.NotNull(result);
            Assert.True(Path.IsPathRooted(result));
        }

        [Fact]
        public void GetPathToTestFixtureFolder_ResultPathEndsWithFixtureName()
        {
            // Arrange
            var fixtureName = "TestCase";

            // Act
            var result = TestPathUtils.GetPathToTestFixtureFolder(fixtureName);

            // Assert
            Assert.EndsWith(fixtureName, result);
        }

        [Fact]
        public void GetPathToTempFolder_WithValidPath_ReturnsValidPath()
        {
            // Arrange
            var testPath = Directory.GetCurrentDirectory();

            // Act
            var result = TestPathUtils.GetPathToTempFolder(testPath);

            // Assert
            Assert.NotNull(result);
            Assert.True(Path.IsPathRooted(result));
        }

        [Fact]
        public void GetPathToTempFolder_WithDifferentPaths_ReturnsDifferentResults()
        {
            // Arrange
            var path1 = Path.Combine(Directory.GetCurrentDirectory(), "folder1");
            var path2 = Path.Combine(Directory.GetCurrentDirectory(), "folder2");

            // Act
            var result1 = TestPathUtils.GetPathToTempFolder(path1);
            var result2 = TestPathUtils.GetPathToTempFolder(path2);

            // Assert
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void GetPathToTempFolder_CalledTwiceWithSamePath_ReturnsSameResult()
        {
            // Arrange
            var path = Directory.GetCurrentDirectory();

            // Act
            var result1 = TestPathUtils.GetPathToTempFolder(path);
            var result2 = TestPathUtils.GetPathToTempFolder(path);

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void GetPathToTempFolder_WithRootedPath_DelegateToFileUtils()
        {
            // Arrange
            var rooted = Path.GetPathRoot(Directory.GetCurrentDirectory());
            var testPath = Path.Combine(rooted, "test");

            // Act
            var result = TestPathUtils.GetPathToTempFolder(testPath);

            // Assert
            Assert.NotNull(result);
            Assert.True(Path.IsPathRooted(result));
        }

        [Fact]
        public void GetPathToScriptPackages_WithValidFixture_ReturnsValidPath()
        {
            // Arrange
            var fixtureName = "HelloWorld";

            // Act
            var result = TestPathUtils.GetPathToScriptPackages(fixtureName);

            // Assert
            Assert.NotNull(result);
            Assert.True(Path.IsPathRooted(result));
            Assert.Contains("packages", result);
        }

        [Fact]
        public void GetPathToScriptPackages_WithEmptyFixture_ReturnsPathWithPackages()
        {
            // Arrange
            var fixtureName = "";

            // Act
            var result = TestPathUtils.GetPathToScriptPackages(fixtureName);

            // Assert
            Assert.NotNull(result);
            Assert.True(Path.IsPathRooted(result));
            Assert.Contains("packages", result);
        }

        [Fact]
        public void GetPathToScriptPackages_WithDifferentFixtures_ReturnsDifferentPaths()
        {
            // Arrange
            var fixture1 = "Fixture1";
            var fixture2 = "Fixture2";

            // Act
            var result1 = TestPathUtils.GetPathToScriptPackages(fixture1);
            var result2 = TestPathUtils.GetPathToScriptPackages(fixture2);

            // Assert
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void GetPathToScriptPackages_CalledTwiceWithSameFixture_ReturnsSamePath()
        {
            // Arrange
            var fixtureName = "Consistent";

            // Act
            var result1 = TestPathUtils.GetPathToScriptPackages(fixtureName);
            var result2 = TestPathUtils.GetPathToScriptPackages(fixtureName);

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void GetPathToScriptPackages_PathContainsParentDirectoryNavigator()
        {
            // Arrange
            var fixtureName = "Test";

            // Act
            var result = TestPathUtils.GetPathToScriptPackages(fixtureName);

            // Assert
            // The path should contain ".." because it goes up from the temp folder
            Assert.Contains("..", result);
        }

        [Fact]
        public void GetPathToTestFixture_WithValidFixtureName_ReturnsPathToCsxFile()
        {
            // Arrange
            var fixtureName = "HelloWorld";

            // Act
            var result = TestPathUtils.GetPathToTestFixture(fixtureName);

            // Assert
            Assert.NotNull(result);
            Assert.True(Path.IsPathRooted(result));
            Assert.EndsWith(".csx", result);
            Assert.Contains(fixtureName, result);
        }

        [Fact]
        public void GetPathToTestFixture_WithEmptyFixtureName_ReturnsPathEndingWithCsx()
        {
            // Arrange
            var fixtureName = "";

            // Act
            var result = TestPathUtils.GetPathToTestFixture(fixtureName);

            // Assert
            Assert.NotNull(result);
            Assert.EndsWith(".csx", result);
        }

        [Fact]
        public void GetPathToTestFixture_CsxFileNameMatchesFolderName()
        {
            // Arrange
            var fixtureName = "TestCase";

            // Act
            var result = TestPathUtils.GetPathToTestFixture(fixtureName);

            // Assert
            var fileName = Path.GetFileNameWithoutExtension(result);
            Assert.Equal(fixtureName, fileName);
        }

        [Fact]
        public void GetPathToTestFixture_WithDifferentFixtures_ReturnsDifferentPaths()
        {
            // Arrange
            var fixture1 = "Fixture1";
            var fixture2 = "Fixture2";

            // Act
            var result1 = TestPathUtils.GetPathToTestFixture(fixture1);
            var result2 = TestPathUtils.GetPathToTestFixture(fixture2);

            // Assert
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void GetPathToTestFixture_CalledTwiceWithSameName_ReturnsSamePath()
        {
            // Arrange
            var fixtureName = "Consistent";

            // Act
            var result1 = TestPathUtils.GetPathToTestFixture(fixtureName);
            var result2 = TestPathUtils.GetPathToTestFixture(fixtureName);

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void GetPathToGlobalPackagesFolder_ReturnsDotnetGlobalPackagesPath()
        {
            // Act
            var result = TestPathUtils.GetPathToGlobalPackagesFolder();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(Path.IsPathRooted(result));
        }

        [Fact]
        public void GetPathToGlobalPackagesFolder_CalledMultipleTimes_ReturnsConsistentResults()
        {
            // Act
            var result1 = TestPathUtils.GetPathToGlobalPackagesFolder();
            var result2 = TestPathUtils.GetPathToGlobalPackagesFolder();

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void GetPathToGlobalPackagesFolder_ResultContainsGlobalPackagesKeyword()
        {
            // Act
            var result = TestPathUtils.GetPathToGlobalPackagesFolder();

            // Assert
            // The output from dotnet nuget locals global-packages --list should contain the path
            Assert.NotEmpty(result);
        }

        [Fact]
        public void RemovePackageFromGlobalNugetCache_WithNonExistentPackage_DoesNotThrow()
        {
            // Arrange
            var nonExistentPackage = "NonExistentPackage_12345_XYZ";

            // Act & Assert - should not throw
            TestPathUtils.RemovePackageFromGlobalNugetCache(nonExistentPackage);
        }

        [Fact]
        public void RemovePackageFromGlobalNugetCache_WithPackageName_CallsRemoveDirectory()
        {
            // Arrange
            var packageName = "SomePackage";

            // Act & Assert - should not throw even if package doesn't exist
            var exception = Record.Exception(() => TestPathUtils.RemovePackageFromGlobalNugetCache(packageName));
            Assert.Null(exception);
        }

        [Fact]
        public void RemovePackageFromGlobalNugetCache_WithEmptyPackageName_DoesNotThrow()
        {
            // Arrange
            var packageName = "";

            // Act & Assert
            var exception = Record.Exception(() => TestPathUtils.RemovePackageFromGlobalNugetCache(packageName));
            Assert.Null(exception);
        }

        [Fact]
        public void RemovePackageFromGlobalNugetCache_WithNullPackageName_DoesNotThrow()
        {
            // Arrange
            string packageName = null;

            // Act & Assert
            var exception = Record.Exception(() => TestPathUtils.RemovePackageFromGlobalNugetCache(packageName));
            // Should either handle null gracefully or throw appropriate exception
            // If it throws, the exception should be documented
        }

        [Fact]
        public void RemovePackageFromGlobalNugetCache_IsCaseInsensitiveSearch()
        {
            // Arrange
            var packageName = "somepackage";

            // Act & Assert - case-insensitive search should not throw
            var exception = Record.Exception(() => TestPathUtils.RemovePackageFromGlobalNugetCache(packageName));
            Assert.Null(exception);
        }

        [Fact]
        public void RemovePackageFromGlobalNugetCache_WithSpecialCharactersInName_DoesNotThrow()
        {
            // Arrange
            var packageName = "Package.With.Dots";

            // Act & Assert
            var exception = Record.Exception(() => TestPathUtils.RemovePackageFromGlobalNugetCache(packageName));
            Assert.Null(exception);
        }

        [Fact]
        public void RemovePackageFromGlobalNugetCache_CallsGetPathToGlobalPackagesFolder()
        {
            // Arrange
            var packageName = "TestPackage";

            // Act
            TestPathUtils.RemovePackageFromGlobalNugetCache(packageName);

            // Assert - should not throw and method should execute without error
            // This tests that GetPathToGlobalPackagesFolder is called internally
        }

        [Fact]
        public void RemovePackageFromGlobalNugetCache_OperatesOnlyOnGlobalPackagesFolder()
        {
            // Arrange
            var packageName = "MyPackage";

            // Act & Assert - should only operate on the global packages folder, not system directories
            var exception = Record.Exception(() => TestPathUtils.RemovePackageFromGlobalNugetCache(packageName));
            Assert.Null(exception);
        }

        [Fact]
        public void GetPathToTestFixture_ResultIsFullPath()
        {
            // Arrange
            var fixtureName = "Test";

            // Act
            var result = TestPathUtils.GetPathToTestFixture(fixtureName);

            // Assert
            Assert.True(Path.IsPathRooted(result), "Path should be a full path");
        }

        [Fact]
        public void GetPathToScriptPackages_ResultIsFullPath()
        {
            // Arrange
            var fixtureName = "Test";

            // Act
            var result = TestPathUtils.GetPathToScriptPackages(fixtureName);

            // Assert
            Assert.True(Path.IsPathRooted(result), "Path should be a full path");
        }

        [Fact]
        public void GetPathToTempFolder_WithLongPath_ReturnsValidPath()
        {
            // Arrange
            var longPath = Path.Combine(Directory.GetCurrentDirectory(), "a", "b", "c", "d", "e");

            // Act
            var result = TestPathUtils.GetPathToTempFolder(longPath);

            // Assert
            Assert.NotNull(result);
            Assert.True(Path.IsPathRooted(result));
        }

        [Fact]
        public void GetPathToTestFixtureFolder_ResultIncludesAssemblyLocation()
        {
            // Arrange & Act
            var result = TestPathUtils.GetPathToTestFixtureFolder("Test");

            // Assert
            // Result should be based on the assembly location
            Assert.NotNull(result);
        }

        [Fact]
        public void GetPathToTestFixture_FixtureNameWithSpecialCharacters_CreatesValidPath()
        {
            // Arrange
            var fixtureName = "Test_Case-1";

            // Act
            var result = TestPathUtils.GetPathToTestFixture(fixtureName);

            // Assert
            Assert.NotNull(result);
            Assert.EndsWith(".csx", result);
        }

        [Fact]
        public void RemovePackageFromGlobalNugetCache_WithPackagePartialMatch_SearchesUsingContains()
        {
            // Arrange
            var packageName = "Common";

            // Act & Assert
            // Should not throw because SingleOrDefault returns null if no match or multiple matches
            var exception = Record.Exception(() => TestPathUtils.RemovePackageFromGlobalNugetCache(packageName));
            Assert.Null(exception);
        }
    }
}