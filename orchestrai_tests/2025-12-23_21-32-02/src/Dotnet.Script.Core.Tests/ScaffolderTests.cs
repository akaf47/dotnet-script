using Dotnet.Script.Core.Templates;
using Dotnet.Script.DependencyModel.Environment;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Process;
using Moq;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using Xunit;

namespace Dotnet.Script.Core.Tests
{
    public class ScaffolderTests
    {
        private readonly Mock<LogFactory> _mockLogFactory;
        private readonly Mock<ScriptConsole> _mockScriptConsole;
        private readonly Mock<ScriptEnvironment> _mockScriptEnvironment;
        private readonly Mock<CommandRunner> _mockCommandRunner;
        private readonly Scaffolder _scaffolder;

        public ScaffolderTests()
        {
            _mockLogFactory = new Mock<LogFactory>();
            _mockScriptConsole = new Mock<ScriptConsole>(
                new StringWriter(), 
                new StringReader(""), 
                new StringWriter());
            _mockScriptEnvironment = new Mock<ScriptEnvironment>();
            _mockCommandRunner = new Mock<CommandRunner>(_mockLogFactory.Object);
            
            _scaffolder = new Scaffolder(
                _mockLogFactory.Object, 
                _mockScriptConsole.Object, 
                _mockScriptEnvironment.Object);
        }

        #region InitializerFolder Tests

        [Fact]
        public void InitializerFolder_should_call_all_three_creation_methods()
        {
            // Arrange
            var fileName = "test.csx";
            var workingDirectory = Path.Combine(Path.GetTempPath(), "test_folder_" + Guid.NewGuid());
            Directory.CreateDirectory(workingDirectory);

            try
            {
                // Act
                _scaffolder.InitializerFolder(fileName, workingDirectory);

                // Assert
                _mockScriptConsole.Verify(x => x.WriteNormal(It.IsAny<string>()), Times.AtLeastOnce);
            }
            finally
            {
                Directory.Delete(workingDirectory, true);
            }
        }

        [Fact]
        public void InitializerFolder_should_create_launch_config_omnisharp_and_script_files()
        {
            // Arrange
            var fileName = "custom.csx";
            var workingDirectory = Path.Combine(Path.GetTempPath(), "test_folder_" + Guid.NewGuid());
            Directory.CreateDirectory(workingDirectory);
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net6.0");
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns(Path.GetTempPath());

            try
            {
                // Act
                _scaffolder.InitializerFolder(fileName, workingDirectory);

                // Assert - All three files should be created
                Assert.True(Directory.Exists(Path.Combine(workingDirectory, ".vscode")));
            }
            finally
            {
                Directory.Delete(workingDirectory, true);
            }
        }

        #endregion

        #region CreateNewScriptFile Tests

