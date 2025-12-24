using System;
using System.Threading.Tasks;
using Dotnet.Script.Core.Versioning;
using Dotnet.Script.DependencyModel.Logging;
using Moq;
using Xunit;

namespace Dotnet.Script.Core.Tests.Versioning
{
    public class LoggedVersionProviderTests
    {
        private readonly Mock<IVersionProvider> _mockVersionProvider;
        private readonly Mock<LogFactory> _mockLogFactory;
        private readonly Mock<Logger> _mockLogger;
        private LoggedVersionProvider _sut;

        public LoggedVersionProviderTests()
        {
            _mockVersionProvider = new Mock<IVersionProvider>();
            _mockLogFactory = new Mock<LogFactory>();
            _mockLogger = new Mock<Logger>();
            
            _mockLogFactory
                .Setup(lf => lf(It.IsAny<Type>()))
                .Returns(_mockLogger.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithVersionProviderAndLogFactory_ShouldInitializeSuccessfully()
        {
            // Arrange & Act
            var provider = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Assert
            Assert.NotNull(provider);
            _mockLogFactory.Verify(lf => lf(typeof(LoggedVersionProvider)), Times.Once);
        }

        [Fact]
        public void Constructor_WithNullVersionProvider_ShouldThrowArgumentNullException()
        {
            // Arrange
            IVersionProvider nullProvider = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => 
                new LoggedVersionProvider(nullProvider, _mockLogFactory.Object));
        }

        [Fact]
        public void Constructor_WithNullLogFactory_ShouldThrowArgumentNullException()
        {
            // Arrange
            LogFactory nullFactory = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => 
                new LoggedVersionProvider(_mockVersionProvider.Object, nullFactory));
        }

        [Fact]
        public void Constructor_WithLogFactoryOnly_ShouldInitializeWithDefaultVersionProvider()
        {
            // Arrange & Act
            var provider = new LoggedVersionProvider(_mockLogFactory.Object);

            // Assert
            Assert.NotNull(provider);
            _mockLogFactory.Verify(lf => lf(typeof(LoggedVersionProvider)), Times.Once);
        }

        [Fact]
        public void Constructor_WithNullLogFactoryOnly_ShouldThrowArgumentNullException()
        {
            // Arrange
            LogFactory nullFactory = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => 
                new LoggedVersionProvider(nullFactory));
        }

        #endregion

        #region GetCurrentVersion Tests

        [Fact]
        public void GetCurrentVersion_WhenProviderReturnsValidVersion_ShouldReturnThatVersion()
        {
            // Arrange
            var expectedVersion = new VersionInfo("1.2.3", isResolved: true);
            _mockVersionProvider.Setup(vp => vp.GetCurrentVersion()).Returns(expectedVersion);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = _sut.GetCurrentVersion();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedVersion.Version, result.Version);
            Assert.Equal(expectedVersion.IsResolved, result.IsResolved);
            _mockVersionProvider.Verify(vp => vp.GetCurrentVersion(), Times.Once);
            _mockLogger.Verify(l => l(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public void GetCurrentVersion_WhenProviderReturnsUnresolvedVersion_ShouldReturnThatVersion()
        {
            // Arrange
            var unresolvedVersion = new VersionInfo(string.Empty, isResolved: false);
            _mockVersionProvider.Setup(vp => vp.GetCurrentVersion()).Returns(unresolvedVersion);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = _sut.GetCurrentVersion();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(unresolvedVersion.Version, result.Version);
            Assert.Equal(unresolvedVersion.IsResolved, result.IsResolved);
            _mockVersionProvider.Verify(vp => vp.GetCurrentVersion(), Times.Once);
        }

        [Fact]
        public void GetCurrentVersion_WhenProviderThrowsException_ShouldReturnUnResolvedVersion()
        {
            // Arrange
            var testException = new InvalidOperationException("Test error");
            _mockVersionProvider.Setup(vp => vp.GetCurrentVersion()).Throws(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = _sut.GetCurrentVersion();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(VersionInfo.UnResolved.Version, result.Version);
            Assert.Equal(VersionInfo.UnResolved.IsResolved, result.IsResolved);
            _mockVersionProvider.Verify(vp => vp.GetCurrentVersion(), Times.Once);
        }

        [Fact]
        public void GetCurrentVersion_WhenProviderThrowsException_ShouldLogError()
        {
            // Arrange
            var testException = new InvalidOperationException("Test error message");
            _mockVersionProvider.Setup(vp => vp.GetCurrentVersion()).Throws(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            _ = _sut.GetCurrentVersion();

            // Assert
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the current version",
                    testException),
                Times.Once);
        }

        [Fact]
        public void GetCurrentVersion_WhenProviderThrowsNullReferenceException_ShouldCatchAndLogError()
        {
            // Arrange
            var testException = new NullReferenceException("Null reference occurred");
            _mockVersionProvider.Setup(vp => vp.GetCurrentVersion()).Throws(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = _sut.GetCurrentVersion();

            // Assert
            Assert.Equal(VersionInfo.UnResolved.Version, result.Version);
            Assert.Equal(VersionInfo.UnResolved.IsResolved, result.IsResolved);
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the current version",
                    testException),
                Times.Once);
        }

        [Fact]
        public void GetCurrentVersion_WhenProviderThrowsAggregateException_ShouldCatchAndLogError()
        {
            // Arrange
            var innerException = new InvalidOperationException("Inner error");
            var testException = new AggregateException("Aggregate error", innerException);
            _mockVersionProvider.Setup(vp => vp.GetCurrentVersion()).Throws(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = _sut.GetCurrentVersion();

            // Assert
            Assert.Equal(VersionInfo.UnResolved.Version, result.Version);
            Assert.False(result.IsResolved);
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the current version",
                    testException),
                Times.Once);
        }

