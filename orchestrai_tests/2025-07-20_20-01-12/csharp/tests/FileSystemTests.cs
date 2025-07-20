```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using DotNetScript.Infrastructure;

namespace DotNetScript.Tests
{
    public class FileSystemTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly FileSystem _fileSystem;

        public FileSystemTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _fileSystem = new FileSystem();
        }

        [Fact]
        public async Task ReadAllTextAsync_ExistingFile_ReturnsContent()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "test.txt");
            var expectedContent = "Hello World";
            await File.WriteAllTextAsync(filePath, expectedContent);

            // Act
            var content = await _fileSystem.ReadAllTextAsync(filePath);

            // Assert
            Assert.Equal(expectedContent, content);
        }

        [Fact]
        public async Task ReadAllTextAsync_NonExistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "nonexistent.txt");

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _fileSystem.ReadAllTextAsync(filePath));
        }

        [Fact]
        public async Task WriteAllTextAsync_ValidPath_WritesContent()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "output.txt");
            var content = "Test content";

            // Act
            await _fileSystem.WriteAllTextAsync(filePath, content);

            // Assert
            var writtenContent = await File.ReadAllTextAsync(filePath);
            Assert.Equal(content, writtenContent);
        }

        [Fact]
        public void Exists_ExistingFile_ReturnsTrue()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "existing.txt");
            File.WriteAllText(filePath, "content");

            // Act
            var exists = _fileSystem.Exists(filePath);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public void Exists_NonExistentFile_ReturnsFalse()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "nonexistent.txt");

            // Act
            var exists = _fileSystem.Exists(filePath);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public void GetFullPath_RelativePath_ReturnsAbsolutePath()
        {
            // Arrange
            var relativePath = "test.txt";

            // Act
            var fullPath = _fileSystem.GetFullPath(relativePath);

            // Assert
            Assert.True(Path.IsPathFullyQualified(fullPath));
            Assert.EndsWith("test.txt", fullPath);
        }

        [Fact]
        public void GetDirectoryName_FilePath_ReturnsDirectory()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "test.txt");

            // Act
            var directory = _fileSystem.GetDirectoryName(filePath);

            // Assert
            Assert.Equal(_testDirectory, directory);
        }

        [Fact]
        public void GetFileName_FilePath_ReturnsFileName()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "test.txt");

            // Act
            var fileName = _fileSystem.GetFileName(filePath);

            // Assert
            Assert.Equal("test.txt", fileName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task ReadAllTextAsync_EmptyOrNullPath_ThrowsArgumentException(string path)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _fileSystem.ReadAllTextAsync(path));
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
```