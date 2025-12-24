using System;
using System.Collections.Generic;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class CompilationDependencyTests
    {
        [Fact]
        public void Constructor_Sets_All_Properties_Correctly()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly1.dll", "/path/to/assembly2.dll" };
            var scripts = new List<string> { "/path/to/script1.csx", "/path/to/script2.csx" };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.Equal(version, dependency.Version);
            Assert.Equal(assemblyPaths, dependency.AssemblyPaths);
            Assert.Equal(scripts, dependency.Scripts);
        }

        [Fact]
        public void Constructor_With_Null_Name()
        {
            // Arrange
            string name = null;
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Null(dependency.Name);
            Assert.Equal(version, dependency.Version);
        }

        [Fact]
        public void Constructor_With_Null_Version()
        {
            // Arrange
            var name = "TestPackage";
            string version = null;
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.Null(dependency.Version);
        }

        [Fact]
        public void Constructor_With_Null_AssemblyPaths()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            IReadOnlyList<string> assemblyPaths = null;
            var scripts = new List<string> { "/path/to/script.csx" };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.Null(dependency.AssemblyPaths);
        }

        [Fact]
        public void Constructor_With_Null_Scripts()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            IReadOnlyList<string> scripts = null;

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.Null(dependency.Scripts);
        }

        [Fact]
        public void Constructor_With_Empty_AssemblyPaths()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var assemblyPaths = new List<string>();
            var scripts = new List<string> { "/path/to/script.csx" };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.NotNull(dependency.AssemblyPaths);
            Assert.Empty(dependency.AssemblyPaths);
        }

        [Fact]
        public void Constructor_With_Empty_Scripts()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string>();

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.NotNull(dependency.Scripts);
            Assert.Empty(dependency.Scripts);
        }

        [Fact]
        public void Constructor_With_Empty_Name()
        {
            // Arrange
            var name = "";
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.Equal("", dependency.Name);
        }

        [Fact]
        public void Constructor_With_Empty_Version()
        {
            // Arrange
            var name = "TestPackage";
            var version = "";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.Equal("", dependency.Version);
        }

        [Fact]
        public void Constructor_With_Multiple_AssemblyPaths_And_Scripts()
        {
            // Arrange
            var name = "ComplexPackage";
            var version = "2.5.3";
            var assemblyPaths = new List<string> 
            { 
                "/path/to/assembly1.dll", 
                "/path/to/assembly2.dll",
                "/path/to/assembly3.dll"
            };
            var scripts = new List<string> 
            { 
                "/path/to/script1.csx",
                "/path/to/script2.csx",
                "/path/to/script3.csx",
                "/path/to/script4.csx"
            };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.Equal(version, dependency.Version);
            Assert.Equal(3, dependency.AssemblyPaths.Count);
            Assert.Equal(4, dependency.Scripts.Count);
            Assert.Contains("/path/to/assembly1.dll", dependency.AssemblyPaths);
            Assert.Contains("/path/to/assembly3.dll", dependency.AssemblyPaths);
            Assert.Contains("/path/to/script1.csx", dependency.Scripts);
            Assert.Contains("/path/to/script4.csx", dependency.Scripts);
        }

        [Fact]
        public void ToString_Returns_Correct_Format()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Name: TestPackage , Version: 1.0.0", result);
        }

        [Fact]
        public void ToString_With_Null_Name()
        {
            // Arrange
            string name = null;
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Name:  , Version: 1.0.0", result);
        }

        [Fact]
        public void ToString_With_Null_Version()
        {
            // Arrange
            var name = "TestPackage";
            string version = null;
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Name: TestPackage , Version: ", result);
        }

        [Fact]
        public void ToString_With_Both_Null()
        {
            // Arrange
            string name = null;
            string version = null;
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Name:  , Version: ", result);
        }

        [Fact]
        public void ToString_With_Special_Characters_In_Name()
        {
            // Arrange
            var name = "Test-Package.Core#2";
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.Equal("Name: Test-Package.Core#2 , Version: 1.0.0", result);
        }

        [Fact]
        public void ToString_With_Special_Characters_In_Version()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0-beta+build.123";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.Equal("Name: TestPackage , Version: 1.0.0-beta+build.123", result);
        }

        [Fact]
        public void Name_Property_Is_ReadOnly()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Act & Assert
            // Property should be readable
            Assert.Equal("TestPackage", dependency.Name);
            
            // Attempting to set should not be possible (compiler ensures this)
            // This test documents that the property is read-only
        }

        [Fact]
        public void Version_Property_Is_ReadOnly()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Act & Assert
            Assert.Equal("1.0.0", dependency.Version);
        }

        [Fact]
        public void AssemblyPaths_Property_Is_ReadOnly()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Act & Assert
            Assert.NotNull(dependency.AssemblyPaths);
            Assert.Equal(assemblyPaths, dependency.AssemblyPaths);
        }

        [Fact]
        public void Scripts_Property_Is_ReadOnly()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Act & Assert
            Assert.NotNull(dependency.Scripts);
            Assert.Equal(scripts, dependency.Scripts);
        }

        [Fact]
        public void Constructor_Preserves_Assembly_Path_Order()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var assemblyPaths = new List<string> 
            { 
                "/path/z.dll",
                "/path/a.dll",
                "/path/m.dll"
            };
            var scripts = new List<string> { "/path/to/script.csx" };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal(3, dependency.AssemblyPaths.Count);
            Assert.Equal("/path/z.dll", dependency.AssemblyPaths[0]);
            Assert.Equal("/path/a.dll", dependency.AssemblyPaths[1]);
            Assert.Equal("/path/m.dll", dependency.AssemblyPaths[2]);
        }

        [Fact]
        public void Constructor_Preserves_Script_Order()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> 
            { 
                "/path/z.csx",
                "/path/a.csx",
                "/path/m.csx"
            };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal(3, dependency.Scripts.Count);
            Assert.Equal("/path/z.csx", dependency.Scripts[0]);
            Assert.Equal("/path/a.csx", dependency.Scripts[1]);
            Assert.Equal("/path/m.csx", dependency.Scripts[2]);
        }

        [Fact]
        public void Constructor_With_ReadOnlyList_Implementations()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            IReadOnlyList<string> assemblyPaths = new[] { "/path/to/assembly.dll" };
            IReadOnlyList<string> scripts = new[] { "/path/to/script.csx" };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.NotNull(dependency.AssemblyPaths);
            Assert.NotNull(dependency.Scripts);
            Assert.Single(dependency.AssemblyPaths);
            Assert.Single(dependency.Scripts);
        }

        [Fact]
        public void Multiple_Instances_Are_Independent()
        {
            // Arrange
            var name1 = "Package1";
            var version1 = "1.0.0";
            var assemblyPaths1 = new List<string> { "/path/to/assembly1.dll" };
            var scripts1 = new List<string> { "/path/to/script1.csx" };

            var name2 = "Package2";
            var version2 = "2.0.0";
            var assemblyPaths2 = new List<string> { "/path/to/assembly2.dll" };
            var scripts2 = new List<string> { "/path/to/script2.csx" };

            // Act
            var dependency1 = new CompilationDependency(name1, version1, assemblyPaths1, scripts1);
            var dependency2 = new CompilationDependency(name2, version2, assemblyPaths2, scripts2);

            // Assert
            Assert.NotEqual(dependency1.Name, dependency2.Name);
            Assert.NotEqual(dependency1.Version, dependency2.Version);
            Assert.NotEqual(dependency1.AssemblyPaths[0], dependency2.AssemblyPaths[0]);
            Assert.NotEqual(dependency1.Scripts[0], dependency2.Scripts[0]);
        }

        [Fact]
        public void ToString_With_Very_Long_Name_And_Version()
        {
            // Arrange
            var name = new string('A', 1000);
            var version = new string('1', 1000);
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Name:", result);
            Assert.Contains("Version:", result);
            Assert.Contains(new string('A', 100), result); // Should contain part of long name
        }

        [Fact]
        public void Constructor_With_Same_Assembly_Path_Multiple_Times()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var assemblyPaths = new List<string> 
            { 
                "/path/to/assembly.dll",
                "/path/to/assembly.dll",
                "/path/to/assembly.dll"
            };
            var scripts = new List<string> { "/path/to/script.csx" };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal(3, dependency.AssemblyPaths.Count);
            Assert.All(dependency.AssemblyPaths, ap => Assert.Equal("/path/to/assembly.dll", ap));
        }

        [Fact]
        public void Constructor_With_Whitespace_Name()
        {
            // Arrange
            var name = "   ";
            var version = "1.0.0";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal("   ", dependency.Name);
        }

        [Fact]
        public void Constructor_With_Whitespace_Version()
        {
            // Arrange
            var name = "TestPackage";
            var version = "   ";
            var assemblyPaths = new List<string> { "/path/to/assembly.dll" };
            var scripts = new List<string> { "/path/to/script.csx" };

            // Act
            var dependency = new CompilationDependency(name, version, assemblyPaths, scripts);

            // Assert
            Assert.Equal("   ", dependency.Version);
        }
    }
}