using System;
using System.Collections.Generic;
using Dotnet.Script.Core;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using Xunit;

namespace Dotnet.Script.Core.Tests
{
    public class CSharpObjectFormatterExtensionsTests
    {
        #region ToDisplayString Tests

        [Fact]
        public void ToDisplayString_WithSimpleException_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new Exception("Test error message");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
            Assert.Contains("System.Exception", result);
            Assert.Contains(":", result);
            Assert.Contains("Test error message", result);
        }

        [Fact]
        public void ToDisplayString_WithInvalidOperationException_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new InvalidOperationException("Invalid operation occurred");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.InvalidOperationException", result);
            Assert.Contains(":", result);
            Assert.Contains("Invalid operation occurred", result);
        }

        [Fact]
        public void ToDisplayString_WithArgumentNullException_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new ArgumentNullException("paramName", "Parameter cannot be null");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.ArgumentNullException", result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void ToDisplayString_WithArgumentException_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new ArgumentException("Argument is invalid");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.ArgumentException", result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void ToDisplayString_WithIOException_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new IOException("File not found");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.IO.IOException", result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void ToDisplayString_WithNullReferenceException_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new NullReferenceException("Object reference not set");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.NullReferenceException", result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void ToDisplayString_WithIndexOutOfRangeException_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new IndexOutOfRangeException("Index is out of range");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.IndexOutOfRangeException", result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void ToDisplayString_WithNotImplementedException_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new NotImplementedException("Method not implemented");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.NotImplementedException", result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void ToDisplayString_WithEmptyMessage_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new Exception("");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.Exception", result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void ToDisplayString_WithNullMessage_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new Exception(null);
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.Exception", result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void ToDisplayString_WithInnerException_FormatsOuterException()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var innerException = new IOException("Inner error");
            var exception = new Exception("Outer error", innerException);
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.Exception", result);
            Assert.Contains(":", result);
            Assert.Contains("Outer error", result);
        }

        [Fact]
        public void ToDisplayString_WithLongMessage_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            string longMessage = new string('a', 1000);
            var exception = new Exception(longMessage);
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.Exception", result);
            Assert.Contains(":", result);
            Assert.Contains(longMessage, result);
        }

        [Fact]
        public void ToDisplayString_WithSpecialCharactersInMessage_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new Exception("Error: !@#$%^&*()[]{}|;:,.<>?");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.Exception", result);
            Assert.Contains(":", result);
            Assert.Contains("Error:", result);
        }

        [Fact]
        public void ToDisplayString_WithMultilineMessage_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new Exception("Line 1\nLine 2\nLine 3");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.Exception", result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void ToDisplayString_WithUnicodeInMessage_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new Exception("Unicode: 你好世界 🌍");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.Exception", result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void ToDisplayString_WithHStackTraceException_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            Exception exception = null;
            try
            {
                throw new Exception("Exception with stack trace");
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.Exception", result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void ToDisplayString_WithAggregateException_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var innerExceptions = new List<Exception>
            {
                new Exception("Error 1"),
                new Exception("Error 2")
            };
            var exception = new AggregateException("Multiple errors", innerExceptions);
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.AggregateException", result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void ToDisplayString_WithCustomException_FormatsCorrectly()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new CustomTestException("Custom error message");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains(typeof(CustomTestException).FullName, result);
            Assert.Contains(":", result);
            Assert.Contains("Custom error message", result);
        }

        [Fact]
        public void ToDisplayString_MultipleExceptionsFormattedDifferently()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var ex1 = new Exception("Message 1");
            var ex2 = new Exception("Message 2");
            
            // Act
            var result1 = ex1.ToDisplayString();
            var result2 = ex2.ToDisplayString();
            
            // Assert
            Assert.NotEqual(result1, result2);
            Assert.Contains("Message 1", result1);
            Assert.Contains("Message 2", result2);
        }

        [Fact]
        public void ToDisplayString_FormatterCalled()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new Exception("Test");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert - Verify that FormatException was called (result should contain formatted content)
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void ToDisplayString_ReturnTypeIsString()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new Exception("Test");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            Assert.IsType<string>(result);
        }

        [Fact]
        public void ToDisplayString_ContainsColonSeparator()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new Exception("Test message");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert - Verify format is: "ExceptionType: FormattedMessage"
            Assert.Contains("System.Exception", result);
            Assert.Contains(":", result);
            var colonIndex = result.IndexOf(':');
            Assert.NotEqual(-1, colonIndex);
        }

        [Fact]
        public void ToDisplayString_ExceptionTypeBeforeColon()
        {
            // Arrange
            var formatter = new CSharpObjectFormatter();
            var exception = new InvalidOperationException("Test");
            
            // Act
            var result = exception.ToDisplayString();
            
            // Assert
            var colonIndex = result.IndexOf(':');
            var exceptionTypePart = result.Substring(0, colonIndex);
            Assert.Contains("InvalidOperationException", exceptionTypePart);
        }

        #endregion
    }

    // Custom exception for testing
    internal class CustomTestException : Exception
    {
        public CustomTestException(string message) : base(message)
        {
        }
    }
}