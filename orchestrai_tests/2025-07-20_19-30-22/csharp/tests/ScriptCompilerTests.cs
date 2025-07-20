using Xunit;
using Moq;
using Dotnet.Script.Core;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptCompilerTests
    {
        private readonly Mock<ICompilationDependencyResolver> _mockDependencyResolver;
        private readonly Mock<Logger> _mockLogger;
        private readonly ScriptCompiler _compiler;

        public ScriptCompilerTests()
        {
            _mockDependencyResolver = new Mock<ICompilationDependencyResolver>();
            _mockLogger = new Mock<Logger>();
            _compiler = new ScriptCompiler(_mockDependencyResolver.Object, _mockLogger.Object);
        }

        [Fact]
        public void CreateCompilationContext_WithValidScript_ReturnsContext()
        {
            // Arrange
            var scriptContext = new ScriptContext
            {
                SourceText = "Console.WriteLine(\"Hello World\");",
                WorkingDirectory = "/tmp",
                Args = new string[0]
            };

            _mockDependencyResolver.Setup(x => x.GetDependencies(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
                                  .Returns(new List<CompilationDependency>());

            // Act
            var result = _compiler.CreateCompilationContext(scriptContext);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Compilation);
        }

        [Fact]
        public void CreateCompilationContext_WithReferences_IncludesReferences()
        {
            // Arrange
            var scriptContext = new ScriptContext
            {
                SourceText = "#r \"System.IO\"\nConsole.WriteLine(\"Hello World\");",
                WorkingDirectory = "/tmp",
                Args = new string[0]
            };

            var dependencies = new List<CompilationDependency>
            {
                new CompilationDependency { Name = "System.IO", AssemblyPaths = new[] { "System.IO.dll" } }
            };

            _mockDependencyResolver.Setup(x => x.GetDependencies(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
                                  .Returns(dependencies);

            // Act
            var result = _compiler.CreateCompilationContext(scriptContext);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Compilation.References.Any());
        }

        [Fact]
        public void CreateCompilationContext_WithSyntaxError_ReturnsContextWithDiagnostics()
        {
            // Arrange
            var scriptContext = new ScriptContext
            {
                SourceText = "Console.WriteLine(\"Hello World\"", // Missing closing parenthesis
                WorkingDirectory = "/tmp",
                Args = new string[0]
            };

            _mockDependencyResolver.Setup(x => x.GetDependencies(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
                                  .Returns(new List<CompilationDependency>());

            // Act
            var result = _compiler.CreateCompilationContext(scriptContext);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Compilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error));
        }
    }
}