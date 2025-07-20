```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace DotNetScript.Tests
{
    public class ScriptCacheTests : IDisposable
    {
        private readonly Mock<ILogger<ScriptCache>> _mockLogger;
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly ScriptCache _cache;
        private readonly string _tempCacheDir;

        public ScriptCacheTests()
        {
            _mockLogger = new Mock<ILogger<ScriptCache>>();
            _mockFileSystem = new Mock<IFileSystem>();
            _tempCacheDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _cache = new ScriptCache(_mockLogger.Object, _mockFileSystem.Object, _tempCacheDir);
        }

        [Fact]
        public async Task GetCachedAssemblyAsync_WithExistingCache_ShouldReturnCachedAssembly()
        {
            // Arrange
            var scriptPath = "test.csx";
            var scriptHash = "hash123";
            var cachedAssemblyPath = Path.Combine(_tempCacheDir, $"{scriptHash}.dll");
            
            _mockFileSystem.Setup(x => x.FileExists(cachedAssemblyPath))
                          .Returns(true);
            _mockFileSystem.Setup(x => x.GetFileHash(scriptPath))
                          .Returns(scriptHash);

            // Act
            var result = await _cache.GetCachedAssemblyAsync(scriptPath);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(cachedAssemblyPath);
        }

        [Fact]
        public async Task GetCachedAssemblyAsync_WithNonExistingCache_ShouldReturnNull()
        {
            // Arrange
            var scriptPath = "test.csx";
            var scriptHash = "hash123";
            var cachedAssemblyPath = Path.Combine(_tempCacheDir, $"{scriptHash}.dll");
            
            _mockFileSystem.Setup(x => x.FileExists(cachedAssemblyPath))
                          .Returns(false);
            _mockFileSystem.Setup(x => x.GetFileHash(scriptPath))
                          .Returns(scriptHash);

            // Act
            var result = await _cache.GetCachedAssemblyAsync(scriptPath);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CacheAssemblyAsync_WithValidAssembly_ShouldSaveToCache()
        {
            // Arrange
            var scriptPath = "test.csx";
            var scriptHash = "hash123";
            var assemblyBytes = new byte[] { 1, 2, 3, 4 };
            var expectedCachePath = Path.Combine(_tempCacheDir, $"{scriptHash}.dll");
            
            _mockFileSystem.Setup(x => x.GetFileHash(scriptPath))
                          .Returns(scriptHash);
            _mockFileSystem.Setup(x => x.WriteAllBytesAsync(expectedCachePath, assemblyBytes))
                          .Returns(Task.CompletedTask);

            // Act
            await _cache.CacheAssemblyAsync(scriptPath, assemblyBytes);

            // Assert
            _mockFileSystem.Verify(x => x.WriteAllBytesAsync(expectedCachePath, assemblyBytes), Times.Once);
        }

        [Fact]
        public async Task CacheAssemblyAsync_WithNullAssemblyBytes_ShouldThrowArgumentNullException()
        {
            // Arrange
            var scriptPath = "test.csx";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _cache.CacheAssemblyAsync(scriptPath, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task GetCachedAssemblyAsync_WithInvalidScriptPath_ShouldThrowArgumentException(string scriptPath)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _cache.GetCachedAssemblyAsync(scriptPath));
        }

        [Fact]
        public void ClearCache_ShouldDeleteAllCachedFiles()
        {
            // Arrange
            var cachedFiles = new[] { "file1.dll", "file2.dll" };
            _mockFileSystem.Setup(x => x.GetFiles(_tempCacheDir, "*.dll"))
                          .Returns(cachedFiles);

            // Act
            _cache.ClearCache();

            // Assert
            foreach (var file in cachedFiles)
            {
                _mockFileSystem.Verify(x => x.DeleteFile(file), Times.Once);
            }
        }

        [Fact]
        public void GetCacheSize_ShouldReturnTotalSizeOfCachedFiles()
        {
            // Arrange
            var cachedFiles = new[] { "file1.dll", "file2.dll" };
            _mockFileSystem.Setup(x => x.GetFiles(_tempCacheDir, "*.dll"))
                          .Returns(cachedFiles);
            _mockFileSystem.Setup(x => x.GetFileSize("file1.dll"))
                          .Returns(1024);
            _mockFileSystem.Setup(x => x.GetFileSize("file2.dll"))
                          .Returns(2048);

            // Act
            var size = _cache.GetCacheSize();

            // Assert
            size.Should().Be(3072);
        }

        [Fact]
        public void IsCacheEnabled_ShouldReturnTrue()
        {
            // Act
            var isEnabled = _cache.IsCacheEnabled;

            // Assert
            isEnabled.Should().BeTrue();
        }

        public void Dispose()
        {
            // Cleanup temp directory if it exists
            if (Directory.Exists(_tempCacheDir))
            {
                Directory.Delete(_tempCacheDir, true);
            }
        }
    }
}
```