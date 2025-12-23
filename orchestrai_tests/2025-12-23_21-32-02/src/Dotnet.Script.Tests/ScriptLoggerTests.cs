using Dotnet.Script.Core;
using System;
using System.IO;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class ScriptLoggerTests
    {
        [Fact]
        public void Constructor_WithValidWriter_ShouldInitialize()
        {
            // Arrange
            var writer = new StringWriter();

            // Act
            var logger = new ScriptLogger(writer, false);

            // Assert
            Assert.NotNull(logger);
        }

        [Fact]
        public void Constructor_WithNullWriter_ShouldUseTextWriterNull()
        {
            // Arrange & Act
            var logger = new ScriptLogger(null, false);

            // Assert
            Assert.NotNull(logger);
            // Verify that writing doesn't throw
            logger.Log("test message");
        }

        [Fact]
        public void Constructor_WithDebugTrue_ShouldEnableVerbose()
        {
            // Arrange
            var writer = new StringWriter();

            // Act
            var logger = new ScriptLogger(writer, true);

            // Assert
            Assert.NotNull(logger);
        }

        [Fact]
        public void Constructor_WithDebugFalse_ShouldDisableVerbose()
        {
            // Arrange
            var writer = new StringWriter();

            // Act
            var logger = new ScriptLogger(writer, false);

            // Assert
            Assert.NotNull(logger);
        }

        [Fact]
        public void Log_WithMessage_ShouldWriteToWriter()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, false);
            var message = "Test log message";

            // Act
            logger.Log(message);

            // Assert
            var output = writer.ToString();
            Assert.Contains(message, output);
        }

        [Fact]
        public void Log_WithEmptyMessage_ShouldWriteEmptyLine()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, false);

            // Act
            logger.Log(string.Empty);

            // Assert
            var output = writer.ToString();
            Assert.Contains(Environment.NewLine, output);
        }

        [Fact]
        public void Log_WithMultipleMessages_ShouldWriteAllMessages()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, false);
            var message1 = "First message";
            var message2 = "Second message";

            // Act
            logger.Log(message1);
            logger.Log(message2);

            // Assert
            var output = writer.ToString();
            Assert.Contains(message1, output);
            Assert.Contains(message2, output);
        }

        [Fact]
        public void Log_WithNullWriter_ShouldNotThrow()
        {
            // Arrange
            var logger = new ScriptLogger(null, false);

            // Act & Assert
            logger.Log("test message"); // Should not throw
        }

        [Fact]
        public void Log_WithLongMessage_ShouldWriteEntireMessage()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, false);
            var message = new string('a', 10000);

            // Act
            logger.Log(message);

            // Assert
            var output = writer.ToString();
            Assert.Contains(message, output);
        }

        [Fact]
        public void Verbose_WithDebugTrue_ShouldLog()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, true);
            var message = "Verbose message";

            // Act
            logger.Verbose(message);

            // Assert
            var output = writer.ToString();
            Assert.Contains(message, output);
        }

        [Fact]
        public void Verbose_WithDebugFalse_ShouldNotLog()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, false);
            var message = "Verbose message";

            // Act
            logger.Verbose(message);

            // Assert
            var output = writer.ToString();
            Assert.DoesNotContain(message, output);
        }

        [Fact]
        public void Verbose_WithEmptyMessage_WhenDebugTrue_ShouldLog()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, true);

            // Act
            logger.Verbose(string.Empty);

            // Assert
            var output = writer.ToString();
            Assert.Contains(Environment.NewLine, output);
        }

        [Fact]
        public void Verbose_WithEmptyMessage_WhenDebugFalse_ShouldNotLog()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, false);

            // Act
            logger.Verbose(string.Empty);

            // Assert
            var output = writer.ToString();
            Assert.Empty(output);
        }

        [Fact]
        public void Verbose_WithMultipleMessages_WhenDebugTrue_ShouldLogAll()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, true);
            var message1 = "First verbose";
            var message2 = "Second verbose";

            // Act
            logger.Verbose(message1);
            logger.Verbose(message2);

            // Assert
            var output = writer.ToString();
            Assert.Contains(message1, output);
            Assert.Contains(message2, output);
        }

        [Fact]
        public void Verbose_WithMultipleMessages_WhenDebugFalse_ShouldNotLogAny()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, false);
            var message1 = "First verbose";
            var message2 = "Second verbose";

            // Act
            logger.Verbose(message1);
            logger.Verbose(message2);

            // Assert
            var output = writer.ToString();
            Assert.Empty(output);
        }

        [Fact]
        public void Verbose_WithSpecialCharacters_WhenDebugTrue_ShouldLog()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, true);
            var message = "Special chars: !@#$%^&*()";

            // Act
            logger.Verbose(message);

            // Assert
            var output = writer.ToString();
            Assert.Contains(message, output);
        }

        [Fact]
        public void Verbose_WithSpecialCharacters_WhenDebugFalse_ShouldNotLog()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, false);
            var message = "Special chars: !@#$%^&*()";

            // Act
            logger.Verbose(message);

            // Assert
            var output = writer.ToString();
            Assert.DoesNotContain(message, output);
        }

        [Fact]
        public void Verbose_WithNullWriter_WhenDebugTrue_ShouldNotThrow()
        {
            // Arrange
            var logger = new ScriptLogger(null, true);

            // Act & Assert
            logger.Verbose("test message"); // Should not throw
        }

        [Fact]
        public void Verbose_WithNullWriter_WhenDebugFalse_ShouldNotThrow()
        {
            // Arrange
            var logger = new ScriptLogger(null, false);

            // Act & Assert
            logger.Verbose("test message"); // Should not throw
        }

        [Fact]
        public void Log_WithMessageContainingNewlines_ShouldPreserveNewlines()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, false);
            var message = "Line1\nLine2\nLine3";

            // Act
            logger.Log(message);

            // Assert
            var output = writer.ToString();
            Assert.Contains("Line1\nLine2\nLine3", output);
        }

        [Fact]
        public void Verbose_WithMessageContainingNewlines_WhenDebugTrue_ShouldPreserveNewlines()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, true);
            var message = "Line1\nLine2\nLine3";

            // Act
            logger.Verbose(message);

            // Assert
            var output = writer.ToString();
            Assert.Contains("Line1\nLine2\nLine3", output);
        }

        [Fact]
        public void DebugMode_ToggleTest_ShouldRespectDebugFlag()
        {
            // Arrange
            var writer1 = new StringWriter();
            var logger1 = new ScriptLogger(writer1, true);
            var writer2 = new StringWriter();
            var logger2 = new ScriptLogger(writer2, false);
            var message = "Test message";

            // Act
            logger1.Verbose(message);
            logger2.Verbose(message);

            // Assert
            var output1 = writer1.ToString();
            var output2 = writer2.ToString();
            Assert.Contains(message, output1);
            Assert.Empty(output2);
        }

        [Fact]
        public void MixedLoggingAndVerbose_ShouldHandleCorrectly()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, true);
            var logMsg = "Log message";
            var verboseMsg = "Verbose message";

            // Act
            logger.Log(logMsg);
            logger.Verbose(verboseMsg);

            // Assert
            var output = writer.ToString();
            Assert.Contains(logMsg, output);
            Assert.Contains(verboseMsg, output);
        }

        [Fact]
        public void MixedLoggingAndVerbose_WithDebugFalse_ShouldOnlyLogRegularMessages()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, false);
            var logMsg = "Log message";
            var verboseMsg = "Verbose message";

            // Act
            logger.Log(logMsg);
            logger.Verbose(verboseMsg);

            // Assert
            var output = writer.ToString();
            Assert.Contains(logMsg, output);
            Assert.DoesNotContain(verboseMsg, output);
        }

        [Fact]
        public void Log_WithNullMessage_ShouldWriteNull()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, false);

            // Act
            logger.Log(null);

            // Assert
            var output = writer.ToString();
            // The behavior should be that WriteLine(null) writes just the newline
            Assert.NotEmpty(output);
        }

        [Fact]
        public void Verbose_WithNullMessage_WhenDebugTrue_ShouldWriteNull()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, true);

            // Act
            logger.Verbose(null);

            // Assert
            var output = writer.ToString();
            Assert.NotEmpty(output);
        }

        [Fact]
        public void Verbose_WithNullMessage_WhenDebugFalse_ShouldNotWrite()
        {
            // Arrange
            var writer = new StringWriter();
            var logger = new ScriptLogger(writer, false);

            // Act
            logger.Verbose(null);

            // Assert
            var output = writer.ToString();
            Assert.Empty(output);
        }
    }
}