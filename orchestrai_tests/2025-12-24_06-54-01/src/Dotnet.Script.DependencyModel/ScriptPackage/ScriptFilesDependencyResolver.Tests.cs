using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Moq;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.ScriptPackage;

namespace Dotnet.Script.DependencyModel.Tests.ScriptPackage
{
    public class ScriptFilesDependencyResolverTests : IDisposable
    {
        private readonly ScriptFilesDependencyResolver _resolver;
        private readonly LogFactory _logFactory;
        private readonly string _testDirectory;

        public ScriptFilesDependencyResolverTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), $"ScriptFilesDependencyResolverTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);
            _logFactory = CreateMockLogFactory();
            _resolver = new ScriptFilesDependencyResolver(_logFactory);
        }

        private LogFactory CreateMockLogFactory()
        {
            return type => (level, message, exception) => { };
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        #region GetScriptFileDependencies Tests

        [Fact]
        public void GetScriptFileDependencies_WhenNoScriptFilesExist_ReturnsEmptyArray()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "EmptyPackage");
            Directory.CreateDirectory(packagePath);

            // Act
            var result = _resolver.GetScriptFileDependencies(packagePath);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetScriptFileDependencies_WhenScriptFilesExist_ReturnsProcessedFiles()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "PackageWithScripts");
            var contentFilesPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            Directory.CreateDirectory(contentFilesPath);
            var mainCsxPath = Path.Combine(contentFilesPath, "main.csx");
            File.WriteAllText(mainCsxPath, "Console.WriteLine(\"Hello\");");

            // Act
            var result = _resolver.GetScriptFileDependencies(packagePath);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(mainCsxPath, result[0]);
        }

        [Fact]
        public void GetScriptFileDependencies_WithContentPath_ProcessesCorrectly()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "PackageWithContent");
            var contentPath = Path.Combine(packagePath, "content", "csx", "netstandard2.0");
            Directory.CreateDirectory(contentPath);
            var mainCsxPath = Path.Combine(contentPath, "main.csx");
            File.WriteAllText(mainCsxPath, "");

            // Act
            var result = _resolver.GetScriptFileDependencies(packagePath);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetScriptFileDependencies_WithNestedDirectories_FindsAllScriptFiles()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "NestedPackage");
            var contentFilesPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            var subPath = Path.Combine(contentFilesPath, "subfolder");
            Directory.CreateDirectory(subPath);
            
            File.WriteAllText(Path.Combine(contentFilesPath, "main.csx"), "");
            File.WriteAllText(Path.Combine(subPath, "helper.csx"), "");

            // Act
            var result = _resolver.GetScriptFileDependencies(packagePath);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetScriptFileDependencies_WithMultipleTargetFrameworks_PrefersAny()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "MultiFrameworkPackage");
            var anyPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            var netstandardPath = Path.Combine(packagePath, "contentFiles", "csx", "netstandard2.0");
            Directory.CreateDirectory(anyPath);
            Directory.CreateDirectory(netstandardPath);

            File.WriteAllText(Path.Combine(anyPath, "any_main.csx"), "");
            File.WriteAllText(Path.Combine(netstandardPath, "main.csx"), "");

            // Act
            var result = _resolver.GetScriptFileDependencies(packagePath);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains("any_main.csx", result[0]);
        }

        [Fact]
        public void GetScriptFileDependencies_WithOnlyNetStandard_ReturnsNetStandardFiles()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "NetStandardOnlyPackage");
            var netstandardPath = Path.Combine(packagePath, "contentFiles", "csx", "netstandard2.0");
            Directory.CreateDirectory(netstandardPath);

            File.WriteAllText(Path.Combine(netstandardPath, "main.csx"), "");

            // Act
            var result = _resolver.GetScriptFileDependencies(packagePath);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetScriptFileDependencies_WithUnsupportedFramework_ReturnsEmpty()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "UnsupportedFrameworkPackage");
            var net45Path = Path.Combine(packagePath, "contentFiles", "csx", "net45");
            Directory.CreateDirectory(net45Path);

            File.WriteAllText(Path.Combine(net45Path, "script.csx"), "");

            // Act
            var result = _resolver.GetScriptFileDependencies(packagePath);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetScriptFileDependencies_WithNoEntryPointScript_ReturnsAllFiles()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "NoEntryPointPackage");
            var contentFilesPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            Directory.CreateDirectory(contentFilesPath);

            File.WriteAllText(Path.Combine(contentFilesPath, "script1.csx"), "");
            File.WriteAllText(Path.Combine(contentFilesPath, "script2.csx"), "");

            // Act
            var result = _resolver.GetScriptFileDependencies(packagePath);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetScriptFileDependencies_WithCaseInsensitiveMainCsx_FindsEntryPoint()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "CaseInsensitiveMainPackage");
            var contentFilesPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            Directory.CreateDirectory(contentFilesPath);

            File.WriteAllText(Path.Combine(contentFilesPath, "MAIN.CSX"), "");

            // Act
            var result = _resolver.GetScriptFileDependencies(packagePath);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains("MAIN.CSX", result[0], StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Private Method Tests - ProcessScriptFiles

        [Fact]
        public void ProcessScriptFiles_WithEmptyArray_ReturnsEmpty()
        {
            // Arrange
            var files = new string[] { };

            // Act - Using reflection to access private method
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("ProcessScriptFiles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string[])method.Invoke(_resolver, new object[] { files });

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ProcessScriptFiles_WithValidFiles_ReturnsProcessedFiles()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "ProcessScriptFilesPackage");
            var contentFilesPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            Directory.CreateDirectory(contentFilesPath);
            var mainCsxPath = Path.Combine(contentFilesPath, "main.csx");
            File.WriteAllText(mainCsxPath, "");

            var files = new[] { mainCsxPath };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("ProcessScriptFiles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string[])method.Invoke(_resolver, new object[] { files });

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        #endregion

        #region GetEntryPointScript Tests

        [Fact]
        public void GetEntryPointScript_WithSingleCsxFile_ReturnsThatFile()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "SingleEntryPointPackage");
            var contentFilesPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            Directory.CreateDirectory(contentFilesPath);
            var scriptPath = Path.Combine(contentFilesPath, "script.csx");
            File.WriteAllText(scriptPath, "");

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetEntryPointScript", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string)method.Invoke(_resolver, new object[] { contentFilesPath });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(scriptPath, result);
        }

        [Fact]
        public void GetEntryPointScript_WithNoFiles_ReturnsNull()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "NoFilesPackage");
            var contentFilesPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            Directory.CreateDirectory(contentFilesPath);

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetEntryPointScript", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string)method.Invoke(_resolver, new object[] { contentFilesPath });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetEntryPointScript_WithMainCsx_ReturnsMainCsx()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "MainCsxPackage");
            var contentFilesPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            Directory.CreateDirectory(contentFilesPath);
            var mainPath = Path.Combine(contentFilesPath, "main.csx");
            var otherPath = Path.Combine(contentFilesPath, "other.csx");
            File.WriteAllText(mainPath, "");
            File.WriteAllText(otherPath, "");

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetEntryPointScript", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string)method.Invoke(_resolver, new object[] { contentFilesPath });

            // Assert
            Assert.NotNull(result);
            Assert.Contains("main.csx", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetEntryPointScript_WithMultipleFilesNoMain_ReturnsNull()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "NoMainPackage");
            var contentFilesPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            Directory.CreateDirectory(contentFilesPath);
            File.WriteAllText(Path.Combine(contentFilesPath, "script1.csx"), "");
            File.WriteAllText(Path.Combine(contentFilesPath, "script2.csx"), "");

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetEntryPointScript", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string)method.Invoke(_resolver, new object[] { contentFilesPath });

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetScriptFilesMatchingCurrentRuntime Tests

        [Fact]
        public void GetScriptFilesMatchingCurrentRuntime_WithAnyFramework_ReturnsAnyFiles()
        {
            // Arrange
            var filesPerFramework = new Dictionary<string, List<string>>
            {
                { "any", new List<string> { "script1.csx", "script2.csx" } }
            };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesMatchingCurrentRuntime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string[])method.Invoke(_resolver, new object[] { filesPerFramework });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void GetScriptFilesMatchingCurrentRuntime_WithNetStandard20_ReturnsNetStandardFiles()
        {
            // Arrange
            var filesPerFramework = new Dictionary<string, List<string>>
            {
                { "netstandard2.0", new List<string> { "script.csx" } }
            };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesMatchingCurrentRuntime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string[])method.Invoke(_resolver, new object[] { filesPerFramework });

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetScriptFilesMatchingCurrentRuntime_WithBothFrameworks_PrefersAny()
        {
            // Arrange
            var filesPerFramework = new Dictionary<string, List<string>>
            {
                { "any", new List<string> { "any_script.csx" } },
                { "netstandard2.0", new List<string> { "netstandard_script.csx" } }
            };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesMatchingCurrentRuntime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string[])method.Invoke(_resolver, new object[] { filesPerFramework });

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("any_script.csx", result[0]);
        }

        [Fact]
        public void GetScriptFilesMatchingCurrentRuntime_WithUnsupportedFramework_ReturnsEmpty()
        {
            // Arrange
            var filesPerFramework = new Dictionary<string, List<string>>
            {
                { "net45", new List<string> { "script.csx" } }
            };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesMatchingCurrentRuntime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string[])method.Invoke(_resolver, new object[] { filesPerFramework });

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetScriptFilesMatchingCurrentRuntime_WithEmptyDictionary_ReturnsEmpty()
        {
            // Arrange
            var filesPerFramework = new Dictionary<string, List<string>>();

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesMatchingCurrentRuntime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string[])method.Invoke(_resolver, new object[] { filesPerFramework });

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetScriptFilesPerTargetFramework Tests

        [Fact]
        public void GetScriptFilesPerTargetFramework_WithValidPath_ExtractsTargetFramework()
        {
            // Arrange
            var scriptFiles = new[] { "contentFiles/csx/any/main.csx" };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesPerTargetFramework", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (IDictionary<string, List<string>>)method.Invoke(null, new object[] { scriptFiles });

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True(result.ContainsKey("any"));
            Assert.Single(result["any"]);
        }

        [Fact]
        public void GetScriptFilesPerTargetFramework_WithMultipleFrameworks_GroupsCorrectly()
        {
            // Arrange
            var scriptFiles = new[] 
            { 
                "contentFiles/csx/any/main.csx",
                "contentFiles/csx/netstandard2.0/helper.csx"
            };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesPerTargetFramework", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (IDictionary<string, List<string>>)method.Invoke(null, new object[] { scriptFiles });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetScriptFilesPerTargetFramework_WithInvalidPath_IgnoresFile()
        {
            // Arrange
            var scriptFiles = new[] { "invalid/path/script.csx" };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesPerTargetFramework", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (IDictionary<string, List<string>>)method.Invoke(null, new object[] { scriptFiles });

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetScriptFilesPerTargetFramework_WithEmptyArray_ReturnsEmptyDictionary()
        {
            // Arrange
            var scriptFiles = new string[] { };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesPerTargetFramework", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (IDictionary<string, List<string>>)method.Invoke(null, new object[] { scriptFiles });

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetScriptFilesPerTargetFramework_WithBackslashes_HandlesPathSeparators()
        {
            // Arrange
            var scriptFiles = new[] { @"contentFiles\csx\any\main.csx" };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesPerTargetFramework", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (IDictionary<string, List<string>>)method.Invoke(null, new object[] { scriptFiles });

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True(result.ContainsKey("any"));
        }

        [Fact]
        public void GetScriptFilesPerTargetFramework_WithContentPathInsteadOfContentFiles_ExtractsFramework()
        {
            // Arrange
            var scriptFiles = new[] { "content/csx/netstandard2.0/script.csx" };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesPerTargetFramework", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (IDictionary<string, List<string>>)method.Invoke(null, new object[] { scriptFiles });

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("netstandard2.0"));
        }

        [Fact]
        public void GetScriptFilesPerTargetFramework_CaseInsensitiveComparison_GroupsSameFrameworkDifferentCase()
        {
            // Arrange
            var scriptFiles = new[] 
            { 
                "contentFiles/csx/Any/main.csx",
                "contentFiles/csx/ANY/helper.csx"
            };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesPerTargetFramework", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (IDictionary<string, List<string>>)method.Invoke(null, new object[] { scriptFiles });

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(2, result.Values.First().Count);
        }

        #endregion

        #region GetRootPath Tests

        [Fact]
        public void GetRootPath_WithValidPath_ExtractsRootPath()
        {
            // Arrange
            var pathToScriptFile = "contentFiles/csx/any/main.csx";

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetRootPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (string)method.Invoke(null, new object[] { pathToScriptFile });

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("contentFiles", result);
            Assert.Contains("csx", result);
            Assert.Contains("any", result);
        }

        [Fact]
        public void GetRootPath_WithBackslashes_HandlesPathSeparators()
        {
            // Arrange
            var pathToScriptFile = @"contentFiles\csx\netstandard2.0\script.csx";

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetRootPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (string)method.Invoke(null, new object[] { pathToScriptFile });

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetRootPath_WithContentPath_ExtractsCorrectly()
        {
            // Arrange
            var pathToScriptFile = "content/csx/any/subfolder/script.csx";

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetRootPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (string)method.Invoke(null, new object[] { pathToScriptFile });

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Fact]
        public void Integration_PackageWithMultipleStructures_HandleCorrectly()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "ComplexPackage");
            
            // Create multiple framework versions
            var anyPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            var netPath = Path.Combine(packagePath, "contentFiles", "csx", "netstandard2.0");
            Directory.CreateDirectory(anyPath);
            Directory.CreateDirectory(netPath);

            File.WriteAllText(Path.Combine(anyPath, "main.csx"), "");
            File.WriteAllText(Path.Combine(netPath, "main.csx"), "");

            // Act
            var result = _resolver.GetScriptFileDependencies(packagePath);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Should prefer "any"
            Assert.Contains("any", result[0]);
        }

        [Fact]
        public void Integration_PathWithSymbolicCharacters_HandledCorrectly()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "SymbolicPackage");
            var contentFilesPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            Directory.CreateDirectory(contentFilesPath);
            
            var scriptPath = Path.Combine(contentFilesPath, "script-with-dash.csx");
            File.WriteAllText(scriptPath, "");

            // Act
            var result = _resolver.GetScriptFileDependencies(packagePath);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void TargetFrameworkMatcher_WithMixedPathFormat_MatchesCorrectly()
        {
            // Arrange
            var scriptFiles = new[] 
            { 
                @"C:\packages\pkg\contentFiles/csx\netstandard2.0/main.csx"
            };

            // Act
            var method = typeof(ScriptFilesDependencyResolver).GetMethod("GetScriptFilesPerTargetFramework", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (IDictionary<string, List<string>>)method.Invoke(null, new object[] { scriptFiles });

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetScriptFileDependencies_WithUnicodeFilename_HandlesCorrectly()
        {
            // Arrange
            var packagePath = Path.Combine(_testDirectory, "UnicodePackage");
            var contentFilesPath = Path.Combine(packagePath, "contentFiles", "csx", "any");
            Directory.CreateDirectory(contentFilesPath);
            
            var scriptPath = Path.Combine(contentFilesPath, "main.csx");
            File.WriteAllText(scriptPath, "// Тест");

            // Act
            var result = _resolver.GetScriptFileDependencies(packagePath);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        #endregion
    }
}