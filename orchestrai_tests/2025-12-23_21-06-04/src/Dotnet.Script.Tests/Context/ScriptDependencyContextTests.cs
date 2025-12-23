using System;
using System.Linq;
using Xunit;
using Dotnet.Script.DependencyModel.Context;

namespace Dotnet.Script.Tests.Context
{
    public class ScriptDependencyContextTests
    {
        [Fact]
        public void Constructor_WithSingleDependency_SetsDependenciesCorrectly()
        {
            // Arrange
            var dependency = new ScriptDependency(
                "Package1",
                "1.0.0",
                new[] { "/path/to/lib1.dll" },
                new string[] { },
                new string[] { },
                new string[] { });
            var dependencies = new[] { dependency };

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.NotNull(context.Dependencies);
            Assert.Single(context.Dependencies);
            Assert.Equal(dependency, context.Dependencies[0]);
        }

        [Fact]
        public void Constructor_WithMultipleDependencies_SetsDependenciesCorrectly()
        {
            // Arrange
            var dependency1 = new ScriptDependency(
                "Package1",
                "1.0.0",
                new[] { "/path/to/lib1.dll" },
                new string[] { },
                new string[] { },
                new string[] { });
            var dependency2 = new ScriptDependency(
                "Package2",
                "2.0.0",
                new[] { "/path/to/lib2.dll" },
                new string[] { },
                new string[] { },
                new string[] { });
            var dependency3 = new ScriptDependency(
                "Package3",
                "3.0.0",
                new[] { "/path/to/lib3.dll" },
                new string[] { },
                new string[] { },
                new string[] { });
            var dependencies = new[] { dependency1, dependency2, dependency3 };

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.NotNull(context.Dependencies);
            Assert.Equal(3, context.Dependencies.Length);
            Assert.Equal(dependency1, context.Dependencies[0]);
            Assert.Equal(dependency2, context.Dependencies[1]);
            Assert.Equal(dependency3, context.Dependencies[2]);
        }

        [Fact]
        public void Constructor_WithEmptyArray_SetsDependenciesCorrectly()
        {
            // Arrange
            var dependencies = new ScriptDependency[] { };

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.NotNull(context.Dependencies);
            Assert.Empty(context.Dependencies);
            Assert.Equal(0, context.Dependencies.Length);
        }

        [Fact]
        public void Constructor_WithNullArray_SetsDependenciesCorrectly()
        {
            // Arrange
            ScriptDependency[] dependencies = null;

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.Null(context.Dependencies);
        }

        [Fact]
        public void Dependencies_Property_IsReadOnly()
        {
            // Arrange
            var dependency = new ScriptDependency(
                "Package1",
                "1.0.0",
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });
            var dependencies = new[] { dependency };
            var context = new ScriptDependencyContext(dependencies);

            // Act & Assert
            Assert.NotNull(context.Dependencies);
            var _ = context.Dependencies;
            Assert.Single(context.Dependencies);
        }

