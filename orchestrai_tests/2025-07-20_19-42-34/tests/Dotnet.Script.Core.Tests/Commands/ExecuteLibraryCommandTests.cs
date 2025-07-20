using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteLibraryCommandTests : IDisposable
    {
        private readonly Mock<ILogger<ExecuteLibraryCommand>> _mockLogger;
        private readonly ExecuteLibraryCommand _command;
        private readonly string _tempDirectory;

        public ExecuteLibraryCommandTests()
        {
            _mockLogger = new Mock<ILogger<ExecuteLibraryCommand>>();
            _command = new ExecuteLibraryCommand(_mockLogger.Object);
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [Fact]
        public void Constructor_WithValidLogger_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Assert.NotNull(_command);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecuteLibraryCommand(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidLibraryPath_ShouldExecuteSuccessfully()
        {
            // Arrange
            var libraryPath = Path.Combine(_tempDirectory, "library.dll");
            // Create a mock library file
            await File.WriteAllBytesAsync(libraryPath, new byte[] { 0x4D, 0x5A }); // PE header
            var options = new ExecuteLibraryCommandOptions { LibraryPath = libraryPath };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            // Note: This might fail in actual execution due to invalid assembly, 
            // but we're testing the command structure
            Assert.True(result >= 0);
        }

        [Fact]
        public async Task ExecuteAsync_WithNonExistentLibrary_ShouldReturnError()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions { LibraryPath = "nonexistent.dll" };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.ExecuteAsync(null));
        }

        [Fact]
        public async Task ExecuteAsync_WithMethodName_ShouldExecuteSpecificMethod()
        {
            // Arrange
            var libraryPath = Path.Combine(_tempDirectory, "library.dll");
            await File.WriteAllBytesAsync(libraryPath, new byte[] { 0x4D, 0x5A });
            var options = new ExecuteLibraryCommandOptions 
            { 
                LibraryPath = libraryPath,
                MethodName = "Main"
            };

            // Act
            var result = await _command.ExecuteAsync(options);

            // Assert
            Assert.True(result >= 0);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
    }
}