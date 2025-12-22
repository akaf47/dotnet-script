using System;
using System.Collections.Generic;
using Xunit;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteInteractiveCommandOptionsTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_Default_InitializesWithDefaultValues()
        {
            // Act
            var options = new ExecuteInteractiveCommandOptions();

            // Assert
            Assert.NotNull(options);
        }

        #endregion

        #region ScriptPath Property Tests

        [Fact]
        public void ScriptPath_CanBeSetAndRetrieved()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var scriptPath = "test.csx";

            // Act
            options.ScriptPath = scriptPath;
            var result = options.ScriptPath;

            // Assert
            Assert.Equal(scriptPath, result);
        }

        [Fact]
        public void ScriptPath_CanBeSetToNull()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();

            // Act
            options.ScriptPath = null;
            var result = options.ScriptPath;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ScriptPath_CanBeSetToEmptyString()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();

            // Act
            options.ScriptPath = string.Empty;
            var result = options.ScriptPath;

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ScriptPath_PreservesWhitespace()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var scriptPath = "  path/to/script.csx  ";

            // Act
            options.ScriptPath = scriptPath;
            var result = options.ScriptPath;

            // Assert
            Assert.Equal(scriptPath, result);
        }

        [Fact]
        public void ScriptPath_SupportsLongPaths()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var longPath = new string('a', 1000) + ".csx";

            // Act
            options.ScriptPath = longPath;
            var result = options.ScriptPath;

            // Assert
            Assert.Equal(longPath, result);
        }

        [Fact]
        public void ScriptPath_SupportsDifferentPathSeparators()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var mixedPath = "path\\to/script.csx";

            // Act
            options.ScriptPath = mixedPath;
            var result = options.ScriptPath;

            // Assert
            Assert.Equal(mixedPath, result);
        }

        #endregion

        #region Arguments Property Tests

        [Fact]
        public void Arguments_CanBeSetAndRetrieved()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var arguments = new List<string> { "arg1", "arg2" };

            // Act
            options.Arguments = arguments;
            var result = options.Arguments;

            // Assert
            Assert.Equal(arguments, result);
        }

        [Fact]
        public void Arguments_CanBeSetToNull()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();

            // Act
            options.Arguments = null;
            var result = options.Arguments;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Arguments_CanBeSetToEmptyList()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var emptyList = new List<string>();

            // Act
            options.Arguments = emptyList;
            var result = options.Arguments;

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Arguments_PreservesSingleArgument()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var arguments = new List<string> { "singleArg" };

            // Act
            options.Arguments = arguments;
            var result = options.Arguments;

            // Assert
            Assert.Single(result);
            Assert.Equal("singleArg", result[0]);
        }

        [Fact]
        public void Arguments_PreservesMultipleArguments()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var arguments = new List<string> { "arg1", "arg2", "arg3", "arg4" };

            // Act
            options.Arguments = arguments;
            var result = options.Arguments;

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal("arg1", result[0]);
            Assert.Equal("arg2", result[1]);
            Assert.Equal("arg3", result[2]);
            Assert.Equal("arg4", result[3]);
        }

        [Fact]
        public void Arguments_PreservesArgumentsWithWhitespace()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var arguments = new List<string> { "  arg1  ", " arg2  " };

            // Act
            options.Arguments = arguments;
            var result = options.Arguments;

            // Assert
            Assert.Equal("  arg1  ", result[0]);
            Assert.Equal(" arg2  ", result[1]);
        }

        [Fact]
        public void Arguments_PreservesArgumentsWithSpecialCharacters()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var arguments = new List<string> { "arg@#$%", "arg=value", "arg\"quote\"" };

            // Act
            options.Arguments = arguments;
            var result = options.Arguments;

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("arg@#$%", result[0]);
        }

        [Fact]
        public void Arguments_PreservesEmptyStringInList()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var arguments = new List<string> { "arg1", "", "arg3" };

            // Act
            options.Arguments = arguments;
            var result = options.Arguments;

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("", result[1]);
        }

        #endregion

        #region EnableDebug Property Tests

        [Fact]
        public void EnableDebug_DefaultValueIsFalse()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();

            // Act
            var result = options.EnableDebug;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EnableDebug_CanBeSetToTrue()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();

            // Act
            options.EnableDebug = true;
            var result = options.EnableDebug;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EnableDebug_CanBeSetToFalse()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();

            // Act
            options.EnableDebug = false;
            var result = options.EnableDebug;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EnableDebug_CanToggleBetweenTrueAndFalse()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();

            // Act
            options.EnableDebug = true;
            Assert.True(options.EnableDebug);
            options.EnableDebug = false;
            var result = options.EnableDebug;

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Timeout Property Tests

        [Fact]
        public void Timeout_CanBeSetAndRetrieved()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var timeout = TimeSpan.FromSeconds(30);

            // Act
            options.Timeout = timeout;
            var result = options.Timeout;

            // Assert
            Assert.Equal(timeout, result);
        }

        [Fact]
        public void Timeout_CanBeSetToZero()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();

            // Act
            options.Timeout = TimeSpan.Zero;
            var result = options.Timeout;

            // Assert
            Assert.Equal(TimeSpan.Zero, result);
        }

        [Fact]
        public void Timeout_CanBeSetToNegativeValue()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var timeout = TimeSpan.FromSeconds(-1);

            // Act
            options.Timeout = timeout;
            var result = options.Timeout;

            // Assert
            Assert.Equal(timeout, result);
        }

        [Fact]
        public void Timeout_CanBeSetToLargeValue()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var timeout = TimeSpan.FromDays(365);

            // Act
            options.Timeout = timeout;
            var result = options.Timeout;

            // Assert
            Assert.Equal(timeout, result);
        }

        #endregion

        #region Combined Property Tests

        [Fact]
        public void MultipleProperties_CanBeSetTogether()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var scriptPath = "test.csx";
            var arguments = new List<string> { "arg1", "arg2" };
            var enableDebug = true;
            var timeout = TimeSpan.FromMinutes(1);

            // Act
            options.ScriptPath = scriptPath;
            options.Arguments = arguments;
            options.EnableDebug = enableDebug;
            options.Timeout = timeout;

            // Assert
            Assert.Equal(scriptPath, options.ScriptPath);
            Assert.Equal(arguments, options.Arguments);
            Assert.True(options.EnableDebug);
            Assert.Equal(timeout, options.Timeout);
        }

        [Fact]
        public void Properties_AreIndependent()
        {
            // Arrange
            var options1 = new ExecuteInteractiveCommandOptions();
            var options2 = new ExecuteInteractiveCommandOptions();

            // Act
            options1.ScriptPath = "test1.csx";
            options2.ScriptPath = "test2.csx";

            // Assert
            Assert.NotEqual(options1.ScriptPath, options2.ScriptPath);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Arguments_WithNullElementInList_IsHandledCorrectly()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var arguments = new List<string> { "arg1", null, "arg3" };

            // Act
            options.Arguments = arguments;
            var result = options.Arguments;

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Null(result[1]);
        }

        [Fact]
        public void ScriptPath_WithUnicodeCharacters_IsPreserved()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var scriptPath = "test_日本語_スクリプト.csx";

            // Act
            options.ScriptPath = scriptPath;
            var result = options.ScriptPath;

            // Assert
            Assert.Equal(scriptPath, result);
        }

        [Fact]
        public void Arguments_WithUnicodeCharacters_IsPreserved()
        {
            // Arrange
            var options = new ExecuteInteractiveCommandOptions();
            var arguments = new List<string> { "arg_日本語", "参数_ñ" };

            // Act
            options.Arguments = arguments;
            var result = options.Arguments;

            // Assert
            Assert.Equal("arg_日本語", result[0]);
            Assert.Equal("参数_ñ", result[1]);
        }

        #endregion
    }
}