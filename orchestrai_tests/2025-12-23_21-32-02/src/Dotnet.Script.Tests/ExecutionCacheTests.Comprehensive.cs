using Dotnet.Script.Shared.Tests;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet.Script.Tests
{
    public class ExecutionCacheTests_Comprehensive
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ExecutionCacheTests_Comprehensive(ITestOutputHelper testOutputHelper)
        {
            testOutputHelper.Capture();
            this.testOutputHelper = testOutputHelper;
        }

        // Tests for ShouldNotUpdateHashWhenSourceIsNotChanged path coverage
        [Fact]
        public void ShouldNotUpdateHashWhenSourceIsNotChanged_VerifyHashFileExists()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScript = Path.Combine(scriptFolder.Path, "main.csx");

            // Act
            WriteScript(pathToScript, "WriteLine(42);");
            var (output, hash) = Execute(pathToScript);

            // Assert
            Assert.Contains("42", output);
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
        }

        [Fact]
        public void ShouldNotUpdateHashWhenSourceIsNotChanged_VerifyHashConsistency()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScript = Path.Combine(scriptFolder.Path, "main.csx");

            // Act
            WriteScript(pathToScript, "WriteLine(\"test\");");
            var (output1, hash1) = Execute(pathToScript);
            var (output2, hash2) = Execute(pathToScript);

            // Assert
            Assert.Equal(output1, output2);
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void ShouldNotUpdateHashWhenSourceIsNotChanged_MultipleRuns()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScript = Path.Combine(scriptFolder.Path, "test.csx");

            // Act
            WriteScript(pathToScript, "WriteLine(100);");
            var hashes = new[] {
                Execute(pathToScript).hash,
                Execute(pathToScript).hash,
                Execute(pathToScript).hash
            };

            // Assert
            Assert.All(hashes, h => Assert.NotNull(h));
            Assert.Equal(hashes[0], hashes[1]);
            Assert.Equal(hashes[1], hashes[2]);
        }

        // Tests for ShouldUpdateHashWhenSourceChanges path coverage
        [Fact]
        public void ShouldUpdateHashWhenSourceChanges_DetectsDifference()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScript = Path.Combine(scriptFolder.Path, "main.csx");

            // Act
            WriteScript(pathToScript, "WriteLine(1);");
            var (output1, hash1) = Execute(pathToScript);
            
            WriteScript(pathToScript, "WriteLine(2);");
            var (output2, hash2) = Execute(pathToScript);

            // Assert
            Assert.Contains("1", output1);
            Assert.Contains("2", output2);
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void ShouldUpdateHashWhenSourceChanges_MultipleChanges()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScript = Path.Combine(scriptFolder.Path, "script.csx");

            // Act
            WriteScript(pathToScript, "var x = 1;");
            var hash1 = Execute(pathToScript).hash;
            
            WriteScript(pathToScript, "var x = 2;");
            var hash2 = Execute(pathToScript).hash;
            
            WriteScript(pathToScript, "var x = 3;");
            var hash3 = Execute(pathToScript).hash;

            // Assert
            Assert.NotNull(hash1);
            Assert.NotNull(hash2);
            Assert.NotNull(hash3);
            Assert.NotEqual(hash1, hash2);
            Assert.NotEqual(hash2, hash3);
            Assert.NotEqual(hash1, hash3);
        }

        [Fact]
        public void ShouldUpdateHashWhenSourceChanges_MinimalChange()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScript = Path.Combine(scriptFolder.Path, "minimal.csx");

            // Act - Change single character
            WriteScript(pathToScript, "Console.WriteLine(\"a\");");
            var hash1 = Execute(pathToScript).hash;
            
            WriteScript(pathToScript, "Console.WriteLine(\"b\");");
            var hash2 = Execute(pathToScript).hash;

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        // Tests for ShouldNotCreateHashWhenScriptIsNotCacheable path coverage
        [Fact]
        public void ShouldNotCreateHashWhenScriptIsNotCacheable_WithNuGetReference()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScript = Path.Combine(scriptFolder.Path, "main.csx");

            // Act
            WriteScript(pathToScript, "#r \"nuget:SomePackage, *\"", "WriteLine(42);");
            var (output, hash) = Execute(pathToScript);

            // Assert
            Assert.NotNull(output);
            Assert.Null(hash);
        }

        [Fact]
        public void ShouldNotCreateHashWhenScriptIsNotCacheable_VerifyNoCacheFile()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScript = Path.Combine(scriptFolder.Path, "uncacheable.csx");

            // Act
            WriteScript(pathToScript, "#r \"nuget:Package, 1.0.0\"", "WriteLine(\"test\");");
            Execute(pathToScript);
            
            var pathToExecutionCache = GetPathToExecutionCache(pathToScript);
            var pathToCacheFile = Path.Combine(pathToExecutionCache, "script.sha256");

            // Assert
            Assert.False(File.Exists(pathToCacheFile));
        }

        [Fact]
        public void ShouldNotCreateHashWhenScriptIsNotCacheable_MultipleNuGetReferences()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScript = Path.Combine(scriptFolder.Path, "multi.csx");

            // Act
            WriteScript(pathToScript, 
                "#r \"nuget:Package1, 1.0\"", 
                "#r \"nuget:Package2, 2.0\"", 
                "WriteLine(42);");
            var (output, hash) = Execute(pathToScript);

            // Assert
            Assert.Null(hash);
        }

        // Tests for ShouldCopyDllAndPdbToExecutionCacheFolder path coverage
        [Fact]
        public void ShouldCopyDllAndPdbToExecutionCacheFolder_VerifyFiles()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScript = Path.Combine(scriptFolder.Path, "main.csx");

            // Act
            WriteScript(pathToScript, "#r \"nuget:LightInject, 5.2.1\"", "WriteLine(42);");
            ScriptTestRunner.Default.Execute($"{pathToScript} --nocache");
            var pathToExecutionCache = GetPathToExecutionCache(pathToScript);

            // Assert
            Assert.True(File.Exists(Path.Combine(pathToExecutionCache, "LightInject.dll")));
            Assert.True(File.Exists(Path.Combine(pathToExecutionCache, "LightInject.pdb")));
        }

        [Fact]
        public void ShouldCopyDllAndPdbToExecutionCacheFolder_VerifyDllNotEmpty()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScript = Path.Combine(scriptFolder.Path, "main.csx");

            // Act
            WriteScript(pathToScript, "#r \"nuget:LightInject, 5.2.1\"", "WriteLine(42);");
            ScriptTestRunner.Default.Execute($"{pathToScript} --nocache");
            var pathToExecutionCache = GetPathToExecutionCache(pathToScript);
            var dllPath = Path.Combine(pathToExecutionCache, "LightInject.dll");

            // Assert
            var fileInfo = new FileInfo(dllPath);
            Assert.True(fileInfo.Length > 0);
        }

        // Tests for ShouldCacheScriptsFromSameFolderIndividually path coverage
        [Fact]
        public void ShouldCacheScriptsFromSameFolderIndividually_VerifyFirstExecutionNotCached()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScriptA = Path.Combine(scriptFolder.Path, "scriptA.csx");
            var idScriptA = Guid.NewGuid().ToString();

            // Act
            File.AppendAllText(pathToScriptA, $"WriteLine(\"{idScriptA}\");");
            var (output, isCached) = Execute_Cached(pathToScriptA);

            // Assert
            Assert.Contains(idScriptA, output);
            Assert.False(isCached);
        }

        [Fact]
        public void ShouldCacheScriptsFromSameFolderIndividually_VerifySecondExecutionCached()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScriptA = Path.Combine(scriptFolder.Path, "scriptA.csx");
            var idScriptA = Guid.NewGuid().ToString();

            // Act
            File.AppendAllText(pathToScriptA, $"WriteLine(\"{idScriptA}\");");
            Execute_Cached(pathToScriptA); // First run
            var (output, isCached) = Execute_Cached(pathToScriptA); // Second run

            // Assert
            Assert.Contains(idScriptA, output);
            Assert.True(isCached);
        }

        [Fact]
        public void ShouldCacheScriptsFromSameFolderIndividually_DifferentScriptsSeparatelyCached()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScriptA = Path.Combine(scriptFolder.Path, "scriptA.csx");
            var pathToScriptB = Path.Combine(scriptFolder.Path, "scriptB.csx");
            var idA = Guid.NewGuid().ToString();
            var idB = Guid.NewGuid().ToString();

            // Act
            File.AppendAllText(pathToScriptA, $"WriteLine(\"{idA}\");");
            File.AppendAllText(pathToScriptB, $"WriteLine(\"{idB}\");");

            var (outputA1, cachedA1) = Execute_Cached(pathToScriptA);
            var (outputB1, cachedB1) = Execute_Cached(pathToScriptB);

            // Assert
            Assert.Contains(idA, outputA1);
            Assert.Contains(idB, outputB1);
            Assert.False(cachedA1);
            Assert.False(cachedB1);

            // Act - Second run
            var (outputA2, cachedA2) = Execute_Cached(pathToScriptA);
            var (outputB2, cachedB2) = Execute_Cached(pathToScriptB);

            // Assert
            Assert.True(cachedA2);
            Assert.True(cachedB2);
        }

        [Fact]
        public void ShouldCacheScriptsFromSameFolderIndividually_UpdateOneScriptInvalidatesItsCache()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScriptA = Path.Combine(scriptFolder.Path, "scriptA.csx");
            var pathToScriptB = Path.Combine(scriptFolder.Path, "scriptB");
            var idA = Guid.NewGuid().ToString();
            var idB = Guid.NewGuid().ToString();

            // Act
            File.AppendAllText(pathToScriptA, $"WriteLine(\"{idA}\");");
            File.AppendAllText(pathToScriptB, $"WriteLine(\"{idB}\");");

            Execute_Cached(pathToScriptA);
            Execute_Cached(pathToScriptB);

            var (outputB_before, cachedB_before) = Execute_Cached(pathToScriptB);
            Assert.True(cachedB_before);

            // Modify scriptB
            var idB2 = Guid.NewGuid().ToString();
            File.AppendAllText(pathToScriptB, $"WriteLine(\"{idB2}\");");

            var (outputB_after, cachedB_after) = Execute_Cached(pathToScriptB);

            // Assert
            Assert.False(cachedB_after);
            Assert.Contains(idB2, outputB_after);
        }

        [Fact]
        public void ShouldCacheScriptsFromSameFolderIndividually_NoExtensionScript()
        {
            // Arrange
            using var scriptFolder = new DisposableFolder();
            var pathToScriptNoExt = Path.Combine(scriptFolder.Path, "scriptWithoutExt");
            var id = Guid.NewGuid().ToString();

            // Act
            File.AppendAllText(pathToScriptNoExt, $"WriteLine(\"{id}\");");
            var (output1, cached1) = Execute_Cached(pathToScriptNoExt);
            var (output2, cached2) = Execute_Cached(pathToScriptNoExt);

            // Assert
            Assert.Contains(id, output1);
            Assert.False(cached1);
            Assert.True(cached2);
        }

        // Helper methods
        private (string Output, string hash) Execute(string pathToScript)
        {
            var result = ScriptTestRunner.Default.Execute(pathToScript);
            testOutputHelper.WriteLine(result.Output);
            Assert.Equal(0, result.ExitCode);
            
            string pathToExecutionCache = GetPathToExecutionCache(pathToScript);
            var pathToCacheFile = Path.Combine(pathToExecutionCache, "script.sha256");
            string cachedhash = null;
            
            if (File.Exists(pathToCacheFile))
            {
                cachedhash = File.ReadAllText(pathToCacheFile);
            }

            return (result.Output, cachedhash);
        }

        private (string Output, bool Cached) Execute_Cached(string pathToScript)
        {
            var (output, exitCode) = ScriptTestRunner.Default.Execute($"{pathToScript} --debug");
            return (Output: output, Cached: output.Contains("Using cached compilation"));
        }

        private static string GetPathToExecutionCache(string pathToScript)
        {
            var pathToTempFolder = Dotnet.Script.DependencyModel.ProjectSystem.FileUtils.GetPathToScriptTempFolder(pathToScript);
            var pathToExecutionCache = Path.Combine(pathToTempFolder, "execution-cache");
            return pathToExecutionCache;
        }

        private static void WriteScript(string path, params string[] lines)
        {
            File.WriteAllLines(path, lines);
        }
    }
}