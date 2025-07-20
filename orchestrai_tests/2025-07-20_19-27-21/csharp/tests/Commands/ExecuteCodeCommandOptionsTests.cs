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