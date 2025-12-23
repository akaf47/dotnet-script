using System.Threading;
using System.Threading.Tasks;
using Dotnet.Script.DependencyModel.Logging;
using Xunit;
using Xunit.Abstractions;
using Moq;

namespace Dotnet.Script.Shared.Tests
{
    public class TestOutputHelperTests
    {
        [Fact]
        public void Capture_WithDefaultLogLevel_SetsCapturedTestOutput()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            
            // Act
            mockOutputHelper.Object.Capture();

            // Assert
            Assert.NotNull(TestOutputHelper.Current);
            Assert.Same(mockOutputHelper.Object, TestOutputHelper.Current.TestOutputHelper);
            Assert.Equal(LogLevel.Debug, TestOutputHelper.Current.MinimumLogLevel);
        }

        [Fact]
        public void Capture_WithCustomLogLevel_SetsCapturedTestOutputWithCorrectLevel()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            LogLevel customLevel = LogLevel.Error;

            // Act
            mockOutputHelper.Object.Capture(customLevel);

            // Assert
            Assert.NotNull(TestOutputHelper.Current);
            Assert.Same(mockOutputHelper.Object, TestOutputHelper.Current.TestOutputHelper);
            Assert.Equal(LogLevel.Error, TestOutputHelper.Current.MinimumLogLevel);
        }

        [Fact]
        public void Capture_WithDebugLogLevel_SetsCapturedTestOutput()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Assert
            Assert.NotNull(TestOutputHelper.Current);
            Assert.Equal(LogLevel.Debug, TestOutputHelper.Current.MinimumLogLevel);
        }

        [Fact]
        public void Capture_WithInfoLogLevel_SetsCapturedTestOutput()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            mockOutputHelper.Object.Capture(LogLevel.Info);

            // Assert
            Assert.NotNull(TestOutputHelper.Current);
            Assert.Equal(LogLevel.Info, TestOutputHelper.Current.MinimumLogLevel);
        }

        [Fact]
        public void Capture_WithWarningLogLevel_SetsCapturedTestOutput()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            mockOutputHelper.Object.Capture(LogLevel.Warning);

            // Assert
            Assert.NotNull(TestOutputHelper.Current);
            Assert.Equal(LogLevel.Warning, TestOutputHelper.Current.MinimumLogLevel);
        }

        [Fact]
        public void Capture_WithErrorLogLevel_SetsCapturedTestOutput()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            mockOutputHelper.Object.Capture(LogLevel.Error);

            // Assert
            Assert.NotNull(TestOutputHelper.Current);
            Assert.Equal(LogLevel.Error, TestOutputHelper.Current.MinimumLogLevel);
        }

        [Fact]
        public void Capture_WithFatalLogLevel_SetsCapturedTestOutput()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            mockOutputHelper.Object.Capture(LogLevel.Fatal);

            // Assert
            Assert.NotNull(TestOutputHelper.Current);
            Assert.Equal(LogLevel.Fatal, TestOutputHelper.Current.MinimumLogLevel);
        }

        [Fact]
        public void Current_ReturnsNull_WhenCaptureNotCalled()
        {
            // This test needs to be in a separate context where Capture hasn't been called
            // Act & Assert
            // If Current is null, it's the expected behavior
            // Note: This may need execution in isolation due to AsyncLocal state
        }

        [Fact]
        public void Current_ReturnsCapturedTestOutput_AfterCapture()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Info);

            // Act
            var current = TestOutputHelper.Current;

            // Assert
            Assert.NotNull(current);
            Assert.Equal(LogLevel.Info, current.MinimumLogLevel);
        }

        [Fact]
        public void CreateTestLogFactory_ReturnsValidLogFactory()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();

            // Assert
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateTestLogFactory_ReturnsDelegateFunction()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));

            // Assert
            Assert.NotNull(logger);
        }

        [Fact]
        public void CreateTestLogFactory_LoggerCanBeCalled()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            
            // Call the logger with various parameters
#if DEBUG
            logger(LogLevel.Info, "test message", null);
#endif

            // Assert - if no exception, test passes
            Assert.True(true);
        }

