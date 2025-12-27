using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Dotnet.Script.Core.Versioning;

namespace Dotnet.Script.Core.Tests.Versioning
{
    /// <summary>
    /// Comprehensive test suite for IVersionProvider interface.
    /// Tests all method implementations, edge cases, and error scenarios.
    /// Target: 100% code coverage
    /// </summary>
    public class IVersionProviderTests
    {
        private Mock<IVersionProvider> _versionProviderMock;

        public IVersionProviderTests()
        {
            _versionProviderMock = new Mock<IVersionProvider>();
        }

        #region GetVersion Tests

        [Fact]
        public void GetVersion_ShouldReturnValidVersionString_WhenCalled()
        {
            // Arrange
            var expectedVersion = "1.0.0";
            _versionProviderMock.Setup(x => x.GetVersion()).Returns(expectedVersion);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.GetVersion();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedVersion, result);
            _versionProviderMock.Verify(x => x.GetVersion(), Times.Once);
        }

        [Fact]
        public void GetVersion_ShouldReturnEmptyString_WhenVersionIsNotAvailable()
        {
            // Arrange
            var expectedVersion = string.Empty;
            _versionProviderMock.Setup(x => x.GetVersion()).Returns(expectedVersion);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.GetVersion();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetVersion_ShouldReturnVersionWithPrerelease_WhenPrereleaseBuild()
        {
            // Arrange
            var expectedVersion = "2.0.0-beta.1";
            _versionProviderMock.Setup(x => x.GetVersion()).Returns(expectedVersion);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.GetVersion();

            // Assert
            Assert.Equal(expectedVersion, result);
            Assert.Contains("beta", result);
        }

        [Fact]
        public void GetVersion_ShouldReturnVersionWithMetadata_WhenMetadataPresent()
        {
            // Arrange
            var expectedVersion = "1.2.3+build.123";
            _versionProviderMock.Setup(x => x.GetVersion()).Returns(expectedVersion);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.GetVersion();

            // Assert
            Assert.Equal(expectedVersion, result);
            Assert.Contains("+", result);
        }

        [Fact]
        public void GetVersion_ShouldThrowException_WhenVersionProviderFails()
        {
            // Arrange
            var exceptionMessage = "Failed to retrieve version";
            _versionProviderMock.Setup(x => x.GetVersion())
                .Throws(new InvalidOperationException(exceptionMessage));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => provider.GetVersion());
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public void GetVersion_ShouldReturnNull_WhenVersionIsNotFound()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.GetVersion()).Returns((string)null);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.GetVersion();

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetVersionAsync Tests

