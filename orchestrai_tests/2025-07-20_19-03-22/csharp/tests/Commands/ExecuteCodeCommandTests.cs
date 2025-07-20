using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core.Commands;
using Dotnet.Script.DependencyModel.Logging;
using Microsoft.CodeAnalysis.Scripting;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteCodeCommandTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IScriptConsole> _mockConsole;
        private readonly ExecuteCodeCommand _command;

        public ExecuteCodeCommandTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockConsole = new Mock<IScriptConsole>();
            _command = new ExecuteCodeCommand(_mockLogger.Object, _mockConsole.Object);
        }

        [Fact]
        public async Task Execute_WithValidCode_ShouldReturnSuccess()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "Console.WriteLine(\"Hello World\");",
                Configuration = "Release"
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_WithInvalidCode_ShouldReturnError()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "invalid code syntax",
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

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Execute_WithEmptyCode_ShouldReturnError(string code)
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = code,
                Configuration = "Release"
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Execute_WithDebugConfiguration_ShouldUseDebugMode()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "var x = 1;",
                Configuration = "Debug"
            };

            // Act
            var result = await _command.Execute(options);

            // Assert
            Assert.Equal(0, result);
        }
    }
}