using System;
using System.Reflection;
using Xunit;

namespace Dotnet.Script.Extras.Properties.Tests
{
    /// <summary>
    /// Tests for AssemblyInfo.cs metadata attributes.
    /// This test class validates that the assembly is properly configured with expected metadata.
    /// </summary>
    public class AssemblyInfoTests
    {
        private readonly Assembly _targetAssembly;

        public AssemblyInfoTests()
        {
            // Get the assembly that contains the AssemblyInfo attributes
            _targetAssembly = typeof(AssemblyInfo).Assembly;
        }

        [Fact]
        public void AssemblyConfiguration_ShouldBeEmpty()
        {
            // Arrange & Act
            var configAttribute = _targetAssembly.GetCustomAttribute<AssemblyConfigurationAttribute>();

            // Assert
            Assert.NotNull(configAttribute);
            Assert.Equal("", configAttribute.Configuration);
        }

        [Fact]
        public void AssemblyConfiguration_ShouldExist()
        {
            // Arrange & Act
            var configAttribute = _targetAssembly.GetCustomAttribute<AssemblyConfigurationAttribute>();

            // Assert
            Assert.NotNull(configAttribute);
        }

        [Fact]
        public void AssemblyCompany_ShouldBeEmpty()
        {
            // Arrange & Act
            var companyAttribute = _targetAssembly.GetCustomAttribute<AssemblyCompanyAttribute>();

            // Assert
            Assert.NotNull(companyAttribute);
            Assert.Equal("", companyAttribute.Company);
        }

        [Fact]
        public void AssemblyCompany_ShouldExist()
        {
            // Arrange & Act
            var companyAttribute = _targetAssembly.GetCustomAttribute<AssemblyCompanyAttribute>();

            // Assert
            Assert.NotNull(companyAttribute);
        }

        [Fact]
        public void AssemblyProduct_ShouldBeDotnetScriptExtras()
        {
            // Arrange & Act
            var productAttribute = _targetAssembly.GetCustomAttribute<AssemblyProductAttribute>();

            // Assert
            Assert.NotNull(productAttribute);
            Assert.Equal("Dotnet.Script.Extras", productAttribute.Product);
        }

        [Fact]
        public void AssemblyProduct_ShouldExist()
        {
            // Arrange & Act
            var productAttribute = _targetAssembly.GetCustomAttribute<AssemblyProductAttribute>();

            // Assert
            Assert.NotNull(productAttribute);
        }

        [Fact]
        public void AssemblyTrademark_ShouldBeEmpty()
        {
            // Arrange & Act
            var trademarkAttribute = _targetAssembly.GetCustomAttribute<AssemblyTrademarkAttribute>();

            // Assert
            Assert.NotNull(trademarkAttribute);
            Assert.Equal("", trademarkAttribute.Trademark);
        }

        [Fact]
        public void AssemblyTrademark_ShouldExist()
        {
            // Arrange & Act
            var trademarkAttribute = _targetAssembly.GetCustomAttribute<AssemblyTrademarkAttribute>();

            // Assert
            Assert.NotNull(trademarkAttribute);
        }

        [Fact]
        public void ComVisible_ShouldBeFalse()
        {
            // Arrange & Act
            var comVisibleAttribute = _targetAssembly.GetCustomAttribute<ComVisibleAttribute>();

            // Assert
            Assert.NotNull(comVisibleAttribute);
            Assert.False(comVisibleAttribute.Value);
        }

        [Fact]
        public void ComVisible_ShouldExist()
        {
            // Arrange & Act
            var comVisibleAttribute = _targetAssembly.GetCustomAttribute<ComVisibleAttribute>();

            // Assert
            Assert.NotNull(comVisibleAttribute);
        }

        [Fact]
        public void Guid_ShouldHaveExpectedValue()
        {
            // Arrange
            var expectedGuid = "bcbb3155-4463-4748-8032-ef82ae297ce1";

            // Act
            var guidAttribute = _targetAssembly.GetCustomAttribute<GuidAttribute>();

            // Assert
            Assert.NotNull(guidAttribute);
            Assert.Equal(expectedGuid, guidAttribute.Value);
        }

        [Fact]
        public void Guid_ShouldExist()
        {
            // Arrange & Act
            var guidAttribute = _targetAssembly.GetCustomAttribute<GuidAttribute>();

            // Assert
            Assert.NotNull(guidAttribute);
        }

        [Fact]
        public void Guid_ShouldBeValidFormat()
        {
            // Arrange & Act
            var guidAttribute = _targetAssembly.GetCustomAttribute<GuidAttribute>();

            // Assert
            Assert.NotNull(guidAttribute);
            var isValidGuid = Guid.TryParse(guidAttribute.Value, out _);
            Assert.True(isValidGuid, $"GUID '{guidAttribute.Value}' is not in valid format");
        }

        [Fact]
        public void AllRequiredAttributes_ShouldBePresent()
        {
            // Arrange
            var expectedAttributeTypes = new[]
            {
                typeof(AssemblyConfigurationAttribute),
                typeof(AssemblyCompanyAttribute),
                typeof(AssemblyProductAttribute),
                typeof(AssemblyTrademarkAttribute),
                typeof(ComVisibleAttribute),
                typeof(GuidAttribute)
            };

            // Act & Assert
            foreach (var attributeType in expectedAttributeTypes)
            {
                var attribute = _targetAssembly.GetCustomAttribute(attributeType);
                Assert.NotNull(attribute);
            }
        }

        [Fact]
        public void AssemblyProduct_ShouldNotBeNull()
        {
            // Arrange & Act
            var productAttribute = _targetAssembly.GetCustomAttribute<AssemblyProductAttribute>();

            // Assert
            Assert.NotNull(productAttribute);
            Assert.NotNull(productAttribute.Product);
        }

        [Fact]
        public void AssemblyProduct_ShouldNotBeEmpty()
        {
            // Arrange & Act
            var productAttribute = _targetAssembly.GetCustomAttribute<AssemblyProductAttribute>();

            // Assert
            Assert.NotNull(productAttribute);
            Assert.NotEmpty(productAttribute.Product);
        }

        [Fact]
        public void Guid_ShouldNotBeNullOrEmpty()
        {
            // Arrange & Act
            var guidAttribute = _targetAssembly.GetCustomAttribute<GuidAttribute>();

            // Assert
            Assert.NotNull(guidAttribute);
            Assert.NotNull(guidAttribute.Value);
            Assert.NotEmpty(guidAttribute.Value);
        }

        [Fact]
        public void ComVisible_ShouldNotAllowCOMInterop()
        {
            // Arrange & Act
            var comVisibleAttribute = _targetAssembly.GetCustomAttribute<ComVisibleAttribute>();

            // Assert
            Assert.NotNull(comVisibleAttribute);
            Assert.False(comVisibleAttribute.Value, 
                "ComVisible should be false to prevent COM interop access to assembly types");
        }

        [Fact]
        public void AssemblyInfo_ReflectiveAccess_ShouldSucceed()
        {
            // Arrange
            var attributeTypes = new[]
            {
                typeof(AssemblyConfigurationAttribute),
                typeof(AssemblyCompanyAttribute),
                typeof(AssemblyProductAttribute),
                typeof(AssemblyTrademarkAttribute),
                typeof(ComVisibleAttribute),
                typeof(GuidAttribute)
            };

            // Act & Assert - Verify reflective access works
            foreach (var attributeType in attributeTypes)
            {
                var attribute = _targetAssembly.GetCustomAttribute(attributeType);
                Assert.NotNull(attribute);
                Assert.IsAssignableFrom(attributeType, attribute);
            }
        }
    }
}