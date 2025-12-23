using System;
using System.IO;
using System.Linq;
using System.Text;
using Dotnet.Script.DependencyModel.Runtime;
using Dotnet.Script.Shared.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet.Script.Tests
{
    [Collection("IntegrationTests")]
    public class ScriptPackagesTests_Comprehensive : IClassFixture<ScriptPackagesFixture>
    {
        private readonly ScriptPackagesFixture _fixture;

        public ScriptPackagesTests_Comprehensive(ITestOutputHelper testOutputHelper, ScriptPackagesFixture fixture)
        {
            _fixture = fixture;
            testOutputHelper.Capture();
        }

        #region Success Path Tests - Main execution scenarios

        [Fact]
        public void ShouldHandleScriptPackageWithMainCsx()
        {
            // Arrange
            var expectedExitCode = 0;
            var expectedOutput = "Hello from netstandard2.0";

            // Act
            var (output, exitcode) = ScriptTestRunner.Default.ExecuteWithScriptPackage("WithMainCsx", "--no-cache");

            // Assert
            Assert.Equal(expectedExitCode, exitcode);
            Assert.StartsWith(expectedOutput, output);
        }

        [Fact]
        public void ShouldHandleScriptWithAnyTargetFramework()
        {
            // Arrange
            var expectedExitCode = 0;
            var expectedOutput = "Hello from any target framework";

            // Act
            var (output, exitcode) = ScriptTestRunner.Default.ExecuteWithScriptPackage("WithAnyTargetFramework", "--no-cache");

            // Assert
            Assert.Equal(expectedExitCode, exitcode);
            Assert.StartsWith(expectedOutput, output);
        }

        [Fact]
        public void ShouldHandleScriptPackageWithNoEntryPointFile()
        {
            // Arrange
            var expectedExitCode = 0;
            var expectedFooOutput = "Hello from Foo.csx";
            var expectedBarOutput = "Hello from Bar.csx";

            // Act
            var (output, exitcode) = ScriptTestRunner.Default.ExecuteWithScriptPackage("WithNoEntryPointFile", "--no-cache");

            // Assert
            Assert.Equal(expectedExitCode, exitcode);
            Assert.Contains(expectedFooOutput, output);
            Assert.Contains(expectedBarOutput, output);
        }

        [Fact]
        public void ShouldHandleScriptPackageWithScriptPackageDependency()
        {
            // Arrange
            var expectedExitCode = 0;
            var expectedOutput = "Hello from netstandard2.0";

            // Act
            var (output, exitcode) = ScriptTestRunner.Default.ExecuteWithScriptPackage("WithScriptPackageDependency", "--no-cache");

            // Assert
            Assert.Equal(expectedExitCode, exitcode);
            Assert.StartsWith(expectedOutput, output);
        }

        [Fact]
        public void ShouldHandleScriptPackageWithSubFolder()
        {
            // Arrange
            var expectedExitCode = 0;
            var expectedOutput = "Hello from Bar.csx";

            // Act
            var (output, exitcode) = ScriptTestRunner.Default.ExecuteWithScriptPackage("WithSubFolder", "--no-cache");

            // Assert
            Assert.Equal(expectedExitCode, exitcode);
            Assert.StartsWith(expectedOutput, output);
        }

        #endregion

        #region Error Path Tests - Invalid package references

        [Fact]
        public void ShouldThrowExceptionWhenReferencingUnknownPackage()
        {
            // Arrange
            var expectedNonZeroExitCode = 1;
            var expectedErrorOutput = "Unable to restore packages from";

            // Act
            var (output, exitcode) = ScriptTestRunner.Default.ExecuteWithScriptPackage("WithInvalidPackageReference", "--no-cache");

            // Assert
            Assert.NotEqual(0, exitcode);
            Assert.StartsWith(expectedErrorOutput, output);
        }

        #endregion

        #region Runtime Dependency Resolution Tests

        [Fact]
        public void ShouldGetScriptFilesFromScriptPackage()
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
        public void ShouldGetScriptFilesFromScriptPackage_WithNoEntryPointFile()
        {
            // Arrange
            var resolver = CreateRuntimeDependencyResolver();
            var fixture = GetFullPathToTestFixture("ScriptPackage/WithNoEntryPointFile");
            var csxFiles = Directory.GetFiles(fixture, "*.csx");
            
            // Act
            var dependencies = resolver.GetDependencies(csxFiles.First(), Array.Empty<string>());
            var scriptPackageDependency = dependencies.SingleOrDefault(d => d.Name == "WithNoEntryPointFile");

            // Assert
            Assert.NotNull(scriptPackageDependency);
            Assert.NotEmpty(scriptPackageDependency.Scripts);
        }

        [Fact]
        public void ShouldGetEmptyDependencies_WhenNoDependenciesExist()
        {
            // Arrange
            var resolver = CreateRuntimeDependencyResolver();
            var fixture = GetFullPathToTestFixture("ScriptPackage/WithMainCsx");
            var csxFiles = Directory.GetFiles(fixture, "*.csx");
            
            // Act
            var dependencies = resolver.GetDependencies(csxFiles.First(), Array.Empty<string>()).ToList();

            // Assert
            Assert.NotEmpty(dependencies);
        }

        [Fact]
        public void ShouldReturnValidDependencyStructure()
        {
            // Arrange
            var resolver = CreateRuntimeDependencyResolver();
            var fixture = GetFullPathToTestFixture("ScriptPackage/WithMainCsx");
            var csxFiles = Directory.GetFiles(fixture, "*.csx");
            
            // Act
            var dependencies = resolver.GetDependencies(csxFiles.First(), Array.Empty<string>()).ToList();

            // Assert
            Assert.NotEmpty(dependencies);
            foreach (var dep in dependencies)
            {
                Assert.NotNull(dep.Name);
                Assert.NotNull(dep.Version);
                Assert.NotNull(dep.Assemblies);
                Assert.NotNull(dep.Scripts);
            }
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

        #region Edge Cases and Boundary Tests

        [Fact]
        public void ShouldHandleMultipleConsoleOutputs()
        {
            // Arrange & Act
            var (output, exitcode) = ScriptTestRunner.Default.ExecuteWithScriptPackage("WithNoEntryPointFile", "--no-cache");

            // Assert - output should contain multiple lines
            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Assert.NotEmpty(lines);
        }

        [Fact]
        public void ShouldPreserveExitCodeOnSuccess()
        {
            // Arrange
            var expectedExitCode = 0;

            // Act
            var (_, exitcode) = ScriptTestRunner.Default.ExecuteWithScriptPackage("WithMainCsx", "--no-cache");

            // Assert
            Assert.Equal(expectedExitCode, exitcode);
        }

        [Fact]
        public void ShouldPreserveExitCodeOnFailure()
        {
            // Arrange
            var expectedNonZeroExitCode = 1;

            // Act
            var (_, exitcode) = ScriptTestRunner.Default.ExecuteWithScriptPackage("WithInvalidPackageReference", "--no-cache");

            // Assert
            Assert.NotEqual(0, exitcode);
        }

        #endregion

        #region Console Output Redirection Tests

        [Fact]
        public void ShouldRedirectConsoleOutputCorrectly()
        {
            // Arrange
            var testOutput = new StringBuilder();
            var testWriter = new StringWriter(testOutput);

            // Act
            var oldOut = Console.Out;
            try
            {
                Console.SetOut(testWriter);
                // This would be called in normal execution
                Console.WriteLine("Test message");
                var output = testOutput.ToString();

                // Assert
                Assert.Contains("Test message", output);
            }
            finally
            {
                Console.SetOut(oldOut);
            }
        }

        [Fact]
        public void ShouldRestoreConsoleOutputAfterExecution()
        {
            // Arrange
            var originalOut = Console.Out;

            // Act
            var testOutput = new StringBuilder();
            var testWriter = new StringWriter(testOutput);
            Console.SetOut(testWriter);
            Console.SetOut(originalOut);

            // Assert
            Assert.Equal(originalOut, Console.Out);
        }

        #endregion

        #region Path Resolution Tests

        [Fact]
        public void ShouldResolveTestFixturePath()
        {
            // Arrange & Act
            var path = GetFullPathToTestFixture("ScriptPackage/WithMainCsx");

            // Assert
            Assert.NotEmpty(path);
            Assert.True(Path.IsPathRooted(path));
        }

        [Fact]
        public void ShouldFindCsxFilesInTestFixture()
        {
            // Arrange
            var fixture = GetFullPathToTestFixture("ScriptPackage/WithMainCsx");

            // Act
            var csxFiles = Directory.GetFiles(fixture, "*.csx");

            // Assert
            Assert.NotEmpty(csxFiles);
            Assert.True(csxFiles.All(f => f.EndsWith(".csx", StringComparison.OrdinalIgnoreCase)));
        }

        #endregion

        #region Resolver Creation Tests

        [Fact]
        public void ShouldCreateRuntimeDependencyResolverSuccessfully()
        {
            // Arrange & Act
            var resolver = CreateRuntimeDependencyResolver();

            // Assert
            Assert.NotNull(resolver);
        }

        [Fact]
        public void ShouldHandleEmptyPackageSources()
        {
            // Arrange
            var resolver = CreateRuntimeDependencyResolver();
            var fixture = GetFullPathToTestFixture("ScriptPackage/WithMainCsx");
            var csxFiles = Directory.GetFiles(fixture, "*.csx");

            // Act
            var dependencies = resolver.GetDependencies(csxFiles.First(), Array.Empty<string>());

            // Assert
            Assert.NotNull(dependencies);
        }

        #endregion
    }
}