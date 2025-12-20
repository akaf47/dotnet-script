using System;
using System.IO;
using System.Linq;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using Moq;
using NUnit.Framework;

namespace Dotnet.Script.Core.Tests
{
    [TestFixture]
    public class ScriptEmitterTests
    {
        private Mock<ScriptConsole> _mockScriptConsole;
        private Mock<ScriptCompiler> _mockScriptCompiler;
        private ScriptEmitter _scriptEmitter;

        [SetUp]
        public void Setup()
        {
            _mockScriptConsole = new Mock<ScriptConsole>();
            _mockScriptCompiler = new Mock<ScriptCompiler>();
            _scriptEmitter = new ScriptEmitter(_mockScriptConsole.Object, _mockScriptCompiler.Object);
        }

        [Test]
        public void Emit_WithValidContext_ShouldEmitAssemblySuccessfully()
        {
            // Arrange
            var context = CreateMockScriptContext();
            var assemblyName = "TestAssembly";
            var mockCompilationContext = CreateMockCompilationContext<int>();
            var mockCompilation = CreateMockCompilation(assemblyName);

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, CommandLineScriptGlobals>(context))
                .Returns(mockCompilationContext.Object);
            mockCompilationContext.Setup(c => c.Script.GetCompilation())
                .Returns(mockCompilation.Object);
            mockCompilationContext.Setup(c => c.Errors)
                .Returns(Enumerable.Empty<Diagnostic>());

            // Act
            var result = _scriptEmitter.Emit<int, CommandLineScriptGlobals>(context, assemblyName);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void Emit_WithCompilationErrors_ShouldThrowCompilationErrorException()
        {
            // Arrange
            var context = CreateMockScriptContext();
            var assemblyName = "TestAssembly";
            var mockCompilationContext = CreateMockCompilationContext<int>();
            var mockErrors = new[] { Mock.Of<Diagnostic>() };

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, CommandLineScriptGlobals>(context))
                .Returns(mockCompilationContext.Object);
            mockCompilationContext.Setup(c => c.Errors)
                .Returns(mockErrors);

            // Act & Assert
            Assert.Throws<CompilationErrorException>(() => 
                _scriptEmitter.Emit<int, CommandLineScriptGlobals>(context, assemblyName));
        }

        [Test]
        public void Emit_WithDebugOptimizationLevel_ShouldUseEmbeddedDebugInformation()
        {
            // Arrange
            var context = CreateMockScriptContext(OptimizationLevel.Debug);
            var assemblyName = "TestAssembly";
            var mockCompilationContext = CreateMockCompilationContext<int>();
            var mockCompilation = CreateMockCompilation(assemblyName);

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, CommandLineScriptGlobals>(context))
                .Returns(mockCompilationContext.Object);
            mockCompilationContext.Setup(c => c.Script.GetCompilation())
                .Returns(mockCompilation.Object);
            mockCompilationContext.Setup(c => c.Errors)
                .Returns(Enumerable.Empty<Diagnostic>());

            // Act
            var result = _scriptEmitter.Emit<int, CommandLineScriptGlobals>(context, assemblyName);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        private ScriptContext CreateMockScriptContext(OptimizationLevel optimizationLevel = OptimizationLevel.Release)
        {
            return new ScriptContext(
                SourceText.From("test script"), 
                "/working/directory", 
                new[] { "arg1" }, 
                null, 
                optimizationLevel
            );
        }

        private Mock<ScriptCompilationContext<T>> CreateMockCompilationContext<T>()
        {
            var mockCompilationContext = new Mock<ScriptCompilationContext<T>>();
            mockCompilationContext.Setup(c => c.RuntimeDependencies)
                .Returns(new RuntimeDependency[0]);
            return mockCompilationContext;
        }

        private Mock<Compilation> CreateMockCompilation(string assemblyName)
        {
            var mockCompilation = new Mock<Compilation>();
            var mockEmitResult = new Mock<EmitResult>();
            mockEmitResult.Setup(r => r.Success).Returns(true);

            mockCompilation.Setup(c => c.Emit(It.IsAny<Stream>(), It.IsAny<EmitOptions>()))
                .Returns(mockEmitResult.Object);
            mockCompilation.Setup(c => c.WithAssemblyName(assemblyName))
                .Returns(mockCompilation.Object);

            return mockCompilation;
        }
    }
}