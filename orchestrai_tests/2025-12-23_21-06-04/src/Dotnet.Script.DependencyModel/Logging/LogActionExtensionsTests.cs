using System;
using System.Collections.Generic;
using Xunit;
using Dotnet.Script.DependencyModel.Logging;

namespace Dotnet.Script.DependencyModel.Tests.Logging
{
    public class LogExtensionsTests
    {
        private List<(LogLevel level, string message, Exception ex)> _logCalls;
        private LogFactory _logFactory;

        public LogExtensionsTests()
        {
            _logCalls = new List<(LogLevel level, string message, Exception ex)>();
            _logFactory = (type) => (level, message, ex) => _logCalls.Add((level, message, ex));
        }

        #region CreateLogger Tests
        [Fact]
        public void CreateLogger_GenericType_ReturnsLoggerForType()
        {
            // Arrange
            var logFactory = (Type type) =>
            {
                Assert.Equal(typeof(LogExtensionsTests), type);
                return (level, msg, ex) => { };
            };

            // Act
            var logger = logFactory.CreateLogger<LogExtensionsTests>();

            // Assert
            Assert.NotNull(logger);
        }

        [Fact]
        public void CreateLogger_WithValidType_CreatesLoggerSuccessfully()
        {
            // Arrange & Act
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Assert
            Assert.NotNull(logger);
            Assert.IsAssignableFrom<Logger>(logger);
        }
        #endregion

        #region Trace Tests
        [Fact]
        public void Trace_WithValidMessage_LogsAtTraceLevel()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();
            var message = "Test trace message";

