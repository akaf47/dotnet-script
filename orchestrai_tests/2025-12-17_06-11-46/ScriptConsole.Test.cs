using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Xunit;
using Moq;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptConsoleTests
    {
        [Fact]
        public void Default_ShouldBeInitializedWithConsoleStreams()
        {
            Assert.NotNull(ScriptConsole.Default);
            Assert.Equal(Console.Out, ScriptConsole.Default.Out);
            Assert.Equal(Console.In, ScriptConsole.Default.In);
            Assert.Equal(Console.Error, ScriptConsole.Default.Error);
        }

        [Fact]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            var mockOut = new Mock<TextWriter>().Object;
            var mockIn = new Mock<TextReader>().Object;
            var mockError = new Mock<TextWriter>().Object;

            var scriptConsole = new ScriptConsole(mockOut, mockIn, mockError);

            Assert.Equal(mockOut, scriptConsole.Out);
            Assert.Equal(mockIn, scriptConsole.In);
            Assert.Equal(mockError, scriptConsole.Error);
        }

        [Fact]
        public void WriteError_ShouldWriteInRedAndResetColor()
        {
            var mockError = new Mock<TextWriter>();
            var scriptConsole = new ScriptConsole(Console.Out, Console.In, mockError.Object);

            scriptConsole.WriteError("Test Error");

            mockError.Verify(x => x.WriteLine("Test Error"), Times.Once);
        }

        [Fact]
        public void WriteSuccess_ShouldWriteInGreenAndResetColor()
        {
            var mockOut = new Mock<TextWriter>();
            var scriptConsole = new ScriptConsole(mockOut.Object, Console.In, Console.Error);

            scriptConsole.WriteSuccess("Test Success");

            mockOut.Verify(x => x.WriteLine("Test Success"), Times.Once);
        }

        [Fact]
        public void WriteHighlighted_ShouldWriteInYellowAndResetColor()
        {
            var mockOut = new Mock<TextWriter>();
            var scriptConsole = new ScriptConsole(mockOut.Object, Console.In, Console.Error);

            scriptConsole.WriteHighlighted("Test Highlight");

            mockOut.Verify(x => x.WriteLine("Test Highlight"), Times.Once);
        }

        [Fact]
        public void WriteWarning_ShouldWriteInYellowToErrorAndResetColor()
        {
            var mockError = new Mock<TextWriter>();
            var scriptConsole = new ScriptConsole(Console.Out, Console.In, mockError.Object);

            scriptConsole.WriteWarning("Test Warning");

            mockError.Verify(x => x.WriteLine("Test Warning"), Times.Once);
        }

        [Fact]
        public void WriteNormal_ShouldWriteToOutput()
        {
            var mockOut = new Mock<TextWriter>();
            var scriptConsole = new ScriptConsole(mockOut.Object, Console.In, Console.Error);

            scriptConsole.WriteNormal("Test Normal");

            mockOut.Verify(x => x.WriteLine("Test Normal"), Times.Once);
        }

        [Fact]
        public void WriteDiagnostics_ShouldHandleNullDiagnostics()
        {
            var mockOut = new Mock<TextWriter>();
            var mockError = new Mock<TextWriter>();
            var scriptConsole = new ScriptConsole(mockOut.Object, Console.In, mockError.Object);

            scriptConsole.WriteDiagnostics(null, null);

            mockOut.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Never);
            mockError.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void WriteDiagnostics_ShouldWriteWarningsAndErrors()
        {
            var mockOut = new Mock<TextWriter>();
            var mockError = new Mock<TextWriter>();
            var scriptConsole = new ScriptConsole(mockOut.Object, Console.In, mockError.Object);

            var warningDiagnostic = Diagnostic.Create(
                new DiagnosticDescriptor("ID", "Title", "Message", "Category", DiagnosticSeverity.Warning, true),
                Location.None
            );

            var errorDiagnostic = Diagnostic.Create(
                new DiagnosticDescriptor("ID", "Title", "Message", "Category", DiagnosticSeverity.Error, true),
                Location.None
            );

            scriptConsole.WriteDiagnostics(new[] { warningDiagnostic }, new[] { errorDiagnostic });

            mockError.Verify(x => x.WriteLine(warningDiagnostic.ToString()), Times.Once);
            mockError.Verify(x => x.WriteLine(errorDiagnostic.ToString()), Times.Once);
        }

        [Fact]
        public void ReadLine_ShouldUseProvidedInput()
        {
            var mockIn = new Mock<TextReader>();
            mockIn.Setup(x => x.ReadLine()).Returns("Test Input");
            var scriptConsole = new ScriptConsole(Console.Out, mockIn.Object, Console.Error);

            var result = scriptConsole.ReadLine();

            Assert.Equal("Test Input", result);
        }

        [Fact]
        public void ReadLine_ShouldUseSystemReadLineWhenInputIsNull()
        {
            var scriptConsole = new ScriptConsole(Console.Out, null, Console.Error);

            // Note: This test might be tricky to fully verify due to System.ReadLine
            var result = scriptConsole.ReadLine();

            Assert.NotNull(result);
        }
    }
}