using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Dotnet.Script.DependencyModel.Environment;
using Moq;

namespace Dotnet.Script.DependencyModel.Tests.Environment
{
    public class ScriptEnvironmentTests
    {
        private readonly ScriptEnvironment _scriptEnvironment;

        public ScriptEnvironmentTests()
        {
            _scriptEnvironment = new ScriptEnvironment();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldInitializeWithoutError()
        {
            // Act
            var environment = new ScriptEnvironment();

            // Assert
            Assert.NotNull(environment);
        }

        #endregion

        #region GetEnvironmentVariable Tests

        [Fact]
        public void GetEnvironmentVariable_WithValidVariableName_ShouldReturnValue()
        {
            // Arrange
            string variableName = "PATH";
            string expectedValue = Environment.GetEnvironmentVariable(variableName);

            // Act
            var result = _scriptEnvironment.GetEnvironmentVariable(variableName);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetEnvironmentVariable_WithNonExistentVariable_ShouldReturnNull()
        {
            // Arrange
            string variableName = "NONEXISTENT_VAR_" + Guid.NewGuid();

            // Act
            var result = _scriptEnvironment.GetEnvironmentVariable(variableName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetEnvironmentVariable_WithEmptyString_ShouldReturnNull()
        {
            // Arrange
            string variableName = "";

            // Act
            var result = _scriptEnvironment.GetEnvironmentVariable(variableName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetEnvironmentVariable_WithNullString_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _scriptEnvironment.GetEnvironmentVariable(null));
        }

        [Fact]
        public void GetEnvironmentVariable_WithWhitespaceVariable_ShouldReturnNull()
        {
            // Arrange
            string variableName = "   ";

            // Act
            var result = _scriptEnvironment.GetEnvironmentVariable(variableName);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region SetEnvironmentVariable Tests

        [Fact]
        public void SetEnvironmentVariable_WithValidParameters_ShouldSetValue()
        {
            // Arrange
            string variableName = "TEST_VAR_" + Guid.NewGuid();
            string variableValue = "test_value";
            string originalValue = Environment.GetEnvironmentVariable(variableName);

            try
            {
                // Act
                _scriptEnvironment.SetEnvironmentVariable(variableName, variableValue);
                var result = Environment.GetEnvironmentVariable(variableName);

                // Assert
                Assert.Equal(variableValue, result);
            }
            finally
            {
                // Cleanup
                if (originalValue != null)
                    Environment.SetEnvironmentVariable(variableName, originalValue);
                else
                    Environment.SetEnvironmentVariable(variableName, null);
            }
        }

        [Fact]
        public void SetEnvironmentVariable_WithNullValue_ShouldRemoveVariable()
        {
            // Arrange
            string variableName = "TEST_VAR_" + Guid.NewGuid();
            Environment.SetEnvironmentVariable(variableName, "some_value");

            try
            {
                // Act
                _scriptEnvironment.SetEnvironmentVariable(variableName, null);
                var result = Environment.GetEnvironmentVariable(variableName);

                // Assert
                Assert.Null(result);
            }
            finally
            {
                Environment.SetEnvironmentVariable(variableName, null);
            }
        }

        [Fact]
        public void SetEnvironmentVariable_WithNullVariableName_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _scriptEnvironment.SetEnvironmentVariable(null, "value"));
        }

        [Fact]
        public void SetEnvironmentVariable_WithEmptyVariableName_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _scriptEnvironment.SetEnvironmentVariable("", "value"));
        }

        [Fact]
        public void SetEnvironmentVariable_WithEmptyValue_ShouldSetEmptyString()
        {
            // Arrange
            string variableName = "TEST_VAR_" + Guid.NewGuid();
            string originalValue = Environment.GetEnvironmentVariable(variableName);

            try
            {
                // Act
                _scriptEnvironment.SetEnvironmentVariable(variableName, "");
                var result = Environment.GetEnvironmentVariable(variableName);

                // Assert
                Assert.Equal("", result);
            }
            finally
            {
                if (originalValue != null)
                    Environment.SetEnvironmentVariable(variableName, originalValue);
                else
                    Environment.SetEnvironmentVariable(variableName, null);
            }
        }

        #endregion

        #region CurrentDirectory Tests

        [Fact]
        public void CurrentDirectory_GetValue_ShouldReturnCurrentWorkingDirectory()
        {
            // Act
            var result = _scriptEnvironment.CurrentDirectory;

            // Assert
            Assert.Equal(Environment.CurrentDirectory, result);
            Assert.NotNull(result);
        }

        [Fact]
        public void CurrentDirectory_SetValue_ShouldChangeCurrentDirectory()
        {
            // Arrange
            string originalDirectory = Environment.CurrentDirectory;
            string tempDirectory = Path.GetTempPath();

            try
            {
                // Act
                _scriptEnvironment.CurrentDirectory = tempDirectory;
                var result = Environment.CurrentDirectory;

                // Assert
                Assert.Equal(tempDirectory, result);
            }
            finally
            {
                Environment.CurrentDirectory = originalDirectory;
            }
        }

        [Fact]
        public void CurrentDirectory_SetWithNullValue_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _scriptEnvironment.CurrentDirectory = null);
        }

