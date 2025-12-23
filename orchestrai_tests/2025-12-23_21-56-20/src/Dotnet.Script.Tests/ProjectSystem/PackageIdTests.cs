using Dotnet.Script.DependencyModel.ProjectSystem;
using System;
using Xunit;

namespace Dotnet.Script.Tests.ProjectSystem
{
    /// <summary>
    /// Comprehensive tests for PackageId class covering equality, hashing, and value semantics.
    /// </summary>
    public class PackageIdTests
    {
        [Fact]
        public void Constructor_ShouldSetValue()
        {
            // Arrange
            var testValue = "Newtonsoft.Json";

            // Act
            var packageId = new PackageId(testValue);

            // Assert
            Assert.Equal(testValue, packageId.Value);
        }

        [Fact]
        public void Constructor_ShouldAcceptEmptyString()
        {
            // Arrange
            var testValue = "";

            // Act
            var packageId = new PackageId(testValue);

            // Assert
            Assert.Equal(testValue, packageId.Value);
            Assert.Empty(packageId.Value);
        }

        [Fact]
        public void Constructor_ShouldAcceptNullValue()
        {
            // Arrange
            string testValue = null;

            // Act
            var packageId = new PackageId(testValue);

            // Assert
            Assert.Null(packageId.Value);
        }