        [Fact]
        public void GetCurrentVersion_WhenProviderThrowsMultipleTimes_ShouldHandleEachCall()
        {
            // Arrange
            var testException = new InvalidOperationException("Test error");
            _mockVersionProvider.Setup(vp => vp.GetCurrentVersion()).Throws(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result1 = _sut.GetCurrentVersion();
            var result2 = _sut.GetCurrentVersion();
            var result3 = _sut.GetCurrentVersion();

            // Assert
            Assert.Equal(VersionInfo.UnResolved.Version, result1.Version);
            Assert.Equal(VersionInfo.UnResolved.Version, result2.Version);
            Assert.Equal(VersionInfo.UnResolved.Version, result3.Version);
            _mockVersionProvider.Verify(vp => vp.GetCurrentVersion(), Times.Exactly(3));
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the current version",
                    It.IsAny<Exception>()),
                Times.Exactly(3));
        }

        [Fact]
        public void GetCurrentVersion_WhenProviderThrowsOutOfMemoryException_ShouldCatchAndLogError()
        {
            // Arrange
            var testException = new OutOfMemoryException("Out of memory");
            _mockVersionProvider.Setup(vp => vp.GetCurrentVersion()).Throws(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = _sut.GetCurrentVersion();

            // Assert
            Assert.Equal(VersionInfo.UnResolved.Version, result.Version);
            Assert.False(result.IsResolved);
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the current version",
                    testException),
                Times.Once);
        }

        #endregion

        #region GetLatestVersion Tests

        [Fact]
        public async Task GetLatestVersion_WhenProviderReturnsValidVersion_ShouldReturnThatVersion()
        {
            // Arrange
            var expectedVersion = new VersionInfo("2.0.0", isResolved: true);
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ReturnsAsync(expectedVersion);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = await _sut.GetLatestVersion();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedVersion.Version, result.Version);
            Assert.Equal(expectedVersion.IsResolved, result.IsResolved);
            _mockVersionProvider.Verify(vp => vp.GetLatestVersion(), Times.Once);
            _mockLogger.Verify(l => l(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task GetLatestVersion_WhenProviderReturnsUnresolvedVersion_ShouldReturnThatVersion()
        {
            // Arrange
            var unresolvedVersion = new VersionInfo(string.Empty, isResolved: false);
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ReturnsAsync(unresolvedVersion);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = await _sut.GetLatestVersion();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(unresolvedVersion.Version, result.Version);
            Assert.Equal(unresolvedVersion.IsResolved, result.IsResolved);
            _mockVersionProvider.Verify(vp => vp.GetLatestVersion(), Times.Once);
        }

        [Fact]
        public async Task GetLatestVersion_WhenProviderThrowsException_ShouldReturnUnResolvedVersion()
        {
            // Arrange
            var testException = new InvalidOperationException("Test error");
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ThrowsAsync(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = await _sut.GetLatestVersion();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(VersionInfo.UnResolved.Version, result.Version);
            Assert.Equal(VersionInfo.UnResolved.IsResolved, result.IsResolved);
            _mockVersionProvider.Verify(vp => vp.GetLatestVersion(), Times.Once);
        }

        [Fact]
        public async Task GetLatestVersion_WhenProviderThrowsException_ShouldLogError()
        {
            // Arrange
            var testException = new InvalidOperationException("Test error message");
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ThrowsAsync(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            _ = await _sut.GetLatestVersion();

            // Assert
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the latest version",
                    testException),
                Times.Once);
        }

        [Fact]
        public async Task GetLatestVersion_WhenProviderThrowsHttpRequestException_ShouldCatchAndLogError()
        {
            // Arrange
            var testException = new System.Net.Http.HttpRequestException("Network error");
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ThrowsAsync(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = await _sut.GetLatestVersion();

            // Assert
            Assert.Equal(VersionInfo.UnResolved.Version, result.Version);
            Assert.False(result.IsResolved);
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the latest version",
                    testException),
                Times.Once);
        }

        [Fact]
        public async Task GetLatestVersion_WhenProviderThrowsTimeoutException_ShouldCatchAndLogError()
        {
            // Arrange
            var testException = new TimeoutException("Request timeout");
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ThrowsAsync(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = await _sut.GetLatestVersion();

            // Assert
            Assert.Equal(VersionInfo.UnResolved.Version, result.Version);
            Assert.False(result.IsResolved);
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the latest version",
                    testException),
                Times.Once);
        }

        [Fact]
        public async Task GetLatestVersion_WhenProviderThrowsAggregateException_ShouldCatchAndLogError()
        {
            // Arrange
            var innerException = new InvalidOperationException("Inner error");
            var testException = new AggregateException("Aggregate error", innerException);
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ThrowsAsync(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = await _sut.GetLatestVersion();

            // Assert
            Assert.Equal(VersionInfo.UnResolved.Version, result.Version);
            Assert.False(result.IsResolved);
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the latest version",
                    testException),
                Times.Once);
        }

        [Fact]
        public async Task GetLatestVersion_WhenProviderThrowsTaskCanceledException_ShouldCatchAndLogError()
        {
            // Arrange
            var testException = new TaskCanceledException("Task cancelled");
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ThrowsAsync(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = await _sut.GetLatestVersion();

            // Assert
            Assert.Equal(VersionInfo.UnResolved.Version, result.Version);
            Assert.False(result.IsResolved);
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the latest version",
                    testException),
                Times.Once);
        }

        [Fact]
        public async Task GetLatestVersion_WhenProviderThrowsMultipleTimes_ShouldHandleEachCall()
        {
            // Arrange
            var testException = new InvalidOperationException("Test error");
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ThrowsAsync(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result1 = await _sut.GetLatestVersion();
            var result2 = await _sut.GetLatestVersion();
            var result3 = await _sut.GetLatestVersion();

            // Assert
            Assert.Equal(VersionInfo.UnResolved.Version, result1.Version);
            Assert.Equal(VersionInfo.UnResolved.Version, result2.Version);
            Assert.Equal(VersionInfo.UnResolved.Version, result3.Version);
            _mockVersionProvider.Verify(vp => vp.GetLatestVersion(), Times.Exactly(3));
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the latest version",
                    It.IsAny<Exception>()),
                Times.Exactly(3));
        }

        [Fact]
        public async Task GetLatestVersion_WhenProviderThrowsNullReferenceException_ShouldCatchAndLogError()
        {
            // Arrange
            var testException = new NullReferenceException("Null reference");
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ThrowsAsync(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = await _sut.GetLatestVersion();

            // Assert
            Assert.Equal(VersionInfo.UnResolved.Version, result.Version);
            Assert.False(result.IsResolved);
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the latest version",
                    testException),
                Times.Once);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void GetCurrentVersion_AndGetLatestVersion_ShouldWorkIndependently()
        {
            // Arrange
            var currentVersion = new VersionInfo("1.0.0", isResolved: true);
            var latestVersion = new VersionInfo("2.0.0", isResolved: true);
            _mockVersionProvider.Setup(vp => vp.GetCurrentVersion()).Returns(currentVersion);
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ReturnsAsync(latestVersion);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var currentResult = _sut.GetCurrentVersion();
            var latestTask = _sut.GetLatestVersion();

            // Assert
            Assert.Equal(currentVersion.Version, currentResult.Version);
            Assert.Equal(latestVersion.Version, latestTask.Result.Version);
            _mockVersionProvider.Verify(vp => vp.GetCurrentVersion(), Times.Once);
            _mockVersionProvider.Verify(vp => vp.GetLatestVersion(), Times.Once);
        }

        [Fact]
        public void GetCurrentVersion_WhenCalledMultipleTimes_ShouldCallProviderEachTime()
        {
            // Arrange
            var version1 = new VersionInfo("1.0.0", isResolved: true);
            var version2 = new VersionInfo("1.0.1", isResolved: true);
            _mockVersionProvider.SetupSequence(vp => vp.GetCurrentVersion())
                .Returns(version1)
                .Returns(version2)
                .Returns(version1);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result1 = _sut.GetCurrentVersion();
            var result2 = _sut.GetCurrentVersion();
            var result3 = _sut.GetCurrentVersion();

            // Assert
            Assert.Equal(version1.Version, result1.Version);
            Assert.Equal(version2.Version, result2.Version);
            Assert.Equal(version1.Version, result3.Version);
            _mockVersionProvider.Verify(vp => vp.GetCurrentVersion(), Times.Exactly(3));
        }

        [Fact]
        public async Task GetLatestVersion_WhenCalledMultipleTimes_ShouldCallProviderEachTime()
        {
            // Arrange
            var version1 = new VersionInfo("2.0.0", isResolved: true);
            var version2 = new VersionInfo("2.1.0", isResolved: true);
            _mockVersionProvider.SetupSequence(vp => vp.GetLatestVersion())
                .ReturnsAsync(version1)
                .ReturnsAsync(version2)
                .ReturnsAsync(version1);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result1 = await _sut.GetLatestVersion();
            var result2 = await _sut.GetLatestVersion();
            var result3 = await _sut.GetLatestVersion();

            // Assert
            Assert.Equal(version1.Version, result1.Version);
            Assert.Equal(version2.Version, result2.Version);
            Assert.Equal(version1.Version, result3.Version);
            _mockVersionProvider.Verify(vp => vp.GetLatestVersion(), Times.Exactly(3));
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void GetCurrentVersion_WhenVersionIsNull_ShouldReturnUnResolvedVersion()
        {
            // Arrange
            _mockVersionProvider.Setup(vp => vp.GetCurrentVersion()).Returns((VersionInfo)null);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = _sut.GetCurrentVersion();

            // Assert
            Assert.Null(result);
            _mockVersionProvider.Verify(vp => vp.GetCurrentVersion(), Times.Once);
        }

        [Fact]
        public async Task GetLatestVersion_WhenVersionIsNull_ShouldReturnNullOrHandleGracefully()
        {
            // Arrange
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ReturnsAsync((VersionInfo)null);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = await _sut.GetLatestVersion();

            // Assert
            Assert.Null(result);
            _mockVersionProvider.Verify(vp => vp.GetLatestVersion(), Times.Once);
        }

        [Fact]
        public void GetCurrentVersion_WhenExceptionMessageIsEmpty_ShouldLogError()
        {
            // Arrange
            var testException = new InvalidOperationException(string.Empty);
            _mockVersionProvider.Setup(vp => vp.GetCurrentVersion()).Throws(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = _sut.GetCurrentVersion();

            // Assert
            Assert.Equal(VersionInfo.UnResolved.Version, result.Version);
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the current version",
                    testException),
                Times.Once);
        }

        [Fact]
        public async Task GetLatestVersion_WhenExceptionMessageIsEmpty_ShouldLogError()
        {
            // Arrange
            var testException = new InvalidOperationException(string.Empty);
            _mockVersionProvider.Setup(vp => vp.GetLatestVersion()).ThrowsAsync(testException);
            _sut = new LoggedVersionProvider(_mockVersionProvider.Object, _mockLogFactory.Object);

            // Act
            var result = await _sut.GetLatestVersion();

            // Assert
            Assert.Equal(VersionInfo.UnResolved.Version, result.Version);
            _mockLogger.Verify(
                l => l(
                    LogLevel.Error,
                    "Failed to retrieve information about the latest version",
                    testException),
                Times.Once);
        }

        #endregion
    }
}