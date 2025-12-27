using System;
using System.IO;
using System.Text;
using Dotnet.Script.Core;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Dotnet.Script.Core.Tests
{
    public class ExtensionsTests
    {
        #region GetRootedPath Tests

        [Fact]
        public void GetRootedPath_WithAbsolutePath_ReturnsPathUnchanged()
        {
            // Arrange
            string absolutePath = Path.IsPathRooted("/") ? "/absolute/path" : "C:\\absolute\\path";
            
            // Act
            var result = absolutePath.GetRootedPath();
            
            // Assert
            Assert.Equal(absolutePath, result);
        }

        [Fact]
        public void GetRootedPath_WithRelativePath_CombinesWithCurrentDirectory()
        {
            // Arrange
            string relativePath = "relative/path.txt";
            string expectedPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            
            // Act
            var result = relativePath.GetRootedPath();
            
            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void GetRootedPath_WithEmptyString_CombinesWithCurrentDirectory()
        {
            // Arrange
            string emptyPath = "";
            string expectedPath = Path.Combine(Directory.GetCurrentDirectory(), emptyPath);
            
            // Act
            var result = emptyPath.GetRootedPath();
            
            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void GetRootedPath_WithDotPath_CombinesWithCurrentDirectory()
        {
            // Arrange
            string dotPath = ".";
            string expectedPath = Path.Combine(Directory.GetCurrentDirectory(), dotPath);
            
            // Act
            var result = dotPath.GetRootedPath();
            
            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void GetRootedPath_WithParentPath_CombinesWithCurrentDirectory()
        {
            // Arrange
            string parentPath = "../relative/path.txt";
            string expectedPath = Path.Combine(Directory.GetCurrentDirectory(), parentPath);
            
            // Act
            var result = parentPath.GetRootedPath();
            
            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void GetRootedPath_WithWindowsAbsolutePath_ReturnsPathUnchanged()
        {
            // Arrange
            string windowsPath = "C:\\absolute\\path\\file.txt";
            if (!Path.IsPathRooted(windowsPath))
            {
                // Skip on non-Windows systems
                return;
            }
            
            // Act
            var result = windowsPath.GetRootedPath();
            
            // Assert
            Assert.Equal(windowsPath, result);
        }

        [Fact]
        public void GetRootedPath_WithUnixAbsolutePath_ReturnsPathUnchanged()
        {
            // Arrange
            string unixPath = "/usr/local/file.txt";
            if (!Path.IsPathRooted(unixPath))
            {
                // Skip on Windows systems
                return;
            }
            
            // Act
            var result = unixPath.GetRootedPath();
            
            // Assert
            Assert.Equal(unixPath, result);
        }

        [Fact]
        public void GetRootedPath_WithSpecialCharacters_CombinesCorrectly()
        {
            // Arrange
            string pathWithSpecialChars = "path with spaces/file-name_123.txt";
            string expectedPath = Path.Combine(Directory.GetCurrentDirectory(), pathWithSpecialChars);
            
            // Act
            var result = pathWithSpecialChars.GetRootedPath();
            
            // Assert
            Assert.Equal(expectedPath, result);
        }

        #endregion

        #region ToSourceText Tests

        [Fact]
        public void ToSourceText_WithValidFile_ReturnsSourceText()
        {
            // Arrange
            string fileName = Path.GetTempFileName();
            string testContent = "public class Test { }";
            try
            {
                File.WriteAllText(fileName, testContent);
                
                // Act
                var result = fileName.ToSourceText();
                
                // Assert
                Assert.NotNull(result);
                Assert.IsAssignableFrom<SourceText>(result);
                Assert.Equal(testContent, result.ToString());
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [Fact]
        public void ToSourceText_WithEmptyFile_ReturnsEmptySourceText()
        {
            // Arrange
            string fileName = Path.GetTempFileName();
            try
            {
                File.WriteAllText(fileName, "");
                
                // Act
                var result = fileName.ToSourceText();
                
                // Assert
                Assert.NotNull(result);
                Assert.IsAssignableFrom<SourceText>(result);
                Assert.Empty(result.ToString());
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [Fact]
        public void ToSourceText_WithLargeFile_ReturnsCompleteSourceText()
        {
            // Arrange
            string fileName = Path.GetTempFileName();
            var largeContent = new StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                largeContent.AppendLine($"public class Test{i} {{ public void Method{i}() {{ }} }}");
            }
            try
            {
                File.WriteAllText(fileName, largeContent.ToString());
                
                // Act
                var result = fileName.ToSourceText();
                
                // Assert
                Assert.NotNull(result);
                Assert.IsAssignableFrom<SourceText>(result);
                Assert.Equal(largeContent.ToString(), result.ToString());
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [Fact]
        public void ToSourceText_WithMultilineContent_ReturnsSourceTextWithLines()
        {
            // Arrange
            string fileName = Path.GetTempFileName();
            string testContent = "line1\nline2\nline3";
            try
            {
                File.WriteAllText(fileName, testContent);
                
                // Act
                var result = fileName.ToSourceText();
                
                // Assert
                Assert.NotNull(result);
                Assert.IsAssignableFrom<SourceText>(result);
                Assert.Equal(testContent, result.ToString());
                Assert.Equal(3, result.Lines.Count);
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [Fact]
        public void ToSourceText_WithSpecialCharacters_PreservesContent()
        {
            // Arrange
            string fileName = Path.GetTempFileName();
            string testContent = "public class Test { public string prop = \"special chars: !@#$%^&*()\"; }";
            try
            {
                File.WriteAllText(fileName, testContent);
                
                // Act
                var result = fileName.ToSourceText();
                
                // Assert
                Assert.NotNull(result);
                Assert.Equal(testContent, result.ToString());
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [Fact]
        public void ToSourceText_WithUnicodeContent_PreservesContent()
        {
            // Arrange
            string fileName = Path.GetTempFileName();
            string testContent = "public class Test { /* 你好世界 */ }";
            try
            {
                File.WriteAllText(fileName, testContent, Encoding.UTF8);
                
                // Act
                var result = fileName.ToSourceText();
                
                // Assert
                Assert.NotNull(result);
                Assert.Equal(testContent, result.ToString());
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [Fact]
        public void ToSourceText_WithNonexistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            string nonexistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "nonexistent.cs");
            
            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => nonexistentPath.ToSourceText());
        }

        [Fact]
        public void ToSourceText_WithDirectoryPath_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            string directoryPath = Path.GetTempPath();
            
            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => directoryPath.ToSourceText());
        }

        [Fact]
        public void ToSourceText_DisposesFileStreamAfterReading()
        {
            // Arrange
            string fileName = Path.GetTempFileName();
            string testContent = "public class Test { }";
            try
            {
                File.WriteAllText(fileName, testContent);
                
                // Act
                var result = fileName.ToSourceText();
                
                // Assert - File should be readable again (stream is disposed)
                Assert.NotNull(result);
                File.Delete(fileName);
                Assert.False(File.Exists(fileName));
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [Fact]
        public void ToSourceText_WithBinaryContent_ReadsAsText()
        {
            // Arrange
            string fileName = Path.GetTempFileName();
            byte[] binaryData = new byte[] { 0x89, 0x50, 0x4E, 0x47 }; // PNG header
            try
            {
                File.WriteAllBytes(fileName, binaryData);
                
                // Act
                var result = fileName.ToSourceText();
                
                // Assert
                Assert.NotNull(result);
                Assert.IsAssignableFrom<SourceText>(result);
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [Fact]
        public void ToSourceText_WithReadOnlyFile_CanStillRead()
        {
            // Arrange
            string fileName = Path.GetTempFileName();
            string testContent = "public class Test { }";
            try
            {
                File.WriteAllText(fileName, testContent);
                var fileInfo = new FileInfo(fileName);
                fileInfo.Attributes = FileAttributes.ReadOnly;
                
                // Act
                var result = fileName.ToSourceText();
                
                // Assert
                Assert.NotNull(result);
                Assert.Equal(testContent, result.ToString());
            }
            finally
            {
                var fileInfo = new FileInfo(fileName);
                fileInfo.Attributes = FileAttributes.Normal;
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        #endregion
    }
}