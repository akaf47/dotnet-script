using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Environment;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis.Scripting;
using Moq;
using NUnit.Framework;

namespace Dotnet.Script.Core.Tests
{
    [TestFixture]
    public class ScriptRunnerTests
    {
        private Mock<ScriptCompiler> _mockScriptCompiler;
        private Mock<LogFactory> _mockLogFactory;
        private Mock<ScriptConsole> _mockScriptConsole;
        private ScriptRunner _scriptRunner;

        [SetUp]
        public void Setup()
        {
            _mockScriptCompiler = new Mock<ScriptCompiler>(MockBehavior.Strict);
            _mockLogFactory = new Mock<LogFactory>();
            _mockScriptConsole = new Mock<ScriptConsole>();

            _mockLogFactory.Setup(f => f.CreateLogger<ScriptRunner>()).Returns(new Mock<Logger>().Object);

            _scriptRunner = new ScriptRunner(_mockScriptCompiler.Object, _mockLogFactory.Object, _mockScriptConsole.Object);
        }

        [Test]
        public async Task Execute_WithDllPath_ShouldRunScriptSuccessfully()
        {
            // Arrange
            string dllPath = "/path/to/script.dll";
            var commandLineArgs = new[] { "arg1", "arg2" };
            var mockRuntimeDeps = new RuntimeDependency[] { };
            var mockRuntimeDepsMap = new Dictionary<string, RuntimeAssembly>();

            _mockScriptCompiler.Setup(c => c.RuntimeDependencyResolver.GetDependenciesForLibrary(dllPath))
                .Returns(mockRuntimeDeps);
            _mockScriptCompiler.Setup(c => c.CreateScriptDependenciesMap(mockRuntimeDeps))
                .Returns(mockRuntimeDepsMap);

            var mockAssembly = CreateMockAssembly();
            var mockMethod = CreateMockMethod<int>();

            // Act
            var result = await _scriptRunner.Execute<int>(dllPath, commandLineArgs);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Execute_WithScriptContext_ShouldRunScriptSuccessfully()
        {
            // Arrange
            var context = CreateMockScriptContext();
            
            // Act
            var result = await _scriptRunner.Execute<int>(context);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Execute_WithCompilationErrors_ShouldThrowCompilationErrorException()
        {
            // Arrange
            var context = CreateMockScriptContext();
            var mockCompilationContext = new Mock<ScriptCompilationContext<int>>();
            var mockErrors = new[] { Mock.Of<Diagnostic>() };

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, CommandLineScriptGlobals>(context))
                .Returns(mockCompilationContext.Object);
            mockCompilationContext.Setup(c => c.Errors).Returns(mockErrors);

            // Act & Assert
            Assert.ThrowsAsync<CompilationErrorException>(async () => 
                await _scriptRunner.Execute<int, CommandLineScriptGlobals>(context, new CommandLineScriptGlobals()));
        }

        [Test]
        public async Task Execute_WithScriptState_ShouldProcessScriptStateCorrectly()
        {
            // Arrange
            var context = CreateMockScriptContext();
            var mockCompilationContext = new Mock<ScriptCompilationContext<int>>();
            var mockScriptState = new Mock<ScriptState<int>>();
            
            mockScriptState.Setup(s => s.ReturnValue).Returns(42);
            mockScriptState.Setup(s => s.Exception).Returns((Exception)null);

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, CommandLineScriptGlobals>(context))
                .Returns(mockCompilationContext.Object);
            mockCompilationContext.Setup(c => c.Script.RunAsync(It.IsAny<CommandLineScriptGlobals>(), It.IsAny<Func<Exception, bool>>()))
                .ReturnsAsync(mockScriptState.Object);

            // Act
            var result = await _scriptRunner.Execute<int, CommandLineScriptGlobals>(context, new CommandLineScriptGlobals());

            // Assert
            Assert.AreEqual(42, result);
        }

        [Test]
        public void Execute_WithScriptException_ShouldThrowScriptRuntimeException()
        {
            // Arrange
            var context = CreateMockScriptContext();
            var mockCompilationContext = new Mock<ScriptCompilationContext<int>>();
            var mockScriptState = new Mock<ScriptState<int>>();
            var mockException = new Exception("Script execution failed");
            
            mockScriptState.Setup(s => s.Exception).Returns(mockException);

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, CommandLineScriptGlobals>(context))
                .Returns(mockCompilationContext.Object);
            mockCompilationContext.Setup(c => c.Script.RunAsync(It.IsAny<CommandLineScriptGlobals>(), It.IsAny<Func<Exception, bool>>()))
                .ReturnsAsync(mockScriptState.Object);

            // Act & Assert
            Assert.ThrowsAsync<ScriptRuntimeException>(async () => 
                await _scriptRunner.Execute<int, CommandLineScriptGlobals>(context, new CommandLineScriptGlobals()));
        }

        private ScriptContext CreateMockScriptContext()
        {
            var mockSourceText = Microsoft.CodeAnalysis.Text.SourceText.From("test script");
            return new ScriptContext(
                mockSourceText, 
                "/working/directory", 
                new[] { "arg1" }, 
                null, 
                Microsoft.CodeAnalysis.OptimizationLevel.Debug
            );
        }

        private Assembly CreateMockAssembly()
        {
            var mockAssembly = new Mock<Assembly>();
            mockAssembly.Setup(a => a.GetType("Submission#0")).Returns(typeof(object));
            return mockAssembly.Object;
        }

        private MethodInfo CreateMockMethod<TReturn>()
        {
            var mockMethod = new Mock<MethodInfo>();
            mockMethod.Setup(m => m.Invoke(null, It.IsAny<object[]>()))
                .Returns(Task.FromResult(default(TReturn)));
            return mockMethod.Object;
        }
    }
}