            // Act
            logger.Trace(message);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Trace, _logCalls[0].level);
            Assert.Equal(message, _logCalls[0].message);
            Assert.Null(_logCalls[0].ex);
        }

        [Fact]
        public void Trace_WithEmptyMessage_LogsEmptyString()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Act
            logger.Trace("");

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Trace, _logCalls[0].level);
            Assert.Equal("", _logCalls[0].message);
        }

        [Fact]
        public void Trace_WithNullMessage_LogsNull()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Act
            logger.Trace(null);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Trace, _logCalls[0].level);
            Assert.Null(_logCalls[0].message);
        }
        #endregion

        #region Debug Tests
        [Fact]
        public void Debug_WithValidMessage_LogsAtDebugLevel()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();
            var message = "Test debug message";

            // Act
            logger.Debug(message);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Debug, _logCalls[0].level);
            Assert.Equal(message, _logCalls[0].message);
            Assert.Null(_logCalls[0].ex);
        }

        [Fact]
        public void Debug_WithEmptyMessage_LogsEmptyString()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Act
            logger.Debug("");

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Debug, _logCalls[0].level);
            Assert.Equal("", _logCalls[0].message);
        }

        [Fact]
        public void Debug_WithNullMessage_LogsNull()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Act
            logger.Debug(null);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Debug, _logCalls[0].level);
            Assert.Null(_logCalls[0].message);
        }
        #endregion

        #region Info Tests
        [Fact]
        public void Info_WithValidMessage_LogsAtInfoLevel()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();
            var message = "Test info message";

            // Act
            logger.Info(message);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Info, _logCalls[0].level);
            Assert.Equal(message, _logCalls[0].message);
            Assert.Null(_logCalls[0].ex);
        }

        [Fact]
        public void Info_WithEmptyMessage_LogsEmptyString()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Act
            logger.Info("");

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Info, _logCalls[0].level);
            Assert.Equal("", _logCalls[0].message);
        }

        [Fact]
        public void Info_WithNullMessage_LogsNull()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Act
            logger.Info(null);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Info, _logCalls[0].level);
            Assert.Null(_logCalls[0].message);
        }
        #endregion

        #region Warning Tests
        [Fact]
        public void Warning_WithValidMessage_LogsAtWarningLevel()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();
            var message = "Test warning message";

            // Act
            logger.Warning(message);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Warning, _logCalls[0].level);
            Assert.Equal(message, _logCalls[0].message);
            Assert.Null(_logCalls[0].ex);
        }

        [Fact]
        public void Warning_WithEmptyMessage_LogsEmptyString()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Act
            logger.Warning("");

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Warning, _logCalls[0].level);
            Assert.Equal("", _logCalls[0].message);
        }

        [Fact]
        public void Warning_WithNullMessage_LogsNull()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Act
            logger.Warning(null);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Warning, _logCalls[0].level);
            Assert.Null(_logCalls[0].message);
        }
        #endregion

        #region Error Tests
        [Fact]
        public void Error_WithValidMessage_LogsAtErrorLevel()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();
            var message = "Test error message";

            // Act
            logger.Error(message);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Error, _logCalls[0].level);
            Assert.Equal(message, _logCalls[0].message);
            Assert.Null(_logCalls[0].ex);
        }

        [Fact]
        public void Error_WithMessageAndException_LogsErrorWithException()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();
            var message = "Test error message";
            var exception = new InvalidOperationException("Test exception");

            // Act
            logger.Error(message, exception);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Error, _logCalls[0].level);
            Assert.Equal(message, _logCalls[0].message);
            Assert.Equal(exception, _logCalls[0].ex);
        }

        [Fact]
        public void Error_WithEmptyMessage_LogsEmptyString()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Act
            logger.Error("");

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Error, _logCalls[0].level);
            Assert.Equal("", _logCalls[0].message);
            Assert.Null(_logCalls[0].ex);
        }

        [Fact]
        public void Error_WithNullMessage_LogsNull()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Act
            logger.Error(null);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Error, _logCalls[0].level);
            Assert.Null(_logCalls[0].message);
            Assert.Null(_logCalls[0].ex);
        }

        [Fact]
        public void Error_WithMessageAndNullException_LogsErrorWithNullException()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();
            var message = "Test error message";

            // Act
            logger.Error(message, null);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Error, _logCalls[0].level);
            Assert.Equal(message, _logCalls[0].message);
            Assert.Null(_logCalls[0].ex);
        }

        [Fact]
        public void Error_WithEmptyMessageAndException_LogsEmptyStringWithException()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();
            var exception = new ArgumentException("Test exception");

            // Act
            logger.Error("", exception);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Error, _logCalls[0].level);
            Assert.Equal("", _logCalls[0].message);
            Assert.Equal(exception, _logCalls[0].ex);
        }
        #endregion

        #region Critical Tests
        [Fact]
        public void Critical_WithValidMessage_LogsAtCriticalLevel()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();
            var message = "Test critical message";

            // Act
            logger.Critical(message);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Critical, _logCalls[0].level);
            Assert.Equal(message, _logCalls[0].message);
            Assert.Null(_logCalls[0].ex);
        }

        [Fact]
        public void Critical_WithMessageAndException_LogsCriticalWithException()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();
            var message = "Test critical message";
            var exception = new SystemException("Test exception");

            // Act
            logger.Critical(message, exception);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Critical, _logCalls[0].level);
            Assert.Equal(message, _logCalls[0].message);
            Assert.Equal(exception, _logCalls[0].ex);
        }

        [Fact]
        public void Critical_WithEmptyMessage_LogsEmptyString()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Act
            logger.Critical("");

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Critical, _logCalls[0].level);
            Assert.Equal("", _logCalls[0].message);
            Assert.Null(_logCalls[0].ex);
        }

        [Fact]
        public void Critical_WithNullMessage_LogsNull()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();

            // Act
            logger.Critical(null);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Critical, _logCalls[0].level);
            Assert.Null(_logCalls[0].message);
            Assert.Null(_logCalls[0].ex);
        }

        [Fact]
        public void Critical_WithMessageAndNullException_LogsCriticalWithNullException()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();
            var message = "Test critical message";

            // Act
            logger.Critical(message, null);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Critical, _logCalls[0].level);
            Assert.Equal(message, _logCalls[0].message);
            Assert.Null(_logCalls[0].ex);
        }

        [Fact]
        public void Critical_WithEmptyMessageAndException_LogsEmptyStringWithException()
        {
            // Arrange
            var logger = _logFactory.CreateLogger<LogExtensionsTests>();
            var exception = new ApplicationException("Test exception");

            // Act
            logger.Critical("", exception);

            // Assert
            Assert.Single(_logCalls);
            Assert.Equal(LogLevel.Critical, _logCalls[0].level);
            Assert.Equal("", _logCalls[0].message);
            Assert.Equal(exception, _logCalls[0].ex);
        }
        #endregion
    }

    public class LevelMapperTests
    {
        #region FromString - Trace Tests
        [Theory]
        [InlineData("t")]
        [InlineData("trace")]
        [InlineData("T")]
        [InlineData("TRACE")]
        [InlineData("Trace")]
        public void FromString_WithTraceValues_ReturnsTraceLevel(string value)
        {
            // Act
            var level = LevelMapper.FromString(value);

            // Assert
            Assert.Equal(LogLevel.Trace, level);
        }
        #endregion

        #region FromString - Debug Tests
        [Theory]
        [InlineData("d")]
        [InlineData("debug")]
        [InlineData("D")]
        [InlineData("DEBUG")]
        [InlineData("Debug")]
        public void FromString_WithDebugValues_ReturnsDebugLevel(string value)
        {
            // Act
            var level = LevelMapper.FromString(value);

            // Assert
            Assert.Equal(LogLevel.Debug, level);
        }
        #endregion

        #region FromString - Info Tests
        [Theory]
        [InlineData("i")]
        [InlineData("info")]
        [InlineData("I")]
        [InlineData("INFO")]
        [InlineData("Info")]
        public void FromString_WithInfoValues_ReturnsInfoLevel(string value)
        {
            // Act
            var level = LevelMapper.FromString(value);

            // Assert
            Assert.Equal(LogLevel.Info, level);
        }
        #endregion

        #region FromString - Warning Tests
        [Theory]
        [InlineData("w")]
        [InlineData("warning")]
        [InlineData("W")]
        [InlineData("WARNING")]
        [InlineData("Warning")]
        public void FromString_WithWarningValues_ReturnsWarningLevel(string value)
        {
            // Act
            var level = LevelMapper.FromString(value);

            // Assert
            Assert.Equal(LogLevel.Warning, level);
        }
        #endregion

        #region FromString - Error Tests
        [Theory]
        [InlineData("e")]
        [InlineData("error")]
        [InlineData("E")]
        [InlineData("ERROR")]
        [InlineData("Error")]
        public void FromString_WithErrorValues_ReturnsErrorLevel(string value)
        {
            // Act
            var level = LevelMapper.FromString(value);

            // Assert
            Assert.Equal(LogLevel.Error, level);
        }
        #endregion

        #region FromString - Critical Tests
        [Theory]
        [InlineData("c")]
        [InlineData("critical")]
        [InlineData("C")]
        [InlineData("CRITICAL")]
        [InlineData("Critical")]
        public void FromString_WithCriticalValues_ReturnsCriticalLevel(string value)
        {
            // Act
            var level = LevelMapper.FromString(value);

            // Assert
            Assert.Equal(LogLevel.Critical, level);
        }
        #endregion

        #region FromString - Invalid/Default Tests
        [Fact]
        public void FromString_WithNullValue_ReturnsWarningLevel()
        {
            // Act
            var level = LevelMapper.FromString(null);

            // Assert
            Assert.Equal(LogLevel.Warning, level);
        }

        [Fact]
        public void FromString_WithEmptyString_ReturnsWarningLevel()
        {
            // Act
            var level = LevelMapper.FromString("");

            // Assert
            Assert.Equal(LogLevel.Warning, level);
        }

        [Fact]
        public void FromString_WithWhitespaceOnly_ReturnsWarningLevel()
        {
            // Act
            var level = LevelMapper.FromString("   ");

            // Assert
            Assert.Equal(LogLevel.Warning, level);
        }

        [Fact]
        public void FromString_WithUnknownValue_ReturnsWarningLevel()
        {
            // Act
            var level = LevelMapper.FromString("unknown");

            // Assert
            Assert.Equal(LogLevel.Warning, level);
        }

        [Fact]
        public void FromString_WithRandomString_ReturnsWarningLevel()
        {
            // Act
            var level = LevelMapper.FromString("xyz");

            // Assert
            Assert.Equal(LogLevel.Warning, level);
        }

        [Fact]
        public void FromString_WithPartialMatch_ReturnsWarningLevel()
        {
            // Act
            var level = LevelMapper.FromString("tr");

            // Assert
            Assert.Equal(LogLevel.Warning, level);
        }

        [Fact]
        public void FromString_WithTabCharacter_ReturnsWarningLevel()
        {
            // Act
            var level = LevelMapper.FromString("\t");

            // Assert
            Assert.Equal(LogLevel.Warning, level);
        }

        [Fact]
        public void FromString_WithNewlineCharacter_ReturnsWarningLevel()
        {
            // Act
            var level = LevelMapper.FromString("\n");

            // Assert
            Assert.Equal(LogLevel.Warning, level);
        }

        [Fact]
        public void FromString_WithMixedCase_TraceWorks()
        {
            // Act
            var level = LevelMapper.FromString("tRaCe");

            // Assert
            Assert.Equal(LogLevel.Trace, level);
        }

        [Fact]
        public void FromString_WithMixedCase_DebugWorks()
        {
            // Act
            var level = LevelMapper.FromString("DeBeWg");

            // Assert
            Assert.Equal(LogLevel.Warning, level);
        }

        [Fact]
        public void FromString_WithNumber_ReturnsWarningLevel()
        {
            // Act
            var level = LevelMapper.FromString("123");

            // Assert
            Assert.Equal(LogLevel.Warning, level);
        }
        #endregion

        #region CreateMap Tests
        [Fact]
        public void CreateMap_ContainsAllTraceMappings()
        {
            // Act
            var level1 = LevelMapper.FromString("t");
            var level2 = LevelMapper.FromString("trace");

            // Assert
            Assert.Equal(LogLevel.Trace, level1);
            Assert.Equal(LogLevel.Trace, level2);
        }

        [Fact]
        public void CreateMap_ContainsAllDebugMappings()
        {
            // Act
            var level1 = LevelMapper.FromString("d");
            var level2 = LevelMapper.FromString("debug");

            // Assert
            Assert.Equal(LogLevel.Debug, level1);
            Assert.Equal(LogLevel.Debug, level2);
        }

        [Fact]
        public void CreateMap_ContainsAllInfoMappings()
        {
            // Act
            var level1 = LevelMapper.FromString("i");
            var level2 = LevelMapper.FromString("info");

            // Assert
            Assert.Equal(LogLevel.Info, level1);
            Assert.Equal(LogLevel.Info, level2);
        }

        [Fact]
        public void CreateMap_ContainsAllWarningMappings()
        {
            // Act
            var level1 = LevelMapper.FromString("w");
            var level2 = LevelMapper.FromString("warning");

            // Assert
            Assert.Equal(LogLevel.Warning, level1);
            Assert.Equal(LogLevel.Warning, level2);
        }

        [Fact]
        public void CreateMap_ContainsAllErrorMappings()
        {
            // Act
            var level1 = LevelMapper.FromString("e");
            var level2 = LevelMapper.FromString("error");

            // Assert
            Assert.Equal(LogLevel.Error, level1);
            Assert.Equal(LogLevel.Error, level2);
        }

        [Fact]
        public void CreateMap_ContainsAllCriticalMappings()
        {
            // Act
            var level1 = LevelMapper.FromString("c");
            var level2 = LevelMapper.FromString("critical");

            // Assert
            Assert.Equal(LogLevel.Critical, level1);
            Assert.Equal(LogLevel.Critical, level2);
        }
        #endregion
    }
}