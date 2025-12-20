using NUnit.Framework;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis.Scripting;

namespace Dotnet.Script.Core.Tests
{
    [TestFixture]
    public class InteractiveRunnerTests
    {
        private Mock<ScriptCompiler> _mockScriptCompiler;
        private Mock<LogFactory> _mockLogFactory;
        private Mock<ScriptConsole> _mockScriptConsole;
        private Mock<TextWriter> _mockTextWriter;
        private InteractiveRunner _runner;

        [SetUp]
        public void Setup()
        {
            _mockScriptCompiler = new Mock<ScriptCompiler>();
            _mockLogFactory = new Mock<LogFactory>();
            _mockScriptConsole = new Mock<ScriptConsole>();
            _mockTextWriter = new Mock<TextWriter>();

            _mockScriptConsole.Setup(x => x.Out).Returns(_mockTextWriter.Object);
            _mockScriptConsole.Setup(x => x.ReadLine()).Returns("test input");

            _runner = new InteractiveRunner(
                _mockScriptCompiler.Object, 
                _mockLogFactory.Object, 
                _mockScriptConsole.Object, 
                new string[] { "https://api.nuget.org/v3/index.json" }
            );
        }

        [Test]
        public async Task Execute_FirstScript_RunsSuccessfully()
        {
            // Arrange
            var context = new ScriptContext(
                Microsoft.CodeAnalysis.Text.SourceText.From("Console.WriteLine('Hello');"), 
                Directory.GetCurrentDirectory(), 
                new string[] { }, 
                ScriptMode.REPL
            );

            // Act
            var result = await _runner.Execute("Console.WriteLine('Hello');");

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Reset_ClearsScriptState()
        {
            // Act
            _runner.Reset();

            // Assert - use reflection to verify private fields are null
            var stateField = typeof(InteractiveRunner).GetField("_scriptState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var optionsField = typeof(InteractiveRunner).GetField("_scriptOptions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Assert.IsNull(stateField.GetValue(_runner));
            Assert.IsNull(optionsField.GetValue(_runner));
        }

        [Test]
        public void Exit_SetsExitFlag()
        {
            // Act
            _runner.Exit();

            // Assert - use reflection to verify private field
            var exitField = typeof(InteractiveRunner).GetField("_shouldExit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsTrue((bool)exitField.GetValue(_runner));
        }

        [Test]
        public async Task RunLoopWithSeed_ExecutesInitialScriptThenEntersLoop()
        {
            // Arrange
            var context = new ScriptContext(
                Microsoft.CodeAnalysis.Text.SourceText.From("Console.WriteLine('Seed');"), 
                Directory.GetCurrentDirectory(), 
                new string[] { }, 
                ScriptMode.REPL
            );

            // Act & Assert
            await _runner.RunLoopWithSeed(context);
        }
    }
}