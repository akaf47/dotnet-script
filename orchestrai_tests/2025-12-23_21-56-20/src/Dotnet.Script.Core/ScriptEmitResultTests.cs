using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptEmitResultTests
    {
        [Fact]
        public void Constructor_WithValidParameters_InitializesPropertiesCorrectly()
        {
            // Arrange
            var peStream = new MemoryStream(new byte[] { 1, 2, 3 });
            var metadataReferences = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];

            // Act
            var result = new ScriptEmitResult(peStream, metadataReferences, runtimeDependencies);

            // Assert
            Assert.NotNull(result.PeStream);
            Assert.Same(peStream, result.PeStream);
            Assert.NotNull(result.RuntimeDependencies);
            Assert.Empty(result.RuntimeDependencies);
            Assert.NotNull(result.DirectiveReferences);
            Assert.Empty(result.DirectiveReferences);
            Assert.NotNull(result.Diagnostics);
            Assert.Empty(result.Diagnostics);
        }

        [Fact]
        public void Constructor_WithNullMetadataReferences_InitializesDirectiveReferencesAsEmpty()
        {
            // Arrange
            var peStream = new MemoryStream();
            var runtimeDependencies = new RuntimeDependency[0];

            // Act
            var result = new ScriptEmitResult(peStream, new List<MetadataReference>(), runtimeDependencies);

            // Assert
            Assert.True(result.DirectiveReferences.IsEmpty);
        }

        [Fact]
        public void Constructor_WithMultipleMetadataReferences_ConvertsToImmutableArray()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location)
            };
            var runtimeDependencies = new RuntimeDependency[0];

            // Act
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);

            // Assert
            Assert.NotEmpty(result.DirectiveReferences);
            Assert.Equal(2, result.DirectiveReferences.Length);
        }

        [Fact]
        public void Constructor_WithRuntimeDependencies_StoresThemCorrectly()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new[]
            {
                new RuntimeDependency("TestDep1", "1.0.0", new List<RuntimeAssembly>(), new List<string>(), new List<string>()),
                new RuntimeDependency("TestDep2", "2.0.0", new List<RuntimeAssembly>(), new List<string>(), new List<string>())
            };

            // Act
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);

            // Assert
            Assert.NotNull(result.RuntimeDependencies);
            Assert.Equal(2, result.RuntimeDependencies.Length);
            Assert.Equal("TestDep1", result.RuntimeDependencies[0].Name);
            Assert.Equal("TestDep2", result.RuntimeDependencies[1].Name);
        }

        [Fact]
        public void Constructor_WithEmptyRuntimeDependencies_InitializesEmptyArray()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];

            // Act
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);

            // Assert
            Assert.NotNull(result.RuntimeDependencies);
            Assert.Empty(result.RuntimeDependencies);
        }

        [Fact]
        public void DefaultConstructor_CreatesInstanceWithDefaultValues()
        {
            // Act
            var result = ScriptEmitResult.Error(new List<Diagnostic>());

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.PeStream);
            Assert.Null(result.RuntimeDependencies);
            Assert.NotNull(result.Diagnostics);
        }

        [Fact]
        public void Success_WithNoDiagnostics_ReturnsTrue()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);

            // Act
            var success = result.Success;

            // Assert
            Assert.True(success);
        }

        [Fact]
        public void Success_WithOnlyWarningDiagnostics_ReturnsTrue()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);
            
            var descriptor = new DiagnosticDescriptor("W001", "Warning", "This is a warning", "Test", DiagnosticSeverity.Warning, true);
            var diagnostic = Diagnostic.Create(descriptor, Location.None);
            result.Diagnostics = ImmutableArray.Create(diagnostic);

            // Act
            var success = result.Success;

            // Assert
            Assert.True(success);
        }

        [Fact]
        public void Success_WithOnlyInfoDiagnostics_ReturnsTrue()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);
            
            var descriptor = new DiagnosticDescriptor("I001", "Info", "This is info", "Test", DiagnosticSeverity.Info, true);
            var diagnostic = Diagnostic.Create(descriptor, Location.None);
            result.Diagnostics = ImmutableArray.Create(diagnostic);

            // Act
            var success = result.Success;

            // Assert
            Assert.True(success);
        }

        [Fact]
        public void Success_WithErrorDiagnostic_ReturnsFalse()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);
            
            var descriptor = new DiagnosticDescriptor("E001", "Error", "This is an error", "Test", DiagnosticSeverity.Error, true);
            var diagnostic = Diagnostic.Create(descriptor, Location.None);
            result.Diagnostics = ImmutableArray.Create(diagnostic);

            // Act
            var success = result.Success;

            // Assert
            Assert.False(success);
        }

        [Fact]
        public void Success_WithMultipleDiagnosticsIncludingError_ReturnsFalse()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);
            
            var warningDescriptor = new DiagnosticDescriptor("W001", "Warning", "This is a warning", "Test", DiagnosticSeverity.Warning, true);
            var warningDiagnostic = Diagnostic.Create(warningDescriptor, Location.None);
            
            var errorDescriptor = new DiagnosticDescriptor("E001", "Error", "This is an error", "Test", DiagnosticSeverity.Error, true);
            var errorDiagnostic = Diagnostic.Create(errorDescriptor, Location.None);
            
            result.Diagnostics = ImmutableArray.Create(warningDiagnostic, errorDiagnostic);

            // Act
            var success = result.Success;

            // Assert
            Assert.False(success);
        }

        [Fact]
        public void Success_WithMultipleErrorDiagnostics_ReturnsFalse()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);
            
            var errorDescriptor1 = new DiagnosticDescriptor("E001", "Error1", "First error", "Test", DiagnosticSeverity.Error, true);
            var errorDiagnostic1 = Diagnostic.Create(errorDescriptor1, Location.None);
            
            var errorDescriptor2 = new DiagnosticDescriptor("E002", "Error2", "Second error", "Test", DiagnosticSeverity.Error, true);
            var errorDiagnostic2 = Diagnostic.Create(errorDescriptor2, Location.None);
            
            result.Diagnostics = ImmutableArray.Create(errorDiagnostic1, errorDiagnostic2);

            // Act
            var success = result.Success;

            // Assert
            Assert.False(success);
        }

        [Fact]
        public void Error_WithEmptyDiagnosticsList_CreateResultWithEmptyDiagnostics()
        {
            // Arrange
            var diagnostics = new List<Diagnostic>();

            // Act
            var result = ScriptEmitResult.Error(diagnostics);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Diagnostics);
            Assert.Empty(result.Diagnostics);
            Assert.True(result.Success);
        }

        [Fact]
        public void Error_WithSingleErrorDiagnostic_CreatesResultWithError()
        {
            // Arrange
            var errorDescriptor = new DiagnosticDescriptor("E001", "Error", "This is an error", "Test", DiagnosticSeverity.Error, true);
            var errorDiagnostic = Diagnostic.Create(errorDescriptor, Location.None);
            var diagnostics = new List<Diagnostic> { errorDiagnostic };

            // Act
            var result = ScriptEmitResult.Error(diagnostics);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Diagnostics);
            Assert.Single(result.Diagnostics);
            Assert.False(result.Success);
        }

        [Fact]
        public void Error_WithMultipleDiagnostics_CreatesResultWithAllDiagnostics()
        {
            // Arrange
            var errorDescriptor = new DiagnosticDescriptor("E001", "Error", "This is an error", "Test", DiagnosticSeverity.Error, true);
            var errorDiagnostic = Diagnostic.Create(errorDescriptor, Location.None);
            
            var warningDescriptor = new DiagnosticDescriptor("W001", "Warning", "This is a warning", "Test", DiagnosticSeverity.Warning, true);
            var warningDiagnostic = Diagnostic.Create(warningDescriptor, Location.None);
            
            var diagnostics = new List<Diagnostic> { errorDiagnostic, warningDiagnostic };

            // Act
            var result = ScriptEmitResult.Error(diagnostics);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Diagnostics);
            Assert.Equal(2, result.Diagnostics.Length);
            Assert.False(result.Success);
        }

        [Fact]
        public void Error_ConvertsEnumerableDiagnosticsToImmutableArray()
        {
            // Arrange
            var errorDescriptor = new DiagnosticDescriptor("E001", "Error", "This is an error", "Test", DiagnosticSeverity.Error, true);
            var errorDiagnostic = Diagnostic.Create(errorDescriptor, Location.None);
            
            IEnumerable<Diagnostic> diagnostics = new[] { errorDiagnostic }.AsEnumerable();

            // Act
            var result = ScriptEmitResult.Error(diagnostics);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ImmutableArray<Diagnostic>>(result.Diagnostics);
        }

        [Fact]
        public void Error_WithWarningOnlyDiagnostics_SuccessIsTrue()
        {
            // Arrange
            var warningDescriptor = new DiagnosticDescriptor("W001", "Warning", "This is a warning", "Test", DiagnosticSeverity.Warning, true);
            var warningDiagnostic = Diagnostic.Create(warningDescriptor, Location.None);
            var diagnostics = new List<Diagnostic> { warningDiagnostic };

            // Act
            var result = ScriptEmitResult.Error(diagnostics);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void Diagnostics_DefaultValue_IsEmptyImmutableArray()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];

            // Act
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);

            // Assert
            Assert.NotNull(result.Diagnostics);
            Assert.True(result.Diagnostics.IsEmpty);
            Assert.IsType<ImmutableArray<Diagnostic>>(result.Diagnostics);
        }

        [Fact]
        public void DirectiveReferences_DefaultValue_IsEmptyImmutableArray()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];

            // Act
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);

            // Assert
            Assert.NotNull(result.DirectiveReferences);
            Assert.True(result.DirectiveReferences.IsEmpty);
            Assert.IsType<ImmutableArray<MetadataReference>>(result.DirectiveReferences);
        }

        [Fact]
        public void PeStream_IsReadOnly()
        {
            // Arrange
            var peStream = new MemoryStream(new byte[] { 1, 2, 3 });
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);

            // Act
            var peStreamProperty = result.GetType().GetProperty("PeStream");

            // Assert
            Assert.NotNull(peStreamProperty);
            Assert.Null(peStreamProperty.SetMethod);
        }

        [Fact]
        public void RuntimeDependencies_IsReadOnly()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);

            // Act
            var runtimeDependenciesProperty = result.GetType().GetProperty("RuntimeDependencies");

            // Assert
            Assert.NotNull(runtimeDependenciesProperty);
            Assert.Null(runtimeDependenciesProperty.SetMethod);
        }

        [Fact]
        public void DirectiveReferences_IsReadOnly()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);

            // Act
            var directiveReferencesProperty = result.GetType().GetProperty("DirectiveReferences");

            // Assert
            Assert.NotNull(directiveReferencesProperty);
            Assert.Null(directiveReferencesProperty.SetMethod);
        }

        [Fact]
        public void Diagnostics_CanBeSet()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);
            
            var errorDescriptor = new DiagnosticDescriptor("E001", "Error", "This is an error", "Test", DiagnosticSeverity.Error, true);
            var errorDiagnostic = Diagnostic.Create(errorDescriptor, Location.None);
            var diagnostics = ImmutableArray.Create(errorDiagnostic);

            // Act
            result.Diagnostics = diagnostics;

            // Assert
            Assert.NotEmpty(result.Diagnostics);
            Assert.Single(result.Diagnostics);
        }

        [Fact]
        public void Success_IsReadOnly()
        {
            // Arrange
            var peStream = new MemoryStream();
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);

            // Act
            var successProperty = result.GetType().GetProperty("Success");

            // Assert
            Assert.NotNull(successProperty);
            Assert.Null(successProperty.SetMethod);
        }

        [Fact]
        public void Constructor_WithLargePeStream_StoresStreamCorrectly()
        {
            // Arrange
            var largeData = new byte[10000];
            for (int i = 0; i < largeData.Length; i++)
            {
                largeData[i] = (byte)(i % 256);
            }
            var peStream = new MemoryStream(largeData);
            var references = new List<MetadataReference>();
            var runtimeDependencies = new RuntimeDependency[0];

            // Act
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);

            // Assert
            Assert.NotNull(result.PeStream);
            Assert.Same(peStream, result.PeStream);
        }

        [Fact]
        public void Error_WithNullDiagnosticsEnumerable_ThrowsException()
        {
            // Arrange
            IEnumerable<Diagnostic> diagnostics = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => ScriptEmitResult.Error(diagnostics));
        }

        [Fact]
        public void Constructor_WithEnumerableMetadataReferences_EnumeratesAndStoresAll()
        {
            // Arrange
            var peStream = new MemoryStream();
            IEnumerable<MetadataReference> references = GetMetadataReferences();
            var runtimeDependencies = new RuntimeDependency[0];

            // Act
            var result = new ScriptEmitResult(peStream, references, runtimeDependencies);

            // Assert
            Assert.NotEmpty(result.DirectiveReferences);
            Assert.Equal(2, result.DirectiveReferences.Length);
        }

        private IEnumerable<MetadataReference> GetMetadataReferences()
        {
            yield return MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            yield return MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location);
        }
    }
}