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
    public class ScriptRunnerTestsTests
    {
        [Fact]
        public void ResolveAssembly_NullRuntimeDepsMap_ReturnsNull()
        {
            // Arrange
            var scriptRunner = CreateScriptRunner();
            var assemblyName = new AssemblyName("AnyAssemblyName");

            // Act
            var result = scriptRunner.ResolveAssembly(
                AssemblyLoadPal.ForCurrentAppDomain, 
                assemblyName, 
                new Dictionary<string, RuntimeAssembly>()
            );

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CreateScriptRunner_CreatesValidInstance()
        {
            // Arrange & Act
            var scriptRunner = CreateScriptRunner();

            // Assert
            Assert.NotNull(scriptRunner);
        }

        private static ScriptRunner CreateScriptRunner()
        {
            var logFactory = TestOutputHelper.CreateTestLogFactory();
            var scriptCompiler = new ScriptCompiler(logFactory, false);
            
            return new ScriptRunner(scriptCompiler, logFactory, ScriptConsole.Default);
        }
    }
}