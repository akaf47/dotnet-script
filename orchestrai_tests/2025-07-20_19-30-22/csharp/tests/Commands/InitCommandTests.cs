using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using System.Threading.Tasks;
using System.IO;

namespace Dotnet.Script.Core.Commands.Tests
{
    public class InitCommandTests
    {
        private readonly Mock<IScaffolder> _mockScaffolder;
        private readonly InitCommand _command;

        public InitCommandTests()
        {
            _mockScaffolder = new Mock<IScaffolder>();
            _command = new InitCommand(_mockScaffolder.Object);
        }

        [Fact]
        public async Task Execute_WithValidOptions_CreatesScriptFile()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var options = new InitCommandOptions
            {
                FileName = "test.csx",
                WorkingDirectory = tempDir
            };

            _mockScaffolder.Setup(x => x.InitializeScript(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
            _mockScaffolder.Verify(x => x.InitializeScript(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Execute_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }
    }
}