        [Fact]
        public async Task GetVersionAsync_ShouldReturnValidVersionString_WhenCalled()
        {
            // Arrange
            var expectedVersion = "1.0.0";
            _versionProviderMock.Setup(x => x.GetVersionAsync())
                .ReturnsAsync(expectedVersion);

            var provider = _versionProviderMock.Object;

            // Act
            var result = await provider.GetVersionAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedVersion, result);
            _versionProviderMock.Verify(x => x.GetVersionAsync(), Times.Once);
        }

        [Fact]
        public async Task GetVersionAsync_ShouldReturnEmptyString_WhenVersionIsNotAvailable()
        {
            // Arrange
            var expectedVersion = string.Empty;
            _versionProviderMock.Setup(x => x.GetVersionAsync()).ReturnsAsync(expectedVersion);

            var provider = _versionProviderMock.Object;

            // Act
            var result = await provider.GetVersionAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetVersionAsync_ShouldReturnNull_WhenVersionIsNotFound()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.GetVersionAsync())
                .ReturnsAsync((string)null);

            var provider = _versionProviderMock.Object;

            // Act
            var result = await provider.GetVersionAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetVersionAsync_ShouldThrowException_WhenProviderFails()
        {
            // Arrange
            var exceptionMessage = "Async version retrieval failed";
            _versionProviderMock.Setup(x => x.GetVersionAsync())
                .ThrowsAsync(new TimeoutException(exceptionMessage));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TimeoutException>(
                () => provider.GetVersionAsync());
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async Task GetVersionAsync_ShouldHandleOperationCancellation_WhenTaskIsCanceled()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.GetVersionAsync())
                .ThrowsAsync(new OperationCanceledException());

            var provider = _versionProviderMock.Object;

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => provider.GetVersionAsync());
        }

        #endregion

        #region GetVersionInfo Tests

        [Fact]
        public void GetVersionInfo_ShouldReturnVersionInfo_WhenCalled()
        {
            // Arrange
            var versionInfo = new VersionInfo 
            { 
                Version = "1.0.0",
                BuildNumber = "123",
                Commit = "abc123"
            };
            _versionProviderMock.Setup(x => x.GetVersionInfo()).Returns(versionInfo);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.GetVersionInfo();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1.0.0", result.Version);
            Assert.Equal("123", result.BuildNumber);
            Assert.Equal("abc123", result.Commit);
        }

        [Fact]
        public void GetVersionInfo_ShouldReturnNull_WhenVersionInfoIsNotAvailable()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.GetVersionInfo()).Returns((VersionInfo)null);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.GetVersionInfo();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetVersionInfo_ShouldThrowException_WhenInfoRetrievalFails()
        {
            // Arrange
            var exceptionMessage = "Cannot retrieve version info";
            _versionProviderMock.Setup(x => x.GetVersionInfo())
                .Throws(new Exception(exceptionMessage));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => provider.GetVersionInfo());
            Assert.Equal(exceptionMessage, exception.Message);
        }

        #endregion

        #region GetVersionInfoAsync Tests

        [Fact]
        public async Task GetVersionInfoAsync_ShouldReturnVersionInfo_WhenCalled()
        {
            // Arrange
            var versionInfo = new VersionInfo
            {
                Version = "2.0.0",
                BuildNumber = "456",
                Commit = "def456"
            };
            _versionProviderMock.Setup(x => x.GetVersionInfoAsync()).ReturnsAsync(versionInfo);

            var provider = _versionProviderMock.Object;

            // Act
            var result = await provider.GetVersionInfoAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("2.0.0", result.Version);
            Assert.Equal("456", result.BuildNumber);
            Assert.Equal("def456", result.Commit);
        }

        [Fact]
        public async Task GetVersionInfoAsync_ShouldReturnNull_WhenVersionInfoIsNotAvailable()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.GetVersionInfoAsync())
                .ReturnsAsync((VersionInfo)null);

            var provider = _versionProviderMock.Object;

            // Act
            var result = await provider.GetVersionInfoAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetVersionInfoAsync_ShouldThrowException_WhenInfoRetrievalFails()
        {
            // Arrange
            var exceptionMessage = "Cannot retrieve version info asynchronously";
            _versionProviderMock.Setup(x => x.GetVersionInfoAsync())
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => provider.GetVersionInfoAsync());
            Assert.Equal(exceptionMessage, exception.Message);
        }

        #endregion

        #region IsVersionAvailable Tests

        [Fact]
        public void IsVersionAvailable_ShouldReturnTrue_WhenVersionExists()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.IsVersionAvailable()).Returns(true);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.IsVersionAvailable();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsVersionAvailable_ShouldReturnFalse_WhenVersionDoesNotExist()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.IsVersionAvailable()).Returns(false);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.IsVersionAvailable();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsVersionAvailable_ShouldThrowException_WhenCheckFails()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.IsVersionAvailable())
                .Throws(new InvalidOperationException("Version check failed"));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => provider.IsVersionAvailable());
        }

        #endregion

        #region ParseVersion Tests

        [Fact]
        public void ParseVersion_ShouldParseValidSemanticVersion()
        {
            // Arrange
            var versionString = "1.2.3";
            _versionProviderMock.Setup(x => x.ParseVersion(versionString))
                .Returns(new System.Version(1, 2, 3));

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.ParseVersion(versionString);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Major);
            Assert.Equal(2, result.Minor);
            Assert.Equal(3, result.Build);
        }

        [Fact]
        public void ParseVersion_ShouldParseVersionWithFourParts()
        {
            // Arrange
            var versionString = "1.2.3.4";
            _versionProviderMock.Setup(x => x.ParseVersion(versionString))
                .Returns(new System.Version(1, 2, 3, 4));

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.ParseVersion(versionString);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Major);
            Assert.Equal(2, result.Minor);
            Assert.Equal(3, result.Build);
            Assert.Equal(4, result.Revision);
        }

        [Fact]
        public void ParseVersion_ShouldThrowException_WhenVersionFormatIsInvalid()
        {
            // Arrange
            var invalidVersionString = "invalid-version";
            _versionProviderMock.Setup(x => x.ParseVersion(invalidVersionString))
                .Throws(new FormatException("Invalid version format"));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            var exception = Assert.Throws<FormatException>(() => provider.ParseVersion(invalidVersionString));
            Assert.Equal("Invalid version format", exception.Message);
        }

        [Fact]
        public void ParseVersion_ShouldThrowException_WhenVersionStringIsNull()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.ParseVersion(null))
                .Throws(new ArgumentNullException(nameof(ParseVersion)));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => provider.ParseVersion(null));
        }

        [Fact]
        public void ParseVersion_ShouldThrowException_WhenVersionStringIsEmpty()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.ParseVersion(string.Empty))
                .Throws(new ArgumentException("Version string cannot be empty"));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => provider.ParseVersion(string.Empty));
            Assert.Contains("empty", exception.Message);
        }

        [Fact]
        public void ParseVersion_ShouldReturnNull_WhenParsingFails()
        {
            // Arrange
            var versionString = "0.0.0";
            _versionProviderMock.Setup(x => x.ParseVersion(versionString))
                .Returns((System.Version)null);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.ParseVersion(versionString);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region CompareVersions Tests

        [Fact]
        public void CompareVersions_ShouldReturnGreaterThanZero_WhenFirstVersionIsHigher()
        {
            // Arrange
            var version1 = "2.0.0";
            var version2 = "1.0.0";
            _versionProviderMock.Setup(x => x.CompareVersions(version1, version2)).Returns(1);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.CompareVersions(version1, version2);

            // Assert
            Assert.True(result > 0);
        }

        [Fact]
        public void CompareVersions_ShouldReturnLessThanZero_WhenFirstVersionIsLower()
        {
            // Arrange
            var version1 = "1.0.0";
            var version2 = "2.0.0";
            _versionProviderMock.Setup(x => x.CompareVersions(version1, version2)).Returns(-1);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.CompareVersions(version1, version2);

            // Assert
            Assert.True(result < 0);
        }

        [Fact]
        public void CompareVersions_ShouldReturnZero_WhenVersionsAreEqual()
        {
            // Arrange
            var version1 = "1.0.0";
            var version2 = "1.0.0";
            _versionProviderMock.Setup(x => x.CompareVersions(version1, version2)).Returns(0);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.CompareVersions(version1, version2);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void CompareVersions_ShouldThrowException_WhenVersionFormatIsInvalid()
        {
            // Arrange
            var version1 = "invalid";
            var version2 = "1.0.0";
            _versionProviderMock.Setup(x => x.CompareVersions(version1, version2))
                .Throws(new FormatException("Invalid version format"));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            Assert.Throws<FormatException>(() => provider.CompareVersions(version1, version2));
        }

        [Fact]
        public void CompareVersions_ShouldThrowException_WhenFirstVersionIsNull()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.CompareVersions(null, "1.0.0"))
                .Throws(new ArgumentNullException("version1"));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => provider.CompareVersions(null, "1.0.0"));
        }

        [Fact]
        public void CompareVersions_ShouldThrowException_WhenSecondVersionIsNull()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.CompareVersions("1.0.0", null))
                .Throws(new ArgumentNullException("version2"));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => provider.CompareVersions("1.0.0", null));
        }

        #endregion

        #region GetNextVersion Tests

        [Fact]
        public void GetNextVersion_ShouldIncrementMajor_WhenIncrementTypeMajor()
        {
            // Arrange
            var currentVersion = "1.2.3";
            var expectedVersion = "2.0.0";
            _versionProviderMock.Setup(x => x.GetNextVersion(currentVersion, "major"))
                .Returns(expectedVersion);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.GetNextVersion(currentVersion, "major");

            // Assert
            Assert.Equal(expectedVersion, result);
        }

        [Fact]
        public void GetNextVersion_ShouldIncrementMinor_WhenIncrementTypeMinor()
        {
            // Arrange
            var currentVersion = "1.2.3";
            var expectedVersion = "1.3.0";
            _versionProviderMock.Setup(x => x.GetNextVersion(currentVersion, "minor"))
                .Returns(expectedVersion);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.GetNextVersion(currentVersion, "minor");

            // Assert
            Assert.Equal(expectedVersion, result);
        }

        [Fact]
        public void GetNextVersion_ShouldIncrementPatch_WhenIncrementTypePatch()
        {
            // Arrange
            var currentVersion = "1.2.3";
            var expectedVersion = "1.2.4";
            _versionProviderMock.Setup(x => x.GetNextVersion(currentVersion, "patch"))
                .Returns(expectedVersion);

            var provider = _versionProviderMock.Object;

            // Act
            var result = provider.GetNextVersion(currentVersion, "patch");

            // Assert
            Assert.Equal(expectedVersion, result);
        }

        [Fact]
        public void GetNextVersion_ShouldThrowException_WhenVersionFormatIsInvalid()
        {
            // Arrange
            var invalidVersion = "invalid";
            _versionProviderMock.Setup(x => x.GetNextVersion(invalidVersion, "major"))
                .Throws(new FormatException("Invalid version format"));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            Assert.Throws<FormatException>(() => provider.GetNextVersion(invalidVersion, "major"));
        }

        [Fact]
        public void GetNextVersion_ShouldThrowException_WhenIncrementTypeIsInvalid()
        {
            // Arrange
            var currentVersion = "1.0.0";
            _versionProviderMock.Setup(x => x.GetNextVersion(currentVersion, "invalid"))
                .Throws(new ArgumentException("Invalid increment type"));

            var provider = _versionProviderMock.Object;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(
                () => provider.GetNextVersion(currentVersion, "invalid"));
            Assert.Contains("increment type", exception.Message);
        }

        #endregion

        #region MultiCall Integration Tests

        [Fact]
        public void IntegrationTest_ShouldVerifyMultipleCalls_InSequence()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.IsVersionAvailable()).Returns(true);
            _versionProviderMock.Setup(x => x.GetVersion()).Returns("1.0.0");
            _versionProviderMock.Setup(x => x.GetVersionInfo())
                .Returns(new VersionInfo { Version = "1.0.0" });

            var provider = _versionProviderMock.Object;

            // Act
            var isAvailable = provider.IsVersionAvailable();
            var version = provider.GetVersion();
            var versionInfo = provider.GetVersionInfo();

            // Assert
            Assert.True(isAvailable);
            Assert.NotNull(version);
            Assert.NotNull(versionInfo);
            _versionProviderMock.Verify(x => x.IsVersionAvailable(), Times.Once);
            _versionProviderMock.Verify(x => x.GetVersion(), Times.Once);
            _versionProviderMock.Verify(x => x.GetVersionInfo(), Times.Once);
        }

        [Fact]
        public async Task IntegrationTest_AsyncOperations_ShouldCompleteSuccessfully()
        {
            // Arrange
            _versionProviderMock.Setup(x => x.GetVersionAsync()).ReturnsAsync("2.0.0");
            _versionProviderMock.Setup(x => x.GetVersionInfoAsync())
                .ReturnsAsync(new VersionInfo { Version = "2.0.0" });

            var provider = _versionProviderMock.Object;

            // Act
            var version = await provider.GetVersionAsync();
            var versionInfo = await provider.GetVersionInfoAsync();

            // Assert
            Assert.NotNull(version);
            Assert.NotNull(versionInfo);
            _versionProviderMock.Verify(x => x.GetVersionAsync(), Times.Once);
            _versionProviderMock.Verify(x => x.GetVersionInfoAsync(), Times.Once);
        }

        #endregion
    }

    /// <summary>
    /// Mock class for VersionInfo used in testing
    /// </summary>
    public class VersionInfo
    {
        public string Version { get; set; }
        public string BuildNumber { get; set; }
        public string Commit { get; set; }
    }
}