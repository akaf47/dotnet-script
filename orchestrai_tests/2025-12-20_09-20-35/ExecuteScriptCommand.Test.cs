using NUnit.Framework;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Environment;

namespace Dotnet.Script.Core.Commands.Tests
{
    [TestFixture]
    public class ExecuteScriptCommandTests
    {
        private Mock<ScriptConsole> _mockScriptConsole;
        private Mock<LogFactory> _mockLogFactory;
        private Mock<Logger> _mockLogger;
        private ExecuteScriptCommand _command;

        [SetUp]
        public void Setup()
        {
            _mockScriptConsole = new Mock<ScriptConsole>();
            _mockLogFactory = new Mock<LogFactory>();
            _mockLogger = new Mock<Logger>();
            _mockLogFactory.Setup(x => x.CreateLogger<ExecuteScriptCommand>()).Returns(_mockLogger.Object);
            _command = new ExecuteScriptCommand(_mockScriptConsole.Object, _mockLogFactory.Object);
        }

        [Test]
        public async Task Run_RemoteFile_DownloadsAndRunsCode()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions
            {
                File = new ScriptFile { IsRemote = true, Path = "http://example.com/script.csx" },
                Arguments = new string[] { "arg1" }
            };

            // Act
            var result = await _command.Run<string, object>(options);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Run_LocalFile_ExecutesLibrary()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions
            {
                File = new ScriptFile { IsRemote = false, Path = "/path/to/local/script.csx" },
                Arguments = new string[] { "arg1" }
            };

            // Act
            var result = await _command.Run<string, object>(options);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void TryCreateHash_NoCacheOption_ReturnsFalse()
        {
            // Arrange
            var options = new ExecuteScriptCommandOptions
            {
                NoCache = true,
                File = new ScriptFile { Path = "/path/to/script.csx" }
            };

            // Act
            var result = _command.TryCreateHash(options, out string hash);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(hash);
        }

        [Test]
        public void TryGetHash_NonExistentCacheFolder_ReturnsFalse()
        {
            // Arrange
            var nonExistentFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            // Act
            var result = _command.TryGetHash(nonExistentFolder, out string hash);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(hash);
        }

        [Test]
        public void TryGetHash_ExistingHashFile_ReturnsTrue()
        {
            // Arrange
            var cacheFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(cacheFolder);
            var hashFilePath = Path.Combine(cacheFolder, "script.sha256");
            File.WriteAllText(hashFilePath, "test-hash");

            try 
            {
                // Act
                var result = _command.TryGetHash(cacheFolder, out string hash);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual("test-hash", hash);
            }
            finally 
            {
                Directory.Delete(cacheFolder, true);
            }
        }
    }
}