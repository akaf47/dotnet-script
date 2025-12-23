using System;
using System.IO;
using Xunit;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class InitCommandOptionsTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidFileNameAndWorkingDirectory_InitializesCorrectly()
        {
            // Arrange
            string fileName = "script.csx";
            string workingDirectory = "/home/user/project";

            // Act
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Assert
            Assert.Equal(fileName, options.FileName);
            Assert.Equal(workingDirectory, options.WorkingDirectory);
        }

        [Fact]
        public void Constructor_WithNullWorkingDirectory_UsesCurrentDirectory()
        {
            // Arrange
            string fileName = "script.csx";
            string workingDirectory = null;
            string expectedDirectory = Directory.GetCurrentDirectory();

            // Act
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Assert
            Assert.Equal(fileName, options.FileName);
            Assert.Equal(expectedDirectory, options.WorkingDirectory);
        }

        [Fact]
        public void Constructor_WithEmptyFileName_IsAccepted()
        {
            // Arrange
            string fileName = "";
            string workingDirectory = "/home/user";

            // Act
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Assert
            Assert.Equal(fileName, options.FileName);
            Assert.Equal(workingDirectory, options.WorkingDirectory);
        }

        [Fact]
        public void Constructor_WithNullFileName_IsAccepted()
        {
            // Arrange
            string fileName = null;
            string workingDirectory = "/home/user";

            // Act
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Assert
            Assert.Null(options.FileName);
            Assert.Equal(workingDirectory, options.WorkingDirectory);
        }

        [Fact]
        public void Constructor_WithWhitespaceFileName_IsPreserved()
        {
            // Arrange
            string fileName = "  script.csx  ";
            string workingDirectory = "/home/user";

            // Act
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Assert
            Assert.Equal(fileName, options.FileName);
        }

        [Fact]
        public void Constructor_WithDifferentPathFormats_WorkingDirectoryPreserved()
        {
            // Arrange
            string fileName = "script.csx";
            string workingDirectory = "/home/user/project";

            // Act
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Assert
            Assert.Equal(workingDirectory, options.WorkingDirectory);
        }

        [Fact]
        public void Constructor_WithFileNameContainingPath_IsAccepted()
        {
            // Arrange
            string fileName = "subfolder/script.csx";
            string workingDirectory = "/home/user";

            // Act
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Assert
            Assert.Equal(fileName, options.FileName);
        }

        [Fact]
        public void Constructor_WithLongPathWorkingDirectory_IsAccepted()
        {
            // Arrange
            string fileName = "script.csx";
            string workingDirectory = new string('a', 500) + "/path";

            // Act
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Assert
            Assert.Equal(workingDirectory, options.WorkingDirectory);
        }

        [Fact]
        public void Constructor_WithLongFileNamePath_IsAccepted()
        {
            // Arrange
            string fileName = new string('a', 200) + ".csx";
            string workingDirectory = "/home/user";

            // Act
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Assert
            Assert.Equal(fileName, options.FileName);
        }

        [Fact]
        public void Constructor_WithSpecialCharactersInFileName_IsPreserved()
        {
            // Arrange
            string fileName = "script-name_2023.csx";
            string workingDirectory = "/home/user";

            // Act
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Assert
            Assert.Equal(fileName, options.FileName);
        }

        #endregion

        #region FileName Property Tests

        [Fact]
        public void FileName_IsReadOnly_CanOnlyBeRetrieved()
        {
            // Arrange
            string fileName = "script.csx";
            string workingDirectory = "/home/user";
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Act
            var retrievedFileName = options.FileName;

            // Assert
            Assert.Equal(fileName, retrievedFileName);
        }

        [Fact]
        public void FileName_ReturnsNullWhenConstructedWithNull()
        {
            // Arrange
            var options = new InitCommandOptions(null, "/home/user");

            // Act
            var retrievedFileName = options.FileName;

            // Assert
            Assert.Null(retrievedFileName);
        }

        [Fact]
        public void FileName_ReturnsEmptyStringWhenConstructedWithEmptyString()
        {
            // Arrange
            var options = new InitCommandOptions("", "/home/user");

            // Act
            var retrievedFileName = options.FileName;

            // Assert
            Assert.Equal("", retrievedFileName);
        }

        [Fact]
        public void FileName_ReturnsExactValueProvided()
        {
            // Arrange
            string fileName = "my_script_2023.csx";
            var options = new InitCommandOptions(fileName, "/home/user");

            // Act
            var retrievedFileName = options.FileName;

            // Assert
            Assert.Equal(fileName, retrievedFileName);
        }

        [Fact]
        public void FileName_WithUnicodeCharacters_IsPreserved()
        {
            // Arrange
            string fileName = "脚本_スクリプト.csx";
            var options = new InitCommandOptions(fileName, "/home/user");

            // Act
            var retrievedFileName = options.FileName;

            // Assert
            Assert.Equal(fileName, retrievedFileName);
        }

        [Fact]
        public void FileName_WithMultipleExtensions_IsPreserved()
        {
            // Arrange
            string fileName = "script.backup.csx";
            var options = new InitCommandOptions(fileName, "/home/user");

            // Act
            var retrievedFileName = options.FileName;

            // Assert
            Assert.Equal(fileName, retrievedFileName);
        }

        #endregion

        #region WorkingDirectory Property Tests

        [Fact]
        public void WorkingDirectory_IsReadOnly_CanOnlyBeRetrieved()
        {
            // Arrange
            string fileName = "script.csx";
            string workingDirectory = "/home/user/project";
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Act
            var retrievedDirectory = options.WorkingDirectory;

            // Assert
            Assert.Equal(workingDirectory, retrievedDirectory);
        }

        [Fact]
        public void WorkingDirectory_DefaultsToCurrentDirectoryWhenNull()
        {
            // Arrange
            string currentDir = Directory.GetCurrentDirectory();
            var options = new InitCommandOptions("script.csx", null);

            // Act
            var retrievedDirectory = options.WorkingDirectory;

            // Assert
            Assert.Equal(currentDir, retrievedDirectory);
        }

        [Fact]
        public void WorkingDirectory_ReturnsExactValueWhenNotNull()
        {
            // Arrange
            string workingDirectory = "/home/user/projects/myapp";
            var options = new InitCommandOptions("script.csx", workingDirectory);

            // Act
            var retrievedDirectory = options.WorkingDirectory;

            // Assert
            Assert.Equal(workingDirectory, retrievedDirectory);
        }

        [Fact]
        public void WorkingDirectory_WithTrailingSlash_IsPreserved()
        {
            // Arrange
            string workingDirectory = "/home/user/project/";
            var options = new InitCommandOptions("script.csx", workingDirectory);

            // Act
            var retrievedDirectory = options.WorkingDirectory;

            // Assert
            Assert.Equal(workingDirectory, retrievedDirectory);
        }

        [Fact]
        public void WorkingDirectory_WithoutTrailingSlash_IsPreserved()
        {
            // Arrange
            string workingDirectory = "/home/user/project";
            var options = new InitCommandOptions("script.csx", workingDirectory);

            // Act
            var retrievedDirectory = options.WorkingDirectory;

            // Assert
            Assert.Equal(workingDirectory, retrievedDirectory);
        }

        [Fact]
        public void WorkingDirectory_WithMixedPathSeparators_IsPreserved()
        {
            // Arrange
            string workingDirectory = "/home\\user/project\\subfolder";
            var options = new InitCommandOptions("script.csx", workingDirectory);

            // Act
            var retrievedDirectory = options.WorkingDirectory;

            // Assert
            Assert.Equal(workingDirectory, retrievedDirectory);
        }

        [Fact]
        public void WorkingDirectory_WithWhitespace_IsPreserved()
        {
            // Arrange
            string workingDirectory = "  /home/user/project  ";
            var options = new InitCommandOptions("script.csx", workingDirectory);

            // Act
            var retrievedDirectory = options.WorkingDirectory;

            // Assert
            Assert.Equal(workingDirectory, retrievedDirectory);
        }

        [Fact]
        public void WorkingDirectory_WithUnicodeCharacters_IsPreserved()
        {
            // Arrange
            string workingDirectory = "/home/用户/プロジェクト";
            var options = new InitCommandOptions("script.csx", workingDirectory);

            // Act
            var retrievedDirectory = options.WorkingDirectory;

            // Assert
            Assert.Equal(workingDirectory, retrievedDirectory);
        }

        [Fact]
        public void WorkingDirectory_WithRelativePath_IsPreserved()
        {
            // Arrange
            string workingDirectory = "./relative/path/to/project";
            var options = new InitCommandOptions("script.csx", workingDirectory);

            // Act
            var retrievedDirectory = options.WorkingDirectory;

            // Assert
            Assert.Equal(workingDirectory, retrievedDirectory);
        }

        #endregion

        #region NullCoalescing Logic Tests

        [Fact]
        public void Constructor_NullWorkingDirectory_CoalescesToCurrentDirectory()
        {
            // Arrange
            string currentDir = Directory.GetCurrentDirectory();

            // Act
            var options = new InitCommandOptions("script.csx", null);

            // Assert
            Assert.NotNull(options.WorkingDirectory);
            Assert.Equal(currentDir, options.WorkingDirectory);
        }

        [Fact]
        public void Constructor_NullWorkingDirectory_DoesNotThrowException()
        {
            // Arrange & Act & Assert
            var options = new InitCommandOptions("script.csx", null);
            Assert.NotNull(options.WorkingDirectory);
        }

        [Fact]
        public void Constructor_WhitespaceWorkingDirectory_IsPreservedNotCoalesced()
        {
            // Arrange
            string workingDirectory = "   ";

            // Act
            var options = new InitCommandOptions("script.csx", workingDirectory);

            // Assert
            Assert.Equal(workingDirectory, options.WorkingDirectory);
        }

        [Fact]
        public void Constructor_EmptyStringWorkingDirectory_IsPreservedNotCoalesced()
        {
            // Arrange
            string workingDirectory = "";

            // Act
            var options = new InitCommandOptions("script.csx", workingDirectory);

            // Assert
            Assert.Equal(workingDirectory, options.WorkingDirectory);
        }

        #endregion

        #region Combined Property Tests

        [Fact]
        public void MultipleInstances_HaveIndependentValues()
        {
            // Arrange & Act
            var options1 = new InitCommandOptions("script1.csx", "/path1");
            var options2 = new InitCommandOptions("script2.csx", "/path2");

            // Assert
            Assert.NotEqual(options1.FileName, options2.FileName);
            Assert.NotEqual(options1.WorkingDirectory, options2.WorkingDirectory);
        }

        [Fact]
        public void Constructor_WithBothParametersNull_WorkingDirectoryGetsCurrentDir()
        {
            // Arrange
            string currentDir = Directory.GetCurrentDirectory();

            // Act
            var options = new InitCommandOptions(null, null);

            // Assert
            Assert.Null(options.FileName);
            Assert.Equal(currentDir, options.WorkingDirectory);
        }

        [Fact]
        public void Constructor_WithFileNameEmptyAndWorkingDirectoryNull_WorkingDirectoryGetsCurrentDir()
        {
            // Arrange
            string currentDir = Directory.GetCurrentDirectory();

            // Act
            var options = new InitCommandOptions("", null);

            // Assert
            Assert.Equal("", options.FileName);
            Assert.Equal(currentDir, options.WorkingDirectory);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Constructor_WithVeryLongFileNameAndPath_IsHandledCorrectly()
        {
            // Arrange
            string fileName = new string('x', 1000) + ".csx";
            string workingDirectory = new string('y', 1000) + "/path";

            // Act
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Assert
            Assert.Equal(fileName, options.FileName);
            Assert.Equal(workingDirectory, options.WorkingDirectory);
        }

        [Fact]
        public void Constructor_WithNewlineInFileName_IsPreserved()
        {
            // Arrange
            string fileName = "script\nname.csx";

            // Act
            var options = new InitCommandOptions(fileName, "/home/user");

            // Assert
            Assert.Equal(fileName, options.FileName);
        }

        [Fact]
        public void Constructor_WithTabInFileName_IsPreserved()
        {
            // Arrange
            string fileName = "script\tname.csx";

            // Act
            var options = new InitCommandOptions(fileName, "/home/user");

            // Assert
            Assert.Equal(fileName, options.FileName);
        }

        [Fact]
        public void Constructor_WithSpecialPathCharacters_IsPreserved()
        {
            // Arrange
            string fileName = "script@#$%.csx";
            string workingDirectory = "/home/user@domain/proj$123";

            // Act
            var options = new InitCommandOptions(fileName, workingDirectory);

            // Assert
            Assert.Equal(fileName, options.FileName);
            Assert.Equal(workingDirectory, options.WorkingDirectory);
        }

        #endregion
    }
}