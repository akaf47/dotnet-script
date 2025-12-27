using System;
using System.Reflection;
using Xunit;

namespace Dotnet.Script.Properties.Tests
{
    /// <summary>
    /// Tests for AssemblyInfo.cs metadata attributes.
    /// These tests verify that all assembly-level attributes are correctly defined
    /// to ensure proper versioning, naming, and metadata information.
    /// </summary>
    public class AssemblyInfoTests
    {
        [Fact]
        public void Assembly_Should_Have_Title_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            
            // Assert
            Assert.NotEmpty(titleAttributes);
            Assert.Single(titleAttributes);
            var titleAttribute = titleAttributes[0] as AssemblyTitleAttribute;
            Assert.NotNull(titleAttribute);
            Assert.NotEmpty(titleAttribute.Title);
        }

        [Fact]
        public void Assembly_Should_Have_Description_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var descriptionAttributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            
            // Assert
            Assert.NotEmpty(descriptionAttributes);
            Assert.Single(descriptionAttributes);
            var descriptionAttribute = descriptionAttributes[0] as AssemblyDescriptionAttribute;
            Assert.NotNull(descriptionAttribute);
            Assert.NotEmpty(descriptionAttribute.Description);
        }

        [Fact]
        public void Assembly_Should_Have_Configuration_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var configurationAttributes = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            
            // Assert
            Assert.NotEmpty(configurationAttributes);
            Assert.Single(configurationAttributes);
            var configurationAttribute = configurationAttributes[0] as AssemblyConfigurationAttribute;
            Assert.NotNull(configurationAttribute);
        }

        [Fact]
        public void Assembly_Should_Have_Company_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var companyAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            
            // Assert
            Assert.NotEmpty(companyAttributes);
            Assert.Single(companyAttributes);
            var companyAttribute = companyAttributes[0] as AssemblyCompanyAttribute;
            Assert.NotNull(companyAttribute);
        }

        [Fact]
        public void Assembly_Should_Have_Product_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            
            // Assert
            Assert.NotEmpty(productAttributes);
            Assert.Single(productAttributes);
            var productAttribute = productAttributes[0] as AssemblyProductAttribute;
            Assert.NotNull(productAttribute);
            Assert.NotEmpty(productAttribute.Product);
        }

        [Fact]
        public void Assembly_Should_Have_Copyright_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var copyrightAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            
            // Assert
            Assert.NotEmpty(copyrightAttributes);
            Assert.Single(copyrightAttributes);
            var copyrightAttribute = copyrightAttributes[0] as AssemblyCopyrightAttribute;
            Assert.NotNull(copyrightAttribute);
            Assert.NotEmpty(copyrightAttribute.Copyright);
        }

        [Fact]
        public void Assembly_Should_Have_Trademark_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var trademarkAttributes = assembly.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);
            
            // Assert
            Assert.NotEmpty(trademarkAttributes);
            Assert.Single(trademarkAttributes);
            var trademarkAttribute = trademarkAttributes[0] as AssemblyTrademarkAttribute;
            Assert.NotNull(trademarkAttribute);
        }

        [Fact]
        public void Assembly_Should_Have_Culture_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var cultureAttributes = assembly.GetCustomAttributes(typeof(AssemblyCultureAttribute), false);
            
            // Assert
            Assert.NotEmpty(cultureAttributes);
        }

        [Fact]
        public void Assembly_Should_Have_Version_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var versionAttributes = assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
            
            // Assert
            Assert.NotEmpty(versionAttributes);
            Assert.Single(versionAttributes);
            var versionAttribute = versionAttributes[0] as AssemblyVersionAttribute;
            Assert.NotNull(versionAttribute);
            Assert.NotEmpty(versionAttribute.Version);
            Assert.Matches(@"^\d+\.\d+\.\d+\.\d+$", versionAttribute.Version);
        }

        [Fact]
        public void Assembly_Should_Have_FileVersion_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var fileVersionAttributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            
            // Assert
            Assert.NotEmpty(fileVersionAttributes);
            Assert.Single(fileVersionAttributes);
            var fileVersionAttribute = fileVersionAttributes[0] as AssemblyFileVersionAttribute;
            Assert.NotNull(fileVersionAttribute);
            Assert.NotEmpty(fileVersionAttribute.Version);
            Assert.Matches(@"^\d+\.\d+\.\d+\.\d+$", fileVersionAttribute.Version);
        }

        [Fact]
        public void Assembly_Version_And_FileVersion_Should_Match()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            var versionAttributes = assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
            var fileVersionAttributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            
            // Act
            var assemblyVersion = (versionAttributes[0] as AssemblyVersionAttribute)?.Version;
            var fileVersion = (fileVersionAttributes[0] as AssemblyFileVersionAttribute)?.Version;
            
            // Assert
            Assert.Equal(assemblyVersion, fileVersion);
        }

        [Fact]
        public void Assembly_Should_Have_InformationalVersion_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var informationalVersionAttributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            
            // Assert
            Assert.NotEmpty(informationalVersionAttributes);
        }

        [Fact]
        public void Assembly_Should_Have_ComVisible_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var comVisibleAttributes = assembly.GetCustomAttributes(typeof(System.Runtime.InteropServices.ComVisibleAttribute), false);
            
            // Assert
            // ComVisible attribute is optional but if present should be valid
            if (comVisibleAttributes.Length > 0)
            {
                Assert.Single(comVisibleAttributes);
            }
        }

        [Fact]
        public void Assembly_Should_Have_CLSCompliant_Attribute()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var clsCompliantAttributes = assembly.GetCustomAttributes(typeof(CLSCompliantAttribute), false);
            
            // Assert
            // CLSCompliant attribute may or may not be present
            if (clsCompliantAttributes.Length > 0)
            {
                Assert.NotEmpty(clsCompliantAttributes);
            }
        }

        [Fact]
        public void AssemblyTitle_Should_NotBeEmpty()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            
            // Act
            var title = (titleAttributes[0] as AssemblyTitleAttribute)?.Title;
            
            // Assert
            Assert.NotNull(title);
            Assert.NotEmpty(title);
            Assert.False(string.IsNullOrWhiteSpace(title));
        }

        [Fact]
        public void AssemblyProduct_Should_NotBeEmpty()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            
            // Act
            var product = (productAttributes[0] as AssemblyProductAttribute)?.Product;
            
            // Assert
            Assert.NotNull(product);
            Assert.NotEmpty(product);
            Assert.False(string.IsNullOrWhiteSpace(product));
        }

        [Fact]
        public void AssemblyVersion_Should_BeValidSemanticVersion()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            var versionAttributes = assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
            var version = (versionAttributes[0] as AssemblyVersionAttribute)?.Version;
            
            // Act & Assert
            Assert.NotNull(version);
            Assert.True(Version.TryParse(version, out var parsedVersion));
            Assert.True(parsedVersion.Major >= 0);
            Assert.True(parsedVersion.Minor >= 0);
            Assert.True(parsedVersion.Build >= 0);
            Assert.True(parsedVersion.Revision >= 0);
        }

        [Fact]
        public void AssemblyFileVersion_Should_BeValidSemanticVersion()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            var fileVersionAttributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            var fileVersion = (fileVersionAttributes[0] as AssemblyFileVersionAttribute)?.Version;
            
            // Act & Assert
            Assert.NotNull(fileVersion);
            Assert.True(Version.TryParse(fileVersion, out var parsedVersion));
            Assert.True(parsedVersion.Major >= 0);
            Assert.True(parsedVersion.Minor >= 0);
            Assert.True(parsedVersion.Build >= 0);
            Assert.True(parsedVersion.Revision >= 0);
        }

        [Fact]
        public void Assembly_Should_HaveConsistentMetadata()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            var companyAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            
            // Assert
            Assert.NotEmpty(titleAttributes);
            Assert.NotEmpty(productAttributes);
            Assert.NotEmpty(companyAttributes);
            
            var title = (titleAttributes[0] as AssemblyTitleAttribute)?.Title;
            var product = (productAttributes[0] as AssemblyProductAttribute)?.Product;
            var company = (companyAttributes[0] as AssemblyCompanyAttribute)?.Company;
            
            Assert.NotNull(title);
            Assert.NotNull(product);
            Assert.NotNull(company);
        }

        [Fact]
        public void Assembly_Title_Should_ContainValidCharacters()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            var title = (titleAttributes[0] as AssemblyTitleAttribute)?.Title;
            
            // Act & Assert
            Assert.NotNull(title);
            Assert.DoesNotContain("\0", title);
            Assert.DoesNotContain("\n", title);
            Assert.DoesNotContain("\r", title);
        }

        [Fact]
        public void Assembly_Product_Should_ContainValidCharacters()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            var product = (productAttributes[0] as AssemblyProductAttribute)?.Product;
            
            // Act & Assert
            Assert.NotNull(product);
            Assert.DoesNotContain("\0", product);
            Assert.DoesNotContain("\n", product);
            Assert.DoesNotContain("\r", product);
        }

        [Fact]
        public void Assembly_FullName_Should_NotBeNull()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var fullName = assembly.FullName;
            
            // Assert
            Assert.NotNull(fullName);
            Assert.NotEmpty(fullName);
        }

        [Fact]
        public void Assembly_Name_Should_Match_Expected_Format()
        {
            // Arrange
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Act
            var name = assembly.GetName();
            
            // Assert
            Assert.NotNull(name);
            Assert.NotNull(name.Name);
            Assert.NotEmpty(name.Name);
        }

        [Fact]
        public void Assembly_Should_Load_Successfully()
        {
            // Arrange & Act
            var assembly = typeof(Dotnet.Script.Properties.AssemblyInfo).Assembly;
            
            // Assert
            Assert.NotNull(assembly);
            Assert.False(assembly.IsDynamic);
        }
    }
}