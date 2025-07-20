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