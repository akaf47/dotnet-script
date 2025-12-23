using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Moq;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Environment;

namespace Dotnet.Script.DependencyModel.Tests.ProjectSystem
{
    public class ScriptProjectProviderTests
    {
        private readonly Mock<ScriptParser> _mockScriptParser;
        private readonly Mock<ScriptFilesResolver> _mockScriptFilesResolver;
        private readonly Mock<LogFactory> _mockLogFactory;
        private readonly Mock<Logger> _mockLogger;
        private ScriptProjectProvider _provider;

        public ScriptProjectProviderTests()
        {
            _mockLogger = new Mock<Logger>();
            _mockLogFactory = new Mock<LogFactory>();
            _mockLogFactory.Setup(f => f(It.IsAny<Type>())).Returns((LogLevel level, string message, Exception ex) =>
            {
                _mockLogger.Object(level, message, ex);
            });

            _mockScriptParser = new Mock<ScriptParser>(_mockLogFactory.Object);
            _mockScriptFilesResolver = new Mock<ScriptFilesResolver>();
            
            _provider = new ScriptProjectProvider(_mockLogFactory.Object);
        }

        #region CreateProjectForRepl Tests

        [Fact]
        public void CreateProjectForRepl_WithValidCode_ReturnsProjectFileInfo()
        {
            // Arrange
            var code = "#r \"nuget: SomePackage, 1.0.0\"";
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(targetDirectory);
            
            try
            {
                var parseResult = new ParseResult(new[] 
                { 
                    new PackageReference("SomePackage", "1.0.0") 
                });

                using var provider = CreateProviderWithMocks(parseResult, parseResult, new HashSet<string>());
                
                // Act
                var result = provider.CreateProjectForRepl(code, targetDirectory);

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(result.Path);
                Assert.True(File.Exists(result.Path));
                Assert.True(result.Path.Contains("interactive"));
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProjectForRepl_WithEmptyCode_ReturnsProjectFileInfo()
        {
            // Arrange
            var code = string.Empty;
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(targetDirectory);

            try
            {
                var parseResult = new ParseResult(new PackageReference[0]);

                using var provider = CreateProviderWithMocks(parseResult, parseResult, new HashSet<string>());

                // Act
                var result = provider.CreateProjectForRepl(code, targetDirectory);

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(result.Path);
                Assert.True(File.Exists(result.Path));
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProjectForRepl_WithDefaultTargetFramework_UsesProvidedFramework()
        {
            // Arrange
            var code = "#r \"nuget: Package, 1.0\"";
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var customFramework = "net60";
            Directory.CreateDirectory(targetDirectory);

            try
            {
                var parseResult = new ParseResult(new[] { new PackageReference("Package", "1.0") });

                using var provider = CreateProviderWithMocks(parseResult, parseResult, new HashSet<string>());

                // Act
                var result = provider.CreateProjectForRepl(code, targetDirectory, customFramework);

                // Assert
                Assert.NotNull(result);
                var content = File.ReadAllText(result.Path);
                Assert.Contains(customFramework, content);
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProjectForRepl_WithSdk_IncludesSdkInProjectFile()
        {
            // Arrange
            var code = "#r \"nuget: WebPackage, 1.0\"";
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(targetDirectory);

            try
            {
                var parseResult = new ParseResult(new[] { new PackageReference("WebPackage", "1.0") });
                parseResult.Sdk = "Microsoft.NET.Sdk.Web";

                using var provider = CreateProviderWithMocks(parseResult, parseResult, new HashSet<string>());

                // Act
                var result = provider.CreateProjectForRepl(code, targetDirectory);

                // Assert
                Assert.NotNull(result);
                var content = File.ReadAllText(result.Path);
                Assert.Contains("Microsoft.NET.Sdk.Web", content);
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProjectForRepl_WithLoadedScriptFiles_MergesPackageReferences()
        {
            // Arrange
            var code = "#r \"nuget: Package1, 1.0\"";
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(targetDirectory);

            try
            {
                var codeParseResult = new ParseResult(new[] { new PackageReference("Package1", "1.0") });
                var filesParseResult = new ParseResult(new[] { new PackageReference("Package2", "2.0") });
                var scriptFiles = new HashSet<string> { "/path/to/script.csx" };

                using var provider = CreateProviderWithMocks(codeParseResult, filesParseResult, scriptFiles);

                // Act
                var result = provider.CreateProjectForRepl(code, targetDirectory);

                // Assert
                Assert.NotNull(result);
                var content = File.ReadAllText(result.Path);
                Assert.Contains("Package1", content);
                Assert.Contains("Package2", content);
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProjectForRepl_WithDuplicatePackages_DeduplicatesReferences()
        {
            // Arrange
            var code = "#r \"nuget: SamePackage, 1.0\"";
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(targetDirectory);

            try
            {
                var codeParseResult = new ParseResult(new[] { new PackageReference("SamePackage", "1.0") });
                var filesParseResult = new ParseResult(new[] { new PackageReference("SamePackage", "1.0") });

                using var provider = CreateProviderWithMocks(codeParseResult, filesParseResult, new HashSet<string>());

                // Act
                var result = provider.CreateProjectForRepl(code, targetDirectory);

                // Assert
                Assert.NotNull(result);
                var content = File.ReadAllText(result.Path);
                var packageCount = content.Split("SamePackage", StringSplitOptions.None).Length - 1;
                Assert.Equal(1, packageCount / 2); // Each package appears twice in XML (Include and Version)
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        #endregion

        #region CreateProject Tests

        [Fact]
        public void CreateProject_WithValidTargetDirectory_ReturnsProjectFileInfo()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var scriptFile = Path.Combine(targetDirectory, "test.csx");
            Directory.CreateDirectory(targetDirectory);
            File.WriteAllText(scriptFile, "#r \"nuget: Package, 1.0\"");

            try
            {
                var parseResult = new ParseResult(new[] { new PackageReference("Package", "1.0") });
                
                using var provider = CreateProviderWithMocks(parseResult, new PackageReference[0], new HashSet<string>());

                // Act
                var result = provider.CreateProject(targetDirectory);

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(result.Path);
                Assert.True(File.Exists(result.Path));
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProject_WithNoScriptFiles_ReturnsNull()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(targetDirectory);

            try
            {
                using var provider = CreateProviderWithMocks(
                    new ParseResult(new PackageReference[0]), 
                    new ParseResult(new PackageReference[0]), 
                    new HashSet<string>());

                // Act
                var result = provider.CreateProject(targetDirectory);

                // Assert
                Assert.Null(result);
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProject_WithNullScriptFiles_ReturnsNull()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(targetDirectory);

            try
            {
                using var provider = CreateProviderWithMocks(
                    new ParseResult(new PackageReference[0]), 
                    new ParseResult(new PackageReference[0]), 
                    new HashSet<string>());

                // Act
                var result = provider.CreateProject(targetDirectory, null);

                // Assert
                Assert.Null(result);
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProject_WithEmptyScriptFilesCollection_ReturnsNull()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(targetDirectory);

            try
            {
                using var provider = CreateProviderWithMocks(
                    new ParseResult(new PackageReference[0]), 
                    new ParseResult(new PackageReference[0]), 
                    new HashSet<string>());

                // Act
                var result = provider.CreateProject(targetDirectory, new string[0]);

                // Assert
                Assert.Null(result);
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProject_WithNoCsproj_AndNuGetScriptReferencesDisabled_ReturnsNull()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var scriptFile = Path.Combine(targetDirectory, "test.csx");
            Directory.CreateDirectory(targetDirectory);
            File.WriteAllText(scriptFile, "");

            try
            {
                using var provider = CreateProviderWithMocks(
                    new ParseResult(new PackageReference[0]), 
                    new ParseResult(new PackageReference[0]), 
                    new HashSet<string>());

                // Act
                var result = provider.CreateProject(targetDirectory, new[] { scriptFile }, "net46", false);

                // Assert
                Assert.Null(result);
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProject_WithScriptFilesEnumerable_ReturnsProjectFileInfo()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var scriptFile = Path.Combine(targetDirectory, "test.csx");
            Directory.CreateDirectory(targetDirectory);
            File.WriteAllText(scriptFile, "#r \"nuget: Package, 1.0\"");

            try
            {
                var parseResult = new ParseResult(new[] { new PackageReference("Package", "1.0") });
                var scriptFiles = new List<string> { scriptFile };

                using var provider = CreateProviderWithMocks(
                    parseResult, 
                    parseResult, 
                    new HashSet<string>());

                // Act
                var result = provider.CreateProject(targetDirectory, scriptFiles);

                // Assert
                Assert.NotNull(result);
                Assert.True(File.Exists(result.Path));
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProject_WithEnableNuGetScriptReferences_SkipsCsprojCheck()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var scriptFile = Path.Combine(targetDirectory, "test.csx");
            Directory.CreateDirectory(targetDirectory);
            File.WriteAllText(scriptFile, "#r \"nuget: Package, 1.0\"");

            try
            {
                var parseResult = new ParseResult(new[] { new PackageReference("Package", "1.0") });

                using var provider = CreateProviderWithMocks(
                    parseResult, 
                    parseResult, 
                    new HashSet<string>());

                // Act
                var result = provider.CreateProject(targetDirectory, new[] { scriptFile }, "net46", true);

                // Assert
                Assert.NotNull(result);
                Assert.True(File.Exists(result.Path));
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        #endregion

        #region CreateProjectForScriptFile Tests

        [Fact]
        public void CreateProjectForScriptFile_WithValidFile_ReturnsProjectFileInfo()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var scriptFile = Path.Combine(targetDirectory, "test.csx");
            Directory.CreateDirectory(targetDirectory);
            File.WriteAllText(scriptFile, "#r \"nuget: Package, 1.0\"");

            try
            {
                var parseResult = new ParseResult(new[] { new PackageReference("Package", "1.0") });

                using var provider = CreateProviderWithMocks(
                    parseResult, 
                    parseResult, 
                    new HashSet<string> { scriptFile });

                // Act
                var result = provider.CreateProjectForScriptFile(scriptFile);

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(result.Path);
                Assert.True(File.Exists(result.Path));
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProjectForScriptFile_WithFileInSubdirectory_CreatesProjectInCorrectLocation()
        {
            // Arrange
            var baseDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var subDirectory = Path.Combine(baseDirectory, "subfolder");
            Directory.CreateDirectory(subDirectory);
            var scriptFile = Path.Combine(subDirectory, "test.csx");
            File.WriteAllText(scriptFile, "");

            try
            {
                var parseResult = new ParseResult(new PackageReference[0]);

                using var provider = CreateProviderWithMocks(
                    parseResult, 
                    parseResult, 
                    new HashSet<string> { scriptFile });

                // Act
                var result = provider.CreateProjectForScriptFile(scriptFile);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.Path.Contains(subDirectory));
            }
            finally
            {
                CleanupDirectory(baseDirectory);
            }
        }

        #endregion

        #region CreateProjectFileFromScriptFiles Tests

        [Fact]
        public void CreateProjectFileFromScriptFiles_WithValidFiles_ReturnsProjectFile()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var scriptFile = Path.Combine(targetDirectory, "test.csx");
            Directory.CreateDirectory(targetDirectory);
            File.WriteAllText(scriptFile, "#r \"nuget: Package, 1.0\"");

            try
            {
                var parseResult = new ParseResult(new[] { new PackageReference("Package", "1.0") });

                using var provider = CreateProviderWithMocks(
                    parseResult, 
                    parseResult, 
                    new HashSet<string>());

                // Act
                var projectFile = provider.CreateProjectFileFromScriptFiles("net46", new[] { scriptFile });

                // Assert
                Assert.NotNull(projectFile);
                Assert.Contains(projectFile.PackageReferences, p => p.Id.Value == "Package");
                Assert.Equal("net46", projectFile.TargetFramework);
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProjectFileFromScriptFiles_WithSdk_SetsSdkProperty()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var scriptFile = Path.Combine(targetDirectory, "test.csx");
            Directory.CreateDirectory(targetDirectory);
            File.WriteAllText(scriptFile, "");

            try
            {
                var parseResult = new ParseResult(new PackageReference[0]);
                parseResult.Sdk = "Microsoft.NET.Sdk.Web";

                using var provider = CreateProviderWithMocks(
                    parseResult, 
                    parseResult, 
                    new HashSet<string>());

                // Act
                var projectFile = provider.CreateProjectFileFromScriptFiles("net46", new[] { scriptFile });

                // Assert
                Assert.NotNull(projectFile);
                Assert.Equal("Microsoft.NET.Sdk.Web", projectFile.Sdk);
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        [Fact]
        public void CreateProjectFileFromScriptFiles_WithMultiplePackages_IncludesAllReferences()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var scriptFile = Path.Combine(targetDirectory, "test.csx");
            Directory.CreateDirectory(targetDirectory);
            File.WriteAllText(scriptFile, "");

            try
            {
                var packages = new[]
                {
                    new PackageReference("Package1", "1.0"),
                    new PackageReference("Package2", "2.0"),
                    new PackageReference("Package3", "3.0")
                };
                var parseResult = new ParseResult(packages);

                using var provider = CreateProviderWithMocks(
                    parseResult, 
                    parseResult, 
                    new HashSet<string>());

                // Act
                var projectFile = provider.CreateProjectFileFromScriptFiles("net46", new[] { scriptFile });

                // Assert
                Assert.NotNull(projectFile);
                Assert.Equal(3, projectFile.PackageReferences.Count);
                Assert.Contains(projectFile.PackageReferences, p => p.Id.Value == "Package1");
                Assert.Contains(projectFile.PackageReferences, p => p.Id.Value == "Package2");
                Assert.Contains(projectFile.PackageReferences, p => p.Id.Value == "Package3");
            }
            finally
            {
                CleanupDirectory(targetDirectory);
            }
        }

        #endregion

        #region GetPathToProjectFile Tests

        [Fact]
        public void GetPathToProjectFile_WithValidDirectory_ReturnsValidPath()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var framework = "net46";

            // Act
            var path = ScriptProjectProvider.GetPathToProjectFile(targetDirectory, framework);

            // Assert
            Assert.NotNull(path);
            Assert.True(path.EndsWith("script.csproj"));
        }

        [Fact]
        public void GetPathToProjectFile_WithCustomProjectName_UsesProvidedName()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var framework = "net46";
            var projectName = "myproject";

            // Act
            var path = ScriptProjectProvider.GetPathToProjectFile(targetDirectory, framework, projectName);

            // Assert
            Assert.NotNull(path);
            Assert.True(path.EndsWith("myproject.csproj"));
        }

        [Fact]
        public void GetPathToProjectFile_WithNullProjectName_UsesDefaultName()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var framework = "net46";

            // Act
            var path = ScriptProjectProvider.GetPathToProjectFile(targetDirectory, framework, null);

            // Assert
            Assert.NotNull(path);
            Assert.True(path.EndsWith("script.csproj"));
        }

        [Fact]
        public void GetPathToProjectFile_WithDifferentFrameworks_CreatesFrameworkSpecificPaths()
        {
            // Arrange
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            // Act
            var path46 = ScriptProjectProvider.GetPathToProjectFile(targetDirectory, "net46");
            var path60 = ScriptProjectProvider.GetPathToProjectFile(targetDirectory, "net60");

            // Assert
            Assert.NotEqual(path46, path60);
            Assert.Contains("net46", path46);
            Assert.Contains("net60", path60);
        }

        #endregion

        #region Private Helper Methods

        private ScriptProjectProvider CreateProviderWithMocks(
            ParseResult codeParseResult, 
            ParseResult filesParseResult, 
            HashSet<string> scriptFiles)
        {
            var mockParser = new Mock<ScriptParser>(_mockLogFactory.Object);
            mockParser.Setup(p => p.ParseFromCode(It.IsAny<string>()))
                .Returns(codeParseResult);
            mockParser.Setup(p => p.ParseFromFiles(It.IsAny<IEnumerable<string>>()))
                .Returns(filesParseResult);

            var mockResolver = new Mock<ScriptFilesResolver>();
            mockResolver.Setup(r => r.GetScriptFilesFromCode(It.IsAny<string>()))
                .Returns(scriptFiles);
            mockResolver.Setup(r => r.GetScriptFiles(It.IsAny<string>()))
                .Returns(scriptFiles);

            return new TestableScriptProjectProvider(
                mockParser.Object, 
                mockResolver.Object, 
                _mockLogFactory.Object, 
                ScriptEnvironment.Default);
        }

        private void CleanupDirectory(string directory)
        {
            try
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        #endregion

        // Testable version that allows passing custom mocks
        private class TestableScriptProjectProvider : ScriptProjectProvider
        {
            private readonly ScriptParser _scriptParser;
            private readonly ScriptFilesResolver _scriptFilesResolver;

            public TestableScriptProjectProvider(
                ScriptParser scriptParser,
                ScriptFilesResolver scriptFilesResolver,
                LogFactory logFactory,
                ScriptEnvironment scriptEnvironment)
                : base(logFactory)
            {
                _scriptParser = scriptParser;
                _scriptFilesResolver = scriptFilesResolver;
            }
        }
    }
}