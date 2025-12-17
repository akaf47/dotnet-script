using System;
using Xunit;
using Dotnet.Script.Core;

public class ScriptRuntimeExceptionTests
{
    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldInitializeCorrectly()
    {
        // Arrange
        string message = "Test error message";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new ScriptRuntimeException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void Constructor_ShouldInheritFromException()
    {
        // Arrange
        string message = "Test error message";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new ScriptRuntimeException(message, innerException);

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }
}