using System;
using System.Collections.Generic;
using System.Reflection;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.CodeAnalysis.Text;
using Moq;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class ScriptCompilationContextTests
    {
        [Fact]
        public void Constructor_WithAllValidParameters_ShouldCreateContext()
        {
            // Arrange
            var mockScript = new Mock<Script<int>>();
            var sourceText = SourceText.From("Console.WriteLine(42);");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var runtimeDependencies = new[] { CreateMockRuntimeDependency("TestDep") };
            var diagnostics = new Diagnostic[0];

            // Act
            var context = new ScriptCompilationContext<int>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Assert
            Assert.NotNull(context);
            Assert.Equal(mockScript.Object, context.Script);
            Assert.Equal(sourceText, context.SourceText);
            Assert.Equal(mockLoader.Object, context.Loader);
            Assert.Equal(scriptOptions, context.ScriptOptions);
            Assert.Equal(runtimeDependencies, context.RuntimeDependencies);
            Assert.Empty(context.Warnings);
            Assert.Empty(context.Errors);
        }

        [Fact]
        public void Constructor_WithWarningDiagnostics_ShouldPopulateWarningsArray()
        {
            // Arrange
            var mockScript = new Mock<Script<string>>();
            var sourceText = SourceText.From("var x = 1;");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var runtimeDependencies = Array.Empty<RuntimeDependency>();

            var warningDiagnostic = CreateDiagnosticWithSeverity(DiagnosticSeverity.Warning);
            var diagnostics = new[] { warningDiagnostic };

            // Act
            var context = new ScriptCompilationContext<string>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Assert
            Assert.Single(context.Warnings);
            Assert.Equal(warningDiagnostic, context.Warnings[0]);
            Assert.Empty(context.Errors);
        }

        [Fact]
        public void Constructor_WithErrorDiagnostics_ShouldPopulateErrorsArray()
        {
            // Arrange
            var mockScript = new Mock<Script<bool>>();
            var sourceText = SourceText.From("invalid code;");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var runtimeDependencies = Array.Empty<RuntimeDependency>();

            var errorDiagnostic = CreateDiagnosticWithSeverity(DiagnosticSeverity.Error);
            var diagnostics = new[] { errorDiagnostic };

            // Act
            var context = new ScriptCompilationContext<bool>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Assert
            Assert.Single(context.Errors);
            Assert.Equal(errorDiagnostic, context.Errors[0]);
            Assert.Empty(context.Warnings);
        }

        [Fact]
        public void Constructor_WithMixedDiagnostics_ShouldSeparateWarningsAndErrors()
        {
            // Arrange
            var mockScript = new Mock<Script<double>>();
            var sourceText = SourceText.From("var code = 1;");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var runtimeDependencies = Array.Empty<RuntimeDependency>();

            var warningDiagnostic1 = CreateDiagnosticWithSeverity(DiagnosticSeverity.Warning);
            var errorDiagnostic1 = CreateDiagnosticWithSeverity(DiagnosticSeverity.Error);
            var warningDiagnostic2 = CreateDiagnosticWithSeverity(DiagnosticSeverity.Warning);
            var errorDiagnostic2 = CreateDiagnosticWithSeverity(DiagnosticSeverity.Error);

            var diagnostics = new[] { warningDiagnostic1, errorDiagnostic1, warningDiagnostic2, errorDiagnostic2 };

            // Act
            var context = new ScriptCompilationContext<double>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Assert
            Assert.Equal(2, context.Warnings.Length);
            Assert.Equal(2, context.Errors.Length);
            Assert.Contains(warningDiagnostic1, context.Warnings);
            Assert.Contains(warningDiagnostic2, context.Warnings);
            Assert.Contains(errorDiagnostic1, context.Errors);
            Assert.Contains(errorDiagnostic2, context.Errors);
        }

        [Fact]
        public void Constructor_WithMultipleRuntimeDependencies_ShouldStoreAllDependencies()
        {
            // Arrange
            var mockScript = new Mock<Script<object>>();
            var sourceText = SourceText.From("//test");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var dep1 = CreateMockRuntimeDependency("Dependency1");
            var dep2 = CreateMockRuntimeDependency("Dependency2");
            var dep3 = CreateMockRuntimeDependency("Dependency3");
            var runtimeDependencies = new[] { dep1, dep2, dep3 };
            var diagnostics = Array.Empty<Diagnostic>();

            // Act
            var context = new ScriptCompilationContext<object>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Assert
            Assert.Equal(3, context.RuntimeDependencies.Length);
            Assert.Contains(dep1, context.RuntimeDependencies);
            Assert.Contains(dep2, context.RuntimeDependencies);
            Assert.Contains(dep3, context.RuntimeDependencies);
        }

        [Fact]
        public void Constructor_WithEmptyRuntimeDependencies_ShouldStoreEmptyArray()
        {
            // Arrange
            var mockScript = new Mock<Script<float>>();
            var sourceText = SourceText.From("//empty");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var runtimeDependencies = Array.Empty<RuntimeDependency>();
            var diagnostics = Array.Empty<Diagnostic>();

            // Act
            var context = new ScriptCompilationContext<float>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Assert
            Assert.Empty(context.RuntimeDependencies);
        }

        [Fact]
        public void Constructor_WithInfoDiagnosticSeverity_ShouldNotIncludeInWarningsOrErrors()
        {
            // Arrange
            var mockScript = new Mock<Script<long>>();
            var sourceText = SourceText.From("//info");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var runtimeDependencies = Array.Empty<RuntimeDependency>();

            var infoDiagnostic = CreateDiagnosticWithSeverity(DiagnosticSeverity.Info);
            var diagnostics = new[] { infoDiagnostic };

            // Act
            var context = new ScriptCompilationContext<long>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Assert
            Assert.Empty(context.Warnings);
            Assert.Empty(context.Errors);
        }

        [Fact]
        public void Constructor_WithHiddenDiagnosticSeverity_ShouldNotIncludeInWarningsOrErrors()
        {
            // Arrange
            var mockScript = new Mock<Script<decimal>>();
            var sourceText = SourceText.From("//hidden");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var runtimeDependencies = Array.Empty<RuntimeDependency>();

            var hiddenDiagnostic = CreateDiagnosticWithSeverity(DiagnosticSeverity.Hidden);
            var diagnostics = new[] { hiddenDiagnostic };

            // Act
            var context = new ScriptCompilationContext<decimal>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Assert
            Assert.Empty(context.Warnings);
            Assert.Empty(context.Errors);
        }

        [Fact]
        public void Script_Property_ShouldReturnProvidedScript()
        {
            // Arrange
            var mockScript = new Mock<Script<int>>();
            var sourceText = SourceText.From("42");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var runtimeDependencies = Array.Empty<RuntimeDependency>();
            var diagnostics = Array.Empty<Diagnostic>();

            var context = new ScriptCompilationContext<int>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Act & Assert
            Assert.Equal(mockScript.Object, context.Script);
        }

        [Fact]
        public void SourceText_Property_ShouldReturnProvidedSourceText()
        {
            // Arrange
            var sourceText = SourceText.From("var x = 1;");
            var mockScript = new Mock<Script<string>>();
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var runtimeDependencies = Array.Empty<RuntimeDependency>();
            var diagnostics = Array.Empty<Diagnostic>();

            var context = new ScriptCompilationContext<string>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Act & Assert
            Assert.Equal(sourceText, context.SourceText);
        }

        [Fact]
        public void Loader_Property_ShouldReturnProvidedLoader()
        {
            // Arrange
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var mockScript = new Mock<Script<bool>>();
            var sourceText = SourceText.From("true");
            var scriptOptions = ScriptOptions.Default;
            var runtimeDependencies = Array.Empty<RuntimeDependency>();
            var diagnostics = Array.Empty<Diagnostic>();

            var context = new ScriptCompilationContext<bool>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Act & Assert
            Assert.Equal(mockLoader.Object, context.Loader);
        }

        [Fact]
        public void ScriptOptions_Property_ShouldReturnProvidedScriptOptions()
        {
            // Arrange
            var scriptOptions = ScriptOptions.Default;
            var mockScript = new Mock<Script<object>>();
            var sourceText = SourceText.From("//test");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var runtimeDependencies = Array.Empty<RuntimeDependency>();
            var diagnostics = Array.Empty<Diagnostic>();

            var context = new ScriptCompilationContext<object>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Act & Assert
            Assert.Equal(scriptOptions, context.ScriptOptions);
        }

        [Fact]
        public void RuntimeDependencies_Property_ShouldReturnProvidedDependencies()
        {
            // Arrange
            var runtimeDependencies = new[] { CreateMockRuntimeDependency("Dep1"), CreateMockRuntimeDependency("Dep2") };
            var mockScript = new Mock<Script<int>>();
            var sourceText = SourceText.From("1");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var diagnostics = Array.Empty<Diagnostic>();

            var context = new ScriptCompilationContext<int>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Act & Assert
            Assert.Equal(runtimeDependencies, context.RuntimeDependencies);
        }

        [Fact]
        public void Constructor_WithDifferentGenericTypes_ShouldWork()
        {
            // Arrange
            var mockScript = new Mock<Script<string>>();
            var sourceText = SourceText.From("\"test\"");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var runtimeDependencies = Array.Empty<RuntimeDependency>();
            var diagnostics = Array.Empty<Diagnostic>();

            // Act
            var context = new ScriptCompilationContext<string>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Assert
            Assert.NotNull(context);
            Assert.IsType<ScriptCompilationContext<string>>(context);
        }

        [Fact]
        public void Constructor_FiltersDiagnosticsCorrectly_WithOnlyErrors()
        {
            // Arrange
            var mockScript = new Mock<Script<int>>();
            var sourceText = SourceText.From("code");
            var mockLoader = new Mock<InteractiveAssemblyLoader>();
            var scriptOptions = ScriptOptions.Default;
            var runtimeDependencies = Array.Empty<RuntimeDependency>();

            var error1 = CreateDiagnosticWithSeverity(DiagnosticSeverity.Error);
            var error2 = CreateDiagnosticWithSeverity(DiagnosticSeverity.Error);
            var info = CreateDiagnosticWithSeverity(DiagnosticSeverity.Info);
            var hidden = CreateDiagnosticWithSeverity(DiagnosticSeverity.Hidden);

            var diagnostics = new[] { error1, error2, info, hidden };

            // Act
            var context = new ScriptCompilationContext<int>(
                mockScript.Object,
                sourceText,
                mockLoader.Object,
                scriptOptions,
                runtimeDependencies,
                diagnostics
            );

            // Assert
            Assert.Equal(2, context.Errors.Length);
            Assert.Empty(context.Warnings);
        }

        // Helper methods
        private RuntimeDependency CreateMockRuntimeDependency(string name)
        {
            return new RuntimeDependency(
                name,
                null,
                null,
                null,
                Array.Empty<RuntimeAssembly>(),
                Array.Empty<string>()
            );
        }

        private Diagnostic CreateDiagnosticWithSeverity(DiagnosticSeverity severity)
        {
            var mockDiagnostic = new Mock<Diagnostic>();
            mockDiagnostic.Setup(d => d.Severity).Returns(severity);
            return mockDiagnostic.Object;
        }
    }
}