using System;
using System.IO;
using Xunit;
using Moq;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptLoggerTests
    {
        [Fact]
        public void Constructor_ShouldHandleNullWriter()
        {
            var logger = new ScriptLogger(null, false);
            Assert.NotNull(logger);
        }

        [Fact]
        public void Log_ShouldWriteMessageToWriter()
        {
            var mockWriter = new Mock<TextWriter>();
            var logger = new ScriptLogger(mockWriter.Object, false);

            logger.Log("Test Message");

            mockWriter.Verify(x => x.WriteLine("Test Message"), Times.Once);
        }

        [Fact]
        public void Verbose_ShouldWriteMessageWhenInDebugMode()
        {
            var mockWriter = new Mock<TextWriter>();
            var logger = new ScriptLogger(mockWriter.Object, true);

            logger.Verbose("Debug Message");

            mockWriter.Verify(x => x.WriteLine("Debug Message"), Times.Once);
        }

        [Fact]
        public void Verbose_ShouldNotWriteMessageWhenNotInDebugMode()
        {
            var mockWriter = new Mock<TextWriter>();
            var logger = new ScriptLogger(mockWriter.Object, false);

            logger.Verbose("Debug Message");

            mockWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Never);
        }
    }
}