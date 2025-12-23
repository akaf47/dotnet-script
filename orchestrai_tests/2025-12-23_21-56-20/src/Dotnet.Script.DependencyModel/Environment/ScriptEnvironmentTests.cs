using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit;
using Dotnet.Script.DependencyModel.Environment;

namespace Dotnet.Script.DependencyModel.Tests.Environment
{
    public class ScriptEnvironmentTests
    {
        [Fact]
        public void Default_ShouldReturnSingletonInstance()
        {
            // Arrange & Act
            var first = ScriptEnvironment.Default;
            var second = ScriptEnvironment.Default;

            // Assert
            Assert.NotNull(first);
            Assert.Same(first, second);
        }

        [Fact]
        public void IsWindows_ShouldReturnBoolean()
        {
            // Arrange
            var env = ScriptEnvironment.Default;

            // Act
            var result = env.IsWindows;

            // Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void PlatformIdentifier_ShouldReturnValidPlatform()
        {
            // Arrange
            var env = ScriptEnvironment.Default;

            // Act
            var result = env.PlatformIdentifier;

            // Assert
            Assert.NotNull(result);
            Assert.True(result == "win" || result == "osx" || result == "linux", 
                $"Platform identifier should be 'win', 'osx', or 'linux', but got '{result}'");
        }

        [Fact]
        public void RuntimeIdentifier_ShouldReturnNonNullString()
        {
            // Arrange
            var env = ScriptEnvironment.Default;

            // Act
            var result = env.RuntimeIdentifier;

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void TargetFramework_ShouldReturnValidFramework()
        {
            // Arrange
            var env = ScriptEnvironment.Default;

            // Act
            var result = env.TargetFramework;

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.StartsWith("net") || result == "net472", 
                $"Target framework should start with 'net', but got '{result}'");
        }

        [Fact]
        public void InstallLocation_ShouldReturnNonEmptyString()
        {
            // Arrange
            var env = ScriptEnvironment.Default;

            // Act
            var result = env.InstallLocation;

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void ProccessorArchitecture_ShouldReturnNonEmptyString()
        {
            // Arrange
            var env = ScriptEnvironment.Default;

            // Act
            var result = env.ProccessorArchitecture;

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void NuGetStoreFolder_ShouldReturnValidPath()
        {
            // Arrange
            var env = ScriptEnvironment.Default;

            // Act
            var result = env.NuGetStoreFolder;

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("store", result);
        }

        [Fact]
        public void NetCoreVersion_ShouldReturnDotnetVersion()
        {
            // Arrange
            var env = ScriptEnvironment.Default;

            // Act
            var result = env.NetCoreVersion;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DotnetVersion>(result);
        }

        [Fact]
        public void IsNetCore_ShouldReturnBoolean()
        {
            // Arrange
            var env = ScriptEnvironment.Default;

            // Act
            var result = env.IsNetCore;

            // Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void OverrideTargetFramework_ShouldSetTargetFramework()
        {
            // Arrange
            var newInstance = new ScriptEnvironment();
            var customFramework = "net9.0";

            // Act
            newInstance.OverrideTargetFramework(customFramework);
            var result = newInstance.TargetFramework;

            // Assert
            Assert.Equal(customFramework, result);
        }

        [Fact]
        public void OverrideTargetFramework_ShouldThrowWhenAlreadyResolved()
        {
            // Arrange
            var newInstance = new ScriptEnvironment();
            var firstFramework = newInstance.TargetFramework; // Resolve the lazy value

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => 
                newInstance.OverrideTargetFramework("net9.0"));
            Assert.Contains("Cannot override target framework", ex.Message);
            Assert.Contains(firstFramework, ex.Message);
        }

        [Fact]
        public void GetNetCoreVersion_ShouldReturnVersionOrNull()
        {
            // Arrange & Act
            var result = ScriptEnvironment.GetNetCoreVersion();

            // Assert
            // Result can be null if not in .NET Core runtime, or a version string
            if (result != null)
            {
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void TargetFramework_WhenOverridden_ShouldIgnoreLazyValue()
        {
            // Arrange
            var newInstance = new ScriptEnvironment();
            var overriddenValue = "netcustom";

            // Act
            newInstance.OverrideTargetFramework(overriddenValue);

            // Assert
            Assert.Equal(overriddenValue, newInstance.TargetFramework);
        }

        [Fact]
        public void IsWindows_ShouldConsistentlyReturnSamePlatformIdentifier()
        {
            // Arrange
            var env = ScriptEnvironment.Default;

            // Act
            var isWindows = env.IsWindows;
            var platformId = env.PlatformIdentifier;

            // Assert
            Assert.Equal(isWindows, platformId == "win");
        }

        [Fact]
        public void RuntimeIdentifier_WhenPlatformIsOsxOrLinux_ShouldContainPlatformAndArch()
        {
            // Arrange
            var env = ScriptEnvironment.Default;

            // Act
            var runtimeId = env.RuntimeIdentifier;
            var platformId = env.PlatformIdentifier;

            // Assert
            if (platformId == "osx" || platformId == "linux")
            {
                Assert.Contains(platformId, runtimeId);
            }
        }

        [Fact]
        public void TargetFramework_ShouldUseNetCoreVersionWhenAvailable()
        {
            // Arrange
            var env = ScriptEnvironment.Default;

            // Act
            var targetFramework = env.TargetFramework;
            var isNetCore = env.IsNetCore;

            // Assert
            if (isNetCore)
            {
                Assert.StartsWith("netcoreapp", targetFramework);
            }
            else
            {
                Assert.Equal("net472", targetFramework);
            }
        }
    }

    public class DotnetVersionTests
    {
        [Fact]
        public void Constructor_WithValidVersion_ShouldParseMajorAndMinor()
        {
            // Arrange
            var version = "3.1.0";
            var tfm = "netcoreapp3.1";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal(version, dv.Version);
            Assert.Equal(tfm, dv.Tfm);
            Assert.Equal(3, dv.Major);
            Assert.Equal(1, dv.Minor);
        }

        [Fact]
        public void Constructor_WithMajorOnly_ShouldParseMajor()
        {
            // Arrange
            var version = "5";
            var tfm = "netcoreapp5";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal(version, dv.Version);
            Assert.Equal(5, dv.Major);
            Assert.Equal(0, dv.Minor);
        }

        [Fact]
        public void Constructor_WithVersion5OrHigher_ShouldUpdateTfm()
        {
            // Arrange
            var version = "5.0.0";
            var tfm = "netcoreapp5.0";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal("net5.0", dv.Tfm);
        }

        [Fact]
        public void Constructor_WithVersion6_ShouldUpdateTfmFormat()
        {
            // Arrange
            var version = "6.0.0";
            var tfm = "netcoreapp6.0";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal("net6.0", dv.Tfm);
        }

        [Fact]
        public void Constructor_WithVersion8_ShouldUpdateTfmFormat()
        {
            // Arrange
            var version = "8.0.0";
            var tfm = "netcoreapp8.0";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal("net8.0", dv.Tfm);
        }

        [Fact]
        public void Constructor_WithVersion9_ShouldUpdateTfmFormat()
        {
            // Arrange
            var version = "9.0.0";
            var tfm = "netcoreapp9.0";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal("net9.0", dv.Tfm);
        }

        [Fact]
        public void Constructor_WithVersion4_ShouldNotUpdateTfm()
        {
            // Arrange
            var version = "4.8.0";
            var tfm = "net472";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal("net472", dv.Tfm);
            Assert.Equal(4, dv.Major);
        }

        [Fact]
        public void Constructor_WithMajorOnly_ShouldHaveZeroMinor()
        {
            // Arrange
            var version = "3";
            var tfm = "netcoreapp3";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal(3, dv.Major);
            Assert.Equal(0, dv.Minor);
        }

        [Fact]
        public void Constructor_WithInvalidFormat_ShouldHaveDefaultValues()
        {
            // Arrange
            var version = "invalid";
            var tfm = "unknown";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal(version, dv.Version);
            Assert.Equal(tfm, dv.Tfm);
            Assert.Equal(0, dv.Major);
            Assert.Equal(0, dv.Minor);
        }

        [Fact]
        public void Unknown_ShouldReturnKnownInstance()
        {
            // Arrange & Act
            var unknown = DotnetVersion.Unknown;

            // Assert
            Assert.NotNull(unknown);
            Assert.Equal("unknown", unknown.Version);
            Assert.Equal("unknown", unknown.Tfm);
        }

        [Fact]
        public void Constructor_WithVersion10_ShouldUpdateTfmFormat()
        {
            // Arrange
            var version = "10.0.0";
            var tfm = "netcoreapp10.0";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal("net10.0", dv.Tfm);
        }

        [Fact]
        public void Constructor_WithDecimalVersion_ShouldParseBothParts()
        {
            // Arrange
            var version = "7.5";
            var tfm = "netcoreapp7.5";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal(7, dv.Major);
            Assert.Equal(5, dv.Minor);
        }

        [Fact]
        public void Constructor_WithLeadingZeros_ShouldParseCorrectly()
        {
            // Arrange
            var version = "03.01.00";
            var tfm = "netcoreapp3.1";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal(3, dv.Major);
            Assert.Equal(1, dv.Minor);
        }

        [Fact]
        public void Constructor_WithComplexVersionString_ShouldParseFirstTwoParts()
        {
            // Arrange
            var version = "5.0.0-preview.1+build.123";
            var tfm = "netcoreapp5.0";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal(5, dv.Major);
            Assert.Equal(0, dv.Minor);
        }

        [Fact]
        public void Constructor_WithEmptyString_ShouldHaveZeroValues()
        {
            // Arrange
            var version = "";
            var tfm = "";

            // Act
            var dv = new DotnetVersion(version, tfm);

            // Assert
            Assert.Equal("", dv.Version);
            Assert.Equal("", dv.Tfm);
            Assert.Equal(0, dv.Major);
            Assert.Equal(0, dv.Minor);
        }

        [Fact]
        public void Constructor_WithVersion5_ShouldUpdateTfmToNetFormat()
        {
            // Arrange
            var version = "5.0.0";
            var originalTfm = "netcoreapp5.0";

            // Act
            var dv = new DotnetVersion(version, originalTfm);

            // Assert
            Assert.Equal("net5.0", dv.Tfm);
            Assert.NotEqual(originalTfm, dv.Tfm);
        }
    }
}