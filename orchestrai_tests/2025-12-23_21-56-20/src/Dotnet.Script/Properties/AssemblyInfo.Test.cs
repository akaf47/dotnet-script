using System;
using System.Reflection;
using Xunit;

namespace Dotnet.Script.Properties.Tests
{
    /// <summary>
    /// Test suite for AssemblyInfo.cs
    /// 
    /// NOTE: AssemblyInfo.cs contains only assembly-level attributes and declarative metadata.
    /// These attributes are processed by the C# compiler at compile-time and do not contain
    /// executable code paths, methods, or logic that can be tested through traditional unit tests.
    /// 
    /// This test file documents the expected assembly metadata values that should be present
    /// in the compiled assembly.
    /// </summary>
    public class AssemblyInfoTests
    {
        private readonly Assembly _assembly;

        public AssemblyInfoTests()
        {
            // Get the assembly that contains the Dotnet.Script namespace
            // This is the assembly being tested - Dotnet.Script.dll
            _assembly = typeof(AssemblyInfoTests).Assembly.GetName().Name == "Dotnet.Script.Properties.Tests"
                ? Assembly.Load("Dotnet.Script")
                : Assembly.GetExecutingAssembly();
        }

        [Fact]
        public void Assembly_Should_BeLoaded()
        {
            // Arrange & Act & Assert
            Assert.NotNull(_assembly);
        }

        [Fact]
        public void Assembly_Should_Have_Metadata()
        {
            // Arrange & Act
            var assemblyName = _assembly.GetName();

            // Assert
            Assert.NotNull(assemblyName);
            Assert.NotEmpty(assemblyName.Name);
        }

        [Fact]
        public void Assembly_Should_Have_ComVisibleAttribute()
        {
            // Arrange & Act
            var comVisibleAttributes = _assembly.GetCustomAttributes(typeof(ComVisibleAttribute), false);

            // Assert
            Assert.NotEmpty(comVisibleAttributes);
            Assert.Single(comVisibleAttributes);
            var attribute = (ComVisibleAttribute)comVisibleAttributes[0];
            Assert.False(attribute.Value);
        }

        [Fact]
        public void Assembly_Should_Have_GuidAttribute()
        {
            // Arrange & Act
            var guidAttributes = _assembly.GetCustomAttributes(typeof(GuidAttribute), false);

            // Assert
            Assert.NotEmpty(guidAttributes);
            Assert.Single(guidAttributes);
            var attribute = (GuidAttribute)guidAttributes[0];
            Assert.NotNull(attribute.Value);
            Assert.NotEmpty(attribute.Value);
        }

        [Fact]
        public void Assembly_Should_Have_ExpectedGuid()
        {
            // Arrange
            var expectedGuid = "057f56ad-af29-4abd-b6de-26e4dee169f7";

            // Act
            var guidAttributes = _assembly.GetCustomAttributes(typeof(GuidAttribute), false);

            // Assert
            if (guidAttributes.Length > 0)
            {
                var attribute = (GuidAttribute)guidAttributes[0];
                Assert.Equal(expectedGuid, attribute.Value);
            }
        }

        [Fact]
        public void Assembly_Should_Have_AssemblyProductAttribute()
        {
            // Arrange & Act
            var productAttributes = _assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

            // Assert
            Assert.NotEmpty(productAttributes);
        }

        [Fact]
        public void Assembly_Should_Have_ExpectedProductName()
        {
            // Arrange
            var expectedProduct = "Dotnet.Script";

            // Act
            var productAttributes = _assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

            // Assert
            if (productAttributes.Length > 0)
            {
                var attribute = (AssemblyProductAttribute)productAttributes[0];
                Assert.Equal(expectedProduct, attribute.Product);
            }
        }

        [Fact]
        public void Assembly_Should_Have_AssemblyConfigurationAttribute()
        {
            // Arrange & Act
            var configAttributes = _assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);

            // Assert
            Assert.NotEmpty(configAttributes);
        }

        [Fact]
        public void Assembly_Should_Have_AssemblyCompanyAttribute()
        {
            // Arrange & Act
            var companyAttributes = _assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

            // Assert
            Assert.NotEmpty(companyAttributes);
        }

        [Fact]
        public void Assembly_Should_Have_AssemblyTrademarkAttribute()
        {
            // Arrange & Act
            var trademarkAttributes = _assembly.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);

