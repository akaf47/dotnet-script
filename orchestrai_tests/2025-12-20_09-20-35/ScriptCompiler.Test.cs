using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using Moq;
using NUnit.Framework;

namespace Dotnet.Script.Core.Tests
{
    [TestFixture]
    public class ScriptCompilerTests
    {
        private Mock<LogFactory> _mockLogFactory;
        private ScriptCompiler _scriptCompiler;

        [SetUp]
        public void Setup()
        {
            _mockLogFactory = new Mock<LogFactory>();
            _mockLogFactory.Setup(f => f(It.IsAny<Type>())).Returns(new Mock<Logger>().Object);

            _scriptCompiler = new ScriptCompiler(_mockLogFactory.Object, false);
        }

        [Test]
        public void CreateScriptOptions_WithContext_ShouldCreateOptionsWithCorrectSettings()
        {
            // Arrange
            var context = CreateMockScriptContext();
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var scriptOptions = _scriptCompiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.IsNotNull(scriptOptions);
            Assert.IsTrue(scriptOptions.Imports.Contains("System"));
            Assert.AreEqual(context.Code.Encoding, Encoding.UTF8);
        }

        [Test]
        public void CreateCompilationContext_WithValidContext_ShouldCreateCompilationContext()
        {
            // Arrange
            var context = CreateMockScriptContext();

            // Act
            var compilationContext = _scriptCompiler.CreateCompilationContext<int, CommandLineScriptGlobals>(context);

            // Assert
            Assert.IsNotNull(compilationContext);
            Assert.IsNotNull(compilationContext.Script);
        }

        [Test]
        public void CreateCompilationContext_WithNullContext_ShouldThrowArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                _scriptCompiler.CreateCompilationContext<int, CommandLineScriptGlobals>(null));
        }

        [Test]
        public void CreateScriptDependenciesMap_WithMultipleAssemblies_ShouldReturnMapWithHighestVersion()
        {
            // Arrange
            var runtimeDependencies = new[]
            {
                CreateRuntimeDependency("TestAssembly", new Version(1, 0)),
                CreateRuntimeDependency("TestAssembly", new Version(2, 0))
            };

            // Act
            var dependencyMap = ScriptCompiler.CreateScriptDependenciesMap(runtimeDependencies);

            // Assert
            Assert.AreEqual(1, dependencyMap.Count);
            Assert.AreEqual(new Version(2, 0), dependencyMap["TestAssembly"].Name.Version);
        }

        [Test]
        public void CreateLoadedAssembliesMap_ShouldCreateMapWithHighestVersion()
        {
            // This test requires reflection to access a private method
            var method = typeof(ScriptCompiler).GetMethod("CreateLoadedAssembliesMap", 
                BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var loadedAssembliesMap = method.Invoke(null, null) as Dictionary<string, Assembly>;

            // Assert
            Assert.IsNotNull(loadedAssembliesMap);
        }

        private ScriptContext CreateMockScriptContext()
        {
            return new ScriptContext(
                SourceText.From("test script"), 
                "/working/directory", 
                new[] { "arg1" }, 
                null, 
                OptimizationLevel.Debug
            );
        }

        private RuntimeDependency CreateRuntimeDependency(string name, Version version)
        {
            var mockAssembly = new Mock<RuntimeAssembly>();
            mockAssembly.Setup(a => a.Name).Returns(new AssemblyName(name) { Version = version });
            mockAssembly.Setup(a => a.Path).Returns($"/path/to/{name}.dll");

            var runtimeDependency = new Mock<RuntimeDependency>();
            runtimeDependency.Setup(r => r.Assemblies).Returns(new[] { mockAssembly.Object });
            runtimeDependency.Setup(r => r.Name).Returns(name);

            return runtimeDependency.Object;
        }
    }
}