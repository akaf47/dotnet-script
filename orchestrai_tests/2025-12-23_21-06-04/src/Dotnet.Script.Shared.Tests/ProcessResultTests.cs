using Xunit;

namespace Dotnet.Script.Shared.Tests
{
    public class ProcessResultTests
    {
        [Fact]
        public void Constructor_SetsAllPropertiesCorrectly()
        {
            // Arrange
            string output = "test output";
            int exitCode = 0;
            string standardOut = "stdout content";
            string standardError = "stderr content";

            // Act
            var result = new ProcessResult(output, exitCode, standardOut, standardError);

            // Assert
            Assert.Equal(output, result.Output);
            Assert.Equal(exitCode, result.ExitCode);
            Assert.Equal(standardOut, result.StandardOut);
            Assert.Equal(standardError, result.StandardError);
        }

        [Fact]
        public void Constructor_WithEmptyStrings()
        {
            // Arrange
            string output = "";
            int exitCode = 1;
            string standardOut = "";
            string standardError = "";

            // Act
            var result = new ProcessResult(output, exitCode, standardOut, standardError);

            // Assert
            Assert.Equal("", result.Output);
            Assert.Equal(1, result.ExitCode);
            Assert.Equal("", result.StandardOut);
            Assert.Equal("", result.StandardError);
        }

        [Fact]
        public void Constructor_WithNullStrings()
        {
            // Arrange
            string output = null;
            int exitCode = -1;
            string standardOut = null;
            string standardError = null;

            // Act
            var result = new ProcessResult(output, exitCode, standardOut, standardError);

            // Assert
            Assert.Null(result.Output);
            Assert.Equal(-1, result.ExitCode);
            Assert.Null(result.StandardOut);
            Assert.Null(result.StandardError);
        }

        [Fact]
        public void Constructor_WithPositiveExitCode()
        {
            // Arrange
            string output = "error message";
            int exitCode = 127;
            string standardOut = "";
            string standardError = "error details";

            // Act
            var result = new ProcessResult(output, exitCode, standardOut, standardError);

            // Assert
            Assert.Equal(127, result.ExitCode);
        }

        [Fact]
        public void Constructor_WithNegativeExitCode()
        {
            // Arrange
            string output = "output";
            int exitCode = -999;
            string standardOut = "stdout";
            string standardError = "stderr";

            // Act
            var result = new ProcessResult(output, exitCode, standardOut, standardError);

            // Assert
            Assert.Equal(-999, result.ExitCode);
        }

        [Fact]
        public void Constructor_WithMaxIntExitCode()
        {
            // Arrange
            string output = "output";
            int exitCode = int.MaxValue;
            string standardOut = "stdout";
            string standardError = "stderr";

            // Act
            var result = new ProcessResult(output, exitCode, standardOut, standardError);

            // Assert
            Assert.Equal(int.MaxValue, result.ExitCode);
        }

        [Fact]
        public void Constructor_WithMinIntExitCode()
        {
            // Arrange
            string output = "output";
            int exitCode = int.MinValue;
            string standardOut = "stdout";
            string standardError = "stderr";

            // Act
            var result = new ProcessResult(output, exitCode, standardOut, standardError);

            // Assert
            Assert.Equal(int.MinValue, result.ExitCode);
        }

        [Fact]
        public void Constructor_WithLongStrings()
        {
            // Arrange
            string output = new string('a', 10000);
            int exitCode = 0;
            string standardOut = new string('b', 10000);
            string standardError = new string('c', 10000);

            // Act
            var result = new ProcessResult(output, exitCode, standardOut, standardError);

            // Assert
            Assert.Equal(10000, result.Output.Length);
            Assert.Equal(10000, result.StandardOut.Length);
            Assert.Equal(10000, result.StandardError.Length);
        }

        [Fact]
        public void Output_Property_IsReadOnly()
        {
            // Arrange
            var result = new ProcessResult("output", 0, "stdout", "stderr");

            // Act & Assert
            Assert.Equal("output", result.Output);
            // Attempting to set would cause compilation error, so we just verify it's readable
        }

        [Fact]
        public void ExitCode_Property_IsReadOnly()
        {
            // Arrange
            var result = new ProcessResult("output", 42, "stdout", "stderr");

            // Act & Assert
            Assert.Equal(42, result.ExitCode);
        }

        [Fact]
        public void StandardOut_Property_IsReadOnly()
        {
            // Arrange
            var result = new ProcessResult("output", 0, "stdout content", "stderr");

            // Act & Assert
            Assert.Equal("stdout content", result.StandardOut);
        }

