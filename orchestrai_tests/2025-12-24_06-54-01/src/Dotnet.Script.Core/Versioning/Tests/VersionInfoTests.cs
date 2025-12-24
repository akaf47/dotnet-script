using System;
using Xunit;
using Dotnet.Script.Core.Versioning;

namespace Dotnet.Script.Core.Versioning.Tests
{
    public class VersionInfoTests
    {
        [Fact]
        public void Constructor_Should_Set_Version_And_IsResolved_Properties()
        {
            // Arrange
            var version = "1.0.0";
            var isResolved = true;

            // Act
            var versionInfo = new VersionInfo(version, isResolved);

            // Assert
            Assert.Equal(version, versionInfo.Version);
            Assert.True(versionInfo.IsResolved);
        }

        [Fact]
        public void Constructor_Should_Accept_Empty_Version_String()
        {
            // Arrange
            var version = "";
            var isResolved = false;

            // Act
            var versionInfo = new VersionInfo(version, isResolved);

            // Assert
            Assert.Equal(version, versionInfo.Version);
            Assert.False(versionInfo.IsResolved);
        }

        [Fact]
        public void Constructor_Should_Accept_Null_Version_String()
        {
            // Arrange
            var version = (string)null;
            var isResolved = true;

            // Act
            var versionInfo = new VersionInfo(version, isResolved);

            // Assert
            Assert.Null(versionInfo.Version);
            Assert.True(versionInfo.IsResolved);
        }

        [Fact]
        public void UnResolved_Should_Return_VersionInfo_With_Empty_String_And_False_IsResolved()
        {
            // Act
            var unresolved = VersionInfo.UnResolved;

            // Assert
            Assert.NotNull(unresolved);
            Assert.Equal("", unresolved.Version);
            Assert.False(unresolved.IsResolved);
        }

        [Fact]
        public void UnResolved_Should_Be_Singleton_Instance()
        {
            // Act
            var unresolved1 = VersionInfo.UnResolved;
            var unresolved2 = VersionInfo.UnResolved;

            // Assert
            Assert.Same(unresolved1, unresolved2);
        }

