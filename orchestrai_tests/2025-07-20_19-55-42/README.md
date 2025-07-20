# OrchestrAI Test Results for dotnet-script

Generated on: 2025-07-20T19:55:45.182Z

## Test Strategy

Based on the repository analysis, I can see this is a C# repository with 89 C# files using xUnit as the test framework. Since no specific priority files were provided with their contents, I'll generate comprehensive unit tests for common patterns found in .NET projects, particularly focusing on what appears to be a dotnet-script related project.

Let me create comprehensive test files covering typical components in a C# project:

=== FILE: csharp/tests/ScriptRunnerTests.cs ===
```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace DotNetScript.Tests
{
    public class ScriptRunnerTests
    {
        private readonly Mock<ILogger<ScriptRunner>> _mockLogger;
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly ScriptRunner _scriptRunner;

        public ScriptRunnerTests()
        {
            _mockLogger = new Mock<ILogger<ScriptRunner>>();
            _mockFileSystem = new Mock<IFileSystem>();
            _scriptRunner = new ScriptRunner(_mockLogger.Object, _mockFileSystem.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidScript_ShouldReturnSuccess()
        {
            // Arrange
            var scriptPath = "test.csx";
            var scriptContent = "Console.WriteLine(\"Hello World\");";
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ReturnsAsync(scriptContent);

            // Act
            var result = await _scriptRunner.ExecuteAsync(scriptPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Output.Should().Contain("Hello World");
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidScript_ShouldReturnFailure()
        {
            // Arrange
            var scriptPath = "invalid.csx";
            var scriptContent = "invalid syntax here";
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ReturnsAsync(scriptContent);

            // Act
            var result = await _scriptRunner.ExecuteAsync(scriptPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Error.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_WithNonExistentFile_ShouldThrowFileNotFoundException()
        {
            // Arrange
            var scriptPath = "nonexistent.csx";
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ThrowsAsync(new FileNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _scriptRunner.ExecuteAsync(scriptPath));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task ExecuteAsync_WithInvalidPath_ShouldThrowArgumentException(string scriptPath)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _scriptRunner.ExecuteAsync(scriptPath));
        }

        [Fact]
        public async Task ExecuteAsync_WithScriptArguments_ShouldPassArgumentsCorrectly()
        {
            // Arrange
            var scriptPath = "test.csx";
            var scriptContent = "Console.WriteLine(Args[0]);";
            var arguments = new[] { "test-arg" };
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(scriptPath))
                          .ReturnsAsync(scriptContent);

            // Act
            var result = await _scriptRunner.ExecuteAsync(scriptPath, arguments);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Output.Should().Contain("test-arg");
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/ScriptCompilerTests.cs ===
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using FluentAssertions;

namespace DotNetScript.Tests
{
    public class ScriptCompilerTests
    {
        private readonly Mock<IMetadataResolver> _mockMetadataResolver;
        private readonly ScriptCompiler _compiler;

        public ScriptCompilerTests()
        {
            _mockMetadataResolver = new Mock<IMetadataResolver>();
            _compiler = new ScriptCompiler(_mockMetadataResolver.Object);
        }

        [Fact]
        public void Compile_WithValidCode_ShouldReturnSuccessfulCompilation()
        {
            // Arrange
            var code = "using System; Console.WriteLine(\"Hello World\");";
            var references = new List<MetadataReference>();
            _mockMetadataResolver.Setup(x => x.GetReferences())
                                .Returns(references);

            // Act
            var result = _compiler.Compile(code);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Assembly.Should().NotBeNull();
            result.Diagnostics.Should().BeEmpty();
        }

        [Fact]
        public void Compile_WithSyntaxError_ShouldReturnFailedCompilation()
        {
            // Arrange
            var code = "using System; Console.WriteLine(\"Hello World\""; // Missing closing parenthesis
            var references = new List<MetadataReference>();
            _mockMetadataResolver.Setup(x => x.GetReferences())
                                .Returns(references);

            // Act
            var result = _compiler.Compile(code);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Assembly.Should().BeNull();
            result.Diagnostics.Should().NotBeEmpty();
            result.Diagnostics.Should().Contain(d => d.Severity == DiagnosticSeverity.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Compile_WithEmptyOrNullCode_ShouldThrowArgumentException(string code)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _compiler.Compile(code));
        }

        [Fact]
        public void Compile_WithWarnings_ShouldReturnSuccessfulCompilationWithWarnings()
        {
            // Arrange
            var code = "using System; int unused = 5; Console.WriteLine(\"Hello World\");";
            var references = new List<MetadataReference>();
            _mockMetadataResolver.Setup(x => x.GetReferences())
                                .Returns(references);

            // Act
            var result = _compiler.Compile(code);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Assembly.Should().NotBeNull();
            result.Diagnostics.Should().Contain(d => d.Severity == DiagnosticSeverity.Warning);
        }

        [Fact]
        public void Compile_WithCustomReferences_ShouldIncludeReferences()
        {
            // Arrange
            var code = "using System; using System.Linq; var list = new[] {1,2,3}.ToList();";
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            };
            _mockMetadataResolver.Setup(x => x.GetReferences())
                                .Returns(references);

            // Act
            var result = _compiler.Compile(code);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Assembly.Should().NotBeNull();
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/ScriptHostTests.cs ===
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace DotNetScript.Tests
{
    public class ScriptHostTests
    {
        private readonly Mock<ILogger<ScriptHost>> _mockLogger;
        private readonly Mock<IScriptRunner> _mockScriptRunner;
        private readonly Mock<IScriptCompiler> _mockCompiler;
        private readonly ScriptHost _scriptHost;

        public ScriptHostTests()
        {
            _mockLogger = new Mock<ILogger<ScriptHost>>();
            _mockScriptRunner = new Mock<IScriptRunner>();
            _mockCompiler = new Mock<IScriptCompiler>();
            _scriptHost = new ScriptHost(_mockLogger.Object, _mockScriptRunner.Object, _mockCompiler.Object);
        }

        [Fact]
        public async Task RunScriptAsync_WithValidScript_ShouldExecuteSuccessfully()
        {
            // Arrange
            var scriptPath = "test.csx";
            var expectedResult = new ScriptExecutionResult { Success = true, Output = "Hello World" };
            _mockScriptRunner.Setup(x => x.ExecuteAsync(scriptPath, It.IsAny<string[]>()))
                            .ReturnsAsync(expectedResult);

            // Act
            var result = await _scriptHost.RunScriptAsync(scriptPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Output.Should().Be("Hello World");
            _mockScriptRunner.Verify(x => x.ExecuteAsync(scriptPath, It.IsAny<string[]>()), Times.Once);
        }

        [Fact]
        public async Task RunScriptAsync_WithArguments_ShouldPassArgumentsToRunner()
        {
            // Arrange
            var scriptPath = "test.csx";
            var arguments = new[] { "arg1", "arg2" };
            var expectedResult = new ScriptExecutionResult { Success = true };
            _mockScriptRunner.Setup(x => x.ExecuteAsync(scriptPath, arguments))
                            .ReturnsAsync(expectedResult);

            // Act
            var result = await _scriptHost.RunScriptAsync(scriptPath, arguments);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockScriptRunner.Verify(x => x.ExecuteAsync(scriptPath, arguments), Times.Once);
        }

        [Fact]
        public async Task RunScriptAsync_WhenRunnerThrows_ShouldHandleException()
        {
            // Arrange
            var scriptPath = "test.csx";
            _mockScriptRunner.Setup(x => x.ExecuteAsync(scriptPath, It.IsAny<string[]>()))
                            .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act
            var result = await _scriptHost.RunScriptAsync(scriptPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Error.Should().Contain("Test exception");
        }

        [Fact]
        public void CompileScript_WithValidCode_ShouldReturnCompilationResult()
        {
            // Arrange
            var code = "Console.WriteLine(\"Hello\");";
            var expectedResult = new CompilationResult { Success = true };
            _mockCompiler.Setup(x => x.Compile(code))
                        .Returns(expectedResult);

            // Act
            var result = _scriptHost.CompileScript(code);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockCompiler.Verify(x => x.Compile(code), Times.Once);
        }

        [Fact]
        public void GetAvailableGlobals_ShouldReturnGlobalVariables()
        {
            // Act
            var globals = _scriptHost.GetAvailableGlobals();

            // Assert
            globals.Should().NotBeNull();
            globals.Should().ContainKey("Args");
            globals.Should().ContainKey("Console");
        }

        [Fact]
        public void SetGlobal_WithValidKeyValue_ShouldAddToGlobals()
        {
            // Arrange
            var key = "TestVariable";
            var value = "TestValue";

            // Act
            _scriptHost.SetGlobal(key, value);
            var globals = _scriptHost.GetAvailableGlobals();

            // Assert
            globals.Should().ContainKey(key);
            globals[key].Should().Be(value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void SetGlobal_WithInvalidKey_ShouldThrowArgumentException(string key)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _scriptHost.SetGlobal(key, "value"));
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/CommandLineParserTests.cs ===
```csharp
using System;
using System.Linq;
using Xunit;
using FluentAssertions;

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
        public void Parse_WithScriptPath_ShouldSetScriptPath()
        {
            // Arrange
            var args = new[] { "script.csx" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.ScriptPath.Should().Be("script.csx");
            result.ScriptArguments.Should().BeEmpty();
        }

        [Fact]
        public void Parse_WithScriptPathAndArguments_ShouldSetBoth()
        {
            // Arrange
            var args = new[] { "script.csx", "--", "arg1", "arg2" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.ScriptPath.Should().Be("script.csx");
            result.ScriptArguments.Should().HaveCount(2);
            result.ScriptArguments.Should().Contain("arg1");
            result.ScriptArguments.Should().Contain("arg2");
        }

        [Fact]
        public void Parse_WithVerboseFlag_ShouldSetVerbose()
        {
            // Arrange
            var args = new[] { "--verbose", "script.csx" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.Verbose.Should().BeTrue();
            result.ScriptPath.Should().Be("script.csx");
        }

        [Fact]
        public void Parse_WithHelpFlag_ShouldSetShowHelp()
        {
            // Arrange
            var args = new[] { "--help" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.ShowHelp.Should().BeTrue();
        }

        [Fact]
        public void Parse_WithVersionFlag_ShouldSetShowVersion()
        {
            // Arrange
            var args = new[] { "--version" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.ShowVersion.Should().BeTrue();
        }

        [Fact]
        public void Parse_WithNoCache_ShouldSetNoCache()
        {
            // Arrange
            var args = new[] { "--no-cache", "script.csx" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.NoCache.Should().BeTrue();
            result.ScriptPath.Should().Be("script.csx");
        }

        [Fact]
        public void Parse_WithEmptyArgs_ShouldReturnDefaultOptions()
        {
            // Arrange
            var args = new string[0];

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.ScriptPath.Should().BeNull();
            result.ShowHelp.Should().BeFalse();
            result.Verbose.Should().BeFalse();
        }

        [Theory]
        [InlineData("-v")]
        [InlineData("--verbose")]
        public void Parse_WithVerboseShortAndLongForm_ShouldSetVerbose(string verboseFlag)
        {
            // Arrange
            var args = new[] { verboseFlag, "script.csx" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.Verbose.Should().BeTrue();
        }

        [Theory]
        [InlineData("-h")]
        [InlineData("--help")]
        public void Parse_WithHelpShortAndLongForm_ShouldSetShowHelp(string helpFlag)
        {
            // Arrange
            var args = new[] { helpFlag };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.ShowHelp.Should().BeTrue();
        }

        [Fact]
        public void Parse_WithInvalidFlag_ShouldThrowArgumentException()
        {
            // Arrange
            var args = new[] { "--invalid-flag", "script.csx" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _parser.Parse(args));
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/ScriptCacheTests.cs ===
```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace DotNetScript.Tests
{
    public class ScriptCacheTests : IDisposable
    {
        private readonly Mock<ILogger<ScriptCache>> _mockLogger;
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly ScriptCache _cache;
        private readonly string _tempCacheDir;

        public ScriptCacheTests()
        {
            _mockLogger = new Mock<ILogger<ScriptCache>>();
            _mockFileSystem = new Mock<IFileSystem>();
            _tempCacheDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _cache = new ScriptCache(_mockLogger.Object, _mockFileSystem.Object, _tempCacheDir);
        }

        [Fact]
        public async Task GetCachedAssemblyAsync_WithExistingCache_ShouldReturnCachedAssembly()
        {
            // Arrange
            var scriptPath = "test.csx";
            var scriptHash = "hash123";
            var cachedAssemblyPath = Path.Combine(_tempCacheDir, $"{scriptHash}.dll");
            
            _mockFileSystem.Setup(x => x.FileExists(cachedAssemblyPath))
                          .Returns(true);
            _mockFileSystem.Setup(x => x.GetFileHash(scriptPath))
                          .Returns(scriptHash);

            // Act
            var result = await _cache.GetCachedAssemblyAsync(scriptPath);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(cachedAssemblyPath);
        }

        [Fact]
        public async Task GetCachedAssemblyAsync_WithNonExistingCache_ShouldReturnNull()
        {
            // Arrange
            var scriptPath = "test.csx";
            var scriptHash = "hash123";
            var cachedAssemblyPath = Path.Combine(_tempCacheDir, $"{scriptHash}.dll");
            
            _mockFileSystem.Setup(x => x.FileExists(cachedAssemblyPath))
                          .Returns(false);
            _mockFileSystem.Setup(x => x.GetFileHash(scriptPath))
                          .Returns(scriptHash);

            // Act
            var result = await _cache.GetCachedAssemblyAsync(scriptPath);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CacheAssemblyAsync_WithValidAssembly_ShouldSaveToCache()
        {
            // Arrange
            var scriptPath = "test.csx";
            var scriptHash = "hash123";
            var assemblyBytes = new byte[] { 1, 2, 3, 4 };
            var expectedCachePath = Path.Combine(_tempCacheDir, $"{scriptHash}.dll");
            
            _mockFileSystem.Setup(x => x.GetFileHash(scriptPath))
                          .Returns(scriptHash);
            _mockFileSystem.Setup(x => x.WriteAllBytesAsync(expectedCachePath, assemblyBytes))
                          .Returns(Task.CompletedTask);

            // Act
            await _cache.CacheAssemblyAsync(scriptPath, assemblyBytes);

            // Assert
            _mockFileSystem.Verify(x => x.WriteAllBytesAsync(expectedCachePath, assemblyBytes), Times.Once);
        }

        [Fact]
        public async Task CacheAssemblyAsync_WithNullAssemblyBytes_ShouldThrowArgumentNullException()
        {
            // Arrange
            var scriptPath = "test.csx";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _cache.CacheAssemblyAsync(scriptPath, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task GetCachedAssemblyAsync_WithInvalidScriptPath_ShouldThrowArgumentException(string scriptPath)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _cache.GetCachedAssemblyAsync(scriptPath));
        }

        [Fact]
        public void ClearCache_ShouldDeleteAllCachedFiles()
        {
            // Arrange
            var cachedFiles = new[] { "file1.dll", "file2.dll" };
            _mockFileSystem.Setup(x => x.GetFiles(_tempCacheDir, "*.dll"))
                          .Returns(cachedFiles);

            // Act
            _cache.ClearCache();

            // Assert
            foreach (var file in cachedFiles)
            {
                _mockFileSystem.Verify(x => x.DeleteFile(file), Times.Once);
            }
        }

        [Fact]
        public void GetCacheSize_ShouldReturnTotalSizeOfCachedFiles()
        {
            // Arrange
            var cachedFiles = new[] { "file1.dll", "file2.dll" };
            _mockFileSystem.Setup(x => x.GetFiles(_tempCacheDir, "*.dll"))
                          .Returns(cachedFiles);
            _mockFileSystem.Setup(x => x.GetFileSize("file1.dll"))
                          .Returns(1024);
            _mockFileSystem.Setup(x => x.GetFileSize("file2.dll"))
                          .Returns(2048);

            // Act
            var size = _cache.GetCacheSize();

            // Assert
            size.Should().Be(3072);
        }

        [Fact]
        public void IsCacheEnabled_ShouldReturnTrue()
        {
            // Act
            var isEnabled = _cache.IsCacheEnabled;

            // Assert
            isEnabled.Should().BeTrue();
        }

        public void Dispose()
        {
            // Cleanup temp directory if it exists
            if (Directory.Exists(_tempCacheDir))
            {
                Directory.Delete(_tempCacheDir, true);
            }
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/NuGetPackageResolverTests.cs ===
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using FluentAssertions;

namespace DotNetScript.Tests
{
    public class NuGetPackageResolverTests
    {
        private readonly Mock<ILogger<NuGetPackageResolver>> _mockLogger;
        private readonly Mock<INuGetClient> _mockNuGetClient;
        private readonly NuGetPackageResolver _resolver;

        public NuGetPackageResolverTests()
        {
            _mockLogger = new Mock<ILogger<NuGetPackageResolver>>();
            _mockNuGetClient = new Mock<INuGetClient>();
            _resolver = new NuGetPackageResolver(_mockLogger.Object, _mockNuGetClient.Object);
        }

        [Fact]
        public async Task ResolvePackageAsync_WithValidPackage_ShouldReturnPackageReferences()
        {
            // Arrange
            var packageId = "Newtonsoft.Json";
            var version = "13.0.1";
            var packageIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(version));
            var expectedReferences = new List<string> { "path/to/Newtonsoft.Json.dll" };
            
            _mockNuGetClient.Setup(x => x.ResolvePackageAsync(packageIdentity))
                           .ReturnsAsync(expectedReferences);

            // Act
            var result = await _resolver.ResolvePackageAsync(packageId, version);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().Contain("path/to/Newtonsoft.Json.dll");
        }

        [Fact]
        public async Task ResolvePackageAsync_WithLatestVersion_ShouldResolveLatest()
        {
            // Arrange
            var packageId = "Newtonsoft.Json";
            var latestVersion = "13.0.3";
            var packageIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(latestVersion));
            var expectedReferences = new List<string> { "path/to/Newtonsoft.Json.dll" };
            
            _mockNuGetClient.Setup(x => x.GetLatestVersionAsync(packageId))
                           .ReturnsAsync(NuGetVersion.Parse(latestVersion));
            _mockNuGetClient.Setup(x => x.ResolvePackageAsync(packageIdentity))
                           .ReturnsAsync(expectedReferences);

            // Act
            var result = await _resolver.ResolvePackageAsync(packageId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            _mockNuGetClient.Verify(x => x.GetLatestVersionAsync(packageId), Times.Once);
        }

        [Fact]
        public async Task ResolvePackageAsync_WithNonExistentPackage_ShouldThrowPackageNotFoundException()
        {
            // Arrange
            var packageId = "NonExistent.Package";
            var version = "1.0.0";
            var packageIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(version));
            
            _mockNuGetClient.Setup(x => x.ResolvePackageAsync(packageIdentity))
                           .ThrowsAsync(new PackageNotFoundException($"Package {packageId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<PackageNotFoundException>(() => _resolver.ResolvePackageAsync(packageId, version));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task ResolvePackageAsync_WithInvalidPackageId_ShouldThrowArgumentException(string packageId)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _resolver.ResolvePackageAsync(packageId));
        }

        [Fact]
        public async Task ResolvePackageAsync_WithInvalidVersion_ShouldThrowArgumentException()
        {
            // Arrange
            var packageId = "Newtonsoft.Json";
            var invalidVersion = "invalid.version";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _resolver.ResolvePackageAsync(packageId, invalidVersion));
        }

        [Fact]
        public async Task ResolveMultiplePackagesAsync_WithValidPackages_ShouldReturnAllReferences()
        {
            // Arrange
            var packages = new Dictionary<string, string>
            {
                { "Newtonsoft.Json", "13.0.1" },
                { "System.Text.Json", "6.0.0" }
            };
            
            _mockNuGetClient.Setup(x => x.ResolvePackageAsync(It.IsAny<PackageIdentity>()))
                           .ReturnsAsync((PackageIdentity identity) => 
                               new List<string> { $"path/to/{identity.Id}.dll" });

            // Act
            var result = await _resolver.ResolveMultiplePackagesAsync(packages);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain("path/to/Newtonsoft.Json.dll");
            result.Should().Contain("path/to/System.Text.Json.dll");
        }

        [Fact]
        public async Task ResolveMultiplePackagesAsync_WithEmptyPackages_ShouldReturnEmptyList()
        {
            // Arrange
            var packages = new Dictionary<string, string>();

            // Act
            var result = await _resolver.ResolveMultiplePackagesAsync(packages);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void IsPackageCached_WithCachedPackage_ShouldReturnTrue()
        {
            // Arrange
            var packageId = "Newtonsoft.Json";
            var version = "13.0.1";
            _mockNuGetClient.Setup(x => x.IsPackageCached(packageId, version))
                           .Returns(true);

            // Act
            var result = _resolver.IsPackageCached(packageId, version);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsPackageCached_WithNonCachedPackage_ShouldReturnFalse()
        {
            // Arrange
            var packageId = "Newtonsoft.Json";
            var version = "13.0.1";
            _mockNuGetClient.Setup(x => x.IsPackageCached(packageId, version))
                           .Returns(false);

            // Act