using NUnit.Framework;
using Moq;
using System.IO;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Runtime;

namespace Dotnet.Script.Core.Commands.Tests
{
    [TestFixture]
    public class PublishCommandTests
    {
        private Mock<ScriptConsole> _mockScriptConsole;
        private Mock<LogFactory> _mockLogFactory;
        private PublishCommand _command;

        [SetUp]
        public void Setup()
        {
            _mockScriptConsole = new Mock<ScriptConsole>();
            _mockLogFactory = new Mock<LogFactory>();
            _command = new PublishCommand(_mockScriptConsole.Object, _mockLogFactory.Object);
        }

        [Test]
        public void Execute_PublishLibrary_CreatesAssembly()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try 
            {
                var options = new PublishCommandOptions
                {
                    File = new ScriptFile { Path = tempFile },
                    PublishType = PublishType.Library,
                    OutputDirectory = Path.GetDirectoryName(tempFile),
                    LibraryName = "TestLibrary",
                    OptimizationLevel = 0,
                    NoCache = false
                };

                // Act
                _command.Execute(options);

                // Assert
                // Verify that the assembly is created in the output directory
                var assemblyPath = Path.Combine(options.OutputDirectory, "TestLibrary.dll");
                Assert.IsTrue(File.Exists(assemblyPath));
            }
            finally 
            {
                File.Delete(tempFile);
            }
        }

        [Test]
        public void Execute_PublishExecutable_CreatesExecutable()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try 
            {
                var options = new PublishCommandOptions
                {
                    File = new ScriptFile { Path = tempFile },
                    PublishType = PublishType.Executable,
                    OutputDirectory = Path.GetDirectoryName(tempFile),
                    RuntimeIdentifier = "win-x64",
                    LibraryName = "TestApp",
                    OptimizationLevel = 0,
                    NoCache = false
                };

                // Act
                _command.Execute(options);

                // Assert
                // Verify that the executable is created in the output directory
                var executablePath = Path.Combine(options.OutputDirectory, "TestApp.exe");
                Assert.IsTrue(File.Exists(executablePath));
            }
            finally 
            {
                File.Delete(tempFile);
            }
        }

        [Test]
        public void Execute_NullOutputDirectory_UsesDefaultPublishLocation()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try 
            {
                var options = new PublishCommandOptions
                {
                    File = new ScriptFile { Path = tempFile },
                    PublishType = PublishType.Library,
                    OutputDirectory = null,
                    LibraryName = "TestLibrary",
                    OptimizationLevel = 0,
                    NoCache = false
                };

                // Act
                _command.Execute(options);

                // Assert
                var defaultPublishPath = Path.Combine(Path.GetDirectoryName(tempFile), "publish");
                var assemblyPath = Path.Combine(defaultPublishPath, "TestLibrary.dll");
                Assert.IsTrue(File.Exists(assemblyPath));
            }
            finally 
            {
                File.Delete(tempFile);
            }
        }
    }
}