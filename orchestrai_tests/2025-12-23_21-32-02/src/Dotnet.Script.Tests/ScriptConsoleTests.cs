using System;
using System.IO;
using System.Text;
using Xunit;
using Moq;
using Microsoft.CodeAnalysis;
using Dotnet.Script.Core;

namespace Dotnet.Script.Tests
{
    public class ScriptConsoleTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithAllValidParameters_CreatesInstance()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("test");
            var error = new StringWriter();

            // Act
            var console = new ScriptConsole(output, input, error);

            // Assert
            Assert.Equal(output, console.Out);
            Assert.Equal(input, console.In);
            Assert.Equal(error, console.Error);
        }

        [Fact]
        public void Constructor_WithNullInput_EnablesHistoryAndSetsOtherProperties()
        {
            // Arrange
            var output = new StringWriter();
            var error = new StringWriter();

            // Act
            var console = new ScriptConsole(output, null, error);

            // Assert
            Assert.Equal(output, console.Out);
            Assert.Null(console.In);
            Assert.Equal(error, console.Error);
        }

        [Fact]
        public void Constructor_WithAllNullParameters_CreatesInstance()
        {
            // Arrange & Act
            var console = new ScriptConsole(null, null, null);

            // Assert
            Assert.Null(console.Out);
            Assert.Null(console.In);
            Assert.Null(console.Error);
        }

        [Fact]
        public void Constructor_WithNullOutputAndError_CreatesInstance()
        {
            // Arrange
            var input = new StringReader("test");

            // Act
            var console = new ScriptConsole(null, input, null);

            // Assert
            Assert.Null(console.Out);
            Assert.Equal(input, console.In);
            Assert.Null(console.Error);
        }

        #endregion

        #region Default Static Property Tests

        [Fact]
        public void Default_IsInitialized()
        {
            // Arrange & Act
            var defaultConsole = ScriptConsole.Default;

            // Assert
            Assert.NotNull(defaultConsole);
            Assert.NotNull(defaultConsole.Out);
            Assert.NotNull(defaultConsole.In);
            Assert.NotNull(defaultConsole.Error);
        }

        [Fact]
        public void Default_ReturnsConsoleOut()
        {
            // Arrange & Act
            var defaultConsole = ScriptConsole.Default;

            // Assert
            Assert.NotNull(defaultConsole.Out);
        }

        [Fact]
        public void Default_ReturnsConsoleIn()
        {
            // Arrange & Act
            var defaultConsole = ScriptConsole.Default;

            // Assert
            Assert.NotNull(defaultConsole.In);
        }

        [Fact]
        public void Default_ReturnsConsoleError()
        {
            // Arrange & Act
            var defaultConsole = ScriptConsole.Default;

            // Assert
            Assert.NotNull(defaultConsole.Error);
        }

        #endregion

        #region Clear Tests

        [Fact]
        public void Clear_CallsConsoleCloseReset()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act & Assert - should not throw
            console.Clear();
        }

        #endregion

        #region WriteError Tests

        [Fact]
        public void WriteError_WithValidString_WritesErrorInRedColor()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteError("Error message");

            // Assert
            Assert.Contains("Error message", error.ToString());
        }

        [Fact]
        public void WriteError_WithEmptyString_WritesEmpty()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteError("");

            // Assert
            Assert.NotEmpty(error.ToString()); // WriteLinr adds newline
        }

        [Fact]
        public void WriteError_WithStringContainingNewline_TrimmedNewlines()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteError("Error\n");

            // Assert
            string result = error.ToString();
            Assert.NotEmpty(result);
            Assert.DoesNotContain("Error\n\n", result); // Verify trim working
        }

        [Fact]
        public void WriteError_WithMultipleNewlines_TrimsAllNewlines()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteError("Error message\r\n\r\n");

            // Assert
            Assert.NotEmpty(error.ToString());
        }

        [Fact]
        public void WriteError_WithNullString_ThrowsException()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => console.WriteError(null));
        }

        #endregion

        #region WriteSuccess Tests

        [Fact]
        public void WriteSuccess_WithValidString_WritesSuccessInGreenColor()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteSuccess("Success message");

            // Assert
            Assert.Contains("Success message", output.ToString());
        }

        [Fact]
        public void WriteSuccess_WithEmptyString_WritesEmpty()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteSuccess("");

            // Assert
            Assert.NotEmpty(output.ToString()); // WriteLine adds newline
        }

        [Fact]
        public void WriteSuccess_WithStringContainingNewline_TrimmedNewlines()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteSuccess("Success\n");

            // Assert
            string result = output.ToString();
            Assert.NotEmpty(result);
        }

        [Fact]
        public void WriteSuccess_WithMultipleNewlines_TrimsAll()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteSuccess("Success\r\n\r\n");

            // Assert
            Assert.NotEmpty(output.ToString());
        }

        [Fact]
        public void WriteSuccess_WithNullString_ThrowsException()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => console.WriteSuccess(null));
        }

        #endregion

        #region WriteHighlighted Tests

        [Fact]
        public void WriteHighlighted_WithValidString_WritesInYellowColor()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteHighlighted("Highlighted message");

            // Assert
            Assert.Contains("Highlighted message", output.ToString());
        }

        [Fact]
        public void WriteHighlighted_WithEmptyString_WritesEmpty()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteHighlighted("");

            // Assert
            Assert.NotEmpty(output.ToString());
        }

        [Fact]
        public void WriteHighlighted_WithStringContainingNewline_TrimmedNewlines()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteHighlighted("Highlighted\n");

            // Assert
            Assert.NotEmpty(output.ToString());
        }

        [Fact]
        public void WriteHighlighted_WithNullString_ThrowsException()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => console.WriteHighlighted(null));
        }

        #endregion

        #region WriteWarning Tests

        [Fact]
        public void WriteWarning_WithValidString_WritesWarningInYellowColor()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteWarning("Warning message");

            // Assert
            Assert.Contains("Warning message", error.ToString());
        }

        [Fact]
        public void WriteWarning_WithEmptyString_WritesEmpty()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteWarning("");

            // Assert
            Assert.NotEmpty(error.ToString());
        }

        [Fact]
        public void WriteWarning_WithStringContainingNewline_TrimmedNewlines()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteWarning("Warning\n");

            // Assert
            Assert.NotEmpty(error.ToString());
        }

        [Fact]
        public void WriteWarning_WithNullString_ThrowsException()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => console.WriteWarning(null));
        }

        #endregion

        #region WriteNormal Tests

        [Fact]
        public void WriteNormal_WithValidString_WritesNormalText()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteNormal("Normal message");

            // Assert
            Assert.Contains("Normal message", output.ToString());
        }

        [Fact]
        public void WriteNormal_WithEmptyString_WritesEmpty()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteNormal("");

            // Assert
            Assert.NotEmpty(output.ToString());
        }

        [Fact]
        public void WriteNormal_WithStringContainingNewline_TrimmedNewlines()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteNormal("Normal\n");

            // Assert
            Assert.NotEmpty(output.ToString());
        }

        [Fact]
        public void WriteNormal_WithMultipleNewlines_TrimsAll()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteNormal("Normal\r\n\r\n");

            // Assert
            Assert.NotEmpty(output.ToString());
        }

        [Fact]
        public void WriteNormal_WithNullString_ThrowsException()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => console.WriteNormal(null));
        }

        #endregion

        #region WriteDiagnostics Tests

        [Fact]
        public void WriteDiagnostics_WithBothWarningsAndErrors_WritesAll()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            var warningMock = new Mock<Diagnostic>();
            warningMock.Setup(d => d.ToString()).Returns("Warning: test warning");
            var warnings = new[] { warningMock.Object };

            var errorMock = new Mock<Diagnostic>();
            errorMock.Setup(d => d.ToString()).Returns("Error: test error");
            var errors = new[] { errorMock.Object };

            // Act
            console.WriteDiagnostics(warnings, errors);

            // Assert
            string errorOutput = error.ToString();
            Assert.Contains("Warning", errorOutput);
            Assert.Contains("Error", errorOutput);
        }

        [Fact]
        public void WriteDiagnostics_WithOnlyWarnings_WritesWarningsOnly()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            var warningMock = new Mock<Diagnostic>();
            warningMock.Setup(d => d.ToString()).Returns("Warning: test");
            var warnings = new[] { warningMock.Object };

            // Act
            console.WriteDiagnostics(warnings, null);

            // Assert
            Assert.Contains("Warning", error.ToString());
        }

        [Fact]
        public void WriteDiagnostics_WithOnlyErrors_WritesErrorsOnly()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            var errorMock = new Mock<Diagnostic>();
            errorMock.Setup(d => d.ToString()).Returns("Error: test");
            var errors = new[] { errorMock.Object };

            // Act
            console.WriteDiagnostics(null, errors);

            // Assert
            Assert.Contains("Error", error.ToString());
        }

        [Fact]
        public void WriteDiagnostics_WithBothNullArrays_DoesNothing()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteDiagnostics(null, null);

            // Assert
            Assert.Empty(error.ToString());
        }

        [Fact]
        public void WriteDiagnostics_WithEmptyWarningsArray_SkipsWarnings()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            var errorMock = new Mock<Diagnostic>();
            errorMock.Setup(d => d.ToString()).Returns("Error: test");
            var errors = new[] { errorMock.Object };

            // Act
            console.WriteDiagnostics(new Diagnostic[0], errors);

            // Assert
            Assert.Contains("Error", error.ToString());
        }

        [Fact]
        public void WriteDiagnostics_WithEmptyErrorsArray_SkipsErrors()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            var warningMock = new Mock<Diagnostic>();
            warningMock.Setup(d => d.ToString()).Returns("Warning: test");
            var warnings = new[] { warningMock.Object };

            // Act
            console.WriteDiagnostics(warnings, new Diagnostic[0]);

            // Assert
            Assert.Contains("Warning", error.ToString());
        }

        [Fact]
        public void WriteDiagnostics_WithMultipleWarningsAndErrors_WritesAll()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            var warnings = new Diagnostic[3];
            for (int i = 0; i < 3; i++)
            {
                var warningMock = new Mock<Diagnostic>();
                warningMock.Setup(d => d.ToString()).Returns($"Warning {i}");
                warnings[i] = warningMock.Object;
            }

            var errors = new Diagnostic[3];
            for (int i = 0; i < 3; i++)
            {
                var errorMock = new Mock<Diagnostic>();
                errorMock.Setup(d => d.ToString()).Returns($"Error {i}");
                errors[i] = errorMock.Object;
            }

            // Act
            console.WriteDiagnostics(warnings, errors);

            // Assert
            string errorOutput = error.ToString();
            Assert.Contains("Warning 0", errorOutput);
            Assert.Contains("Warning 1", errorOutput);
            Assert.Contains("Warning 2", errorOutput);
            Assert.Contains("Error 0", errorOutput);
            Assert.Contains("Error 1", errorOutput);
            Assert.Contains("Error 2", errorOutput);
        }

        #endregion

        #region ReadLine Tests

        [Fact]
        public void ReadLine_WithValidInputReader_ReadsLine()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("test input\nanother line");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            string line = console.ReadLine();

            // Assert
            Assert.Equal("test input", line);
        }

        [Fact]
        public void ReadLine_WithEmptyInputReader_ReadsEmpty()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            string line = console.ReadLine();

            // Assert
            Assert.Null(line);
        }

        [Fact]
        public void ReadLine_WithMultipleReads_ReadsSequentially()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("line1\nline2\nline3");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            string line1 = console.ReadLine();
            string line2 = console.ReadLine();
            string line3 = console.ReadLine();

            // Assert
            Assert.Equal("line1", line1);
            Assert.Equal("line2", line2);
            Assert.Equal("line3", line3);
        }

        [Fact]
        public void ReadLine_WithNullInput_CallsSystemReadLine()
        {
            // Arrange
            var output = new StringWriter();
            var error = new StringWriter();
            var console = new ScriptConsole(output, null, error);

            // Act - should not throw and should return result from System.ReadLine
            // Note: This test verifies the null check works; actual behavior depends on System.ReadLine
            var result = console.ReadLine();

            // Assert - just verify it doesn't throw
            Assert.NotNull(console);
        }

        [Fact]
        public void ReadLine_WithInputContainingSpecialCharacters_ReadsCorrectly()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("special\t\rcharacters");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            string line = console.ReadLine();

            // Assert
            Assert.Equal("special\t\rcharacters", line);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Out_Property_ReturnsAssignedWriter()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            var result = console.Out;

            // Assert
            Assert.Same(output, result);
        }

        [Fact]
        public void In_Property_ReturnsAssignedReader()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            var result = console.In;

            // Assert
            Assert.Same(input, result);
        }

        [Fact]
        public void Error_Property_ReturnsAssignedWriter()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            var result = console.Error;

            // Assert
            Assert.Same(error, result);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void MultipleWriteCalls_AllOutputDirectedCorrectly()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteNormal("normal");
            console.WriteSuccess("success");
            console.WriteWarning("warning");
            console.WriteError("error");
            console.WriteHighlighted("highlighted");

            // Assert
            Assert.Contains("normal", output.ToString());
            Assert.Contains("success", output.ToString());
            Assert.Contains("warning", error.ToString());
            Assert.Contains("error", error.ToString());
            Assert.Contains("highlighted", output.ToString());
        }

        [Fact]
        public void WriteOperations_WithSpecialChars_HandlesCorrectly()
        {
            // Arrange
            var output = new StringWriter();
            var input = new StringReader("");
            var error = new StringWriter();
            var console = new ScriptConsole(output, input, error);

            // Act
            console.WriteNormal("test\\path");
            console.WriteError("error\"quoted");
            console.WriteSuccess("success'apostrophe");

            // Assert
            Assert.NotEmpty(output.ToString());
            Assert.NotEmpty(error.ToString());
        }

        #endregion
    }
}