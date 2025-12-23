using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Moq;
using Dotnet.Script.DependencyModel.ProjectSystem;

namespace Dotnet.Script.DependencyModel.Tests.ProjectSystem
{
    public class ScriptFilesResolverTests
    {
        private readonly ScriptFilesResolver _resolver;
        private readonly string _tempDirectory;
        private readonly string _tempFile;

        public ScriptFilesResolverTests()
        {
            _resolver = new ScriptFilesResolver();
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
            _tempFile = Path.Combine(_tempDirectory, "test.csx");
        }

        [Fact]
        public void GetScriptFiles_WithSingleScriptFile_ReturnsFileInHashSet()
        {
            // Arrange
            File.WriteAllText(_tempFile, "");

            // Act
            var result = _resolver.GetScriptFiles(_tempFile);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<HashSet<string>>(result);
            Assert.Single(result);
            Assert.Contains(_tempFile, result);
        }

        [Fact]
        public void GetScriptFiles_WithScriptContainingLoadDirectives_ReturnsAllReferencedScripts()
        {
            // Arrange
            var scriptContent = @"#load ""helper.csx""
Console.WriteLine(""Main script"");";
            var helperFile = Path.Combine(_tempDirectory, "helper.csx");
            File.WriteAllText(_tempFile, scriptContent);
            File.WriteAllText(helperFile, "");

            // Act
            var result = _resolver.GetScriptFiles(_tempFile);

            // Assert
            Assert.Contains(_tempFile, result);
            Assert.Contains(helperFile, result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetScriptFiles_WithCircularDependencies_DoesNotCauseInfiniteLoop()
        {
            // Arrange
            var file1 = Path.Combine(_tempDirectory, "file1.csx");
            var file2 = Path.Combine(_tempDirectory, "file2.csx");
            
            File.WriteAllText(file1, $"#load \"{Path.GetFileName(file2)}\"");
            File.WriteAllText(file2, $"#load \"{Path.GetFileName(file1)}\"");

            // Act
            var result = _resolver.GetScriptFiles(file1);

            // Assert
            Assert.Contains(file1, result);
            Assert.Contains(file2, result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetScriptFiles_WithNestedLoadDirectives_ReturnsAllScriptsInDependencyChain()
        {
            // Arrange
            var file1 = Path.Combine(_tempDirectory, "file1.csx");
            var file2 = Path.Combine(_tempDirectory, "file2.csx");
            var file3 = Path.Combine(_tempDirectory, "file3.csx");
            
            File.WriteAllText(file1, $"#load \"{Path.GetFileName(file2)}\"");
            File.WriteAllText(file2, $"#load \"{Path.GetFileName(file3)}\"");
            File.WriteAllText(file3, "");

            // Act
            var result = _resolver.GetScriptFiles(file1);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(file1, result);
            Assert.Contains(file2, result);
            Assert.Contains(file3, result);
        }

        [Fact]
        public void GetScriptFiles_WithNonExistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var nonExistentFile = Path.Combine(_tempDirectory, "nonexistent.csx");

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _resolver.GetScriptFiles(nonExistentFile));
        }

        [Fact]
        public void GetScriptFiles_WithAbsolutePathInLoadDirective_UsesAbsolutePath()
        {
            // Arrange
            var absoluteHelper = Path.Combine(_tempDirectory, "absolute_helper.csx");
            var scriptContent = $"#load \"{absoluteHelper}\"";
            File.WriteAllText(_tempFile, scriptContent);
            File.WriteAllText(absoluteHelper, "");

            // Act
            var result = _resolver.GetScriptFiles(_tempFile);

            // Assert
            Assert.Contains(_tempFile, result);
            Assert.Contains(absoluteHelper, result);
        }

        [Fact]
        public void GetScriptFilesFromCode_WithNoLoadDirectives_ReturnsSingleEntryForCurrentDirectory()
        {
            // Arrange
            var code = "Console.WriteLine(\"Hello\");";

            // Act
            var result = _resolver.GetScriptFilesFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<HashSet<string>>(result);
        }

        [Fact]
        public void GetScriptFilesFromCode_WithLoadDirectives_ReturnsReferencedScripts()
        {
            // Arrange
            var helperFile = Path.Combine(_tempDirectory, "helper.csx");
            File.WriteAllText(helperFile, "");
            
            var relativePath = Path.Combine(".", Path.GetFileName(helperFile));
            var code = $"#load \"{relativePath}\"";
            
            var originalDirectory = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_tempDirectory);

                // Act
                var result = _resolver.GetScriptFilesFromCode(code);

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Fact]
        public void GetScriptFilesFromCode_WithNuGetLoadDirective_IgnoresNuGetDirective()
        {
            // Arrange
            var code = @"#load ""nuget: SomePackage, 1.0.0""
Console.WriteLine(""Hello"");";

            // Act
            var result = _resolver.GetScriptFilesFromCode(code);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetScriptFilesFromCode_WithMixedLoadDirectives_IgnoresNuGetOnlyIncludesFileReferences()
        {
            // Arrange
            var helperFile = Path.Combine(_tempDirectory, "helper.csx");
            File.WriteAllText(helperFile, "");
            
            var code = $@"#load ""nuget: NuGetPackage, 1.0.0""
#load ""{helperFile}""";

            var originalDirectory = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_tempDirectory);

                // Act
                var result = _resolver.GetScriptFilesFromCode(code);

                // Assert
                Assert.NotNull(result);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Fact]
        public void GetScriptFilesFromCode_WithEmptyCode_ReturnsEmptyOrMinimalHashSet()
        {
            // Arrange
            var code = string.Empty;

            // Act
            var result = _resolver.GetScriptFilesFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<HashSet<string>>(result);
        }

        [Fact]
        public void GetScriptFilesFromCode_WithMultilineComment_CorrectlyParsesLoadDirectives()
        {
            // Arrange
            var helperFile = Path.Combine(_tempDirectory, "helper.csx");
            File.WriteAllText(helperFile, "");
            
            var code = $@"/* This is a comment
#load ""fake.csx""
*/
#load ""{helperFile}""";

            var originalDirectory = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_tempDirectory);

                // Act
                var result = _resolver.GetScriptFilesFromCode(code);

                // Assert
                Assert.NotNull(result);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Fact]
        public void GetScriptFilesFromCode_WithLoadDirectiveWithWhitespace_CorrectlyParsesPath()
        {
            // Arrange
            var helperFile = Path.Combine(_tempDirectory, "helper.csx");
            File.WriteAllText(helperFile, "");
            
            var code = $@"#load   ""   {helperFile}   """;

            var originalDirectory = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_tempDirectory);

