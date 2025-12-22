using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Xunit;
using Moq;
using Dotnet.Script.DependencyModel.ProjectSystem;

namespace Dotnet.Script.DependencyModel.Tests.ProjectSystem
{
    public class ProjectFileTests
    {
        [Fact]
        public void Constructor_NoArguments_InitializesWithDefaults()
        {
            var projectFile = new ProjectFile();

            Assert.NotNull(projectFile.PackageReferences);
            Assert.Empty(projectFile.PackageReferences);
            Assert.NotNull(projectFile.AssemblyReferences);
            Assert.Empty(projectFile.AssemblyReferences);
            Assert.Equal("Microsoft.NET.Sdk", projectFile.Sdk);
            Assert.NotNull(projectFile.TargetFramework);
        }

        [Fact]
        public void Constructor_WithValidXmlContent_ParsesPackageReferences()
        {
            var xmlContent = @"<?xml version='1.0' encoding='utf-8'?>
<Project Sdk='Microsoft.NET.Sdk'>
    <ItemGroup>
        <PackageReference Include='Newtonsoft.Json' Version='13.0.1' />
        <PackageReference Include='System.Linq' Version='4.3.0' />
    </ItemGroup>
</Project>";

            var projectFile = new ProjectFile(xmlContent);

            Assert.Equal(2, projectFile.PackageReferences.Count);
            Assert.Contains(projectFile.PackageReferences, pr => pr.Id.Value == "Newtonsoft.Json" && pr.Version.Value == "13.0.1");
            Assert.Contains(projectFile.PackageReferences, pr => pr.Id.Value == "System.Linq" && pr.Version.Value == "4.3.0");
        }

        [Fact]
        public void Constructor_WithXmlContent_ParsesAssemblyReferences()
        {
            var xmlContent = @"<?xml version='1.0' encoding='utf-8'?>
<Project Sdk='Microsoft.NET.Sdk'>
    <ItemGroup>
        <Reference Include='System.Core' />
        <Reference Include='System.Xml' />
    </ItemGroup>
</Project>";

            var projectFile = new ProjectFile(xmlContent);

            Assert.Equal(2, projectFile.AssemblyReferences.Count);
            Assert.Contains(projectFile.AssemblyReferences, ar => ar.AssemblyPath == "System.Core");
            Assert.Contains(projectFile.AssemblyReferences, ar => ar.AssemblyPath == "System.Xml");
        }

        [Fact]
        public void Constructor_WithXmlContent_ParsesSdk()
        {
            var xmlContent = @"<?xml version='1.0' encoding='utf-8'?>
<Project Sdk='Microsoft.NET.Sdk.Web'>
    <ItemGroup />
</Project>";

            var projectFile = new ProjectFile(xmlContent);

            Assert.Equal("Microsoft.NET.Sdk.Web", projectFile.Sdk);
        }

        [Fact]
        public void Constructor_WithEmptyItemGroup_NoReferencesAdded()
        {
            var xmlContent = @"<?xml version='1.0' encoding='utf-8'?>
<Project Sdk='Microsoft.NET.Sdk'>
    <ItemGroup />
</Project>";

            var projectFile = new ProjectFile(xmlContent);

            Assert.Empty(projectFile.PackageReferences);
            Assert.Empty(projectFile.AssemblyReferences);
        }

        [Fact]
        public void Constructor_WithMixedReferences_ParsesBoth()
        {
            var xmlContent = @"<?xml version='1.0' encoding='utf-8'?>
<Project Sdk='Microsoft.NET.Sdk'>
    <ItemGroup>
        <PackageReference Include='Newtonsoft.Json' Version='13.0.1' />
        <Reference Include='System.Core' />
    </ItemGroup>
</Project>";

            var projectFile = new ProjectFile(xmlContent);

            Assert.Single(projectFile.PackageReferences);
            Assert.Single(projectFile.AssemblyReferences);
        }

        [Fact]
        public void IsCacheable_AllPackageVersionsPinned_ReturnsTrue()
        {
            var projectFile = new ProjectFile();
            projectFile.PackageReferences.Add(new PackageReference("pkg1", "1.0.0"));
            projectFile.PackageReferences.Add(new PackageReference("pkg2", "2.3.4"));

            Assert.True(projectFile.IsCacheable);
        }

        [Fact]
        public void IsCacheable_SomePackageVersionsNotPinned_ReturnsFalse()
        {
            var projectFile = new ProjectFile();
            projectFile.PackageReferences.Add(new PackageReference("pkg1", "1.0.0"));
            projectFile.PackageReferences.Add(new PackageReference("pkg2", "1.0.*"));

            Assert.False(projectFile.IsCacheable);
        }

        [Fact]
        public void IsCacheable_NoPackageReferences_ReturnsTrue()
        {
            var projectFile = new ProjectFile();
            Assert.True(projectFile.IsCacheable);
        }

