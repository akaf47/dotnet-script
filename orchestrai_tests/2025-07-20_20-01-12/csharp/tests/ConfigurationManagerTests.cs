```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using DotNetScript.Configuration;
using DotNetScript.Models;

namespace DotNetScript.Tests
{
    public class ConfigurationManagerTests : IDisposable
    {
        private readonly Mock<ILogger<ConfigurationManager>> _mockLogger;
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly ConfigurationManager _configManager;
        private readonly string _testConfigPath;

        public ConfigurationManagerTests()
        {
            _mockLogger = new Mock<ILogger<ConfigurationManager>>();
            _mockFileSystem = new Mock<IFileSystem>();
            _configManager = new ConfigurationManager(_mockLogger.Object, _mockFileSystem.Object);
            _testConfigPath = Path.Combine(Path.GetTempPath(), "test-config.json");
        }

        [Fact]
        public async Task LoadConfigurationAsync_ValidConfigFile_ReturnsConfiguration()
        {
            // Arrange
            var configJson = @"{
                ""DefaultReferences"": [""System.Text.Json""],
                ""DefaultUsings"": [""System.Text.Json""],
                ""CacheEnabled"": true,
                ""Verbose"": false
            }";

            _mockFileSystem.Setup(x => x.Exists(_testConfigPath)).Returns(true);
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(_testConfigPath)).ReturnsAsync(configJson);

            // Act
            var config = await _configManager.LoadConfigurationAsync(_testConfigPath);

            // Assert
            Assert.NotNull(config);
            Assert.Contains("System.Text.Json", config.DefaultReferences);
            Assert.Contains("System.Text.Json", config.DefaultUsings);
            Assert.True(config.CacheEnabled);
            Assert.False(config.Verbose);
        }

        [Fact]
        public async Task LoadConfigurationAsync_NonExistentFile_ReturnsDefaultConfiguration()
        {
            // Arrange
            _mockFileSystem.Setup(x => x.Exists(_testConfigPath)).Returns(false);

            // Act
            var config = await _configManager.LoadConfigurationAsync(_testConfigPath);

            // Assert
            Assert.NotNull(config);
            Assert.True(config.CacheEnabled);
            Assert.False(config.Verbose);
        }

        [Fact]
        public async Task LoadConfigurationAsync_InvalidJson_ThrowsInvalidOperationException()
        {
            // Arrange
            var invalidJson = "{ invalid json }";
            _mockFileSystem.Setup(x => x.Exists(_testConfigPath)).Returns(true);
            _mockFileSystem.Setup(x => x.ReadAllTextAsync(_testConfigPath)).ReturnsAsync(invalidJson);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _configManager.LoadConfigurationAsync(_testConfigPath));
        }

        [Fact]
        public async Task SaveConfigurationAsync_ValidConfiguration_SavesCorrectly()
        {
            // Arrange
            var config = new ScriptConfiguration
            {
                DefaultReferences = new[] { "System.Text.Json" },
                DefaultUsings = new[] { "System.Text.Json" },
                CacheEnabled = false,
                Verbose = true
            };

            string savedContent = null;
            _mockFileSystem.Setup(x => x.WriteAllTextAsync(_testConfigPath, It.IsAny<string>()))
                          .Callback<string, string>((path, content) => savedContent = content)
                          .Returns(Task.CompletedTask);

            // Act
            await _configManager.SaveConfigurationAsync(_testConfigPath, config);

            // Assert
            Assert.NotNull(savedContent);
            Assert.Contains("System.Text.Json", savedContent);
            Assert.Contains("\"CacheEnabled\": false", savedContent);
            Assert.Contains("\"Verbose\": true", savedContent);
        }

        [Fact]
        public async Task SaveConfigurationAsync_NullConfiguration_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _configManager.SaveConfigurationAsync(_testConfigPath, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task LoadConfigurationAsync_EmptyOrNullPath_ThrowsArgumentException(string path)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _configManager.LoadConfigurationAsync(path));
        }

        [Fact]
        public void GetDefaultConfiguration_ReturnsValidDefaults()
        {
            // Act
            var config = _configManager.GetDefaultConfiguration();

            // Assert
            Assert.NotNull(config);
            Assert.True(config.CacheEnabled);
            Assert.False(config.Verbose);
            Assert.NotEmpty(config.DefaultReferences);
            Assert.NotEmpty(config.DefaultUsings);
        }

        [Fact]
        public async Task MergeWithCommandLineOptions_ValidOptions_Merges