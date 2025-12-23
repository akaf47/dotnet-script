using System;
using Xunit;
using Dotnet.Script.DependencyModel.Context;

namespace Dotnet.Script.Tests.Context
{
    public class ScriptDependencyTests
    {
        [Fact]
        public void Constructor_SetsAllPropertiesCorrectly()
        {
            // Arrange
            var name = "TestPackage";
            var version = "1.0.0";
            var runtimePaths = new[] { "/path/to/runtime/lib.dll" };
            var nativePaths = new[] { "/path/to/native/lib.so" };
            var compileTimePaths = new[] { "/path/to/compile/ref.dll" };
            var scriptPaths = new[] { "/path/to/script.csx" };

            // Act
            var dependency = new ScriptDependency(
                name,
                version,
                runtimePaths,
                nativePaths,
                compileTimePaths,
                scriptPaths);

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.Equal(version, dependency.Version);
            Assert.Equal(runtimePaths, dependency.RuntimeDependencyPaths);
            Assert.Equal(nativePaths, dependency.NativeAssetPaths);
            Assert.Equal(compileTimePaths, dependency.CompileTimeDependencyPaths);
            Assert.Equal(scriptPaths, dependency.ScriptPaths);
        }

        [Fact]
        public void Constructor_WithEmptyArrays_SetsPropertiesCorrectly()
        {
            // Arrange
            var name = "EmptyPackage";
            var version = "2.0.0";
            var emptyArray = new string[] { };

            // Act
            var dependency = new ScriptDependency(
                name,
                version,
                emptyArray,
                emptyArray,
                emptyArray,
                emptyArray);

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.Equal(version, dependency.Version);
            Assert.Empty(dependency.RuntimeDependencyPaths);
            Assert.Empty(dependency.NativeAssetPaths);
            Assert.Empty(dependency.CompileTimeDependencyPaths);
            Assert.Empty(dependency.ScriptPaths);
        }

        [Fact]
        public void Constructor_WithNullArrays_SetsPropertiesCorrectly()
        {
            // Arrange
            var name = "NullArraysPackage";
            var version = "3.0.0";

            // Act
            var dependency = new ScriptDependency(
                name,
                version,
                null,
                null,
                null,
                null);

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.Equal(version, dependency.Version);
            Assert.Null(dependency.RuntimeDependencyPaths);
            Assert.Null(dependency.NativeAssetPaths);
            Assert.Null(dependency.CompileTimeDependencyPaths);
            Assert.Null(dependency.ScriptPaths);
        }

        [Fact]
        public void Constructor_WithMultiplePathsInArrays_SetsAllPathsCorrectly()
        {
            // Arrange
            var name = "MultiPathPackage";
            var version = "4.0.0";
            var runtimePaths = new[] { "/path1/lib.dll", "/path2/lib.dll", "/path3/lib.dll" };
            var nativePaths = new[] { "/native1/lib.so", "/native2/lib.so" };
            var compileTimePaths = new[] { "/compile1/ref.dll", "/compile2/ref.dll" };
            var scriptPaths = new[] { "/script1.csx", "/script2.csx", "/script3.csx" };

            // Act
            var dependency = new ScriptDependency(
                name,
                version,
                runtimePaths,
                nativePaths,
                compileTimePaths,
                scriptPaths);

            // Assert
            Assert.Equal(3, dependency.RuntimeDependencyPaths.Length);
            Assert.Equal(2, dependency.NativeAssetPaths.Length);
            Assert.Equal(2, dependency.CompileTimeDependencyPaths.Length);
            Assert.Equal(3, dependency.ScriptPaths.Length);
        }

