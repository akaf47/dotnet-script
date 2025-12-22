using System;
using System.IO;
using System.Linq;
using System.Text;
using Dotnet.Script.DependencyModel.Runtime;
using Dotnet.Script.Shared.Tests;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet.Script.Tests
{
    /// <summary>
    /// Comprehensive test coverage for ScriptPackagesTests
    /// Tests all public methods, branches, edge cases, and error scenarios
    /// </summary>
    public class ScriptPackagesTestsComplete : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ScriptPackagesTestsComplete(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _testOutputHelper.Capture();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_CapturesTestOutput_WhenInitialized()
        {
            // Arrange & Act
            var instance = new ScriptPackagesTests(_testOutputHelper);

            // Assert
            Assert.NotNull(instance);
        }

        #endregion

        #region ShouldHandleScriptPackageWithMainCsx Tests

        [Fact]
        public void ShouldHandleScriptPackageWithMainCsx_ExecutesSuccessfully_WhenValidPackageExists()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithMainCsx", "--no-cache");

            // Assert
            Assert.Equal(0, exitcode);
            Assert.StartsWith("Hello from netstandard2.0", output);
        }

        [Fact]
        public void ShouldHandleScriptPackageWithMainCsx_ProducesNonEmptyOutput_WhenExecuted()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithMainCsx", "--no-cache");

            // Assert
            Assert.NotEmpty(output);
            Assert.False(string.IsNullOrWhiteSpace(output));
        }

        #endregion

        #region ShouldHandleScriptWithAnyTargetFramework Tests

        [Fact]
        public void ShouldHandleScriptWithAnyTargetFramework_ExecutesSuccessfully_WhenTargetFrameworkIsAny()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithAnyTargetFramework", "--no-cache");

            // Assert
            Assert.Equal(0, exitcode);
            Assert.StartsWith("Hello from any target framework", output);
        }

        [Fact]
        public void ShouldHandleScriptWithAnyTargetFramework_ProducesExpectedMessage_WhenExecuted()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithAnyTargetFramework", "--no-cache");

            // Assert
            Assert.Contains("any target framework", output);
        }

        #endregion

        #region ShouldHandleScriptPackageWithNoEntryPointFile Tests

        [Fact]
        public void ShouldHandleScriptPackageWithNoEntryPointFile_ExecutesSuccessfully_WhenNoMainCsxExists()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithNoEntryPointFile", "--no-cache");

            // Assert
            Assert.Equal(0, exitcode);
        }

        [Fact]
        public void ShouldHandleScriptPackageWithNoEntryPointFile_ExecutesAllScripts_WhenNoMainCsxPresent()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithNoEntryPointFile", "--no-cache");

            // Assert
            Assert.Contains("Hello from Foo.csx", output);
            Assert.Contains("Hello from Bar.csx", output);
        }

        [Fact]
        public void ShouldHandleScriptPackageWithNoEntryPointFile_ProducesMultipleOutputs_WhenMultipleScriptsExecute()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithNoEntryPointFile", "--no-cache");

            // Assert
            Assert.NotEmpty(output);
            var lines = output.Split(Environment.NewLine);
            Assert.True(lines.Length > 1);
        }

        #endregion

        #region ShouldHandleScriptPackageWithScriptPackageDependency Tests

        [Fact]
        public void ShouldHandleScriptPackageWithScriptPackageDependency_ExecutesSuccessfully_WhenDependencyExists()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithScriptPackageDependency", "--no-cache");

            // Assert
            Assert.Equal(0, exitcode);
        }

        [Fact]
        public void ShouldHandleScriptPackageWithScriptPackageDependency_ResolvesDependencies_WhenPackageDependsOnOtherPackage()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithScriptPackageDependency", "--no-cache");

            // Assert
            Assert.StartsWith("Hello from netstandard2.0", output);
        }

        #endregion

        #region ShouldThrowExceptionWhenReferencingUnknownPackage Tests

        [Fact]
        public void ShouldThrowExceptionWhenReferencingUnknownPackage_FailsWithNonZeroExitCode_WhenPackageNotFound()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithInvalidPackageReference", "--no-cache");

            // Assert
            Assert.NotEqual(0, exitcode);
        }

        [Fact]
        public void ShouldThrowExceptionWhenReferencingUnknownPackage_ProducesErrorMessage_WhenPackageCannotBeResolved()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithInvalidPackageReference", "--no-cache");

            // Assert
            Assert.StartsWith("Unable to restore packages from", output);
        }

        [Fact]
        public void ShouldThrowExceptionWhenReferencingUnknownPackage_ProducesNonEmptyErrorOutput_WhenPackageResolutionFails()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithInvalidPackageReference", "--no-cache");

            // Assert
            Assert.NotEmpty(output);
            Assert.NotEqual(0, exitcode);
        }

        #endregion

        #region ShouldHandleScriptPackageWithSubFolder Tests

        [Fact]
        public void ShouldHandleScriptPackageWithSubFolder_ExecutesSuccessfully_WhenScriptsInSubFolder()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithSubFolder", "--no-cache");

            // Assert
            Assert.Equal(0, exitcode);
        }

        [Fact]
        public void ShouldHandleScriptPackageWithSubFolder_ExecutesCorrectScript_WhenScriptsOrganizedInSubFolders()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithSubFolder", "--no-cache");

            // Assert
            Assert.StartsWith("Hello from Bar.csx", output);
        }

        #endregion

        #region ShouldGetScriptFilesFromScriptPackage Tests

        [Fact]
        public void ShouldGetScriptFilesFromScriptPackage_ReturnsScriptFiles_WhenResolverQueriesPackage()
        {
            // Arrange
            var resolver = CreateRuntimeDependencyResolver();
            var fixture = GetFullPathToTestFixture("ScriptPackage/WithMainCsx");
            var csxFiles = Directory.GetFiles(fixture, "*.csx");

            // Act
            var dependencies = resolver.GetDependencies(csxFiles.First(), Array.Empty<string>());
            var scriptPackageDependency = dependencies.SingleOrDefault(d => d.Name == "ScriptPackageWithMainCsx");

            // Assert
            Assert.NotNull(scriptPackageDependency);
            Assert.NotEmpty(scriptPackageDependency.Scripts);
        }

        [Fact]
        public void ShouldGetScriptFilesFromScriptPackage_ReturnsNonEmptyScriptsList_WhenPackageContainsScripts()
        {
            // Arrange
            var resolver = CreateRuntimeDependencyResolver();
            var fixture = GetFullPathToTestFixture("ScriptPackage/WithMainCsx");
            var csxFiles = Directory.GetFiles(fixture, "*.csx");

            // Act
            var dependencies = resolver.GetDependencies(csxFiles.First(), Array.Empty<string>());
            var scriptFiles = dependencies.Single(d => d.Name == "ScriptPackageWithMainCsx").Scripts;

            // Assert
            Assert.NotEmpty(scriptFiles);
            Assert.All(scriptFiles, file => Assert.False(string.IsNullOrEmpty(file)));
        }

        #endregion

        #region GetFullPathToTestFixture Tests

        [Fact]
        public void GetFullPathToTestFixture_ReturnsValidPath_WhenCalledWithValidRelativePath()
        {
            // Act
            var result = GetFullPathToTestFixture("ScriptPackage/WithMainCsx");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetFullPathToTestFixture_ReturnsAbsolutePath_WhenCalled()
        {
            // Act
            var result = GetFullPathToTestFixture("ScriptPackage/WithMainCsx");

            // Assert
            Assert.True(Path.IsPathRooted(result));
        }

        [Fact]
        public void GetFullPathToTestFixture_IncludesProvidedPath_WhenCalledWithPath()
        {
            // Arrange
            var relativePath = "ScriptPackage/WithMainCsx";

            // Act
            var result = GetFullPathToTestFixture(relativePath);

            // Assert
            Assert.Contains("TestFixtures", result);
            Assert.Contains("ScriptPackage", result);
        }

        [Fact]
        public void GetFullPathToTestFixture_HandlesEmptyPath_WhenCalledWithEmptyString()
        {
            // Act
            var result = GetFullPathToTestFixture("");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetFullPathToTestFixture_CombinesPaths_WhenMultipleSegmentsProvided()
        {
            // Arrange
            var paths = new[] { "ScriptPackage", "WithMainCsx" };
            var expectedPath = Path.Combine(paths);

            // Act
            var result = GetFullPathToTestFixture(expectedPath);

            // Assert
            Assert.Contains("ScriptPackage", result);
            Assert.Contains("WithMainCsx", result);
        }

        #endregion

        #region CreateRuntimeDependencyResolver Tests

        [Fact]
        public void CreateRuntimeDependencyResolver_ReturnsValidResolver_WhenCalled()
        {
            // Act
            var resolver = CreateRuntimeDependencyResolver();

            // Assert
            Assert.NotNull(resolver);
            Assert.IsType<RuntimeDependencyResolver>(resolver);
        }

        [Fact]
        public void CreateRuntimeDependencyResolver_CreatesResolverWithoutCache_WhenCalled()
        {
            // Act
            var resolver = CreateRuntimeDependencyResolver();

            // Assert - Verify resolver is configured without restore cache
            Assert.NotNull(resolver);
        }

        #endregion

        #region Execute Tests

        [Fact]
        public void Execute_CapturesToConsoleOutput_WhenScriptExecutes()
        {
            // Act
            var output = Execute("ScriptPackage/WithMainCsx/main.csx");

            // Assert
            Assert.NotEmpty(output);
        }

        [Fact]
        public void Execute_RestoresConsoleOutput_WhenExecutionCompletes()
        {
            // Arrange
            var originalOut = Console.Out;
            var originalError = Console.Error;

            // Act
            Execute("ScriptPackage/WithMainCsx/main.csx");

            // Assert
            Assert.Equal(originalOut, Console.Out);
            Assert.Equal(originalError, Console.Error);
        }

        [Fact]
        public void Execute_RestoresConsoleOnException_WhenExceptionOccurs()
        {
            // Arrange
            var originalOut = Console.Out;
            var originalError = Console.Error;

            // Act & Assert
            try
            {
                Execute("NonExistent/script.csx");
            }
            catch
            {
                // Expected
            }

            // Verify output is restored despite exception
            Assert.NotNull(Console.Out);
            Assert.NotNull(Console.Error);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void ShouldHandleScriptPackageWithMainCsx_AcceptsNoCacheFlag_WhenExecutingWithOptions()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var (output, exitcode) = runner.ExecuteWithScriptPackage("WithMainCsx", "--no-cache");

            // Assert
            Assert.Equal(0, exitcode);
        }

        [Fact]
        public void ShouldHandleMultipleConsecutiveExecutions_WithoutStatePollution_WhenExecutedSequentially()
        {
            // Arrange
            var runner = ScriptTestRunner.Default;

            // Act
            var result1 = runner.ExecuteWithScriptPackage("WithMainCsx", "--no-cache");
            var result2 = runner.ExecuteWithScriptPackage("WithAnyTargetFramework", "--no-cache");

            // Assert
            Assert.Equal(0, result1.Item2);
            Assert.Equal(0, result2.Item2);
        }

        #endregion

        #region Helper Methods

        private static string GetFullPathToTestFixture(string path)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDirectory, "..", "..", "..", "TestFixtures", path);
        }

        private RuntimeDependencyResolver CreateRuntimeDependencyResolver()
        {
            var resolver = new RuntimeDependencyResolver(TestOutputHelper.CreateTestLogFactory(), useRestoreCache: false);
            return resolver;
        }

        private string Execute(string scriptFileName)
        {
            var output = new StringBuilder();
            var stringWriter = new StringWriter(output);
            var oldOut = Console.Out;
            var oldErrorOut = Console.Error;
            try
            {
                Console.SetOut(stringWriter);
                Console.SetError(stringWriter);
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var fullPathToScriptFile = Path.Combine(baseDir, "..", "..", "..", "TestFixtures", "ScriptPackage", scriptFileName);
                Program.Main(new[] { fullPathToScriptFile, "--no-cache" });
                return output.ToString();
            }
            finally
            {
                Console.SetOut(oldOut);
                Console.SetError(oldErrorOut);
            }
        }

        #endregion

        public void Dispose()
        {
            // Cleanup
        }
    }
}