using System;
using System.Diagnostics;
using Xunit;
using Dotnet.Script;

namespace Dotnet.Script.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void Main_ValidArgs_ReturnsZero()
        {
            string[] args = new string[] { "eval", "Console.WriteLine(\"Hello World\");" };
            int exitCode = Program.Main(args);
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void Main_InvalidArgs_ReturnsOne()
        {
            string[] args = new string[] { "unknownCommand" };
            int exitCode = Program.Main(args);
            Assert.Equal(1, exitCode);
        }

        [Fact]
        public void CreateLogFactory_ValidVerbosity_ReturnsLogFactory()
        {
            var logFactory = Program.CreateLogFactory("info");
            Assert.NotNull(logFactory);
        }

        [Fact]
        public void CreateLogFactory_InvalidVerbosity_ReturnsLogFactory()
        {
            var logFactory = Program.CreateLogFactory("invalid");
            Assert.NotNull(logFactory);
        }

        [Fact]
        public async void Wain_NavigateToInteractive_ReturnsSuccess()
        {
            // Arrange
            string[] args = new string[] { "--interactive" };

            // Act
            int exitCode = await Program.Wain(args);

            // Assert
            Assert.Equal(0, exitCode);
        }
    }
}