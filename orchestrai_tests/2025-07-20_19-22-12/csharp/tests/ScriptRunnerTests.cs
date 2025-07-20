using Xunit;
using Moq;
using Dotnet.Script.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptRunnerTests
    {
        private readonly Mock<ILogger<ScriptRunner>> _loggerMock;
        private readonly ScriptRunner _scriptRunner;

        public ScriptRunnerTests()
        {
            _loggerMock = new Mock<ILogger<ScriptRunner>>();
            _scriptRunner = new ScriptRunner(_loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidCode_ReturnsResult()
        {
            // Arrange
            var code = "return 42;";

            // Act
            var result = await _scriptRunner.ExecuteAsync<int>(code);

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidCode_ThrowsException()
        {
            // Arrange
            var code = "invalid syntax";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _scriptRunner.ExecuteAsync<object>(code));
        }

        [Fact]
        public async Task ExecuteAsync_WithNullCode_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _scriptRunner.ExecuteAsync<object>(null));
        }

        [Fact]
        public async Task ExecuteFileAsync_WithValidFile_ReturnsResult()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "return \"Hello World\";");

            try
            {
                // Act
                var result = await _scriptRunner.ExecuteFileAsync<string>(tempFile);

                // Assert
                Assert.Equal("Hello World", result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public async