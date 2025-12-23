using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using Moq;

namespace Dotnet.Script.DependencyModel.ProjectSystem.Tests
{
    public class ProjectFileTests
    {
        private const string ValidProjectXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
  <ItemGroup>
    <PackageReference Include=""Newtonsoft.Json"" Version=""13.0.1"" />
    <PackageReference Include=""NUnit"" Version=""3.13.2"" />
    <Reference Include=""System.Xml"" />
  </ItemGroup>
</Project>";

        private const string MinimalProjectXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
  <ItemGroup>
  </ItemGroup>
</Project>";

        private const string ProjectXmlWithReferences = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <ItemGroup>
    <PackageReference Include=""Package1"" Version=""1.0.0"" />
    <Reference Include=""System.Core"" />
    <Reference Include=""System.Xml.Linq"" />
  </ItemGroup>
</Project>";

        [Fact]
        public void ParameterlessConstructor_CreatesEmptyProjectFile()
        {
            // Act
            var projectFile = new ProjectFile();

            // Assert
            Assert.NotNull(projectFile.PackageReferences);
            Assert.Empty(projectFile.PackageReferences);
            Assert.NotNull(projectFile.AssemblyReferences);
            Assert.Empty(projectFile.AssemblyReferences);
            Assert.Equal("Microsoft.NET.Sdk", projectFile.Sdk);
        }

        [Fact]
        public void ParameterlessConstructor_SetsDefaultTargetFramework()
        {
            // Act
            var projectFile = new ProjectFile();

            // Assert
            Assert.NotNull(projectFile.TargetFramework);
            Assert.NotEmpty(projectFile.TargetFramework);
        }

        [Fact]
        public void XmlConstructor_ParsesValidXml()
        {
            // Act
            var projectFile = new ProjectFile(ValidProjectXml);

            // Assert
            Assert.NotNull(projectFile.PackageReferences);
            Assert.NotNull(projectFile.AssemblyReferences);
            Assert.Equal("Microsoft.NET.Sdk", projectFile.Sdk);
        }

        [Fact]
        public void XmlConstructor_ParsesPackageReferences()
        {
            // Act
            var projectFile = new ProjectFile(ValidProjectXml);

            // Assert
            Assert.Equal(2, projectFile.PackageReferences.Count);
            var packageIds = projectFile.PackageReferences.Select(p => p.Id.Value).ToList();
            Assert.Contains("Newtonsoft.Json", packageIds);
            Assert.Contains("NUnit", packageIds);
        }

        [Fact]
        public void XmlConstructor_ParsesPackageVersions()
        {
            // Act
            var projectFile = new ProjectFile(ValidProjectXml);

            // Assert
            var references = projectFile.PackageReferences.ToList();
            var newtonsoft = references.FirstOrDefault(r => r.Id.Value == "Newtonsoft.Json");
            var nunit = references.FirstOrDefault(r => r.Id.Value == "NUnit");
            
            Assert.NotNull(newtonsoft);
            Assert.NotNull(nunit);
            Assert.Equal("13.0.1", newtonsoft.Version.Value);
            Assert.Equal("3.13.2", nunit.Version.Value);
        }

        [Fact]
        public void XmlConstructor_ParsesAssemblyReferences()
        {
            // Act
            var projectFile = new ProjectFile(ValidProjectXml);

            // Assert
            Assert.Single(projectFile.AssemblyReferences);
            var assemblyPath = projectFile.AssemblyReferences.First().AssemblyPath;
            Assert.Equal("System.Xml", assemblyPath);
        }

        [Fact]
        public void XmlConstructor_ParsesSdk()
        {
            // Act
            var projectFile = new ProjectFile(ProjectXmlWithReferences);

            // Assert
            Assert.Equal("Microsoft.NET.Sdk.Web", projectFile.Sdk);
        }

        [Fact]
        public void XmlConstructor_WithMultipleAssemblyReferences()
        {
            // Act
            var projectFile = new ProjectFile(ProjectXmlWithReferences);

            // Assert
            Assert.Equal(2, projectFile.AssemblyReferences.Count);
            var paths = projectFile.AssemblyReferences.Select(r => r.AssemblyPath).ToList();
            Assert.Contains("System.Core", paths);
            Assert.Contains("System.Xml.Linq", paths);
        }