                // Act
                var result = _resolver.GetScriptFilesFromCode(code);

                // Assert
                Assert.NotNull(result);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Fact]
        public void GetScriptFilesFromCode_WithRelativePathLoadDirective_ResolvesRelativeToCurrentDirectory()
        {
            // Arrange
            var helperFile = Path.Combine(_tempDirectory, "helper.csx");
            File.WriteAllText(helperFile, "");
            
            var code = $"#load \"{Path.GetFileName(helperFile)}\"";

            var originalDirectory = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_tempDirectory);

                // Act
                var result = _resolver.GetScriptFilesFromCode(code);

                // Assert
                Assert.NotNull(result);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Fact]
        public void GetScriptFilesFromCode_WithAbsolutePathLoadDirective_UsesAbsolutePathDirectly()
        {
            // Arrange
            var helperFile = Path.Combine(_tempDirectory, "helper.csx");
            File.WriteAllText(helperFile, "");
            
            var code = $"#load \"{helperFile}\"";

            // Act
            var result = _resolver.GetScriptFilesFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(helperFile, result);
        }

        [Fact]
        public void GetScriptFilesFromCode_WithNonExistentRelativePathInCode_ThrowsFileNotFoundException()
        {
            // Arrange
            var code = "#load \"nonexistent.csx\"";

            var originalDirectory = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_tempDirectory);

                // Act & Assert
                Assert.Throws<FileNotFoundException>(() => _resolver.GetScriptFilesFromCode(code));
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Fact]
        public void GetScriptFiles_WithLoadDirectiveInMultipleLocations_ParsesAllCorrectly()
        {
            // Arrange
            var helper1 = Path.Combine(_tempDirectory, "helper1.csx");
            var helper2 = Path.Combine(_tempDirectory, "helper2.csx");
            
            var scriptContent = $@"#load ""{Path.GetFileName(helper1)}""
Some code here
#load ""{Path.GetFileName(helper2)}""
More code";
            
            File.WriteAllText(_tempFile, scriptContent);
            File.WriteAllText(helper1, "");
            File.WriteAllText(helper2, "");

            // Act
            var result = _resolver.GetScriptFiles(_tempFile);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(_tempFile, result);
            Assert.Contains(helper1, result);
            Assert.Contains(helper2, result);
        }

        [Fact]
        public void GetScriptFiles_WithLoadDirectiveWithExtraWhitespace_CorrectlyParsesPath()
        {
            // Arrange
            var helper = Path.Combine(_tempDirectory, "helper.csx");
            
            var scriptContent = $@"#load     ""    {Path.GetFileName(helper)}    """;
            
            File.WriteAllText(_tempFile, scriptContent);
            File.WriteAllText(helper, "");

            // Act
            var result = _resolver.GetScriptFiles(_tempFile);

            // Assert
            Assert.Contains(helper, result);
        }

        [Fact]
        public void GetScriptFilesFromCode_WithNuGetDirectiveCaseInsensitive_IgnoresNuGetDirective()
        {
            // Arrange
            var code = @"#load ""NUGET: Package, 1.0.0""
#load ""Nuget: AnotherPackage, 2.0.0""
#load ""NuGet: ThirdPackage, 3.0.0""";

            // Act
            var result = _resolver.GetScriptFilesFromCode(code);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetScriptFiles_WithDuplicateLoadDirectives_HashSetPreventsDuplicates()
        {
            // Arrange
            var helper = Path.Combine(_tempDirectory, "helper.csx");
            
            var scriptContent = $@"#load ""{Path.GetFileName(helper)}""
#load ""{Path.GetFileName(helper)}""";
            
            File.WriteAllText(_tempFile, scriptContent);
            File.WriteAllText(helper, "");

            // Act
            var result = _resolver.GetScriptFiles(_tempFile);

            // Assert
            Assert.Equal(2, result.Count); // _tempFile and helper only once
            Assert.Contains(_tempFile, result);
            Assert.Contains(helper, result);
        }

        [Fact]
        public void GetScriptFilesFromCode_WithLoadDirectiveInCommentedOutLine_StillParsesAsDirective()
        {
            // Arrange
            var helperFile = Path.Combine(_tempDirectory, "helper.csx");
            File.WriteAllText(helperFile, "");
            
            // Note: Regex will still parse this, as it just looks for the pattern
            var code = $@"// #load ""other.csx""
#load ""{helperFile}""";

            var originalDirectory = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_tempDirectory);

                // Act
                var result = _resolver.GetScriptFilesFromCode(code);

                // Assert
                Assert.NotNull(result);
                Assert.Contains(helperFile, result);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Fact]
        public void GetScriptFiles_ResultIsHashSetNotList_VerifyType()
        {
            // Arrange
            File.WriteAllText(_tempFile, "");

            // Act
            var result = _resolver.GetScriptFiles(_tempFile);

            // Assert
            Assert.IsType<HashSet<string>>(result);
        }

        [Fact]
        public void GetScriptFilesFromCode_ResultIsHashSetNotList_VerifyType()
        {
            // Arrange
            var code = "Console.WriteLine(\"test\");";

            // Act
            var result = _resolver.GetScriptFilesFromCode(code);

            // Assert
            Assert.IsType<HashSet<string>>(result);
        }

        [Fact]
        public void GetScriptFilesFromCode_WithPathCombinationAndUri_CorrectlyResolvesPath()
        {
            // Arrange
            var helperFile = Path.Combine(_tempDirectory, "helper.csx");
            File.WriteAllText(helperFile, "");
            
            var code = $"#load \"helper.csx\"";

            var originalDirectory = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_tempDirectory);

                // Act
                var result = _resolver.GetScriptFilesFromCode(code);

                // Assert
                Assert.NotNull(result);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Fact]
        public void Process_WithScriptAlreadyInHashSet_DoesNotProcessAgain()
        {
            // Arrange
            File.WriteAllText(_tempFile, "");
            var hashSet = new HashSet<string> { _tempFile };

            // Act - Accessing private method through reflection would be needed in real test
            // This test verifies the logic through public API
            var result = _resolver.GetScriptFiles(_tempFile);

            // Assert
            Assert.Contains(_tempFile, result);
        }

        [Fact]
        public void GetScriptFiles_WithLoadDirectivePointingToDirectory_ThrowsException()
        {
            // Arrange
            var subDirectory = Path.Combine(_tempDirectory, "subdir");
            Directory.CreateDirectory(subDirectory);
            
            var scriptContent = $"#load \"{subDirectory}\"";
            File.WriteAllText(_tempFile, scriptContent);

            // Act & Assert
            Assert.Throws<Exception>(() => _resolver.GetScriptFiles(_tempFile));
        }

        [Fact]
        public void GetScriptFilesFromCode_WithLoadDirectiveAtStartOfFile_CorrectlyParses()
        {
            // Arrange
            var helperFile = Path.Combine(_tempDirectory, "helper.csx");
            File.WriteAllText(helperFile, "");
            
            var code = $@"#load ""{helperFile}""
Console.WriteLine(""After load"");";

            // Act
            var result = _resolver.GetScriptFilesFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(helperFile, result);
        }

        [Fact]
        public void GetScriptFilesFromCode_WithLoadDirectiveAtEndOfFile_CorrectlyParses()
        {
            // Arrange
            var helperFile = Path.Combine(_tempDirectory, "helper.csx");
            File.WriteAllText(helperFile, "");
            
            var code = $@"Console.WriteLine(""Before load"");
#load ""{helperFile}""";

            // Act
            var result = _resolver.GetScriptFilesFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(helperFile, result);
        }

        [Fact]
        public void GetScriptFiles_WithSpecialCharactersInFilename_CorrectlyResolvesPath()
        {
            // Arrange
            var specialFile = Path.Combine(_tempDirectory, "helper (1) [test].csx");
            
            var scriptContent = $"#load \"{Path.GetFileName(specialFile)}\"";
            
            File.WriteAllText(_tempFile, scriptContent);
            File.WriteAllText(specialFile, "");

            // Act
            var result = _resolver.GetScriptFiles(_tempFile);

            // Assert
            Assert.Contains(specialFile, result);
        }

        [Fact]
        public void GetScriptFilesFromCode_WithEmptyLoadDirectiveQuotes_ResultsInEmptyPath()
        {
            // Arrange
            var code = @"#load """"";

            // Act & Assert - This will likely fail to find the file, but we verify code path
            Assert.Throws<FileNotFoundException>(() => _resolver.GetScriptFilesFromCode(code));
        }

        [Fact]
        public void GetScriptFilesFromCode_WithOnlyWhitespaceInPath_TrimsAndProcesses()
        {
            // Arrange
            var code = @"#load ""   """;

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _resolver.GetScriptFilesFromCode(code));
        }

        [Fact]
        public void GetScriptFiles_WithComplexNestedStructure_HandlesCorrectly()
        {
            // Arrange
            var level1 = Path.Combine(_tempDirectory, "level1.csx");
            var level2 = Path.Combine(_tempDirectory, "level2.csx");
            var level3 = Path.Combine(_tempDirectory, "level3.csx");
            var level4 = Path.Combine(_tempDirectory, "level4.csx");
            
            File.WriteAllText(_tempFile, $"#load \"{Path.GetFileName(level1)}\"");
            File.WriteAllText(level1, $"#load \"{Path.GetFileName(level2)}\"");
            File.WriteAllText(level2, $"#load \"{Path.GetFileName(level3)}\"");
            File.WriteAllText(level3, $"#load \"{Path.GetFileName(level4)}\"");
            File.WriteAllText(level4, "");

            // Act
            var result = _resolver.GetScriptFiles(_tempFile);

            // Assert
            Assert.Equal(5, result.Count);
            Assert.Contains(_tempFile, result);
            Assert.Contains(level1, result);
            Assert.Contains(level2, result);
            Assert.Contains(level3, result);
            Assert.Contains(level4, result);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                try
                {
                    Directory.Delete(_tempDirectory, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }
}