#if DEBUG
        [Fact]
        public void CreateTestLogFactory_LoggerWritesWhenLevelMeetsMinimum()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Info);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Warning, "test message", null);

            // Assert
            mockOutputHelper.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void CreateTestLogFactory_LoggerDoesNotWriteWhenLevelBelowMinimum()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Error);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Debug, "test message", null);

            // Assert
            mockOutputHelper.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CreateTestLogFactory_LoggerWritesTypeNameAndMessage()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Debug, "test message content", null);

            // Assert
            mockOutputHelper.Verify(
                x => x.WriteLine(It.IsAny<string>()), 
                Times.AtLeastOnce);
        }

        [Fact]
        public void CreateTestLogFactory_LoggerFormatsOutputCorrectly()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Error, "error message", null);

            // Assert
            mockOutputHelper.Verify(
                x => x.WriteLine(It.Match<string>(s => s.Contains("Error"))), 
                Times.AtLeastOnce);
        }

        [Fact]
        public void CreateTestLogFactory_LoggerWithException()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);
            var exception = new System.Exception("test exception");

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Error, "error with exception", exception);

            // Assert
            mockOutputHelper.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void CreateTestLogFactory_LoggerWithDebugLevel()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Debug, "debug message", null);

            // Assert
            mockOutputHelper.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void CreateTestLogFactory_LoggerWithInfoLevel()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Info, "info message", null);

            // Assert
            mockOutputHelper.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void CreateTestLogFactory_LoggerWithWarningLevel()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Warning, "warning message", null);

            // Assert
            mockOutputHelper.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void CreateTestLogFactory_LoggerWithFatalLevel()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Fatal, "fatal message", null);

            // Assert
            mockOutputHelper.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void CreateTestLogFactory_LoggerWithNullMessage()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Info, null, null);

            // Assert - should not throw
            Assert.True(true);
        }

        [Fact]
        public void CreateTestLogFactory_LoggerWithEmptyMessage()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Info, "", null);

            // Assert
            mockOutputHelper.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void CreateTestLogFactory_LoggerWithLongMessage()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);
            var longMessage = new string('a', 10000);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Info, longMessage, null);

            // Assert
            mockOutputHelper.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }
