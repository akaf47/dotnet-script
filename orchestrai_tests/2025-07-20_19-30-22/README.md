# OrchestrAI Test Results for dotnet-script

Generated on: 2025-07-20T19:30:25.164Z

## Test Strategy

I'll generate comprehensive unit tests for all the C# files in this dotnet-script repository. Based on the file structure, this appears to be a .NET script execution tool with multiple components. I'll create xUnit test files covering all the major classes and functionality.

=== FILE: csharp/tests/CSharpObjectFormatterExtensionsTests.cs ===
using Xunit;
using Dotnet.Script.Core;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using System;

namespace Dotnet.Script.Core.Tests
{
    public class CSharpObjectFormatterExtensionsTests
    {
        [Fact]
        public void FormatObject_WithNull_ReturnsNullString()
        {
            // Arrange
            object obj = null;

            // Act
            var result = obj.FormatObject();

            // Assert
            Assert.Equal("null", result);
        }

        [Fact]
        public void FormatObject_WithString_ReturnsFormattedString()
        {
            // Arrange
            var obj = "test string";

            // Act
            var result = obj.FormatObject();

            // Assert
            Assert.Contains("test string", result);
        }

        [Fact]
        public void FormatObject_WithInteger_ReturnsFormattedInteger()
        {
            // Arrange
            var obj = 42;

            // Act
            var result = obj.FormatObject();

            // Assert
            Assert.Equal("42", result);
        }

