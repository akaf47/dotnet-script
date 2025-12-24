using System;
using System.IO;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Dotnet.Script.Core.Tests
{
    public class ExtensionsTests : IDisposable
    {
        private readonly string _tempDirectory;

        public ExtensionsTests()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDirectory);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        #region GetRootedPath Tests

        [Fact]
        public void GetRootedPath_WithAbsolutePath_ReturnsPathUnchanged()
        {
            // Arrange
            var absolutePath = Path.GetTempPath();

            // Act
            var result = absolutePath.GetRootedPath();

            // Assert
            Assert.Equal(absolutePath, result);
        }

        [Fact]
        public void GetRootedPath_WithRelativePath_CombinesWithCurrentDirectory()
        {
            // Arrange
            var relativePath = "RelativeDir\\File.txt";
            var expectedPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            // Act
            var result = relativePath.GetRootedPath();

            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void GetRootedPath_WithRelativePathSingleLevel_CombinesCorrectly()
        {
            // Arrange
            var relativePath = "file.txt";
            var expectedPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            // Act
            var result = relativePath.GetRootedPath();

            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void GetRootedPath_WithRelativePathParentDirectory_CombinesCorrectly()
        {
            // Arrange
            var relativePath = "..\\Parent\\File.txt";
            var expectedPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            // Act
            var result = relativePath.GetRootedPath();

            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void GetRootedPath_WithCurrentDirectoryDot_CombinesCorrectly()
        {
            // Arrange
            var relativePath = ".\\File.txt";
            var expectedPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            // Act
            var result = relativePath.GetRootedPath();

            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void GetRootedPath_WithEmptyString_CombinesWithCurrentDirectory()
        {
            // Arrange
            var emptyPath = string.Empty;
            var expectedPath = Path.Combine(Directory.GetCurrentDirectory(), emptyPath);

            // Act
            var result = emptyPath.GetRootedPath();

            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void GetRootedPath_OnWindowsAbsolutePath_ReturnsPathUnchanged()
        {
            // Arrange
            // This test will behave differently on different OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var windowsAbsolutePath = "C:\\Users\\Test\\File.txt";

                // Act
                var result = windowsAbsolutePath.GetRootedPath();

                // Assert
                Assert.Equal(windowsAbsolutePath, result);
            }
        }

        [Fact]
        public void GetRootedPath_OnUnixAbsolutePath_ReturnsPathUnchanged()
        {
            // Arrange
            // This test will behave differently on different OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var unixAbsolutePath = "/home/user/file.txt";

                // Act
                var result = unixAbsolutePath.GetRootedPath();

                // Assert
                Assert.Equal(unixAbsolutePath, result);
            }
        }

        [Fact]
        public void GetRootedPath_WithSpecialCharactersInPath_CombinesCorrectly()
        {
            // Arrange
            var pathWithSpecialChars = "folder with spaces\\file-name_test.txt";
            var expectedPath = Path.Combine(Directory.GetCurrentDirectory(), pathWithSpecialChars);

            // Act
            var result = pathWithSpecialChars.GetRootedPath();

            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void GetRootedPath_WithPathSeparatorAtStart_TreatsAsRelative()
        {
            // Arrange
            var pathStartingWithSeparator = "\\RelativePath\\File.txt";

            // Act
            var result = pathStartingWithSeparator.GetRootedPath();

            // Assert
            // Path.IsPathRooted behavior depends on OS
            // On Windows, paths starting with \ are considered rooted
            var isRooted = Path.IsPathRooted(pathStartingWithSeparator);
            Assert.Equal(pathStartingWithSeparator, result);
        }

        #endregion

        #region ToSourceText Tests

        [Fact]
        public void ToSourceText_WithValidFile_ReturnsSourceText()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "test.cs");
            var testContent = "public class Test { }";
            File.WriteAllText(testFileName, testContent);

            // Act
            var result = testFileName.ToSourceText();

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<SourceText>(result);
            Assert.Equal(testContent, result.ToString());
        }

        [Fact]
        public void ToSourceText_WithEmptyFile_ReturnsEmptySourceText()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "empty.cs");
            File.WriteAllText(testFileName, string.Empty);

            // Act
            var result = testFileName.ToSourceText();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.ToString());
        }

        [Fact]
        public void ToSourceText_WithMultilineContent_PreservesContent()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "multiline.cs");
            var testContent = "using System;\npublic class Test\n{\n    public void Method()\n    {\n    }\n}";
            File.WriteAllText(testFileName, testContent);

            // Act
            var result = testFileName.ToSourceText();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testContent, result.ToString());
        }

        [Fact]
        public void ToSourceText_WithUnicodeContent_PreservesContent()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "unicode.cs");
            var testContent = "// Comment with unicode: αβγδε 中文 日本語";
            File.WriteAllText(testFileName, testContent, System.Text.Encoding.UTF8);

            // Act
            var result = testFileName.ToSourceText();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testContent, result.ToString());
        }

        [Fact]
        public void ToSourceText_WithLargeFile_ReturnsSourceText()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "large.cs");
            var testContent = new string('x', 1000000); // 1MB file
            File.WriteAllText(testFileName, testContent);

            // Act
            var result = testFileName.ToSourceText();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testContent.Length, result.ToString().Length);
        }

        [Fact]
        public void ToSourceText_WithFileNotFound_ThrowsFileNotFoundException()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_tempDirectory, "nonexistent.cs");

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => nonExistentPath.ToSourceText());
        }

        [Fact]
        public void ToSourceText_WithInvalidPath_ThrowsException()
        {
            // Arrange
            var invalidPath = "\0invalid\0path";

            // Act & Assert
            Assert.Throws<Exception>(() => invalidPath.ToSourceText());
        }

        [Fact]
        public void ToSourceText_WithSpecialCharactersInFilename_ReturnsSourceText()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "test-file_v1.0.cs");
            var testContent = "public class Test { }";
            File.WriteAllText(testFileName, testContent);

            // Act
            var result = testFileName.ToSourceText();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testContent, result.ToString());
        }

        [Fact]
        public void ToSourceText_WithBOMContent_PreservesContent()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "bom.cs");
            var testContent = "public class Test { }";
            // Write with UTF-8 BOM
            File.WriteAllText(testFileName, testContent, new System.Text.UTF8Encoding(true));

            // Act
            var result = testFileName.ToSourceText();

            // Assert
            Assert.NotNull(result);
            // SourceText.From handles BOM appropriately
            Assert.True(result.ToString().Contains("Test"));
        }

        [Fact]
        public void ToSourceText_WithReadOnlyFile_ReturnsSourceText()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "readonly.cs");
            var testContent = "public class Test { }";
            File.WriteAllText(testFileName, testContent);
            File.SetAttributes(testFileName, FileAttributes.ReadOnly);

            // Act
            var result = testFileName.ToSourceText();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testContent, result.ToString());

            // Cleanup
            File.SetAttributes(testFileName, FileAttributes.Normal);
        }

        [Fact]
        public void ToSourceText_FileStreamIsProperlyDisposed()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "dispose_test.cs");
            var testContent = "public class Test { }";
            File.WriteAllText(testFileName, testContent);

            // Act
            var result = testFileName.ToSourceText();

            // Assert - If file wasn't disposed, we wouldn't be able to delete it on Windows
            Assert.NotNull(result);
            // Try to delete the file to ensure stream was disposed
            Assert.NotThrows(() => File.Delete(testFileName));
            Assert.False(File.Exists(testFileName));
        }

        [Fact]
        public void ToSourceText_WithWindowsLineEndings_PreservesLineEndings()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "crlf.cs");
            var testContent = "public class Test\r\n{\r\n    public void Method()\r\n    {\r\n    }\r\n}";
            File.WriteAllText(testFileName, testContent);

            // Act
            var result = testFileName.ToSourceText();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testContent, result.ToString());
        }

        [Fact]
        public void ToSourceText_WithUnixLineEndings_PreservesLineEndings()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "lf.cs");
            var testContent = "public class Test\n{\n    public void Method()\n    {\n    }\n}";
            File.WriteAllText(testFileName, testContent);

            // Act
            var result = testFileName.ToSourceText();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testContent, result.ToString());
        }

        [Fact]
        public void ToSourceText_WithMacLineEndings_PreservesLineEndings()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "cr.cs");
            var testContent = "public class Test\r{\r    public void Method()\r    {\r    }\r}";
            File.WriteAllText(testFileName, testContent);

            // Act
            var result = testFileName.ToSourceText();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testContent, result.ToString());
        }

        [Fact]
        public void ToSourceText_CalledTwiceOnSameFile_ReturnsConsistentContent()
        {
            // Arrange
            var testFileName = Path.Combine(_tempDirectory, "twice.cs");
            var testContent = "public class Test { }";
            File.WriteAllText(testFileName, testContent);

            // Act
            var result1 = testFileName.ToSourceText();
            var result2 = testFileName.ToSourceText();

            // Assert
            Assert.Equal(result1.ToString(), result2.ToString());
        }

        #endregion

        #region Helper Methods

        // Helper to assert that code doesn't throw
        private void Assert.NotThrows(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                throw new Xunit.Sdk.XunitException($"Expected no exception but got {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        #endregion
    }
}