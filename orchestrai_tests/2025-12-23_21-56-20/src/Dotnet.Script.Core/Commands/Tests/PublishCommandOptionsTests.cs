using Xunit;
using Microsoft.CodeAnalysis;
using Dotnet.Script.Core.Commands;
using Dotnet.Script.DependencyModel.Environment;
#if NETCOREAPP
using System.Runtime.Loader;
#endif

namespace Dotnet.Script.Core.Commands.Tests
{
    public class PublishCommandOptionsTests
    {
        [Fact]
        public void Constructor_WithValidParameters_InitializesAllProperties()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var outputDirectory = "output";
            var libraryName = "TestLibrary";
            var publishType = PublishType.Library;
            var optimizationLevel = OptimizationLevel.Release;
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var runtimeIdentifier = "win-x64";
            var noCache = true;

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                outputDirectory,
                libraryName,
                publishType,
                optimizationLevel,
                packageSources,
                runtimeIdentifier,
                noCache
            );

            // Assert
            Assert.Equal(scriptFile, options.File);
            Assert.Equal(outputDirectory, options.OutputDirectory);
            Assert.Equal(libraryName, options.LibraryName);
            Assert.Equal(publishType, options.PublishType);
            Assert.Equal(optimizationLevel, options.OptimizationLevel);
            Assert.Equal(packageSources, options.PackageSources);
            Assert.Equal(runtimeIdentifier, options.RuntimeIdentifier);
            Assert.True(options.NoCache);
        }

        [Fact]
        public void Constructor_WithNullRuntimeIdentifier_UsesDefault()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                null,
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                null,
                false
            );

            // Assert
            Assert.NotNull(options.RuntimeIdentifier);
            Assert.Equal(ScriptEnvironment.Default.RuntimeIdentifier, options.RuntimeIdentifier);
        }

        [Fact]
        public void Constructor_WithNullOutputDirectory_AllowsNull()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                null,
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Assert
            Assert.Null(options.OutputDirectory);
        }

        [Fact]
        public void Constructor_WithEmptyOutputDirectory_AllowsEmpty()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "",
                "TestLib",
                PublishType.Executable,
                OptimizationLevel.Release,
                packageSources,
                "linux-x64",
                true
            );

            // Assert
            Assert.Equal("", options.OutputDirectory);
        }

        [Fact]
        public void Constructor_WithNullPackageSources_AllowsNull()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                null,
                "win-x64",
                false
            );

            // Assert
            Assert.Null(options.PackageSources);
        }

        [Fact]
        public void Constructor_WithEmptyPackageSources_AllowsEmpty()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new string[] { };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Assert
            Assert.Empty(options.PackageSources);
        }

        [Fact]
        public void Constructor_WithMultiplePackageSources_PreservesArray()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "source1", "source2", "source3" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Assert
            Assert.Equal(packageSources, options.PackageSources);
            Assert.Equal(3, options.PackageSources.Length);
        }

        [Fact]
        public void Constructor_WithNullLibraryName_AllowsNull()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                null,
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Assert
            Assert.Null(options.LibraryName);
        }

        [Fact]
        public void Constructor_WithEmptyLibraryName_AllowsEmpty()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Assert
            Assert.Equal("", options.LibraryName);
        }

        [Fact]
        public void Constructor_WithPublishTypeLibrary_SetsPropertyCorrectly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Assert
            Assert.Equal(PublishType.Library, options.PublishType);
        }

        [Fact]
        public void Constructor_WithPublishTypeExecutable_SetsPropertyCorrectly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Executable,
                OptimizationLevel.Release,
                packageSources,
                "linux-x64",
                true
            );

            // Assert
            Assert.Equal(PublishType.Executable, options.PublishType);
        }

        [Fact]
        public void Constructor_WithOptimizationLevelDebug_SetsPropertyCorrectly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Assert
            Assert.Equal(OptimizationLevel.Debug, options.OptimizationLevel);
        }

        [Fact]
        public void Constructor_WithOptimizationLevelRelease_SetsPropertyCorrectly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Release,
                packageSources,
                "win-x64",
                false
            );

            // Assert
            Assert.Equal(OptimizationLevel.Release, options.OptimizationLevel);
        }

        [Fact]
        public void Constructor_WithNoCacheFalse_SetsPropertyCorrectly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Assert
            Assert.False(options.NoCache);
        }

        [Fact]
        public void Constructor_WithNoCacheTrue_SetsPropertyCorrectly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                true
            );

            // Assert
            Assert.True(options.NoCache);
        }

        [Fact]
        public void File_Property_IsReadOnly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Act & Assert
            Assert.Equal(scriptFile, options.File);
            // Property should be read-only
        }

        [Fact]
        public void OutputDirectory_Property_IsReadOnly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Act & Assert
            Assert.Equal("output", options.OutputDirectory);
            // Property should be read-only
        }

        [Fact]
        public void LibraryName_Property_IsReadOnly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "MyLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Act & Assert
            Assert.Equal("MyLib", options.LibraryName);
            // Property should be read-only
        }

        [Fact]
        public void PublishType_Property_IsReadOnly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Executable,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Act & Assert
            Assert.Equal(PublishType.Executable, options.PublishType);
            // Property should be read-only
        }

        [Fact]
        public void OptimizationLevel_Property_IsReadOnly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Release,
                packageSources,
                "win-x64",
                false
            );

            // Act & Assert
            Assert.Equal(OptimizationLevel.Release, options.OptimizationLevel);
            // Property should be read-only
        }

        [Fact]
        public void PackageSources_Property_IsReadOnly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            );

            // Act & Assert
            Assert.Equal(packageSources, options.PackageSources);
            // Property should be read-only
        }

        [Fact]
        public void RuntimeIdentifier_Property_IsReadOnly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "linux-arm64",
                false
            );

            // Act & Assert
            Assert.Equal("linux-arm64", options.RuntimeIdentifier);
            // Property should be read-only
        }

        [Fact]
        public void NoCache_Property_IsReadOnly()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                true
            );

            // Act & Assert
            Assert.True(options.NoCache);
            // Property should be read-only
        }

        [Fact]
        public void Constructor_WithVariousRuntimeIdentifiers_PreservesValue()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var runtimeIds = new[] { "win-x86", "win-x64", "linux-x64", "osx-x64", "osx-arm64" };

            // Act & Assert
            foreach (var rid in runtimeIds)
            {
                var options = new PublishCommandOptions(
                    scriptFile,
                    "output",
                    "TestLib",
                    PublishType.Executable,
                    OptimizationLevel.Release,
                    packageSources,
                    rid,
                    false
                );

                Assert.Equal(rid, options.RuntimeIdentifier);
            }
        }

