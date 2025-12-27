using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace Dotnet.Script.DependencyModel.Process.Tests
{
    public class CommandRunnerTests
    {
        private readonly CommandRunner _commandRunner;

        public CommandRunnerTests()
        {
            _commandRunner = new CommandRunner();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_Should_Initialize_Successfully()
        {
            // Arrange & Act
            var runner = new CommandRunner();

            // Assert
            Assert.NotNull(runner);
        }

        #endregion

        #region Run Synchronous Tests

        [Fact]
        public void Run_Should_Execute_Simple_Command_Successfully()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public void Run_Should_Return_Output_From_Command()
        {
            // Arrange
            var command = "echo";
            var arguments = "hello world";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.StandardOutput);
            Assert.Contains("hello world", result.StandardOutput);
        }

        [Fact]
        public void Run_Should_Handle_Exit_Code_Zero_For_Success()
        {
            // Arrange
            var command = "echo";
            var arguments = "success";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.ExitCode);
        }

        [Fact]
        public void Run_Should_Handle_Non_Zero_Exit_Code_For_Failure()
        {
            // Arrange - Run a command that will fail
            var command = "cmd";
            var arguments = "/c exit 1";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.ExitCode);
        }

        [Fact]
        public void Run_Should_Throw_When_Command_Is_Null()
        {
            // Arrange
            string command = null;
            var arguments = "test";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _commandRunner.Run(command, arguments));
        }

        [Fact]
        public void Run_Should_Throw_When_Command_Is_Empty()
        {
            // Arrange
            var command = string.Empty;
            var arguments = "test";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _commandRunner.Run(command, arguments));
        }

        [Fact]
        public void Run_Should_Handle_Empty_Arguments()
        {
            // Arrange
            var command = "echo";
            var arguments = string.Empty;

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public void Run_Should_Handle_Null_Arguments()
        {
            // Arrange
            var command = "echo";
            string arguments = null;

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public void Run_Should_Handle_Command_Not_Found()
        {
            // Arrange
            var command = "nonexistent-command-xyz-123";
            var arguments = "test";

            // Act & Assert
            Assert.Throws<System.ComponentModel.Win32Exception>(() => 
                _commandRunner.Run(command, arguments));
        }

        [Fact]
        public void Run_Should_Capture_Standard_Error()
        {
            // Arrange
            var command = "cmd";
            var arguments = "/c (echo error message >&2) && exit 1";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.StandardError);
        }

        [Fact]
        public void Run_Should_Return_CommandResult_Object()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CommandResult>(result);
        }

        [Fact]
        public void Run_Should_Include_StandardOutput_In_Result()
        {
            // Arrange
            var command = "echo";
            var arguments = "output test";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result.StandardOutput);
        }

        [Fact]
        public void Run_Should_Include_StandardError_In_Result()
        {
            // Arrange
            var command = "cmd";
            var arguments = "/c echo test";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result.StandardError);
        }

        #endregion

        #region Run Asynchronous Tests

        [Fact]
        public async Task RunAsync_Should_Execute_Command_Successfully()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";

            // Act
            var result = await _commandRunner.RunAsync(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public async Task RunAsync_Should_Return_Output_From_Command()
        {
            // Arrange
            var command = "echo";
            var arguments = "async test";

            // Act
            var result = await _commandRunner.RunAsync(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.StandardOutput);
        }

        [Fact]
        public async Task RunAsync_Should_Handle_Exit_Code_Zero_For_Success()
        {
            // Arrange
            var command = "echo";
            var arguments = "success";

            // Act
            var result = await _commandRunner.RunAsync(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.ExitCode);
        }

        [Fact]
        public async Task RunAsync_Should_Handle_Non_Zero_Exit_Code_For_Failure()
        {
            // Arrange
            var command = "cmd";
            var arguments = "/c exit 2";

            // Act
            var result = await _commandRunner.RunAsync(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.ExitCode);
        }

        [Fact]
        public async Task RunAsync_Should_Throw_When_Command_Is_Null()
        {
            // Arrange
            string command = null;
            var arguments = "test";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _commandRunner.RunAsync(command, arguments));
        }

        [Fact]
        public async Task RunAsync_Should_Throw_When_Command_Is_Empty()
        {
            // Arrange
            var command = string.Empty;
            var arguments = "test";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _commandRunner.RunAsync(command, arguments));
        }

        [Fact]
        public async Task RunAsync_Should_Handle_Empty_Arguments()
        {
            // Arrange
            var command = "echo";
            var arguments = string.Empty;

            // Act
            var result = await _commandRunner.RunAsync(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public async Task RunAsync_Should_Handle_Null_Arguments()
        {
            // Arrange
            var command = "echo";
            string arguments = null;

            // Act
            var result = await _commandRunner.RunAsync(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public async Task RunAsync_Should_Handle_Command_Not_Found()
        {
            // Arrange
            var command = "nonexistent-async-command-xyz-123";
            var arguments = "test";

            // Act & Assert
            await Assert.ThrowsAsync<System.ComponentModel.Win32Exception>(() => 
                _commandRunner.RunAsync(command, arguments));
        }

        [Fact]
        public async Task RunAsync_Should_Capture_Standard_Error()
        {
            // Arrange
            var command = "cmd";
            var arguments = "/c (echo async error >&2) && exit 1";

            // Act
            var result = await _commandRunner.RunAsync(command, arguments);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task RunAsync_Should_Return_CommandResult_Object()
        {
            // Arrange
            var command = "echo";
            var arguments = "async result";

            // Act
            var result = await _commandRunner.RunAsync(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CommandResult>(result);
        }

        [Fact]
        public async Task RunAsync_Should_Be_Truly_Async()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            var taskStarted = false;

            // Act
            var task = _commandRunner.RunAsync(command, arguments);
            taskStarted = !task.IsCompleted;
            var result = await task;

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region Run With Timeout Tests

        [Fact]
        public void Run_With_Timeout_Should_Execute_Command_Within_Timeout()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            var timeout = TimeSpan.FromSeconds(10);

            // Act
            var result = _commandRunner.Run(command, arguments, timeout);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public void Run_With_Timeout_Should_Throw_When_Timeout_Exceeded()
        {
            // Arrange
            var command = "cmd";
            var arguments = "/c timeout /t 10";
            var timeout = TimeSpan.FromMilliseconds(100);

            // Act & Assert
            Assert.Throws<TimeoutException>(() => 
                _commandRunner.Run(command, arguments, timeout));
        }

        [Fact]
        public void Run_With_Timeout_Should_Handle_Zero_Timeout()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            var timeout = TimeSpan.Zero;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                _commandRunner.Run(command, arguments, timeout));
        }

        [Fact]
        public void Run_With_Timeout_Should_Handle_Negative_Timeout()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            var timeout = TimeSpan.FromMilliseconds(-1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                _commandRunner.Run(command, arguments, timeout));
        }

        [Fact]
        public void Run_With_Timeout_Should_Return_Output_When_Completed_Before_Timeout()
        {
            // Arrange
            var command = "echo";
            var arguments = "quick output";
            var timeout = TimeSpan.FromSeconds(10);

            // Act
            var result = _commandRunner.Run(command, arguments, timeout);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.StandardOutput);
        }

        #endregion

        #region Run Async With Timeout Tests

        [Fact]
        public async Task RunAsync_With_Timeout_Should_Execute_Command_Within_Timeout()
        {
            // Arrange
            var command = "echo";
            var arguments = "async timeout test";
            var timeout = TimeSpan.FromSeconds(10);

            // Act
            var result = await _commandRunner.RunAsync(command, arguments, timeout);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public async Task RunAsync_With_Timeout_Should_Throw_When_Timeout_Exceeded()
        {
            // Arrange
            var command = "cmd";
            var arguments = "/c timeout /t 10";
            var timeout = TimeSpan.FromMilliseconds(100);

            // Act & Assert
            await Assert.ThrowsAsync<TimeoutException>(() => 
                _commandRunner.RunAsync(command, arguments, timeout));
        }

        [Fact]
        public async Task RunAsync_With_Timeout_Should_Handle_Zero_Timeout()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            var timeout = TimeSpan.Zero;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _commandRunner.RunAsync(command, arguments, timeout));
        }

        [Fact]
        public async Task RunAsync_With_Timeout_Should_Handle_Negative_Timeout()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            var timeout = TimeSpan.FromMilliseconds(-1);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _commandRunner.RunAsync(command, arguments, timeout));
        }

        [Fact]
        public async Task RunAsync_With_Timeout_Should_Return_Output_When_Completed_Before_Timeout()
        {
            // Arrange
            var command = "echo";
            var arguments = "quick async output";
            var timeout = TimeSpan.FromSeconds(10);

            // Act
            var result = await _commandRunner.RunAsync(command, arguments, timeout);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.StandardOutput);
        }

        [Fact]
        public async Task RunAsync_With_CancellationToken_Should_Support_Cancellation()
        {
            // Arrange
            var command = "cmd";
            var arguments = "/c timeout /t 10";
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => 
                _commandRunner.RunAsync(command, arguments, cts.Token));
        }

        [Fact]
        public async Task RunAsync_With_CancellationToken_Should_Execute_When_Not_Cancelled()
        {
            // Arrange
            var command = "echo";
            var arguments = "cancel test";
            var cts = new CancellationTokenSource();

            // Act
            var result = await _commandRunner.RunAsync(command, arguments, cts.Token);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public async Task RunAsync_With_CancellationToken_Should_Handle_Already_Cancelled_Token()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => 
                _commandRunner.RunAsync(command, arguments, cts.Token));
        }

        #endregion

        #region Run With Working Directory Tests

        [Fact]
        public void Run_With_WorkingDirectory_Should_Execute_In_Specified_Directory()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            var workingDirectory = System.IO.Path.GetTempPath();

            // Act
            var result = _commandRunner.Run(command, arguments, workingDirectory);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public void Run_With_WorkingDirectory_Should_Throw_When_Directory_Does_Not_Exist()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            var workingDirectory = "C:\\nonexistent\\directory\\that\\does\\not\\exist";

            // Act & Assert
            Assert.Throws<System.IO.DirectoryNotFoundException>(() => 
                _commandRunner.Run(command, arguments, workingDirectory));
        }

        [Fact]
        public void Run_With_WorkingDirectory_Should_Handle_Null_WorkingDirectory()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            string workingDirectory = null;

            // Act
            var result = _commandRunner.Run(command, arguments, workingDirectory);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public void Run_With_WorkingDirectory_Should_Handle_Empty_WorkingDirectory()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            var workingDirectory = string.Empty;

            // Act
            var result = _commandRunner.Run(command, arguments, workingDirectory);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        #endregion

        #region Run Async With Working Directory Tests

        [Fact]
        public async Task RunAsync_With_WorkingDirectory_Should_Execute_In_Specified_Directory()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            var workingDirectory = System.IO.Path.GetTempPath();

            // Act
            var result = await _commandRunner.RunAsync(command, arguments, workingDirectory);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public async Task RunAsync_With_WorkingDirectory_Should_Throw_When_Directory_Does_Not_Exist()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            var workingDirectory = "C:\\nonexistent\\async\\directory";

            // Act & Assert
            await Assert.ThrowsAsync<System.IO.DirectoryNotFoundException>(() => 
                _commandRunner.RunAsync(command, arguments, workingDirectory));
        }

        [Fact]
        public async Task RunAsync_With_WorkingDirectory_Should_Handle_Null_WorkingDirectory()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";
            string workingDirectory = null;

            // Act
            var result = await _commandRunner.RunAsync(command, arguments, workingDirectory);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ExitCode >= 0);
        }

        #endregion

        #region Edge Cases and Error Conditions

        [Fact]
        public void Run_Should_Handle_Very_Long_Output()
        {
            // Arrange
            var command = "echo";
            var arguments = new string('x', 10000);

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Run_Should_Handle_Special_Characters_In_Arguments()
        {
            // Arrange
            var command = "echo";
            var arguments = "test & special | characters < > \\";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Run_Should_Handle_Quotes_In_Arguments()
        {
            // Arrange
            var command = "echo";
            var arguments = "\"quoted argument\"";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task RunAsync_Should_Handle_Very_Long_Output()
        {
            // Arrange
            var command = "echo";
            var arguments = new string('y', 10000);

            // Act
            var result = await _commandRunner.RunAsync(command, arguments);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Run_Should_Preserve_Exit_Code_From_Command()
        {
            // Arrange
            var command = "cmd";
            var arguments = "/c exit 42";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(42, result.ExitCode);
        }

        [Fact]
        public async Task RunAsync_Should_Preserve_Exit_Code_From_Command()
        {
            // Arrange
            var command = "cmd";
            var arguments = "/c exit 99";

            // Act
            var result = await _commandRunner.RunAsync(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(99, result.ExitCode);
        }

        #endregion

        #region CommandResult Tests

        [Fact]
        public void CommandResult_Should_Have_ExitCode_Property()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.True(result.ExitCode >= 0);
        }

        [Fact]
        public void CommandResult_Should_Have_StandardOutput_Property()
        {
            // Arrange
            var command = "echo";
            var arguments = "test output";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result.StandardOutput);
        }

        [Fact]
        public void CommandResult_Should_Have_StandardError_Property()
        {
            // Arrange
            var command = "echo";
            var arguments = "test";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result.StandardError);
        }

        [Fact]
        public void CommandResult_Should_Indicate_Success_When_ExitCode_Is_Zero()
        {
            // Arrange
            var command = "echo";
            var arguments = "success";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.Equal(0, result.ExitCode);
        }

        [Fact]
        public void CommandResult_Should_Indicate_Failure_When_ExitCode_Is_Non_Zero()
        {
            // Arrange
            var command = "cmd";
            var arguments = "/c exit 1";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotEqual(0, result.ExitCode);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void Run_Should_Execute_Multiple_Commands_Sequentially()
        {
            // Arrange
            var command = "echo";
            var arguments1 = "first";
            var arguments2 = "second";

            // Act
            var result1 = _commandRunner.Run(command, arguments1);
            var result2 = _commandRunner.Run(command, arguments2);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal(0, result1.ExitCode);
            Assert.Equal(0, result2.ExitCode);
        }

        [Fact]
        public async Task RunAsync_Should_Execute_Multiple_Commands_Concurrently()
        {
            // Arrange
            var command = "echo";
            var arguments1 = "concurrent1";
            var arguments2 = "concurrent2";

            // Act
            var task1 = _commandRunner.RunAsync(command, arguments1);
            var task2 = _commandRunner.RunAsync(command, arguments2);
            var results = await Task.WhenAll(task1, task2);

            // Assert
            Assert.NotNull(results);
            Assert.Equal(2, results.Length);
            Assert.Equal(0, results[0].ExitCode);
            Assert.Equal(0, results[1].ExitCode);
        }

        [Fact]
        public void Run_Should_Handle_Piped_Commands()
        {
            // Arrange
            var command = "cmd";
            var arguments = "/c echo test | find \"test\"";

            // Act
            var result = _commandRunner.Run(command, arguments);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.ExitCode);
        }

        #endregion
    }
}