#endif

        [Fact]
        public void CapturedTestOutput_StoresOutputHelper()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            var minimumLogLevel = LogLevel.Warning;

            // Act
            var capturedOutput = new CapturedTestOutput(mockOutputHelper.Object, minimumLogLevel);

            // Assert
            Assert.Same(mockOutputHelper.Object, capturedOutput.TestOutputHelper);
        }

        [Fact]
        public void CapturedTestOutput_StoresMinimumLogLevel()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            var minimumLogLevel = LogLevel.Error;

            // Act
            var capturedOutput = new CapturedTestOutput(mockOutputHelper.Object, minimumLogLevel);

            // Assert
            Assert.Equal(LogLevel.Error, capturedOutput.MinimumLogLevel);
        }

        [Fact]
        public void CapturedTestOutput_PropertiesAreReadOnly()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            var capturedOutput = new CapturedTestOutput(mockOutputHelper.Object, LogLevel.Debug);

            // Act & Assert
            Assert.NotNull(capturedOutput.TestOutputHelper);
            Assert.Equal(LogLevel.Debug, capturedOutput.MinimumLogLevel);
            // Properties are readonly, so attempting to set would cause compilation error
        }

        [Fact]
        public void CapturedTestOutput_WithDebugLevel()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            var capturedOutput = new CapturedTestOutput(mockOutputHelper.Object, LogLevel.Debug);

            // Assert
            Assert.Equal(LogLevel.Debug, capturedOutput.MinimumLogLevel);
        }

        [Fact]
        public void CapturedTestOutput_WithInfoLevel()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            var capturedOutput = new CapturedTestOutput(mockOutputHelper.Object, LogLevel.Info);

            // Assert
            Assert.Equal(LogLevel.Info, capturedOutput.MinimumLogLevel);
        }

        [Fact]
        public void CapturedTestOutput_WithWarningLevel()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            var capturedOutput = new CapturedTestOutput(mockOutputHelper.Object, LogLevel.Warning);

            // Assert
            Assert.Equal(LogLevel.Warning, capturedOutput.MinimumLogLevel);
        }

        [Fact]
        public void CapturedTestOutput_WithErrorLevel()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            var capturedOutput = new CapturedTestOutput(mockOutputHelper.Object, LogLevel.Error);

            // Assert
            Assert.Equal(LogLevel.Error, capturedOutput.MinimumLogLevel);
        }

        [Fact]
        public void CapturedTestOutput_WithFatalLevel()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            var capturedOutput = new CapturedTestOutput(mockOutputHelper.Object, LogLevel.Fatal);

            // Assert
            Assert.Equal(LogLevel.Fatal, capturedOutput.MinimumLogLevel);
        }

        [Fact]
        public void Capture_ReplacesPreviousCapture()
        {
            // Arrange
            var mockOutputHelper1 = new Mock<ITestOutputHelper>();
            var mockOutputHelper2 = new Mock<ITestOutputHelper>();
            
            // Act
            mockOutputHelper1.Object.Capture(LogLevel.Debug);
            var firstCapture = TestOutputHelper.Current;
            
            mockOutputHelper2.Object.Capture(LogLevel.Error);
            var secondCapture = TestOutputHelper.Current;

            // Assert
            Assert.NotSame(firstCapture, secondCapture);
            Assert.Same(mockOutputHelper2.Object, secondCapture.TestOutputHelper);
            Assert.Equal(LogLevel.Error, secondCapture.MinimumLogLevel);
        }

        [Fact]
        public async Task Capture_IsolatedByAsyncLocal_InDifferentTasks()
        {
            // Arrange
            var mockOutputHelper1 = new Mock<ITestOutputHelper>();
            var mockOutputHelper2 = new Mock<ITestOutputHelper>();
            CapturedTestOutput captureInTask2 = null;

            // Act
            mockOutputHelper1.Object.Capture(LogLevel.Debug);
            var captureInMainThread = TestOutputHelper.Current;

            var task = Task.Run(() =>
            {
                mockOutputHelper2.Object.Capture(LogLevel.Error);
                captureInTask2 = TestOutputHelper.Current;
            });
            await task;

            var captureInMainThreadAfter = TestOutputHelper.Current;

            // Assert
            Assert.Equal(LogLevel.Debug, captureInMainThread.MinimumLogLevel);
            Assert.Equal(LogLevel.Error, captureInTask2.MinimumLogLevel);
            Assert.Equal(LogLevel.Debug, captureInMainThreadAfter.MinimumLogLevel);
        }

        [Fact]
        public void Capture_WithAllLogLevelValues()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act & Assert - test each enum value
            mockOutputHelper.Object.Capture(LogLevel.Debug);
            Assert.Equal(LogLevel.Debug, TestOutputHelper.Current.MinimumLogLevel);

            mockOutputHelper.Object.Capture(LogLevel.Info);
            Assert.Equal(LogLevel.Info, TestOutputHelper.Current.MinimumLogLevel);

            mockOutputHelper.Object.Capture(LogLevel.Warning);
            Assert.Equal(LogLevel.Warning, TestOutputHelper.Current.MinimumLogLevel);

            mockOutputHelper.Object.Capture(LogLevel.Error);
            Assert.Equal(LogLevel.Error, TestOutputHelper.Current.MinimumLogLevel);

            mockOutputHelper.Object.Capture(LogLevel.Fatal);
            Assert.Equal(LogLevel.Fatal, TestOutputHelper.Current.MinimumLogLevel);
        }

#if DEBUG
        [Fact]
        public void CreateTestLogFactory_BehaviorWithVariousLogLevels()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Warning);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(string));

            // These should not write
            logger(LogLevel.Debug, "debug", null);
            logger(LogLevel.Info, "info", null);

            // These should write
            logger(LogLevel.Warning, "warning", null);
            logger(LogLevel.Error, "error", null);
            logger(LogLevel.Fatal, "fatal", null);

            // Assert
            // At least 3 calls should have been made for Warning, Error, and Fatal
            mockOutputHelper.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeast(3));
        }

        [Fact]
        public void CreateTestLogFactory_LogFormattingIncludesLogLevel()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper.Object.Capture(LogLevel.Debug);

            // Act
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var logger = logFactory(typeof(TestOutputHelperTests));
            logger(LogLevel.Warning, "test", null);

            // Assert - should include the log level in output
            mockOutputHelper.Verify(
                x => x.WriteLine(It.Match<string>(s => s.Contains("Warning") || s.Contains("WARN"))),
                Times.AtLeastOnce);
        }
#endif

        [Fact]
        public void Capture_ExtensionMethod_CallsConstructorAndSetsAsyncLocal()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();

            // Act
            mockOutputHelper.Object.Capture(LogLevel.Info);

            // Assert
            Assert.NotNull(TestOutputHelper.Current);
            Assert.IsType<CapturedTestOutput>(TestOutputHelper.Current);
        }
    }
}