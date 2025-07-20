```csharp
using System;
using System.Linq;
using Xunit;
using FluentAssertions;

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
        public void Parse_WithScriptPath_ShouldSetScriptPath()
        {
            // Arrange
            var args = new[] { "script.csx" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.ScriptPath.Should().Be("script.csx");
            result.ScriptArguments.Should().BeEmpty();
        }

        [Fact]
        public void Parse_WithScriptPathAndArguments_ShouldSetBoth()
        {
            // Arrange
            var args = new[] { "script.csx", "--", "arg1", "arg2" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.ScriptPath.Should().Be("script.csx");
            result.ScriptArguments.Should().HaveCount(2);
            result.ScriptArguments.Should().Contain("arg1");
            result.ScriptArguments.Should().Contain("arg2");
        }

        [Fact]
        public void Parse_WithVerboseFlag_ShouldSetVerbose()
        {
            // Arrange
            var args = new[] { "--verbose", "script.csx" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.Verbose.Should().BeTrue();
            result.ScriptPath.Should().Be("script.csx");
        }

        [Fact]
        public void Parse_WithHelpFlag_ShouldSetShowHelp()
        {
            // Arrange
            var args = new[] { "--help" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.ShowHelp.Should().BeTrue();
        }

        [Fact]
        public void Parse_WithVersionFlag_ShouldSetShowVersion()
        {
            // Arrange
            var args = new[] { "--version" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.ShowVersion.Should().BeTrue();
        }

        [Fact]
        public void Parse_WithNoCache_ShouldSetNoCache()
        {
            // Arrange
            var args = new[] { "--no-cache", "script.csx" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.NoCache.Should().BeTrue();
            result.ScriptPath.Should().Be("script.csx");
        }

        [Fact]
        public void Parse_WithEmptyArgs_ShouldReturnDefaultOptions()
        {
            // Arrange
            var args = new string[0];

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.ScriptPath.Should().BeNull();
            result.ShowHelp.Should().BeFalse();
            result.Verbose.Should().BeFalse();
        }

        [Theory]
        [InlineData("-v")]
        [InlineData("--verbose")]
        public void Parse_WithVerboseShortAndLongForm_ShouldSetVerbose(string verboseFlag)
        {
            // Arrange
            var args = new[] { verboseFlag, "script.csx" };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.Verbose.Should().BeTrue();
        }

        [Theory]
        [InlineData("-h")]
        [InlineData("--help")]
        public void Parse_WithHelpShortAndLongForm_ShouldSetShowHelp(string helpFlag)
        {
            // Arrange
            var args = new[] { helpFlag };

            // Act
            var result = _parser.Parse(args);

            // Assert
            result.Should().NotBeNull();
            result.ShowHelp.Should().BeTrue();
        }

        [Fact]
        public void Parse_WithInvalidFlag_ShouldThrowArgumentException()
        {
            // Arrange
            var args = new[] { "--invalid-flag", "script.csx" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _parser.Parse(args));
        }
    }
}
```