```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using FluentAssertions;

namespace DotNetScript.Tests
{
    public class NuGetPackageResolverTests
    {
        private readonly Mock<ILogger<NuGetPackageResolver>> _mockLogger;
        private readonly Mock<INuGetClient> _mockNuGetClient;
        private readonly NuGetPackageResolver _resolver;

        public NuGetPackageResolverTests()
        {
            _mockLogger = new Mock<ILogger<NuGetPackageResolver>>();
            _mockNuGetClient = new Mock<INuGetClient>();
            _resolver = new NuGetPackageResolver(_mockLogger.Object, _mockNuGetClient.Object);
        }

        [Fact]
        public async Task ResolvePackageAsync_WithValidPackage_ShouldReturnPackageReferences()
        {
            // Arrange
            var packageId = "Newtonsoft.Json";
            var version = "13.0.1";
            var packageIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(version));
            var expectedReferences = new List<string> { "path/to/Newtonsoft.Json.dll" };
            
            _mockNuGetClient.Setup(x => x.ResolvePackageAsync(packageIdentity))
                           .ReturnsAsync(expectedReferences);

            // Act
            var result = await _resolver.ResolvePackageAsync(packageId, version);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().Contain("path/to/Newtonsoft.Json.dll");
        }

        [Fact]
        public async Task ResolvePackageAsync_WithLatestVersion_ShouldResolveLatest()
        {
            // Arrange
            var packageId = "Newtonsoft.Json";
            var latestVersion = "13.0.3";
            var packageIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(latestVersion));
            var expectedReferences = new List<string> { "path/to/Newtonsoft.Json.dll" };
            
            _mockNuGetClient.Setup(x => x.GetLatestVersionAsync(packageId))
                           .ReturnsAsync(NuGetVersion.Parse(latestVersion));
            _mockNuGetClient.Setup(x => x.ResolvePackageAsync(packageIdentity))
                           .ReturnsAsync(expectedReferences);

            // Act
            var result = await _resolver.ResolvePackageAsync(packageId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            _mockNuGetClient.Verify(x => x.GetLatestVersionAsync(packageId), Times.Once);
        }

        [Fact]
        public async Task ResolvePackageAsync_WithNonExistentPackage_ShouldThrowPackageNotFoundException()
        {
            // Arrange
            var packageId = "NonExistent.Package";
            var version = "1.0.0";
            var packageIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(version));
            
            _mockNuGetClient.Setup(x => x.ResolvePackageAsync(packageIdentity))
                           .ThrowsAsync(new PackageNotFoundException($"Package {packageId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<PackageNotFoundException>(() => _resolver.ResolvePackageAsync(packageId, version));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task ResolvePackageAsync_WithInvalidPackageId_ShouldThrowArgumentException(string packageId)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _resolver.ResolvePackageAsync(packageId));
        }

        [Fact]
        public async Task ResolvePackageAsync_WithInvalidVersion_ShouldThrowArgumentException()
        {
            // Arrange
            var packageId = "Newtonsoft.Json";
            var invalidVersion = "invalid.version";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _resolver.ResolvePackageAsync(packageId, invalidVersion));
        }

        [Fact]
        public async Task ResolveMultiplePackagesAsync_WithValidPackages_ShouldReturnAllReferences()
        {
            // Arrange
            var packages = new Dictionary<string, string>
            {
                { "Newtonsoft.Json", "13.0.1" },
                { "System.Text.Json", "6.0.0" }
            };
            
            _mockNuGetClient.Setup(x => x.ResolvePackageAsync(It.IsAny<PackageIdentity>()))
                           .ReturnsAsync((PackageIdentity identity) => 
                               new List<string> { $"path/to/{identity.Id}.dll" });

            // Act
            var result = await _resolver.ResolveMultiplePackagesAsync(packages);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain("path/to/Newtonsoft.Json.dll");
            result.Should().Contain("path/to/System.Text.Json.dll");
        }

        [Fact]
        public async Task ResolveMultiplePackagesAsync_WithEmptyPackages_ShouldReturnEmptyList()
        {
            // Arrange
            var packages = new Dictionary<string, string>();

            // Act
            var result = await _resolver.ResolveMultiplePackagesAsync(packages);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void IsPackageCached_WithCachedPackage_ShouldReturnTrue()
        {
            // Arrange
            var packageId = "Newtonsoft.Json";
            var version = "13.0.1";
            _mockNuGetClient.Setup(x => x.IsPackageCached(packageId, version))
                           .Returns(true);

            // Act
            var result = _resolver.IsPackageCached(packageId, version);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsPackageCached_WithNonCachedPackage_ShouldReturnFalse()
        {
            // Arrange
            var packageId = "Newtonsoft.Json";
            var version = "13.0.1";
            _mockNuGetClient.Setup(x => x.IsPackageCached(packageId, version))
                           .Returns(false);

            // Act