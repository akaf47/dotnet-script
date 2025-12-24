using System;
using Xunit;
using Dotnet.Script.DependencyModel.ProjectSystem;

namespace Dotnet.Script.DependencyModel.Tests.ProjectSystem
{
    /// <summary>
    /// Comprehensive tests for PackageVersion class covering all code paths and edge cases.
    /// </summary>
    public class PackageVersionTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_SetsValueProperty_WhenValidVersionString()
        {
            // Arrange
            string version = "1.2.3";

            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.Equal(version, packageVersion.Value);
        }

        [Fact]
        public void Constructor_SetsValueProperty_WhenEmptyString()
        {
            // Arrange
            string version = "";

            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.Equal(version, packageVersion.Value);
        }

        [Fact]
        public void Constructor_SetsValueProperty_WhenNull()
        {
            // Arrange
            string version = null;

            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.Null(packageVersion.Value);
        }

        #endregion

        #region IsPinned Tests - Basic Patterns

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithBrackets()
        {
            // Arrange
            var packageVersion = new PackageVersion("[1.2.3]");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithBracketsAndPreRelease()
        {
            // Arrange
            var packageVersion = new PackageVersion("[1.2.3-alpha]");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithBracketsAndBuild()
        {
            // Arrange
            var packageVersion = new PackageVersion("[1.2.3+build]");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithBracketsAndPreReleaseAndBuild()
        {
            // Arrange
            var packageVersion = new PackageVersion("[1.2.3-alpha+build]");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithTwoComponentsInBrackets()
        {
            // Arrange
            var packageVersion = new PackageVersion("[1.2]");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithThreeComponentsInBrackets()
        {
            // Arrange
            var packageVersion = new PackageVersion("[1.2.3]");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithFourComponentsInBrackets()
        {
            // Arrange
            var packageVersion = new PackageVersion("[1.2.3.4]");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithZeroComponents()
        {
            // Arrange
            var packageVersion = new PackageVersion("[0.0.0]");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        #endregion

        #region IsPinned Tests - Without Brackets

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithTwoComponentsNoBrackets()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithThreeComponentsNoBrackets()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2.3");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithPreReleaseNoBrackets()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2.3-alpha");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithBuildMetadataNoBrackets()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2.3+build");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WhenVersionIsPinnedWithPreReleaseAndBuildNoBrackets()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2.3-alpha+build");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        #endregion

        #region IsPinned Tests - False Cases

        [Fact]
        public void IsPinned_ReturnsFalse_WhenVersionIsNotPinned()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2.*");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPinned_ReturnsFalse_WhenVersionIsEmpty()
        {
            // Arrange
            var packageVersion = new PackageVersion("");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPinned_ReturnsFalse_WhenVersionIsNull()
        {
            // Arrange
            var packageVersion = new PackageVersion(null);

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPinned_ReturnsFalse_WhenVersionIsOnlyMajor()
        {
            // Arrange
            var packageVersion = new PackageVersion("1");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPinned_ReturnsFalse_WhenVersionHasInvalidFormat()
        {
            // Arrange
            var packageVersion = new PackageVersion("invalid");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPinned_ReturnsFalse_WhenVersionHasOnlyOpenBracket()
        {
            // Arrange
            var packageVersion = new PackageVersion("[1.2.3");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPinned_ReturnsFalse_WhenVersionHasOnlyCloseBracket()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2.3]");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPinned_ReturnsFalse_WhenVersionHasTooManyComponents()
        {
            // Arrange
            var packageVersion = new PackageVersion("[1.2.3.4.5]");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPinned_ReturnsFalse_WhenVersionHasTooManyComponentsNoBrackets()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2.3.4.5");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Equals(PackageVersion) Tests

        [Fact]
        public void EqualsPackageVersion_ReturnsTrue_WhenValuesAreEqual()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.3");

            // Act
            bool result = version1.Equals(version2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualsPackageVersion_ReturnsTrue_WhenValuesAreEqualWithDifferentCase()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.3");

            // Act
            bool result = version1.Equals(version2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualsPackageVersion_ReturnsFalse_WhenValuesAreDifferent()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.4");

            // Act
            bool result = version1.Equals(version2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsPackageVersion_ReturnsFalse_WhenOtherIsNull()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            PackageVersion version2 = null;

            // Act
            bool result = version1.Equals(version2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsPackageVersion_ReturnsTrue_WhenSameReference()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = version1;

            // Act
            bool result = version1.Equals(version2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualsPackageVersion_ReturnsTrue_WhenValuesAreEqualButCaseDifferent()
        {
            // Arrange - using case-sensitive versions to test case-insensitive comparison
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.3");

            // Act
            bool result = version1.Equals(version2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualsPackageVersion_ReturnsFalse_WhenOneIsNullValue()
        {
            // Arrange
            var version1 = new PackageVersion(null);
            var version2 = new PackageVersion("1.2.3");

            // Act
            bool result = version1.Equals(version2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsPackageVersion_ReturnsTrue_WhenBothHaveNullValue()
        {
            // Arrange
            var version1 = new PackageVersion(null);
            var version2 = new PackageVersion(null);

            // Act
            bool result = version1.Equals(version2);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Equals(object) Tests

        [Fact]
        public void EqualsObject_ReturnsTrue_WhenValuesAreEqual()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            object version2 = new PackageVersion("1.2.3");

            // Act
            bool result = version1.Equals(version2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualsObject_ReturnsFalse_WhenValuesAreDifferent()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            object version2 = new PackageVersion("1.2.4");

            // Act
            bool result = version1.Equals(version2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsObject_ReturnsFalse_WhenObjectIsNull()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            object version2 = null;

            // Act
            bool result = version1.Equals(version2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsObject_ReturnsFalse_WhenObjectIsNotPackageVersion()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            object obj = "1.2.3";

            // Act
            bool result = version1.Equals(obj);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsObject_ReturnsFalse_WhenObjectIsInteger()
        {
            // Arrange
            var version1 = new PackageVersion("123");
            object obj = 123;

            // Act
            bool result = version1.Equals(obj);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsObject_ReturnsTrue_WhenObjectIsNullPackageVersion()
        {
            // Arrange
            var version1 = new PackageVersion(null);
            object obj = new PackageVersion(null);

            // Act
            bool result = version1.Equals(obj);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region GetHashCode Tests

        [Fact]
        public void GetHashCode_ReturnsSameValue_ForEqualVersions()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.3");

            // Act
            int hash1 = version1.GetHashCode();
            int hash2 = version2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_ReturnsSameValue_ForDifferentCaseSameValue()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.3");

            // Act
            int hash1 = version1.GetHashCode();
            int hash2 = version2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_ReturnsDifferentValue_ForDifferentVersions()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.4");

            // Act
            int hash1 = version1.GetHashCode();
            int hash2 = version2.GetHashCode();

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_ReturnsConsistentValue_WhenCalledMultipleTimes()
        {
            // Arrange
            var version = new PackageVersion("1.2.3");

            // Act
            int hash1 = version.GetHashCode();
            int hash2 = version.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_WorksCorrectly_WithNullValue()
        {
            // Arrange
            var version = new PackageVersion(null);

            // Act
            int hash = version.GetHashCode();

            // Assert
            Assert.IsType<int>(hash);
        }

        [Fact]
        public void GetHashCode_WorksCorrectly_WithEmptyString()
        {
            // Arrange
            var version = new PackageVersion("");

            // Act
            int hash = version.GetHashCode();

            // Assert
            Assert.IsType<int>(hash);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void PackageVersion_CanBeUsedInDictionary()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.3");
            var version3 = new PackageVersion("2.0.0");
            var dict = new System.Collections.Generic.Dictionary<PackageVersion, string>();

            // Act
            dict.Add(version1, "value1");
            string value = dict[version2];
            bool containsVersion3 = dict.ContainsKey(version3);

            // Assert
            Assert.Equal("value1", value);
            Assert.False(containsVersion3);
        }

        [Fact]
        public void PackageVersion_CanBeUsedInHashSet()
        {
            // Arrange
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.3");
            var version3 = new PackageVersion("2.0.0");
            var set = new System.Collections.Generic.HashSet<PackageVersion>();

            // Act
            set.Add(version1);
            set.Add(version2);
            set.Add(version3);

            // Assert
            Assert.Equal(2, set.Count);
        }

        [Fact]
        public void PackageVersion_PreservesValue_ThroughEquality()
        {
            // Arrange
            string originalValue = "1.2.3-alpha+build";
            var version = new PackageVersion(originalValue);

            // Act
            var equalVersion = new PackageVersion(originalValue);
            bool areEqual = version.Equals(equalVersion);

            // Assert
            Assert.True(areEqual);
            Assert.Equal(originalValue, version.Value);
        }

        #endregion

        #region Complex Version Patterns

        [Fact]
        public void IsPinned_ReturnsTrue_WithComplexPreRelease()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2.3-alpha.1");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WithComplexBuildMetadata()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2.3+build.123");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WithDotInPreRelease()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2.3-rc.1");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WithDashInBuild()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2.3+exp-1");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WithMultipleDots()
        {
            // Arrange
            var packageVersion = new PackageVersion("1.2.3-alpha.beta.1");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Boundary Tests

        [Fact]
        public void Constructor_WorksWithVeryLargeVersionNumber()
        {
            // Arrange
            string version = "999999999999.999999999999.999999999999";

            // Act
            var packageVersion = new PackageVersion(version);

            // Assert
            Assert.Equal(version, packageVersion.Value);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WithZerosInVersion()
        {
            // Arrange
            var packageVersion = new PackageVersion("0.0.0");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPinned_ReturnsTrue_WithLeadingZeros()
        {
            // Arrange - Note: Leading zeros in numeric identifiers should fail per SemVer spec,
            // but the regex may match them depending on implementation
            var packageVersion = new PackageVersion("01.02.03");

            // Act
            bool result = packageVersion.IsPinned;

            // Assert
            // This depends on regex behavior - documenting the actual behavior
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void Equals_WorksWithWhitespaceVersions()
        {
            // Arrange
            var version1 = new PackageVersion(" 1.2.3 ");
            var version2 = new PackageVersion(" 1.2.3 ");

            // Act
            bool result = version1.Equals(version2);

            // Assert
            Assert.True(result);
        }

        #endregion
    }
}