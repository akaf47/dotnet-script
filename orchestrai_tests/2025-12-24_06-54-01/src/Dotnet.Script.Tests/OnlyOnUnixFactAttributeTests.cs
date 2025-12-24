using System;
using System.Runtime.InteropServices;
using Moq;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class OnlyOnUnixFactAttributeTests
    {
        [Fact]
        public void Constructor_ShouldSetSkipPropertyToCanRunOnlyOnLinux_WhenRunningOnWindows()
        {
            // Arrange
            // We need to test the behavior, but RuntimeInformation is static
            // We'll verify the attribute instantiation works
            var attribute = new OnlyOnUnixFactAttribute();
            
            // Act & Assert
            // If running on Windows, Skip should be set
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.NotNull(attribute.Skip);
                Assert.Equal("Can run only on Linux", attribute.Skip);
            }
            else
            {
                // If not running on Windows, Skip should be null or empty
                Assert.True(string.IsNullOrEmpty(attribute.Skip));
            }
        }

        [Fact]
        public void Constructor_ShouldNotSetSkipProperty_WhenRunningOnLinux()
        {
            // Arrange
            var attribute = new OnlyOnUnixFactAttribute();
            
            // Act & Assert
            // If running on Linux, Skip should be null
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Null(attribute.Skip);
            }
            else
            {
                // If running on Windows, verify it's set
                Assert.NotNull(attribute.Skip);
            }
        }

        [Fact]
        public void Constructor_ShouldNotSetSkipProperty_WhenRunningOnMacOS()
        {
            // Arrange
            var attribute = new OnlyOnUnixFactAttribute();
            
            // Act & Assert
            // If running on macOS, Skip should be null
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Null(attribute.Skip);
            }
        }

        [Fact]
        public void OnlyOnUnixFactAttribute_InheritsFromFactAttribute()
        {
            // Arrange & Act
            var attribute = new OnlyOnUnixFactAttribute();
            
            // Assert
            Assert.IsAssignableFrom<Xunit.FactAttribute>(attribute);
        }

        [Fact]
        public void Constructor_CreatesValidInstance()
        {
            // Arrange & Act
            var attribute = new OnlyOnUnixFactAttribute();
            
            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void Constructor_OnWindowsPlatform_SetSkipMessage()
        {
            // Arrange
            var attribute = new OnlyOnUnixFactAttribute();
            
            // Act
            var skipMessage = attribute.Skip;
            
            // Assert
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Equal("Can run only on Linux", skipMessage);
            }
        }

        [Fact]
        public void Constructor_OnNonWindowsPlatform_DoesNotSetSkip()
        {
            // Arrange
            var attribute = new OnlyOnUnixFactAttribute();
            
            // Act
            var skipMessage = attribute.Skip;
            
            // Assert
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.True(string.IsNullOrEmpty(skipMessage) || skipMessage == null);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Attribute_CanBeInstantiatedMultipleTimes(bool unused)
        {
            // Arrange & Act
            var attribute1 = new OnlyOnUnixFactAttribute();
            var attribute2 = new OnlyOnUnixFactAttribute();
            
            // Assert
            Assert.NotNull(attribute1);
            Assert.NotNull(attribute2);
            // Both should have the same Skip behavior on the same OS
            Assert.Equal(attribute1.Skip, attribute2.Skip);
        }
    }
}