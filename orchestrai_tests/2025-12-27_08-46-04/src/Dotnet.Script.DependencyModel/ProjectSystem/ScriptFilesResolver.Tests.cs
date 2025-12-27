using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Moq;
using Dotnet.Script.DependencyModel.ProjectSystem;

namespace Dotnet.Script.DependencyModel.ProjectSystem.Tests
{
    public class ScriptFilesResolverTests : IDisposable
    {
        private readonly string _testRoot;
        private ScriptFilesResolver _resolver;

        public ScriptFilesResolverTests()
        {
            _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testRoot);
            _resolver = new ScriptFilesResolver();
        }

        public void Dispose()
        {
            if (Directory.Exists(_testRoot))
            {
                Directory.Delete(_testRoot, true);
            }
        }

        #region GetScriptFiles Tests

        [Fact]
        public void GetScriptFiles_WithValidCsxFile_ReturnsFile()
        {
            // Arrange
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(scriptFile, "// valid script");

            // Act
            var result = _resolver.GetScriptFiles(_testRoot);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(scriptFile, result.First());
        }

        [Fact]
        public void GetScriptFiles_WithMultipleCsxFiles_ReturnsAllFiles()
        {
            // Arrange
            var script1 = Path.Combine(_testRoot, "script1.csx");
            var script2 = Path.Combine(_testRoot, "script2.csx");
            File.WriteAllText(script1, "// script 1");
            File.WriteAllText(script2, "// script 2");

            // Act
            var result = _resolver.GetScriptFiles(_testRoot);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(script1, result);
            Assert.Contains(script2, result);
        }

        [Fact]
        public void GetScriptFiles_WithNestedDirectories_ReturnsFilesRecursively()
        {
            // Arrange
            var subDir = Path.Combine(_testRoot, "nested", "deep");
            Directory.CreateDirectory(subDir);
            var scriptFile = Path.Combine(subDir, "script.csx");
            File.WriteAllText(scriptFile, "// nested script");

            // Act
            var result = _resolver.GetScriptFiles(_testRoot);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(scriptFile, result.First());
        }

        [Fact]
        public void GetScriptFiles_WithNonCsxFiles_IgnoresNonScriptFiles()
        {
            // Arrange
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            var dllFile = Path.Combine(_testRoot, "library.dll");
            var csFile = Path.Combine(_testRoot, "code.cs");
            File.WriteAllText(scriptFile, "// script");
            File.WriteAllText(dllFile, "binary");
            File.WriteAllText(csFile, "// code");

            // Act
            var result = _resolver.GetScriptFiles(_testRoot);

            // Assert
            Assert.Single(result);
            Assert.Equal(scriptFile, result.First());
        }

