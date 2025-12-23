using System;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using Xunit;
using Moq;

namespace Dotnet.Script.Core.Tests
{
    public class CSharpObjectFormatterExtensionsTests
    {
        [Fact]
        public void ToDisplayString_WithValidException_ReturnsFormattedString()
        {
            // Arrange
            var mockFormatter = new Mock<CSharpObjectFormatter>();
            var exception = new InvalidOperationException("Test exception message");
            var expectedFormattedMessage = "Test formatted message";
            
            mockFormatter
                .Setup(f => f.FormatException(exception))
                .Returns(expectedFormattedMessage);

            // Act
            var result = mockFormatter.Object.ToDisplayString(exception);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.InvalidOperationException", result);
            Assert.Contains(expectedFormattedMessage, result);
            Assert.StartsWith("System.InvalidOperationException: ", result);
        }

        [Fact]
        public void ToDisplayString_WithDifferentExceptionTypes_ReturnsCorrectTypeName()
        {
            // Arrange
            var mockFormatter = new Mock<CSharpObjectFormatter>();
            var exception = new ArgumentNullException("paramName");
            var expectedFormattedMessage = "Parameter was null";
            
            mockFormatter
                .Setup(f => f.FormatException(exception))
                .Returns(expectedFormattedMessage);

            // Act
            var result = mockFormatter.Object.ToDisplayString(exception);

            // Assert
            Assert.Contains("System.ArgumentNullException", result);
            Assert.Contains("Parameter was null", result);
        }

        [Fact]
        public void ToDisplayString_WithNullReferenceException_ReturnsFormattedString()
        {
            // Arrange
            var mockFormatter = new Mock<CSharpObjectFormatter>();
            var exception = new NullReferenceException();
            var expectedFormattedMessage = "Object reference not set";
            
            mockFormatter
                .Setup(f => f.FormatException(exception))
                .Returns(expectedFormattedMessage);

            // Act
            var result = mockFormatter.Object.ToDisplayString(exception);

            // Assert
            Assert.Contains("System.NullReferenceException", result);
            Assert.Contains(expectedFormattedMessage", result);
        }

        [Fact]
        public void ToDisplayString_WithEmptyFormattedMessage_ReturnsTypeWithColonAndEmpty()
        {
            // Arrange
            var mockFormatter = new Mock<CSharpObjectFormatter>();
            var exception = new Exception("test");
            var emptyMessage = string.Empty;
            
            mockFormatter
                .Setup(f => f.FormatException(exception))
                .Returns(emptyMessage);

            // Act
            var result = mockFormatter.Object.ToDisplayString(exception);

            // Assert
            Assert.Equal("System.Exception: ", result);
        }

        [Fact]
        public void ToDisplayString_WithMultilineFormattedMessage_IncludesFullMessage()
        {
            // Arrange
            var mockFormatter = new Mock<CSharpObjectFormatter>();
            var exception = new Exception("test");
            var multilineMessage = "Line1\nLine2\nLine3";
            
            mockFormatter
                .Setup(f => f.FormatException(exception))
                .Returns(multilineMessage);

            // Act
            var result = mockFormatter.Object.ToDisplayString(exception);

            // Assert
            Assert.Contains("Line1", result);
            Assert.Contains("Line2", result);
            Assert.Contains("Line3", result);
        }

        [Fact]
        public void ToDisplayString_WithSpecialCharactersInMessage_PreservesCharacters()
        {
            // Arrange
            var mockFormatter = new Mock<CSharpObjectFormatter>();
            var exception = new Exception("test");
            var specialMessage = "Message with !@#$%^&*() special chars";
            
            mockFormatter
                .Setup(f => f.FormatException(exception))
                .Returns(specialMessage);

            // Act
            var result = mockFormatter.Object.ToDisplayString(exception);

            // Assert
            Assert.Contains(specialMessage, result);
        }

        [Fact]
        public void ToDisplayString_FormatExceptionIsCalled()
        {
            // Arrange
            var mockFormatter = new Mock<CSharpObjectFormatter>();
            var exception = new Exception("test");
            
            mockFormatter
                .Setup(f => f.FormatException(exception))
                .Returns("formatted");

            // Act
            mockFormatter.Object.ToDisplayString(exception);

            // Assert
            mockFormatter.Verify(f => f.FormatException(exception), Times.Once);
        }

        [Fact]
        public void ToDisplayString_BuildsStringBuilderCorrectly()
        {
            // Arrange
            var mockFormatter = new Mock<CSharpObjectFormatter>();
            var exception = new ApplicationException("app error");
            var formattedMessage = "App Exception Details";
            
            mockFormatter
                .Setup(f => f.FormatException(exception))
                .Returns(formattedMessage);

            // Act
            var result = mockFormatter.Object.ToDisplayString(exception);

            // Assert
            var expectedResult = $"{exception.GetType()}: {formattedMessage}";
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ToDisplayString_WithLongMessage_HandlesCorrectly()
        {
            // Arrange
            var mockFormatter = new Mock<CSharpObjectFormatter>();
            var exception = new Exception();
            var longMessage = new string('x', 10000);
            
            mockFormatter
                .Setup(f => f.FormatException(exception))
                .Returns(longMessage);

            // Act
            var result = mockFormatter.Object.ToDisplayString(exception);

            // Assert
            Assert.Contains("xxxx", result);
            Assert.Contains(longMessage, result);
        }
    }
}