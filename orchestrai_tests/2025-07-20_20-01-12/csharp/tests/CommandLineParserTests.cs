```csharp
using System;
using Xunit;
using DotNetScript.CLI;
using DotNetScript.Models;

namespace DotNetScript.Tests
{
    public class CommandLineParserTests
    {
        private readonly CommandLineParser _parser;

        public CommandLineParserTests()
        {
            _parser = new CommandLineParser();
        }

        [Fact]
        public void Parse_ValidScriptPath_ReturnsCorrectOptions()
        {
            // Arrange
            var args = new[] { "script.csx" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.Equal("script.csx", options.ScriptPath);
            Assert.Empty(options.ScriptArgs);
            Assert.False(options.ShowHelp);
            Assert.False(options.ShowVersion);
        }

        [Fact]
        public void Parse_ScriptWithArguments_ReturnsCorrectOptions()
        {
            // Arrange
            var args = new[] { "script.csx", "--", "arg1", "arg2" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.Equal("script.csx", options.ScriptPath);
            Assert.Equal(new[] { "arg1", "arg2" }, options.ScriptArgs);
        }

        [Fact]
        public void Parse_HelpFlag_ReturnsHelpOption()
        {
            // Arrange
            var args = new[] { "--help" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.True(options.ShowHelp);
        }

        [Fact]
        public void Parse_VersionFlag_ReturnsVersionOption()
        {
            // Arrange
            var args = new[] { "--version" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.True(options.ShowVersion);
        }

        [Fact]
        public void Parse_VerboseFlag_ReturnsVerboseOption()
        {
            // Arrange
            var args = new[] { "script.csx", "--verbose" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.True(options.Verbose);
        }

        [Fact]
        public void Parse_NoCache_ReturnsNoCacheOption()
        {
            // Arrange
            var args = new[] { "script.csx", "--no-cache" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.True(options.NoCache);
        }

        [Fact]
        public void Parse_EmptyArgs_ThrowsArgumentException()
        {
            // Arrange
            var args = new string[0];

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _parser.Parse(args));
        }

        [Theory]
        [InlineData(new[] { "-h" })]
        [InlineData(new[] { "/?" })]
        public void Parse_HelpShortcuts_ReturnsHelpOption(string[] args)
        {
            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.True(options.ShowHelp);
        }

        [Fact]
        public void Parse_ConfigFile_ReturnsConfigOption()
        {
            // Arrange
            var args = new[] { "script.csx", "--config", "config.json" };

            // Act
            var options = _parser.Parse(args);

            // Assert
            Assert.Equal("config.json", options.ConfigFile);
        }
    }
}
```