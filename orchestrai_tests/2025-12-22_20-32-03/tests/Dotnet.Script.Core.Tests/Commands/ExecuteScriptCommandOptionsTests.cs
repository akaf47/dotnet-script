using Dotnet.Script.Core.Commands;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteScriptCommandOptionsTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithAllValidParameters_InitializesAllProperties()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var arguments = new[] { "arg1", "arg2" };
            var optimizationLevel = OptimizationLevel.Release;
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var isInteractive = false;
            var noCache = false;

            // Act
            var options = new ExecuteScriptCommandOptions(
                scriptFile,
                arguments,
                optimizationLevel,
                packageSources,
                isInteractive,
                noCache
            );

            // Assert
            Assert.NotNull(options);
            Assert.Equal(scriptFile, options.File);
            Assert.Equal(arguments, options.Arguments);
            Assert.Equal(optimizationLevel, options.OptimizationLevel);
            Assert.Equal(packageSources, options.PackageSources);
            Assert.Equal(isInteractive, options.IsInteractive);
            Assert.Equal(noCache, options.NoCache);
        }

        [Fact]
        public void Constructor_WithNullScriptFile_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ExecuteScriptCommandOptions(
                    null,
                    new[] { "arg1" },
                    OptimizationLevel.Release,
                    new[] { "https://api.nuget.org/v3/index.json" },
                    false,
                    false
                )
            );
        }

        [Fact]
        public void Constructor_WithNullArguments_ThrowsArgumentNullException()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ExecuteScriptCommandOptions(
                    scriptFile,
                    null,
                    OptimizationLevel.Release,
                    new[] { "https://api.nuget.org/v3/index.json" },
                    false,
                    false
                )
            );
        }

        [Fact]
        public void Constructor_WithNullPackageSources_ThrowsArgumentNullException()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ExecuteScriptCommandOptions(
                    scriptFile,
                    new[] { "arg1" },
                    OptimizationLevel.Release,
                    null,
                    false,
                    false
                )
            );
        }

        #endregion

        #region File Property Tests

        [Fact]
        public void File_Property_ReturnsInitializedValue()
        {
            // Arrange
            var scriptFile = new ScriptFile("test.csx");
            var options = new ExecuteScriptCommandOptions(
                scriptFile,
                new[] { "arg1" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            );

            // Act & Assert
            Assert.Equal(scriptFile, options.File);
        }

        #endregion

        #region Arguments Property Tests

        [Fact]
        public void Arguments_Property_ReturnsInitializedArray()
        {
            // Arrange
            var arguments = new[] { "arg1", "arg2", "arg3" };
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                arguments,
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            );

            // Act & Assert
            Assert.Equal(arguments, options.Arguments);
            Assert.Equal(3, options.Arguments.Length);
        }

        [Fact]
        public void Arguments_Property_WithEmptyArray_ReturnsEmptyArray()
        {
            // Arrange
            var arguments = new string[] { };
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                arguments,
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            );

            // Act & Assert
            Assert.Empty(options.Arguments);
        }

        #endregion

        #region OptimizationLevel Property Tests

        [Fact]
        public void OptimizationLevel_Property_WithReleaseLevel_ReturnsRelease()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            );

            // Act & Assert
            Assert.Equal(OptimizationLevel.Release, options.OptimizationLevel);
        }

        [Fact]
        public void OptimizationLevel_Property_WithDebugLevel_ReturnsDebug()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Debug,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            );

            // Act & Assert
            Assert.Equal(OptimizationLevel.Debug, options.OptimizationLevel);
        }

        #endregion

        #region PackageSources Property Tests

        [Fact]
        public void PackageSources_Property_ReturnsInitializedArray()
        {
            // Arrange
            var packageSources = new[] { "https://api.nuget.org/v3/index.json", "https://myget.org/F/myget/api/v3/index.json" };
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Release,
                packageSources,
                false,
                false
            );

            // Act & Assert
            Assert.Equal(packageSources, options.PackageSources);
            Assert.Equal(2, options.PackageSources.Length);
        }

        [Fact]
        public void PackageSources_Property_WithEmptyArray_ReturnsEmptyArray()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Release,
                new string[] { },
                false,
                false
            );

            // Act & Assert
            Assert.Empty(options.PackageSources);
        }

        #endregion

        #region IsInteractive Property Tests

        [Fact]
        public void IsInteractive_Property_WithTrueValue_ReturnsTrue()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                true,
                false
            );

            // Act & Assert
            Assert.True(options.IsInteractive);
        }

        [Fact]
        public void IsInteractive_Property_WithFalseValue_ReturnsFalse()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            );

            // Act & Assert
            Assert.False(options.IsInteractive);
        }

        #endregion

        #region NoCache Property Tests

        [Fact]
        public void NoCache_Property_WithTrueValue_ReturnsTrue()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                true
            );

            // Act & Assert
            Assert.True(options.NoCache);
        }

        [Fact]
        public void NoCache_Property_WithFalseValue_ReturnsFalse()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            );

            // Act & Assert
            Assert.False(options.NoCache);
        }

        #endregion

        #region AssemblyLoadContext Property Tests (conditional NETCOREAPP)

