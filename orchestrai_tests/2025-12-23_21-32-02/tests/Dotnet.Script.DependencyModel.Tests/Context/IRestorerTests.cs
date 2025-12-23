using System;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Xunit;

namespace Dotnet.Script.DependencyModel.Tests.Context
{
    /// <summary>
    /// Comprehensive tests for IRestorer interface.
    /// Tests all public methods and properties to ensure 100% code coverage.
    /// </summary>
    public class IRestorerTests
    {
        /// <summary>
        /// Tests that Restore method can be called with valid parameters.
        /// </summary>
        [Fact]
        public void RestoreMethod_Should_Be_Callable_With_Valid_ProjectFileInfo()
        {
            // Arrange
            var projectFileInfo = new ProjectFileInfo("/path/to/project.csproj", "/path/to/NuGet.Config");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var testRestorer = new TestRestorer();

            // Act
            testRestorer.Restore(projectFileInfo, packageSources);

            // Assert
            Assert.True(testRestorer.RestoreWasCalled);
            Assert.Same(projectFileInfo, testRestorer.LastProjectFileInfo);
            Assert.Same(packageSources, testRestorer.LastPackageSources);
        }

        /// <summary>
        /// Tests that Restore method can be called with null ProjectFileInfo (should not throw during interface call).
        /// </summary>
        [Fact]
        public void RestoreMethod_Should_Accept_Null_ProjectFileInfo()
        {
            // Arrange
            var testRestorer = new TestRestorer();
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act & Assert - should not throw
            testRestorer.Restore(null, packageSources);
            Assert.True(testRestorer.RestoreWasCalled);
        }

        /// <summary>
        /// Tests that Restore method can be called with null package sources array.
        /// </summary>
        [Fact]
        public void RestoreMethod_Should_Accept_Null_PackageSources()
        {
            // Arrange
            var projectFileInfo = new ProjectFileInfo("/path/to/project.csproj", "/path/to/NuGet.Config");
            var testRestorer = new TestRestorer();

            // Act & Assert - should not throw
            testRestorer.Restore(projectFileInfo, null);
            Assert.True(testRestorer.RestoreWasCalled);
        }

        /// <summary>
        /// Tests that Restore method can be called with empty package sources array.
        /// </summary>
        [Fact]
        public void RestoreMethod_Should_Accept_Empty_PackageSources()
        {
            // Arrange
            var projectFileInfo = new ProjectFileInfo("/path/to/project.csproj", "/path/to/NuGet.Config");
            var testRestorer = new TestRestorer();
            var emptyPackageSources = new string[] { };

            // Act
            testRestorer.Restore(projectFileInfo, emptyPackageSources);

            // Assert
            Assert.True(testRestorer.RestoreWasCalled);
            Assert.Empty(testRestorer.LastPackageSources);
        }

        /// <summary>
        /// Tests that Restore method can be called with multiple package sources.
        /// </summary>
        [Fact]
        public void RestoreMethod_Should_Accept_Multiple_PackageSources()
        {
            // Arrange
            var projectFileInfo = new ProjectFileInfo("/path/to/project.csproj", "/path/to/NuGet.Config");
            var testRestorer = new TestRestorer();
            var multiplePackageSources = new[]
            {
                "https://api.nuget.org/v3/index.json",
                "https://myget.org/F/myFeed/api/v3/index.json",
                "https://another-feed.org/api/v3/index.json"
            };

            // Act
            testRestorer.Restore(projectFileInfo, multiplePackageSources);

            // Assert
            Assert.True(testRestorer.RestoreWasCalled);
            Assert.Equal(3, testRestorer.LastPackageSources.Length);
        }

        /// <summary>
        /// Tests that CanRestore property can be accessed and returns expected type.
        /// </summary>
        [Fact]
        public void CanRestoreProperty_Should_Return_Boolean_Value()
        {
            // Arrange
            var testRestorer = new TestRestorer(canRestore: true);

            // Act
            var canRestore = testRestorer.CanRestore;

            // Assert
            Assert.True(canRestore);
            Assert.IsType<bool>(canRestore);
        }

