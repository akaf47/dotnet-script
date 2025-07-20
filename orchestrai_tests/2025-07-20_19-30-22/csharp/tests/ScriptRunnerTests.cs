using Xunit;
using Moq;
using Dotnet.Script.Core;
using System.Threading.Tasks;
using System.IO;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptRunnerTests
    {
        private readonly Mock<IScriptCompiler> _mockCompiler;
        private readonly Mock<IScriptEmitter> _mockEmitter;
        private readonly Mock<Logger> _mockLogger;
        private readonly ScriptRunner _runner;

        public ScriptRunnerTests()
        {
            _mockCompiler = new Mock<IScriptCompiler>();
            _mockEmitter = new Mock<IScriptEmitter>();
            _mockLogger = new Mock<Logger>();
            _runner = new ScriptRunner(_mockCompiler.Object, _mockEmitter.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Execute_WithValidScript_ReturnsSuccess()
        {
            // Arrange
            var scriptContext = new ScriptContext
            {
                SourceText = "Console.WriteLine(\"Hello World\");",
                WorkingDirectory = "/tmp",
                Args = new string[0]
            };

            var compilationContext = new ScriptCompilationContext<object>();
            var emitResult = new ScriptEmitResult
            {
                Success = true,
                AssemblyPath = "/tmp/script.dll"
            };

            _mockCompiler.Setup(x => x.CreateCompilationContext<object>(It.IsAny<ScriptContext>()))
                        .Returns(compilationContext);

            _mockEmitter.Setup(x => x.Emit<object>(It.IsAny<ScriptCompilationContext<object>>(), It.IsAny<string>()))
                       .Returns(emitResult);

            // Act
            var result = await _runner.Execute<object>(scriptContext);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_WithCompilationError_ReturnsError()
        {
            // Arrange
            var scriptContext = new ScriptContext
            {
                SourceText = "invalid syntax",
                WorkingDirectory = "/tmp",
                Args = new string[0]
            };

            var compilationContext = new ScriptCompilationContext<object>();
            var emitResult = new ScriptEmitResult
            {
                Success = false,
                Diagnostics = new[] { "Compilation error" }
            };

            _mockCompiler.Setup(x => x.CreateCompilationContext<object>(It.IsAny<ScriptContext>()))
                        .Returns(compilationContext);

            _mockEmitter.Setup(x => x.Emit<object>(It.IsAny<ScriptCompilationContext<object>>(), It.IsAny<string>()))
                       .Returns(emitResult);

            // Act
            var result = await _runner.Execute<object>(scriptContext);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Execute_WithNullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _runner.Execute<object>(null));
        }
    }
}