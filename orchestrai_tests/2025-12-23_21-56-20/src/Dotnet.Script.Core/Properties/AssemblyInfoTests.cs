using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace Dotnet.Script.Core.Properties.Tests
{
    public class AssemblyInfoTests
    {
        [Fact]
        public void Assembly_HasCorrectProductName()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var productAttribute = assembly.GetCustomAttribute<AssemblyProductAttribute>();

            // Assert
            Assert.NotNull(productAttribute);
            Assert.Equal("Dotnet.Script.Core", productAttribute.Product);
        }

        [Fact]
        public void Assembly_HasEmptyConfiguration()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var configAttribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();

            // Assert
            Assert.NotNull(configAttribute);
            Assert.Equal(string.Empty, configAttribute.Configuration);
        }

        [Fact]
        public void Assembly_HasEmptyCompany()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var companyAttribute = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();

            // Assert
            Assert.NotNull(companyAttribute);
            Assert.Equal(string.Empty, companyAttribute.Company);
        }

        [Fact]
        public void Assembly_HasEmptyTrademark()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var trademarkAttribute = assembly.GetCustomAttribute<AssemblyTrademarkAttribute>();

            // Assert
            Assert.NotNull(trademarkAttribute);
            Assert.Equal(string.Empty, trademarkAttribute.Trademark);
        }

        [Fact]
        public void Assembly_NotVisibleToCom()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var comVisibleAttribute = assembly.GetCustomAttribute<ComVisibleAttribute>();

            // Assert
            Assert.NotNull(comVisibleAttribute);
            Assert.False(comVisibleAttribute.Value);
        }

        [Fact]
        public void Assembly_HasGuidAttribute()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;
            var expectedGuid = "684fefee-451b-4e68-b662-c16e3a7da794";

            // Act
            var guidAttribute = assembly.GetCustomAttribute<GuidAttribute>();

            // Assert
            Assert.NotNull(guidAttribute);
            Assert.Equal(expectedGuid, guidAttribute.Value);
        }

        [Fact]
        public void Assembly_HasInternalsVisibleToTestsAssembly()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var internalsVisibleAttributes = assembly.GetCustomAttributes<InternalsVisibleToAttribute>();

            // Assert
            Assert.NotEmpty(internalsVisibleAttributes);
            var testAssemblyAttribute = Array.Find(internalsVisibleAttributes.ToArray(), 
                attr => attr.AssemblyName.Contains("Dotnet.Script.Tests"));
            Assert.NotNull(testAssemblyAttribute);
        }

        [Fact]
        public void Assembly_HasInternalsVisibleToSharedTestsAssembly()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var internalsVisibleAttributes = assembly.GetCustomAttributes<InternalsVisibleToAttribute>();

            // Assert
            Assert.NotEmpty(internalsVisibleAttributes);
            var sharedTestsAttribute = Array.Find(internalsVisibleAttributes.ToArray(), 
                attr => attr.AssemblyName.Contains("Dotnet.Script.Shared.Tests"));
            Assert.NotNull(sharedTestsAttribute);
        }

        [Fact]
        public void Assembly_InternalsVisibleToAttributeCount()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var internalsVisibleAttributes = assembly.GetCustomAttributes<InternalsVisibleToAttribute>();

            // Assert
            Assert.NotEmpty(internalsVisibleAttributes);
            Assert.True(internalsVisibleAttributes.Count() >= 2, 
                "Assembly should have at least 2 InternalsVisibleTo attributes");
        }

        [Fact]
        public void Assembly_ProductAttributeIsNotNull()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var attributes = assembly.GetCustomAttributes<AssemblyProductAttribute>();

            // Assert
            Assert.NotEmpty(attributes);
        }

        [Fact]
        public void Assembly_HasValidGuidFormat()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var guidAttribute = assembly.GetCustomAttribute<GuidAttribute>();

            // Assert
            Assert.NotNull(guidAttribute);
            // Verify it's a valid GUID format
            bool isValidGuid = Guid.TryParse(guidAttribute.Value, out _);
            Assert.True(isValidGuid, $"Assembly GUID '{guidAttribute.Value}' is not in valid GUID format");
        }

        [Fact]
        public void Assembly_ComVisibleAttributeValueIsFalse()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var comVisibleAttribute = assembly.GetCustomAttribute<ComVisibleAttribute>();

            // Assert
            Assert.NotNull(comVisibleAttribute);
            Assert.False(comVisibleAttribute.Value, "COM visibility should be false");
        }

        [Fact]
        public void Assembly_ConfigurationAttributeExists()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var configAttribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();

            // Assert
            Assert.NotNull(configAttribute);
        }

        [Fact]
        public void Assembly_CompanyAttributeExists()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var companyAttribute = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();

            // Assert
            Assert.NotNull(companyAttribute);
        }

        [Fact]
        public void Assembly_TrademarkAttributeExists()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var trademarkAttribute = assembly.GetCustomAttribute<AssemblyTrademarkAttribute>();

            // Assert
            Assert.NotNull(trademarkAttribute);
        }

        [Fact]
        public void Assembly_GuidAttributeExists()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var guidAttribute = assembly.GetCustomAttribute<GuidAttribute>();

            // Assert
            Assert.NotNull(guidAttribute);
        }

        [Fact]
        public void Assembly_InternalsVisibleToAttributeExists()
        {
            // Arrange
            var assembly = typeof(AssemblyInfoTests).Assembly;

            // Act
            var internalsVisibleAttributes = assembly.GetCustomAttributes<InternalsVisibleToAttribute>();

            // Assert
            Assert.NotEmpty(internalsVisibleAttributes);
        }
    }
}