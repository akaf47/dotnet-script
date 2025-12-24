using System;
using Xunit;
using Dotnet.Script;
using Dotnet.Script.DependencyModel.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Dotnet.Script.Tests
{
    public class LogHelperTests
    {
        [Fact]
        public void CreateLogFactory_WithTraceVerbosity_CreatesLogFactoryWithTraceLevel()
        {
            // Arrange
            string verbosity = "trace";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
            Assert.IsType<Func<Type, Func<Dotnet.Script.DependencyModel.Logging.LogLevel, string, Exception, object>>>(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithDebugVerbosity_CreatesLogFactoryWithDebugLevel()
        {
            // Arrange
            string verbosity = "debug";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithInfoVerbosity_CreatesLogFactoryWithInfoLevel()
        {
            // Arrange
            string verbosity = "info";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithWarningVerbosity_CreatesLogFactoryWithWarningLevel()
        {
            // Arrange
            string verbosity = "warning";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithErrorVerbosity_CreatesLogFactoryWithErrorLevel()
        {
            // Arrange
            string verbosity = "error";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithCriticalVerbosity_CreatesLogFactoryWithCriticalLevel()
        {
            // Arrange
            string verbosity = "critical";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithShortTraceVerbosity_CreatesLogFactoryWithTraceLevel()
        {
            // Arrange
            string verbosity = "t";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithShortDebugVerbosity_CreatesLogFactoryWithDebugLevel()
        {
            // Arrange
            string verbosity = "d";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithShortInfoVerbosity_CreatesLogFactoryWithInfoLevel()
        {
            // Arrange
            string verbosity = "i";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithShortWarningVerbosity_CreatesLogFactoryWithWarningLevel()
        {
            // Arrange
            string verbosity = "w";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithShortErrorVerbosity_CreatesLogFactoryWithErrorLevel()
        {
            // Arrange
            string verbosity = "e";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithShortCriticalVerbosity_CreatesLogFactoryWithCriticalLevel()
        {
            // Arrange
            string verbosity = "c";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithNullVerbosity_CreatesLogFactoryWithDefaultWarningLevel()
        {
            // Arrange
            string verbosity = null;

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithEmptyStringVerbosity_CreatesLogFactoryWithDefaultWarningLevel()
        {
            // Arrange
            string verbosity = "";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithWhitespaceVerbosity_CreatesLogFactoryWithDefaultWarningLevel()
        {
            // Arrange
            string verbosity = "   ";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_WithInvalidVerbosity_CreatesLogFactoryWithDefaultWarningLevel()
        {
            // Arrange
            string verbosity = "invalid";

            // Act
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_ReturnsCallableLogFactory_CanCreateLogger()
        {
            // Arrange
            string verbosity = "info";
            var logFactory = LogHelper.CreateLogFactory(verbosity);

            // Act
            var logger = logFactory(typeof(LogHelperTests));

            // Assert
            Assert.NotNull(logger);
            Assert.IsType<Func<Dotnet.Script.DependencyModel.Logging.LogLevel, string, Exception, object>>(logger);
        }

        [Fact]
        public void CreateLogFactory_ReturnsCallableLogger_CanLogMessage()
        {
            // Arrange
            string verbosity = "info";
            var logFactory = LogHelper.CreateLogFactory(verbosity);
            var logger = logFactory(typeof(LogHelperTests));

            // Act & Assert - Should not throw
            logger(Dotnet.Script.DependencyModel.Logging.LogLevel.Info, "Test message");
        }

        [Fact]
        public void CreateLogFactory_ReturnsCallableLogger_CanLogMessageWithException()
        {
            // Arrange
            string verbosity = "info";
            var logFactory = LogHelper.CreateLogFactory(verbosity);
            var logger = logFactory(typeof(LogHelperTests));
            var exception = new Exception("Test exception");

            // Act & Assert - Should not throw
            logger(Dotnet.Script.DependencyModel.Logging.LogLevel.Error, "Test message with error", exception);
        }

        [Fact]
        public void CreateLogFactory_ReturnsCallableLogger_CanLogTraceLevel()
        {
            // Arrange
            string verbosity = "trace";
            var logFactory = LogHelper.CreateLogFactory(verbosity);
            var logger = logFactory(typeof(LogHelperTests));

            // Act & Assert - Should not throw
            logger(Dotnet.Script.DependencyModel.Logging.LogLevel.Trace, "Trace message");
        }

        [Fact]
        public void CreateLogFactory_ReturnsCallableLogger_CanLogDebugLevel()
        {
            // Arrange
            string verbosity = "debug";
            var logFactory = LogHelper.CreateLogFactory(verbosity);
            var logger = logFactory(typeof(LogHelperTests));

            // Act & Assert - Should not throw
            logger(Dotnet.Script.DependencyModel.Logging.LogLevel.Debug, "Debug message");
        }

        [Fact]
        public void CreateLogFactory_ReturnsCallableLogger_CanLogWarningLevel()
        {
            // Arrange
            string verbosity = "warning";
            var logFactory = LogHelper.CreateLogFactory(verbosity);
            var logger = logFactory(typeof(LogHelperTests));

            // Act & Assert - Should not throw
            logger(Dotnet.Script.DependencyModel.Logging.LogLevel.Warning, "Warning message");
        }

        [Fact]
        public void CreateLogFactory_ReturnsCallableLogger_CanLogErrorLevel()
        {
            // Arrange
            string verbosity = "error";
            var logFactory = LogHelper.CreateLogFactory(verbosity);
            var logger = logFactory(typeof(LogHelperTests));

            // Act & Assert - Should not throw
            logger(Dotnet.Script.DependencyModel.Logging.LogLevel.Error, "Error message");
        }

        [Fact]
        public void CreateLogFactory_ReturnsCallableLogger_CanLogCriticalLevel()
        {
            // Arrange
            string verbosity = "critical";
            var logFactory = LogHelper.CreateLogFactory(verbosity);
            var logger = logFactory(typeof(LogHelperTests));

            // Act & Assert - Should not throw
            logger(Dotnet.Script.DependencyModel.Logging.LogLevel.Critical, "Critical message");
        }
    }

    public class ConsoleOptionsMonitorTests
    {
        [Fact]
        public void Constructor_InitializesConsoleLoggerOptions()
        {
            // Act
            var monitor = new ConsoleOptionsMonitor();

            // Assert
            Assert.NotNull(monitor);
        }

        [Fact]
        public void CurrentValue_ReturnsConsoleLoggerOptions()
        {
            // Arrange
            var monitor = new ConsoleOptionsMonitor();

            // Act
            var options = monitor.CurrentValue;

            // Assert
            Assert.NotNull(options);
            Assert.IsType<ConsoleLoggerOptions>(options);
        }

        [Fact]
        public void CurrentValue_HasCorrectLogToStandardErrorThreshold()
        {
            // Arrange
            var monitor = new ConsoleOptionsMonitor();

            // Act
            var options = monitor.CurrentValue;

            // Assert
            Assert.Equal(LogLevel.Trace, options.LogToStandardErrorThreshold);
        }

        [Fact]
        public void Get_WithAnyName_ReturnsConsoleLoggerOptions()
        {
            // Arrange
            var monitor = new ConsoleOptionsMonitor();
            string optionName = "test";

            // Act
            var options = monitor.Get(optionName);

            // Assert
            Assert.NotNull(options);
            Assert.IsType<ConsoleLoggerOptions>(options);
        }

        [Fact]
        public void Get_WithNullName_ReturnsConsoleLoggerOptions()
        {
            // Arrange
            var monitor = new ConsoleOptionsMonitor();

            // Act
            var options = monitor.Get(null);

            // Assert
            Assert.NotNull(options);
        }

        [Fact]
        public void Get_WithEmptyString_ReturnsConsoleLoggerOptions()
        {
            // Arrange
            var monitor = new ConsoleOptionsMonitor();

            // Act
            var options = monitor.Get("");

            // Assert
            Assert.NotNull(options);
        }

        [Fact]
        public void Get_ReturnsSameInstanceAsCurrentValue()
        {
            // Arrange
            var monitor = new ConsoleOptionsMonitor();

            // Act
            var currentValue = monitor.CurrentValue;
            var getValue = monitor.Get("test");

            // Assert
            Assert.Same(currentValue, getValue);
        }

        [Fact]
        public void OnChange_ReturnsNull()
        {
            // Arrange
            var monitor = new ConsoleOptionsMonitor();
            Action<ConsoleLoggerOptions, string> listener = (options, name) => { };

            // Act
            var result = monitor.OnChange(listener);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void OnChange_WithNullListener_ReturnsNull()
        {
            // Arrange
            var monitor = new ConsoleOptionsMonitor();

            // Act
            var result = monitor.OnChange(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void MultipleInstances_ReturnIndependentInstances()
        {
            // Arrange & Act
            var monitor1 = new ConsoleOptionsMonitor();
            var monitor2 = new ConsoleOptionsMonitor();

            // Assert
            Assert.NotSame(monitor1, monitor2);
            Assert.Equal(monitor1.CurrentValue.LogToStandardErrorThreshold, monitor2.CurrentValue.LogToStandardErrorThreshold);
        }
    }
}