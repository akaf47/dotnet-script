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