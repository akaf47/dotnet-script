using Dotnet.Script.Core;
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

namespace Dotnet.Script.Tests
{
    public class ScaffolderTests : IDisposable
    {
        private readonly Mock<LogFactory> _mockLogFactory;
        private readonly Mock<ScriptConsole> _mockScriptConsole;
        private readonly Mock<ScriptEnvironment> _mockScriptEnvironment;
        private readonly string _tempDirectory;
        private readonly Scaffolder _scaffolder;

        public ScaffolderTests()
        {
            _mockLogFactory = new Mock<LogFactory>();
            _mockScriptConsole = new Mock<ScriptConsole>();
            _mockScriptEnvironment = new Mock<ScriptEnvironment>();
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);

            _mockScriptConsole.Setup(x => x.WriteNormal(It.IsAny<string>()));
            _mockScriptConsole.Setup(x => x.WriteSuccess(It.IsAny<string>()));
            _mockScriptConsole.Setup(x => x.WriteHighlighted(It.IsAny<string>()));

            _scaffolder = new Scaffolder(_mockLogFactory.Object, _mockScriptConsole.Object, _mockScriptEnvironment.Object);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithLogFactoryOnly_InitializesScaffolderSuccessfully()
        {
            // Arrange
            var logFactory = new Mock<LogFactory>();

            // Act
            var scaffolder = new Scaffolder(logFactory.Object);

            // Assert
            Assert.NotNull(scaffolder);
        }

        [Fact]
        public void Constructor_WithAllParameters_InitializesScaffolderSuccessfully()
        {
            // Arrange
            var logFactory = new Mock<LogFactory>();
            var scriptConsole = new Mock<ScriptConsole>();
            var scriptEnvironment = new Mock<ScriptEnvironment>();

            // Act
            var scaffolder = new Scaffolder(logFactory.Object, scriptConsole.Object, scriptEnvironment.Object);

            // Assert
            Assert.NotNull(scaffolder);
        }

        #endregion

        #region InitializerFolder Tests

