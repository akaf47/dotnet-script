using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Dotnet.Script.DependencyModel.Logging;
using System.Threading.Tasks;
using System.IO;

namespace Dotnet.Script.Core.Commands.Tests
{
    public class ExecuteScriptCommandTests
    {
        private readonly Mock<IScriptRunner> _mockScriptRunner;
        private readonly Mock<Logger> _mockLogger;
        private readonly ExecuteScriptCommand _command;

        public ExecuteScriptCommandTests()
        {
            _mockScriptRunner = new Mock<IScriptRunner>();
            _mockLogger = new Mock<Logger>();
            _command = new ExecuteScriptCommand(_mockScriptRunner.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Execute_WithValidScriptFile_ReturnsSuccess()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "Console.WriteLine(\"Hello World\");");

            var options = new ExecuteScriptCommandOptions
            {
                File = tempFile,
                Configuration = "Release"
            };

            _mockScriptRunner.Setup(x => x.Execute(It.IsAny<ScriptContext>()))
                           .Returns(Task.FromResult(0));

            try
            {
                // Act
                var result = await _command.Execute(options);

                // Assert
                Assert.Equal(0, result);
                _mockScriptRunner.Verify(x => x.Execute(It.IsAny<ScriptContext>()), Times.Once);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task Execute_WithNonExistentFile_ReturnsError()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions
            {
                File = "nonexistent.csx",
                Configuration = "Release"
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Execute_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }
    }
}