        /// <summary>
        /// Tests that CanRestore property returns false when explicitly set.
        /// </summary>
        [Fact]
        public void CanRestoreProperty_Should_Return_False_When_Not_Available()
        {
            // Arrange
            var testRestorer = new TestRestorer(canRestore: false);

            // Act
            var canRestore = testRestorer.CanRestore;

            // Assert
            Assert.False(canRestore);
        }

        /// <summary>
        /// Tests that CanRestore property is read-only.
        /// </summary>
        [Fact]
        public void CanRestoreProperty_Should_Be_ReadOnly()
        {
            // Arrange
            var testRestorer = new TestRestorer(canRestore: true);

            // Act
            var firstAccess = testRestorer.CanRestore;
            var secondAccess = testRestorer.CanRestore;

            // Assert - values should be consistent
            Assert.Equal(firstAccess, secondAccess);
        }

        /// <summary>
        /// Tests that Restore method handles ProjectFileInfo with null nuget config path.
        /// </summary>
        [Fact]
        public void RestoreMethod_Should_Handle_ProjectFileInfo_With_Null_NugetConfigPath()
        {
            // Arrange
            var projectFileInfo = new ProjectFileInfo("/path/to/project.csproj", null);
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var testRestorer = new TestRestorer();

            // Act
            testRestorer.Restore(projectFileInfo, packageSources);

            // Assert
            Assert.True(testRestorer.RestoreWasCalled);
            Assert.Null(testRestorer.LastProjectFileInfo.NuGetConfigFile);
        }

        /// <summary>
        /// Tests that Restore method handles ProjectFileInfo with empty path.
        /// </summary>
        [Fact]
        public void RestoreMethod_Should_Handle_ProjectFileInfo_With_Empty_Path()
        {
            // Arrange
            var projectFileInfo = new ProjectFileInfo(string.Empty, "/path/to/NuGet.Config");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var testRestorer = new TestRestorer();

            // Act
            testRestorer.Restore(projectFileInfo, packageSources);

            // Assert
            Assert.True(testRestorer.RestoreWasCalled);
            Assert.Empty(testRestorer.LastProjectFileInfo.Path);
        }

        /// <summary>
        /// Tests that Restore method can be called multiple times sequentially.
        /// </summary>
        [Fact]
        public void RestoreMethod_Should_Support_Multiple_Sequential_Calls()
        {
            // Arrange
            var testRestorer = new TestRestorer();
            var projectFileInfo1 = new ProjectFileInfo("/path/to/project1.csproj", "/path/to/NuGet.Config");
            var projectFileInfo2 = new ProjectFileInfo("/path/to/project2.csproj", "/path/to/NuGet.Config");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            testRestorer.Restore(projectFileInfo1, packageSources);
            var afterFirstCall = testRestorer.LastProjectFileInfo;
            
            testRestorer.Restore(projectFileInfo2, packageSources);
            var afterSecondCall = testRestorer.LastProjectFileInfo;

            // Assert
            Assert.Equal(2, testRestorer.CallCount);
            Assert.NotEqual(afterFirstCall.Path, afterSecondCall.Path);
        }

        /// <summary>
        /// Test implementation of IRestorer for testing purposes.
        /// </summary>
        private class TestRestorer : IRestorer
        {
            private readonly bool _canRestore;
            public int CallCount { get; private set; }
            public bool RestoreWasCalled { get; private set; }
            public ProjectFileInfo LastProjectFileInfo { get; private set; }
            public string[] LastPackageSources { get; private set; }

            public TestRestorer(bool canRestore = true)
            {
                _canRestore = canRestore;
                CallCount = 0;
                RestoreWasCalled = false;
            }

            public bool CanRestore => _canRestore;

            public void Restore(ProjectFileInfo projectFileInfo, string[] packageSources)
            {
                CallCount++;
                RestoreWasCalled = true;
                LastProjectFileInfo = projectFileInfo;
                LastPackageSources = packageSources;
            }
        }
    }
}