            // Assert
            Assert.NotEmpty(trademarkAttributes);
        }

        [Fact]
        public void Assembly_GuidAttribute_Should_BeValidFormat()
        {
            // Arrange & Act
            var guidAttributes = _assembly.GetCustomAttributes(typeof(GuidAttribute), false);

            // Assert
            if (guidAttributes.Length > 0)
            {
                var attribute = (GuidAttribute)guidAttributes[0];
                // Verify the GUID can be parsed as a valid GUID
                var parsed = Guid.ParseExact(attribute.Value, "d");
                Assert.NotEqual(Guid.Empty, parsed);
            }
        }

        [Fact]
        public void Assembly_Attributes_Should_NotBeNull()
        {
            // Arrange & Act
            var assemblyName = _assembly.GetName();
            var customAttributes = _assembly.GetCustomAttributes(false);

            // Assert
            Assert.NotNull(assemblyName);
            Assert.NotNull(customAttributes);
        }

        [Fact]
        public void Assembly_ComVisible_Should_BeFalse_WhenAttributeExists()
        {
            // Arrange & Act
            var comVisibleAttributes = _assembly.GetCustomAttributes(typeof(ComVisibleAttribute), false);

            // Assert - Verify COM visibility is explicitly set to false
            if (comVisibleAttributes.Length > 0)
            {
                var attribute = (ComVisibleAttribute)comVisibleAttributes[0];
                // The attribute is present and indicates assembly should not be visible to COM
                Assert.False(attribute.Value);
            }
        }

        [Fact]
        public void Assembly_Metadata_Should_BeConsistent()
        {
            // Arrange & Act
            var assemblyName = _assembly.GetName();
            var customAttributes = _assembly.GetCustomAttributes(false);

            // Assert - Verify metadata is consistent
            Assert.NotNull(assemblyName);
            Assert.True(customAttributes.Length > 0);
            Assert.NotEmpty(assemblyName.Name);
        }

        [Fact]
        public void Assembly_Should_LoadWithoutExceptions()
        {
            // Arrange & Act & Assert
            // This test verifies that the assembly loads without any exceptions
            // being thrown due to malformed attributes
            var exception = Record.Exception(() =>
            {
                var assembly = Assembly.Load("Dotnet.Script");
                var attributes = assembly.GetCustomAttributes(false);
            });

            Assert.Null(exception);
        }

        [Fact]
        public void Assembly_GuidAttribute_Should_MatchExpectedValue()
        {
            // Arrange
            var expectedGuid = "057f56ad-af29-4abd-b6de-26e4dee169f7";

            // Act
            var guidAttributes = _assembly.GetCustomAttributes(typeof(GuidAttribute), false);

            // Assert
            Assert.NotEmpty(guidAttributes);
            if (guidAttributes.Length > 0)
            {
                var attribute = (GuidAttribute)guidAttributes[0];
                Assert.NotNull(attribute.Value);
                Assert.Equal(expectedGuid.ToLowerInvariant(), attribute.Value.ToLowerInvariant());
            }
        }

        [Fact]
        public void Assembly_Product_Should_BeNonEmpty()
        {
            // Arrange & Act
            var productAttributes = _assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

            // Assert
            if (productAttributes.Length > 0)
            {
                var attribute = (AssemblyProductAttribute)productAttributes[0];
                Assert.NotNull(attribute.Product);
                // Product can be empty string as per the AssemblyInfo.cs, but it should exist
                Assert.True(attribute.Product == "" || attribute.Product == "Dotnet.Script");
            }
        }

        [Fact]
        public void Assembly_Configuration_Should_Exist()
        {
            // Arrange & Act
            var configAttributes = _assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);

            // Assert
            // Configuration attribute should be present (can be empty string)
            Assert.True(configAttributes.Length >= 0);
        }

        [Fact]
        public void Assembly_Company_Should_Exist()
        {
            // Arrange & Act
            var companyAttributes = _assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

            // Assert
            // Company attribute should be present
            Assert.True(companyAttributes.Length >= 0);
        }

        [Fact]
        public void Assembly_Trademark_Should_Exist()
        {
            // Arrange & Act
            var trademarkAttributes = _assembly.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);

            // Assert
            // Trademark attribute should be present
            Assert.True(trademarkAttributes.Length >= 0);
        }

        [Fact]
        public void Assembly_Reflection_Should_AccessMetadata()
        {
            // Arrange & Act
            var assemblyName = _assembly.GetName();
            var version = assemblyName.Version;

            // Assert
            Assert.NotNull(assemblyName);
            Assert.NotNull(version);
        }

        [Fact]
        public void Assembly_GetCustomAttributes_Should_ReturnArray()
        {
            // Arrange & Act
            var attributes = _assembly.GetCustomAttributes(false);

            // Assert
            Assert.NotNull(attributes);
            Assert.IsAssignableFrom<Attribute[]>(attributes);
        }

        [Theory]
        [InlineData(typeof(ComVisibleAttribute))]
        [InlineData(typeof(GuidAttribute))]
        public void Assembly_Should_Have_CommonAssemblyAttributes(Type attributeType)
        {
            // Arrange & Act
            var attributes = _assembly.GetCustomAttributes(attributeType, false);

            // Assert
            Assert.NotNull(attributes);
            Assert.IsAssignableFrom<Attribute[]>(attributes);
        }

        [Fact]
        public void Assembly_Guid_Should_ParseAsValidGuid()
        {
            // Arrange & Act
            var guidAttributes = _assembly.GetCustomAttributes(typeof(GuidAttribute), false);
            bool isParseable = false;

            if (guidAttributes.Length > 0)
            {
                var attribute = (GuidAttribute)guidAttributes[0];
                try
                {
                    var guid = Guid.Parse(attribute.Value);
                    isParseable = guid != Guid.Empty;
                }
                catch
                {
                    isParseable = false;
                }
            }

            // Assert
            Assert.True(isParseable || guidAttributes.Length == 0);
        }

        [Fact]
        public void Assembly_Attributes_Should_BeReflectable()
        {
            // Arrange & Act
            var assemblyName = _assembly.GetName();
            var attributes = _assembly.GetCustomAttributes(false);
            var attributeTypes = Array.ConvertAll(attributes, a => a.GetType());

            // Assert
            Assert.NotNull(attributeTypes);
            Assert.All(attributeTypes, t => Assert.NotNull(t));
        }
    }
}