        [Fact]
        public void StandardError_Property_IsReadOnly()
        {
            // Arrange
            var result = new ProcessResult("output", 0, "stdout", "stderr content");

            // Act & Assert
            Assert.Equal("stderr content", result.StandardError);
        }

        [Fact]
        public void Deconstruct_WithTwoVariables_AssignsOutputAndExitCode()
        {
            // Arrange
            var result = new ProcessResult("test output", 5, "stdout", "stderr");

            // Act
            (string output, int exitCode) = result;

            // Assert
            Assert.Equal("test output", output);
            Assert.Equal(5, exitCode);
        }

        [Fact]
        public void Deconstruct_WithNullOutput_AssignsNullAndExitCode()
        {
            // Arrange
            var result = new ProcessResult(null, 0, "stdout", "stderr");

            // Act
            (string output, int exitCode) = result;

            // Assert
            Assert.Null(output);
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void Deconstruct_WithZeroExitCode()
        {
            // Arrange
            var result = new ProcessResult("output", 0, "stdout", "stderr");

            // Act
            (string output, int exitCode) = result;

            // Assert
            Assert.Equal("output", output);
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void Deconstruct_WithNegativeExitCode()
        {
            // Arrange
            var result = new ProcessResult("output", -15, "stdout", "stderr");

            // Act
            (string output, int exitCode) = result;

            // Assert
            Assert.Equal("output", output);
            Assert.Equal(-15, exitCode);
        }

        [Fact]
        public void Deconstruct_WithMaxIntExitCode()
        {
            // Arrange
            var result = new ProcessResult("output", int.MaxValue, "stdout", "stderr");

            // Act
            (string output, int exitCode) = result;

            // Assert
            Assert.Equal("output", output);
            Assert.Equal(int.MaxValue, exitCode);
        }

        [Fact]
        public void Deconstruct_WithEmptyOutput()
        {
            // Arrange
            var result = new ProcessResult("", 10, "stdout", "stderr");

            // Act
            (string output, int exitCode) = result;

            // Assert
            Assert.Equal("", output);
            Assert.Equal(10, exitCode);
        }

        [Fact]
        public void Deconstruct_IgnoresStandardOutAndError()
        {
            // Arrange
            var result = new ProcessResult("output", 0, "stdout", "stderr");

            // Act
            (string output, int exitCode) = result;

            // Assert - StandardOut and StandardError are not included in deconstruction
            Assert.Equal("output", output);
            Assert.Equal(0, exitCode);
            Assert.Equal("stdout", result.StandardOut); // Verify StandardOut is still accessible
            Assert.Equal("stderr", result.StandardError); // Verify StandardError is still accessible
        }

        [Fact]
        public void AllProperties_CanBeAccessedIndependently()
        {
            // Arrange
            var result = new ProcessResult("output", 1, "stdout", "stderr");

            // Act & Assert
            var output = result.Output;
            var exitCode = result.ExitCode;
            var standardOut = result.StandardOut;
            var standardError = result.StandardError;

            Assert.Equal("output", output);
            Assert.Equal(1, exitCode);
            Assert.Equal("stdout", standardOut);
            Assert.Equal("stderr", standardError);
        }

        [Fact]
        public void Constructor_WithMultilineStrings()
        {
            // Arrange
            string output = "line1\nline2\nline3";
            int exitCode = 0;
            string standardOut = "out1\nout2";
            string standardError = "err1\nerr2\nerr3\nerr4";

            // Act
            var result = new ProcessResult(output, exitCode, standardOut, standardError);

            // Assert
            Assert.Contains("\n", result.Output);
            Assert.Contains("\n", result.StandardOut);
            Assert.Contains("\n", result.StandardError);
        }

        [Fact]
        public void Constructor_WithSpecialCharacters()
        {
            // Arrange
            string output = "!@#$%^&*()_+-=[]{}|;:',.<>?/";
            int exitCode = 0;
            string standardOut = "special: \t\r\n";
            string standardError = "unicode: ñáéíóú";

            // Act
            var result = new ProcessResult(output, exitCode, standardOut, standardError);

            // Assert
            Assert.Equal("!@#$%^&*()_+-=[]{}|;:',.<>?/", result.Output);
            Assert.Equal("special: \t\r\n", result.StandardOut);
            Assert.Equal("unicode: ñáéíóú", result.StandardError);
        }
    }
}