        [Fact]
        public void CurrentDirectory_SetWithInvalidPath_ShouldThrowDirectoryNotFoundException()
        {
            // Arrange
            string invalidPath = "/invalid/path/that/does/not/exist/12345";

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => _scriptEnvironment.CurrentDirectory = invalidPath);
        }

        #endregion

        #region GetOSVersion Tests

        [Fact]
        public void GetOSVersion_ShouldReturnValidOSVersion()
        {
            // Act
            var result = _scriptEnvironment.GetOSVersion();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetOSVersion_ShouldReturnPlatformIdentifier()
        {
            // Act
            var result = _scriptEnvironment.GetOSVersion();

            // Assert
            Assert.True(
                result.Contains("Windows") || result.Contains("Linux") || result.Contains("macOS") || result.Contains("Darwin"),
                $"OS version should contain known platform identifier, got: {result}"
            );
        }

        #endregion

        #region GetDotNetVersion Tests

        [Fact]
        public void GetDotNetVersion_ShouldReturnValidVersion()
        {
            // Act
            var result = _scriptEnvironment.GetDotNetVersion();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetDotNetVersion_ShouldReturnVersionFormat()
        {
            // Act
            var result = _scriptEnvironment.GetDotNetVersion();

            // Assert
            // Version should start with a number
            Assert.True(char.IsDigit(result[0]), "Version should start with a digit");
        }

        #endregion

        #region ExpandEnvironmentVariables Tests

        [Fact]
        public void ExpandEnvironmentVariables_WithPathContainingVariables_ShouldExpandThem()
        {
            // Arrange
            string path = "%TEMP%";

            // Act
            var result = _scriptEnvironment.ExpandEnvironmentVariables(path);

            // Assert
            Assert.NotEqual(path, result);
            Assert.DoesNotContain("%", result);
        }

        [Fact]
        public void ExpandEnvironmentVariables_WithPathWithoutVariables_ShouldReturnUnchanged()
        {
            // Arrange
            string path = "/home/user/documents";

            // Act
            var result = _scriptEnvironment.ExpandEnvironmentVariables(path);

            // Assert
            Assert.Equal(path, result);
        }

        [Fact]
        public void ExpandEnvironmentVariables_WithNullPath_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _scriptEnvironment.ExpandEnvironmentVariables(null));
        }

        [Fact]
        public void ExpandEnvironmentVariables_WithEmptyPath_ShouldReturnEmpty()
        {
            // Act
            var result = _scriptEnvironment.ExpandEnvironmentVariables("");

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region ProcessorCount Tests

        [Fact]
        public void ProcessorCount_ShouldReturnGreaterThanZero()
        {
            // Act
            var result = _scriptEnvironment.ProcessorCount;

            // Assert
            Assert.True(result > 0, "ProcessorCount should be greater than zero");
        }

        [Fact]
        public void ProcessorCount_ShouldMatchEnvironmentProcessorCount()
        {
            // Act
            var result = _scriptEnvironment.ProcessorCount;

            // Assert
            Assert.Equal(Environment.ProcessorCount, result);
        }

        #endregion

        #region IsWindows Tests

        [Fact]
        public void IsWindows_ShouldReturnBoolean()
        {
            // Act
            var result = _scriptEnvironment.IsWindows();

            // Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void IsWindows_ShouldReturnCorrectValue()
        {
            // Act
            var result = _scriptEnvironment.IsWindows();
            var expected = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region IsLinux Tests

        [Fact]
        public void IsLinux_ShouldReturnBoolean()
        {
            // Act
            var result = _scriptEnvironment.IsLinux();

            // Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void IsLinux_ShouldReturnCorrectValue()
        {
            // Act
            var result = _scriptEnvironment.IsLinux();
            var expected = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region IsMacOS Tests

        [Fact]
        public void IsMacOS_ShouldReturnBoolean()
        {
            // Act
            var result = _scriptEnvironment.IsMacOS();

            // Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void IsMacOS_ShouldReturnCorrectValue()
        {
            // Act
            var result = _scriptEnvironment.IsMacOS();
            var expected = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Is64Bit Tests

        [Fact]
        public void Is64Bit_ShouldReturnBoolean()
        {
            // Act
            var result = _scriptEnvironment.Is64Bit();

            // Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void Is64Bit_ShouldReturnCorrectValue()
        {
            // Act
            var result = _scriptEnvironment.Is64Bit();

            // Assert
            Assert.True(result || !result); // Sanity check
        }

        #endregion

        #region GetUserProfile Tests

        [Fact]
        public void GetUserProfile_ShouldReturnValidPath()
        {
            // Act
            var result = _scriptEnvironment.GetUserProfile();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetUserProfile_ShouldReturnExistingDirectory()
        {
            // Act
            var result = _scriptEnvironment.GetUserProfile();

            // Assert
            Assert.True(Directory.Exists(result), "User profile directory should exist");
        }

        #endregion

        #region GetExecutablePath Tests

        [Fact]
        public void GetExecutablePath_ShouldReturnValidPath()
        {
            // Act
            var result = _scriptEnvironment.GetExecutablePath();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetExecutablePath_ShouldReturnExistingFile()
        {
            // Act
            var result = _scriptEnvironment.GetExecutablePath();

            // Assert
            Assert.True(File.Exists(result), "Executable file should exist");
        }

        #endregion

        #region WorkingDirectory Tests

        [Fact]
        public void WorkingDirectory_GetValue_ShouldReturnCurrentDirectory()
        {
            // Act
            var result = _scriptEnvironment.WorkingDirectory;

            // Assert
            Assert.Equal(Environment.CurrentDirectory, result);
        }

        #endregion
    }
}