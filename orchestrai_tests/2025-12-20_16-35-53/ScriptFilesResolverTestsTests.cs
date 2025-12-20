using System;
using System.IO;
using System.Linq;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class ScriptFilesResolverTestsTests
    {
        [Fact]
        public void WriteScript_CreatesValidScriptFile()
        {
            // Arrange
            using var rootFolder = new DisposableFolder();
            var content = "// Test content";
            var scriptName = "TestScript.csx";

            // Act
            var fullPath = ScriptFilesResolverTests.WriteScript(content, rootFolder.Path, scriptName);

            // Assert
            Assert.True(File.Exists(fullPath));
            Assert.Equal(content, File.ReadAllText(fullPath));
        }

        [Theory]
        [InlineData("", 1)]
        [InlineData("#load \"Bar.csx\"", 2)]
        public void GetScriptFiles_WithRootScriptAndOptionalLoad_ReturnsCorrectNumberOfFiles(string rootScriptContent, int expectedFileCount)
        {
            // Arrange
            using var rootFolder = new DisposableFolder();
            var rootScript = ScriptFilesResolverTests.WriteScript(rootScriptContent, rootFolder.Path, "Foo.csx");
            if (rootScriptContent.Contains("#load")) 
            {
                ScriptFilesResolverTests.WriteScript(string.Empty, rootFolder.Path, "Bar.csx");
            }

            var scriptFilesResolver = new ScriptFilesResolver();

            // Act
            var files = scriptFilesResolver.GetScriptFiles(rootScript);

            // Assert
            Assert.Equal(expectedFileCount, files.Count);
        }

        [Fact]
        public void GetScriptFiles_WithSubfolderLoad_ResolvesScriptCorrectly()
        {
            // Arrange
            using var rootFolder = new DisposableFolder();
            var subFolder = Path.Combine(rootFolder.Path, "SubFolder");
            Directory.CreateDirectory(subFolder);

            var rootScript = ScriptFilesResolverTests.WriteScript("#load \"SubFolder/Bar.csx\"", rootFolder.Path, "Foo.csx");
            ScriptFilesResolverTests.WriteScript(string.Empty, subFolder, "Bar.csx");

            var scriptFilesResolver = new ScriptFilesResolver();

            // Act
            var files = scriptFilesResolver.GetScriptFiles(rootScript);

            // Assert
            Assert.Equal(2, files.Count);
            Assert.Contains(files, f => f.Contains("Foo.csx"));
            Assert.Contains(files, f => f.Contains("Bar.csx"));
        }

        [Theory]
        [InlineData("#LOAD \"Bar.csx\"", 1)]
        [InlineData("#load \"NuGet.csx\"", 2)]
        public void GetScriptFiles_WithCaseInsensitiveLoad_ResolvesCorrectly(string rootScriptContent, int expectedFileCount)
        {
            // Arrange
            using var rootFolder = new DisposableFolder();
            var rootScript = ScriptFilesResolverTests.WriteScript(rootScriptContent, rootFolder.Path, "Foo.csx");
            if (rootScriptContent.Contains("NuGet.csx"))
            {
                ScriptFilesResolverTests.WriteScript(string.Empty, rootFolder.Path, "NuGet.csx");
            }
            else
            {
                ScriptFilesResolverTests.WriteScript(string.Empty, rootFolder.Path, "Bar.csx");
            }

            var scriptFilesResolver = new ScriptFilesResolver();

            // Act
            var files = scriptFilesResolver.GetScriptFiles(rootScript);

            // Assert
            Assert.Equal(expectedFileCount, files.Count);
        }
    }
}