        [Fact]
        public void Constructor_ShouldPreserveExactCasing()
        {
            // Arrange
            var testValue = "SoMeWeIrD.CaSiNg";

            // Act
            var packageId = new PackageId(testValue);

            // Assert
            Assert.Equal(testValue, packageId.Value);
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenComparingIdenticalInstances()
        {
            // Arrange
            var packageId1 = new PackageId("Newtonsoft.Json");
            var packageId2 = new PackageId("Newtonsoft.Json");

            // Act
            var result = packageId1.Equals(packageId2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenComparingDifferentCasing()
        {
            // Arrange
            var packageId1 = new PackageId("Newtonsoft.Json");
            var packageId2 = new PackageId("newtonsoft.json");

            // Act
            var result = packageId1.Equals(packageId2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenComparingWithMixedCasing()
        {
            // Arrange
            var packageId1 = new PackageId("NEWTONSOFT.JSON");
            var packageId2 = new PackageId("NewtonSoft.Json");

            // Act
            var result = packageId1.Equals(packageId2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenComparingDifferentValues()
        {
            // Arrange
            var packageId1 = new PackageId("Newtonsoft.Json");
            var packageId2 = new PackageId("System.Collections");

            // Act
            var result = packageId1.Equals(packageId2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenComparingWithNull()
        {
            // Arrange
            var packageId = new PackageId("Newtonsoft.Json");

            // Act
            var result = packageId.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenComparingReferenceToItself()
        {
            // Arrange
            var packageId = new PackageId("Newtonsoft.Json");

            // Act
            var result = packageId.Equals(packageId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenOtherIsNull_UsingReferenceEquals()
        {
            // Arrange
            var packageId = new PackageId("Newtonsoft.Json");
            PackageId other = null;

            // Act
            var result = packageId.Equals(other);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ShouldCheckReferenceEqualityFirst()
        {
            // Arrange
            var packageId = new PackageId("Test");

            // Act
            var result = packageId.Equals(packageId);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("Package1", "Package1", true)]
        [InlineData("Package1", "PACKAGE1", true)]
        [InlineData("package1", "Package1", true)]
        [InlineData("Package1", "Package2", false)]
        [InlineData("", "", true)]
        public void Equals_ShouldFollowOrdinalIgnoreCase(string value1, string value2, bool expected)
        {
            // Arrange
            var packageId1 = new PackageId(value1);
            var packageId2 = new PackageId(value2);

            // Act
            var result = packageId1.Equals(packageId2);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetHashCode_ShouldReturnSameHashCode_ForEqualObjects()
        {
            // Arrange
            var packageId1 = new PackageId("Newtonsoft.Json");
            var packageId2 = new PackageId("Newtonsoft.Json");

            // Act
            var hash1 = packageId1.GetHashCode();
            var hash2 = packageId2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_ShouldReturnSameHashCode_ForDifferentCasing()
        {
            // Arrange
            var packageId1 = new PackageId("Newtonsoft.Json");
            var packageId2 = new PackageId("newtonsoft.json");

            // Act
            var hash1 = packageId1.GetHashCode();
            var hash2 = packageId2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_ShouldUsesOrdinalIgnoreCase()
        {
            // Arrange
            var packageId1 = new PackageId("Test");
            var packageId2 = new PackageId("TEST");

            // Act
            var hash1 = packageId1.GetHashCode();
            var hash2 = packageId2.GetHashCode();

            // Assert
            // Hash codes should be identical due to OrdinalIgnoreCase comparison
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_ShouldReturnDifferentHashes_ForDifferentValues()
        {
            // Arrange
            var packageId1 = new PackageId("Package1");
            var packageId2 = new PackageId("Package2");

            // Act
            var hash1 = packageId1.GetHashCode();
            var hash2 = packageId2.GetHashCode();

            // Assert
            // Different values should (likely) produce different hash codes
            // Note: Hash collisions are possible but unlikely for different strings
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_ShouldReturnIntType()
        {
            // Arrange
            var packageId = new PackageId("Test");

            // Act
            var hashCode = packageId.GetHashCode();

            // Assert
            Assert.IsType<int>(hashCode);
        }

        [Fact]
        public void GetHashCode_ShouldBeConsistent_WhenCalledMultipleTimes()
        {
            // Arrange
            var packageId = new PackageId("Newtonsoft.Json");

            // Act
            var hash1 = packageId.GetHashCode();
            var hash2 = packageId.GetHashCode();
            var hash3 = packageId.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
            Assert.Equal(hash2, hash3);
        }

        [Fact]
        public void Equals_ObjectOverload_ShouldReturnTrue_ForEqualPackageIds()
        {
            // Arrange
            var packageId1 = new PackageId("Newtonsoft.Json");
            var packageId2 = new PackageId("Newtonsoft.Json");

            // Act
            var result = packageId1.Equals((object)packageId2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ObjectOverload_ShouldReturnFalse_ForDifferentPackageIds()
        {
            // Arrange
            var packageId1 = new PackageId("Newtonsoft.Json");
            var packageId2 = new PackageId("System.Collections");

            // Act
            var result = packageId1.Equals((object)packageId2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ObjectOverload_ShouldReturnFalse_WhenObjectIsNull()
        {
            // Arrange
            var packageId = new PackageId("Newtonsoft.Json");
            object nullObject = null;

            // Act
            var result = packageId.Equals(nullObject);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ObjectOverload_ShouldReturnFalse_WhenObjectIsNotPackageId()
        {
            // Arrange
            var packageId = new PackageId("Newtonsoft.Json");
            object notAPackageId = "Newtonsoft.Json";

            // Act
            var result = packageId.Equals(notAPackageId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ObjectOverload_ShouldReturnFalse_WhenObjectIsInteger()
        {
            // Arrange
            var packageId = new PackageId("123");
            object intObject = 123;

            // Act
            var result = packageId.Equals(intObject);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ObjectOverload_ShouldReturnTrue_WhenComparingWithItself()
        {
            // Arrange
            var packageId = new PackageId("Test");

            // Act
            var result = packageId.Equals((object)packageId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ObjectOverload_ShouldUsePackageIdEqualsMethod()
        {
            // Arrange
            var packageId1 = new PackageId("package");
            var packageId2 = new PackageId("PACKAGE");

            // Act
            var result = packageId1.Equals((object)packageId2);

            // Assert
            // Should use case-insensitive comparison via Equals(PackageId)
            Assert.True(result);
        }

        [Fact]
        public void PackageIdComparison_ShouldBeSymmetric()
        {
            // Arrange
            var packageId1 = new PackageId("Test");
            var packageId2 = new PackageId("TEST");

            // Act
            var result1 = packageId1.Equals(packageId2);
            var result2 = packageId2.Equals(packageId1);

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void PackageIdComparison_ShouldBeTransitive()
        {
            // Arrange
            var packageId1 = new PackageId("Test");
            var packageId2 = new PackageId("TEST");
            var packageId3 = new PackageId("test");

            // Act
            var equals1And2 = packageId1.Equals(packageId2);
            var equals2And3 = packageId2.Equals(packageId3);
            var equals1And3 = packageId1.Equals(packageId3);

            // Assert
            if (equals1And2 && equals2And3)
            {
                Assert.True(equals1And3);
            }
        }

        [Fact]
        public void PackageId_CanBeUsedInHashSet()
        {
            // Arrange
            var packageId1 = new PackageId("Newtonsoft.Json");
            var packageId2 = new PackageId("newtonsoft.json");
            var packageId3 = new PackageId("System.Collections");

            // Act
            var set = new System.Collections.Generic.HashSet<PackageId>
            {
                packageId1,
                packageId2,
                packageId3
            };

            // Assert
            // packageId1 and packageId2 should be considered equal due to case-insensitive comparison
            Assert.Equal(2, set.Count);
            Assert.Contains(packageId1, set);
            Assert.Contains(packageId3, set);
        }

        [Fact]
        public void PackageId_CanBeUsedAsDictionaryKey()
        {
            // Arrange
            var packageId1 = new PackageId("Newtonsoft.Json");
            var packageId2 = new PackageId("newtonsoft.json");

            var dict = new System.Collections.Generic.Dictionary<PackageId, string>
            {
                { packageId1, "value1" }
            };

            // Act
            var canAccess = dict.TryGetValue(packageId2, out var value);

            // Assert
            Assert.True(canAccess);
            Assert.Equal("value1", value);
        }

        [Fact]
        public void Value_Property_IsReadOnly()
        {
            // Arrange
            var packageId = new PackageId("Test");

            // Act
            var property = typeof(PackageId).GetProperty(nameof(PackageId.Value));

            // Assert
            Assert.NotNull(property);
            Assert.NotNull(property.GetMethod);
            Assert.Null(property.SetMethod);
        }

        [Fact]
        public void PackageId_ShouldImplementIEquatable()
        {
            // Arrange & Act
            var interfaces = typeof(PackageId).GetInterfaces();

            // Assert
            Assert.Contains(typeof(IEquatable<PackageId>), interfaces);
        }

        [Fact]
        public void Constructor_WithSpecialCharacters()
        {
            // Arrange
            var testValue = "My.Package-Name_123";

            // Act
            var packageId = new PackageId(testValue);

            // Assert
            Assert.Equal(testValue, packageId.Value);
        }

        [Fact]
        public void Equals_WithNullValues_ShouldReturnTrue()
        {
            // Arrange
            var packageId1 = new PackageId(null);
            var packageId2 = new PackageId(null);

            // Act
            var result = packageId1.Equals(packageId2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithOneNullValue_ShouldReturnFalse()
        {
            // Arrange
            var packageId1 = new PackageId(null);
            var packageId2 = new PackageId("Package");

            // Act
            var result = packageId1.Equals(packageId2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_WithNullValue_ShouldNotThrow()
        {
            // Arrange
            var packageId = new PackageId(null);

            // Act
            var exception = Record.Exception(() => packageId.GetHashCode());

            // Assert
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("A")]
        [InlineData("abc")]
        [InlineData("ABC")]
        [InlineData("aBc")]
        public void Equals_ShouldIgnoreCase_WithSingleLetters(string input)
        {
            // Arrange
            var packageId1 = new PackageId(input);
            var packageId2 = new PackageId(input.ToLowerInvariant());

            // Act
            var result = packageId1.Equals(packageId2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void PackageId_WithVeryLongValue()
        {
            // Arrange
            var longValue = new string('x', 10000);
            var packageId1 = new PackageId(longValue);
            var packageId2 = new PackageId(longValue.ToUpperInvariant());

            // Act
            var result = packageId1.Equals(packageId2);

            // Assert
            Assert.True(result);
        }
    }
}