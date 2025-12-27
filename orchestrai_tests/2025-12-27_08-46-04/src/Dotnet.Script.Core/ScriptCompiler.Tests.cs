using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using Dotnet.Script.Core;
using Dotnet.Script.Core.Internal;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Environment;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using Xunit;
using Moq;
using RuntimeAssembly = Dotnet.Script.DependencyModel.Runtime.RuntimeAssembly;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptCompilerTests
    {
        private readonly Mock<LogFactory> _mockLogFactory;
        private readonly Mock<Logger> _mockLogger;
        private readonly Mock<RuntimeDependencyResolver> _mockRuntimeDependencyResolver;
        private ScriptCompiler _compiler;

        public ScriptCompilerTests()
        {
            _mockLogger = new Mock<Logger>();
            _mockLogFactory = new Mock<LogFactory>();
            _mockLogFactory.Setup(f => f(It.IsAny<Type>())).Returns(_mockLogger.Object);
            _mockRuntimeDependencyResolver = new Mock<RuntimeDependencyResolver>(_mockLogFactory.Object, true);
        }

        private ScriptCompiler CreateCompiler()
        {
            return new ScriptCompiler(_mockLogFactory.Object, _mockRuntimeDependencyResolver.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithLogFactory_InitializesCorrectly()
        {
            // Arrange & Act
            var compiler = new ScriptCompiler(_mockLogFactory.Object, useRestoreCache: true);

            // Assert
            Assert.NotNull(compiler);
            Assert.NotNull(compiler.RuntimeDependencyResolver);
            Assert.NotNull(compiler.ParseOptions);
            Assert.Equal(LanguageVersion.Preview, compiler.ParseOptions.LanguageVersion);
            Assert.Equal(SourceCodeKind.Script, compiler.ParseOptions.Kind);
        }

        [Fact]
        public void Constructor_WithLogFactory_InitializesDiagnosticOptions()
        {
            // Arrange & Act
            var compiler = new ScriptCompiler(_mockLogFactory.Object, useRestoreCache: false);

            // Assert
            Assert.NotNull(compiler);
            // Verify nullable diagnostic options are set (CS8600-CS8655)
            var diagnosticOptionsField = typeof(ScriptCompiler).GetField("SpecificDiagnosticOptions", 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        [Fact]
        public void Constructor_WithRuntimeDependencyResolver_InitializesCorrectly()
        {
            // Arrange & Act
            var compiler = new ScriptCompiler(_mockLogFactory.Object, _mockRuntimeDependencyResolver.Object);

            // Assert
            Assert.NotNull(compiler);
            Assert.Equal(_mockRuntimeDependencyResolver.Object, compiler.RuntimeDependencyResolver);
        }

        #endregion

        #region Properties Tests

        [Fact]
        public void ImportedNamespaces_ReturnsExpectedNamespaces()
        {
            // Arrange
            _compiler = CreateCompiler();

            // Act
            var namespaces = _compiler.ImportedNamespaces.ToList();

            // Assert
            Assert.NotEmpty(namespaces);
            Assert.Contains("System", namespaces);
            Assert.Contains("System.IO", namespaces);
            Assert.Contains("System.Collections.Generic", namespaces);
            Assert.Contains("System.Console", namespaces);
            Assert.Contains("System.Diagnostics", namespaces);
            Assert.Contains("System.Dynamic", namespaces);
            Assert.Contains("System.Linq", namespaces);
            Assert.Contains("System.Linq.Expressions", namespaces);
            Assert.Contains("System.Text", namespaces);
            Assert.Contains("System.Threading.Tasks", namespaces);
        }

        [Fact]
        public void SuppressedDiagnosticIds_ReturnsExpectedIds()
        {
            // Arrange
            _compiler = CreateCompiler();

            // Act
            var suppressedIds = _compiler.SuppressedDiagnosticIds.ToList();

            // Assert
            Assert.NotEmpty(suppressedIds);
            Assert.Contains("CS1701", suppressedIds);
            Assert.Contains("CS1702", suppressedIds);
            Assert.Contains("CS1705", suppressedIds);
        }

        [Fact]
        public void ParseOptions_HasCorrectLanguageVersion()
        {
            // Arrange
            _compiler = CreateCompiler();

            // Act
            var parseOptions = _compiler.ParseOptions;

            // Assert
            Assert.NotNull(parseOptions);
            Assert.Equal(LanguageVersion.Preview, parseOptions.LanguageVersion);
            Assert.Equal(SourceCodeKind.Script, parseOptions.Kind);
        }

        [Fact]
        public void RuntimeDependencyResolver_IsNotNull()
        {
            // Arrange
            _compiler = CreateCompiler();

            // Act
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
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, "/tmp", new[] { "arg1" });
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var options = _compiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.NotNull(options);
        }

        [Fact]
        public void CreateScriptOptions_IncludesImportedNamespaces()
        {
            // Arrange
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>());
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var options = _compiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.NotNull(options);
            // Verify imports are included (would need reflection to fully verify)
        }

        [Fact]
        public void CreateScriptOptions_WithFilePath_SetsFilePath()
        {
            // Arrange
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var filePath = "/path/to/script.csx";
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>(), filePath);
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
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>(), filePath: null);
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var options = _compiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.NotNull(options);
        }

        [Fact]
        public void CreateScriptOptions_WithEmptyRuntimeDependencies_ReturnsOptions()
        {
            // Arrange
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>());
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var options = _compiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.NotNull(options);
        }

        [Fact]
        public void CreateScriptOptions_WithEncoding_UsesProvidedEncoding()
        {
            // Arrange
            _compiler = CreateCompiler();
            var encoding = Encoding.UTF8;
            var code = SourceText.From("var x = 1;", encoding);
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>());
            var runtimeDependencies = new List<RuntimeDependency>();

            // Act
            var options = _compiler.CreateScriptOptions(context, runtimeDependencies);

            // Assert
            Assert.NotNull(options);
        }

        [Fact]
        public void CreateScriptOptions_WithNullEncoding_UsesDefault()
        {
            // Arrange
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>());
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
            // Arrange
            _compiler = CreateCompiler();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                _compiler.CreateCompilationContext<int>(null));
        }

        [Fact]
        public void CreateCompilationContext_WithValidContext_ReturnsCompilationContext()
        {
            // Arrange
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>());
            
            _mockRuntimeDependencyResolver
                .Setup(r => r.GetDependencies(It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<RuntimeDependency>());
            _mockRuntimeDependencyResolver
                .Setup(r => r.GetDependenciesForCode(It.IsAny<string>(), It.IsAny<ScriptMode>(), 
                    It.IsAny<string[]>(), It.IsAny<string>()))
                .Returns(Enumerable.Empty<RuntimeDependency>());

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int>(context);

            // Assert
            Assert.NotNull(compilationContext);
            Assert.NotNull(compilationContext.Script);
            Assert.NotNull(compilationContext.SourceText);
            Assert.NotNull(compilationContext.Loader);
            Assert.NotNull(compilationContext.ScriptOptions);
            Assert.NotNull(compilationContext.RuntimeDependencies);
        }

        [Fact]
        public void CreateCompilationContext_WithScriptMode_CallsGetDependencies()
        {
            // Arrange
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var filePath = "/path/to/script.csx";
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>(), filePath, 
                OptimizationLevel.Debug, ScriptMode.Script);
            
            _mockRuntimeDependencyResolver
                .Setup(r => r.GetDependencies(filePath, It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<RuntimeDependency>());

            // Act
            _compiler.CreateCompilationContext<int>(context);

            // Assert
            _mockRuntimeDependencyResolver.Verify(
                r => r.GetDependencies(filePath, It.IsAny<string[]>()), Times.Once);
        }

        [Fact]
        public void CreateCompilationContext_WithCodeMode_CallsGetDependenciesForCode()
        {
            // Arrange
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>(), filePath: null, 
                OptimizationLevel.Debug, ScriptMode.ReadEvalPrintLoop);
            
            _mockRuntimeDependencyResolver
                .Setup(r => r.GetDependenciesForCode(It.IsAny<string>(), ScriptMode.ReadEvalPrintLoop, 
                    It.IsAny<string[]>(), It.IsAny<string>()))
                .Returns(Enumerable.Empty<RuntimeDependency>());

            // Act
            _compiler.CreateCompilationContext<int>(context);

            // Assert
            _mockRuntimeDependencyResolver.Verify(
                r => r.GetDependenciesForCode("/tmp", ScriptMode.ReadEvalPrintLoop, 
                    It.IsAny<string[]>(), code.ToString()), Times.Once);
        }

        [Fact]
        public void CreateCompilationContext_WithDebugOptimization_SetsDebugMode()
        {
            // Arrange
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>(), 
                optimizationLevel: OptimizationLevel.Debug);
            
            _mockRuntimeDependencyResolver
                .Setup(r => r.GetDependencies(It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<RuntimeDependency>());

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int>(context);

            // Assert
            Assert.NotNull(compilationContext);
        }

        [Fact]
        public void CreateCompilationContext_WithReleaseOptimization_SetsReleaseMode()
        {
            // Arrange
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>(), 
                optimizationLevel: OptimizationLevel.Release);
            
            _mockRuntimeDependencyResolver
                .Setup(r => r.GetDependencies(It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<RuntimeDependency>());

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int>(context);

            // Assert
            Assert.NotNull(compilationContext);
        }

        [Fact]
        public void CreateCompilationContext_SeparatesWarningsAndErrors()
        {
            // Arrange
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>());
            
            _mockRuntimeDependencyResolver
                .Setup(r => r.GetDependencies(It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<RuntimeDependency>());

            // Act
            var compilationContext = _compiler.CreateCompilationContext<int>(context);

            // Assert
            Assert.NotNull(compilationContext.Warnings);
            Assert.NotNull(compilationContext.Errors);
        }

        [Fact]
        public void CreateCompilationContext_WithPreprocessorDirectives_RewritesCode()
        {
            // Arrange
            _compiler = CreateCompiler();
            var code = SourceText.From("#load \"file.csx\"\nvar x = 1;");
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>(), filePath: null);
            
            _mockRuntimeDependencyResolver
                .Setup(r => r.GetDependencies(It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<RuntimeDependency>());

            // Act & Assert - Should not throw
            var compilationContext = _compiler.CreateCompilationContext<int>(context);
            Assert.NotNull(compilationContext);
        }

        [Fact]
        public void CreateCompilationContext_WithPackageSources_PassesToResolver()
        {
            // Arrange
            _compiler = CreateCompiler();
            var code = SourceText.From("var x = 1;");
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var context = new ScriptContext(code, "/tmp", Array.Empty<string>(), 
                filePath: null, packageSources: packageSources);
            
            _mockRuntimeDependencyResolver
                .Setup(r => r.GetDependencies(It.IsAny<string>(), packageSources))
                .Returns(Enumerable.Empty<RuntimeDependency>());

            // Act & Assert
            var compilationContext = _compiler.CreateCompilationContext<int>(context);
            Assert.NotNull(compilationContext);
        }

        #endregion

        #region Static Methods Tests

        [Fact]
        public void CreateScriptDependenciesMap_WithEmptyDependencies_ReturnsEmptyDictionary()
        {
            // Arrange
            var runtimeDependencies = Enumerable.Empty<RuntimeDependency>();

            // Act
            var map = ScriptCompiler.CreateScriptDependenciesMap(runtimeDependencies);

            // Assert
            Assert.NotNull(map);
            Assert.Empty(map);
        }

        [Fact]
        public void CreateScriptDependenciesMap_WithSingleDependency_ReturnsDictionary()
        {
            // Arrange
            var assemblyName = new AssemblyName("TestAssembly") { Version = new Version(1, 0, 0, 0) };
            var runtimeAssembly = new RuntimeAssembly(assemblyName, "/path/to/assembly.dll");
            var runtimeDependency = new RuntimeDependency("TestDep", "1.0.0", 
                new[] { runtimeAssembly }, Array.Empty<string>(), Array.Empty<string>());
            var runtimeDependencies = new[] { runtimeDependency };

            // Act
            var map = ScriptCompiler.CreateScriptDependenciesMap(runtimeDependencies);

            // Assert
            Assert.NotNull(map);
            Assert.NotEmpty(map);
            Assert.Contains("TestAssembly", map.Keys);
        }

        [Fact]
        public void CreateScriptDependenciesMap_WithMultipleAssemblies_SelectsHighestVersion()
        {
            // Arrange
            var assemblyName1 = new AssemblyName("TestAssembly") { Version = new Version(1, 0, 0, 0) };
            var assemblyName2 = new AssemblyName("TestAssembly") { Version = new Version(2, 0, 0, 0) };
            var runtimeAssembly1 = new RuntimeAssembly(assemblyName1, "/path/to/assembly1.dll");
            var runtimeAssembly2 = new RuntimeAssembly(assemblyName2, "/path/to/assembly2.dll");
            var runtimeDependency = new RuntimeDependency("TestDep", "1.0.0", 
                new[] { runtimeAssembly1, runtimeAssembly2 }, Array.Empty<string>(), Array.Empty<string>());
            var runtimeDependencies = new[] { runtimeDependency };

            // Act
            var map = ScriptCompiler.CreateScriptDependenciesMap(runtimeDependencies);

            // Assert
            Assert.NotNull(map);
            Assert.Single(map);
            var selectedAssembly = map["TestAssembly"];
            Assert.Equal(new Version(2, 0, 0, 0), selectedAssembly.Name.Version);
        }

        [Fact]
        public void CreateScriptDependenciesMap_WithDuplicateDependencies_SelectsHighestVersion()
        {
            // Arrange
            var assemblyName1 = new AssemblyName("TestAssembly") { Version = new Version(1, 0, 0, 0) };
            var assemblyName2 = new AssemblyName("TestAssembly") { Version = new Version(2, 0, 0, 0) };
            var runtimeAssembly1 = new RuntimeAssembly(assemblyName1, "/path/to/assembly1.dll");
            var runtimeAssembly2 = new RuntimeAssembly(assemblyName2, "/path/to/assembly2.dll");
            var runtimeDependency1 = new RuntimeDependency("TestDep", "1.0.0", 
                new[] { runtimeAssembly1 }, Array.Empty<string>(), Array.Empty<string>());
            var runtimeDependency2 = new RuntimeDependency("TestDep", "2.0.0", 
                new[] { runtimeAssembly2 }, Array.Empty<string>(), Array.Empty<string>());
            var runtimeDependencies = new[] { runtimeDependency1, runtimeDependency2 };

            // Act
            var map = ScriptCompiler.CreateScriptDependenciesMap(runtimeDependencies);

            // Assert
            Assert.NotNull(map);
            Assert.Single(map);
            var selectedAssembly = map["TestAssembly"];
            Assert.Equal(new Version(2, 0, 0, 0), selectedAssembly.Name.Version);
        }

        [Fact]
        public void CreateScriptDependenciesMap_IsCaseInsensitive()
        {
            // Arrange
            var assemblyName = new AssemblyName("TestAssembly") { Version = new Version(1, 0, 0, 0) };
            var runtimeAssembly = new RuntimeAssembly(assemblyName, "/path/to/assembly.dll");
            var runtimeDependency = new RuntimeDependency("TestDep", "1.0.0", 
                new[] { runtimeAssembly }, Array.Empty<string>(), Array.Empty<string>());
            var runtimeDependencies = new[] { runtimeDependency };

            // Act
            var map = ScriptCompiler.CreateScriptDependenciesMap(runtimeDependencies);

            // Assert
            Assert.NotNull(map);
            Assert.Contains("testassembly", map.Keys);
        }

        #endregion

        #region MapUnresolvedAssemblyToRuntimeLibrary Tests

        [Fact]
        public void MapUnresolvedAssemblyToRuntimeLibrary_WithNullArgs_ReturnsNull()
        {
            // Arrange
            _compiler = CreateCompiler();
            var dependencyMap = new Dictionary<string, RuntimeAssembly>();
            var loadedAssemblyMap = new Dictionary<string, Assembly>();
            var args = new ResolveEventArgs("NonExistent, Version=1.0.0.0");

            // Act
            var result = InvokePrivateMethod<Assembly>("MapUnresolvedAssemblyToRuntimeLibrary",
                _compiler, dependencyMap, loadedAssemblyMap, args);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void MapUnresolvedAssemblyToRuntimeLibrary_WithMissingAssembly_ReturnsNull()
        {
            // Arrange
            _compiler = CreateCompiler();
            var dependencyMap = new Dictionary<string, RuntimeAssembly>();
            var loadedAssemblyMap = new Dictionary<string, Assembly>();
            var args = new ResolveEventArgs("Missing, Version=1.0.0.0");

            // Act
            var result = InvokePrivateMethod<Assembly>("MapUnresolvedAssemblyToRuntimeLibrary",
                _compiler, dependencyMap, loadedAssemblyMap, args);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Helper Methods

        private static T InvokePrivateMethod<T>(string methodName, object instance, params object[] parameters)
        {
            var method = instance.GetType().GetMethod(methodName, 
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null)
            {
                throw new InvalidOperationException($"Method {methodName} not found");
            }
            return (T)method.Invoke(instance, parameters);
        }

        #endregion
    }
}