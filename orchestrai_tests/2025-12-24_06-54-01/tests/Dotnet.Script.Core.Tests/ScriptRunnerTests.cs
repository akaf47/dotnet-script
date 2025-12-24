using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Environment;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using Moq;
using Xunit;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptRunnerTests
    {
        private Mock<ScriptCompiler> _mockScriptCompiler;
        private Mock<LogFactory> _mockLogFactory;
        private Mock<ScriptConsole> _mockScriptConsole;
        private ScriptRunner _scriptRunner;

        public ScriptRunnerTests()
        {
            _mockLogFactory = new Mock<LogFactory>();
            _mockScriptConsole = new Mock<ScriptConsole>(null, null, null);
            _mockScriptCompiler = new Mock<ScriptCompiler>(_mockLogFactory.Object, false);

            _mockLogFactory.Setup(lf => lf.CreateLogger<ScriptRunner>())
                .Returns(new Mock<Logger>().Object);

            _scriptRunner = new ScriptRunner(_mockScriptCompiler.Object, _mockLogFactory.Object, _mockScriptConsole.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldInitializeWithValidParameters()
        {
            // Arrange
            var mockCompiler = new Mock<ScriptCompiler>(_mockLogFactory.Object, false);
            var mockConsole = new Mock<ScriptConsole>(null, null, null);

            // Act
            var runner = new ScriptRunner(mockCompiler.Object, _mockLogFactory.Object, mockConsole.Object);

            // Assert
            Assert.NotNull(runner);
            Assert.NotNull(runner.ScriptCompiler);
            Assert.NotNull(runner.ScriptConsole);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenScriptCompilerIsNull()
        {
            // Act & Assert
            Assert.Throws<NullReferenceException>(() =>
                new ScriptRunner(null, _mockLogFactory.Object, _mockScriptConsole.Object));
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenLogFactoryIsNull()
        {
            // Act & Assert
            Assert.Throws<NullReferenceException>(() =>
                new ScriptRunner(_mockScriptCompiler.Object, null, _mockScriptConsole.Object));
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenScriptConsoleIsNull()
        {
            // Act & Assert
            Assert.Throws<NullReferenceException>(() =>
                new ScriptRunner(_mockScriptCompiler.Object, _mockLogFactory.Object, null));
        }

        #endregion

        #region Execute<TReturn>(ScriptContext) Tests

        [Fact]
        public async Task ExecuteWithScriptContext_ShouldCallExecuteGeneric()
        {
            // Arrange
            var code = SourceText.From("return 42;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new[] { "arg1", "arg2" });
            
            var mockScript = new Mock<Script<int>>();
            var mockState = new Mock<ScriptState<int>>();
            mockState.Setup(s => s.Exception).Returns((Exception)null);
            mockState.Setup(s => s.ReturnValue).Returns(42);
            
            mockScript.Setup(s => s.RunAsync(It.IsAny<CommandLineScriptGlobals>(), It.IsAny<Func<Exception, bool>>()))
                .ReturnsAsync(mockState.Object);

            var mockCompilationContext = new ScriptCompilationContext<int>(
                mockScript.Object, code, new Mock<InteractiveAssemblyLoader>().Object, 
                ScriptOptions.Default, Array.Empty<RuntimeDependency>(), Array.Empty<Diagnostic>());

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, CommandLineScriptGlobals>(context))
                .Returns(mockCompilationContext);

            _mockScriptConsole.Setup(c => c.WriteDiagnostics(It.IsAny<Diagnostic[]>(), It.IsAny<Diagnostic[]>()));

            // Act
            var result = await _scriptRunner.Execute<int>(context);

            // Assert
            Assert.Equal(42, result);
            _mockScriptCompiler.Verify(c => c.CreateCompilationContext<int, CommandLineScriptGlobals>(context), Times.Once);
        }

        [Fact]
        public async Task ExecuteWithScriptContext_ShouldAddCommandLineArgs()
        {
            // Arrange
            var code = SourceText.From("return Args.Count;");
            var args = new[] { "arg1", "arg2", "arg3" };
            var context = new ScriptContext(code, Environment.CurrentDirectory, args);

            var mockScript = new Mock<Script<int>>();
            var mockState = new Mock<ScriptState<int>>();
            mockState.Setup(s => s.Exception).Returns((Exception)null);
            mockState.Setup(s => s.ReturnValue).Returns(3);

            mockScript.Setup(s => s.RunAsync(It.IsAny<CommandLineScriptGlobals>(), It.IsAny<Func<Exception, bool>>()))
                .Callback<CommandLineScriptGlobals, Func<Exception, bool>>((globals, _) =>
                {
                    Assert.Equal(3, globals.Args.Count);
                })
                .ReturnsAsync(mockState.Object);

            var mockCompilationContext = new ScriptCompilationContext<int>(
                mockScript.Object, code, new Mock<InteractiveAssemblyLoader>().Object,
                ScriptOptions.Default, Array.Empty<RuntimeDependency>(), Array.Empty<Diagnostic>());

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, CommandLineScriptGlobals>(context))
                .Returns(mockCompilationContext);

            _mockScriptConsole.Setup(c => c.WriteDiagnostics(It.IsAny<Diagnostic[]>(), It.IsAny<Diagnostic[]>()));

            // Act
            var result = await _scriptRunner.Execute<int>(context);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task ExecuteWithScriptContext_ShouldHandleEmptyArgs()
        {
            // Arrange
            var code = SourceText.From("return 0;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new string[0]);

            var mockScript = new Mock<Script<int>>();
            var mockState = new Mock<ScriptState<int>>();
            mockState.Setup(s => s.Exception).Returns((Exception)null);
            mockState.Setup(s => s.ReturnValue).Returns(0);

            mockScript.Setup(s => s.RunAsync(It.IsAny<CommandLineScriptGlobals>(), It.IsAny<Func<Exception, bool>>()))
                .ReturnsAsync(mockState.Object);

            var mockCompilationContext = new ScriptCompilationContext<int>(
                mockScript.Object, code, new Mock<InteractiveAssemblyLoader>().Object,
                ScriptOptions.Default, Array.Empty<RuntimeDependency>(), Array.Empty<Diagnostic>());

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, CommandLineScriptGlobals>(context))
                .Returns(mockCompilationContext);

            _mockScriptConsole.Setup(c => c.WriteDiagnostics(It.IsAny<Diagnostic[]>(), It.IsAny<Diagnostic[]>()));

            // Act
            var result = await _scriptRunner.Execute<int>(context);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ExecuteWithScriptContext_ShouldThrowCompilationErrorException_WhenCompilationFails()
        {
            // Arrange
            var code = SourceText.From("invalid syntax");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new string[0]);

            var errorDiagnostic = new Mock<Diagnostic>();
            var diagnostics = new[] { errorDiagnostic.Object };

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, CommandLineScriptGlobals>(context))
                .Throws(new CompilationErrorException("Compilation failed", ImmutableArray<Diagnostic>.Empty));

            // Act & Assert
            await Assert.ThrowsAsync<CompilationErrorException>(() => _scriptRunner.Execute<int>(context));
        }

        [Fact]
        public async Task ExecuteWithScriptContext_ShouldWriteDiagnostics()
        {
            // Arrange
            var code = SourceText.From("return 42;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new string[0]);

            var mockScript = new Mock<Script<int>>();
            var mockState = new Mock<ScriptState<int>>();
            mockState.Setup(s => s.Exception).Returns((Exception)null);
            mockState.Setup(s => s.ReturnValue).Returns(42);

            mockScript.Setup(s => s.RunAsync(It.IsAny<CommandLineScriptGlobals>(), It.IsAny<Func<Exception, bool>>()))
                .ReturnsAsync(mockState.Object);

            var warningDiag = new Mock<Diagnostic>();
            var diagnostics = new[] { warningDiag.Object };

            var mockCompilationContext = new ScriptCompilationContext<int>(
                mockScript.Object, code, new Mock<InteractiveAssemblyLoader>().Object,
                ScriptOptions.Default, Array.Empty<RuntimeDependency>(), diagnostics);

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, CommandLineScriptGlobals>(context))
                .Returns(mockCompilationContext);

            _mockScriptConsole.Setup(c => c.WriteDiagnostics(It.IsAny<Diagnostic[]>(), It.IsAny<Diagnostic[]>()));

            // Act
            var result = await _scriptRunner.Execute<int>(context);

            // Assert
            _mockScriptConsole.Verify(c => c.WriteDiagnostics(It.IsAny<Diagnostic[]>(), It.IsAny<Diagnostic[]>()), Times.Once);
        }

        #endregion

        #region Execute<TReturn, THost>(ScriptContext, THost) Tests

        [Fact]
        public async Task ExecuteWithHost_ShouldSuccessfullyExecuteScript()
        {
            // Arrange
            var code = SourceText.From("return 42;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new string[0]);
            var host = new TestHost();

            var mockScript = new Mock<Script<int>>();
            var mockState = new Mock<ScriptState<int>>();
            mockState.Setup(s => s.Exception).Returns((Exception)null);
            mockState.Setup(s => s.ReturnValue).Returns(42);

            mockScript.Setup(s => s.RunAsync(It.IsAny<TestHost>(), It.IsAny<Func<Exception, bool>>()))
                .ReturnsAsync(mockState.Object);

            var mockCompilationContext = new ScriptCompilationContext<int>(
                mockScript.Object, code, new Mock<InteractiveAssemblyLoader>().Object,
                ScriptOptions.Default, Array.Empty<RuntimeDependency>(), Array.Empty<Diagnostic>());

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, TestHost>(context))
                .Returns(mockCompilationContext);

            _mockScriptConsole.Setup(c => c.WriteDiagnostics(It.IsAny<Diagnostic[]>(), It.IsAny<Diagnostic[]>()));

            // Act
            var result = await _scriptRunner.Execute<int, TestHost>(context, host);

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public async Task ExecuteWithHost_ShouldThrowCompilationErrorException_WhenCompilationFails()
        {
            // Arrange
            var code = SourceText.From("invalid");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new string[0]);
            var host = new TestHost();

            var errorDiagnostic = new Mock<Diagnostic>();
            errorDiagnostic.Setup(d => d.Severity).Returns(DiagnosticSeverity.Error);

            var mockScript = new Mock<Script<int>>();
            var mockCompilationContext = new ScriptCompilationContext<int>(
                mockScript.Object, code, new Mock<InteractiveAssemblyLoader>().Object,
                ScriptOptions.Default, Array.Empty<RuntimeDependency>(), 
                new[] { errorDiagnostic.Object });

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, TestHost>(context))
                .Returns(mockCompilationContext);

            _mockScriptConsole.Setup(c => c.WriteDiagnostics(It.IsAny<Diagnostic[]>(), It.IsAny<Diagnostic[]>()));

            // Act & Assert
            await Assert.ThrowsAsync<CompilationErrorException>(() => _scriptRunner.Execute<int, TestHost>(context, host));
        }

        [Fact]
        public async Task ExecuteWithHost_ShouldHandleCompilationWarnings()
        {
            // Arrange
            var code = SourceText.From("return 42;");
            var context = new ScriptContext(code, Environment.CurrentDirectory, new string[0]);
            var host = new TestHost();

            var warningDiagnostic = new Mock<Diagnostic>();
            warningDiagnostic.Setup(d => d.Severity).Returns(DiagnosticSeverity.Warning);

            var mockScript = new Mock<Script<int>>();
            var mockState = new Mock<ScriptState<int>>();
            mockState.Setup(s => s.Exception).Returns((Exception)null);
            mockState.Setup(s => s.ReturnValue).Returns(42);

            mockScript.Setup(s => s.RunAsync(It.IsAny<TestHost>(), It.IsAny<Func<Exception, bool>>()))
                .ReturnsAsync(mockState.Object);

            var mockCompilationContext = new ScriptCompilationContext<int>(
                mockScript.Object, code, new Mock<InteractiveAssemblyLoader>().Object,
                ScriptOptions.Default, Array.Empty<RuntimeDependency>(), 
                new[] { warningDiagnostic.Object });

            _mockScriptCompiler.Setup(c => c.CreateCompilationContext<int, TestHost>(context))
                .Returns(mockCompilationContext);

            _mockScriptConsole.Setup(c => c.WriteDiagnostics(It.IsAny<Diagnostic[]>(), It.IsAny<Diagnostic[]>()));

            // Act
            var result = await _scriptRunner.Execute<int, TestHost>(context, host);

            // Assert
            Assert.Equal(42, result);
        }

        #endregion

        #region Execute<TReturn, THost>(ScriptCompilationContext, THost) Tests

        [Fact]
        public async Task ExecuteWithCompilationContext_ShouldReturnScriptResult()
        {
            // Arrange
            var code = SourceText.From("return 42;");
            var mockScript = new Mock<Script<int>>();
            var mockState = new Mock<ScriptState<int>>();
            mockState.Setup(s => s.Exception).Returns((Exception)null);
            mockState.Setup(s => s.ReturnValue).Returns(42);

            mockScript.Setup(s => s.RunAsync(It.IsAny<TestHost>(), It.IsAny<Func<Exception, bool>>()))
                .ReturnsAsync(mockState.Object);

            var compilationContext = new ScriptCompilationContext<int>(
                mockScript.Object, code, new Mock<InteractiveAssemblyLoader>().Object,
                ScriptOptions.Default, Array.Empty<RuntimeDependency>(), Array.Empty<Diagnostic>());

            var host = new TestHost();

            // Act
            var result = await _scriptRunner.Execute<int, TestHost>(compilationContext, host);

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public async Task ExecuteWithCompilationContext_ShouldThrowScriptRuntimeException_WhenScriptThrows()
        {
            // Arrange
            var code = SourceText.From("throw new System.Exception(\"Script error\");");
            var mockScript = new Mock<Script<int>>();
            var scriptException = new InvalidOperationException("Test exception");
            var mockState = new Mock<ScriptState<int>>();
            mockState.Setup(s => s.Exception).Returns(scriptException);
            mockState.Setup(s => s.ReturnValue).Returns(0);

            mockScript.Setup(s => s.RunAsync(It.IsAny<TestHost>(), It.IsAny<Func<Exception, bool>>()))
                .ReturnsAsync(mockState.Object);

            var compilationContext = new ScriptCompilationContext<int>(
                mockScript.Object, code, new Mock<InteractiveAssemblyLoader>().Object,
                ScriptOptions.Default, Array.Empty<RuntimeDependency>(), Array.Empty<Diagnostic>());

            var host = new TestHost();

            _mockScriptConsole.Setup(c => c.WriteError(It.IsAny<string>()));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ScriptRuntimeException>(() => _scriptRunner.Execute<int, TestHost>(compilationContext, host));
            Assert.IsType<ScriptRuntimeException>(ex);
            Assert.Equal("Script execution resulted in an exception.", ex.Message);
            Assert.Equal(scriptException, ex.InnerException);
            _mockScriptConsole.Verify(c => c.WriteError(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteWithCompilationContext_ShouldConfigureExceptionHandler()
        {
            // Arrange
            var code = SourceText.From("return 42;");
            var mockScript = new Mock<Script<int>>();
            var mockState = new Mock<ScriptState<int>>();
            mockState.Setup(s => s.Exception).Returns((Exception)null);
            mockState.Setup(s => s.ReturnValue).Returns(42);

            Func<Exception, bool> capturedExceptionHandler = null;

            mockScript.Setup(s => s.RunAsync(It.IsAny<TestHost>(), It.IsAny<Func<Exception, bool>>()))
                .Callback<TestHost, Func<Exception, bool>>((_, handler) =>
                {
                    capturedExceptionHandler = handler;
                })
                .ReturnsAsync(mockState.Object);

            var compilationContext = new ScriptCompilationContext<int>(
                mockScript.Object, code, new Mock<InteractiveAssemblyLoader>().Object,
                ScriptOptions.Default, Array.Empty<RuntimeDependency>(), Array.Empty<Diagnostic>());

            var host = new TestHost();

            // Act
            var result = await _scriptRunner.Execute<int, TestHost>(compilationContext, host);

            // Assert
            Assert.NotNull(capturedExceptionHandler);
            Assert.True(capturedExceptionHandler(new Exception("test")));
        }

        #endregion

        #region ResolveAssembly Tests

        [Fact]
        public void ResolveAssembly_ShouldReturnNull_WhenAssemblyNotInDependencies()
        {
            // Arrange
            var pal = new Mock<AssemblyLoadPal>().Object;
            var assemblyName = new AssemblyName("NonExistentAssembly");
            var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>();

            // Act
            var result = _scriptRunner.ResolveAssembly(pal, assemblyName, runtimeDepsMap);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ResolveAssembly_ShouldReturnAssembly_WhenFoundInDependencies()
        {
            // Arrange
            var tempAssemblyPath = Path.Combine(Path.GetTempPath(), "TestAssembly.dll");
            File.WriteAllBytes(tempAssemblyPath, new byte[] { });

            try
            {
                var mockPal = new Mock<AssemblyLoadPal>();
                var testAssembly = typeof(ScriptRunnerTests).Assembly;
                mockPal.Setup(p => p.LoadFrom(tempAssemblyPath)).Returns(testAssembly);

                var assemblyName = new AssemblyName("TestAssembly");
                var runtimeAssembly = new RuntimeAssembly(new AssemblyName("TestAssembly"), tempAssemblyPath);
                var runtimeDepsMap = new Dictionary<string, RuntimeAssembly> { { "TestAssembly", runtimeAssembly } };

                // Act
                var result = _scriptRunner.ResolveAssembly(mockPal.Object, assemblyName, runtimeDepsMap);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(testAssembly.GetName().Name, result.GetName().Name);
            }
            finally
            {
                if (File.Exists(tempAssemblyPath))
                    File.Delete(tempAssemblyPath);
            }
        }

        [Fact]
        public void ResolveAssembly_ShouldBeCase_Insensitive()
        {
            // Arrange
            var tempAssemblyPath = Path.Combine(Path.GetTempPath(), "TestAssembly.dll");
            File.WriteAllBytes(tempAssemblyPath, new byte[] { });

            try
            {
                var mockPal = new Mock<AssemblyLoadPal>();
                var testAssembly = typeof(ScriptRunnerTests).Assembly;
                mockPal.Setup(p => p.LoadFrom(tempAssemblyPath)).Returns(testAssembly);

                var assemblyName = new AssemblyName("testassembly"); // lowercase
                var runtimeAssembly = new RuntimeAssembly(new AssemblyName("TestAssembly"), tempAssemblyPath);
                var runtimeDepsMap = new Dictionary<string, RuntimeAssembly> { { "TestAssembly", runtimeAssembly } };

                // Act - This should work due to case-insensitive comparison
                var result = _scriptRunner.ResolveAssembly(mockPal.Object, assemblyName, runtimeDepsMap);

                // Assert
                Assert.NotNull(result);
            }
            finally
            {
                if (File.Exists(tempAssemblyPath))
                    File.Delete(tempAssemblyPath);
            }
        }

        #endregion

        #region ProcessScriptState Tests

        [Fact]
        public void ProcessScriptState_ShouldReturnReturnValue_WhenNoException()
        {
            // Arrange
            var mockState = new Mock<ScriptState<int>>();
            mockState.Setup(s => s.Exception).Returns((Exception)null);
            mockState.Setup(s => s.ReturnValue).Returns(42);

            // Act
            var result = _scriptRunner.ProcessScriptState<int>(mockState.Object);

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public void ProcessScriptState_ShouldThrowScriptRuntimeException_WhenExceptionExists()
        {
            // Arrange
            var exception = new InvalidOperationException("Script error");
            var mockState = new Mock<ScriptState<int>>();
            mockState.Setup(s => s.Exception).Returns(exception);
            mockState.Setup(s => s.ReturnValue).Returns(0);

            _mockScriptConsole.Setup(c => c.WriteError(It.IsAny<string>()));

            // Act & Assert
            var ex = Assert.Throws<ScriptRuntimeException>(() => _scriptRunner.ProcessScriptState<int>(mockState.Object));
            Assert.Equal("Script execution resulted in an exception.", ex.Message);
            Assert.Equal(exception, ex.InnerException);
        }

        [Fact]
        public void ProcessScriptState_ShouldWriteErrorToConsole_WhenExceptionExists()
        {
            // Arrange
            var exception = new InvalidOperationException("Test error");
            var mockState = new Mock<ScriptState<int>>();
            mockState.Setup(s => s.Exception).Returns(exception);

            _mockScriptConsole.Setup(c => c.WriteError(It.IsAny<string>()));

            // Act
            try
            {
                _scriptRunner.ProcessScriptState<int>(mockState.Object);
            }
            catch (ScriptRuntimeException)
            {
                // Expected
            }

            // Assert
            _mockScriptConsole.Verify(c => c.WriteError(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ProcessScriptState_ShouldHandleStringReturnType()
        {
            // Arrange
            var mockState = new Mock<ScriptState<string>>();
            mockState.Setup(s => s.Exception).Returns((Exception)null);
            mockState.Setup(s => s.ReturnValue).Returns("test result");

            // Act
            var result = _scriptRunner.ProcessScriptState<string>(mockState.Object);

            // Assert
            Assert.Equal("test result", result);
        }

        [Fact]
        public void ProcessScriptState_ShouldHandleNullReturnValue()
        {
            // Arrange
            var mockState = new Mock<ScriptState<string>>();
            mockState.Setup(s => s.Exception).Returns((Exception)null);
            mockState.Setup(s => s.ReturnValue).Returns((string)null);

            // Act
            var result = _scriptRunner.ProcessScriptState<string>(mockState.Object);

            // Assert
            Assert.Null(result);
        }

        #endregion

        // Helper class for testing
        private class TestHost
        {
        }
    }
}