using System;
using System.Linq;
using System.Text;
using Dotnet.Script.DependencyModel.Environment;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Dotnet.Script.Shared.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet.Script.Tests
{
    public class ScriptParserTestsTests
    {
        [Fact]
        public void Constructor_InitializesCorrectly()
        {
            // Arrange
            var mockTestOutputHelper = new Mock<ITestOutputHelper>();
            mockTestOutputHelper.Setup(x => x.Capture(It.IsAny<int>()));

            // Act
            var scriptParserTests = new ScriptParserTests(mockTestOutputHelper.Object);

            // Assert
            Assert.NotNull(scriptParserTests);
        }

        [Fact]
        public void CreateParser_CreatesValidScriptParser()
        {
            // Arrange
            var scriptParserTests = new ScriptParserTests(null);

            // Act
            var parser = scriptParserTests.CreateParser();

            // Assert
            Assert.NotNull(parser);
        }

        [Theory]
        [InlineData("#r \"nuget:Package, 1.2.3\"", 1, "Package", "1.2.3")]
        [InlineData("#load \"nuget:Package\"", 1, "Package", "")]
        [InlineData("#r \"nuget:AnotherPackage, 3.2.1\"", 1, "AnotherPackage", "3.2.1")]
        public void ParseFromCode_VariousPackageFormats_ResolvesCorrectly(string code, int expectedCount, string expectedId, string expectedVersion)
        {
            // Arrange
            var scriptParserTests = new ScriptParserTests(null);
            var parser = scriptParserTests.CreateParser();

            // Act
            var result = parser.ParseFromCode(code);

            // Assert
            Assert.Equal(expectedCount, result.PackageReferences.Count);
            Assert.Equal(expectedId, result.PackageReferences.Single().Id.Value);
            Assert.Equal(expectedVersion, result.PackageReferences.Single().Version.Value);
        }

        [Fact]
        public void ParseFromCode_MultiplePackages_ResolvesAllPackages()
        {
            // Arrange
            var code = new StringBuilder();
            code.AppendLine("#r \"nuget:Package, 1.2.3\"");
            code.AppendLine("#r \"nuget:AnotherPackage, 3.2.1\"");

            var scriptParserTests = new ScriptParserTests(null);
            var parser = scriptParserTests.CreateParser();

            // Act
            var result = parser.ParseFromCode(code.ToString());

            // Assert
            Assert.Equal(2, result.PackageReferences.Count);
            Assert.Equal("Package", result.PackageReferences.First().Id.Value);
            Assert.Equal("1.2.3", result.PackageReferences.First().Version.Value);
            Assert.Equal("AnotherPackage", result.PackageReferences.Last().Id.Value);
            Assert.Equal("3.2.1", result.PackageReferences.Last().Version.Value);
        }

        [Theory]
        [InlineData("#load \"nuget:Package, 1.2.3\"\n\n\"Hello world\"")]
        [InlineData("#r \"\n\"nuget:Package, 1.2.3\"")]
        [InlineData("#r \"nuget:\nPackage, 1.2.3\"")]
        public void ParseFromCode_InvalidDirectives_ReturnsEmptyPackageReferences(string code)
        {
            // Arrange
            var scriptParserTests = new ScriptParserTests(null);
            var parser = scriptParserTests.CreateParser();

            // Act
            var result = parser.ParseFromCode(code);

            // Assert
            Assert.Equal(0, result.PackageReferences.Count);
        }
    }
}