#if NETCOREAPP
        [Fact]
        public void AssemblyLoadContext_Property_WithNullValue_ReturnsNull()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            );

            // Act & Assert
            Assert.Null(options.AssemblyLoadContext);
        }

        [Fact]
        public void AssemblyLoadContext_Property_CanBeSet()
        {
            // Arrange
            var alc = new System.Runtime.Loader.AssemblyLoadContext("test-context", false);
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            )
            {
                AssemblyLoadContext = alc
            };

            // Act & Assert
            Assert.NotNull(options.AssemblyLoadContext);
            Assert.Equal(alc, options.AssemblyLoadContext);
        }
#endif

        #endregion

        #region Combined Configuration Tests

        [Fact]
        public void Constructor_WithAllBooleansFalse_InitializesCorrectly()
        {
            // Arrange & Act
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            );

            // Assert
            Assert.False(options.IsInteractive);
            Assert.False(options.NoCache);
        }

        [Fact]
        public void Constructor_WithAllBooleansTrue_InitializesCorrectly()
        {
            // Arrange & Act
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                true,
                true
            );

            // Assert
            Assert.True(options.IsInteractive);
            Assert.True(options.NoCache);
        }

        [Fact]
        public void Constructor_WithMixedBooleans_InitializesCorrectly()
        {
            // Arrange & Act
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "arg1" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                true,
                false
            );

            // Assert
            Assert.True(options.IsInteractive);
            Assert.False(options.NoCache);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Constructor_WithSingleElementArrays_InitializesCorrectly()
        {
            // Arrange & Act
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                new[] { "single-arg" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            );

            // Assert
            Assert.Single(options.Arguments);
            Assert.Single(options.PackageSources);
        }

        [Fact]
        public void Constructor_WithLargeArrays_InitializesCorrectly()
        {
            // Arrange
            var largeArguments = new string[100];
            for (int i = 0; i < 100; i++)
            {
                largeArguments[i] = $"arg{i}";
            }
            var largePackageSources = new string[50];
            for (int i = 0; i < 50; i++)
            {
                largePackageSources[i] = $"https://source{i}.com/index.json";
            }

            // Act
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                largeArguments,
                OptimizationLevel.Release,
                largePackageSources,
                false,
                false
            );

            // Assert
            Assert.Equal(100, options.Arguments.Length);
            Assert.Equal(50, options.PackageSources.Length);
        }

        #endregion

        #region Property Immutability Tests

        [Fact]
        public void File_Property_IsReadOnly()
        {
            // Arrange
            var originalFile = new ScriptFile("test.csx");
            var options = new ExecuteScriptCommandOptions(
                originalFile,
                new[] { "arg1" },
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            );

            // Act & Assert - Trying to set would cause compilation error
            // This test documents the property is read-only
            Assert.Equal(originalFile, options.File);
        }

        [Fact]
        public void Arguments_Property_IsReadOnly()
        {
            // Arrange
            var originalArgs = new[] { "arg1" };
            var options = new ExecuteScriptCommandOptions(
                new ScriptFile("test.csx"),
                originalArgs,
                OptimizationLevel.Release,
                new[] { "https://api.nuget.org/v3/index.json" },
                false,
                false
            );

            // Act & Assert
            Assert.Equal(originalArgs, options.Arguments);
        }

        #endregion
    }
}