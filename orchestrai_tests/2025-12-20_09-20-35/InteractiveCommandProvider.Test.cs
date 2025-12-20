using NUnit.Framework;

namespace Dotnet.Script.Core.Tests
{
    [TestFixture]
    public class InteractiveCommandProviderTests
    {
        private InteractiveCommandProvider _provider;

        [SetUp]
        public void Setup()
        {
            _provider = new InteractiveCommandProvider();
        }

        [Test]
        public void TryProvideCommand_ValidCommand_ReturnsTrue()
        {
            // Arrange
            string[] validCommands = { "#reset", "#cls", "#exit" };

            foreach (var command in validCommands)
            {
                // Act
                var result = _provider.TryProvideCommand(command, out var providedCommand);

                // Assert
                Assert.IsTrue(result);
                Assert.IsNotNull(providedCommand);
                Assert.AreEqual(command.Substring(1), providedCommand.Name);
            }
        }

        [Test]
        public void TryProvideCommand_InvalidCommand_ReturnsFalse()
        {
            // Arrange
            string[] invalidCommands = { 
                "#unknown", 
                "not a command", 
                "", 
                null, 
                "#r System.Linq", 
                "#load script.csx" 
            };

            foreach (var command in invalidCommands)
            {
                // Act
                var result = _provider.TryProvideCommand(command, out var providedCommand);

                // Assert
                Assert.IsFalse(result);
                Assert.IsNull(providedCommand);
            }
        }

        [Test]
        public void TryProvideCommand_CompilerDirectives_ReturnsFalse()
        {
            // Arrange
            string[] compilerDirectives = { 
                "#r System.Linq", 
                "#load script.csx" 
            };

            foreach (var directive in compilerDirectives)
            {
                // Act
                var result = _provider.TryProvideCommand(directive, out var providedCommand);

                // Assert
                Assert.IsFalse(result);
                Assert.IsNull(providedCommand);
            }
        }
    }
}