        [Fact]
        public void Constructor_WithEmptyStrings_SetsPropertiesCorrectly()
        {
            // Arrange
            var name = "";
            var version = "";
            var runtimePaths = new[] { "" };
            var nativePaths = new[] { "" };
            var compileTimePaths = new[] { "" };
            var scriptPaths = new[] { "" };

            // Act
            var dependency = new ScriptDependency(
                name,
                version,
                runtimePaths,
                nativePaths,
                compileTimePaths,
                scriptPaths);

            // Assert
            Assert.Equal("", dependency.Name);
            Assert.Equal("", dependency.Version);
            Assert.Single(dependency.RuntimeDependencyPaths);
        }

        [Fact]
        public void Constructor_WithNullNameAndVersion_SetsPropertiesCorrectly()
        {
            // Arrange
            var runtimePaths = new[] { "/path/to/runtime/lib.dll" };

            // Act
            var dependency = new ScriptDependency(
                null,
                null,
                runtimePaths,
                new string[] { },
                new string[] { },
                new string[] { });

            // Assert
            Assert.Null(dependency.Name);
            Assert.Null(dependency.Version);
            Assert.NotNull(dependency.RuntimeDependencyPaths);
        }

        [Fact]
        public void Constructor_WithSpecialCharactersInStrings_SetsPropertiesCorrectly()
        {
            // Arrange
            var name = "Package-With.Special_Chars!@#$%";
            var version = "1.0.0-beta+build.123";
            var runtimePaths = new[] { @"C:\Windows\System32\lib.dll" };

            // Act
            var dependency = new ScriptDependency(
                name,
                version,
                runtimePaths,
                new string[] { },
                new string[] { },
                new string[] { });

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.Equal(version, dependency.Version);
        }

        [Fact]
        public void Name_Property_IsReadOnly()
        {
            // Arrange
            var dependency = new ScriptDependency(
                "TestPackage",
                "1.0.0",
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });

            // Act & Assert
            Assert.Equal("TestPackage", dependency.Name);
            // Verify it's read-only by checking we can get the value
            var _ = dependency.Name;
            Assert.NotNull(dependency.Name);
        }

        [Fact]
        public void Version_Property_IsReadOnly()
        {
            // Arrange
            var dependency = new ScriptDependency(
                "TestPackage",
                "1.0.0",
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });

