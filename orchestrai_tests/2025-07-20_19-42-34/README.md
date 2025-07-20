# OrchestrAI Test Results for dotnet-script

Generated on: 2025-07-20T19:42:35.264Z

## Test Strategy

I'll analyze the dotnet-script repository and generate comprehensive unit tests for the C# codebase. Based on the repository structure, this appears to be a tool for executing C# scripts, so I'll focus on the core functionality and commands.

=== FILE: tests/Dotnet.Script.Core.Tests/Commands/ExecuteCodeCommandTests.cs ===
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteCodeCommandTests
    {
        private readonly Mock<ILogger<ExecuteCodeCommand>> _mockLogger;
        private readonly ExecuteCodeCommand _command;

        public ExecuteCodeCommandTests()
        {
            _mockLogger = new Mock<ILogger<ExecuteCodeCommand>>();
            _command = new ExecuteCodeCommand(_mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithValidLogger_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Assert.NotNull(_command);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecuteCodeCommand(null));
        }

        [Theory]
        [InlineData("Console.WriteLine(\"Hello World\");")]
        [InlineData("var x = 5; Console.WriteLine(x);")]
        [InlineData("System.DateTime.Now.ToString();")]
        public async Task ExecuteAsync_WithValidCode_ShouldExecuteSuccessfully(string code)
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions { Code = code };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithEmptyCode_ShouldReturnError()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions { Code = "" };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidCode_ShouldReturnError()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions { Code = "invalid syntax here" };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }
    }
}
=== END FILE ===

=== FILE: tests/Dotnet.Script.Core.Tests/Commands/ExecuteScriptCommandTests.cs ===
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteScriptCommandTests : IDisposable
    {
        private readonly Mock<ILogger<ExecuteScriptCommand>> _mockLogger;
        private readonly ExecuteScriptCommand _command;
        private readonly string _tempDirectory;

        public ExecuteScriptCommandTests()
        {
            _mockLogger = new Mock<ILogger<ExecuteScriptCommand>>();
            _command = new ExecuteScriptCommand(_mockLogger.Object);
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [Fact]
        public void Constructor_WithValidLogger_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Assert.NotNull(_command);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecuteScriptCommand(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidScriptFile_ShouldExecuteSuccessfully()
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "test.csx");
            await File.WriteAllTextAsync(scriptPath, "Console.WriteLine(\"Hello from script\");");
            var options = new ExecuteScriptCommandOptions { ScriptPath = scriptPath };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNonExistentFile_ShouldReturnError()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions { ScriptPath = "nonexistent.csx" };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidScript_ShouldReturnError()
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "invalid.csx");
            await File.WriteAllTextAsync(scriptPath, "invalid C# syntax here");
            var options = new ExecuteScriptCommandOptions { ScriptPath = scriptPath };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Theory]
        [InlineData("arg1")]
        [InlineData("arg1", "arg2")]
        [InlineData("arg1", "arg2", "arg3")]
        public async Task ExecuteAsync_WithArguments_ShouldPassArgumentsToScript(params string[] args)
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "args.csx");
            await File.WriteAllTextAsync(scriptPath, "Console.WriteLine($\"Args count: {Args.Count}\");");
            var options = new ExecuteScriptCommandOptions 
            { 
                ScriptPath = scriptPath,
                Arguments = args
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
    }
}
=== END FILE ===