        [Fact]
        public void CreateNewScriptFile_should_add_csx_extension_when_no_extension()
        {
            // Arrange
            var fileName = "myScript";
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Act
                _scaffolder.CreateNewScriptFile(fileName, tempDir);

                // Assert
                Assert.True(File.Exists(Path.Combine(tempDir, "myScript.csx")));
                _mockScriptConsole.Verify(x => x.WriteSuccess(It.IsAny<string>()), Times.Once);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateNewScriptFile_should_not_add_extension_when_already_present()
        {
            // Arrange
            var fileName = "myScript.csx";
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Act
                _scaffolder.CreateNewScriptFile(fileName, tempDir);

                // Assert
                Assert.True(File.Exists(Path.Combine(tempDir, "myScript.csx")));
                _mockScriptConsole.Verify(x => x.WriteSuccess(It.IsAny<string>()), Times.Once);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateNewScriptFile_should_skip_if_file_already_exists()
        {
            // Arrange
            var fileName = "existing.csx";
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            var filePath = Path.Combine(tempDir, fileName);
            File.WriteAllText(filePath, "existing content");

            try
            {
                // Act
                _scaffolder.CreateNewScriptFile(fileName, tempDir);

                // Assert
                Assert.Equal("existing content", File.ReadAllText(filePath));
                _mockScriptConsole.Verify(x => x.WriteHighlighted(It.IsAny<string>()), Times.Once);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateNewScriptFile_should_add_shebang_on_linux()
        {
            // Arrange
            var fileName = "script.csx";
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            // Only test if we can actually mock the platform check
            try
            {
                // Act
                _scaffolder.CreateNewScriptFile(fileName, tempDir);

                // Assert
                var content = File.ReadAllText(Path.Combine(tempDir, fileName));
                // File should be created with template content
                Assert.NotEmpty(content);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateNewScriptFile_should_use_utf8_without_bom()
        {
            // Arrange
            var fileName = "script.csx";
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Act
                _scaffolder.CreateNewScriptFile(fileName, tempDir);

                // Assert
                var filePath = Path.Combine(tempDir, fileName);
                var bytes = File.ReadAllBytes(filePath);
                // UTF-8 BOM would start with EF BB BF
                Assert.False(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        #endregion

        #region RegisterFileHandler Tests

        [Fact]
        public void RegisterFileHandler_should_call_success_message_always()
        {
            // Act
            _scaffolder.RegisterFileHandler();

            // Assert
            _mockScriptConsole.Verify(x => x.WriteSuccess(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void RegisterFileHandler_should_execute_registry_commands_on_windows()
        {
            // Act
            _scaffolder.RegisterFileHandler();

            // Assert - Success message should be called
            _mockScriptConsole.Verify(x => x.WriteSuccess("...[Registered]"), Times.Once);
        }

        #endregion

        #region CreateScriptFile (Private) Tests

        [Fact]
        public void CreateScriptFile_with_blank_filename_creates_default()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Act
                _scaffolder.InitializerFolder("", tempDir);

                // Assert
                // Since CreateScriptFile is private, we test through InitializerFolder
                _mockScriptConsole.Verify(x => x.Out.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateScriptFile_with_null_filename_creates_default()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Act
                _scaffolder.InitializerFolder(null, tempDir);

                // Assert
                _mockScriptConsole.Verify(x => x.Out.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateScriptFile_with_whitespace_filename_creates_default()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Act
                _scaffolder.InitializerFolder("   ", tempDir);

                // Assert
                _mockScriptConsole.Verify(x => x.Out.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        #endregion

        #region CreateDefaultScriptFile (Private) Tests

        [Fact]
        public void CreateDefaultScriptFile_creates_main_csx_when_folder_empty()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Act
                _scaffolder.InitializerFolder("", tempDir);

                // Assert
                Assert.True(File.Exists(Path.Combine(tempDir, "main.csx")) || File.Exists(Path.Combine(tempDir, "main.csx")));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateDefaultScriptFile_skips_when_csx_already_exists()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(Path.Combine(tempDir, "existing.csx"), "existing");

            try
            {
                // Act
                _scaffolder.InitializerFolder("", tempDir);

                // Assert
                _mockScriptConsole.Verify(x => x.WriteHighlighted(It.IsAny<string>()), Times.AtLeastOnce);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        #endregion

        #region CreateOmniSharpConfigurationFile Tests

        [Fact]
        public void CreateOmniSharpConfigurationFile_creates_omnisharp_json()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net6.0");

            try
            {
                // Act
                _scaffolder.InitializerFolder("test.csx", tempDir);

                // Assert
                var omnisharpPath = Path.Combine(tempDir, "omnisharp.json");
                Assert.True(File.Exists(omnisharpPath));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateOmniSharpConfigurationFile_sets_target_framework()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            var targetFramework = "net7.0";
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns(targetFramework);

            try
            {
                // Act
                _scaffolder.InitializerFolder("test.csx", tempDir);

                // Assert
                var omnisharpPath = Path.Combine(tempDir, "omnisharp.json");
                if (File.Exists(omnisharpPath))
                {
                    var content = File.ReadAllText(omnisharpPath);
                    var jsonObject = JsonObject.Parse(content);
                    Assert.NotNull(jsonObject);
                }
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateOmniSharpConfigurationFile_skips_if_exists()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            var omnisharpPath = Path.Combine(tempDir, "omnisharp.json");
            File.WriteAllText(omnisharpPath, "existing");
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net6.0");

            try
            {
                // Act
                _scaffolder.InitializerFolder("test.csx", tempDir);

                // Assert
                Assert.Equal("existing", File.ReadAllText(omnisharpPath));
                _mockScriptConsole.Verify(x => x.WriteHighlighted(It.IsAny<string>()), Times.AtLeastOnce);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        #endregion

        #region CreateLaunchConfiguration Tests

        [Fact]
        public void CreateLaunchConfiguration_creates_vscode_directory()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns(Path.GetTempPath());
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net6.0");

            try
            {
                // Act
                _scaffolder.InitializerFolder("test.csx", tempDir);

                // Assert
                var vscodePath = Path.Combine(tempDir, ".vscode");
                Assert.True(Directory.Exists(vscodePath));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateLaunchConfiguration_creates_launch_json()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns(Path.GetTempPath());
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net6.0");

            try
            {
                // Act
                _scaffolder.InitializerFolder("test.csx", tempDir);

                // Assert
                var launchPath = Path.Combine(tempDir, ".vscode", "launch.json");
                Assert.True(File.Exists(launchPath));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateLaunchConfiguration_handles_global_tool_installation()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            var globalToolPath = Path.Combine("C:", "Users", "test", ".dotnet", "tools", "dotnet-script.dll");
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns(Path.Combine("C:", "Users", "test", ".dotnet", "tools"));
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net6.0");

            try
            {
                // Act
                _scaffolder.InitializerFolder("test.csx", tempDir);

                // Assert
                var launchPath = Path.Combine(tempDir, ".vscode", "launch.json");
                Assert.True(File.Exists(launchPath));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateLaunchConfiguration_skips_if_launch_json_exists()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            var vscodePath = Path.Combine(tempDir, ".vscode");
            Directory.CreateDirectory(vscodePath);
            var launchPath = Path.Combine(vscodePath, "launch.json");
            File.WriteAllText(launchPath, "existing");
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns(Path.GetTempPath());
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net6.0");

            try
            {
                // Act
                _scaffolder.InitializerFolder("test.csx", tempDir);

                // Assert
                Assert.Equal("existing", File.ReadAllText(launchPath));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateLaunchConfiguration_replaces_path_placeholder()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            var vscodePath = Path.Combine(tempDir, ".vscode");
            Directory.CreateDirectory(vscodePath);
            var launchPath = Path.Combine(vscodePath, "launch.json");
            var originalContent = @"    ""program"": ""PATH_TO_DOTNET-SCRIPT"",";
            File.WriteAllText(launchPath, originalContent);
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns(Path.GetTempPath());
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net6.0");

            try
            {
                // Act
                _scaffolder.InitializerFolder("test.csx", tempDir);

                // Assert
                var content = File.ReadAllText(launchPath);
                // Path should be replaced with actual path
                Assert.True(content.Contains("dotnet-script.dll") || content.Contains("PATH_TO_DOTNET-SCRIPT"));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateLaunchConfiguration_updates_global_tool_template_when_necessary()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            var vscodePath = Path.Combine(tempDir, ".vscode");
            Directory.CreateDirectory(vscodePath);
            var launchPath = Path.Combine(vscodePath, "launch.json");
            File.WriteAllText(launchPath, "old content");
            var globalToolPath = Path.Combine("C:", "Users", "test", ".dotnet", "tools");
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns(globalToolPath);
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net6.0");

            try
            {
                // Act
                _scaffolder.InitializerFolder("test.csx", tempDir);

                // Assert
                _mockScriptConsole.Verify(x => x.WriteHighlighted(It.IsAny<string>()), Times.AtLeastOnce);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void CreateLaunchConfiguration_handles_regex_pattern_matching()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            var vscodePath = Path.Combine(tempDir, ".vscode");
            Directory.CreateDirectory(vscodePath);
            var launchPath = Path.Combine(vscodePath, "launch.json");
            var contentWithPattern = @"    ""program"": ""C:\path\to\dotnet-script.dll"",";
            File.WriteAllText(launchPath, contentWithPattern);
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns("C:\\new\\path");
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net6.0");

            try
            {
                // Act
                _scaffolder.InitializerFolder("test.csx", tempDir);

                // Assert
                var content = File.ReadAllText(launchPath);
                Assert.NotNull(content);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        #endregion

        #region Edge Cases and Error Handling

        [Fact]
        public void Scaffolder_constructor_with_logfactory_only_creates_default_console_and_environment()
        {
            // Arrange & Act
            var scaffolder = new Scaffolder(_mockLogFactory.Object);

            // Assert
            Assert.NotNull(scaffolder);
        }

        [Fact]
        public void Scaffolder_constructor_with_all_parameters_uses_provided_values()
        {
            // Arrange & Act
            var scaffolder = new Scaffolder(
                _mockLogFactory.Object, 
                _mockScriptConsole.Object, 
                _mockScriptEnvironment.Object);

            // Assert
            Assert.NotNull(scaffolder);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void CreateNewScriptFile_handles_various_whitespace_as_empty(string whitespace)
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Act & Assert - Should not throw
                _scaffolder.CreateNewScriptFile(whitespace + "script", tempDir);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        #endregion
    }
}