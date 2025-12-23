using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace Dotnet.Script.Extras.Tests.Properties
{
    /// <summary>
    /// Comprehensive tests for AssemblyInfo metadata.
    /// These tests verify that all assembly-level attributes are correctly applied
    /// and contain the expected values.
    /// </summary>
    public class AssemblyInfoTests
    {
        private readonly Assembly _assembly = typeof(AssemblyInfoTests).Assembly;

        [Fact]
        public void Assembly_HasAssemblyConfigurationAttribute()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var configAttr = (AssemblyConfigurationAttribute)attributes[0];
            Assert.NotNull(configAttr);
        }

        [Fact]
        public void Assembly_AssemblyConfigurationAttribute_HasCorrectValue()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var configAttr = (AssemblyConfigurationAttribute)attributes[0];
            // Expected to be empty string based on the source
            Assert.Equal("", configAttr.Configuration);
        }

        [Fact]
        public void Assembly_HasAssemblyCompanyAttribute()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var companyAttr = (AssemblyCompanyAttribute)attributes[0];
            Assert.NotNull(companyAttr);
        }

        [Fact]
        public void Assembly_AssemblyCompanyAttribute_HasCorrectValue()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var companyAttr = (AssemblyCompanyAttribute)attributes[0];
            // Expected to be empty string based on the source
            Assert.Equal("", companyAttr.Company);
        }

        [Fact]
        public void Assembly_HasAssemblyProductAttribute()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var productAttr = (AssemblyProductAttribute)attributes[0];
            Assert.NotNull(productAttr);
        }

        [Fact]
        public void Assembly_AssemblyProductAttribute_HasCorrectValue()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var productAttr = (AssemblyProductAttribute)attributes[0];
            Assert.Equal("Dotnet.Script.Extras", productAttr.Product);
        }

        [Fact]
        public void Assembly_HasAssemblyTrademarkAttribute()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var trademarkAttr = (AssemblyTrademarkAttribute)attributes[0];
            Assert.NotNull(trademarkAttr);
        }

        [Fact]
        public void Assembly_AssemblyTrademarkAttribute_HasCorrectValue()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var trademarkAttr = (AssemblyTrademarkAttribute)attributes[0];
            // Expected to be empty string based on the source
            Assert.Equal("", trademarkAttr.Trademark);
        }

        [Fact]
        public void Assembly_HasComVisibleAttribute()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(ComVisibleAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var comAttr = (ComVisibleAttribute)attributes[0];
            Assert.NotNull(comAttr);
        }

        [Fact]
        public void Assembly_ComVisibleAttribute_HasCorrectValue()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(ComVisibleAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var comAttr = (ComVisibleAttribute)attributes[0];
            // Expected to be false based on the source
            Assert.False(comAttr.Value);
        }

        [Fact]
        public void Assembly_HasGuidAttribute()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(GuidAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var guidAttr = (GuidAttribute)attributes[0];
            Assert.NotNull(guidAttr);
        }

        [Fact]
        public void Assembly_GuidAttribute_HasCorrectValue()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");
            var expectedGuid = "bcbb3155-4463-4748-8032-ef82ae297ce1";

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(GuidAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var guidAttr = (GuidAttribute)attributes[0];
            Assert.Equal(expectedGuid, guidAttr.Value);
        }

        [Fact]
        public void Assembly_GuidAttribute_IsValidFormat()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(typeof(GuidAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attributes);
            var guidAttr = (GuidAttribute)attributes[0];
            
            // Verify it's a valid GUID format
            var canParse = Guid.TryParse(guidAttr.Value, out var guid);
            Assert.True(canParse);
            Assert.NotEqual(Guid.Empty, guid);
        }

        [Fact]
        public void Assembly_AllExpectedAttributesPresent()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");
            var attributeTypes = new[]
            {
                typeof(AssemblyConfigurationAttribute),
                typeof(AssemblyCompanyAttribute),
                typeof(AssemblyProductAttribute),
                typeof(AssemblyTrademarkAttribute),
                typeof(ComVisibleAttribute),
                typeof(GuidAttribute)
            };

            // Act
            var presentAttributes = new System.Collections.Generic.List<Type>();
            foreach (var attrType in attributeTypes)
            {
                var attrs = assembly.GetCustomAttributes(attrType, inherit: false);
                if (attrs.Length > 0)
                {
                    presentAttributes.Add(attrType);
                }
            }

            // Assert
            Assert.Equal(attributeTypes.Length, presentAttributes.Count);
            foreach (var attrType in attributeTypes)
            {
                Assert.Contains(attrType, presentAttributes);
            }
        }

        [Fact]
        public void Assembly_Name_IsCorrect()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var assemblyName = assembly.GetName().Name;

            // Assert
            Assert.Equal("Dotnet.Script.Extras", assemblyName);
        }

        [Fact]
        public void Assembly_HasValidStructure()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var types = assembly.GetTypes();

            // Assert
            Assert.NotNull(types);
            Assert.NotEmpty(types);
        }

        [Fact]
        public void Assembly_Attributes_CanBeLoadedMultipleTimes()
        {
            // Arrange
            var assembly1 = Assembly.Load("Dotnet.Script.Extras");
            var assembly2 = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attrs1 = assembly1.GetCustomAttributes(typeof(GuidAttribute), inherit: false);
            var attrs2 = assembly2.GetCustomAttributes(typeof(GuidAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(attrs1);
            Assert.NotEmpty(attrs2);
            var guidAttr1 = (GuidAttribute)attrs1[0];
            var guidAttr2 = (GuidAttribute)attrs2[0];
            Assert.Equal(guidAttr1.Value, guidAttr2.Value);
        }

        [Fact]
        public void AssemblyInfo_Attributes_AreAccessible()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var allAttributes = assembly.GetCustomAttributes(inherit: false);

            // Assert
            Assert.NotNull(allAttributes);
            Assert.NotEmpty(allAttributes);
            
            // Verify that various attribute types are present
            var attributeTypeNames = new System.Collections.Generic.HashSet<string>();
            foreach (var attr in allAttributes)
            {
                attributeTypeNames.Add(attr.GetType().Name);
            }

            Assert.Contains("AssemblyConfigurationAttribute", attributeTypeNames);
            Assert.Contains("AssemblyCompanyAttribute", attributeTypeNames);
            Assert.Contains("AssemblyProductAttribute", attributeTypeNames);
            Assert.Contains("AssemblyTrademarkAttribute", attributeTypeNames);
            Assert.Contains("ComVisibleAttribute", attributeTypeNames);
            Assert.Contains("GuidAttribute", attributeTypeNames);
        }

        [Fact]
        public void Assembly_Product_MatchesExpectedValue()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(productAttributes);
            var productAttr = (AssemblyProductAttribute)productAttributes[0];
            Assert.NotNull(productAttr.Product);
            Assert.Equal("Dotnet.Script.Extras", productAttr.Product);
        }

        [Fact]
        public void Assembly_ComVisible_IsFalseAsExpected()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var comAttributes = assembly.GetCustomAttributes(typeof(ComVisibleAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(comAttributes);
            var comAttr = (ComVisibleAttribute)comAttributes[0];
            Assert.False(comAttr.Value, "ComVisible should be false for internal assemblies");
        }

        [Fact]
        public void Assembly_Configuration_IsEmptyString()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var configAttributes = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(configAttributes);
            var configAttr = (AssemblyConfigurationAttribute)configAttributes[0];
            Assert.Equal("", configAttr.Configuration);
        }

        [Fact]
        public void Assembly_Company_IsEmptyString()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var companyAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(companyAttributes);
            var companyAttr = (AssemblyCompanyAttribute)companyAttributes[0];
            Assert.Equal("", companyAttr.Company);
        }

        [Fact]
        public void Assembly_Trademark_IsEmptyString()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var trademarkAttributes = assembly.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(trademarkAttributes);
            var trademarkAttr = (AssemblyTrademarkAttribute)trademarkAttributes[0];
            Assert.Equal("", trademarkAttr.Trademark);
        }

        [Fact]
        public void Assembly_Guid_IsNotEmpty()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var guidAttributes = assembly.GetCustomAttributes(typeof(GuidAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(guidAttributes);
            var guidAttr = (GuidAttribute)guidAttributes[0];
            Assert.NotNull(guidAttr.Value);
            Assert.NotEmpty(guidAttr.Value);
            Assert.NotEqual(Guid.Empty.ToString(), guidAttr.Value);
        }

        [Fact]
        public void AssemblyInfo_NamespaceDoesNotExport()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var comAttributes = assembly.GetCustomAttributes(typeof(ComVisibleAttribute), inherit: false);

            // Assert
            Assert.NotEmpty(comAttributes);
            var comAttr = (ComVisibleAttribute)comAttributes[0];
            
            // ComVisible(false) means types are not exported to COM by default
            Assert.False(comAttr.Value);
        }

        [Fact]
        public void AssemblyInfo_ProductIdentifier_MatchesAssemblyName()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var assemblyName = assembly.GetName().Name;
            var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), inherit: false);

            // Assert
            var productAttr = (AssemblyProductAttribute)productAttributes[0];
            Assert.Equal(assemblyName, productAttr.Product);
        }

        [Fact]
        public void AssemblyInfo_HasExpectedNumberOfAttributes()
        {
            // Arrange
            var assembly = Assembly.Load("Dotnet.Script.Extras");

            // Act
            var attributes = assembly.GetCustomAttributes(inherit: false);

            // Assert
            // Should have at least 6 attributes (the ones we set)
            Assert.True(attributes.Length >= 6, $"Expected at least 6 attributes, found {attributes.Length}");
        }
    }
}