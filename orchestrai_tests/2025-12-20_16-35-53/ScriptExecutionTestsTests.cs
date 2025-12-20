using System;
using Xunit;
using Moq;
using System.IO;

namespace Dotnet.Script.Tests
{
    public class ScriptExecutionTestsTests
    {
        [Fact]
        public void Constructor_InitializesCorrectly()
        {
            // Arrange
            var mockTestOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            var scriptExecutionTests = new ScriptExecutionTests(mockTestOutputHelper.Object);

            // Assert: No exception should be thrown
        }

        [Theory]
        [InlineData("HelloWorld", "Hello World")]
        [InlineData("NonExistentFixture", "")]
        public void ExecuteFixture_WithDifferentInputs_ReturnsExpectedOutput(string fixtureName, string expectedOutput)
        {
            // Arrange
            var scriptExecutionTests = new ScriptExecutionTests(null);

            // Act
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture(fixtureName);

            // Assert
            if (!string.IsNullOrEmpty(expectedOutput))
            {
                Assert.Contains(expectedOutput, output);
            }
        }

        [Fact]
        public void CreateTestScript_GeneratesValidScriptFile()
        {
            // Arrange
            using var tempFolder = new DisposableFolder();

            // Act
            var scriptPath = ScriptExecutionTests.CreateTestScript(tempFolder.Path);

            // Assert
            Assert.True(File.Exists(scriptPath));
            var scriptContent = File.ReadAllText(scriptPath);
            Assert.Contains("#r \"nuget:NuGetConfigTestLibrary, 1.0.0\"", scriptContent);
            Assert.Contains("WriteLine(\"Success\");", scriptContent);
        }

        [Fact]
        public void CreateTestPackage_GeneratesValidPackage()
        {
            // Arrange
            using var tempFolder = new DisposableFolder();

            // Act
            ScriptExecutionTests.CreateTestPackage(tempFolder.Path);

            // Assert
            Assert.True(Directory.Exists(Path.Combine(tempFolder.Path, "NuGetConfigTestLibrary")));
            Assert.True(Directory.Exists(Path.Combine(tempFolder.Path, "packagePath")));
            Assert.True(File.Exists(Path.Combine(tempFolder.Path, "NuGet.Config")));
        }
    }
}