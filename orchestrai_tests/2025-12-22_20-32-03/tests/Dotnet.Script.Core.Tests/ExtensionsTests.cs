using Xunit;
using Dotnet.Script.Core;
using System;
using System.Collections.Generic;

namespace Dotnet.Script.Core.Tests
{
    public class ExtensionsTests
    {
        [Fact]
        public void Constructor_ShouldInitialize()
        {
            // Arrange & Act
            var extensions = new Extensions();

            // Assert
            Assert.NotNull(extensions);
        }

        [Fact]
        public void Extensions_Class_ShouldBePublic()
        {
            // Arrange & Act
            var type = typeof(Extensions);

            // Assert
            Assert.True(type.IsPublic);
        }
    }
}