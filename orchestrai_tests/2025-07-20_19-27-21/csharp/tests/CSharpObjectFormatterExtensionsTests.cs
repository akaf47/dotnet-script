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