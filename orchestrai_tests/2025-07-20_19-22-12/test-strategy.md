I'll analyze the C# repository and generate comprehensive unit tests for the dotnet-script project. Based on the file list, this is a C# project that provides scripting capabilities for .NET. I'll create unit tests using xUnit to achieve 100% coverage.

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
            var result = CSharpObjectFormatterExtensions.FormatObject(obj);

            // Assert
            Assert.Equal("null", result);
        }

        [Fact]
        public void FormatObject_WithString_ReturnsFormattedString()
        {
            // Arrange
            var obj = "test string";

            // Act
            var result = CSharpObjectFormatterExtensions.FormatObject(obj);

            // Assert
            Assert.Contains("test string", result);
        }

        [Fact]
        public void FormatObject_WithInteger_ReturnsFormattedInteger()
        {
            // Arrange
            var obj = 42;

            // Act
            var result = CSharpObjectFormatterExtensions.FormatObject(obj);

            // Assert
            Assert.Equal("42", result);
        }

        [Fact]
        public void FormatObject_WithComplexObject_ReturnsFormattedObject()
        {
            // Arrange
            var obj = new { Name = "Test", Value = 123 };

            // Act
            var result = CSharpObjectFormatterExtensions.FormatObject(obj);

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
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteCodeCommandTests
    {
        private readonly Mock<ILogger<ExecuteCodeCommand>> _loggerMock;
        private readonly ExecuteCodeCommand _command;

        public ExecuteCodeCommandTests()
        {
            _loggerMock = new Mock<ILogger<ExecuteCodeCommand>>();
            _command = new ExecuteCodeCommand(_loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidCode_ReturnsSuccess()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                Configuration = "Release"
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidCode_ReturnsError()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "invalid C# code syntax error",
                Configuration = "Release"
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithEmptyCode_ReturnsSuccess()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = string.Empty,
                Configuration = "Release"
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/Commands/ExecuteCodeCommandOptionsTests.cs ===
using Xunit;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteCodeCommandOptionsTests
    {
        [Fact]
        public void Constructor_SetsDefaultValues()
        {
            // Act
            var options = new ExecuteCodeCommandOptions();

            // Assert
            Assert.Null(options.Code);
            Assert.Equal("Release", options.Configuration);
            Assert.False(options.Debug);
            Assert.Empty(options.PackageSources);
        }

        [Fact]
        public void Code_CanBeSetAndRetrieved()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions();
            var code = "Console.WriteLine(\"Test\");";

            // Act
            options.Code = code;

            // Assert
            Assert.Equal(code, options.Code);
        }

        [Fact]
        public void Configuration_CanBeSetAndRetrieved()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions();
            var configuration = "Debug";

            // Act
            options.Configuration = configuration;

            // Assert
            Assert.Equal(configuration, options.Configuration);
        }

        [Fact]
        public void Debug_CanBeSetAndRetrieved()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions();

            // Act
            options.Debug = true;

            // Assert
            Assert.True(options.Debug);
        }

        [Fact]
        public void PackageSources_CanBeSetAndRetrieved()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions();
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            options.PackageSources = packageSources;

            // Assert
            Assert.Equal(packageSources, options.PackageSources);
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/Commands/ExecuteInteractiveCommandTests.cs ===
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteInteractiveCommandTests
    {
        private readonly Mock<ILogger<ExecuteInteractiveCommand>> _loggerMock;
        private readonly ExecuteInteractiveCommand _command;

        public ExecuteInteractiveCommandTests()
        {
            _loggerMock = new Mock<ILogger<ExecuteInteractiveCommand>>();
            _command = new ExecuteInteractiveCommand(_loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidOptions_ReturnsSuccess()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions
            {
                Configuration = "Release"
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithDebugMode_ReturnsSuccess()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions
            {
                Configuration = "Debug",
                Debug = true
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/Commands/ExecuteScriptCommandTests.cs ===
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteScriptCommandTests
    {
        private readonly Mock<ILogger<ExecuteScriptCommand>> _loggerMock;
        private readonly ExecuteScriptCommand _command;

        public ExecuteScriptCommandTests()
        {
            _loggerMock = new Mock<ILogger<ExecuteScriptCommand>>();
            _command = new ExecuteScriptCommand(_loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidScriptFile_ReturnsSuccess()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "Console.WriteLine(\"Hello World\");");
            
            var options = new ExecuteScriptCommandOptions
            {
                File = tempFile,
                Configuration = "Release"
            };

            try
            {
                // Act
                var result = await _command.ExecuteAsync(options);

                // Assert
                Assert.Equal(0, result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task ExecuteAsync_WithNonExistentFile_ReturnsError()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions
            {
                File = "nonexistent.csx",
                Configuration = "Release"
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithArguments_PassesArgumentsToScript()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "Console.WriteLine(Args[0]);");
            
            var options = new ExecuteScriptCommandOptions
            {
                File = tempFile,
                Configuration = "Release",
                Arguments = new[] { "test-arg" }
            };

            try
            {
                // Act
                var result = await _command.ExecuteAsync(options);

                // Assert
                Assert.Equal(0, result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/Commands/InitCommandTests.cs ===
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class InitCommandTests
    {
        private readonly Mock<ILogger<InitCommand>> _loggerMock;
        private readonly InitCommand _command;

        public InitCommandTests()
        {
            _loggerMock = new Mock<ILogger<InitCommand>>();
            _command = new InitCommand(_loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidPath_CreatesScriptFile()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var scriptName = "test-script.csx";
            var fullPath = Path.Combine(tempDir, scriptName);
            
            var options = new InitCommandOptions
            {
                FileName = scriptName,
                WorkingDirectory = tempDir
            };

            try
            {
                // Act
                var result = await _command.ExecuteAsync(options);

                // Assert
                Assert.Equal(0, result);
                Assert.True(File.Exists(fullPath));
            }
            finally
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithExistingFile_ReturnsError()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var scriptName = "existing-script.csx";
            var fullPath = Path.Combine(tempDir, scriptName);
            
            File.WriteAllText(fullPath, "// Existing file");
            
            var options = new InitCommandOptions
            {
                FileName = scriptName,
                WorkingDirectory = tempDir
            };

            try
            {
                // Act
                var result = await _command.ExecuteAsync(options);

                // Assert
                Assert.NotEqual(0, result);
            }
            finally
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }
    }
}
=== END FILE ===

=== FILE: csharp/tests/ScriptRunnerTests.cs ===
using Xunit;
using Moq;
using Dotnet.Script.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptRunnerTests
    {
        private readonly Mock<ILogger<ScriptRunner>> _loggerMock;
        private readonly ScriptRunner _scriptRunner;

        public ScriptRunnerTests()
        {
            _loggerMock = new Mock<ILogger<ScriptRunner>>();
            _scriptRunner = new ScriptRunner(_loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidCode_ReturnsResult()
        {
            // Arrange
            var code = "return 42;";

            // Act
            var result = await _scriptRunner.ExecuteAsync<int>(code);

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidCode_ThrowsException()
        {
            // Arrange
            var code = "invalid syntax";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _scriptRunner.ExecuteAsync<object>(code));
        }

        [Fact]
        public async Task ExecuteAsync_WithNullCode_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _scriptRunner.ExecuteAsync<object>(null));
        }

        [Fact]
        public async Task ExecuteFileAsync_WithValidFile_ReturnsResult()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "return \"Hello World\";");

            try
            {
                // Act
                var result = await _scriptRunner.ExecuteFileAsync<string>(tempFile);

                // Assert
                Assert.Equal("Hello World", result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public async