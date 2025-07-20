# OrchestrAI Test Results for dotnet-script

Generated on: 2025-07-20T20:01:13.343Z

## Test Strategy

Based on the repository analysis, I can see this is a C# repository with 89 C# files using xUnit as the test framework. Since no priority files were provided with detailed content, I'll generate comprehensive unit tests for common patterns found in .NET projects, particularly focusing on what appears to be a dotnet-script project.

Let me create comprehensive test files covering the most likely core components:

=== FILE: csharp/tests/ScriptRunnerTests.cs ===
```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using DotNetScript.Core;
using DotNetScript.Models;

namespace DotNetScript.Tests
{
    public class ScriptRunnerTests
    {
        private readonly Mock<ILogger<ScriptRunner>> _mockLogger;
        private readonly Mock<IScriptCompiler> _mockCompiler;
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly ScriptRunner _scriptRunner;

        public ScriptRunnerTests()
        {
            _mockLogger = new Mock<ILogger<ScriptRunner>>();
            _mockCompiler = new Mock<IScriptCompiler>();
            _mockFileSystem = new Mock<IFileSystem>();
            _scriptRunner = new ScriptRunner(_mockLogger.Object, _mockCompiler.Object, _mockFileSystem.Object);
        }

        [Fact]
        public async Task RunScriptAsync_ValidScript_ReturnsSuccess()
        {
            // Arrange
            var scriptPath = "test.csx";
            var scriptContent = "Console.WriteLine(\"Hello World\");";
            var expectedResult = new ScriptResult { Success = true, Output = "Hello World" };

            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ReturnsAsync(scriptContent);
            _mockCompiler.Setup(x => x.CompileAndRunAsync(scriptContent))
                        .ReturnsAsync(expectedResult);

            // Act
            var result = await _scriptRunner.RunScriptAsync(scriptPath);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Hello World", result.Output);
            _mockFileSystem.Verify(x => x.ReadAllTextAsync(scriptPath), Times.Once);
            _mockCompiler.Verify(x => x.CompileAndRunAsync(scriptContent), Times.Once);
        }

        [Fact]
        public async Task RunScriptAsync_FileNotFound_ThrowsFileNotFoundException()
        {
            // Arrange
            var scriptPath = "nonexistent.csx";
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ThrowsAsync(new FileNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _scriptRunner.RunScriptAsync(scriptPath));
        }

        [Fact]
        public async Task RunScriptAsync_CompilationError_ReturnsFailure()
        {
            // Arrange
            var scriptPath = "invalid.csx";
            var scriptContent = "invalid C# code";
            var expectedResult = new ScriptResult { Success = false, Error = "Compilation failed" };

            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ReturnsAsync(scriptContent);
            _mockCompiler.Setup(x => x.CompileAndRunAsync(scriptContent))
                        .ReturnsAsync(expectedResult);

            // Act
            var result = await _scriptRunner.RunScriptAsync(scriptPath);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Compilation failed", result.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task RunScriptAsync_EmptyOrNullPath_ThrowsArgumentException(string scriptPath)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _scriptRunner.RunScriptAsync(scriptPath));
        }

        [Fact]
        public async Task RunScriptWithArgsAsync_ValidScriptWithArgs_ReturnsSuccess()
        {
            // Arrange
            var scriptPath = "test.csx";
            var args = new[] { "arg1", "arg2" };
            var scriptContent = "Console.WriteLine(Args[0]);";
            var expectedResult = new ScriptResult { Success = true, Output = "arg1" };

            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ReturnsAsync(scriptContent);
            _mockCompiler.Setup(x => x.CompileAndRunAsync(scriptContent, args))
                        .ReturnsAsync(expectedResult);

            // Act
            var result = await _scriptRunner.RunScriptWithArgsAsync(scriptPath, args);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("arg1", result.Output);
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/ScriptCompilerTests.cs ===
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using DotNetScript.Core;
using DotNetScript.Models;

namespace DotNetScript.Tests
{
    public class ScriptCompilerTests
    {
        private readonly Mock<IAssemblyResolver> _mockAssemblyResolver;
        private readonly ScriptCompiler _compiler;

        public ScriptCompilerTests()
        {
            _mockAssemblyResolver = new Mock<IAssemblyResolver>();
            _compiler = new ScriptCompiler(_mockAssemblyResolver.Object);
        }

        [Fact]
        public async Task CompileAndRunAsync_SimpleScript_ReturnsSuccess()
        {
            // Arrange
            var script = "return 42;";
            _mockAssemblyResolver.Setup(x => x.GetReferences())
                                .Returns(new List<MetadataReference>());

            // Act
            var result = await _compiler.CompileAndRunAsync(script);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("42", result.Output);
        }

        [Fact]
        public async Task CompileAndRunAsync_ScriptWithSyntaxError_ReturnsFailure()
        {
            // Arrange
            var script = "invalid syntax {{{";

            // Act
            var result = await _compiler.CompileAndRunAsync(script);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Contains("syntax", result.Error.ToLower());
        }

        [Fact]
        public async Task CompileAndRunAsync_ScriptWithRuntimeError_ReturnsFailure()
        {
            // Arrange
            var script = "throw new Exception(\"Runtime error\");";

            // Act
            var result = await _compiler.CompileAndRunAsync(script);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Runtime error", result.Error);
        }

        [Fact]
        public async Task CompileAndRunAsync_ScriptWithConsoleOutput_CapturesOutput()
        {
            // Arrange
            var script = "Console.WriteLine(\"Hello World\");";

            // Act
            var result = await _compiler.CompileAndRunAsync(script);

            // Assert
            Assert.True(result.Success);
            Assert.Contains("Hello World", result.Output);
        }

        [Fact]
        public async Task CompileAndRunAsync_ScriptWithImports_ReturnsSuccess()
        {
            // Arrange
            var script = @"
                using System.Linq;
                var numbers = new[] { 1, 2, 3, 4, 5 };
                return numbers.Sum();
            ";

            // Act
            var result = await _compiler.CompileAndRunAsync(script);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("15", result.Output);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task CompileAndRunAsync_EmptyOrNullScript_ThrowsArgumentException(string script)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _compiler.CompileAndRunAsync(script));
        }

        [Fact]
        public async Task CompileAndRunAsync_ScriptWithArgs_PassesArgsCorrectly()
        {
            // Arrange
            var script = "return Args[0] + Args[1];";
            var args = new[] { "Hello", " World" };

            // Act
            var result = await _compiler.CompileAndRunAsync(script, args);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Hello World", result.Output);
        }

        [Fact]
        public void ValidateScript_ValidScript_ReturnsTrue()
        {
            // Arrange
            var script = "Console.WriteLine(\"Valid\");";

            // Act
            var isValid = _compiler.ValidateScript(script);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateScript_InvalidScript_ReturnsFalse()
        {
            // Arrange
            var script = "invalid syntax {{{";

            // Act
            var isValid = _compiler.ValidateScript(script);

            // Assert
            Assert.False(isValid);
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/CommandLineParserTests.cs ===
```csharp
using System;
using Xunit;
using DotNetScript.CLI;
using DotNetScript.Models;

