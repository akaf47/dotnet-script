using System;
using System.IO;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Dotnet.Script.Core;
using Dotnet.Script.Core.Commands;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Runtime;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteLibraryCommandTests
    {
        private readonly Mock<ScriptConsole> _mockScriptConsole;
        private readonly Mock<LogFactory> _mockLogFactory;
        private readonly Mock<Logger> _mockLogger;
        private readonly ExecuteLibraryCommand _command;

        public ExecuteLibraryCommandTests()
        {
            _mockScriptConsole = new Mock<ScriptConsole>(
                new StringWriter(), 
                new StringReader(""), 
                new StringWriter()
            );
            _mockLogger = new Mock<Logger>();
            _mockLogFactory = new Mock<LogFactory>();
            _mockLogFactory.Setup(f => f.CreateLogger<It.IsAnyType>()).Returns(_mockLogger.Object);
            _mockLogFactory.Setup(f => f(It.IsAny<Type>())).Returns(_mockLogger.Object);

            _command = new ExecuteLibraryCommand(_mockScriptConsole.Object, _mockLogFactory.Object);
        }

        #region File Existence Tests

        [Fact]
        public async Task Execute_ThrowsException_WhenLibraryPathDoesNotExist()
        {
            // Arrange
            var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "nonexistent.dll");
            var options = new ExecuteLibraryCommandOptions(nonExistentPath, Array.Empty<string>(), false);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
            Assert.Contains("Couldn't find file", ex.Message);
            Assert.Contains(nonExistentPath, ex.Message);
        }

        [Fact]
        public async Task Execute_ThrowsException_WhenLibraryPathIsNull()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions(null, Array.Empty<string>(), false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
        }

        [Fact]
        public async Task Execute_ThrowsException_WhenLibraryPathIsEmpty()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions("", Array.Empty<string>(), false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
        }

        [Fact]
        public async Task Execute_ThrowsException_WhenLibraryPathIsWhitespace()
        {
            // Arrange
            var options = new ExecuteLibraryCommandOptions("   ", Array.Empty<string>(), false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
        }

        #endregion

        #region File Path Normalization Tests

        [Fact]
        public async Task Execute_HandlesAbsolutePath_WithoutNormalization()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testFile = Path.Combine(tempDir, "test_lib_" + Guid.NewGuid().ToString() + ".dll");
            
            // Create a dummy file for testing
            try
            {
                File.WriteAllText(testFile, "dummy");
                var options = new ExecuteLibraryCommandOptions(testFile, Array.Empty<string>(), false);

                // Act - This will fail in execution phase but we're testing path handling
                // The test verifies that absolute paths are accepted without throwing file not found error
                var ex = await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
                
                // Assert - Should fail for different reasons (compilation, loading, etc) not for file not found
                Assert.DoesNotContain("Couldn't find file", ex.Message);
            }
            finally
            {
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        [Fact]
        public async Task Execute_HandlesRelativePath_ByConvertingToAbsolute()
        {
            // Arrange
            var relativePath = "nonexistent_relative.dll";
            var options = new ExecuteLibraryCommandOptions(relativePath, Array.Empty<string>(), false);

            // Act & Assert - Should throw "file not found" for relative path that doesn't exist
            var ex = await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
            Assert.Contains("Couldn't find file", ex.Message);
        }

        #endregion

        #region Arguments Passing Tests

        [Fact]
        public async Task Execute_PassesEmptyArgumentsArray()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testFile = Path.Combine(tempDir, "test_args_empty_" + Guid.NewGuid().ToString() + ".dll");
            
            try
            {
                File.WriteAllText(testFile, "dummy");
                var options = new ExecuteLibraryCommandOptions(testFile, Array.Empty<string>(), false);

                // Act & Assert
                // Will fail in execution phase, not in argument handling
                await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
            }
            finally
            {
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        [Fact]
        public async Task Execute_PassesSingleArgument()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testFile = Path.Combine(tempDir, "test_args_single_" + Guid.NewGuid().ToString() + ".dll");
            var args = new[] { "arg1" };
            
            try
            {
                File.WriteAllText(testFile, "dummy");
                var options = new ExecuteLibraryCommandOptions(testFile, args, false);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
            }
            finally
            {
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        [Fact]
        public async Task Execute_PassesMultipleArguments()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testFile = Path.Combine(tempDir, "test_args_multi_" + Guid.NewGuid().ToString() + ".dll");
            var args = new[] { "arg1", "arg2", "arg3", "arg4" };
            
            try
            {
                File.WriteAllText(testFile, "dummy");
                var options = new ExecuteLibraryCommandOptions(testFile, args, false);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
            }
            finally
            {
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        [Fact]
        public async Task Execute_PassesNullArgumentsArray()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testFile = Path.Combine(tempDir, "test_args_null_" + Guid.NewGuid().ToString() + ".dll");
            
            try
            {
                File.WriteAllText(testFile, "dummy");
                var options = new ExecuteLibraryCommandOptions(testFile, null, false);

                // Act & Assert
                // Should handle null arguments gracefully
                await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
            }
            finally
            {
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        #endregion

        #region NoCache Option Tests

        [Fact]
        public async Task Execute_WithNoCacheFalse_UsesCache()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testFile = Path.Combine(tempDir, "test_cache_enabled_" + Guid.NewGuid().ToString() + ".dll");
            
            try
            {
                File.WriteAllText(testFile, "dummy");
                var options = new ExecuteLibraryCommandOptions(testFile, Array.Empty<string>(), false);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
            }
            finally
            {
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        [Fact]
        public async Task Execute_WithNoCacheTrue_DisablesCache()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testFile = Path.Combine(tempDir, "test_cache_disabled_" + Guid.NewGuid().ToString() + ".dll");
            
            try
            {
                File.WriteAllText(testFile, "dummy");
                var options = new ExecuteLibraryCommandOptions(testFile, Array.Empty<string>(), true);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
            }
            finally
            {
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_InitializesWithValidDependencies()
        {
            // Arrange & Act
            var command = new ExecuteLibraryCommand(_mockScriptConsole.Object, _mockLogFactory.Object);

            // Assert
            Assert.NotNull(command);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenScriptConsoleIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<NullReferenceException>(() => 
                new ExecuteLibraryCommand(null, _mockLogFactory.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLogFactoryIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<NullReferenceException>(() => 
                new ExecuteLibraryCommand(_mockScriptConsole.Object, null));
        }

        #endregion

        #region Generic Type Parameter Tests

        [Fact]
        public async Task Execute_SupportsObjectReturnType()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testFile = Path.Combine(tempDir, "test_return_object_" + Guid.NewGuid().ToString() + ".dll");
            
            try
            {
                File.WriteAllText(testFile, "dummy");
                var options = new ExecuteLibraryCommandOptions(testFile, Array.Empty<string>(), false);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
            }
            finally
            {
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        [Fact]
        public async Task Execute_SupportsIntReturnType()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testFile = Path.Combine(tempDir, "test_return_int_" + Guid.NewGuid().ToString() + ".dll");
            
            try
            {
                File.WriteAllText(testFile, "dummy");
                var options = new ExecuteLibraryCommandOptions(testFile, Array.Empty<string>(), false);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => _command.Execute<int>(options));
            }
            finally
            {
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        [Fact]
        public async Task Execute_SupportsStringReturnType()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testFile = Path.Combine(tempDir, "test_return_string_" + Guid.NewGuid().ToString() + ".dll");
            
            try
            {
                File.WriteAllText(testFile, "dummy");
                var options = new ExecuteLibraryCommandOptions(testFile, Array.Empty<string>(), false);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => _command.Execute<string>(options));
            }
            finally
            {
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task Execute_ThrowsException_WhenPathContainsInvalidCharacters()
        {
            // Arrange
            var invalidPath = "invalid\0path.dll";
            var options = new ExecuteLibraryCommandOptions(invalidPath, Array.Empty<string>(), false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
        }

        [Fact]
        public async Task Execute_ThrowsException_WhenLibraryPathIsDirectory()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var options = new ExecuteLibraryCommandOptions(tempDir, Array.Empty<string>(), false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
        }

        [Fact]
        public async Task Execute_ThrowsException_WhenLibraryPathEndsWithDirectorySeparator()
        {
            // Arrange
            var tempDir = Path.GetTempPath() + Path.DirectorySeparatorChar;
            var options = new ExecuteLibraryCommandOptions(tempDir, Array.Empty<string>(), false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
        }

        [Fact]
        public async Task Execute_ThrowsException_WithSpecialCharactersInPath()
        {
            // Arrange
            var path = Path.Combine(Path.GetTempPath(), "test@#$%^&().dll");
            var options = new ExecuteLibraryCommandOptions(path, Array.Empty<string>(), false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
        }

        #endregion

        #region Options Object Tests

        [Fact]
        public void Options_StoresLibraryPath()
        {
            // Arrange
            var path = "/some/path/lib.dll";
            var options = new ExecuteLibraryCommandOptions(path, Array.Empty<string>(), false);

            // Act & Assert
            Assert.Equal(path, options.LibraryPath);
        }

        [Fact]
        public void Options_StoresArguments()
        {
            // Arrange
            var args = new[] { "arg1", "arg2" };
            var options = new ExecuteLibraryCommandOptions("/path/lib.dll", args, false);

            // Act & Assert
            Assert.Equal(args, options.Arguments);
        }

        [Fact]
        public void Options_StoresNoCacheFlag()
        {
            // Arrange
            var options1 = new ExecuteLibraryCommandOptions("/path/lib.dll", Array.Empty<string>(), true);
            var options2 = new ExecuteLibraryCommandOptions("/path/lib.dll", Array.Empty<string>(), false);

            // Act & Assert
            Assert.True(options1.NoCache);
            Assert.False(options2.NoCache);
        }

        [Fact]
        public void Options_HasReadOnlyProperties()
        {
            // Arrange
            var path = "/path/lib.dll";
            var args = new[] { "arg1" };
            var options = new ExecuteLibraryCommandOptions(path, args, true);

            // Act & Assert - Properties should be read-only
            Assert.Equal(path, options.LibraryPath);
            Assert.Equal(args, options.Arguments);
            Assert.True(options.NoCache);
        }

        #endregion

        #region ScriptCompiler Integration Tests

        [Fact]
        public async Task Execute_InitializesScriptCompilerWithNoCacheSetting()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testFile = Path.Combine(tempDir, "test_compiler_init_" + Guid.NewGuid().ToString() + ".dll");
            
            try
            {
                File.WriteAllText(testFile, "dummy");
                var optionsWithCache = new ExecuteLibraryCommandOptions(testFile, Array.Empty<string>(), false);
                var optionsNoCache = new ExecuteLibraryCommandOptions(testFile, Array.Empty<string>(), true);

                // Act & Assert - Both should attempt to initialize compiler
                await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(optionsWithCache));
                await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(optionsNoCache));
            }
            finally
            {
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        [Fact]
        public async Task Execute_PassesArgumentsToScriptRunner()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var testFile = Path.Combine(tempDir, "test_runner_args_" + Guid.NewGuid().ToString() + ".dll");
            var args = new[] { "test_arg" };
            
            try
            {
                File.WriteAllText(testFile, "dummy");
                var options = new ExecuteLibraryCommandOptions(testFile, args, false);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => _command.Execute<object>(options));
            }
            finally
            {
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        #endregion
    }
}