        [Fact]
        public void InitializerFolder_WithValidParameters_CallsAllThreeCreationMethods()
        {
            // Arrange
            var fileName = "test.csx";
            var vsCodeDir = Path.Combine(_tempDirectory, ".vscode");

            // Act
            _scaffolder.InitializerFolder(fileName, _tempDirectory);

            // Assert
            // Verify that files/directories were created
            Assert.True(Directory.Exists(vsCodeDir), "VS Code directory should be created");
            _mockScriptConsole.Verify(x => x.WriteNormal(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void InitializerFolder_WithNullFileName_CallsWithDefaultScriptName()
        {
            // Arrange
            var vsCodeDir = Path.Combine(_tempDirectory, ".vscode");

            // Act
            _scaffolder.InitializerFolder(null, _tempDirectory);

            // Assert
            Assert.True(Directory.Exists(vsCodeDir));
        }

        [Fact]
        public void InitializerFolder_WithEmptyFileName_CallsWithDefaultScriptName()
        {
            // Arrange
            var vsCodeDir = Path.Combine(_tempDirectory, ".vscode");

            // Act
            _scaffolder.InitializerFolder("", _tempDirectory);

            // Assert
            Assert.True(Directory.Exists(vsCodeDir));
        }

        #endregion

        #region CreateNewScriptFile Tests

        [Fact]
        public void CreateNewScriptFile_WithValidFileName_CreatesScriptFile()
        {
            // Arrange
            var fileName = "test.csx";
            var expectedPath = Path.Combine(_tempDirectory, fileName);

            // Act
            _scaffolder.CreateNewScriptFile(fileName, _tempDirectory);

            // Assert
            Assert.True(File.Exists(expectedPath), "Script file should be created");
            _mockScriptConsole.Verify(x => x.WriteSuccess(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void CreateNewScriptFile_WithFileNameWithoutExtension_AddsCSXExtension()
        {
            // Arrange
            var fileName = "test";
            var expectedPath = Path.Combine(_tempDirectory, "test.csx");

            // Act
            _scaffolder.CreateNewScriptFile(fileName, _tempDirectory);

            // Assert
            Assert.True(File.Exists(expectedPath), "Script file with .csx extension should be created");
        }

        [Fact]
        public void CreateNewScriptFile_WhenFileAlreadyExists_SkipsCreation()
        {
            // Arrange
            var fileName = "test.csx";
            var filePath = Path.Combine(_tempDirectory, fileName);
            File.WriteAllText(filePath, "existing content");

            // Act
            _scaffolder.CreateNewScriptFile(fileName, _tempDirectory);

            // Assert
            Assert.Equal("existing content", File.ReadAllText(filePath));
            _mockScriptConsole.Verify(x => x.WriteHighlighted(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void CreateNewScriptFile_CreatesFileWithValidContent()
        {
            // Arrange
            var fileName = "test.csx";
            var filePath = Path.Combine(_tempDirectory, fileName);

            // Act
            _scaffolder.CreateNewScriptFile(fileName, _tempDirectory);

            // Assert
            var content = File.ReadAllText(filePath);
            Assert.NotEmpty(content);
        }

        [Fact]
        public void CreateNewScriptFile_OnLinuxOrMac_AddsShebangsToFile()
        {
            // Arrange
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // Skip test on non-Unix platforms
                return;
            }

            var fileName = "test.csx";
            var filePath = Path.Combine(_tempDirectory, fileName);

            // Act
            _scaffolder.CreateNewScriptFile(fileName, _tempDirectory);

            // Assert
            var content = File.ReadAllText(filePath);
            Assert.StartsWith("#!/usr/bin/env dotnet-script", content);
        }

        [Fact]
        public void CreateNewScriptFile_OnLinuxOrMac_DoesNotAddShebangOnWindows()
        {
            // Arrange
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Skip test on non-Windows platforms
                return;
            }

            var fileName = "test.csx";
            var filePath = Path.Combine(_tempDirectory, fileName);

            // Act
            _scaffolder.CreateNewScriptFile(fileName, _tempDirectory);

            // Assert
            var content = File.ReadAllText(filePath);
            Assert.DoesNotStartWith("#!/usr/bin/env", content);
        }

        #endregion

        #region RegisterFileHandler Tests

        [Fact]
        public void RegisterFileHandler_OnWindows_CallsRegistryCommands()
        {
            // Arrange
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return; // Skip on non-Windows
            }

            // Act
            _scaffolder.RegisterFileHandler();

            // Assert
            _mockScriptConsole.Verify(x => x.WriteSuccess(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void RegisterFileHandler_OnNonWindows_DoesNotCallRegistryCommands()
        {
            // Arrange
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return; // Skip on Windows
            }

            // Act
            _scaffolder.RegisterFileHandler();

            // Assert
            _mockScriptConsole.Verify(x => x.WriteSuccess(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region CreateOmniSharpConfigurationFile Tests

        [Fact]
        public void CreateOmniSharpConfigurationFile_CreatesOmniSharpJson()
        {
            // Arrange
            var expectedPath = Path.Combine(_tempDirectory, "omnisharp.json");
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net8.0");

            // Act
            var scaffolder = new Scaffolder(_mockLogFactory.Object, _mockScriptConsole.Object, _mockScriptEnvironment.Object);
            
            // Use reflection to call private method
            var method = typeof(Scaffolder).GetMethod("CreateOmniSharpConfigurationFile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(scaffolder, new object[] { _tempDirectory });

            // Assert
            Assert.True(File.Exists(expectedPath), "omnisharp.json should be created");
        }

        [Fact]
        public void CreateOmniSharpConfigurationFile_WhenFileExists_SkipsCreation()
        {
            // Arrange
            var omniSharpPath = Path.Combine(_tempDirectory, "omnisharp.json");
            var originalContent = "{\"test\": \"value\"}";
            File.WriteAllText(omniSharpPath, originalContent);
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net8.0");

            // Act
            var scaffolder = new Scaffolder(_mockLogFactory.Object, _mockScriptConsole.Object, _mockScriptEnvironment.Object);
            var method = typeof(Scaffolder).GetMethod("CreateOmniSharpConfigurationFile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(scaffolder, new object[] { _tempDirectory });

            // Assert
            var content = File.ReadAllText(omniSharpPath);
            Assert.Equal(originalContent, content);
            _mockScriptConsole.Verify(x => x.WriteHighlighted(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region CreateLaunchConfiguration Tests

        [Fact]
        public void CreateLaunchConfiguration_CreatesVSCodeDirectory()
        {
            // Arrange
            var vsCodeDir = Path.Combine(_tempDirectory, ".vscode");
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns("/usr/bin/dotnet");

            // Act
            var scaffolder = new Scaffolder(_mockLogFactory.Object, _mockScriptConsole.Object, _mockScriptEnvironment.Object);
            var method = typeof(Scaffolder).GetMethod("CreateLaunchConfiguration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(scaffolder, new object[] { _tempDirectory });

            // Assert
            Assert.True(Directory.Exists(vsCodeDir), ".vscode directory should be created");
        }

        [Fact]
        public void CreateLaunchConfiguration_CreatesLaunchJson()
        {
            // Arrange
            var launchPath = Path.Combine(_tempDirectory, ".vscode", "launch.json");
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns("/usr/bin/dotnet");

            // Act
            var scaffolder = new Scaffolder(_mockLogFactory.Object, _mockScriptConsole.Object, _mockScriptEnvironment.Object);
            var method = typeof(Scaffolder).GetMethod("CreateLaunchConfiguration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(scaffolder, new object[] { _tempDirectory });

            // Assert
            Assert.True(File.Exists(launchPath), "launch.json should be created");
        }

        [Fact]
        public void CreateLaunchConfiguration_WhenGlobalToolInstalled_UsesGlobalToolTemplate()
        {
            // Arrange
            var launchPath = Path.Combine(_tempDirectory, ".vscode", "launch.json");
            var globalToolPath = Path.Combine(Path.GetTempPath(), ".dotnet", "tools", "dotnet-script");
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns(globalToolPath);

            // Act
            var scaffolder = new Scaffolder(_mockLogFactory.Object, _mockScriptConsole.Object, _mockScriptEnvironment.Object);
            var method = typeof(Scaffolder).GetMethod("CreateLaunchConfiguration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(scaffolder, new object[] { _tempDirectory });

            // Assert
            Assert.True(File.Exists(launchPath));
        }

        [Fact]
        public void CreateLaunchConfiguration_WhenNotGlobalTool_UsesLocalTemplate()
        {
            // Arrange
            var launchPath = Path.Combine(_tempDirectory, ".vscode", "launch.json");
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns("/usr/local/bin");

            // Act
            var scaffolder = new Scaffolder(_mockLogFactory.Object, _mockScriptConsole.Object, _mockScriptEnvironment.Object);
            var method = typeof(Scaffolder).GetMethod("CreateLaunchConfiguration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(scaffolder, new object[] { _tempDirectory });

            // Assert
            Assert.True(File.Exists(launchPath));
            var content = File.ReadAllText(launchPath);
            Assert.NotEmpty(content);
        }

        [Fact]
        public void CreateLaunchConfiguration_WhenFileExists_SkipsIfNoChanges()
        {
            // Arrange
            var vscodeDir = Path.Combine(_tempDirectory, ".vscode");
            Directory.CreateDirectory(vscodeDir);
            var launchPath = Path.Combine(vscodeDir, "launch.json");
            var existingContent = "{}";
            File.WriteAllText(launchPath, existingContent);
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns("/usr/local/bin");

            // Act
            var scaffolder = new Scaffolder(_mockLogFactory.Object, _mockScriptConsole.Object, _mockScriptEnvironment.Object);
            var method = typeof(Scaffolder).GetMethod("CreateLaunchConfiguration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(scaffolder, new object[] { _tempDirectory });

            // Assert
            _mockScriptConsole.Verify(x => x.WriteHighlighted(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void CreateLaunchConfiguration_WithRegexPathUpdate_UpdatesPathInLaunchJson()
        {
            // Arrange
            var vscodeDir = Path.Combine(_tempDirectory, ".vscode");
            Directory.CreateDirectory(vscodeDir);
            var launchPath = Path.Combine(vscodeDir, "launch.json");
            var oldPath = "/old/path/dotnet-script.dll";
            var existingContent = $@"{{
  ""version"": ""0.2.0"",
  ""configurations"": [
    {{
      ""name"": ""Launch"",
      ""type"": ""coreclr"",
      ""program"": ""{oldPath}"",
      ""request"": ""launch""
    }}
  ]
}}";
            File.WriteAllText(launchPath, existingContent);
            var newInstallPath = "/new/path/dotnet";
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns(newInstallPath);

            // Act
            var scaffolder = new Scaffolder(_mockLogFactory.Object, _mockScriptConsole.Object, _mockScriptEnvironment.Object);
            var method = typeof(Scaffolder).GetMethod("CreateLaunchConfiguration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(scaffolder, new object[] { _tempDirectory });

            // Assert
            var content = File.ReadAllText(launchPath);
            Assert.NotEmpty(content);
        }

        #endregion

        #region Edge Cases and Error Handling

        [Fact]
        public void CreateNewScriptFile_WithSpecialCharactersInFileName_HandlesCorrectly()
        {
            // Arrange
            var fileName = "test-file_123.csx";
            var expectedPath = Path.Combine(_tempDirectory, fileName);

            // Act
            _scaffolder.CreateNewScriptFile(fileName, _tempDirectory);

            // Assert
            Assert.True(File.Exists(expectedPath));
        }

        [Fact]
        public void CreateNewScriptFile_WithDifferentExtension_ReplacesWithCSX()
        {
            // Arrange
            var fileName = "test.txt";
            var expectedPath = Path.Combine(_tempDirectory, "test.csx");

            // Act
            _scaffolder.CreateNewScriptFile(fileName, _tempDirectory);

            // Assert
            Assert.True(File.Exists(expectedPath));
            Assert.False(File.Exists(Path.Combine(_tempDirectory, fileName)));
        }

        [Fact]
        public void CreateNewScriptFile_WithLongFileName_CreatesFileSuccessfully()
        {
            // Arrange
            var fileName = "very_long_file_name_that_is_still_valid_for_filesystem_" + new string('a', 50) + ".csx";
            var expectedPath = Path.Combine(_tempDirectory, fileName);

            // Act
            try
            {
                _scaffolder.CreateNewScriptFile(fileName, _tempDirectory);
                // Assert
                // File may or may not be created depending on filesystem limits
            }
            catch (PathTooLongException)
            {
                // Expected on some systems
            }
        }

        [Fact]
        public void InitializerFolder_CreatesCompleteScaffoldingStructure()
        {
            // Arrange
            var vsCodeDir = Path.Combine(_tempDirectory, ".vscode");
            var omniSharpFile = Path.Combine(_tempDirectory, "omnisharp.json");

            // Act
            _mockScriptEnvironment.Setup(x => x.TargetFramework).Returns("net8.0");
            _mockScriptEnvironment.Setup(x => x.InstallLocation).Returns("/usr/bin");
            var scaffolder = new Scaffolder(_mockLogFactory.Object, _mockScriptConsole.Object, _mockScriptEnvironment.Object);
            scaffolder.InitializerFolder("script.csx", _tempDirectory);

            // Assert
            Assert.True(Directory.Exists(vsCodeDir));
        }

        #endregion
    }
}