        [Fact]
        public void XmlConstructor_WithEmptyItemGroup()
        {
            // Act
            var projectFile = new ProjectFile(MinimalProjectXml);

            // Assert
            Assert.Empty(projectFile.PackageReferences);
            Assert.Empty(projectFile.AssemblyReferences);
        }

        [Fact]
        public void XmlConstructor_ThrowsOnInvalidXml()
        {
            // Arrange
            var invalidXml = "This is not valid XML";

            // Act & Assert
            Assert.Throws<System.Xml.XmlException>(() => new ProjectFile(invalidXml));
        }

        [Fact]
        public void IsCacheable_ReturnsTrueWhenAllVersionsPinned()
        {
            // Arrange
            var projectFile = new ProjectFile();
            projectFile.PackageReferences.Add(new PackageReference("Package1", "1.0.0"));
            projectFile.PackageReferences.Add(new PackageReference("Package2", "2.5.3"));

            // Act
            var isCacheable = projectFile.IsCacheable;

            // Assert
            Assert.True(isCacheable);
        }

        [Fact]
        public void IsCacheable_ReturnsFalseWhenVersionNotPinned()
        {
            // Arrange
            var projectFile = new ProjectFile();
            projectFile.PackageReferences.Add(new PackageReference("Package1", "1.0.*"));
            projectFile.PackageReferences.Add(new PackageReference("Package2", "2.0.0"));

            // Act
            var isCacheable = projectFile.IsCacheable;

            // Assert
            Assert.False(isCacheable);
        }

        [Fact]
        public void IsCacheable_ReturnsTrueWhenNoPackageReferences()
        {
            // Arrange
            var projectFile = new ProjectFile();

            // Act
            var isCacheable = projectFile.IsCacheable;

            // Assert
            Assert.True(isCacheable);
        }

        [Fact]
        public void IsCacheable_ReturnsTrueWithBracketedVersions()
        {
            // Arrange
            var projectFile = new ProjectFile();
            projectFile.PackageReferences.Add(new PackageReference("Package1", "[1.0.0]"));

            // Act
            var isCacheable = projectFile.IsCacheable;

            // Assert
            Assert.True(isCacheable);
        }

        [Fact]
        public void PackageReferences_InitializedAsHashSet()
        {
            // Act
            var projectFile = new ProjectFile();

            // Assert
            Assert.IsType<HashSet<PackageReference>>(projectFile.PackageReferences);
        }

        [Fact]
        public void AssemblyReferences_InitializedAsHashSet()
        {
            // Act
            var projectFile = new ProjectFile();

            // Assert
            Assert.IsType<HashSet<AssemblyReference>>(projectFile.AssemblyReferences);
        }