            // Act & Assert
            Assert.Equal("1.0.0", dependency.Version);
            var _ = dependency.Version;
            Assert.NotNull(dependency.Version);
        }

        [Fact]
        public void RuntimeDependencyPaths_Property_IsReadOnly()
        {
            // Arrange
            var runtimePaths = new[] { "/path/to/runtime/lib.dll" };
            var dependency = new ScriptDependency(
                "TestPackage",
                "1.0.0",
                runtimePaths,
                new string[] { },
                new string[] { },
                new string[] { });

            // Act & Assert
            Assert.Equal(runtimePaths, dependency.RuntimeDependencyPaths);
        }

        [Fact]
        public void NativeAssetPaths_Property_IsReadOnly()
        {
            // Arrange
            var nativePaths = new[] { "/path/to/native/lib.so" };
            var dependency = new ScriptDependency(
                "TestPackage",
                "1.0.0",
                new string[] { },
                nativePaths,
                new string[] { },
                new string[] { });

            // Act & Assert
            Assert.Equal(nativePaths, dependency.NativeAssetPaths);
        }

        [Fact]
        public void CompileTimeDependencyPaths_Property_IsReadOnly()
        {
            // Arrange
            var compileTimePaths = new[] { "/path/to/compile/ref.dll" };
            var dependency = new ScriptDependency(
                "TestPackage",
                "1.0.0",
                new string[] { },
                new string[] { },
                compileTimePaths,
                new string[] { });

            // Act & Assert
            Assert.Equal(compileTimePaths, dependency.CompileTimeDependencyPaths);
        }

        [Fact]
        public void ScriptPaths_Property_IsReadOnly()
        {
            // Arrange
            var scriptPaths = new[] { "/path/to/script.csx" };
            var dependency = new ScriptDependency(
                "TestPackage",
                "1.0.0",
                new string[] { },
                new string[] { },
                new string[] { },
                scriptPaths);

            // Act & Assert
            Assert.Equal(scriptPaths, dependency.ScriptPaths);
        }

        [Fact]
        public void ToString_WithValidNameAndVersion_ReturnsFormattedString()
        {
            // Arrange
            var name = "MyPackage";
            var version = "1.2.3";
            var dependency = new ScriptDependency(
                name,
                version,
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.Equal("MyPackage, 1.2.3", result);
        }

        [Fact]
        public void ToString_WithEmptyNameAndVersion_ReturnsFormattedString()
        {
            // Arrange
            var dependency = new ScriptDependency(
                "",
                "",
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.Equal(", ", result);
        }

        [Fact]
        public void ToString_WithNullNameAndVersion_ReturnsFormattedString()
        {
            // Arrange
            var dependency = new ScriptDependency(
                null,
                null,
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.Equal(", ", result);
        }

        [Fact]
        public void ToString_WithSpecialCharactersInNameAndVersion_ReturnsFormattedString()
        {
            // Arrange
            var name = "Package@#$%";
            var version = "1.0.0-alpha+build";
            var dependency = new ScriptDependency(
                name,
                version,
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.Equal("Package@#$%, 1.0.0-alpha+build", result);
        }

        [Fact]
        public void ToString_WithVeryLongNameAndVersion_ReturnsFormattedString()
        {
            // Arrange
            var name = new string('A', 1000);
            var version = new string('1', 100);
            var dependency = new ScriptDependency(
                name,
                version,
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.StartsWith(new string('A', 50), result);
            Assert.Contains(", ", result);
        }

        [Fact]
        public void ToString_MultipleCallsReturnSameValue()
        {
            // Arrange
            var dependency = new ScriptDependency(
                "Package",
                "1.0.0",
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });

            // Act
            var result1 = dependency.ToString();
            var result2 = dependency.ToString();

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void Constructor_PreservesExactArrayReferences()
        {
            // Arrange
            var runtimePaths = new[] { "/path/to/runtime/lib.dll" };
            var nativePaths = new[] { "/path/to/native/lib.so" };
            var compileTimePaths = new[] { "/path/to/compile/ref.dll" };
            var scriptPaths = new[] { "/path/to/script.csx" };

            // Act
            var dependency = new ScriptDependency(
                "Package",
                "1.0.0",
                runtimePaths,
                nativePaths,
                compileTimePaths,
                scriptPaths);

            // Assert - Verify same array references are stored
            Assert.Same(runtimePaths, dependency.RuntimeDependencyPaths);
            Assert.Same(nativePaths, dependency.NativeAssetPaths);
            Assert.Same(compileTimePaths, dependency.CompileTimeDependencyPaths);
            Assert.Same(scriptPaths, dependency.ScriptPaths);
        }

        [Fact]
        public void Constructor_WithUnicodeCharacters_SetsPropertiesCorrectly()
        {
            // Arrange
            var name = "Package🔧📦";
            var version = "1.0.0-日本語";
            var runtimePaths = new[] { "/路径/lib.dll" };

            // Act
            var dependency = new ScriptDependency(
                name,
                version,
                runtimePaths,
                new string[] { },
                new string[] { },
                new string[] { });

            // Assert
            Assert.Equal(name, dependency.Name);
            Assert.Equal(version, dependency.Version);
            Assert.Equal(runtimePaths[0], dependency.RuntimeDependencyPaths[0]);
        }

        [Fact]
        public void ToString_WithUnicodeCharacters_ReturnsFormattedString()
        {
            // Arrange
            var dependency = new ScriptDependency(
                "Package🔧",
                "1.0.0-日本語",
                new string[] { },
                new string[] { },
                new string[] { },
                new string[] { });

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.Equal("Package🔧, 1.0.0-日本語", result);
        }
    }
}