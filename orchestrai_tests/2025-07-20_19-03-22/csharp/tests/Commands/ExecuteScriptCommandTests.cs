using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteScriptCommandTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IScriptConsole> _mockConsole;
        private readonly ExecuteScriptCommand _command;
        private readonly string _tempDirectory;

        public ExecuteScriptCommandTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockConsole = new Mock<IScriptConsole>();
            _command = new ExecuteScriptCommand(_mockLogger.Object, _mockConsole.Object);
            _tempDirectory = Path.GetTempPath();
        }

        [Fact]
        public async Task Execute_WithValidScriptFile_ShouldReturnSuccess()
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "test.csx");
            File.WriteAllText(scriptPath, "Console.WriteLine(\"Hello from script\");");
            
            var options = new ExecuteScriptCommandOptions
            {
                File = scriptPath,
                Configuration = "Release"
            };

            try
            {
                // Act
                var result = await _command.Execute(options);

                // Assert
                Assert.Equal(0, result);
            }
            finally
            {
                if (File.Exists(scriptPath))
                    File.Delete(scriptPath);
            }
        }

        [Fact]
        public async Task Execute_WithNonExistentFile_ShouldReturnError()
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
        public async Task Execute_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _command.Execute(null));
        }

        [Fact]
        public async Task Execute_WithArguments_ShouldPassArgumentsToScript()
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, "test_args.csx");
            File.WriteAllText(scriptPath, "Console.WriteLine(Args[0]);");
            
            var options = new ExecuteScriptCommandOptions
            {
                File = scriptPath,
                Configuration = "Release",
                Arguments = new[] { "test-arg" }
            };

            try
            {
                // Act
                var result = await _command.Execute(options);

                // Assert
                Assert.Equal(0, result);
            }
            finally
            {
                if (File.Exists(scriptPath))
                    File.Delete(scriptPath);
            }
        }

        [Theory]
        [InlineData("Debug")]
        [InlineData("Release")]
        public async Task Execute_WithDifferentConfigurations_ShouldWork(string configuration)
        {
            // Arrange
            var scriptPath = Path.Combine(_tempDirectory, $"test_{configuration}.csx");
            File.WriteAllText(scriptPath, "var x = 1;");
            
            var options = new ExecuteScriptCommandOptions
            {
                File = scriptPath,
                Configuration = configuration
            };

            try
            {
                // Act
                var result = await _command.Execute(options);

                // Assert
                Assert.Equal(0, result);
            }
            finally
            {
                if (File.Exists(scriptPath))
                    File.Delete(scriptPath);
            }
        }
    }
}