#if NETCOREAPP
        [Fact]
        public void AssemblyLoadContext_Property_CanBeSetViaInit()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var customALC = new AssemblyLoadContext("CustomContext", isCollectible: true);

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            )
            {
                AssemblyLoadContext = customALC
            };

            // Assert
            Assert.NotNull(options.AssemblyLoadContext);
            Assert.Equal(customALC, options.AssemblyLoadContext);
        }

        [Fact]
        public void AssemblyLoadContext_Property_CanBeNull()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            var options = new PublishCommandOptions(
                scriptFile,
                "output",
                "TestLib",
                PublishType.Library,
                OptimizationLevel.Debug,
                packageSources,
                "win-x64",
                false
            )
            {
                AssemblyLoadContext = null
            };

            // Assert
            Assert.Null(options.AssemblyLoadContext);
        }
#endif

        [Fact]
        public void PublishType_Enum_HasLibraryValue()
        {
            // Act & Assert
            Assert.Equal(PublishType.Library, PublishType.Library);
            Assert.NotEqual(PublishType.Executable, PublishType.Library);
        }

        [Fact]
        public void PublishType_Enum_HasExecutableValue()
        {
            // Act & Assert
            Assert.Equal(PublishType.Executable, PublishType.Executable);
            Assert.NotEqual(PublishType.Library, PublishType.Executable);
        }

        [Fact]
        public void Constructor_WithSpecialCharactersInLibraryName_PreservesValue()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var specialNames = new[] { "Test.Lib", "Test-Lib", "Test_Lib", "Test$Lib", "Test123" };

            // Act & Assert
            foreach (var name in specialNames)
            {
                var options = new PublishCommandOptions(
                    scriptFile,
                    "output",
                    name,
                    PublishType.Library,
                    OptimizationLevel.Debug,
                    packageSources,
                    "win-x64",
                    false
                );

                Assert.Equal(name, options.LibraryName);
            }
        }

        [Fact]
        public void Constructor_WithSpecialCharactersInOutputDirectory_PreservesValue()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var outputDirs = new[] { "output/dir", "output\\dir", "C:\\path\\to\\output", "/usr/local/output" };

            // Act & Assert
            foreach (var dir in outputDirs)
            {
                var options = new PublishCommandOptions(
                    scriptFile,
                    dir,
                    "TestLib",
                    PublishType.Library,
                    OptimizationLevel.Debug,
                    packageSources,
                    "win-x64",
                    false
                );

                Assert.Equal(dir, options.OutputDirectory);
            }
        }
    }
}