namespace DotNetScript.Tests
{
    public class CommandLineParserTests
    {
        private readonly CommandLineParser _parser;

        public CommandLineParserTests()
        {
            _parser = new CommandLineParser();
        }

        [Fact]
        public void Parse_ValidScriptPath_ReturnsCorrectOptions()
        {
            // Arrange
            var args = new[] { "script.csx" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.Equal("script.csx", options.ScriptPath);
            Assert.Empty(options.ScriptArgs);
            Assert.False(options.ShowHelp);
            Assert.False(options.ShowVersion);
        }

        [Fact]
        public void Parse_ScriptWithArguments_ReturnsCorrectOptions()
        {
            // Arrange
            var args = new[] { "script.csx", "--", "arg1", "arg2" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.Equal("script.csx", options.ScriptPath);
            Assert.Equal(new[] { "arg1", "arg2" }, options.ScriptArgs);
        }

        [Fact]
        public void Parse_HelpFlag_ReturnsHelpOption()
        {
            // Arrange
            var args = new[] { "--help" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.True(options.ShowHelp);
        }

        [Fact]
        public void Parse_VersionFlag_ReturnsVersionOption()
        {
            // Arrange
            var args = new[] { "--version" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.True(options.ShowVersion);
        }

        [Fact]
        public void Parse_VerboseFlag_ReturnsVerboseOption()
        {
            // Arrange
            var args = new[] { "script.csx", "--verbose" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.True(options.Verbose);
        }

        [Fact]
        public void Parse_NoCache_ReturnsNoCacheOption()
        {
            // Arrange
            var args = new[] { "script.csx", "--no-cache" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.True(options.NoCache);
        }

        [Fact]
        public void Parse_EmptyArgs_ThrowsArgumentException()
        {
            // Arrange
            var args = new string[0];

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _parser.Parse(args));
        }

        [Theory]
        [InlineData(new[] { "-h" })]
        [InlineData(new[] { "/?" })]
        public void Parse_HelpShortcuts_ReturnsHelpOption(string[] args)
        {
            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.True(options.ShowHelp);
        }

        [Fact]
        public void Parse_ConfigFile_ReturnsConfigOption()
        {
            // Arrange
            var args = new[] { "script.csx", "--config", "config.json" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.Equal("config.json", options.ConfigFile);
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/ScriptResultTests.cs ===
```csharp
using System;
using Xunit;
using DotNetScript.Models;

namespace DotNetScript.Tests
{
    public class ScriptResultTests
    {
        [Fact]
        public void Constructor_Default_InitializesCorrectly()
        {
            // Act
            var result = new ScriptResult();

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Output);
            Assert.Null(result.Error);
            Assert.Equal(TimeSpan.Zero, result.ExecutionTime);
        }

        [Fact]
        public void Success_WhenTrue_ErrorShouldBeNull()
        {
            // Arrange
            var result = new ScriptResult
            {
                Success = true,
                Output = "Success output"
            };

            // Act & Assert
            Assert.True(result.Success);
            Assert.Equal("Success output", result.Output);
            Assert.Null(result.Error);
        }

        [Fact]
        public void Success_WhenFalse_OutputShouldBeNull()
        {
            // Arrange
            var result = new ScriptResult
            {
                Success = false,
                Error = "Error message"
            };

            // Act & Assert
            Assert.False(result.Success);
            Assert.Equal("Error message", result.Error);
            Assert.Null(result.Output);
        }

        [Fact]
        public void ExecutionTime_SetValue_ReturnsCorrectValue()
        {
            // Arrange
            var expectedTime = TimeSpan.FromMilliseconds(500);
            var result = new ScriptResult
            {
                ExecutionTime = expectedTime
            };

            // Act & Assert
            Assert.Equal(expectedTime, result.ExecutionTime);
        }

        [Fact]
        public void CreateSuccess_StaticMethod_ReturnsSuccessResult()
        {
            // Arrange
            var output = "Test output";
            var executionTime = TimeSpan.FromMilliseconds(100);

            // Act
            var result = ScriptResult.CreateSuccess(output, executionTime);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(output, result.Output);
            Assert.Null(result.Error);
            Assert.Equal(executionTime, result.ExecutionTime);
        }

        [Fact]
        public void CreateFailure_StaticMethod_ReturnsFailureResult()
        {
            // Arrange
            var error = "Test error";
            var executionTime = TimeSpan.FromMilliseconds(50);

            // Act
            var result = ScriptResult.CreateFailure(error, executionTime);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(error, result.Error);
            Assert.Null(result.Output);
            Assert.Equal(executionTime, result.ExecutionTime);
        }

        [Fact]
        public void ToString_SuccessResult_ReturnsFormattedString()
        {
            // Arrange
            var result = ScriptResult.CreateSuccess("Hello World", TimeSpan.FromMilliseconds(100));

            // Act
            var stringResult = result.ToString();

            // Assert
            Assert.Contains("Success", stringResult);
            Assert.Contains("Hello World", stringResult);
            Assert.Contains("100", stringResult);
        }

        [Fact]
        public void ToString_FailureResult_ReturnsFormattedString()
        {
            // Arrange
            var result = ScriptResult.CreateFailure("Compilation error", TimeSpan.FromMilliseconds(50));

            // Act
            var stringResult = result.ToString();

            // Assert
            Assert.Contains("Failure", stringResult);
            Assert.Contains("Compilation error", stringResult);
            Assert.Contains("50", stringResult);
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/FileSystemTests.cs ===
```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using DotNetScript.Infrastructure;

namespace DotNetScript.Tests
{
    public class FileSystemTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly FileSystem _fileSystem;

        public FileSystemTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _fileSystem = new FileSystem();
        }

        [Fact]
        public async Task ReadAllTextAsync_ExistingFile_ReturnsContent()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "test.txt");
            var expectedContent = "Hello World";
            await File.WriteAllTextAsync(filePath, expectedContent);

            // Act
            var content = await _fileSystem.ReadAllTextAsync(filePath);

            // Assert
            Assert.Equal(expectedContent, content);
        }

        [Fact]
        public async Task ReadAllTextAsync_NonExistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "nonexistent.txt");

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _fileSystem.ReadAllTextAsync(filePath));
        }

        [Fact]
        public async Task WriteAllTextAsync_ValidPath_WritesContent()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "output.txt");
            var content = "Test content";

            // Act
            await _fileSystem.WriteAllTextAsync(filePath, content);

            // Assert
            var writtenContent = await File.ReadAllTextAsync(filePath);
            Assert.Equal(content, writtenContent);
        }