        [Fact]
        public void FormatObject_WithComplexObject_ReturnsFormattedObject()
        {
            // Arrange
            var obj = new { Name = "Test", Value = 123 };

            // Act
            var result = obj.FormatObject();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/Commands/ExecuteCodeCommandTests.cs ===
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Dotnet.Script.DependencyModel.Logging;
using System.Threading.Tasks;

namespace Dotnet.Script.Core.Commands.Tests
{
    public class ExecuteCodeCommandTests
    {
        private readonly Mock<IScriptRunner> _mockScriptRunner;
        private readonly Mock<Logger> _mockLogger;
        private readonly ExecuteCodeCommand _command;

        public ExecuteCodeCommandTests()
        {
            _mockScriptRunner = new Mock<IScriptRunner>();
            _mockLogger = new Mock<Logger>();
            _command = new ExecuteCodeCommand(_mockScriptRunner.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Execute_WithValidCode_ReturnsSuccess()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                Configuration = "Release"
            };

            _mockScriptRunner.Setup(x => x.Execute(It.IsAny<ScriptContext>()))
                           .Returns(Task.FromResult(0));

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            _mockScriptRunner.Verify(x => x.Execute(It.IsAny<ScriptContext>()), Times.Once);
        }

        [Fact]
        public async Task Execute_WithInvalidCode_ReturnsError()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "invalid code syntax",
                Configuration = "Release"
            };

            _mockScriptRunner.Setup(x => x.Execute(It.IsAny<ScriptContext>()))
                           .Returns(Task.FromResult(1));

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Execute_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/Commands/ExecuteScriptCommandTests.cs ===
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Dotnet.Script.DependencyModel.Logging;
using System.Threading.Tasks;
using System.IO;

namespace Dotnet.Script.Core.Commands.Tests
{
    public class ExecuteScriptCommandTests
    {
        private readonly Mock<IScriptRunner> _mockScriptRunner;
        private readonly Mock<Logger> _mockLogger;
        private readonly ExecuteScriptCommand _command;

        public ExecuteScriptCommandTests()
        {
            _mockScriptRunner = new Mock<IScriptRunner>();
            _mockLogger = new Mock<Logger>();
            _command = new ExecuteScriptCommand(_mockScriptRunner.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Execute_WithValidScriptFile_ReturnsSuccess()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "Console.WriteLine(\"Hello World\");");

            var options = new ExecuteScriptCommandOptions
            {
                File = tempFile,
                Configuration = "Release"
            };

            _mockScriptRunner.Setup(x => x.Execute(It.IsAny<ScriptContext>()))
                           .Returns(Task.FromResult(0));

            try
            {
                // Act
                var result = await _command.Execute(options);

                // Assert
                Assert.Equal(0, result);
                _mockScriptRunner.Verify(x => x.Execute(It.IsAny<ScriptContext>()), Times.Once);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task Execute_WithNonExistentFile_ReturnsError()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions
            {
                File = "nonexistent.csx",
                Configuration = "Release"
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Execute_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/Commands/InitCommandTests.cs ===
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using System.Threading.Tasks;
using System.IO;

namespace Dotnet.Script.Core.Commands.Tests
{
    public class InitCommandTests
    {
        private readonly Mock<IScaffolder> _mockScaffolder;
        private readonly InitCommand _command;

        public InitCommandTests()
        {
            _mockScaffolder = new Mock<IScaffolder>();
            _command = new InitCommand(_mockScaffolder.Object);
        }

        [Fact]
        public async Task Execute_WithValidOptions_CreatesScriptFile()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var options = new InitCommandOptions
            {
                FileName = "test.csx",
                WorkingDirectory = tempDir
            };

            _mockScaffolder.Setup(x => x.InitializeScript(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            _mockScaffolder.Verify(x => x.InitializeScript(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Execute_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/ScriptCompilerTests.cs ===
using Xunit;
using Moq;
using Dotnet.Script.Core;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptCompilerTests
    {
        private readonly Mock<ICompilationDependencyResolver> _mockDependencyResolver;
        private readonly Mock<Logger> _mockLogger;
        private readonly ScriptCompiler _compiler;

        public ScriptCompilerTests()
        {
            _mockDependencyResolver = new Mock<ICompilationDependencyResolver>();
            _mockLogger = new Mock<Logger>();
            _compiler = new ScriptCompiler(_mockDependencyResolver.Object, _mockLogger.Object);
        }

        [Fact]
        public void CreateCompilationContext_WithValidScript_ReturnsContext()
        {
            // Arrange
            var scriptContext = new ScriptContext
            {
                SourceText = "Console.WriteLine(\"Hello World\");",
                WorkingDirectory = "/tmp",
                Args = new string[0]
            };

            _mockDependencyResolver.Setup(x => x.GetDependencies(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
                                  .Returns(new List<CompilationDependency>());

            // Act
            var result = _compiler.CreateCompilationContext(scriptContext);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Compilation);
        }

        [Fact]
        public void CreateCompilationContext_WithReferences_IncludesReferences()
        {
            // Arrange
            var scriptContext = new ScriptContext
            {
                SourceText = "#r \"System.IO\"\nConsole.WriteLine(\"Hello World\");",
                WorkingDirectory = "/tmp",
                Args = new string[0]
            };

            var dependencies = new List<CompilationDependency>
            {
                new CompilationDependency { Name = "System.IO", AssemblyPaths = new[] { "System.IO.dll" } }
            };

            _mockDependencyResolver.Setup(x => x.GetDependencies(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
                                  .Returns(dependencies);

            // Act
            var result = _compiler.CreateCompilationContext(scriptContext);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Compilation.References.Any());
        }

        [Fact]
        public void CreateCompilationContext_WithSyntaxError_ReturnsContextWithDiagnostics()
        {
            // Arrange
            var scriptContext = new ScriptContext
            {
                SourceText = "Console.WriteLine(\"Hello World\"", // Missing closing parenthesis
                WorkingDirectory = "/tmp",
                Args = new string[0]
            };

            _mockDependencyResolver.Setup(x => x.GetDependencies(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
                                  .Returns(new List<CompilationDependency>());

            // Act
            var result = _compiler.CreateCompilationContext(scriptContext);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Compilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error));
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/ScriptRunnerTests.cs ===
using Xunit;
using Moq;
using Dotnet.Script.Core;
using System.Threading.Tasks;
using System.IO;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptRunnerTests
    {
        private readonly Mock<IScriptCompiler> _mockCompiler;
        private readonly Mock<IScriptEmitter> _mockEmitter;
        private readonly Mock<Logger> _mockLogger;
        private readonly ScriptRunner _runner;

        public ScriptRunnerTests()
        {
            _mockCompiler = new Mock<IScriptCompiler>();
            _mockEmitter = new Mock<IScriptEmitter>();
            _mockLogger = new Mock<Logger>();
            _runner = new ScriptRunner(_mockCompiler.Object, _mockEmitter.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Execute_WithValidScript_ReturnsSuccess()
        {
            // Arrange
            var scriptContext = new ScriptContext
            {
                SourceText = "Console.WriteLine(\"Hello World\");",
                WorkingDirectory = "/tmp",
                Args = new string[0]
            };

            var compilationContext = new ScriptCompilationContext<object>();
            var emitResult = new ScriptEmitResult
            {
                Success = true,
                AssemblyPath = "/tmp/script.dll"
            };

            _mockCompiler.Setup(x => x.CreateCompilationContext<object>(It.IsAny<ScriptContext>()))
                        .Returns(compilationContext);

            _mockEmitter.Setup(x => x.Emit<object>(It.IsAny<ScriptCompilationContext<object>>(), It.IsAny<string>()))
                       .Returns(emitResult);

            // Act
            var result = await _runner.Execute<object>(scriptContext);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_WithCompilationError_ReturnsError()
        {
            // Arrange
            var scriptContext = new ScriptContext
            {
                SourceText = "invalid syntax",
                WorkingDirectory = "/tmp",
                Args = new string[0]
            };

            var compilationContext = new ScriptCompilationContext<object>();
            var emitResult = new ScriptEmitResult
            {
                Success = false,
                Diagnostics = new[] { "Compilation error" }
            };

            _mockCompiler.Setup(x => x.CreateCompilationContext<object>(It.IsAny<ScriptContext>()))
                        .Returns(compilationContext);

            _mockEmitter.Setup(x => x.Emit<object>(It.IsAny<ScriptCompilationContext<object>>(), It.IsAny<string>()))
                       .Returns(emitResult);

            // Act
            var result = await _runner.Execute<object>(scriptContext);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Execute_WithNullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _runner.Execute<object>(null));
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/Interactive/InteractiveRunnerTests.cs ===
using Xunit;
using Moq;
using Dotnet.Script.Core.Interactive;
using System.Threading.Tasks;
using System.IO;

namespace Dotnet.Script.Core.Interactive.Tests
{
    public class InteractiveRunnerTests
    {
        private readonly Mock<IScriptRunner> _mockScriptRunner;
        private readonly Mock<IInteractiveCommandProvider> _mockCommandProvider;
        private readonly Mock<TextReader> _