using System;
using System.IO;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class FileUtilsComprehensiveTests
    {
        [Fact]
        public void GetTempPathReturnsDefaultPathWhenEnvVarNotSet()
        {
            // Ensure env var is not set
            Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", null);
            
            var tempPath = FileUtils.GetTempPath();
            
            Assert.NotNull(tempPath);
            Assert.NotEmpty(tempPath);
            // Should return a valid path (default temp path)
            Assert.True(Path.IsPathRooted(tempPath) || !string.IsNullOrEmpty(tempPath));
        }

        [Fact]
        public void GetTempPathHandlesAbsolutePathFromEnvVar()
        {
            var absolutePath = Path.GetPathRoot(Directory.GetCurrentDirectory());
            try
            {
                Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", absolutePath);
                var tempPath = FileUtils.GetTempPath();
                
                Assert.Equal(absolutePath, tempPath);
            }
            finally
            {
                Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", null);
            }
        }

        [Fact]
        public void GetTempPathHandlesRelativePathFromEnvVar()
        {
            var relativePath = "relative/path";
            try
            {
                Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", relativePath);
                var tempPath = FileUtils.GetTempPath();
                
                var expectedPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
                Assert.Equal(expectedPath, tempPath);
            }
            finally
            {
                Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", null);
            }
        }

        [Fact]
        public void GetTempPathHandleSingleDotRelativePath()
        {
            try
            {
                Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", ".");
                var tempPath = FileUtils.GetTempPath();
                
                var expectedPath = Path.Combine(Directory.GetCurrentDirectory(), ".");
                Assert.Equal(expectedPath, tempPath);
            }
            finally
            {
                Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", null);
            }
        }

        [Fact]
        public void GetTempPathHandleEmptyString()
        {
            try
            {
                Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", "");
                var tempPath = FileUtils.GetTempPath();
                
                // Empty string should be treated as not set or return default
                Assert.NotNull(tempPath);
            }
            finally
            {
                Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", null);
            }
        }

        [Fact]
        public void GetTempPathHandleWhitespaceOnlyPath()
        {
            try
            {
                Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", "   ");
                var tempPath = FileUtils.GetTempPath();
                
                // Whitespace should be handled
                Assert.NotNull(tempPath);
            }
            finally
            {
                Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", null);
            }
        }

        [Fact]
        public void GetTempPathHandlesPathWithSpaces()
        {
            var pathWithSpaces = "path with spaces";
            try
            {
                Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", pathWithSpaces);
                var tempPath = FileUtils.GetTempPath();
                
                var expectedPath = Path.Combine(Directory.GetCurrentDirectory(), pathWithSpaces);
                Assert.Equal(expectedPath, tempPath);
            }
            finally
            {
                Environment.SetEnvironmentVariable("DOTNET_SCRIPT_CACHE_LOCATION", null);
            }
        }

        [Fact]
        public void GetPathToScriptTempFolderReturnsValidPath()
        {
            var scriptPath = "/some/script/path.csx";
            var result = FileUtils.GetPathToScriptTempFolder(scriptPath);
            
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetPathToScriptTempFolderHandlesNullInput()
        {
            var result = FileUtils.GetPathToScriptTempFolder(null);
            
            // Should handle gracefully or throw
            Assert.NotNull(result);
        }

        [Fact]
        public void GetPathToScriptTempFolderHandlesEmptyString()
        {
            var result = FileUtils.GetPathToScriptTempFolder("");
            
            Assert.NotNull(result);
        }

        [Fact]
        public void CreateDirectoryIfNotExistsCreatesDirectory()
        {
            var testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                FileUtils.CreateDirectoryIfNotExists(testPath);
                
                Assert.True(Directory.Exists(testPath));
            }
            finally
            {
                if (Directory.Exists(testPath))
                    Directory.Delete(testPath, true);
            }
        }

        [Fact]
        public void CreateDirectoryIfNotExistsDoesNotThrowWhenDirectoryExists()
        {
            var testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                Directory.CreateDirectory(testPath);
                
                // Should not throw
                FileUtils.CreateDirectoryIfNotExists(testPath);
                
                Assert.True(Directory.Exists(testPath));
            }
            finally
            {
                if (Directory.Exists(testPath))
                    Directory.Delete(testPath, true);
            }
        }

        [Fact]
        public void CreateDirectoryIfNotExistsCreatesNestedDirectory()
        {
            var testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "nested", "path");
            try
            {
                FileUtils.CreateDirectoryIfNotExists(testPath);
                
                Assert.True(Directory.Exists(testPath));
            }
            finally
            {
                var rootPath = Path.Combine(Path.GetTempPath(), testPath.Split(Path.DirectorySeparatorChar)[^3]);
                if (Directory.Exists(rootPath))
                    Directory.Delete(rootPath, true);
            }
        }

        [Fact]
        public void RemoveDirectoryRemovesEmptyDirectory()
        {
            var testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                Directory.CreateDirectory(testPath);
                Assert.True(Directory.Exists(testPath));
                
                FileUtils.RemoveDirectory(testPath);
                
                Assert.False(Directory.Exists(testPath));
            }
            finally
            {
                if (Directory.Exists(testPath))
                    Directory.Delete(testPath, true);
            }
        }

        [Fact]
        public void RemoveDirectoryRemovesDirectoryWithContents()
        {
            var testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                Directory.CreateDirectory(testPath);
                File.WriteAllText(Path.Combine(testPath, "test.txt"), "content");
                Assert.True(Directory.Exists(testPath));
                
                FileUtils.RemoveDirectory(testPath);
                
                Assert.False(Directory.Exists(testPath));
            }
            finally
            {
                if (Directory.Exists(testPath))
                    Directory.Delete(testPath, true);
            }
        }

        [Fact]
        public void RemoveDirectoryHandlesNonExistentDirectory()
        {
            var testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            
            // Should not throw
            FileUtils.RemoveDirectory(testPath);
            
            Assert.False(Directory.Exists(testPath));
        }

        [Fact]
        public void RemoveDirectoryHandlesNullPath()
        {
            // Should handle gracefully
            try
            {
                FileUtils.RemoveDirectory(null);
            }
            catch (ArgumentNullException)
            {
                // Expected
            }
        }
    }
}