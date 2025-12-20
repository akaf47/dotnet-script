using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Dotnet.Script.DependencyModel.Environment;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Dotnet.Script.Shared.Tests;
using Xunit;
using Xunit.Abstractions;
using Moq;

namespace Dotnet.Script.Tests
{
    public class ScriptProjectProviderTestsTests
    {
        [Fact]
        public void Constructor_InitializesCorrectly()
        {
            // Arrange
            var mockTestOutputHelper = new Mock<ITestOutputHelper>();
            mockTestOutputHelper.Setup(x => x.Capture());

            // Act
            var scriptProjectProviderTests = new ScriptProjectProviderTests(mockTestOutputHelper.Object);

            // Assert
            Assert.NotNull(scriptProjectProviderTests);
        }

        [Fact]
        public void ShouldLogProjectFileContent_VerifyLoggedContent()
        {
            // Arrange
            var log = new StringBuilder();
            var logAction = new Func<string, (string, string)>(level => (level, ""));
            var scriptProjectProviderTests = new ScriptProjectProviderTests(null);
            var provider = new ScriptProjectProvider(level => ((l, msg, ex) => log.AppendLine(msg)));

            // Act
            provider.CreateProject(TestPathUtils.GetPathToTestFixtureFolder("HelloWorld"), ScriptEnvironment.Default.TargetFramework, true);
            var output = log.ToString();

            // Assert
            Assert.Contains("<Project Sdk=\"Microsoft.NET.Sdk\">", output);
        }

        [Fact]
        public void ShouldUseSpecifiedSdk_WebSdkCorrectlySet()
        {
            // Arrange
            var scriptProjectProviderTests = new ScriptProjectProviderTests(null);
            var provider = new ScriptProjectProvider(TestOutputHelper.CreateTestLogFactory());

            // Act
            var projectFileInfo = provider.CreateProject(TestPathUtils.GetPathToTestFixtureFolder("WebApi"), ScriptEnvironment.Default.TargetFramework, true);
            var projectSdk = XDocument.Load(projectFileInfo.Path).Descendants("Project").Single().Attributes("Sdk").Single().Value;

            // Assert
            Assert.Equal("Microsoft.NET.Sdk.Web", projectSdk);
        }

        [Theory]
        [InlineData("#!/usr/bin/env dotnet-script\n#r \"sdk:Microsoft.NET.Sdk.Web\"")]
        [InlineData("#!/usr/bin/env dotnet-script\n\n#r \"sdk:Microsoft.NET.Sdk.Web\"")]
        public void ShouldHandleShebangBeforeSdk_CorrectSdkParsing(string code)
        {
            // Arrange
            var scriptProjectProviderTests = new ScriptProjectProviderTests(null);
            var parser = new ScriptParser(TestOutputHelper.CreateTestLogFactory());

            // Act
            var result = parser.ParseFromCode(code);

            // Assert
            Assert.Equal("Microsoft.NET.Sdk.Web", result.Sdk);
        }
    }
}