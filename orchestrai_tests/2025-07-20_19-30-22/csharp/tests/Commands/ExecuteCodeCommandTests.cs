using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Dotnet.Script.DependencyModel.Logging;
using System.Threading.Tasks;

namespace Dotnet.Script.Core.Commands.Tests
{
    public class ExecuteCodeCommandTests
    {
        private readonly Mock<IScriptRunner> _mockScriptRunner;
        private readonly Mock<Logger> _mockLogger;
        private readonly ExecuteCodeCommand _command;

        public ExecuteCodeCommandTests()
        {
            _mockScriptRunner = new Mock<IScriptRunner>();
            _mockLogger = new Mock<Logger>();
            _command = new ExecuteCodeCommand(_mockScriptRunner.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Execute_WithValidCode_ReturnsSuccess()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                Configuration = "Release"
            };

            _mockScriptRunner.Setup(x => x.Execute(It.IsAny<ScriptContext>()))
                           .Returns(Task.FromResult(0));

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            _mockScriptRunner.Verify(x => x.Execute(It.IsAny<ScriptContext>()), Times.Once);
        }

        [Fact]
        public async Task Execute_WithInvalidCode_ReturnsError()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "invalid code syntax",
                Configuration = "Release"
            };

            _mockScriptRunner.Setup(x => x.Execute(It.IsAny<ScriptContext>()))
                           .Returns(Task.FromResult(1));

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Execute_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }
    }
}