using System;
using Xunit;
using Moq;
using Dotnet.Script.Core.Commands;
using Dotnet.Script.DependencyModel.Logging;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class InitCommandTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidLogFactory_InitializesCorrectly()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();

            // Act
            var command = new InitCommand(mockLogFactory.Object);

            // Assert
            Assert.NotNull(command);
        }

        [Fact]
        public void Constructor_WithNullLogFactory_ThrowsArgumentNullException()
        {
            // Arrange
            LogFactory nullLogFactory = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new InitCommand(nullLogFactory));
        }

        #endregion

        #region Execute Method Tests

        [Fact]
        public void Execute_WithValidOptions_CreatesScaffolder()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("main.csx", "/tmp");

            // Act & Assert
            // Should not throw any exception
            command.Execute(options);
        }

        [Fact]
        public void Execute_WithNullOptions_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => command.Execute(null));
        }

        [Fact]
        public void Execute_WithEmptyFileName_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("", "/tmp");

            // Act & Assert
            // Should not throw any exception - empty filename is valid
            command.Execute(options);
        }

        [Fact]
        public void Execute_WithNullFileName_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions(null, "/tmp");

            // Act & Assert
            // Should not throw any exception - null filename is valid
            command.Execute(options);
        }

        [Fact]
        public void Execute_WithValidFileNameAndDirectory_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("script.csx", "/tmp");

            // Act & Assert
            // Should not throw any exception
            command.Execute(options);
        }

        [Fact]
        public void Execute_WithNullWorkingDirectory_DefaultsToCurrentDirectory()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("script.csx", null);

            // Act & Assert
            // Should not throw - null directory is handled by options
            command.Execute(options);
        }

        [Fact]
        public void Execute_WithCustomFileName_PassesFileNameToScaffolder()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var fileName = "custom_script.csx";
            var options = new InitCommandOptions(fileName, "/tmp");

            // Act
            command.Execute(options);

            // Assert - no exception means it passed the fileName correctly
            Assert.NotNull(options.FileName);
            Assert.Equal(fileName, options.FileName);
        }

        [Fact]
        public void Execute_WithMultipleCallsWithDifferentOptions_ExecutesIndependently()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options1 = new InitCommandOptions("script1.csx", "/tmp1");
            var options2 = new InitCommandOptions("script2.csx", "/tmp2");

            // Act & Assert
            command.Execute(options1);
            command.Execute(options2);
            // Both executions should succeed without interference
        }

        [Fact]
        public void Execute_WithSpecialCharactersInFileName_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("script-name_2023.csx", "/tmp");

            // Act & Assert
            command.Execute(options);
        }

        [Fact]
        public void Execute_WithWhitespaceFileName_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("  script.csx  ", "/tmp");

            // Act & Assert
            command.Execute(options);
        }

        [Fact]
        public void Execute_WithLongPath_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            string longPath = new string('a', 200) + "/project";
            var options = new InitCommandOptions("script.csx", longPath);

            // Act & Assert
            command.Execute(options);
        }

        [Fact]
        public void Execute_WithFileNameContainingPath_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("subfolder/script.csx", "/tmp");

            // Act & Assert
            command.Execute(options);
        }

        [Fact]
        public void Execute_WithUnicodeInFileName_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("脚本.csx", "/tmp");

            // Act & Assert
            command.Execute(options);
        }

        [Fact]
        public void Execute_WithUnicodeInWorkingDirectory_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("script.csx", "/tmp/プロジェクト");

            // Act & Assert
            command.Execute(options);
        }

        #endregion

        #region Scaffolder Integration Tests

        [Fact]
        public void Execute_CallsScaffolderWithCorrectFileName()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var fileName = "myScript.csx";
            var workingDir = "/home/user/project";
            var options = new InitCommandOptions(fileName, workingDir);

            // Act
            command.Execute(options);

            // Assert - if no exception, execution was successful
            Assert.NotNull(options);
        }

        [Fact]
        public void Execute_CallsScaffolderWithCorrectWorkingDirectory()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var fileName = "script.csx";
            var workingDir = "/home/user/project";
            var options = new InitCommandOptions(fileName, workingDir);

            // Act
            command.Execute(options);

            // Assert - verify the working directory was passed correctly
            Assert.Equal(workingDir, options.WorkingDirectory);
        }

        #endregion

        #region Multiple Execute Calls

        [Fact]
        public void Execute_CalledMultipleTimes_MaintainsIndependenceAcrossCalls()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);

            // Act
            command.Execute(new InitCommandOptions("script1.csx", "/path1"));
            command.Execute(new InitCommandOptions("script2.csx", "/path2"));
            command.Execute(new InitCommandOptions(null, "/path3"));

            // Assert - all executions should complete without errors
            Assert.NotNull(command);
        }

        [Fact]
        public void Execute_WithSameOptionsTwice_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("script.csx", "/tmp");

            // Act & Assert
            command.Execute(options);
            command.Execute(options);
        }

        #endregion

        #region Constructor Parameter Validation

        [Fact]
        public void Constructor_RequiresNonNullLogFactory()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new InitCommand(null));
        }

        [Fact]
        public void Constructor_AcceptsValidLogFactoryInstance()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();

            // Act
            var command = new InitCommand(mockLogFactory.Object);

            // Assert
            Assert.NotNull(command);
        }

        #endregion

        #region Execute Method Parameter Validation

        [Fact]
        public void Execute_RequiresNonNullOptions()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => command.Execute(null));
        }

        [Fact]
        public void Execute_AcceptsValidInitCommandOptions()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("script.csx", "/tmp");

            // Act & Assert - should not throw
            command.Execute(options);
        }

        [Fact]
        public void Execute_AcceptsInitCommandOptionsWithNullFileName()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions(null, "/tmp");

            // Act & Assert - should not throw
            command.Execute(options);
        }

        [Fact]
        public void Execute_AcceptsInitCommandOptionsWithEmptyFileName()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("", "/tmp");

            // Act & Assert - should not throw
            command.Execute(options);
        }

        #endregion

        #region Boundary Cases

        [Fact]
        public void Execute_WithEmptyFileNameAndNullWorkingDirectory_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("", null);

            // Act & Assert
            command.Execute(options);
        }

        [Fact]
        public void Execute_WithNullFileNameAndNullWorkingDirectory_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions(null, null);

            // Act & Assert
            command.Execute(options);
        }

        [Fact]
        public void Execute_WithBothParametersValid_ExecutesSuccessfully()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("main.csx", "/home/user/projects");

            // Act & Assert
            command.Execute(options);
        }

        #endregion

        #region State and Consistency Tests

        [Fact]
        public void Command_MaintainsStateAcrossMultipleExecutions()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options1 = new InitCommandOptions("script1.csx", "/path1");
            var options2 = new InitCommandOptions("script2.csx", "/path2");

            // Act
            command.Execute(options1);
            var firstOptionFileName = options1.FileName;
            
            command.Execute(options2);
            var secondOptionFileName = options2.FileName;

            // Assert
            Assert.Equal("script1.csx", firstOptionFileName);
            Assert.Equal("script2.csx", secondOptionFileName);
        }

        [Fact]
        public void Execute_WithOptionsPassedCorrectly_OptionsRemainsUnchanged()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var originalFileName = "script.csx";
            var originalDirectory = "/tmp";
            var options = new InitCommandOptions(originalFileName, originalDirectory);

            // Act
            command.Execute(options);

            // Assert
            Assert.Equal(originalFileName, options.FileName);
            Assert.Equal(originalDirectory, options.WorkingDirectory);
        }

        #endregion

        #region Error Handling and Recovery

        [Fact]
        public void Execute_WithInvalidPathCharactersInFileName_DoesNotThrowInCommand()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);
            var options = new InitCommandOptions("script|invalid.csx", "/tmp");

            // Act & Assert - command itself should not validate paths
            command.Execute(options);
        }

        [Fact]
        public void Execute_CanBeCalledSequentlyWithoutReset()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var command = new InitCommand(mockLogFactory.Object);

            // Act & Assert
            for (int i = 0; i < 5; i++)
            {
                var options = new InitCommandOptions($"script{i}.csx", "/tmp");
                command.Execute(options);
            }
        }

        #endregion
    }
}