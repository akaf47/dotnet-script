using System.Collections.Generic;
using Xunit;

namespace Dotnet.Script.DependencyModel.ProjectSystem.Tests
{
    public class ParseResultTests
    {
        [Fact]
        public void Constructor_WithValidPackageReferences_SetsPackageReferencesProperty()
        {
            // Arrange
            var packageReferences = new List<PackageReference>
            {
                new PackageReference("TestPackage", "1.0.0")
            };

            // Act
            var result = new ParseResult(packageReferences);

            // Assert
            Assert.Same(packageReferences, result.PackageReferences);
        }

        [Fact]
        public void Constructor_WithEmptyPackageReferences_SetsEmptyCollection()
        {
            // Arrange
            var packageReferences = new List<PackageReference>();

            // Act
            var result = new ParseResult(packageReferences);

            // Assert
            Assert.Empty(result.PackageReferences);
        }

        [Fact]
        public void Constructor_WithMultiplePackageReferences_SetsAllReferences()
        {
            // Arrange
            var packageReferences = new List<PackageReference>
            {
                new PackageReference("Package1", "1.0.0"),
                new PackageReference("Package2", "2.0.0"),
                new PackageReference("Package3", "3.0.0")
            };

            // Act
            var result = new ParseResult(packageReferences);

            // Assert
            Assert.Equal(3, result.PackageReferences.Count);
            Assert.Same(packageReferences, result.PackageReferences);
        }

        [Fact]
        public void Constructor_WithNullPackageReferences_StoresNullReference()
        {
            // Act & Assert
            var result = new ParseResult(null);
            Assert.Null(result.PackageReferences);
        }

        [Fact]
        public void Sdk_Property_CanBeSet()
        {
            // Arrange
            var packageReferences = new List<PackageReference>();
            var result = new ParseResult(packageReferences);
            var expectedSdk = "CustomSDK";

            // Act
            result.Sdk = expectedSdk;

            // Assert
            Assert.Equal(expectedSdk, result.Sdk);
        }

        [Fact]
        public void Sdk_Property_DefaultIsNull()
        {
            // Arrange
            var packageReferences = new List<PackageReference>();

            // Act
            var result = new ParseResult(packageReferences);

            // Assert
            Assert.Null(result.Sdk);
        }

        [Fact]
        public void Sdk_Property_CanBeSetToEmptyString()
        {
            // Arrange
            var packageReferences = new List<PackageReference>();
            var result = new ParseResult(packageReferences);

            // Act
            result.Sdk = string.Empty;

            // Assert
            Assert.Equal(string.Empty, result.Sdk);
        }

        [Fact]
        public void Sdk_Property_CanBeSetMultipleTimes()
        {
            // Arrange
            var packageReferences = new List<PackageReference>();
            var result = new ParseResult(packageReferences);

            // Act
            result.Sdk = "FirstSdk";
            result.Sdk = "SecondSdk";

            // Assert
            Assert.Equal("SecondSdk", result.Sdk);
        }

        [Fact]
        public void PackageReferences_IsReadOnlyCollection()
        {
            // Arrange
            var packageReferences = new List<PackageReference>();
            var result = new ParseResult(packageReferences);

            // Act & Assert
            Assert.IsAssignableFrom<IReadOnlyCollection<PackageReference>>(result.PackageReferences);
        }
    }
}