        [Fact]
        public void Constructor_PreservesExactArrayReference()
        {
            // Arrange
            var dependency1 = new ScriptDependency(
                "Package1",
                "1.0.0",
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });
            var dependencies = new[] { dependency1 };

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.Same(dependencies, context.Dependencies);
        }

        [Fact]
        public void Constructor_WithLargeDependencyArray_SetsDependenciesCorrectly()
        {
            // Arrange
            var dependencies = new ScriptDependency[100];
            for (int i = 0; i < 100; i++)
            {
                dependencies[i] = new ScriptDependency(
                    $"Package{i}",
                    $"{i}.0.0",
                    new string[] { },
                    new string[] { },
                    new string[] { },
                    new string[] { });
            }

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.NotNull(context.Dependencies);
            Assert.Equal(100, context.Dependencies.Length);
            for (int i = 0; i < 100; i++)
            {
                Assert.Equal($"Package{i}", context.Dependencies[i].Name);
            }
        }

        [Fact]
        public void Constructor_WithDependenciesContainingComplexPaths_SetsDependenciesCorrectly()
        {
            // Arrange
            var runtimePaths = new[] 
            { 
                "/usr/local/lib/package1.dll",
                @"C:\Windows\System32\package1.dll",
                "/home/user/.nuget/packages/package1/1.0.0/lib/net6.0/package1.dll"
            };
            var nativePaths = new[] 
            { 
                "/usr/lib/x86_64-linux-gnu/libpackage1.so",
                "/opt/homebrew/lib/libpackage1.dylib"
            };
            var dependency = new ScriptDependency(
                "ComplexPackage",
                "1.0.0",
                runtimePaths,
                nativePaths,
                new string[] { },
                new string[] { });
            var dependencies = new[] { dependency };

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.NotNull(context.Dependencies);
            Assert.Single(context.Dependencies);
            Assert.Equal(3, context.Dependencies[0].RuntimeDependencyPaths.Length);
            Assert.Equal(2, context.Dependencies[0].NativeAssetPaths.Length);
        }

        [Fact]
        public void Constructor_WithDependenciesWithEmptyNameAndVersion_SetsDependenciesCorrectly()
        {
            // Arrange
            var dependency = new ScriptDependency(
                "",
                "",
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });
            var dependencies = new[] { dependency };

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.NotNull(context.Dependencies);
            Assert.Single(context.Dependencies);
            Assert.Equal("", context.Dependencies[0].Name);
            Assert.Equal("", context.Dependencies[0].Version);
        }

        [Fact]
        public void Constructor_WithDependenciesWithNullNameAndVersion_SetsDependenciesCorrectly()
        {
            // Arrange
            var dependency = new ScriptDependency(
                null,
                null,
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });
            var dependencies = new[] { dependency };

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.NotNull(context.Dependencies);
            Assert.Single(context.Dependencies);
            Assert.Null(context.Dependencies[0].Name);
            Assert.Null(context.Dependencies[0].Version);
        }

        [Fact]
        public void Constructor_WithMixedDependencies_SetsDependenciesCorrectly()
        {
            // Arrange
            var dep1 = new ScriptDependency(
                "Package1",
                "1.0.0",
                new[] { "/path1.dll" },
                new string[] { },
                new string[] { },
                new string[] { });
            var dep2 = new ScriptDependency(
                null,
                null,
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });
            var dep3 = new ScriptDependency(
                "Package3",
                "",
                new string[] { },
                new[] { "/native.so" },
                new string[] { },
                new string[] { });
            var dependencies = new[] { dep1, dep2, dep3 };

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.NotNull(context.Dependencies);
            Assert.Equal(3, context.Dependencies.Length);
            Assert.Equal("Package1", context.Dependencies[0].Name);
            Assert.Null(context.Dependencies[1].Name);
            Assert.Equal("Package3", context.Dependencies[2].Name);
        }

        [Fact]
        public void Constructor_WithDependenciesAccessingPropertiesMultipleTimes_ReturnsSameValues()
        {
            // Arrange
            var dependency = new ScriptDependency(
                "Package1",
                "1.0.0",
                new[] { "/path/to/lib1.dll" },
                new string[] { },
                new string[] { },
                new string[] { });
            var dependencies = new[] { dependency };
            var context = new ScriptDependencyContext(dependencies);

            // Act
            var deps1 = context.Dependencies;
            var deps2 = context.Dependencies;
            var deps3 = context.Dependencies;

            // Assert
            Assert.Same(deps1, deps2);
            Assert.Same(deps2, deps3);
            Assert.Equal(deps1[0].Name, deps2[0].Name);
        }

        [Fact]
        public void Dependencies_Property_CanBeEnumerated()
        {
            // Arrange
            var dep1 = new ScriptDependency("Package1", "1.0.0", new string[] { }, new string[] { }, new string[] { }, new string[] { });
            var dep2 = new ScriptDependency("Package2", "2.0.0", new string[] { }, new string[] { }, new string[] { }, new string[] { });
            var dep3 = new ScriptDependency("Package3", "3.0.0", new string[] { }, new string[] { }, new string[] { }, new string[] { });
            var dependencies = new[] { dep1, dep2, dep3 };
            var context = new ScriptDependencyContext(dependencies);

            // Act
            var names = context.Dependencies.Select(d => d.Name).ToList();

            // Assert
            Assert.Equal(3, names.Count);
            Assert.Contains("Package1", names);
            Assert.Contains("Package2", names);
            Assert.Contains("Package3", names);
        }

        [Fact]
        public void Dependencies_Property_CanBeAccessedByIndex()
        {
            // Arrange
            var dep1 = new ScriptDependency("Package1", "1.0.0", new string[] { }, new string[] { }, new string[] { }, new string[] { });
            var dep2 = new ScriptDependency("Package2", "2.0.0", new string[] { }, new string[] { }, new string[] { }, new string[] { });
            var dependencies = new[] { dep1, dep2 };
            var context = new ScriptDependencyContext(dependencies);

            // Act & Assert
            Assert.Equal("Package1", context.Dependencies[0].Name);
            Assert.Equal("Package2", context.Dependencies[1].Name);
            Assert.Equal("1.0.0", context.Dependencies[0].Version);
            Assert.Equal("2.0.0", context.Dependencies[1].Version);
        }

        [Fact]
        public void Constructor_WithDependenciesContainingUnicodeCharacters_SetsDependenciesCorrectly()
        {
            // Arrange
            var dependency = new ScriptDependency(
                "Package🔧📦",
                "1.0.0-日本語",
                new[] { "/路径/lib.dll" },
                new string[] { },
                new string[] { },
                new string[] { });
            var dependencies = new[] { dependency };

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.NotNull(context.Dependencies);
            Assert.Single(context.Dependencies);
            Assert.Equal("Package🔧📦", context.Dependencies[0].Name);
            Assert.Equal("1.0.0-日本語", context.Dependencies[0].Version);
        }

        [Fact]
        public void Constructor_WithDependenciesWithSpecialCharacters_SetsDependenciesCorrectly()
        {
            // Arrange
            var dependency = new ScriptDependency(
                "Package-With.Special_Chars!@#$%",
                "1.0.0-alpha+build.123",
                new[] { @"C:\Windows\System32\lib.dll" },
                new string[] { },
                new string[] { },
                new string[] { });
            var dependencies = new[] { dependency };

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.NotNull(context.Dependencies);
            Assert.Single(context.Dependencies);
            Assert.Equal("Package-With.Special_Chars!@#$%", context.Dependencies[0].Name);
        }

        [Fact]
        public void Constructor_WithArrayOfDifferentSizedDependencies_SetsDependenciesCorrectly()
        {
            // Arrange
            var dep1 = new ScriptDependency(
                "Small",
                "1.0.0",
                new[] { "/path1.dll" },
                new string[] { },
                new string[] { },
                new string[] { });
            var dep2 = new ScriptDependency(
                "Large",
                "2.0.0",
                new[] { "/path1.dll", "/path2.dll", "/path3.dll", "/path4.dll", "/path5.dll" },
                new[] { "/native1.so", "/native2.so" },
                new[] { "/compile1.dll", "/compile2.dll", "/compile3.dll" },
                new[] { "/script1.csx", "/script2.csx" });
            var dependencies = new[] { dep1, dep2 };

            // Act
            var context = new ScriptDependencyContext(dependencies);

            // Assert
            Assert.NotNull(context.Dependencies);
            Assert.Equal(2, context.Dependencies.Length);
            Assert.Single(context.Dependencies[0].RuntimeDependencyPaths);
            Assert.Equal(5, context.Dependencies[1].RuntimeDependencyPaths.Length);
        }
    }
}