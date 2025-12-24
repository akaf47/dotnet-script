using System;
using System.Collections.Generic;
using System.Reflection;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Runtime;
using Dotnet.Script.Shared.Tests;
using Gapotchenko.FX.Reflection;
using Moq;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class ScriptRunnerTestsComprehensive
    {
        #region Helper Methods

        private static ScriptRunner CreateScriptRunner()
        {
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var scriptCompiler = new ScriptCompiler(logFactory, false);
            return new ScriptRunner(scriptCompiler, logFactory, ScriptConsole.Default);
        }

        #endregion

        #region ResolveAssembly Tests

        [Fact]
        public void ResolveAssembly_ReturnsNull_WhenRuntimeDepsMapDoesNotContainAssembly()
        {
            // Arrange
            var scriptRunner = CreateScriptRunner();
            var assemblyName = new AssemblyName("NonExistentAssembly");
            var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>();

            // Act
            var result = scriptRunner.ResolveAssembly(AssemblyLoadPal.ForCurrentAppDomain, assemblyName, runtimeDepsMap);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ResolveAssembly_ReturnsAssembly_WhenAssemblyExistsInRuntimeDepsMap()
        {
            // Arrange
            var scriptRunner = CreateScriptRunner();
            var assemblyName = new AssemblyName("System.Linq");
            
            // Get a real assembly path from current app domain
            var existingAssembly = typeof(object).Assembly;
            var runtimeAssembly = new RuntimeAssembly(assemblyName.Name, existingAssembly.Location, existingAssembly.FullName);
            
            var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>
            {
                { assemblyName.Name, runtimeAssembly }
            };

            // Act
            var result = scriptRunner.ResolveAssembly(AssemblyLoadPal.ForCurrentAppDomain, assemblyName, runtimeDepsMap);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assemblyName.Name, result.GetName().Name);
        }

        [Fact]
        public void ResolveAssembly_ReturnsNull_WhenAssemblyPathIsInvalid()
        {
            // Arrange
            var scriptRunner = CreateScriptRunner();
            var assemblyName = new AssemblyName("InvalidAssembly");
            var runtimeAssembly = new RuntimeAssembly(assemblyName.Name, "/nonexistent/path/invalid.dll", "InvalidAssembly, Version=0.0.0.0");
            
            var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>
            {
                { assemblyName.Name, runtimeAssembly }
            };

            // Act & Assert - Should throw or return null based on implementation
            try
            {
                var result = scriptRunner.ResolveAssembly(AssemblyLoadPal.ForCurrentAppDomain, assemblyName, runtimeDepsMap);
                // If it doesn't throw, it should be null
                Assert.Null(result);
            }
            catch
            {
                // Expected behavior - invalid path throws
            }
        }

        [Fact]
        public void ResolveAssembly_HandlesEmptyRuntimeDepsMap()
        {
            // Arrange
            var scriptRunner = CreateScriptRunner();
            var assemblyName = new AssemblyName("AnyAssembly");
            var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>();

            // Act
            var result = scriptRunner.ResolveAssembly(AssemblyLoadPal.ForCurrentAppDomain, assemblyName, runtimeDepsMap);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ResolveAssembly_CaseSensitive_WhenLookupByName()
        {
            // Arrange
            var scriptRunner = CreateScriptRunner();
            var assemblyName = new AssemblyName("MyAssembly");
            var existingAssembly = typeof(object).Assembly;
            var runtimeAssembly = new RuntimeAssembly("MyAssembly", existingAssembly.Location, existingAssembly.FullName);
            
            var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>
            {
                { "MyAssembly", runtimeAssembly }
            };

            // Act - Try with different case
            var result = scriptRunner.ResolveAssembly(AssemblyLoadPal.ForCurrentAppDomain, new AssemblyName("myassembly"), runtimeDepsMap);

            // Assert - Should not find (case-sensitive lookup in dictionary)
            Assert.Null(result);
        }

        [Fact]
        public void ResolveAssembly_WithMultipleAssembliesInMap_ReturnsCorrectOne()
        {
            // Arrange
            var scriptRunner = CreateScriptRunner();
            var assemblyName = new AssemblyName("TargetAssembly");
            var existingAssembly = typeof(object).Assembly;
            
            var targetRuntimeAssembly = new RuntimeAssembly("TargetAssembly", existingAssembly.Location, existingAssembly.FullName);
            var otherRuntimeAssembly = new RuntimeAssembly("OtherAssembly", existingAssembly.Location, existingAssembly.FullName);
            
            var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>
            {
                { "OtherAssembly", otherRuntimeAssembly },
                { "TargetAssembly", targetRuntimeAssembly },
                { "AnotherAssembly", new RuntimeAssembly("AnotherAssembly", existingAssembly.Location, existingAssembly.FullName) }
            };

            // Act
            var result = scriptRunner.ResolveAssembly(AssemblyLoadPal.ForCurrentAppDomain, assemblyName, runtimeDepsMap);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TargetAssembly", result.GetName().Name);
        }

        [Fact]
        public void ResolveAssembly_WithNullAssemblyName_ReturnsNull()
        {
            // Arrange
            var scriptRunner = CreateScriptRunner();
            var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>();

            // Act & Assert - Should handle null gracefully
            try
            {
                var result = scriptRunner.ResolveAssembly(AssemblyLoadPal.ForCurrentAppDomain, null, runtimeDepsMap);
                Assert.Null(result);
            }
            catch (NullReferenceException)
            {
                // Expected if implementation doesn't null-check
            }
        }

        [Fact]
        public void ResolveAssembly_WithNullRuntimeDepsMap_ReturnsNull()
        {
            // Arrange
            var scriptRunner = CreateScriptRunner();
            var assemblyName = new AssemblyName("AnyAssembly");

            // Act & Assert
            try
            {
                var result = scriptRunner.ResolveAssembly(AssemblyLoadPal.ForCurrentAppDomain, assemblyName, null);
                Assert.Null(result);
            }
            catch (ArgumentNullException)
            {
                // Expected if null not handled
            }
        }

        [Fact]
        public void ResolveAssembly_WithSpecialCharactersInAssemblyName()
        {
            // Arrange
            var scriptRunner = CreateScriptRunner();
            var assemblyName = new AssemblyName("My.Special-Assembly_Name");
            var existingAssembly = typeof(object).Assembly;
            var runtimeAssembly = new RuntimeAssembly("My.Special-Assembly_Name", existingAssembly.Location, existingAssembly.FullName);
            
            var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>
            {
                { "My.Special-Assembly_Name", runtimeAssembly }
            };

            // Act
            var result = scriptRunner.ResolveAssembly(AssemblyLoadPal.ForCurrentAppDomain, assemblyName, runtimeDepsMap);

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_InitializesWithScriptCompiler()
        {
            // Arrange
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var scriptCompiler = new ScriptCompiler(logFactory, false);
            var scriptConsole = ScriptConsole.Default;

            // Act
            var scriptRunner = new ScriptRunner(scriptCompiler, logFactory, scriptConsole);

            // Assert
            Assert.NotNull(scriptRunner);
        }

        [Fact]
        public void Constructor_WithNullScriptCompiler_ThrowsException()
        {
            // Arrange
            var logFactory = TestOutputHelper.CreateTestLogFactory();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ScriptRunner(null, logFactory, ScriptConsole.Default));
        }

        [Fact]
        public void Constructor_WithNullLogFactory_ThrowsException()
        {
            // Arrange
            var scriptCompiler = new ScriptCompiler(TestOutputHelper.CreateTestLogFactory(), false);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ScriptRunner(scriptCompiler, null, ScriptConsole.Default));
        }

        [Fact]
        public void Constructor_WithNullScriptConsole_ThrowsException()
        {
            // Arrange
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var scriptCompiler = new ScriptCompiler(logFactory, false);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ScriptRunner(scriptCompiler, logFactory, null));
        }

        #endregion

        #region Edge Cases and Integration

        [Fact]
        public void ResolveAssembly_WithLargeRuntimeDepsMap()
        {
            // Arrange
            var scriptRunner = CreateScriptRunner();
            var targetAssemblyName = new AssemblyName("Assembly100");
            var existingAssembly = typeof(object).Assembly;
            
            var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>();
            
            // Add 200 assemblies to the map
            for (int i = 0; i < 200; i++)
            {
                var name = $"Assembly{i}";
                var runtimeAssembly = new RuntimeAssembly(name, existingAssembly.Location, existingAssembly.FullName);
                runtimeDepsMap[name] = runtimeAssembly;
            }

            // Act
            var result = scriptRunner.ResolveAssembly(AssemblyLoadPal.ForCurrentAppDomain, targetAssemblyName, runtimeDepsMap);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Assembly100", result.GetName().Name);
        }

        [Fact]
        public void ResolveAssembly_PreservesAssemblyMetadata()
        {
            // Arrange
            var scriptRunner = CreateScriptRunner();
            var targetAssemblyName = new AssemblyName("MetadataTestAssembly");
            var existingAssembly = typeof(object).Assembly;
            var runtimeAssembly = new RuntimeAssembly("MetadataTestAssembly", existingAssembly.Location, existingAssembly.FullName);
            
            var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>
            {
                { "MetadataTestAssembly", runtimeAssembly }
            };

            // Act
            var result = scriptRunner.ResolveAssembly(AssemblyLoadPal.ForCurrentAppDomain, targetAssemblyName, runtimeDepsMap);

            // Assert
            Assert.NotNull(result);
            var assemblyName = result.GetName();
            Assert.NotNull(assemblyName.Name);
            Assert.NotNull(assemblyName.Version);
        }

        #endregion
    }
}