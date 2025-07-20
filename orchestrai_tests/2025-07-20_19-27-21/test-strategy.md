I'll analyze the C# repository and generate comprehensive unit tests for the dotnet-script project. Based on the file list, this is a C# project that provides scripting capabilities for .NET. I'll create comprehensive xUnit tests for the core components.

=== FILE: csharp/tests/CSharpObjectFormatterExtensionsTests.cs ===
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Dotnet.Script.Core;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;

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
        public void FormatObject_WithString_ReturnsQuotedString()
        {
            // Arrange
            var obj = "test string";

            // Act
            var result = CSharpObjectFormatterExtensions.FormatObject(obj);

            // Assert
            Assert.Contains("\"test string\"", result);
        }

        [Fact]
        public void FormatObject_WithInteger_ReturnsIntegerString()
        {
            // Arrange
            var obj = 42;

            // Act
            var result = CSharpObjectFormatterExtensions.FormatObject(obj);

            // Assert
            Assert.Equal("42", result);
        }

        [Fact]
        public void FormatObject_WithCollection_ReturnsFormattedCollection()
        {
            // Arrange
            var obj = new List<int> { 1, 2, 3 };

            // Act
            var result = CSharpObjectFormatterExtensions.FormatObject(obj);

            // Assert
            Assert.Contains("1", result);
            Assert.Contains("2", result);
            Assert.Contains("3", result);
        }

        [Fact]
        public void FormatObject_WithComplexObject_ReturnsFormattedObject()
        {
            // Arrange
            var obj = new { Name = "Test", Value = 123 };

            // Act
            var result = CSharpObjectFormatterExtensions.FormatObject(obj);

            // Assert
            Assert.Contains("Name", result);
            Assert.Contains("Test", result);
            Assert.Contains("Value", result);
            Assert.Contains("123", result);
        }

        [Theory]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        public void FormatObject_WithBoolean_ReturnsLowercaseBoolean(bool input, string expected)
        {
            // Act
            var result = CSharpObjectFormatterExtensions.FormatObject(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatObject_WithException_ReturnsExceptionDetails()
        {
            // Arrange
            var exception = new InvalidOperationException("Test exception");

            // Act
            var result = CSharpObjectFormatterExtensions.FormatObject(exception);

            // Assert
            Assert.Contains("InvalidOperationException", result);
            Assert.Contains("Test exception", result);
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/Commands/ExecuteCodeCommandTests.cs ===
```csharp
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteCodeCommandTests
    {
        private readonly Mock<ILogger<ExecuteCodeCommand>> _mockLogger;
        private readonly Mock<IScriptExecutor> _mockScriptExecutor;
        private readonly ExecuteCodeCommand _command;

        public ExecuteCodeCommandTests()
        {
            _mockLogger = new Mock<ILogger<ExecuteCodeCommand>>();
            _mockScriptExecutor = new Mock<IScriptExecutor>();
            _command = new ExecuteCodeCommand(_mockScriptExecutor.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidCode_ExecutesSuccessfully()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            _mockScriptExecutor
                .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<string>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
            _mockScriptExecutor.Verify(x => x.ExecuteAsync(
                options.Code, 
                It.IsAny<string[]>(), 
                options.WorkingDirectory), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithEmptyCode_ThrowsArgumentException()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = string.Empty,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _command.ExecuteAsync(options));
        }

        [Fact]
        public async Task ExecuteAsync_WithScriptExecutionFailure_ReturnsErrorCode()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "throw new Exception(\"Test error\");",
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            _mockScriptExecutor
                .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithReferences_PassesReferencesToExecutor()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                WorkingDirectory = Directory.GetCurrentDirectory(),
                References = new[] { "System.dll", "System.Core.dll" }
            };

            _mockScriptExecutor
                .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<string>()))
                .ReturnsAsync(0);

            // Act
            await _command.ExecuteAsync(options);

            // Assert
            _mockScriptExecutor.Verify(x => x.ExecuteAsync(
                options.Code,
                options.References,
                options.WorkingDirectory), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_LogsExecutionStart()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            _mockScriptExecutor
                .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<string>()))
                .ReturnsAsync(0);

            // Act
            await _command.ExecuteAsync(options);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Executing code")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/Commands/ExecuteCodeCommandOptionsTests.cs ===
```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteCodeCommandOptionsTests
    {
        [Fact]
        public void Code_WhenSet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions();
            var expectedCode = "Console.WriteLine(\"Hello World\");";

            // Act
            options.Code = expectedCode;

            // Assert
            Assert.Equal(expectedCode, options.Code);
        }

        [Fact]
        public void WorkingDirectory_WhenSet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions();
            var expectedDirectory = "/path/to/directory";

            // Act
            options.WorkingDirectory = expectedDirectory;

            // Assert
            Assert.Equal(expectedDirectory, options.WorkingDirectory);
        }

        [Fact]
        public void References_WhenSet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions();
            var expectedReferences = new[] { "System.dll", "System.Core.dll" };

            // Act
            options.References = expectedReferences;

            // Assert
            Assert.Equal(expectedReferences, options.References);
        }

        [Fact]
        public void NuGetPackages_WhenSet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions();
            var expectedPackages = new[] { "Newtonsoft.Json", "Microsoft.Extensions.Logging" };

            // Act
            options.NuGetPackages = expectedPackages;

            // Assert
            Assert.Equal(expectedPackages, options.NuGetPackages);
        }

        [Fact]
        public void Validate_WithValidOptions_ReturnsNoErrors()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                WorkingDirectory = "/valid/path"
            };

            var context = new ValidationContext(options);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(options, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Validate_WithEmptyCode_ReturnsValidationError()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = string.Empty,
                WorkingDirectory = "/valid/path"
            };

            var context = new ValidationContext(options);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(options, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(ExecuteCodeCommandOptions.Code)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Code_WithInvalidValue_FailsValidation(string invalidCode)
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = invalidCode,
                WorkingDirectory = "/valid/path"
            };

            var context = new ValidationContext(options, null, null) { MemberName = nameof(ExecuteCodeCommandOptions.Code) };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(invalidCode, context, results);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ToString_ReturnsFormattedString()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                WorkingDirectory = "/test/path",
                References = new[] { "System.dll" }
            };

            // Act
            var result = options.ToString();

            // Assert
            Assert.Contains("Code", result);
            Assert.Contains("WorkingDirectory", result);
            Assert.Contains("/test/path", result);
        }
    }
}
```
=== END FILE ===

=== FILE: csharp/tests/Commands/ExecuteInteractiveCommandTests.cs ===
```csharp
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteInteractiveCommandTests
    {
        private readonly Mock<ILogger<ExecuteInteractiveCommand>> _mockLogger;
        private readonly Mock<IInteractiveRunner> _mockInteractiveRunner;
        private readonly ExecuteInteractiveCommand _command;

        public ExecuteInteractiveCommandTests()
        {
            _mockLogger = new Mock<ILogger<ExecuteInteractiveCommand>>();
            _mockInteractiveRunner = new Mock<IInteractiveRunner>();
            _command = new ExecuteInteractiveCommand(_mockInteractiveRunner.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidOptions_StartsInteractiveSession()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions
            {
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            _mockInteractiveRunner
                .Setup(x => x.RunInteractiveAsync(It.IsAny<string>(), It.IsAny<string[]>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
            _mockInteractiveRunner.Verify(x => x.RunInteractiveAsync(
                options.WorkingDirectory,
                It.IsAny<string[]>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithReferences_PassesReferencesToRunner()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions
            {
                WorkingDirectory = Directory.GetCurrentDirectory(),
                References = new[] { "System.dll", "System.Core.dll" }
            };

            _mockInteractiveRunner
                .Setup(x => x.RunInteractiveAsync(It.IsAny<string>(), It.IsAny<string[]>()))
                .ReturnsAsync(0);

            // Act
            await _command.ExecuteAsync(options);

            // Assert
            _mockInteractiveRunner.Verify(x => x.Run