        [Fact]
        public void Exists_ExistingFile_ReturnsTrue()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "existing.txt");
            File.WriteAllText(filePath, "content");

            // Act
            var exists = _fileSystem.Exists(filePath);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public void Exists_NonExistentFile_ReturnsFalse()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "nonexistent.txt");

            // Act
            var exists = _fileSystem.Exists(filePath);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public void GetFullPath_RelativePath_ReturnsAbsolutePath()
        {
            // Arrange
            var relativePath = "test.txt";

            // Act
            var fullPath = _fileSystem.GetFullPath(relativePath);

            // Assert
            Assert.True(Path.IsPathFullyQualified(fullPath));
            Assert.EndsWith("test.txt", fullPath);
        }

        [Fact]
        public void GetDirectoryName_FilePath_ReturnsDirectory()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "test.txt");

            // Act
            var directory = _fileSystem.GetDirectoryName(filePath);

            // Assert
            Assert.Equal(_testDirectory, directory);
        }

        [Fact]
        public void GetFileName_FilePath_ReturnsFileName()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "test.txt");

            // Act
            var fileName = _fileSystem.GetFileName(filePath);

            // Assert
            Assert.Equal("test.txt", fileName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task ReadAllTextAsync_EmptyOrNullPath_ThrowsArgumentException(string path)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _fileSystem.ReadAllTextAsync(path));
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/AssemblyResolverTests.cs ===
```csharp
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using Microsoft.CodeAnalysis;
using DotNetScript.Core;

namespace DotNetScript.Tests
{
    public class AssemblyResolverTests
    {
        private readonly AssemblyResolver _resolver;

        public AssemblyResolverTests()
        {
            _resolver = new AssemblyResolver();
        }

        [Fact]
        public void GetReferences_Default_ReturnsSystemReferences()
        {
            // Act
            var references = _resolver.GetReferences();

            // Assert
            Assert.NotEmpty(references);
            Assert.Contains(references, r => r.Display.Contains("System.Runtime"));
            Assert.Contains(references, r => r.Display.Contains("System.Console"));
        }

        [Fact]
        public void GetReferences_IncludesNetCoreApp_ReturnsNetCoreReferences()
        {
            // Act
            var references = _resolver.GetReferences();

            // Assert
            Assert.Contains(references, r => r.Display.Contains("netcoreapp") || r.Display.Contains("net6.0") || r.Display.Contains("net7.0"));
        }

        [Fact]
        public void AddReference_ValidAssembly_AddsToReferences()
        {
            // Arrange
            var assembly = typeof(System.Linq.Enumerable).Assembly;

            // Act
            _resolver.AddReference(assembly);
            var references = _resolver.GetReferences();

            // Assert
            Assert.Contains(references, r => r.Display.Contains("System.Linq"));
        }

        [Fact]
        public void AddReference_NullAssembly_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _resolver.AddReference(null));
        }

        [Fact]
        public void AddReferenceByName_ValidAssemblyName_AddsToReferences()
        {
            // Arrange
            var assemblyName = "System.Text.Json";

            // Act
            _resolver.AddReferenceByName(assemblyName);
            var references = _resolver.GetReferences();

            // Assert
            Assert.Contains(references, r => r.Display.Contains("System.Text.Json"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddReferenceByName_EmptyOrNullName_ThrowsArgumentException(string assemblyName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _resolver.AddReferenceByName(assemblyName));
        }

        [Fact]
        public void AddReferenceByName_NonExistentAssembly_ThrowsFileNotFoundException()
        {
            // Arrange
            var assemblyName = "NonExistent.Assembly";

            // Act & Assert
            Assert.Throws<System.IO.FileNotFoundException>(() => _resolver.AddReferenceByName(assemblyName));
        }

        [Fact]
        public void GetUsings_Default_ReturnsCommonUsings()
        {
            // Act
            var usings = _resolver.GetUsings();

            // Assert
            Assert.Contains("System", usings);
            Assert.Contains("System.Collections.Generic", usings);
            Assert.Contains("System.Linq", usings);
            Assert.Contains("System.Threading.Tasks", usings);
        }

        [Fact]
        public void AddUsing_ValidNamespace_AddsToUsings()
        {
            // Arrange
            var namespaceName = "System.Text.Json";

            // Act
            _resolver.AddUsing(namespaceName);
            var usings = _resolver.GetUsings();

            // Assert
            Assert.Contains(namespaceName, usings);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddUsing_EmptyOrNullNamespace_ThrowsArgumentException(string namespaceName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _resolver.AddUsing(namespaceName));
        }

        [Fact]
        public void Reset_ClearsCustomReferencesAndUsings()
        {
            // Arrange
            _resolver.AddReferenceByName("System.Text.Json");
            _resolver.AddUsing("System.Text.Json");

            // Act
            _resolver.Reset();
            var references = _resolver.GetReferences();
            var usings = _resolver.GetUsings();

            // Assert
            Assert.DoesNotContain(references, r => r.Display.Contains("System.Text.Json"));
            Assert.DoesNotContain("System.Text.Json", usings);
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/ConfigurationManagerTests.cs ===
```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using DotNetScript.Configuration;
using DotNetScript.Models;