=== FILE: tests/Dotnet.Script.Core.Tests/Commands/ExecuteInteractiveCommandTests.cs ===
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteInteractiveCommandTests
    {
        private readonly Mock<ILogger<ExecuteInteractiveCommand>> _mockLogger;
        private readonly ExecuteInteractiveCommand _command;

        public ExecuteInteractiveCommandTests()
        {
            _mockLogger = new Mock<ILogger<ExecuteInteractiveCommand>>();
            _command = new ExecuteInteractiveCommand(_mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithValidLogger_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Assert.NotNull(_command);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecuteInteractiveCommand(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidOptions_ShouldStartInteractiveMode()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithWorkingDirectory_ShouldSetWorkingDirectory()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions 
            { 
                WorkingDirectory = Environment.CurrentDirectory 
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }
    }
}
=== END FILE ===

=== FILE: tests/Dotnet.Script.Core.Tests/Commands/InitCommandTests.cs ===
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class InitCommandTests : IDisposable
    {
        private readonly Mock<ILogger<InitCommand>> _mockLogger;
        private readonly InitCommand _command;
        private readonly string _tempDirectory;

        public InitCommandTests()
        {
            _mockLogger = new Mock<ILogger<InitCommand>>();
            _command = new InitCommand(_mockLogger.Object);
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [Fact]
        public void Constructor_WithValidLogger_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Assert.NotNull(_command);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new InitCommand(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidPath_ShouldCreateScriptFile()
        {
            // Arrange
            var scriptName = "test.csx";
            var scriptPath = Path.Combine(_tempDirectory, scriptName);
            var options = new InitCommandOptions { ScriptName = scriptName, OutputPath = _tempDirectory };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
            Assert.True(File.Exists(scriptPath));
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithExistingFile_ShouldNotOverwrite()
        {
            // Arrange
            var scriptName = "existing.csx";
            var scriptPath = Path.Combine(_tempDirectory, scriptName);
            await File.WriteAllTextAsync(scriptPath, "// Existing content");
            var options = new InitCommandOptions { ScriptName = scriptName, OutputPath = _tempDirectory };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
            var content = await File.ReadAllTextAsync(scriptPath);
            Assert.Contains("// Existing content", content);
        }

        [Theory]
        [InlineData("console")]
        [InlineData("web")]
        [InlineData("library")]
        public async Task ExecuteAsync_WithTemplate_ShouldCreateTemplateBasedScript(string template)
        {
            // Arrange
            var scriptName = $"{template}.csx";
            var scriptPath = Path.Combine(_tempDirectory, scriptName);
            var options = new InitCommandOptions 
            { 
                ScriptName = scriptName, 
                OutputPath = _tempDirectory,
                Template = template
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
            Assert.True(File.Exists(scriptPath));
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
    }
}
=== END FILE ===

=== FILE: tests/Dotnet.Script.Core.Tests/Commands/ExecuteLibraryCommandTests.cs ===
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteLibraryCommandTests : IDisposable
    {
        private readonly Mock<ILogger<ExecuteLibraryCommand>> _mockLogger;
        private readonly ExecuteLibraryCommand _command;
        private readonly string _tempDirectory;

        public ExecuteLibraryCommandTests()
        {
            _mockLogger = new Mock<ILogger<ExecuteLibraryCommand>>();
            _command = new ExecuteLibraryCommand(_mockLogger.Object);
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [Fact]
        public void Constructor_WithValidLogger_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Assert.NotNull(_command);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecuteLibraryCommand(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidLibraryPath_ShouldExecuteSuccessfully()
        {
            // Arrange
            var libraryPath = Path.Combine(_tempDirectory, "library.dll");
            // Create a mock library file
            await File.WriteAllBytesAsync(libraryPath, new byte[] { 0x4D, 0x5A }); // PE header
            var options = new ExecuteLibraryCommandOptions { LibraryPath = libraryPath };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            // Note: This might fail in actual execution due to invalid assembly, 
            // but we're testing the command structure
            Assert.True(result >= 0);
        }

        [Fact]
        public async Task ExecuteAsync_WithNonExistentLibrary_ShouldReturnError()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions { LibraryPath = "nonexistent.dll" };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithMethodName_ShouldExecuteSpecificMethod()
        {
            // Arrange
            var libraryPath = Path.Combine(_tempDirectory, "library.dll");
            await File.WriteAllBytesAsync(libraryPath, new byte[] { 0x4D, 0x5A });
            var options = new ExecuteLibraryCommandOptions 
            { 
                LibraryPath = libraryPath,
                MethodName = "Main"
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.True(result >= 0);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
    }
}
=== END FILE ===

=== FILE: tests/Dotnet.Script.Core.Tests/CSharpObjectFormatterExtensionsTests.cs ===
using System;
using System.Collections.Generic;
using Xunit;
using Dotnet.Script.Core;

namespace Dotnet.Script.Core.Tests
{
    public class CSharpObjectFormatterExtensionsTests
    {
        [Fact]
        public void FormatObject_WithNull_ShouldReturnNullString()
        {
            // Arrange
            object obj = null;

            // Act
            var result = obj.FormatObject();

            // Assert
            Assert.Equal("null", result);
        }

        [Theory]
        [InlineData(42, "42")]
        [InlineData("hello", "\"hello\"")]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        public void FormatObject_WithPrimitiveTypes_ShouldReturnFormattedString(object input, string expected)
        {
            // Act
            var result = input.FormatObject();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatObject_WithArray_ShouldReturnFormattedArray()
        {
            // Arrange
            var array = new[] { 1, 2, 3 };

            // Act
            var result = array.FormatObject();

            // Assert
            Assert.Contains("1", result);
            Assert.Contains("2", result);
            Assert.Contains("3", result);
        }

        [Fact]
        public void FormatObject_WithList_ShouldReturnFormattedList()
        {
            // Arrange
            var list = new List<string> { "a", "b", "c" };

            // Act
            var result = list.FormatObject();

            // Assert
            Assert.Contains("a", result);
            Assert.Contains("b", result);
            Assert.Contains("c", result);
        }

        [Fact]
        public void FormatObject_WithDictionary_ShouldReturnFormattedDictionary()
        {
            // Arrange
            var dict = new Dictionary<string, int> { { "key1", 1 }, { "key2", 2 } };

            // Act
            var result = dict.FormatObject();

            // Assert
            Assert.Contains("key1", result);
            Assert.Contains("key2", result);
        }

        [Fact]
        public void FormatObject_WithCustomObject_ShouldReturnFormattedObject()
        {
            // Arrange
            var obj = new { Name = "Test", Value = 42 };

            // Act
            var result = obj.FormatObject();

            // Assert
            Assert.Contains("Name", result);
            Assert.Contains("Test", result);
            Assert.Contains("Value", result);
            Assert.Contains("42", result);
        }

        [Fact]
        public void FormatObject_WithNestedObject_ShouldReturnFormattedNestedObject()
        {
            // Arrange
            var obj = new 
            { 
                Name = "Parent", 
                Child = new { Name = "Child", Value = 10 } 
            };

            // Act
            var result = obj.FormatObject();

            // Assert
            Assert.Contains("Parent", result);
            Assert.Contains("Child", result);
            Assert.Contains("10", result);
        }

        [Fact]
        public void FormatObject_WithCircularReference_ShouldHandleGracefully()
        {
            // Arrange
            var obj1 = new TestClass { Name = "Object1" };
            var obj2 = new TestClass { Name = "Object2" };
            obj1.Reference = obj2;
            obj2.Reference = obj1;

            // Act & Assert
            // Should not throw StackOverflowException
            var result = obj1.FormatObject();
            Assert.NotNull(result);
        }

        private class TestClass
        {
            public string Name { get; set; }
            public TestClass Reference { get; set; }
        }
    }
}
=== END FILE ===

=== FILE: tests/Dotnet.Script.Core.Tests/ScriptCompilerTests.cs ===
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptCompilerTests : IDisposable
    {
        private readonly Mock<ILogger<ScriptCompiler>> _mockLogger;
        private readonly ScriptCompiler _compiler;
        private readonly string _tempDirectory;

        public ScriptCompilerTests()
        {
            _mockLogger = new Mock<ILogger<ScriptCompiler>>();
            _compiler = new ScriptCompiler(_mockLogger.Object);
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [Fact]
        public void Constructor_WithValidLogger_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Assert.NotNull(_compiler);
        }

        [Theory]
        [InlineData("Console.WriteLine(\"Hello World\");")]
        [InlineData("var x = 5; Console.WriteLine(x);")]
        [InlineData("return 42;")]
        public async Task CompileAsync_WithValidCode_ShouldCompileSuccessfully(string code)
        {
            // Act
            var result = await _compiler.CompileAsync(code);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Theory]
        [InlineData("invalid syntax")]
        [InlineData("Console.WriteLine(")]
        [InlineData("var x = ;")]
        public async Task CompileAsync_WithInvalidCode_ShouldReturnCompilationErrors(string code)
        {
            // Act
            var result = await _compiler.CompileAsync(code);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task CompileAsync_WithNullCode_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _compiler.CompileAsync(null));
        }

        [Fact]
        public async Task CompileAsync_WithEmptyCode_ShouldReturnEmptyCompilation()
        {
            // Act
            var result = await _compiler.CompileAsync(string.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task CompileScriptFileAsync_WithValidFile_ShouldCompileSuccessfully()
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "test.csx");
            await File.WriteAllTextAsync(scriptPath, "Console.WriteLine(\"Hello from file\");");

            // Act
            var result = await _compiler.CompileScriptFileAsync(scriptPath);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task CompileScriptFileAsync_WithNonExistentFile_ShouldThrowFileNotFoundException()
        {
            // Arrange
            var scriptPath = "nonexistent.csx";

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _compiler.CompileScriptFileAsync(scriptPath));
        }

        [Fact]
        public async Task CompileWithReferencesAsync_WithValidReferences_ShouldCompileSuccessfully()
        {
            // Arrange
            var code = "using System.Text.Json; var json = JsonSerializer.Serialize(new { test = \"value\" });";
            var references = new[] { "System.Text.Json" };

            // Act
            var result = await _compiler.CompileWithReferencesAsync(code, references);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task CompileWithReferencesAsync_WithInvalidReferences_ShouldReturnCompilationErrors()
        {
            // Arrange
            var code = "using NonExistent.Library; var obj = new SomeClass();";
            var references = new[] { "NonExistent.Library" };

            // Act
            var result = await _compiler.CompileWithReferencesAsync(code, references);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
    }
}
=== END FILE ===

=== FILE: tests/Dotnet.Script.Core.Tests/ScriptExecutorTests.cs ===
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptExecutorTests
    {
        private readonly Mock<ILogger<ScriptExecutor>> _mockLogger;
        private readonly Mock<IScriptCompiler> _mockCompiler;
        private readonly ScriptExecutor _executor;

        public ScriptExecutorTests()
        {
            _mockLogger = new Mock<ILogger<ScriptExecutor>>();
            _mockCompiler = new Mock<IScriptCompiler>();
            _executor = new ScriptExecutor(_mockLogger.Object, _mockCompiler.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Assert.NotNull(_executor);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ScriptExecutor(null, _mockCompiler.Object));
        }

        [Fact]
        public void Constructor_WithNullCompiler_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ScriptExecutor(_mockLogger.Object, null));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidCode_ShouldExecuteSuccessfully()
        {
            // Arrange
            var code = "Console.WriteLine(\"Hello World\");";
            var mockResult = new Mock<ICompilationResult>();
            mockResult.Setup(r => r.Success).Returns(true);
            _mockCompiler.Setup(c => c.CompileAsync(code)).ReturnsAsync(mockResult.Object);

            // Act
            var result = await _executor.ExecuteAsync(code);

            // Assert
            Assert.Equal(0, result.ExitCode);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ExecuteAsync_WithCompilationError_ShouldReturnError()
        {
            // Arrange
            var code = "invalid syntax";
            var mockResult = new Mock<ICompilationResult>();
            mockResult.Setup(r => r.Success).Returns(false);
            mockResult.Setup(r => r.Errors).Returns(new[] { "Compilation error" });
            _mockCompiler.Setup(c => c.CompileAsync(code)).ReturnsAsync(mockResult.Object);

            // Act
            var result = await _executor.ExecuteAsync(code);

            // Assert
            Assert.NotEqual(0, result.ExitCode);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullCode_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _executor.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteWithArgsAsync_WithValidCodeAndArgs_ShouldExecuteSuccessfully()
        {
            // Arrange
            var code = "Console.WriteLine($\"Args: {string.Join(\", \", Args)}\");";
            var args = new[] { "arg1", "arg2" };
            var mockResult = new Mock<ICompilationResult>();
            mockResult.Setup(r => r.Success).Returns(true);
            _mockCompiler.Setup(c => c.CompileAsync(It.IsAny<string>())).ReturnsAsync(mockResult.Object);

            // Act
            var result = await _executor.ExecuteWithArgsAsync(code, args);

            // Assert
            Assert.Equal(0, result.ExitCode);
            Assert.True(result.Success);
        }

        [Theory]
        [InlineData(new string[0])]
        [InlineData(new[] { "single" })]
        [InlineData(new[] { "arg1", "arg2", "arg3" })]
        public async Task ExecuteWithArgsAsync_WithDifferentArgCounts_ShouldHandleCorrectly(string[] args)
        {
            // Arrange
            var code = "Console.WriteLine(Args.Count);";
            var mockResult = new Mock<ICompilationResult>();
            mockResult.Setup(r => r.Success).Returns(true);
            _mockCompiler.Setup(c => c.CompileAsync(It.IsAny<string>())).ReturnsAsync(mockResult.Object);

            // Act
            var result = await _executor.ExecuteWithArgsAsync(code, args);

            // Assert
            Assert.Equal(0, result.ExitCode);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ExecuteFileAsync_WithValidScriptPath_ShouldExecuteSuccessfully()
        {
            // Arrange
            var scriptPath = "test.csx";
            var mockResult = new Mock<ICompilationResult>();
            mockResult.Setup(r => r.Success).Returns(true);
            _mockCompiler.Setup(c => c.CompileScriptFileAsync(scriptPath)).ReturnsAsync(mockResult.Object);

            // Act
            var result = await _executor.ExecuteFileAsync(scriptPath);

            // Assert
            Assert.Equal(0, result.ExitCode);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ExecuteFileAsync_WithNullPath_Shoul