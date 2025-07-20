using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Dotnet.Script.Core;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptRunnerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IScriptConsole> _mockConsole;
        private readonly ScriptRunner _scriptRunner;

        public ScriptRunnerTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockConsole = new Mock<IScriptConsole>();
            _scriptRunner = new ScriptRunner(_mockLogger.Object, _mockConsole.Object);
        }

        [Fact]
        public async Task Execute_WithSimpleScript_ShouldReturnResult()
        {
            // Arrange
            var code = "return 42;";
            var context = new ScriptContext(code, Path.GetTempPath());

            // Act
            var result = await _scriptRunner.Execute<int>(context);

            // Assert
            Assert.Equal(42, result.ReturnValue);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_WithInvalidScript_ShouldReturnError()
        {
            // Arrange
            var code = "invalid syntax here";
            var context = new ScriptContext(code, Path.GetTempPath());

            // Act
            var result = await _scriptRunner.Execute<object>(context);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Exception);
        }

        [Fact]
        public async Task Execute_WithNullContext_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _scriptRunner.Execute<object>(null));
        }

        [Fact]
        public async Task Execute_WithUsingStatements_ShouldWork()
        {
            // Arrange
            var code = @"
                using System;
                return DateTime.Now.Year;
            ";
            var context = new ScriptContext(code, Path.GetTempPath());

            // Act
            var result = await _scriptRunner.Execute<int>(context);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.ReturnValue >= 2020);
        }

        [Fact]
        public async Task Execute_WithNuGetPackages_ShouldLoadPackages()
        {
            // Arrange
            var code = @"
                #r ""nuget: Newtonsoft.Json, 13.0.1""
                using Newtonsoft.Json;
                var obj = new { Name = ""Test"" };
                return JsonConvert.SerializeObject(obj);
            ";
            var context = new ScriptContext(code, Path.GetTempPath());

            // Act
            var result = await _scriptRunner.Execute<string>(context);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("Test", result.ReturnValue);
        }

        [Theory]
        [InlineData("Debug")]
        [InlineData("Release")]
        public async Task Execute_WithDiffer