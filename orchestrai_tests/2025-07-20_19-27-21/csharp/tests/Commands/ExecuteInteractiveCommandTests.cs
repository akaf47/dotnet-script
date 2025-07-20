```csharp
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteInteractiveCommandTests
    {
        private readonly Mock<ILogger<ExecuteInteractiveCommand>> _mockLogger;
        private readonly Mock<IInteractiveRunner> _mockInteractiveRunner;
        private readonly ExecuteInteractiveCommand _command;

        public ExecuteInteractiveCommandTests()
        {
            _mockLogger = new Mock<ILogger<ExecuteInteractiveCommand>>();
            _mockInteractiveRunner = new Mock<IInteractiveRunner>();
            _command = new ExecuteInteractiveCommand(_mockInteractiveRunner.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidOptions_StartsInteractiveSession()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions
            {
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            _mockInteractiveRunner
                .Setup(x => x.RunInteractiveAsync(It.IsAny<string>(), It.IsAny<string[]>()))
                .ReturnsAsync(0);

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.Equal(0, result);
            _mockInteractiveRunner.Verify(x => x.RunInteractiveAsync(
                options.WorkingDirectory,
                It.IsAny<string[]>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithReferences_PassesReferencesToRunner()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions
            {
                WorkingDirectory = Directory.GetCurrentDirectory(),
                References = new[] { "System.dll", "System.Core.dll" }
            };

            _mockInteractiveRunner
                .Setup(x => x.RunInteractiveAsync(It.IsAny<string>(), It.IsAny<string[]>()))
                .ReturnsAsync(0);

            // Act
            await _command.ExecuteAsync(options);

            // Assert
            _mockInteractiveRunner.Verify(x => x.Run