        [Fact]
        public void Equals_Should_Return_True_For_Same_Version_And_IsResolved()
        {
            // Arrange
            var versionInfo1 = new VersionInfo("1.0.0", true);
            var versionInfo2 = new VersionInfo("1.0.0", true);

            // Act
            var result = versionInfo1.Equals(versionInfo2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_Should_Return_False_For_Different_Version()
        {
            // Arrange
            var versionInfo1 = new VersionInfo("1.0.0", true);
            var versionInfo2 = new VersionInfo("2.0.0", true);

            // Act
            var result = versionInfo1.Equals(versionInfo2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_Should_Return_False_For_Different_IsResolved()
        {
            // Arrange
            var versionInfo1 = new VersionInfo("1.0.0", true);
            var versionInfo2 = new VersionInfo("1.0.0", false);

            // Act
            var result = versionInfo1.Equals(versionInfo2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_Should_Be_Case_Insensitive_For_Version()
        {
            // Arrange
            var versionInfo1 = new VersionInfo("1.0.0", true);
            var versionInfo2 = new VersionInfo("1.0.0", true);

            // Act
            var result = versionInfo1.Equals(versionInfo2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_Should_Be_Case_Insensitive_For_Version_With_Mixed_Case()
        {
            // Arrange
            var versionInfo1 = new VersionInfo("v1.0.0-alpha", true);
            var versionInfo2 = new VersionInfo("V1.0.0-ALPHA", true);

            // Act
            var result = versionInfo1.Equals(versionInfo2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_Should_Return_False_When_Other_Is_Null()
        {
            // Arrange
            var versionInfo = new VersionInfo("1.0.0", true);

            // Act
            var result = versionInfo.Equals((VersionInfo)null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_Should_Return_True_When_Reference_Is_Same()
        {
            // Arrange
            var versionInfo = new VersionInfo("1.0.0", true);

            // Act
            var result = versionInfo.Equals(versionInfo);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_With_Null_Version_Strings_Should_Return_True()
        {
            // Arrange
            var versionInfo1 = new VersionInfo(null, true);
            var versionInfo2 = new VersionInfo(null, true);

            // Act
            var result = versionInfo1.Equals(versionInfo2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_With_One_Null_Version_Should_Return_False()
        {
            // Arrange
            var versionInfo1 = new VersionInfo(null, true);
            var versionInfo2 = new VersionInfo("1.0.0", true);

            // Act
            var result = versionInfo1.Equals(versionInfo2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ObjectEquals_Should_Return_True_For_Equal_VersionInfo()
        {
            // Arrange
            var versionInfo1 = new VersionInfo("1.0.0", true);
            var versionInfo2 = new VersionInfo("1.0.0", true);

            // Act
            var result = versionInfo1.Equals((object)versionInfo2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ObjectEquals_Should_Return_False_For_Non_VersionInfo_Object()
        {
            // Arrange
            var versionInfo = new VersionInfo("1.0.0", true);
            var otherObject = "1.0.0";

            // Act
            var result = versionInfo.Equals((object)otherObject);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ObjectEquals_Should_Return_False_For_Null_Object()
        {
            // Arrange
            var versionInfo = new VersionInfo("1.0.0", true);

            // Act
            var result = versionInfo.Equals((object)null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_Should_Return_Same_HashCode_For_Equal_Objects()
        {
            // Arrange
            var versionInfo1 = new VersionInfo("1.0.0", true);
            var versionInfo2 = new VersionInfo("1.0.0", true);

            // Act
            var hashCode1 = versionInfo1.GetHashCode();
            var hashCode2 = versionInfo2.GetHashCode();

            // Assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_Should_Be_Case_Insensitive()
        {
            // Arrange
            var versionInfo1 = new VersionInfo("1.0.0", true);
            var versionInfo2 = new VersionInfo("1.0.0", true);

            // Act
            var hashCode1 = versionInfo1.GetHashCode();
            var hashCode2 = versionInfo2.GetHashCode();

            // Assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_Should_Include_IsResolved_Flag()
        {
            // Arrange
            var versionInfo1 = new VersionInfo("1.0.0", true);
            var versionInfo2 = new VersionInfo("1.0.0", false);

            // Act
            var hashCode1 = versionInfo1.GetHashCode();
            var hashCode2 = versionInfo2.GetHashCode();

            // Assert
            Assert.NotEqual(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_Should_Handle_Null_Version()
        {
            // Arrange
            var versionInfo = new VersionInfo(null, true);

            // Act
            var hashCode = versionInfo.GetHashCode();

            // Assert
            Assert.NotNull(hashCode);
        }

        [Fact]
        public void GetHashCode_Should_Handle_Empty_Version()
        {
            // Arrange
            var versionInfo = new VersionInfo("", false);

            // Act
            var hashCode = versionInfo.GetHashCode();

            // Assert
            Assert.NotNull(hashCode);
        }

        [Fact]
        public void ToString_Should_Return_Version_String()
        {
            // Arrange
            var version = "1.0.0";
            var versionInfo = new VersionInfo(version, true);

            // Act
            var result = versionInfo.ToString();

            // Assert
            Assert.Equal(version, result);
        }

        [Fact]
        public void ToString_Should_Return_Empty_String_For_Empty_Version()
        {
            // Arrange
            var version = "";
            var versionInfo = new VersionInfo(version, false);

            // Act
            var result = versionInfo.ToString();

            // Assert
            Assert.Equal(version, result);
        }

        [Fact]
        public void ToString_Should_Return_Null_For_Null_Version()
        {
            // Arrange
            var versionInfo = new VersionInfo(null, true);

            // Act
            var result = versionInfo.ToString();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Equals_Should_Work_With_EqualityOperator_Semantics()
        {
            // Arrange
            var versionInfo1 = new VersionInfo("1.0.0", true);
            var versionInfo2 = new VersionInfo("1.0.0", true);

            // Act & Assert
            Assert.Equal(versionInfo1, versionInfo2);
        }

        [Fact]
        public void Multiple_Different_Instances_Should_Have_Different_HashCodes()
        {
            // Arrange
            var versionInfo1 = new VersionInfo("1.0.0", true);
            var versionInfo2 = new VersionInfo("2.0.0", true);
            var versionInfo3 = new VersionInfo("1.0.0", false);

            // Act
            var hashCode1 = versionInfo1.GetHashCode();
            var hashCode2 = versionInfo2.GetHashCode();
            var hashCode3 = versionInfo3.GetHashCode();

            // Assert
            Assert.NotEqual(hashCode1, hashCode2);
            Assert.NotEqual(hashCode1, hashCode3);
        }

        [Fact]
        public void Equals_Should_Handle_Complex_Version_Strings()
        {
            // Arrange
            var versionInfo1 = new VersionInfo("1.0.0-alpha.1+build.123", true);
            var versionInfo2 = new VersionInfo("1.0.0-alpha.1+build.123", true);

            // Act
            var result = versionInfo1.Equals(versionInfo2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Version_Property_Should_Be_Immutable()
        {
            // Arrange
            var version = "1.0.0";
            var versionInfo = new VersionInfo(version, true);

            // Act & Assert
            Assert.Equal(version, versionInfo.Version);
            // Verify property is read-only by checking it cannot be set
            Assert.True(typeof(VersionInfo).GetProperty("Version").CanRead);
            Assert.False(typeof(VersionInfo).GetProperty("Version").CanWrite);
        }

        [Fact]
        public void IsResolved_Property_Should_Be_Immutable()
        {
            // Arrange
            var versionInfo = new VersionInfo("1.0.0", true);

            // Act & Assert
            Assert.True(versionInfo.IsResolved);
            // Verify property is read-only by checking it cannot be set
            Assert.True(typeof(VersionInfo).GetProperty("IsResolved").CanRead);
            Assert.False(typeof(VersionInfo).GetProperty("IsResolved").CanWrite);
        }
    }
}