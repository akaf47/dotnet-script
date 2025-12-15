using Xunit;
using Microsoft.Extensions.Logging;

namespace Dotnet.Script.Tests
{
    public class LogHelperTests
    {
        [Fact]
        public void CreateLogFactory_ValidVerbosity_ReturnsLogFactory()
        {
            // Arrange
            string verbosity = "info";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_InvalidVerbosity_ReturnsLogFactory()
        {
            // Arrange
            string verbosity = "invalid";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void LogFactory_LogMessage_Success()
        {
            // Arrange
            var logFactory = LogHelper.CreateLogFactory("debug");
            var logger = logFactory(typeof(LogHelperTests));

            // Act
            logger(LogLevel.Information, "Test message", null);

            // No assertion, just ensure it doesn't throw an exception.
        }
    }
}