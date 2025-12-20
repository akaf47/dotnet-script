using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis.Text;

namespace Dotnet.Script.Core.Commands.Tests
{
    [TestFixture]
    public class ExecuteCodeCommandTests
    {
        private Mock<ScriptConsole> _mockScriptConsole;
        private Mock<LogFactory> _mockLogFactory;
        private ExecuteCodeCommand _command;

        [SetUp]
        public void Setup()
        {
            _mockScriptConsole = new Mock<ScriptConsole>();
            _mockLogFactory = new Mock<LogFactory>();
            _command = new ExecuteCodeCommand(_mockScriptConsole.Object, _mockLogFactory.Object);
        }

        [Test]
        public async Task Execute_ValidCode_ReturnsExpectedResult()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "return 42;",
                WorkingDirectory = "/test/directory",
                Arguments = new string[] { "arg1" },
                OptimizationLevel = 0,
                NoCache = false
            };

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.AreEqual(42, result);
        }

        [Test]
        public async Task Execute_CodeWithPackageSources_ExecutesSuccessfully()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "return 42;",
                WorkingDirectory = "/test/directory",
                Arguments = new string[] { "arg1" },
                OptimizationLevel = 0,
                NoCache = false,
                PackageSources = new[] { "https://api.nuget.org/v3/index.json" }
            };

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.AreEqual(42, result);
        }

        [Test]
        public async Task Execute_NullWorkingDirectory_UsesCurrentDirectory()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions
            {
                Code = "return 42;",
                WorkingDirectory = null,
                Arguments = new string[] { "arg1" },
                OptimizationLevel = 0,
                NoCache = false
            };

            // Act
            var result = await _command.Execute<int>(options);

            // Assert
            Assert.AreEqual(42, result);
        }
    }
}