namespace DotNetScript.Tests
{
    public class ConfigurationManagerTests : IDisposable
    {
        private readonly Mock<ILogger<ConfigurationManager>> _mockLogger;
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly ConfigurationManager _configManager;
        private readonly string _testConfigPath;

        public ConfigurationManagerTests()
        {
            _mockLogger = new Mock<ILogger<ConfigurationManager>>();
            _mockFileSystem = new Mock<IFileSystem>();
            _configManager = new ConfigurationManager(_mockLogger.Object, _mockFileSystem.Object);
            _testConfigPath = Path.Combine(Path.GetTempPath(), "test-config.json");
        }

        [Fact]
        public async Task LoadConfigurationAsync_ValidConfigFile_ReturnsConfiguration()
        {
            // Arrange
            var configJson = @"{
                ""DefaultReferences"": [""System.Text.Json""],
                ""DefaultUsings"": [""System.Text.Json""],
                ""CacheEnabled"": true,
                ""Verbose"": false
            }";

            _mockFileSystem.Setup(x => x.Exists(_testConfigPath)).Returns(true);
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(_testConfigPath)).ReturnsAsync(configJson);

            // Act
            var config = await _configManager.LoadConfigurationAsync(_testConfigPath);

            // Assert
            Assert.NotNull(config);
            Assert.Contains("System.Text.Json", config.DefaultReferences);
            Assert.Contains("System.Text.Json", config.DefaultUsings);
            Assert.True(config.CacheEnabled);
            Assert.False(config.Verbose);
        }

        [Fact]
        public async Task LoadConfigurationAsync_NonExistentFile_ReturnsDefaultConfiguration()
        {
            // Arrange
            _mockFileSystem.Setup(x => x.Exists(_testConfigPath)).Returns(false);

            // Act
            var config = await _configManager.LoadConfigurationAsync(_testConfigPath);

            // Assert
            Assert.NotNull(config);
            Assert.True(config.CacheEnabled);
            Assert.False(config.Verbose);
        }

        [Fact]
        public async Task LoadConfigurationAsync_InvalidJson_ThrowsInvalidOperationException()
        {
            // Arrange
            var invalidJson = "{ invalid json }";
            _mockFileSystem.Setup(x => x.Exists(_testConfigPath)).Returns(true);
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(_testConfigPath)).ReturnsAsync(invalidJson);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _configManager.LoadConfigurationAsync(_testConfigPath));
        }

        [Fact]
        public async Task SaveConfigurationAsync_ValidConfiguration_SavesCorrectly()
        {
            // Arrange
            var config = new ScriptConfiguration
            {
                DefaultReferences = new[] { "System.Text.Json" },
                DefaultUsings = new[] { "System.Text.Json" },
                CacheEnabled = false,
                Verbose = true
            };

            string savedContent = null;
            _mockFileSystem.Setup(x => x.WriteAllTextAsync(_testConfigPath, It.IsAny<string>()))
                          .Callback<string, string>((path, content) => savedContent = content)
                          .Returns(Task.CompletedTask);

            // Act
            await _configManager.SaveConfigurationAsync(_testConfigPath, config);

            // Assert
            Assert.NotNull(savedContent);
            Assert.Contains("System.Text.Json", savedContent);
            Assert.Contains("\"CacheEnabled\": false", savedContent);
            Assert.Contains("\"Verbose\": true", savedContent);
        }

        [Fact]
        public async Task SaveConfigurationAsync_NullConfiguration_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _configManager.SaveConfigurationAsync(_testConfigPath, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task LoadConfigurationAsync_EmptyOrNullPath_ThrowsArgumentException(string path)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _configManager.LoadConfigurationAsync(path));
        }

        [Fact]
        public void GetDefaultConfiguration_ReturnsValidDefaults()
        {
            // Act
            var config = _configManager.GetDefaultConfiguration();

            // Assert
            Assert.NotNull(config);
            Assert.True(config.CacheEnabled);
            Assert.False(config.Verbose);
            Assert.NotEmpty(config.DefaultReferences);
            Assert.NotEmpty(config.DefaultUsings);
        }

        [Fact]
        public async Task MergeWithCommandLineOptions_ValidOptions_Merges