        [Fact]
        public void GetScriptFiles_WithEmptyDirectory_ReturnsEmptyCollection()
        {
            // Arrange
            var emptyDir = Path.Combine(_testRoot, "empty");
            Directory.CreateDirectory(emptyDir);

            // Act
            var result = _resolver.GetScriptFiles(emptyDir);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetScriptFiles_WithNullPath_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _resolver.GetScriptFiles(null));
        }

        [Fact]
        public void GetScriptFiles_WithNonExistentPath_ThrowsDirectoryNotFoundException()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_testRoot, "nonexistent");

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => _resolver.GetScriptFiles(nonExistentPath));
        }

        [Fact]
        public void GetScriptFiles_WithEmptyString_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _resolver.GetScriptFiles(string.Empty));
        }

        [Fact]
        public void GetScriptFiles_WithWhitespaceOnlyPath_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _resolver.GetScriptFiles("   "));
        }

        [Fact]
        public void GetScriptFiles_WithRelativePath_ResolvesCorrectly()
        {
            // Arrange
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(scriptFile, "// script");
            var originalDir = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_testRoot);

                // Act
                var result = _resolver.GetScriptFiles(".");

                // Assert
                Assert.Single(result);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
            }
        }

        [Fact]
        public void GetScriptFiles_WithCaseInsensitiveExtension_ReturnsCsxRegardlessOfCase()
        {
            // Arrange
            var scriptFileLower = Path.Combine(_testRoot, "script.csx");
            var scriptFileUpper = Path.Combine(_testRoot, "SCRIPT.CSX");
            var scriptFileMixed = Path.Combine(_testRoot, "Script.CsX");
            File.WriteAllText(scriptFileLower, "// lower");
            File.WriteAllText(scriptFileUpper, "// upper");
            File.WriteAllText(scriptFileMixed, "// mixed");

            // Act
            var result = _resolver.GetScriptFiles(_testRoot);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        #endregion

        #region GetScriptFiles with SearchOption Tests

        [Fact]
        public void GetScriptFiles_WithSearchOptionAllDirectories_ReturnsAllFilesRecursively()
        {
            // Arrange
            var subDir = Path.Combine(_testRoot, "sub");
            Directory.CreateDirectory(subDir);
            var rootScript = Path.Combine(_testRoot, "root.csx");
            var subScript = Path.Combine(subDir, "sub.csx");
            File.WriteAllText(rootScript, "// root");
            File.WriteAllText(subScript, "// sub");

            // Act
            var result = _resolver.GetScriptFiles(_testRoot, SearchOption.AllDirectories);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(rootScript, result);
            Assert.Contains(subScript, result);
        }

        [Fact]
        public void GetScriptFiles_WithSearchOptionTopDirectoryOnly_ReturnsOnlyTopLevelFiles()
        {
            // Arrange
            var subDir = Path.Combine(_testRoot, "sub");
            Directory.CreateDirectory(subDir);
            var rootScript = Path.Combine(_testRoot, "root.csx");
            var subScript = Path.Combine(subDir, "sub.csx");
            File.WriteAllText(rootScript, "// root");
            File.WriteAllText(subScript, "// sub");

            // Act
            var result = _resolver.GetScriptFiles(_testRoot, SearchOption.TopDirectoryOnly);

            // Assert
            Assert.Single(result);
            Assert.Equal(rootScript, result.First());
        }

        #endregion

        #region ResolveScriptPath Tests

        [Fact]
        public void ResolveScriptPath_WithAbsolutePath_ReturnsAbsolutePath()
        {
            // Arrange
            var absolutePath = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(absolutePath, "// script");

            // Act
            var result = _resolver.ResolveScriptPath(absolutePath);

            // Assert
            Assert.Equal(absolutePath, result);
        }

        [Fact]
        public void ResolveScriptPath_WithRelativePath_ResolvesToAbsolutePath()
        {
            // Arrange
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(scriptFile, "// script");
            var originalDir = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_testRoot);

                // Act
                var result = _resolver.ResolveScriptPath("script.csx");

                // Assert
                Assert.Equal(scriptFile, result);
                Assert.True(Path.IsPathRooted(result));
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
            }
        }

        [Fact]
        public void ResolveScriptPath_WithNullPath_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _resolver.ResolveScriptPath(null));
        }

        [Fact]
        public void ResolveScriptPath_WithEmptyString_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _resolver.ResolveScriptPath(string.Empty));
        }

        [Fact]
        public void ResolveScriptPath_WithWhitespaceOnlyPath_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _resolver.ResolveScriptPath("   "));
        }

        [Fact]
        public void ResolveScriptPath_WithNonExistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var nonExistentFile = Path.Combine(_testRoot, "nonexistent.csx");

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _resolver.ResolveScriptPath(nonExistentFile));
        }

        [Fact]
        public void ResolveScriptPath_WithPathTraversal_ResolvesCorrectly()
        {
            // Arrange
            var subDir = Path.Combine(_testRoot, "sub");
            Directory.CreateDirectory(subDir);
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(scriptFile, "// script");
            var originalDir = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(subDir);

                // Act
                var result = _resolver.ResolveScriptPath("../script.csx");

                // Assert
                Assert.Equal(scriptFile, result);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
            }
        }

        #endregion

        #region FilterScriptFiles Tests

        [Fact]
        public void FilterScriptFiles_WithAllValidScriptFiles_ReturnsAllFiles()
        {
            // Arrange
            var files = new[]
            {
                Path.Combine(_testRoot, "script1.csx"),
                Path.Combine(_testRoot, "script2.csx")
            };
            foreach (var file in files)
            {
                File.WriteAllText(file, "// script");
            }

            // Act
            var result = _resolver.FilterScriptFiles(files);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(files, result);
        }

        [Fact]
        public void FilterScriptFiles_WithMixedFiles_ReturnsOnlyScriptFiles()
        {
            // Arrange
            var scriptFile1 = Path.Combine(_testRoot, "script1.csx");
            var dllFile = Path.Combine(_testRoot, "library.dll");
            var scriptFile2 = Path.Combine(_testRoot, "script2.csx");
            File.WriteAllText(scriptFile1, "// script");
            File.WriteAllText(dllFile, "binary");
            File.WriteAllText(scriptFile2, "// script");
            var files = new[] { scriptFile1, dllFile, scriptFile2 };

            // Act
            var result = _resolver.FilterScriptFiles(files);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(scriptFile1, result);
            Assert.Contains(scriptFile2, result);
            Assert.DoesNotContain(dllFile, result);
        }

        [Fact]
        public void FilterScriptFiles_WithNoScriptFiles_ReturnsEmptyCollection()
        {
            // Arrange
            var dllFile = Path.Combine(_testRoot, "library.dll");
            var csFile = Path.Combine(_testRoot, "code.cs");
            File.WriteAllText(dllFile, "binary");
            File.WriteAllText(csFile, "// code");
            var files = new[] { dllFile, csFile };

            // Act
            var result = _resolver.FilterScriptFiles(files);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FilterScriptFiles_WithNullCollection_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _resolver.FilterScriptFiles(null));
        }

        [Fact]
        public void FilterScriptFiles_WithEmptyCollection_ReturnsEmptyCollection()
        {
            // Arrange
            var files = Enumerable.Empty<string>();

            // Act
            var result = _resolver.FilterScriptFiles(files);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FilterScriptFiles_WithNullElementInCollection_SkipsNullElement()
        {
            // Arrange
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(scriptFile, "// script");
            var files = new[] { scriptFile, null };

            // Act
            var result = _resolver.FilterScriptFiles(files);

            // Assert
            Assert.Single(result);
            Assert.Equal(scriptFile, result.First());
        }

        #endregion

        #region ValidateScriptPath Tests

        [Fact]
        public void ValidateScriptPath_WithValidCsxFile_ReturnsTrue()
        {
            // Arrange
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(scriptFile, "// script");

            // Act
            var result = _resolver.ValidateScriptPath(scriptFile);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateScriptPath_WithNonExistentFile_ReturnsFalse()
        {
            // Arrange
            var nonExistentFile = Path.Combine(_testRoot, "nonexistent.csx");

            // Act
            var result = _resolver.ValidateScriptPath(nonExistentFile);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateScriptPath_WithNonCsxFile_ReturnsFalse()
        {
            // Arrange
            var csFile = Path.Combine(_testRoot, "code.cs");
            File.WriteAllText(csFile, "// code");

            // Act
            var result = _resolver.ValidateScriptPath(csFile);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateScriptPath_WithNullPath_ReturnsFalse()
        {
            // Act
            var result = _resolver.ValidateScriptPath(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateScriptPath_WithEmptyString_ReturnsFalse()
        {
            // Act
            var result = _resolver.ValidateScriptPath(string.Empty);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateScriptPath_WithWhitespaceOnlyPath_ReturnsFalse()
        {
            // Act
            var result = _resolver.ValidateScriptPath("   ");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetScriptDependencies Tests

        [Fact]
        public void GetScriptDependencies_WithValidScript_ReturnsEmptyListWhenNoDependencies()
        {
            // Arrange
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(scriptFile, "// no dependencies");

            // Act
            var result = _resolver.GetScriptDependencies(scriptFile);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetScriptDependencies_WithLoadDirective_ReturnsDependency()
        {
            // Arrange
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            var depFile = Path.Combine(_testRoot, "dependency.csx");
            File.WriteAllText(depFile, "// dependency");
            File.WriteAllText(scriptFile, "#load \"dependency.csx\"\n// main script");

            // Act
            var result = _resolver.GetScriptDependencies(scriptFile);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(depFile, result.First());
        }

        [Fact]
        public void GetScriptDependencies_WithMultipleLoadDirectives_ReturnsAllDependencies()
        {
            // Arrange
            var dep1 = Path.Combine(_testRoot, "dep1.csx");
            var dep2 = Path.Combine(_testRoot, "dep2.csx");
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(dep1, "// dep1");
            File.WriteAllText(dep2, "// dep2");
            File.WriteAllText(scriptFile, "#load \"dep1.csx\"\n#load \"dep2.csx\"\n// main");

            // Act
            var result = _resolver.GetScriptDependencies(scriptFile);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(dep1, result);
            Assert.Contains(dep2, result);
        }

        [Fact]
        public void GetScriptDependencies_WithRelativePath_ResolvesPathsCorrectly()
        {
            // Arrange
            var subDir = Path.Combine(_testRoot, "sub");
            Directory.CreateDirectory(subDir);
            var depFile = Path.Combine(_testRoot, "dependency.csx");
            var scriptFile = Path.Combine(subDir, "script.csx");
            File.WriteAllText(depFile, "// dep");
            File.WriteAllText(scriptFile, "#load \"../dependency.csx\"\n// main");

            // Act
            var result = _resolver.GetScriptDependencies(scriptFile);

            // Assert
            Assert.Single(result);
            Assert.Equal(depFile, result.First());
        }

        [Fact]
        public void GetScriptDependencies_WithNullPath_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _resolver.GetScriptDependencies(null));
        }

        [Fact]
        public void GetScriptDependencies_WithNonExistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var nonExistentFile = Path.Combine(_testRoot, "nonexistent.csx");

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _resolver.GetScriptDependencies(nonExistentFile));
        }

        [Fact]
        public void GetScriptDependencies_WithNonExistentDependency_ThrowsFileNotFoundException()
        {
            // Arrange
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(scriptFile, "#load \"nonexistent.csx\"\n// main");

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _resolver.GetScriptDependencies(scriptFile));
        }

        #endregion

        #region GetAllScriptFiles Tests

        [Fact]
        public void GetAllScriptFiles_WithRootDirectory_ReturnsRootAndAllDependencies()
        {
            // Arrange
            var dep1 = Path.Combine(_testRoot, "dep1.csx");
            var dep2 = Path.Combine(_testRoot, "dep2.csx");
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(dep1, "// dep1");
            File.WriteAllText(dep2, "// dep2");
            File.WriteAllText(scriptFile, "#load \"dep1.csx\"\n#load \"dep2.csx\"\n// main");

            // Act
            var result = _resolver.GetAllScriptFiles(scriptFile);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Contains(scriptFile, result);
            Assert.Contains(dep1, result);
            Assert.Contains(dep2, result);
        }

        [Fact]
        public void GetAllScriptFiles_WithNestedDependencies_ReturnsAllFilesRecursively()
        {
            // Arrange
            var dep1 = Path.Combine(_testRoot, "dep1.csx");
            var dep2 = Path.Combine(_testRoot, "dep2.csx");
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(dep2, "// dep2");
            File.WriteAllText(dep1, "#load \"dep2.csx\"\n// dep1");
            File.WriteAllText(scriptFile, "#load \"dep1.csx\"\n// main");

            // Act
            var result = _resolver.GetAllScriptFiles(scriptFile);

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Contains(scriptFile, result);
            Assert.Contains(dep1, result);
            Assert.Contains(dep2, result);
        }

        [Fact]
        public void GetAllScriptFiles_WithCircularDependency_AvoidsDuplication()
        {
            // Arrange
            var dep1 = Path.Combine(_testRoot, "dep1.csx");
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            File.WriteAllText(dep1, "#load \"script.csx\"\n// dep1");
            File.WriteAllText(scriptFile, "#load \"dep1.csx\"\n// main");

            // Act
            var result = _resolver.GetAllScriptFiles(scriptFile);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(scriptFile, result);
            Assert.Contains(dep1, result);
        }

        [Fact]
        public void GetAllScriptFiles_WithNullPath_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _resolver.GetAllScriptFiles(null));
        }

        [Fact]
        public void GetAllScriptFiles_WithNonExistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var nonExistentFile = Path.Combine(_testRoot, "nonexistent.csx");

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _resolver.GetAllScriptFiles(nonExistentFile));
        }

        #endregion

        #region GetScriptContent Tests

        [Fact]
        public void GetScriptContent_WithValidFile_ReturnsFileContent()
        {
            // Arrange
            var scriptFile = Path.Combine(_testRoot, "script.csx");
            var content = "// test content\nvar x = 5;";
            File.WriteAllText(scriptFile, content);

            // Act
            var result = _resolver.GetScriptContent(scriptFile);

            // Assert
            Assert.Equal(content, result);
        }

        [Fact]
        public void GetScriptContent_WithEmptyFile_ReturnsEmptyString()
        {
            // Arrange
            var scriptFile = Path.Combine(_testRoot, "empty.csx");
            File.WriteAllText(scriptFile, string.Empty);

            // Act
            var result = _resolver.GetScriptContent(scriptFile);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetScriptContent_WithNullPath_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _resolver.GetScriptContent(null));
        }

        [Fact]
        public void GetScriptContent_WithNonExistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var nonExistentFile = Path.Combine(_testRoot, "nonexistent.csx");

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _resolver.GetScriptContent(nonExistentFile));
        }

        #endregion

        #region IsScriptFile Tests

        [Fact]
        public void IsScriptFile_WithCsxExtension_ReturnsTrue()
        {
            // Act
            var result = _resolver.IsScriptFile("script.csx");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsScriptFile_WithCsxExtensionUpperCase_ReturnsTrue()
        {
            // Act
            var result = _resolver.IsScriptFile("SCRIPT.CSX");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsScriptFile_WithCsxExtensionMixedCase_ReturnsTrue()
        {
            // Act
            var result = _resolver.IsScriptFile("Script.CsX");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsScriptFile_WithCsExtension_ReturnsFalse()
        {
            // Act
            var result = _resolver.IsScriptFile("code.cs");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsScriptFile_WithDllExtension_ReturnsFalse()
        {
            // Act
            var result = _resolver.IsScriptFile("library.dll");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsScriptFile_WithNullPath_ReturnsFalse()
        {
            // Act
            var result = _resolver.IsScriptFile(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsScriptFile_WithEmptyString_ReturnsFalse()
        {
            // Act
            var result = _resolver.IsScriptFile(string.Empty);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsScriptFile_WithNoCsxExtension_ReturnsFalse()
        {
            // Act
            var result = _resolver.IsScriptFile("scriptcsx");

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}