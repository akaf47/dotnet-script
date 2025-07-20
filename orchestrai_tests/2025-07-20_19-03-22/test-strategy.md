I'll analyze the C# repository and generate comprehensive unit tests for the dotnet-script project. Based on the file list, this is a .NET Core application for executing C# scripts, so I'll focus on creating xUnit tests for the core functionality.

=== FILE: csharp/tests/Commands/ExecuteCodeCommandTests.cs ===
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;
using Dotnet.Script.DependencyModel.Logging;
using Microsoft.CodeAnalysis.Scripting;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteCodeCommandTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IScriptConsole> _mockConsole;
        private readonly ExecuteCodeCommand _command;

        public ExecuteCodeCommandTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockConsole = new Mock<IScriptConsole>();
            _command = new ExecuteCodeCommand(_mockLogger.Object, _mockConsole.Object);
        }

        [Fact]
        public async Task Execute_WithValidCode_ShouldReturnSuccess()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                Configuration = "Release"
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_WithInvalidCode_ShouldReturnError()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "invalid code syntax",
                Configuration = "Release"
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Execute_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Execute_WithEmptyCode_ShouldReturnError(string code)
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = code,
                Configuration = "Release"
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Execute_WithDebugConfiguration_ShouldUseDebugMode()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "var x = 1;",
                Configuration = "Debug"
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/Commands/ExecuteScriptCommandTests.cs ===
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteScriptCommandTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IScriptConsole> _mockConsole;
        private readonly ExecuteScriptCommand _command;
        private readonly string _tempDirectory;

        public ExecuteScriptCommandTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockConsole = new Mock<IScriptConsole>();
            _command = new ExecuteScriptCommand(_mockLogger.Object, _mockConsole.Object);
            _tempDirectory = Path.GetTempPath();
        }

        [Fact]
        public async Task Execute_WithValidScriptFile_ShouldReturnSuccess()
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "test.csx");
            File.WriteAllText(scriptPath, "Console.WriteLine(\"Hello from script\");");
            
            var options = new ExecuteScriptCommandOptions
            {
                File = scriptPath,
                Configuration = "Release"
            };

            try
            {
                // Act
                var result = await _command.Execute(options);

                // Assert
                Assert.Equal(0, result);
            }
            finally
            {
                if (File.Exists(scriptPath))
                    File.Delete(scriptPath);
            }
        }

        [Fact]
        public async Task Execute_WithNonExistentFile_ShouldReturnError()
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
        public async Task Execute_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }

        [Fact]
        public async Task Execute_WithArguments_ShouldPassArgumentsToScript()
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "test_args.csx");
            File.WriteAllText(scriptPath, "Console.WriteLine(Args[0]);");
            
            var options = new ExecuteScriptCommandOptions
            {
                File = scriptPath,
                Configuration = "Release",
                Arguments = new[] { "test-arg" }
            };

            try
            {
                // Act
                var result = await _command.Execute(options);

                // Assert
                Assert.Equal(0, result);
            }
            finally
            {
                if (File.Exists(scriptPath))
                    File.Delete(scriptPath);
            }
        }

        [Theory]
        [InlineData("Debug")]
        [InlineData("Release")]
        public async Task Execute_WithDifferentConfigurations_ShouldWork(string configuration)
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, $"test_{configuration}.csx");
            File.WriteAllText(scriptPath, "var x = 1;");
            
            var options = new ExecuteScriptCommandOptions
            {
                File = scriptPath,
                Configuration = configuration
            };

            try
            {
                // Act
                var result = await _command.Execute(options);

                // Assert
                Assert.Equal(0, result);
            }
            finally
            {
                if (File.Exists(scriptPath))
                    File.Delete(scriptPath);
            }
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/Commands/ExecuteInteractiveCommandTests.cs ===
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
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IScriptConsole> _mockConsole;
        private readonly ExecuteInteractiveCommand _command;

        public ExecuteInteractiveCommandTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockConsole = new Mock<IScriptConsole>();
            _command = new ExecuteInteractiveCommand(_mockLogger.Object, _mockConsole.Object);
        }

        [Fact]
        public async Task Execute_WithValidOptions_ShouldReturnSuccess()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions
            {
                Configuration = "Release"
            };

            // Mock console input to exit immediately
            _mockConsole.Setup(c => c.ReadLine()).Returns("#exit");

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }

        [Fact]
        public async Task Execute_WithDebugConfiguration_ShouldUseDebugMode()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions
            {
                Configuration = "Debug"
            };

            _mockConsole.Setup(c => c.ReadLine()).Returns("#exit");

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecuteInteractiveCommand(null, _mockConsole.Object));
        }

        [Fact]
        public void Constructor_WithNullConsole_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecuteInteractiveCommand(_mockLogger.Object, null));
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/Commands/InitCommandTests.cs ===
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class InitCommandTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly InitCommand _command;
        private readonly string _tempDirectory;

        public InitCommandTests()
        {
            _mockLogger = new Mock<ILogger>();
            _command = new InitCommand(_mockLogger.Object);
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [Fact]
        public async Task Execute_WithValidDirectory_ShouldCreateScriptFile()
        {
            // Arrange
            var options = new InitCommandOptions
            {
                Name = "test-script",
                Directory = _tempDirectory
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            Assert.True(File.Exists(Path.Combine(_tempDirectory, "test-script.csx")));
        }

        [Fact]
        public async Task Execute_WithoutName_ShouldCreateMainScript()
        {
            // Arrange
            var options = new InitCommandOptions
            {
                Directory = _tempDirectory
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            Assert.True(File.Exists(Path.Combine(_tempDirectory, "main.csx")));
        }

        [Fact]
        public async Task Execute_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }

        [Fact]
        public async Task Execute_WithExistingFile_ShouldNotOverwrite()
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "existing.csx");
            var originalContent = "// Original content";
            File.WriteAllText(scriptPath, originalContent);

            var options = new InitCommandOptions
            {
                Name = "existing",
                Directory = _tempDirectory
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            var content = File.ReadAllText(scriptPath);
            Assert.Equal(originalContent, content);
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

=== FILE: csharp/tests/ScriptRunnerTests.cs ===
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptRunnerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IScriptConsole> _mockConsole;
        private readonly ScriptRunner _scriptRunner;

        public ScriptRunnerTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockConsole = new Mock<IScriptConsole>();
            _scriptRunner = new ScriptRunner(_mockLogger.Object, _mockConsole.Object);
        }

        [Fact]
        public async Task Execute_WithSimpleScript_ShouldReturnResult()
        {
            // Arrange
            var code = "return 42;";
            var context = new ScriptContext(code, Path.GetTempPath());

            // Act
            var result = await _scriptRunner.Execute<int>(context);

            // Assert
            Assert.Equal(42, result.ReturnValue);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_WithInvalidScript_ShouldReturnError()
        {
            // Arrange
            var code = "invalid syntax here";
            var context = new ScriptContext(code, Path.GetTempPath());

            // Act
            var result = await _scriptRunner.Execute<object>(context);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Exception);
        }

        [Fact]
        public async Task Execute_WithNullContext_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _scriptRunner.Execute<object>(null));
        }

        [Fact]
        public async Task Execute_WithUsingStatements_ShouldWork()
        {
            // Arrange
            var code = @"
                using System;
                return DateTime.Now.Year;
            ";
            var context = new ScriptContext(code, Path.GetTempPath());

            // Act
            var result = await _scriptRunner.Execute<int>(context);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.ReturnValue >= 2020);
        }

        [Fact]
        public async Task Execute_WithNuGetPackages_ShouldLoadPackages()
        {
            // Arrange
            var code = @"
                #r ""nuget: Newtonsoft.Json, 13.0.1""
                using Newtonsoft.Json;
                var obj = new { Name = ""Test"" };
                return JsonConvert.SerializeObject(obj);
            ";
            var context = new ScriptContext(code, Path.GetTempPath());

            // Act
            var result = await _scriptRunner.Execute<string>(context);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("Test", result.ReturnValue);
        }

        [Theory]
        [InlineData("Debug")]
        [InlineData("Release")]
        public async Task Execute_WithDiffer