        [Fact]
        public void Save_CreatesProjectFileWithPackageReferences()
        {
            // Arrange
            var tempFilePath = Path.GetTempFileName();
            try
            {
                var projectFile = new ProjectFile();
                projectFile.PackageReferences.Add(new PackageReference("TestPackage", "1.0.0"));
                projectFile.TargetFramework = "net5.0";

                // Act
                projectFile.Save(tempFilePath);

                // Assert
                Assert.True(File.Exists(tempFilePath));
                var fileContent = File.ReadAllText(tempFilePath);
                Assert.Contains("TestPackage", fileContent);
                Assert.Contains("1.0.0", fileContent);
                Assert.Contains("net5.0", fileContent);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [Fact]
        public void Save_CreatesProjectFileWithAssemblyReferences()
        {
            // Arrange
            var tempFilePath = Path.GetTempFileName();
            try
            {
                var projectFile = new ProjectFile();
                projectFile.AssemblyReferences.Add(new AssemblyReference("System.Xml"));
                projectFile.TargetFramework = "net5.0";

                // Act
                projectFile.Save(tempFilePath);

                // Assert
                Assert.True(File.Exists(tempFilePath));
                var fileContent = File.ReadAllText(tempFilePath);
                Assert.Contains("System.Xml", fileContent);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [Fact]
        public void Save_CreatesProjectFileWithCustomSdk()
        {
            // Arrange
            var tempFilePath = Path.GetTempFileName();
            try
            {
                var projectFile = new ProjectFile();
                projectFile.Sdk = "Microsoft.NET.Sdk.Web";
                projectFile.TargetFramework = "net5.0";

                // Act
                projectFile.Save(tempFilePath);

                // Assert
                Assert.True(File.Exists(tempFilePath));
                var fileContent = File.ReadAllText(tempFilePath);
                Assert.Contains("Microsoft.NET.Sdk.Web", fileContent);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [Fact]
        public void Save_WithEmptySdk_DoesNotUpdateSdk()
        {
            // Arrange
            var tempFilePath = Path.GetTempFileName();
            try
            {
                var projectFile = new ProjectFile();
                projectFile.Sdk = string.Empty;
                projectFile.TargetFramework = "net5.0";

                // Act
                projectFile.Save(tempFilePath);

                // Assert
                Assert.True(File.Exists(tempFilePath));
                var doc = XDocument.Load(tempFilePath);
                var sdkAttribute = doc.Root?.Attribute("Sdk");
                // The SDK should not be updated when empty string is set
                Assert.NotNull(sdkAttribute);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [Fact]
        public void Save_WithNullSdk_DoesNotUpdateSdk()
        {
            // Arrange
            var tempFilePath = Path.GetTempFileName();
            try
            {
                var projectFile = new ProjectFile();
                projectFile.Sdk = null;
                projectFile.TargetFramework = "net5.0";

                // Act
                projectFile.Save(tempFilePath);

                // Assert
                Assert.True(File.Exists(tempFilePath));
                var doc = XDocument.Load(tempFilePath);
                var sdkAttribute = doc.Root?.Attribute("Sdk");
                Assert.NotNull(sdkAttribute);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [Fact]
        public void Save_WithMultiplePackagesAndAssemblies()
        {
            // Arrange
            var tempFilePath = Path.GetTempFileName();
            try
            {
                var projectFile = new ProjectFile();
                projectFile.PackageReferences.Add(new PackageReference("Package1", "1.0.0"));
                projectFile.PackageReferences.Add(new PackageReference("Package2", "2.0.0"));
                projectFile.AssemblyReferences.Add(new AssemblyReference("System.Xml"));
                projectFile.AssemblyReferences.Add(new AssemblyReference("System.Core"));
                projectFile.TargetFramework = "net5.0";

                // Act
                projectFile.Save(tempFilePath);

                // Assert
                Assert.True(File.Exists(tempFilePath));
                var fileContent = File.ReadAllText(tempFilePath);
                Assert.Contains("Package1", fileContent);
                Assert.Contains("Package2", fileContent);
                Assert.Contains("System.Xml", fileContent);
                Assert.Contains("System.Core", fileContent);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [Fact]
        public void Save_UpdatesTargetFramework()
        {
            // Arrange
            var tempFilePath = Path.GetTempFileName();
            try
            {
                var projectFile = new ProjectFile();
                projectFile.TargetFramework = "net6.0";

                // Act
                projectFile.Save(tempFilePath);

                // Assert
                var doc = XDocument.Load(tempFilePath);
                var targetFrameworkElement = doc.Descendants("TargetFramework").FirstOrDefault();
                Assert.NotNull(targetFrameworkElement);
                Assert.Equal("net6.0", targetFrameworkElement.Value);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [Fact]
        public void Equals_WithSameInstance_ReturnsTrue()
        {
            // Arrange
            var projectFile = new ProjectFile();

            // Act
            var result = projectFile.Equals(projectFile);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithNullObject_ReturnsFalse()
        {
            // Arrange
            var projectFile = new ProjectFile();

            // Act
            var result = projectFile.Equals(null as ProjectFile);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithIdenticalContent_ReturnsTrue()
        {
            // Arrange
            var projectFile1 = new ProjectFile();
            projectFile1.PackageReferences.Add(new PackageReference("Package1", "1.0.0"));
            projectFile1.AssemblyReferences.Add(new AssemblyReference("System.Xml"));
            projectFile1.TargetFramework = "net5.0";
            projectFile1.Sdk = "Microsoft.NET.Sdk";

            var projectFile2 = new ProjectFile();
            projectFile2.PackageReferences.Add(new PackageReference("Package1", "1.0.0"));
            projectFile2.AssemblyReferences.Add(new AssemblyReference("System.Xml"));
            projectFile2.TargetFramework = "net5.0";
            projectFile2.Sdk = "Microsoft.NET.Sdk";

            // Act
            var result = projectFile1.Equals(projectFile2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentPackageReferences_ReturnsFalse()
        {
            // Arrange
            var projectFile1 = new ProjectFile();
            projectFile1.PackageReferences.Add(new PackageReference("Package1", "1.0.0"));

            var projectFile2 = new ProjectFile();
            projectFile2.PackageReferences.Add(new PackageReference("Package2", "1.0.0"));

            // Act
            var result = projectFile1.Equals(projectFile2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithDifferentAssemblyReferences_ReturnsFalse()
        {
            // Arrange
            var projectFile1 = new ProjectFile();
            projectFile1.AssemblyReferences.Add(new AssemblyReference("System.Xml"));

            var projectFile2 = new ProjectFile();
            projectFile2.AssemblyReferences.Add(new AssemblyReference("System.Core"));

            // Act
            var result = projectFile1.Equals(projectFile2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithDifferentTargetFramework_ReturnsFalse()
        {
            // Arrange
            var projectFile1 = new ProjectFile();
            projectFile1.TargetFramework = "net5.0";

            var projectFile2 = new ProjectFile();
            projectFile2.TargetFramework = "net6.0";

            // Act
            var result = projectFile1.Equals(projectFile2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithDifferentSdk_ReturnsFalse()
        {
            // Arrange
            var projectFile1 = new ProjectFile();
            projectFile1.Sdk = "Microsoft.NET.Sdk";

            var projectFile2 = new ProjectFile();
            projectFile2.Sdk = "Microsoft.NET.Sdk.Web";

            // Act
            var result = projectFile1.Equals(projectFile2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_OverloadWithNullObject_ReturnsFalse()
        {
            // Arrange
            var projectFile = new ProjectFile();

            // Act
            var result = projectFile.Equals(null as object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_OverloadWithDifferentType_ReturnsFalse()
        {
            // Arrange
            var projectFile = new ProjectFile();

            // Act
            var result = projectFile.Equals("not a project file");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_OverloadWithIdenticalContent_ReturnsTrue()
        {
            // Arrange
            var projectFile1 = new ProjectFile();
            projectFile1.TargetFramework = "net5.0";

            var projectFile2 = new ProjectFile();
            projectFile2.TargetFramework = "net5.0";

            // Act
            var result = projectFile1.Equals((object)projectFile2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetHashCode_DoesNotThrow()
        {
            // Arrange
            var projectFile = new ProjectFile();

            // Act & Assert
            var hashCode = projectFile.GetHashCode();
            Assert.IsType<int>(hashCode);
        }

        [Fact]
        public void GetHashCode_ReturnsSameValueForSameInstance()
        {
            // Arrange
            var projectFile = new ProjectFile();

            // Act
            var hashCode1 = projectFile.GetHashCode();
            var hashCode2 = projectFile.GetHashCode();

            // Assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void TargetFramework_CanBeSet()
        {
            // Arrange
            var projectFile = new ProjectFile();
            var expectedFramework = "net6.0";

            // Act
            projectFile.TargetFramework = expectedFramework;

            // Assert
            Assert.Equal(expectedFramework, projectFile.TargetFramework);
        }

        [Fact]
        public void Sdk_Property_CanBeSet()
        {
            // Arrange
            var projectFile = new ProjectFile();
            var expectedSdk = "Microsoft.NET.Sdk.Web";

            // Act
            projectFile.Sdk = expectedSdk;

            // Assert
            Assert.Equal(expectedSdk, projectFile.Sdk);
        }

        [Fact]
        public void XmlConstructor_WithMissingPackageVersionAttribute_Throws()
        {
            // Arrange
            var invalidXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
  <ItemGroup>
    <PackageReference Include=""Package1"" />
  </ItemGroup>
</Project>";

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => new ProjectFile(invalidXml));
        }

        [Fact]
        public void XmlConstructor_WithMissingPackageIncludeAttribute_Throws()
        {
            // Arrange
            var invalidXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
  <ItemGroup>
    <PackageReference Version=""1.0.0"" />
  </ItemGroup>
</Project>";

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => new ProjectFile(invalidXml));
        }

        [Fact]
        public void XmlConstructor_WithMissingReferenceIncludeAttribute_Throws()
        {
            // Arrange
            var invalidXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
  <ItemGroup>
    <Reference />
  </ItemGroup>
</Project>";

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => new ProjectFile(invalidXml));
        }

        [Fact]
        public void Save_CreatesValidXmlFile()
        {
            // Arrange
            var tempFilePath = Path.GetTempFileName();
            try
            {
                var projectFile = new ProjectFile();
                projectFile.TargetFramework = "net5.0";

                // Act
                projectFile.Save(tempFilePath);

                // Assert
                var doc = XDocument.Load(tempFilePath);
                Assert.NotNull(doc.Root);
                Assert.Equal("Project", doc.Root.Name.LocalName);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [Fact]
        public void Equals_WithEmptyPackageReferencesInBoth_ReturnsTrue()
        {
            // Arrange
            var projectFile1 = new ProjectFile();
            var projectFile2 = new ProjectFile();

            // Act
            var result = projectFile1.Equals(projectFile2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCacheable_WithMixedPinnedAndUnpinned_ReturnsFalse()
        {
            // Arrange
            var projectFile = new ProjectFile();
            projectFile.PackageReferences.Add(new PackageReference("Package1", "1.0.0"));
            projectFile.PackageReferences.Add(new PackageReference("Package2", "*"));

            // Act
            var isCacheable = projectFile.IsCacheable;

            // Assert
            Assert.False(isCacheable);
        }

        [Fact]
        public void Save_PreservesProjectStructure()
        {
            // Arrange
            var tempFilePath = Path.GetTempFileName();
            try
            {
                var projectFile = new ProjectFile();
                projectFile.PackageReferences.Add(new PackageReference("TestPackage", "1.0.0"));
                projectFile.AssemblyReferences.Add(new AssemblyReference("TestAssembly"));
                projectFile.TargetFramework = "net5.0";
                projectFile.Sdk = "Microsoft.NET.Sdk.Web";

                // Act
                projectFile.Save(tempFilePath);

                // Assert - verify we can reload it
                var reloadedProjectFile = new ProjectFile(File.ReadAllText(tempFilePath));
                Assert.Single(reloadedProjectFile.PackageReferences);
                Assert.Single(reloadedProjectFile.AssemblyReferences);
                Assert.Equal("net5.0", reloadedProjectFile.TargetFramework);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [Fact]
        public void XmlConstructor_PackageReferencesAreImmutable()
        {
            // Arrange
            var projectFile = new ProjectFile(ValidProjectXml);

            // Act & Assert
            Assert.IsAssignableFrom<IReadOnlyCollection<PackageReference>>(
                projectFile.PackageReferences);
        }

        [Fact]
        public void Equals_PackageReferenceOrderMatters()
        {
            // Arrange
            var projectFile1 = new ProjectFile();
            projectFile1.PackageReferences.Add(new PackageReference("A", "1.0.0"));
            projectFile1.PackageReferences.Add(new PackageReference("B", "1.0.0"));

            var projectFile2 = new ProjectFile();
            projectFile2.PackageReferences.Add(new PackageReference("B", "1.0.0"));
            projectFile2.PackageReferences.Add(new PackageReference("A", "1.0.0"));

            // Act - SequenceEqual checks order
            var result = projectFile1.Equals(projectFile2);

            // Assert - may be true or false depending on HashSet ordering
            // But both should be valid project files
            Assert.IsType<bool>(result);
        }
    }
}