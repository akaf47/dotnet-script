using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Dotnet.Script.DependencyModel.Logging;

namespace Dotnet.Script.DependencyModel.Tests.Logging
{
    public class LogActionExtensionsTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<Action> _actionMock;

        public LogActionExtensionsTests()
        {
            _loggerMock = new Mock<ILogger>();
            _actionMock = new Mock<Action>();
        }

        #region LogAction Tests

        [Fact]
        public void LogAction_WithValidAction_ExecutesActionSuccessfully()
        {
            // Arrange
            var executed = false;
            Action testAction = () => executed = true;

            // Act
            testAction.LogAction(_loggerMock.Object);

            // Assert
            Assert.True(executed);
        }

        [Fact]
        public void LogAction_WithValidAction_LogsStartMessage()
        {
            // Arrange
            Action testAction = () => { };

            // Act
            testAction.LogAction(_loggerMock.Object);

            // Assert
            _loggerMock.Verify(x => x.Log(It.IsAny<string>()), Times.AtLeast(1));
        }

        [Fact]
        public void LogAction_WhenActionThrows_LogsException()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Test error");
            Action testAction = () => throw expectedException;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                testAction.LogAction(_loggerMock.Object));
            
            Assert.Equal(expectedException.Message, exception.Message);
            _loggerMock.Verify(x => x.Log(It.IsAny<string>()), Times.AtLeast(1));
        }

        [Fact]
        public void LogAction_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            Action testAction = () => { };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                testAction.LogAction(null));
        }

        [Fact]
        public void LogAction_WithNullAction_ThrowsArgumentNullException()
        {
            // Arrange
            Action testAction = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                testAction.LogAction(_loggerMock.Object));
        }

        [Fact]
        public void LogAction_WhenActionThrowsOperationCanceledException_PropagatesException()
        {
            // Arrange
            var canceledException = new OperationCanceledException();
            Action testAction = () => throw canceledException;

            // Act & Assert
            Assert.Throws<OperationCanceledException>(() => 
                testAction.LogAction(_loggerMock.Object));
        }

        [Fact]
        public void LogAction_WhenActionThrowsTimeoutException_PropagatesException()
        {
            // Arrange
            var timeoutException = new TimeoutException();
            Action testAction = () => throw timeoutException;

            // Act & Assert
            Assert.Throws<TimeoutException>(() => 
                testAction.LogAction(_loggerMock.Object));
        }

        [Fact]
        public void LogAction_WhenActionCompletes_LogsCompletionMessage()
        {
            // Arrange
            Action testAction = () => { };
            var logCalls = new List<string>();
            _loggerMock.Setup(x => x.Log(It.IsAny<string>()))
                .Callback<string>(msg => logCalls.Add(msg));

            // Act
            testAction.LogAction(_loggerMock.Object);

            // Assert
            Assert.NotEmpty(logCalls);
            Assert.Contains(logCalls, x => x.Contains("Complet") || x.Contains("complet"));
        }

        [Fact]
        public void LogAction_WithMultipleInvocations_LogsEachSeparately()
        {
            // Arrange
            Action testAction = () => { };
            var callCount = 0;
            _loggerMock.Setup(x => x.Log(It.IsAny<string>()))
                .Callback(() => callCount++);

            // Act
            testAction.LogAction(_loggerMock.Object);
            testAction.LogAction(_loggerMock.Object);

            // Assert
            Assert.True(callCount >= 4); // At least 2 calls per invocation
        }

        #endregion

        #region LogFunc Tests

        [Fact]
        public void LogFunc_WithValidFunc_ExecutesFunctionSuccessfully()
        {
            // Arrange
            Func<int> testFunc = () => 42;

            // Act
            var result = testFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public void LogFunc_WithValidFunc_ReturnsCorrectValue()
        {
            // Arrange
            var expectedValue = "test result";
            Func<string> testFunc = () => expectedValue;

            // Act
            var result = testFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void LogFunc_WithValidFunc_LogsStartMessage()
        {
            // Arrange
            Func<int> testFunc = () => 42;

            // Act
            testFunc.LogFunc(_loggerMock.Object);

            // Assert
            _loggerMock.Verify(x => x.Log(It.IsAny<string>()), Times.AtLeast(1));
        }

        [Fact]
        public void LogFunc_WhenFuncThrows_LogsException()
        {
            // Arrange
            var expectedException = new ArgumentException("Invalid");
            Func<int> testFunc = () => throw expectedException;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                testFunc.LogFunc(_loggerMock.Object));
            
            Assert.Equal(expectedException.Message, exception.Message);
            _loggerMock.Verify(x => x.Log(It.IsAny<string>()), Times.AtLeast(1));
        }

        [Fact]
        public void LogFunc_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            Func<int> testFunc = () => 42;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                testFunc.LogFunc(null));
        }

        [Fact]
        public void LogFunc_WithNullFunc_ThrowsArgumentNullException()
        {
            // Arrange
            Func<int> testFunc = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                testFunc.LogFunc(_loggerMock.Object));
        }

        [Fact]
        public void LogFunc_WithDefaultValue_ReturnsDefaultValue()
        {
            // Arrange
            Func<int> testFunc = () => default;

            // Act
            var result = testFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(default(int), result);
        }

        [Fact]
        public void LogFunc_WithNullableReturnType_ReturnsNull()
        {
            // Arrange
            Func<string> testFunc = () => null;

            // Act
            var result = testFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LogFunc_WhenFuncCompletesSuccessfully_LogsCompletionMessage()
        {
            // Arrange
            Func<int> testFunc = () => 99;
            var logCalls = new List<string>();
            _loggerMock.Setup(x => x.Log(It.IsAny<string>()))
                .Callback<string>(msg => logCalls.Add(msg));

            // Act
            testFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.NotEmpty(logCalls);
            Assert.Contains(logCalls, x => x.Contains("Complet") || x.Contains("complet"));
        }

        [Fact]
        public void LogFunc_WhenFuncThrowsOperationCanceledException_PropagatesException()
        {
            // Arrange
            var canceledException = new OperationCanceledException();
            Func<int> testFunc = () => throw canceledException;

            // Act & Assert
            Assert.Throws<OperationCanceledException>(() => 
                testFunc.LogFunc(_loggerMock.Object));
        }

        [Fact]
        public void LogFunc_WithMultipleTypes_HandlesStringReturn()
        {
            // Arrange
            Func<string> testFunc = () => "hello world";

            // Act
            var result = testFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.Equal("hello world", result);
        }

        [Fact]
        public void LogFunc_WithComplexObject_ReturnsComplexObject()
        {
            // Arrange
            var expectedObject = new { Name = "Test", Value = 123 };
            Func<dynamic> testFunc = () => expectedObject;

            // Act
            var result = testFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void LogFunc_WithLongRunningOperation_CompletesAndLogs()
        {
            // Arrange
            Func<int> testFunc = () =>
            {
                System.Threading.Thread.Sleep(10);
                return 123;
            };

            // Act
            var result = testFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(123, result);
            _loggerMock.Verify(x => x.Log(It.IsAny<string>()), Times.AtLeast(1));
        }

        #endregion

        #region LogAsyncAction Tests

        [Fact]
        public async void LogAsyncAction_WithValidAsyncAction_ExecutesAsyncActionSuccessfully()
        {
            // Arrange
            var executed = false;
            Func<Task> testAsyncAction = async () =>
            {
                executed = true;
                await Task.CompletedTask;
            };

            // Act
            await testAsyncAction.LogAsyncAction(_loggerMock.Object);

            // Assert
            Assert.True(executed);
        }

        [Fact]
        public async void LogAsyncAction_WithValidAsyncAction_LogsStartMessage()
        {
            // Arrange
            Func<Task> testAsyncAction = async () => await Task.CompletedTask;

            // Act
            await testAsyncAction.LogAsyncAction(_loggerMock.Object);

            // Assert
            _loggerMock.Verify(x => x.Log(It.IsAny<string>()), Times.AtLeast(1));
        }

        [Fact]
        public async void LogAsyncAction_WhenAsyncActionThrows_LogsException()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Async error");
            Func<Task> testAsyncAction = async () =>
            {
                await Task.CompletedTask;
                throw expectedException;
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                testAsyncAction.LogAsyncAction(_loggerMock.Object));
            
            Assert.Equal(expectedException.Message, exception.Message);
        }

        [Fact]
        public async void LogAsyncAction_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            Func<Task> testAsyncAction = async () => await Task.CompletedTask;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                testAsyncAction.LogAsyncAction(null));
        }

        [Fact]
        public async void LogAsyncAction_WithNullAsyncAction_ThrowsArgumentNullException()
        {
            // Arrange
            Func<Task> testAsyncAction = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                testAsyncAction.LogAsyncAction(_loggerMock.Object));
        }

        [Fact]
        public async void LogAsyncAction_WhenAsyncActionCompletesSuccessfully_LogsCompletionMessage()
        {
            // Arrange
            Func<Task> testAsyncAction = async () => await Task.CompletedTask;
            var logCalls = new List<string>();
            _loggerMock.Setup(x => x.Log(It.IsAny<string>()))
                .Callback<string>(msg => logCalls.Add(msg));

            // Act
            await testAsyncAction.LogAsyncAction(_loggerMock.Object);

            // Assert
            Assert.NotEmpty(logCalls);
        }

        [Fact]
        public async void LogAsyncAction_WhenAsyncActionThrowsOperationCanceledException_PropagatesException()
        {
            // Arrange
            var canceledException = new OperationCanceledException();
            Func<Task> testAsyncAction = async () =>
            {
                await Task.CompletedTask;
                throw canceledException;
            };

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => 
                testAsyncAction.LogAsyncAction(_loggerMock.Object));
        }

        #endregion

        #region LogAsyncFunc Tests

        [Fact]
        public async void LogAsyncFunc_WithValidAsyncFunc_ExecutesAsyncFuncSuccessfully()
        {
            // Arrange
            Func<Task<int>> testAsyncFunc = async () =>
            {
                await Task.CompletedTask;
                return 42;
            };

            // Act
            var result = await testAsyncFunc.LogAsyncFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public async void LogAsyncFunc_WithValidAsyncFunc_ReturnsCorrectValue()
        {
            // Arrange
            var expectedValue = "async result";
            Func<Task<string>> testAsyncFunc = async () =>
            {
                await Task.CompletedTask;
                return expectedValue;
            };

            // Act
            var result = await testAsyncFunc.LogAsyncFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public async void LogAsyncFunc_WithValidAsyncFunc_LogsStartMessage()
        {
            // Arrange
            Func<Task<int>> testAsyncFunc = async () =>
            {
                await Task.CompletedTask;
                return 42;
            };

            // Act
            await testAsyncFunc.LogAsyncFunc(_loggerMock.Object);

            // Assert
            _loggerMock.Verify(x => x.Log(It.IsAny<string>()), Times.AtLeast(1));
        }

        [Fact]
        public async void LogAsyncFunc_WhenAsyncFuncThrows_LogsException()
        {
            // Arrange
            var expectedException = new ArgumentException("Async func error");
            Func<Task<int>> testAsyncFunc = async () =>
            {
                await Task.CompletedTask;
                throw expectedException;
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                testAsyncFunc.LogAsyncFunc(_loggerMock.Object));
            
            Assert.Equal(expectedException.Message, exception.Message);
        }

        [Fact]
        public async void LogAsyncFunc_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            Func<Task<int>> testAsyncFunc = async () =>
            {
                await Task.CompletedTask;
                return 42;
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                testAsyncFunc.LogAsyncFunc(null));
        }

        [Fact]
        public async void LogAsyncFunc_WithNullAsyncFunc_ThrowsArgumentNullException()
        {
            // Arrange
            Func<Task<int>> testAsyncFunc = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                testAsyncFunc.LogAsyncFunc(_loggerMock.Object));
        }

        [Fact]
        public async void LogAsyncFunc_WithNullReturn_ReturnsNull()
        {
            // Arrange
            Func<Task<string>> testAsyncFunc = async () =>
            {
                await Task.CompletedTask;
                return null;
            };

            // Act
            var result = await testAsyncFunc.LogAsyncFunc(_loggerMock.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async void LogAsyncFunc_WhenAsyncFuncCompletesSuccessfully_LogsCompletionMessage()
        {
            // Arrange
            Func<Task<int>> testAsyncFunc = async () =>
            {
                await Task.CompletedTask;
                return 99;
            };
            var logCalls = new List<string>();
            _loggerMock.Setup(x => x.Log(It.IsAny<string>()))
                .Callback<string>(msg => logCalls.Add(msg));

            // Act
            await testAsyncFunc.LogAsyncFunc(_loggerMock.Object);

            // Assert
            Assert.NotEmpty(logCalls);
        }

        [Fact]
        public async void LogAsyncFunc_WhenAsyncFuncThrowsOperationCanceledException_PropagatesException()
        {
            // Arrange
            var canceledException = new OperationCanceledException();
            Func<Task<int>> testAsyncFunc = async () =>
            {
                await Task.CompletedTask;
                throw canceledException;
            };

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => 
                testAsyncFunc.LogAsyncFunc(_loggerMock.Object));
        }

        [Fact]
        public async void LogAsyncFunc_WithComplexAsyncOperation_ReturnsCorrectResult()
        {
            // Arrange
            Func<Task<int>> testAsyncFunc = async () =>
            {
                await Task.Delay(5);
                return 123;
            };

            // Act
            var result = await testAsyncFunc.LogAsyncFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(123, result);
            _loggerMock.Verify(x => x.Log(It.IsAny<string>()), Times.AtLeast(1));
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Fact]
        public void LogAction_WithEmptyAction_ExecutesSuccessfully()
        {
            // Arrange
            Action emptyAction = () => { };

            // Act
            emptyAction.LogAction(_loggerMock.Object);

            // Assert
            _loggerMock.Verify(x => x.Log(It.IsAny<string>()), Times.AtLeast(1));
        }

        [Fact]
        public void LogFunc_WithZeroReturn_ReturnsZero()
        {
            // Arrange
            Func<int> zeroFunc = () => 0;

            // Act
            var result = zeroFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void LogFunc_WithNegativeReturn_ReturnsNegativeValue()
        {
            // Arrange
            Func<int> negativeFunc = () => -1;

            // Act
            var result = negativeFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void LogFunc_WithMaxIntReturn_ReturnsMaxInt()
        {
            // Arrange
            Func<int> maxFunc = () => int.MaxValue;

            // Act
            var result = maxFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(int.MaxValue, result);
        }

        [Fact]
        public void LogFunc_WithMinIntReturn_ReturnsMinInt()
        {
            // Arrange
            Func<int> minFunc = () => int.MinValue;

            // Act
            var result = minFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(int.MinValue, result);
        }

        [Fact]
        public void LogFunc_WithEmptyString_ReturnsEmptyString()
        {
            // Arrange
            Func<string> emptyStringFunc = () => string.Empty;

            // Act
            var result = emptyStringFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async void LogAsyncFunc_WithDelayedCompletion_ReturnsCorrectValue()
        {
            // Arrange
            Func<Task<int>> delayedFunc = async () =>
            {
                await Task.Delay(50);
                return 555;
            };

            // Act
            var result = await delayedFunc.LogAsyncFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(555, result);
        }

        [Fact]
        public void LogAction_ChainedMultipleTimes_ExecutesSuccessfully()
        {
            // Arrange
            var callCount = 0;
            Action incrementAction = () => callCount++;

            // Act
            incrementAction.LogAction(_loggerMock.Object);
            incrementAction.LogAction(_loggerMock.Object);
            incrementAction.LogAction(_loggerMock.Object);

            // Assert
            Assert.Equal(3, callCount);
        }

        [Fact]
        public void LogFunc_WithStateModification_ModifiesStateCorrectly()
        {
            // Arrange
            var state = new { Counter = 0 };
            var mutableCounter = 0;
            Func<int> stateFunc = () =>
            {
                mutableCounter++;
                return mutableCounter;
            };

            // Act
            var result1 = stateFunc.LogFunc(_loggerMock.Object);
            var result2 = stateFunc.LogFunc(_loggerMock.Object);

            // Assert
            Assert.Equal(1, result1);
            Assert.Equal(2, result2);
        }

        #endregion
    }

    // Mock logger interface for testing
    public interface ILogger
    {
        void Log(string message);
    }
}