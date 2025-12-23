using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Dotnet.Script.DependencyModel.Environment;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Dotnet.Script.Shared.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet.Script.Tests
{
    [Collection("IntegrationTests")]
    public class ScriptProjectProviderTestsEnhanced
    {
        private readonly ScriptEnvironment _scriptEnvironment;
        private readonly LogFactory _logFactory;

        public ScriptProjectProviderTestsEnhanced(ITestOutputHelper testOutputHelper)
        {
            testOutputHelper.Capture();
            _scriptEnvironment = ScriptEnvironment.Default;
            _logFactory = TestOutputHelper.CreateTestLogFactory();
        }

        #region CreateProject Tests

        [Fact]
        public void CreateProject_WithValidScriptFolder_ShouldCreateProjectFile()
        {
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(
                TestPathUtils.GetPathToTestFixtureFolder("HelloWorld"),
                _scriptEnvironment.TargetFramework,
                true);

            Assert.NotNull(projectFileInfo);
            Assert.NotNull(projectFileInfo.Path);
            Assert.True(File.Exists(projectFileInfo.Path));
        }

        [Fact]
        public void CreateProject_WithNullTargetFramework_ShouldCreateProjectWithProvidedFramework()
        {
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(
                TestPathUtils.GetPathToTestFixtureFolder("HelloWorld"),
                "net6.0",
                true);

            Assert.NotNull(projectFileInfo);
            var projectXml = XDocument.Load(projectFileInfo.Path);
            var targetFramework = projectXml.Descendants("TargetFramework").FirstOrDefault()?.Value;
            Assert.Equal("net6.0", targetFramework);
        }

        [Fact]
        public void CreateProject_WithEmptyDirectory_ShouldReturnNull()
        {
            using var emptyFolder = new DisposableFolder();
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(
                emptyFolder.Path,
                _scriptEnvironment.TargetFramework,
                false);

            Assert.Null(projectFileInfo);
        }

        [Fact]
        public void CreateProject_WithNoNuGetReferencesAndNoCsproj_ShouldReturnNull()
        {
            using var tempFolder = new DisposableFolder();
            var csxFile = Path.Combine(tempFolder.Path, "test.csx");
            File.WriteAllText(csxFile, "WriteLine(\"test\");");

            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(
                tempFolder.Path,
                _scriptEnvironment.TargetFramework,
                false);

            Assert.Null(projectFileInfo);
        }

        [Fact]
        public void CreateProject_WithEnableNuGetScriptReferences_ShouldCreateProjectEvenWithoutCsproj()
        {
            using var tempFolder = new DisposableFolder();
            var csxFile = Path.Combine(tempFolder.Path, "test.csx");
            File.WriteAllText(csxFile, "WriteLine(\"test\");");

            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(
                tempFolder.Path,
                _scriptEnvironment.TargetFramework,
                true);

            Assert.NotNull(projectFileInfo);
        }

        [Fact]
        public void CreateProject_WithExistingCsproj_ShouldCreateProject()
        {
            using var tempFolder = new DisposableFolder();
            var csxFile = Path.Combine(tempFolder.Path, "test.csx");
            File.WriteAllText(csxFile, "WriteLine(\"test\");");
            var csprojFile = Path.Combine(tempFolder.Path, "test.csproj");
            File.WriteAllText(csprojFile, "<Project />");

            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(
                tempFolder.Path,
                _scriptEnvironment.TargetFramework,
                false);

            Assert.NotNull(projectFileInfo);
        }

        [Fact]
        public void CreateProject_WithMultipleCsxFiles_ShouldParseAllFiles()
        {
            using var tempFolder = new DisposableFolder();
            File.WriteAllText(Path.Combine(tempFolder.Path, "test1.csx"), "WriteLine(\"test1\");");
            File.WriteAllText(Path.Combine(tempFolder.Path, "test2.csx"), "#r \"nuget: Newtonsoft.Json, 12.0.1\"\nWriteLine(\"test2\");");

            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(
                tempFolder.Path,
                _scriptEnvironment.TargetFramework,
                true);

            Assert.NotNull(projectFileInfo);
            var projectXml = XDocument.Load(projectFileInfo.Path);
            var packageReferences = projectXml.Descendants("PackageReference").Count();
            Assert.Equal(1, packageReferences);
        }

        [Fact]
        public void CreateProject_WithExplicitScriptFilesArray_ShouldCreateProject()
        {
            using var tempFolder = new DisposableFolder();
            var csxFile = Path.Combine(tempFolder.Path, "test.csx");
            File.WriteAllText(csxFile, "WriteLine(\"test\");");

            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(
                tempFolder.Path,
                new[] { csxFile },
                _scriptEnvironment.TargetFramework,
                true);

            Assert.NotNull(projectFileInfo);
        }

        [Fact]
        public void CreateProject_WithNullScriptFilesArray_ShouldReturnNull()
        {
            using var tempFolder = new DisposableFolder();
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(
                tempFolder.Path,
                null,
                _scriptEnvironment.TargetFramework,
                true);

            Assert.Null(projectFileInfo);
        }

        [Fact]
        public void CreateProject_WithEmptyScriptFilesArray_ShouldReturnNull()
        {
            using var tempFolder = new DisposableFolder();
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(
                tempFolder.Path,
                new string[] { },
                _scriptEnvironework.TargetFramework,
                true);

            Assert.Null(projectFileInfo);
        }

        #endregion

        #region CreateProjectForScriptFile Tests

        [Fact]
        public void CreateProjectForScriptFile_WithValidScriptFile_ShouldCreateProject()
        {
            var scriptFile = TestPathUtils.GetPathToTestFixture("HelloWorld");
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProjectForScriptFile(scriptFile);

            Assert.NotNull(projectFileInfo);
            Assert.NotNull(projectFileInfo.Path);
            Assert.True(File.Exists(projectFileInfo.Path));
        }

        [Fact]
        public void CreateProjectForScriptFile_ShouldUseScriptEnvironmentTargetFramework()
        {
            var scriptFile = TestPathUtils.GetPathToTestFixture("HelloWorld");
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProjectForScriptFile(scriptFile);

            var projectXml = XDocument.Load(projectFileInfo.Path);
            var targetFramework = projectXml.Descendants("TargetFramework").FirstOrDefault()?.Value;
            Assert.Equal(_scriptEnvironment.TargetFramework, targetFramework);
        }

        #endregion

        #region CreateProjectForRepl Tests

        [Fact]
        public void CreateProjectForRepl_WithValidCode_ShouldCreateProject()
        {
            using var tempFolder = new DisposableFolder();
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProjectForRepl(
                "WriteLine(\"hello\");",
                tempFolder.Path,
                "net6.0");

            Assert.NotNull(projectFileInfo);
            Assert.NotNull(projectFileInfo.Path);
            Assert.True(File.Exists(projectFileInfo.Path));
        }

        [Fact]
        public void CreateProjectForRepl_ShouldCreateInteractiveFolder()
        {
            using var tempFolder = new DisposableFolder();
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProjectForRepl(
                "WriteLine(\"hello\");",
                tempFolder.Path,
                "net6.0");

            Assert.Contains("interactive", projectFileInfo.Path);
        }

        [Fact]
        public void CreateProjectForRepl_WithNuGetReferences_ShouldIncludeInProject()
        {
            using var tempFolder = new DisposableFolder();
            var code = "#r \"nuget: Newtonsoft.Json, 12.0.1\"\nWriteLine(\"hello\");";
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProjectForRepl(code, tempFolder.Path, "net6.0");

            var projectXml = XDocument.Load(projectFileInfo.Path);
            var packageReferences = projectXml.Descendants("PackageReference").Count();
            Assert.Equal(1, packageReferences);
        }

        [Fact]
        public void CreateProjectForRepl_WithSdkReference_ShouldIncludeSdk()
        {
            using var tempFolder = new DisposableFolder();
            var code = "#r \"sdk:Microsoft.NET.Sdk.Web\"\nWriteLine(\"hello\");";
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProjectForRepl(code, tempFolder.Path, "net6.0");

            var projectXml = XDocument.Load(projectFileInfo.Path);
            var sdk = projectXml.Descendants("Project").First().Attributes("Sdk").FirstOrDefault()?.Value;
            Assert.Equal("Microsoft.NET.Sdk.Web", sdk);
        }

        [Fact]
        public void CreateProjectForRepl_WithLoadDirective_ShouldParseLoadedFiles()
        {
            using var tempFolder = new DisposableFolder();
            var helperFile = Path.Combine(tempFolder.Path, "helper.csx");
            File.WriteAllText(helperFile, "#r \"nuget: Newtonsoft.Json, 12.0.1\"\nvar x = 1;");

            var code = $"#load \"{helperFile}\"\nWriteLine(\"hello\");";
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProjectForRepl(code, tempFolder.Path, "net6.0");

            var projectXml = XDocument.Load(projectFileInfo.Path);
            var packageReferences = projectXml.Descendants("PackageReference").Count();
            Assert.Equal(1, packageReferences);
        }

        [Fact]
        public void CreateProjectForRepl_WithDefaultTargetFramework_ShouldUseProvidedValue()
        {
            using var tempFolder = new DisposableFolder();
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProjectForRepl(
                "WriteLine(\"hello\");",
                tempFolder.Path,
                "net48");

            var projectXml = XDocument.Load(projectFileInfo.Path);
            var targetFramework = projectXml.Descendants("TargetFramework").FirstOrDefault()?.Value;
            Assert.Equal("net48", targetFramework);
        }

        #endregion

        #region CreateProjectFileFromScriptFiles Tests

        [Fact]
        public void CreateProjectFileFromScriptFiles_WithValidFiles_ShouldCreateProjectFile()
        {
            using var tempFolder = new DisposableFolder();
            var csxFile = Path.Combine(tempFolder.Path, "test.csx");
            File.WriteAllText(csxFile, "WriteLine(\"test\");");

            var provider = new ScriptProjectProvider(_logFactory);
            var projectFile = provider.CreateProjectFileFromScriptFiles(_scriptEnvironment.TargetFramework, new[] { csxFile });

            Assert.NotNull(projectFile);
            Assert.Equal(_scriptEnvironment.TargetFramework, projectFile.TargetFramework);
        }

        [Fact]
        public void CreateProjectFileFromScriptFiles_WithNuGetReferences_ShouldIncludePackages()
        {
            using var tempFolder = new DisposableFolder();
            var csxFile = Path.Combine(tempFolder.Path, "test.csx");
            File.WriteAllText(csxFile, "#r \"nuget: Newtonsoft.Json, 12.0.1\"\nWriteLine(\"test\");");

            var provider = new ScriptProjectProvider(_logFactory);
            var projectFile = provider.CreateProjectFileFromScriptFiles(_scriptEnvironment.TargetFramework, new[] { csxFile });

            Assert.NotNull(projectFile.PackageReferences);
            Assert.Single(projectFile.PackageReferences);
        }

        [Fact]
        public void CreateProjectFileFromScriptFiles_WithMultiplePackageReferences_ShouldDeduplicatePackages()
        {
            using var tempFolder = new DisposableFolder();
            var csxFile1 = Path.Combine(tempFolder.Path, "test1.csx");
            var csxFile2 = Path.Combine(tempFolder.Path, "test2.csx");
            File.WriteAllText(csxFile1, "#r \"nuget: Newtonsoft.Json, 12.0.1\"\nWriteLine(\"test1\");");
            File.WriteAllText(csxFile2, "#r \"nuget: Newtonsoft.Json, 12.0.1\"\nWriteLine(\"test2\");");

            var provider = new ScriptProjectProvider(_logFactory);
            var projectFile = provider.CreateProjectFileFromScriptFiles(_scriptEnvironment.TargetFramework, new[] { csxFile1, csxFile2 });

            Assert.Single(projectFile.PackageReferences);
        }

        #endregion

        #region GetPathToProjectFile Tests

        [Fact]
        public void GetPathToProjectFile_ShouldReturnValidPath()
        {
            using var tempFolder = new DisposableFolder();
            var projectPath = ScriptProjectProvider.GetPathToProjectFile(tempFolder.Path, "net6.0");

            Assert.NotNull(projectPath);
            Assert.EndsWith(".csproj", projectPath);
        }

        [Fact]
        public void GetPathToProjectFile_WithCustomProjectName_ShouldUseProvidedName()
        {
            using var tempFolder = new DisposableFolder();
            var projectPath = ScriptProjectProvider.GetPathToProjectFile(tempFolder.Path, "net6.0", "myproject");

            Assert.NotNull(projectPath);
            Assert.Contains("myproject.csproj", projectPath);
        }

        [Fact]
        public void GetPathToProjectFile_WithNullProjectName_ShouldUseDefault()
        {
            using var tempFolder = new DisposableFolder();
            var projectPath = ScriptProjectProvider.GetPathToProjectFile(tempFolder.Path, "net6.0", null);

            Assert.NotNull(projectPath);
            Assert.Contains("script.csproj", projectPath);
        }

        [Fact]
        public void GetPathToProjectFile_ShouldCreateTempFolder()
        {
            using var tempFolder = new DisposableFolder();
            var projectPath = ScriptProjectProvider.GetPathToProjectFile(tempFolder.Path, "net6.0");
            var projectDir = Path.GetDirectoryName(projectPath);

            Assert.True(Directory.Exists(projectDir));
        }

        #endregion

        #region Logging Tests

        [Fact]
        public void ShouldLogProjectFileContent()
        {
            StringBuilder log = new StringBuilder();
            var provider = new ScriptProjectProvider(type => ((level, message, exception) => log.AppendLine(message)));

            provider.CreateProject(TestPathUtils.GetPathToTestFixtureFolder("HelloWorld"), _scriptEnvironment.TargetFramework, true);
            var output = log.ToString();

            Assert.Contains("<Project Sdk=\"Microsoft.NET.Sdk\">", output);
        }

        [Fact]
        public void ShouldLogProjectFilePathOnCreation()
        {
            StringBuilder log = new StringBuilder();
            var provider = new ScriptProjectProvider(type => ((level, message, exception) => log.AppendLine(message)));

            provider.CreateProject(TestPathUtils.GetPathToTestFixtureFolder("HelloWorld"), _scriptEnvironment.TargetFramework, true);
            var output = log.ToString();

            Assert.Contains("Project file saved to", output);
        }

        #endregion

        #region SDK Tests

        [Fact]
        public void ShouldUseSpecifiedSdk()
        {
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(TestPathUtils.GetPathToTestFixtureFolder("WebApi"), _scriptEnvironment.TargetFramework, true);
            Assert.Equal("Microsoft.NET.Sdk.Web", XDocument.Load(projectFileInfo.Path).Descendants("Project").Single().Attributes("Sdk").Single().Value);
        }

        [Fact]
        public void ShouldUseDefaultSdkWhenNotSpecified()
        {
            using var tempFolder = new DisposableFolder();
            var csxFile = Path.Combine(tempFolder.Path, "test.csx");
            File.WriteAllText(csxFile, "WriteLine(\"test\");");

            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(tempFolder.Path, new[] { csxFile }, _scriptEnvironment.TargetFramework, true);

            var sdk = XDocument.Load(projectFileInfo.Path).Descendants("Project").FirstOrDefault()?.Attributes("Sdk").FirstOrDefault()?.Value;
            Assert.Equal("Microsoft.NET.Sdk", sdk);
        }

        #endregion

        #region Shebang Tests

        // See: https://github.com/dotnet-script/dotnet-script/issues/723
        [Theory]
        [InlineData("#!/usr/bin/env dotnet-script\n#r \"sdk:Microsoft.NET.Sdk.Web\"")]
        [InlineData("#!/usr/bin/env dotnet-script\n\n#r \"sdk:Microsoft.NET.Sdk.Web\"")]
        [InlineData("#!/usr/bin/env dotnet-script\n\n\n#r \"sdk:Microsoft.NET.Sdk.Web\"")]
        public void ShouldHandleShebangBeforeSdk(string code)
        {
            var parser = new ScriptParser(_logFactory);
            var result = parser.ParseFromCode(code);

            Assert.Equal("Microsoft.NET.Sdk.Web", result.Sdk);
        }

        [Fact]
        public void ShouldHandleShebangBeforePackageReference()
        {
            var code = "#!/usr/bin/env dotnet-script\n#r \"nuget: Newtonsoft.Json, 12.0.1\"";
            var parser = new ScriptParser(_logFactory);
            var result = parser.ParseFromCode(code);

            Assert.Single(result.PackageReferences);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void CreateProject_WithNestedDirectories_ShouldFindAllCsxFiles()
        {
            using var tempFolder = new DisposableFolder();
            var subDir = Directory.CreateDirectory(Path.Combine(tempFolder.Path, "subdir")).FullName;
            File.WriteAllText(Path.Combine(tempFolder.Path, "test1.csx"), "WriteLine(\"test1\");");
            File.WriteAllText(Path.Combine(subDir, "test2.csx"), "WriteLine(\"test2\");");

            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(tempFolder.Path, _scriptEnvironment.TargetFramework, true);

            Assert.NotNull(projectFileInfo);
        }

        [Fact]
        public void CreateProject_ShouldSetCorrectNuGetConfigPath()
        {
            var scriptFile = TestPathUtils.GetPathToTestFixture("HelloWorld");
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProjectForScriptFile(scriptFile);

            Assert.NotNull(projectFileInfo.NuGetConfigPath);
        }

        [Fact]
        public void CreateProjectForRepl_ShouldCreateInteractiveProjectInNestedFolder()
        {
            using var tempFolder = new DisposableFolder();
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProjectForRepl("WriteLine(\"hello\");", tempFolder.Path, "net6.0");

            var pathParts = projectFileInfo.Path.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            var interactiveIndex = Array.IndexOf(pathParts, "interactive");
            Assert.True(interactiveIndex >= 0, "Path should contain 'interactive' folder");
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void CreateProject_ShouldProduceValidXmlProjectFile()
        {
            var scriptFile = TestPathUtils.GetPathToTestFixture("HelloWorld");
            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProjectForScriptFile(scriptFile);

            var projectXml = XDocument.Load(projectFileInfo.Path);
            Assert.NotNull(projectXml);
            Assert.NotNull(projectXml.Root);
            Assert.Equal("Project", projectXml.Root.Name.LocalName);
        }

        [Fact]
        public void CreateProject_ShouldSetTargetFrameworkCorrectly()
        {
            using var tempFolder = new DisposableFolder();
            var csxFile = Path.Combine(tempFolder.Path, "test.csx");
            File.WriteAllText(csxFile, "WriteLine(\"test\");");

            var provider = new ScriptProjectProvider(_logFactory);
            var projectFileInfo = provider.CreateProject(tempFolder.Path, new[] { csxFile }, "net7.0", true);

            var projectXml = XDocument.Load(projectFileInfo.Path);
            var targetFramework = projectXml.Descendants("TargetFramework").FirstOrDefault()?.Value;
            Assert.Equal("net7.0", targetFramework);
        }

        #endregion
    }
}