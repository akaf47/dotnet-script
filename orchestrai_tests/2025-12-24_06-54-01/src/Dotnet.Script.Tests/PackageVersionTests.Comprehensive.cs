using Dotnet.Script.DependencyModel.ProjectSystem;
using Xunit;

namespace Dotnet.Script.Tests
{
    /// <summary>
    /// Comprehensive tests for PackageVersion class covering all code paths,
    /// branches, edge cases, and error scenarios.
    /// </summary>
    public class PackageVersionComprehensiveTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void Constructor_ShouldInitializeValue_WhenValidVersionPassed()
        {
            // Arrange
            var version = "1.2.3";

            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.Equal(version, packageVersion.Value);
        }

        [Fact]
        public void Constructor_ShouldInitializeValue_WithEmptyString()
        {
            // Arrange
            var version = "";

            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.Equal(version, packageVersion.Value);
        }

        [Fact]
        public void Constructor_ShouldInitializeValue_WithNull()
        {
            // Arrange
            string version = null;

            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.Null(packageVersion.Value);
        }

        #endregion

        #region IsPinned Property Tests - Pinned Versions

        [Theory]
        [InlineData("1.2.3")]
        [InlineData("0.0.0")]
        [InlineData("999.999.999")]
        public void IsPinned_ShouldBeTrue_ForMajorMinorPatchVersions(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.True(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("1.2.3.4")]
        [InlineData("0.0.0.0")]
        [InlineData("999.999.999.999")]
        public void IsPinned_ShouldBeTrue_ForMajorMinorPatchBuildVersions(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.True(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("1.2.3-beta1")]
        [InlineData("1.2.3-alpha")]
        [InlineData("1.2.3-rc1")]
        [InlineData("0.1.4-beta")]
        [InlineData("2.0.0-preview3.20122.2")]
        [InlineData("1.0.0-ci-20180920T1656")]
        public void IsPinned_ShouldBeTrue_ForVersionsWithPrereleaseSuffixes(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.True(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("1.2.3-BETA1")]
        [InlineData("1.2.3-Alpha")]
        [InlineData("1.2.3-RC1")]
        public void IsPinned_ShouldBeTrue_ForPrereleaseVersionsRegardlessOfCase(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.True(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("[1.2]")]
        [InlineData("[1.2.3]")]
        [InlineData("[1.2.3.4]")]
        [InlineData("[0.0.0]")]
        public void IsPinned_ShouldBeTrue_ForBracketedVersions(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.True(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("[1.2.3-beta1]")]
        [InlineData("[1.2.3-alpha]")]
        [InlineData("[0.1.4-beta]")]
        public void IsPinned_ShouldBeTrue_ForBracketedVersionsWithPrerelease(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.True(packageVersion.IsPinned);
        }

        #endregion

        #region IsPinned Property Tests - Unpinned Versions

        [Theory]
        [InlineData("1.0")]
        [InlineData("0.0")]
        [InlineData("999.999")]
        public void IsPinned_ShouldBeFalse_ForMajorMinorOnlyVersions(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.False(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("(1.0,)")]
        [InlineData("(0.0,)")]
        [InlineData("(1.5,)")]
        public void IsPinned_ShouldBeFalse_ForOpenEndedRanges(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.False(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("(,1.0]")]
        [InlineData("(,2.0]")]
        [InlineData("(,0.0]")]
        public void IsPinned_ShouldBeFalse_ForLowerBoundedRanges(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.False(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("[1.0,2.0]")]
        [InlineData("[0.0,1.0]")]
        [InlineData("[1.5,3.5]")]
        public void IsPinned_ShouldBeFalse_ForClosedRanges(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.False(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("(1.0,2.0)")]
        [InlineData("(0.0,1.0)")]
        [InlineData("(1.5,3.5)")]
        public void IsPinned_ShouldBeFalse_ForOpenRanges(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.False(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("[1.0,2.0)")]
        [InlineData("[0.0,1.0)")]
        [InlineData("[1.5,3.5)")]
        public void IsPinned_ShouldBeFalse_ForHalfOpenRanges(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.False(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("(1.0)")]
        [InlineData("(0.0)")]
        [InlineData("(2.5)")]
        public void IsPinned_ShouldBeFalse_ForExactVersionInParentheses(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.False(packageVersion.IsPinned);
        }

        [Fact]
        public void IsPinned_ShouldBeFalse_ForEmptyString()
        {
            // Act
            var packageVersion = new PackageVersion("");

            // Assert
            Assert.False(packageVersion.IsPinned);
        }

        [Fact]
        public void IsPinned_ShouldBeFalse_ForNull()
        {
            // Act
            var packageVersion = new PackageVersion(null);

            // Assert
            Assert.False(packageVersion.IsPinned);
        }

        #endregion

        #region Equals Method Tests

        [Fact]
        public void Equals_ShouldReturnTrue_WhenVersionsAreIdentical()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.3");

            // Act
            var result = version1.Equals(version2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenVersionsAreIdenticalButDifferentCase()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3-BETA1");
            var version2 = new PackageVersion("1.2.3-beta1");

            // Act
            var result = version1.Equals(version2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenSameObjectReferenceCompared()
        {
            // Arrange
            var version = new PackageVersion("1.2.3");

            // Act
            var result = version.Equals(version);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenVersionsAreDifferent()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.4");

            // Act
            var result = version1.Equals(version2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenComparingWithNull()
        {
            // Arrange
            var version = new PackageVersion("1.2.3");
            PackageVersion nullVersion = null;

            // Act
            var result = version.Equals(nullVersion);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ObjectOverride_ShouldReturnTrue_WhenVersionsAreIdentical()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            object version2 = new PackageVersion("1.2.3");

            // Act
            var result = version1.Equals(version2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ObjectOverride_ShouldReturnTrue_WhenVersionsAreIdenticalButDifferentCase()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3-BETA1");
            object version2 = new PackageVersion("1.2.3-beta1");

            // Act
            var result = version1.Equals(version2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ObjectOverride_ShouldReturnFalse_WhenVersionsAreDifferent()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            object version2 = new PackageVersion("1.2.4");

            // Act
            var result = version1.Equals(version2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ObjectOverride_ShouldReturnFalse_WhenComparingWithNull()
        {
            // Arrange
            var version = new PackageVersion("1.2.3");
            object nullVersion = null;

            // Act
            var result = version.Equals(nullVersion);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ObjectOverride_ShouldReturnFalse_WhenComparingWithDifferentType()
        {
            // Arrange
            var version = new PackageVersion("1.2.3");
            object otherType = "1.2.3";

            // Act
            var result = version.Equals(otherType);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ObjectOverride_ShouldReturnFalse_WhenComparingWithInteger()
        {
            // Arrange
            var version = new PackageVersion("1.2.3");
            object otherType = 123;

            // Act
            var result = version.Equals(otherType);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetHashCode Tests

        [Fact]
        public void GetHashCode_ShouldReturnSameValue_ForIdenticalVersions()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.3");

            // Act
            var hash1 = version1.GetHashCode();
            var hash2 = version2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_ShouldReturnSameValue_ForVersionsWithDifferentCase()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3-BETA1");
            var version2 = new PackageVersion("1.2.3-beta1");

            // Act
            var hash1 = version1.GetHashCode();
            var hash2 = version2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_ShouldReturnDifferentValue_ForDifferentVersions()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.4");

            // Act
            var hash1 = version1.GetHashCode();
            var hash2 = version2.GetHashCode();

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_ShouldBeConsistent_WhenCalledMultipleTimes()
        {
            // Arrange
            var version = new PackageVersion("1.2.3-beta1");
            var hash1 = version.GetHashCode();

            // Act
            var hash2 = version.GetHashCode();
            var hash3 = version.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
            Assert.Equal(hash2, hash3);
        }

        [Fact]
        public void GetHashCode_ShouldWork_WithEmptyString()
        {
            // Arrange
            var version = new PackageVersion("");

            // Act
            var hash = version.GetHashCode();

            // Assert
            Assert.IsType<int>(hash);
        }

        [Fact]
        public void GetHashCode_ShouldWork_WithNull()
        {
            // Arrange
            var version = new PackageVersion(null);

            // Act
            var hash = version.GetHashCode();

            // Assert
            Assert.IsType<int>(hash);
        }

        #endregion

        #region IEquatable Implementation Tests

        [Fact]
        public void IEquatable_ShouldBeImplemented()
        {
            // Arrange
            var version = new PackageVersion("1.2.3");

            // Act & Assert
            Assert.IsAssignableFrom<System.Collections.Generic.IEquatable<PackageVersion>>(version);
        }

        [Fact]
        public void EqualsOperator_ShouldWork_WhenUsedInCollections()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.3");
            var version3 = new PackageVersion("1.2.4");
            var versions = new System.Collections.Generic.HashSet<PackageVersion> { version1 };

            // Act
            var containsVersion2 = versions.Contains(version2);
            var containsVersion3 = versions.Contains(version3);

            // Assert
            Assert.True(containsVersion2);
            Assert.False(containsVersion3);
        }

        #endregion

        #region Edge Cases and Special Scenarios

        [Theory]
        [InlineData("1.0.0-0")]
        [InlineData("1.0.0-x.7.z.92")]
        [InlineData("1.0.0-x-y-z")]
        public void IsPinned_ShouldBeTrue_ForValidPrereleaseFormats(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.True(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1.2")]
        [InlineData("1.2.3.4.5")]
        public void IsPinned_ShouldBeFalse_ForInvalidNumberOfVersionSegments(string version)
        {
            // Arrange & Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.False(packageVersion.IsPinned);
        }

        [Fact]
        public void Constructor_ShouldNotThrow_WhenInvalidVersionString()
        {
            // Act & Assert
            var packageVersion = new PackageVersion("invalid-version-string");
            Assert.False(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("[1.2.3]")]
        [InlineData("[1.2.3-beta]")]
        [InlineData("[1.2.3-beta+build]")]
        public void IsPinned_ShouldBeTrue_ForBracketedFormatsWithPrerelease(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.True(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("1.2.3+build")]
        [InlineData("1.2.3+build.info")]
        public void IsPinned_ShouldBeTrue_ForVersionsWithBuildMetadata(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.True(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("1.2.3-beta+build")]
        [InlineData("1.2.3-rc.1+build.123")]
        public void IsPinned_ShouldBeTrue_ForVersionsWithPrereleaseAndBuild(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.True(packageVersion.IsPinned);
        }

        #endregion

        #region Case Sensitivity Tests

        [Theory]
        [InlineData("1.2.3-Beta1", "1.2.3-beta1")]
        [InlineData("1.2.3-BETA1", "1.2.3-beta1")]
        [InlineData("1.2.3-BeTa1", "1.2.3-beta1")]
        [InlineData("1.2.3-ALPHA", "1.2.3-alpha")]
        [InlineData("1.2.3-RC1", "1.2.3-rc1")]
        public void Equals_ShouldBeCaseInsensitive_ForPrereleaseIdentifiers(string version1, string version2)
        {
            // Arrange
            var pv1 = new PackageVersion(version1);
            var pv2 = new PackageVersion(version2);

            // Act
            var result = pv1.Equals(pv2);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("1.2.3-Beta1", "1.2.3-beta1")]
        [InlineData("1.2.3-BETA1", "1.2.3-beta1")]
        [InlineData("1.2.3-BeTa1", "1.2.3-beta1")]
        public void GetHashCode_ShouldBeSame_ForVersionsWithDifferentCases(string version1, string version2)
        {
            // Arrange
            var pv1 = new PackageVersion(version1);
            var pv2 = new PackageVersion(version2);

            // Act
            var hash1 = pv1.GetHashCode();
            var hash2 = pv2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        #endregion

        #region Numeric Pattern Tests

        [Theory]
        [InlineData("0.0.0")]
        [InlineData("0.1.0")]
        [InlineData("1.0.0")]
        [InlineData("10.0.0")]
        [InlineData("100.0.0")]
        [InlineData("1000.0.0")]
        public void IsPinned_ShouldBeTrue_ForZeroLeadingVersionNumbers(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.True(packageVersion.IsPinned);
        }

        [Theory]
        [InlineData("01.0.0")]
        [InlineData("1.02.0")]
        [InlineData("1.0.03")]
        public void IsPinned_ShouldBeFalse_ForVersionNumbersWithLeadingZeros(string version)
        {
            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.False(packageVersion.IsPinned);
        }

        #endregion
    }
}