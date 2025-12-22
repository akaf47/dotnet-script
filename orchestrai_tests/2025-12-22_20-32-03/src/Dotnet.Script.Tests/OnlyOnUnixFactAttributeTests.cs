using System.Runtime.InteropServices;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class OnlyOnUnixFactAttributeTests
    {
        [Fact]
        public void ConstructorSkipsTestOnWindows()
        {
            var attribute = new OnlyOnUnixFactAttribute();
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.NotNull(attribute.Skip);
                Assert.NotEmpty(attribute.Skip);
                Assert.Contains("Linux", attribute.Skip);
            }
            else
            {
                Assert.Null(attribute.Skip);
            }
        }

        [Fact]
        public void ConstructorDoesNotSkipTestOnLinux()
        {
            var attribute = new OnlyOnUnixFactAttribute();
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Assert.Null(attribute.Skip);
            }
        }

        [Fact]
        public void ConstructorDoesNotSkipTestOnMac()
        {
            var attribute = new OnlyOnUnixFactAttribute();
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.Null(attribute.Skip);
            }
        }

        [Fact]
        public void SkipMessageIsInformative()
        {
            var attribute = new OnlyOnUnixFactAttribute();
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.NotNull(attribute.Skip);
                Assert.True(attribute.Skip.Length > 0);
            }
        }

        [Fact]
        public void MultipleInstancesHaveConsistentBehavior()
        {
            var attribute1 = new OnlyOnUnixFactAttribute();
            var attribute2 = new OnlyOnUnixFactAttribute();
            
            Assert.Equal(attribute1.Skip, attribute2.Skip);
        }
    }
}