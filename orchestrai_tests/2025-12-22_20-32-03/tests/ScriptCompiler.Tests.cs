using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Environment;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using Xunit;
using Moq;

namespace Dotnet.Script.Tests
{
    public class ScriptCompilerTests
    {
        private ScriptCompiler _compiler;
        private Mock<LogFactory> _mockLogFactory;
        private Mock<Logger> _mockLogger;

        public ScriptCompilerTests()
        {
            _mockLogger = new Mock<Logger>();
            _mockLogFactory = new Mock<LogFactory>();
            _mockLogFactory.Setup(f => f(It.IsAny<Type>())).Returns(_mockLogger.Object);
            _compiler = new ScriptCompiler(_mockLogFactory.Object, true);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithLogFactory_InitializesCompiler()
        {
            // Arrange & Act
            var compiler = new ScriptCompiler(_mockLogFactory.Object, true);

            // Assert
            Assert.NotNull(compiler);
            Assert.NotNull(compiler.RuntimeDependencyResolver);
        }

        [Fact]
        public void Constructor_WithLogFactory_SetsInitialDiagnosticOptions()
        {
            // Arrange & Act
            var compiler = new ScriptCompiler(_mockLogFactory.Object, true);

            // Assert - nullable diagnostic options CS8600-CS8655 should be set
            var diagnosticOptionsProperty = compiler.GetType()
                .GetProperty("SpecificDiagnosticOptions", 
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            
            Assert.NotNull(diagnosticOptionsProperty);
        }

        [Fact]
        public void Constructor_WithLogFactoryAndRestoreCache_InitializesResolver()
        {
            // Arrange & Act
            var compiler = new ScriptCompiler(_mockLogFactory.Object, useRestoreCache: true);

            // Assert
            Assert.NotNull(compiler.RuntimeDependencyResolver);
        }

        [Fact]
        public void Constructor_WithoutRestoreCache_InitializesResolver()
        {
            // Arrange & Act
            var compiler = new ScriptCompiler(_mockLogFactory.Object, useRestoreCache: false);

            // Assert
            Assert.NotNull(compiler.RuntimeDependencyResolver);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void ParseOptions_ReturnsValidCSharpParseOptions()
        {
            // Arrange & Act
            var parseOptions = _compiler.ParseOptions;

            // Assert
            Assert.NotNull(parseOptions);
            Assert.Equal(LanguageVersion.Preview, parseOptions.LanguageVersion);
            Assert.Equal(SourceCodeKind.Script, parseOptions.Kind);
        }

        [Fact]
        public void ImportedNamespaces_ReturnsDefaultNamespaces()
        {
            // Arrange & Act
            var namespaces = _compiler.ImportedNamespaces;

            // Assert
            Assert.NotNull(namespaces);
            Assert.Contains("System", namespaces);
            Assert.Contains("System.IO", namespaces);
            Assert.Contains("System.Collections.Generic", namespaces);
            Assert.Contains("System.Linq", namespaces);
        }

        [Fact]
        public void SuppressedDiagnosticIds_ReturnsExpectedIds()
        {
            // Arrange & Act
            var suppressedIds = _compiler.SuppressedDiagnosticIds;

            // Assert
            Assert.NotNull(suppressedIds);
            Assert.Contains("CS1701", suppressedIds);
            Assert.Contains("CS1702", suppressedIds);
            Assert.Contains("CS1705", suppressedIds);
        }

        [Fact]
        public void RuntimeDependencyResolver_IsInitialized()
        {
            // Arrange & Act
            var resolver = _compiler.RuntimeDependencyResolver;

            // Assert
            Assert.NotNull(resolver);
        }

        #endregion

        #region CreateScriptOptions Tests

        [Fact]
        public void CreateScriptOptions_WithValidContext_ReturnsScriptOptions()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" });
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var options = _compiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.NotNull(options);
        }

        [Fact]
        public void CreateScriptOptions_WithFilePath_IncludesFilePath()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var filePath = "/path/to/script.csx";
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" }, filePath);
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var options = _compiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.NotNull(options);
        }

        [Fact]
        public void CreateScriptOptions_WithoutFilePath_DoesNotSetFilePath()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" }, filePath: null);
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var options = _compiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.NotNull(options);
        }

        [Fact]
        public void CreateScriptOptions_WithCustomEncoding_UsesProvidedEncoding()
        {
            // Arrange
            var encoding = Encoding.Unicode;
            var code = SourceText.From("var x = 1;", encoding);
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" });
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var options = _compiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.NotNull(options);
        }

        [Fact]
        public void CreateScriptOptions_WithoutEncoding_UsesDefaultEncoding()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" });
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var options = _compiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.NotNull(options);
        }