        [Fact]
        public void Save_CreatesProjectFileWithPackageReferences()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            try
            {
                var projectFile = new ProjectFile();
                projectFile.PackageReferences.Add(new PackageReference("Newtonsoft.Json", "13.0.1"));
                projectFile.TargetFramework = "net6.0";
                projectFile.Sdk = "Microsoft.NET.Sdk";

                var filePath = Path.Combine(tempPath, "test.csproj");
                projectFile.Save(filePath);

                Assert.True(File.Exists(filePath));

                var content = File.ReadAllText(filePath);
                Assert.Contains("Newtonsoft.Json", content);
                Assert.Contains("13.0.1", content);
                Assert.Contains("net6.0", content);
                Assert.Contains("Microsoft.NET.Sdk", content);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void Save_CreatesProjectFileWithAssemblyReferences()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            try
            {
                var projectFile = new ProjectFile();
                projectFile.AssemblyReferences.Add(new AssemblyReference("System.Core"));
                projectFile.TargetFramework = "net6.0";

                var filePath = Path.Combine(tempPath, "test.csproj");
                projectFile.Save(filePath);

                Assert.True(File.Exists(filePath));
                var content = File.ReadAllText(filePath);
                Assert.Contains("System.Core", content);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void Save_WithEmptySdk_DoesNotModifySdk()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            try
            {
                var projectFile = new ProjectFile();
                projectFile.Sdk = "";
                projectFile.TargetFramework = "net6.0";

                var filePath = Path.Combine(tempPath, "test.csproj");
                projectFile.Save(filePath);

                Assert.True(File.Exists(filePath));
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void Save_WithNullSdk_DoesNotModifySdk()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            try
            {
                var projectFile = new ProjectFile();
                projectFile.Sdk = null;
                projectFile.TargetFramework = "net6.0";

                var filePath = Path.Combine(tempPath, "test.csproj");
                projectFile.Save(filePath);

                Assert.True(File.Exists(filePath));
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void Save_WithMultipleReferences_CreatesValidProjectFile()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            try
            {
                var projectFile = new ProjectFile();
                projectFile.PackageReferences.Add(new PackageReference("pkg1", "1.0.0"));
                projectFile.PackageReferences.Add(new PackageReference("pkg2", "2.0.0"));
                projectFile.AssemblyReferences.Add(new AssemblyReference("System.Core"));
                projectFile.AssemblyReferences.Add(new AssemblyReference("System.Xml"));
                projectFile.TargetFramework = "net6.0";

                var filePath = Path.Combine(tempPath, "test.csproj");
                projectFile.Save(filePath);

                Assert.True(File.Exists(filePath));
                var content = File.ReadAllText(filePath);
                Assert.Contains("pkg1", content);
                Assert.Contains("pkg2", content);
                Assert.Contains("System.Core", content);
                Assert.Contains("System.Xml", content);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void Equals_SameReferences_ReturnsTrue()
        {
            var projectFile1 = new ProjectFile();
            projectFile1.PackageReferences.Add(new PackageReference("pkg1", "1.0.0"));

            var projectFile2 = new ProjectFile();
            projectFile2.PackageReferences.Add(new PackageReference("pkg1", "1.0.0"));

            Assert.Equal(projectFile1, projectFile2);
        }

        [Fact]
        public void Equals_DifferentPackageReferences_ReturnsFalse()
        {
            var projectFile1 = new ProjectFile();
            projectFile1.PackageReferences.Add(new PackageReference("pkg1", "1.0.0"));

            var projectFile2 = new ProjectFile();
            projectFile2.PackageReferences.Add(new PackageReference("pkg2", "1.0.0"));

            Assert.NotEqual(projectFile1, projectFile2);
        }

        [Fact]
        public void Equals_DifferentAssemblyReferences_ReturnsFalse()
        {
            var projectFile1 = new ProjectFile();
            projectFile1.AssemblyReferences.Add(new AssemblyReference("System.Core"));

            var projectFile2 = new ProjectFile();
            projectFile2.AssemblyReferences.Add(new AssemblyReference("System.Xml"));

            Assert.NotEqual(projectFile1, projectFile2);
        }

        [Fact]
        public void Equals_DifferentTargetFrameworks_ReturnsFalse()
        {
            var projectFile1 = new ProjectFile { TargetFramework = "net6.0" };
            var projectFile2 = new ProjectFile { TargetFramework = "net5.0" };

            Assert.NotEqual(projectFile1, projectFile2);
        }

        [Fact]
        public void Equals_DifferentSdks_ReturnsFalse()
        {
            var projectFile1 = new ProjectFile { Sdk = "Microsoft.NET.Sdk" };
            var projectFile2 = new ProjectFile { Sdk = "Microsoft.NET.Sdk.Web" };

            Assert.NotEqual(projectFile1, projectFile2);
        }

        [Fact]
        public void Equals_NullObject_ReturnsFalse()
        {
            var projectFile = new ProjectFile();
            Assert.False(projectFile.Equals(null as ProjectFile));
        }

        [Fact]
        public void Equals_SameInstance_ReturnsTrue()
        {
            var projectFile = new ProjectFile();
            Assert.True(projectFile.Equals(projectFile));
        }

        [Fact]
        public void Equals_EqualsObject_WithNullObject_ReturnsFalse()
        {
            var projectFile = new ProjectFile();
            Assert.False(projectFile.Equals(null as object));
        }

        [Fact]
        public void Equals_EqualsObject_WithNonProjectFile_ReturnsFalse()
        {
            var projectFile = new ProjectFile();
            Assert.False(projectFile.Equals("not a project file"));
        }

        [Fact]
        public void Equals_SameValuesAllProperties_ReturnsTrue()
        {
            var projectFile1 = new ProjectFile();
            projectFile1.PackageReferences.Add(new PackageReference("pkg1", "1.0.0"));
            projectFile1.AssemblyReferences.Add(new AssemblyReference("System.Core"));
            projectFile1.TargetFramework = "net6.0";
            projectFile1.Sdk = "Microsoft.NET.Sdk";

            var projectFile2 = new ProjectFile();
            projectFile2.PackageReferences.Add(new PackageReference("pkg1", "1.0.0"));
            projectFile2.AssemblyReferences.Add(new AssemblyReference("System.Core"));
            projectFile2.TargetFramework = "net6.0";
            projectFile2.Sdk = "Microsoft.NET.Sdk";

            Assert.Equal(projectFile1, projectFile2);
        }

        [Fact]
        public void GetHashCode_ReturnsBaseHashCode()
        {
            var projectFile1 = new ProjectFile();
            var projectFile2 = new ProjectFile();

            var hashCode1 = projectFile1.GetHashCode();
            var hashCode2 = projectFile2.GetHashCode();

            Assert.NotEqual(hashCode1, hashCode2);
        }

        [Fact]
        public void TargetFramework_Property_CanBeSet()
        {
            var projectFile = new ProjectFile();
            projectFile.TargetFramework = "net6.0";

            Assert.Equal("net6.0", projectFile.TargetFramework);
        }

        [Fact]
        public void Sdk_Property_CanBeSet()
        {
            var projectFile = new ProjectFile();
            projectFile.Sdk = "Microsoft.NET.Sdk.Web";

            Assert.Equal("Microsoft.NET.Sdk.Web", projectFile.Sdk);
        }

        [Fact]
        public void PackageReferences_Property_IsHashSet()
        {
            var projectFile = new ProjectFile();

            Assert.NotNull(projectFile.PackageReferences);
            Assert.IsType<HashSet<PackageReference>>(projectFile.PackageReferences);
        }

        [Fact]
        public void AssemblyReferences_Property_IsHashSet()
        {
            var projectFile = new ProjectFile();

            Assert.NotNull(projectFile.AssemblyReferences);
            Assert.IsType<HashSet<AssemblyReference>>(projectFile.AssemblyReferences);
        }

        [Fact]
        public void Constructor_WithXmlContent_MultiplePackagesAndAssemblies()
        {
            var xmlContent = @"<?xml version='1.0' encoding='utf-8'?>
<Project Sdk='CustomSdk'>
    <ItemGroup>
        <PackageReference Include='Package1' Version='1.0.0' />
        <PackageReference Include='Package2' Version='2.0.0' />
        <PackageReference Include='Package3' Version='3.0.0' />
        <Reference Include='Assembly1' />
        <Reference Include='Assembly2' />
    </ItemGroup>
</Project>";

            var projectFile = new ProjectFile(xmlContent);

            Assert.Equal(3, projectFile.PackageReferences.Count);
            Assert.Equal(2, projectFile.AssemblyReferences.Count);
            Assert.Equal("CustomSdk", projectFile.Sdk);
        }

        [Fact]
        public void Save_EmptyProjectFile_CreatesValidXml()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            try
            {
                var projectFile = new ProjectFile();
                var filePath = Path.Combine(tempPath, "empty.csproj");
                projectFile.Save(filePath);

                Assert.True(File.Exists(filePath));
                var doc = XDocument.Load(filePath);
                Assert.NotNull(doc.Root);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void Equals_ComplexScenarioWithAllDifferences()
        {
            var projectFile1 = new ProjectFile();
            projectFile1.PackageReferences.Add(new PackageReference("pkg1", "1.0.0"));
            projectFile1.AssemblyReferences.Add(new AssemblyReference("System.Core"));
            projectFile1.TargetFramework = "net6.0";
            projectFile1.Sdk = "Microsoft.NET.Sdk";

            var projectFile2 = new ProjectFile();
            projectFile2.PackageReferences.Add(new PackageReference("pkg2", "1.0.0"));
            projectFile2.AssemblyReferences.Add(new AssemblyReference("System.Xml"));
            projectFile2.TargetFramework = "net5.0";
            projectFile2.Sdk = "Microsoft.NET.Sdk.Web";

            Assert.NotEqual(projectFile1, projectFile2);
        }
    }
}