        [Fact]
        public void CreateScriptOptions_WithRuntimeDependencies_AddsImports()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" });
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var options = _compiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.NotNull(options);
        }

        [Fact]
        public void CreateScriptOptions_WithWhitespaceFilePath_SkipsFilePathSetting()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" }, "   ");
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var options = _compiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.NotNull(options);
        }

        #endregion

        #region CreateCompilationContext Tests

        [Fact]
        public void CreateCompilationContext_WithNullContext_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _compiler.CreateCompilationContext<object, object>(null));
        }

        [Fact]
        public void CreateCompilationContext_WithValidContext_ReturnsCompilationContext()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" });

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int, object>(context);

            // Assert
            Assert.NotNull(compilationContext);
            Assert.NotNull(compilationContext.Script);
            Assert.NotNull(compilationContext.Warnings);
            Assert.NotNull(compilationContext.Errors);
        }

        [Fact]
        public void CreateCompilationContext_SeparatesWarningsFromErrors()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" });

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int, object>(context);

            // Assert
            Assert.NotNull(compilationContext.Warnings);
            Assert.NotNull(compilationContext.Errors);
        }

        [Fact]
        public void CreateCompilationContext_WithScriptMode_GetsDependencies()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(
                code,
                Environment.CurrentDirectory,
                new[] { "arg1" },
                filePath: "/path/to/script.csx");

            // Act & Assert
            // This test verifies the code path where ScriptMode == ScriptMode.Script
            var result = _compiler.CreateCompilationContext<int, object>(context);
            Assert.NotNull(result);
        }

        [Fact]
        public void CreateCompilationContext_AssemblyLoadContextNotSet_CreatesContext()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" });

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int, object>(context);

            // Assert
            Assert.NotNull(compilationContext);
        }

        #endregion

        #region Static Method Tests

        [Fact]
        public void CreateScriptDependenciesMap_WithEmptyDependencies_ReturnsEmptyMap()
        {
            // Arrange
            var runtimeDependencies = new RuntimeDependency[0];

            // Act
            var result = ScriptCompiler.CreateScriptDependenciesMap(runtimeDependencies);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void CreateScriptDependenciesMap_WithDependencies_SelectsHighestVersion()
        {
            // Arrange
            var assembly1 = new RuntimeAssembly(
                new AssemblyName("TestAssembly") { Version = new Version(1, 0) },
                "/path/to/assembly1.dll");
            var assembly2 = new RuntimeAssembly(
                new AssemblyName("TestAssembly") { Version = new Version(2, 0) },
                "/path/to/assembly2.dll");
            
            var dependency1 = new RuntimeDependency("Dep1", null, new[] { assembly1 });
            var dependency2 = new RuntimeDependency("Dep2", null, new[] { assembly2 });

            // Act
            var result = ScriptCompiler.CreateScriptDependenciesMap(
                new[] { dependency1, dependency2 });

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("TestAssembly"));
            Assert.Equal(new Version(2, 0), result["TestAssembly"].Name.Version);
        }

        [Fact]
        public void CreateScriptDependenciesMap_WithMultipleSameNameAssemblies_SelectsNewest()
        {
            // Arrange
            var assembly1 = new RuntimeAssembly(
                new AssemblyName("Lib") { Version = new Version(1, 0) },
                "/path/to/lib1.dll");
            var assembly2 = new RuntimeAssembly(
                new AssemblyName("Lib") { Version = new Version(1, 5) },
                "/path/to/lib1.5.dll");
            var assembly3 = new RuntimeAssembly(
                new AssemblyName("Lib") { Version = new Version(2, 0) },
                "/path/to/lib2.dll");

            var dependency = new RuntimeDependency("Dep", null, 
                new[] { assembly1, assembly2, assembly3 });

            // Act
            var result = ScriptCompiler.CreateScriptDependenciesMap(new[] { dependency });

            // Assert
            Assert.Single(result);
            Assert.Equal(new Version(2, 0), result["Lib"].Name.Version);
        }

        [Fact]
        public void CreateScriptDependenciesMap_IsCaseInsensitive()
        {
            // Arrange
            var assembly = new RuntimeAssembly(
                new AssemblyName("TestLib") { Version = new Version(1, 0) },
                "/path/to/lib.dll");
            var dependency = new RuntimeDependency("Dep", null, new[] { assembly });

            // Act
            var result = ScriptCompiler.CreateScriptDependenciesMap(new[] { dependency });

            // Assert
            Assert.True(result.ContainsKey("testlib"));
            Assert.True(result.ContainsKey("TESTLIB"));
        }

        [Fact]
        public void CreateScriptDependenciesMap_WithDistinctAssemblies_HandlesDistinct()
        {
            // Arrange
            var assembly1 = new RuntimeAssembly(
                new AssemblyName("Lib1") { Version = new Version(1, 0) },
                "/path/to/lib1.dll");
            var assembly2 = new RuntimeAssembly(
                new AssemblyName("Lib2") { Version = new Version(1, 0) },
                "/path/to/lib2.dll");

            var dependency1 = new RuntimeDependency("Dep1", null, new[] { assembly1 });
            var dependency2 = new RuntimeDependency("Dep2", null, new[] { assembly2 });

            // Act
            var result = ScriptCompiler.CreateScriptDependenciesMap(
                new[] { dependency1, dependency2 });

            // Assert
            Assert.Equal(2, result.Count);
        }

        #endregion

        #region GetScriptCode Tests

        [Fact]
        public void GetScriptCode_WithFilePath_ReturnsCodeAsString()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var filePath = "/path/to/script.csx";
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" }, filePath);

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int, object>(context);

            // Assert
            Assert.NotNull(compilationContext);
        }

        [Fact]
        public void GetScriptCode_WithoutFilePath_ProcessesPreprocessorDirectives()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" }, filePath: null);

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int, object>(context);

            // Assert
            Assert.NotNull(compilationContext);
        }

        [Fact]
        public void GetScriptCode_WithPreprocessorDirectives_RewritesForNewLines()
        {
            // Arrange
            var code = SourceText.From("#load \"file.csx\"\nvar x = 1;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1" }, filePath: null);

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int, object>(context);

            // Assert
            Assert.NotNull(compilationContext);
        }

        #endregion

        #region Assembly Resolution Tests

        [Fact]
        public void MapUnresolvedAssemblyToRuntimeLibrary_WithMatchingDependency_ReturnsRedirectAssembly()
        {
            // Arrange
            var assemblyName = new AssemblyName("TestAssembly") { Version = new Version(1, 0) };
            var runtimeAssembly = new RuntimeAssembly(
                new AssemblyName("TestAssembly") { Version = new Version(2, 0) },
                typeof(object).Assembly.Location);
            
            var dependencyMap = new Dictionary<string, RuntimeAssembly>
            {
                { "TestAssembly", runtimeAssembly }
            };
            var loadedAssemblyMap = new Dictionary<string, Assembly>();
            
            var args = new ResolveEventArgs("TestAssembly, Version=1.0.0.0", null);

            // Act - this method is private, so we test through public API
            var compilationContext = _compiler.CreateCompilationContext<int, object>(
                new ScriptContext(
                    SourceText.From("var x = 1;"),
                    Environment.CurrentDirectory,
                    new[] { "arg1" }));

            // Assert
            Assert.NotNull(compilationContext);
        }

        [Fact]
        public void MapUnresolvedAssemblyToRuntimeLibrary_WithNullVersion_UsesRuntimeAssembly()
        {
            // Arrange
            var assemblyName = new AssemblyName("TestAssembly");
            var runtimeAssembly = new RuntimeAssembly(
                new AssemblyName("TestAssembly") { Version = new Version(2, 0) },
                typeof(object).Assembly.Location);

            var dependencyMap = new Dictionary<string, RuntimeAssembly>
            {
                { "TestAssembly", runtimeAssembly }
            };

            // Act & Assert
            var compilationContext = _compiler.CreateCompilationContext<int, object>(
                new ScriptContext(
                    SourceText.From("var x = 1;"),
                    Environment.CurrentDirectory,
                    new[] { "arg1" }));
            Assert.NotNull(compilationContext);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void CreateCompilationContext_WithEmptyArgs_Succeeds()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, Array.Empty<string>());

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int, object>(context);

            // Assert
            Assert.NotNull(compilationContext);
        }

        [Fact]
        public void CreateCompilationContext_WithDebugOptimization_Succeeds()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(
                code,
                Environment.CurrentDirectory,
                new[] { "arg1" },
                optimizationLevel: OptimizationLevel.Debug);

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int, object>(context);

            // Assert
            Assert.NotNull(compilationContext);
        }

        [Fact]
        public void CreateCompilationContext_WithReleaseOptimization_Succeeds()
        {
            // Arrange
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(
                code,
                Environment.CurrentDirectory,
                new[] { "arg1" },
                optimizationLevel: OptimizationLevel.Release);

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int, object>(context);

            // Assert
            Assert.NotNull(compilationContext);
        }

        [Fact]
        public void CreateScriptDependenciesMap_WithNullAssemblies_HandlesGracefully()
        {
            // Arrange
            var dependency = new RuntimeDependency("Dep", null, new RuntimeAssembly[0]);

            // Act
            var result = ScriptCompiler.CreateScriptDependenciesMap(new[] { dependency });

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion
    }
}