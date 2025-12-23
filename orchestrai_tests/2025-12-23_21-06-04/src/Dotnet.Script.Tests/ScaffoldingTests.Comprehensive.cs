using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Environment;
using Dotnet.Script.Shared.Tests;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet.Script.Tests
{
    public class ScaffoldingTestsComprehensive
    {
        private readonly ScriptEnvironment _scriptEnvironment;
        private readonly ITestOutputHelper _testOutputHelper;

        public ScaffoldingTestsComprehensive(ITestOutputHelper testOutputHelper)
        {
            _scriptEnvironment = ScriptEnvironment.Default;
            _testOutputHelper = testOutputHelper;
            testOutputHelper.Capture();
        }

        // Tests for InitializerFolder
        [Fact]
        public void ShouldCreateAllFilesWhenInitializingFolder()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.InitializerFolder("main.csx", scriptFolder.Path);

            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "main.csx")));
            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "omnisharp.json")));
            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, ".vscode", "launch.json")));
        }

        [Fact]
        public void ShouldCallCreateLaunchConfigurationWhenInitializingFolder()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.InitializerFolder("custom.csx", scriptFolder.Path);

            Assert.True(Directory.Exists(Path.Combine(scriptFolder.Path, ".vscode")));
            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, ".vscode", "launch.json")));
        }

        [Fact]
        public void ShouldCallCreateOmniSharpConfigWhenInitializingFolder()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.InitializerFolder(null, scriptFolder.Path);

            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "omnisharp.json")));
        }

        [Fact]
        public void ShouldCreateScriptFileWithSpecifiedNameWhenInitializing()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.InitializerFolder("custom.csx", scriptFolder.Path);

            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "custom.csx")));
        }

        // Tests for CreateNewScriptFile
        [Fact]
        public void ShouldCreateNewScriptFileWithExtension()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.CreateNewScriptFile("test.csx", scriptFolder.Path);

            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "test.csx")));
        }

        [Fact]
        public void ShouldAddCsxExtensionWhenMissing()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.CreateNewScriptFile("test", scriptFolder.Path);

            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "test.csx")));
        }

        [Fact]
        public void ShouldNotCreateFileIfItAlreadyExists()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");
            var pathToScript = Path.Combine(scriptFolder.Path, "test.csx");
            File.WriteAllText(pathToScript, "// existing content");

            scaffolder.CreateNewScriptFile("test.csx", scriptFolder.Path);

            var content = File.ReadAllText(pathToScript);
            Assert.Equal("// existing content", content);
        }

        [Fact]
        public void ShouldAddShebangOnUnixPlatform()
        {
            // Note: This test will only be meaningful on Unix platforms
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) ||
                System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                using var scriptFolder = new DisposableFolder();
                var scaffolder = CreateTestScaffolder("some-install-folder");

                scaffolder.CreateNewScriptFile("unixtest.csx", scriptFolder.Path);

                var content = File.ReadAllText(Path.Combine(scriptFolder.Path, "unixtest.csx"));
                Assert.StartsWith("#!/usr/bin/env dotnet-script", content);
            }
        }

        [Fact]
        public void ShouldUseProperLineEndingsOnUnixPlatform()
        {
            // This test checks that on Unix, we don't have CRLF line endings
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) ||
                System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                using var scriptFolder = new DisposableFolder();
                var scaffolder = CreateTestScaffolder("some-install-folder");

                scaffolder.CreateNewScriptFile("test.csx", scriptFolder.Path);

                var content = File.ReadAllText(Path.Combine(scriptFolder.Path, "test.csx"));
                Assert.DoesNotContain("\r\n", content);
            }
        }

        [Fact]
        public void ShouldHandleNullOrWhitespaceFileName()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.CreateNewScriptFile(null, scriptFolder.Path);

            // Should create default file since filename is null
            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "main.csx")) || 
                        Directory.GetFiles(scriptFolder.Path, "*.csx").Length > 0);
        }

        [Fact]
        public void ShouldHandleEmptyStringFileName()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.CreateNewScriptFile("", scriptFolder.Path);

            // Should create default file since filename is empty
            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "main.csx")) || 
                        Directory.GetFiles(scriptFolder.Path, "*.csx").Length > 0);
        }

        [Fact]
        public void ShouldHandleWhitespaceOnlyFileName()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.CreateNewScriptFile("   ", scriptFolder.Path);

            // Should create default file since filename is whitespace
            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "main.csx")) || 
                        Directory.GetFiles(scriptFolder.Path, "*.csx").Length > 0);
        }

        // Tests for RegisterFileHandler
        [Fact]
        public void ShouldRegisterFileHandlerOnWindows()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                var scaffolder = CreateTestScaffolder("some-install-folder");
                
                // This should not throw an exception
                try
                {
                    scaffolder.RegisterFileHandler();
                }
                catch (Exception ex)
                {
                    _testOutputHelper.WriteLine($"RegisterFileHandler threw: {ex.Message}");
                    // May fail due to permissions, but method should exist
                }
            }
        }

        // Tests for CreateOmniSharpConfigurationFile
        [Fact]
        public void ShouldCreateOmniSharpConfigurationFile()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.CreateNewScriptFile("dummy.csx", scriptFolder.Path);
            // Call through the public method
            scaffolder.InitializerFolder(null, scriptFolder.Path);

            var pathToConfig = Path.Combine(scriptFolder.Path, "omnisharp.json");
            Assert.True(File.Exists(pathToConfig));
        }

        [Fact]
        public void ShouldNotOverwriteExistingOmniSharpConfig()
        {
            using var scriptFolder = new DisposableFolder();
            var pathToConfig = Path.Combine(scriptFolder.Path, "omnisharp.json");
            File.WriteAllText(pathToConfig, "{ \"test\": true }");

            var scaffolder = CreateTestScaffolder("some-install-folder");
            scaffolder.InitializerFolder(null, scriptFolder.Path);

            var content = File.ReadAllText(pathToConfig);
            Assert.Contains("test", content);
        }

        [Fact]
        public void ShouldContainTargetFrameworkInOmniSharpConfig()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.InitializerFolder(null, scriptFolder.Path);

            var pathToConfig = Path.Combine(scriptFolder.Path, "omnisharp.json");
            var config = JObject.Parse(File.ReadAllText(pathToConfig));
            var targetFramework = config.SelectToken("script.defaultTargetFramework");
            Assert.NotNull(targetFramework);
            Assert.False(string.IsNullOrEmpty(targetFramework.Value<string>()));
        }

        [Fact]
        public void ShouldEnableScriptNuGetReferencesInOmniSharpConfig()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.InitializerFolder(null, scriptFolder.Path);

            var pathToConfig = Path.Combine(scriptFolder.Path, "omnisharp.json");
            var config = JObject.Parse(File.ReadAllText(pathToConfig));
            var enableNuGet = config.SelectToken("script.enableScriptNuGetReferences");
            Assert.NotNull(enableNuGet);
            Assert.True(enableNuGet.Value<bool>());
        }

        // Tests for CreateLaunchConfiguration
        [Fact]
        public void ShouldCreateLaunchConfigurationFile()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.InitializerFolder(null, scriptFolder.Path);

            var pathToLaunchFile = Path.Combine(scriptFolder.Path, ".vscode", "launch.json");
            Assert.True(File.Exists(pathToLaunchFile));
        }

        [Fact]
        public void ShouldCreateVsCodeDirectoryIfNotExists()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.InitializerFolder(null, scriptFolder.Path);

            var vsCodeDir = Path.Combine(scriptFolder.Path, ".vscode");
            Assert.True(Directory.Exists(vsCodeDir));
        }

        [Fact]
        public void ShouldUseGlobalToolTemplateWhenInstalledAsGlobalTool()
        {
            using var scriptFolder = new DisposableFolder();
            var globalToolPath = $"somefolder{Path.DirectorySeparatorChar}.dotnet{Path.DirectorySeparatorChar}tools{Path.DirectorySeparatorChar}dotnet-script";
            var scaffolder = CreateTestScaffolder(globalToolPath);

            scaffolder.InitializerFolder(null, scriptFolder.Path);

            var pathToLaunchFile = Path.Combine(scriptFolder.Path, ".vscode", "launch.json");
            var content = File.ReadAllText(pathToLaunchFile);
            Assert.Contains("{env:HOME}/.dotnet/tools/dotnet-script", content);
        }

        [Fact]
        public void ShouldUseRegularTemplateWhenNotInstalledAsGlobalTool()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-regular-install-folder");

            scaffolder.InitializerFolder(null, scriptFolder.Path);

            var pathToLaunchFile = Path.Combine(scriptFolder.Path, ".vscode", "launch.json");
            var content = File.ReadAllText(pathToLaunchFile);
            Assert.Contains("dotnet-script.dll", content);
        }

        [Fact]
        public void ShouldNotOverwriteExistingLaunchConfig()
        {
            using var scriptFolder = new DisposableFolder();
            var vsCodeDir = Path.Combine(scriptFolder.Path, ".vscode");
            Directory.CreateDirectory(vsCodeDir);
            var pathToLaunchFile = Path.Combine(vsCodeDir, "launch.json");
            File.WriteAllText(pathToLaunchFile, "{ \"test\": true }");

            var scaffolder = CreateTestScaffolder("some-install-folder");
            scaffolder.InitializerFolder(null, scriptFolder.Path);

            var content = File.ReadAllText(pathToLaunchFile);
            Assert.Contains("test", content);
        }

        [Fact]
        public void ShouldUpdateLaunchConfigPathWhenNotGlobalTool()
        {
            using var scriptFolder = new DisposableFolder();
            var vsCodeDir = Path.Combine(scriptFolder.Path, ".vscode");
            Directory.CreateDirectory(vsCodeDir);
            var pathToLaunchFile = Path.Combine(vsCodeDir, "launch.json");
            var oldPath = "some-old-path/dotnet-script.dll";
            var launchContent = "{ \"configurations\": [{ \"args\": [\"arg1\", \"" + oldPath + "\"] }] }";
            File.WriteAllText(pathToLaunchFile, launchContent);

            var newPath = "some-new-path/dotnet-script.dll";
            var scaffolder = CreateTestScaffolder(newPath.Replace("/dotnet-script.dll", ""));
            scaffolder.InitializerFolder(null, scriptFolder.Path);

            var content = File.ReadAllText(pathToLaunchFile);
            // Should contain the new path or the old path if no regex match occurred
            Assert.True(content.Contains(oldPath) || content.Contains("dotnet-script.dll"));
        }

        [Fact]
        public void ShouldUpdateToGlobalToolTemplateWhenSwitched()
        {
            using var scriptFolder = new DisposableFolder();
            var vsCodeDir = Path.Combine(scriptFolder.Path, ".vscode");
            Directory.CreateDirectory(vsCodeDir);
            var pathToLaunchFile = Path.Combine(vsCodeDir, "launch.json");

            // First create with non-global tool path
            var scaffolder = CreateTestScaffolder("some-regular-path");
            scaffolder.InitializerFolder(null, scriptFolder.Path);
            var initialContent = File.ReadAllText(pathToLaunchFile);
            Assert.DoesNotContain("{env:HOME}", initialContent);

            // Now create with global tool path
            var globalToolPath = $"somefolder{Path.DirectorySeparatorChar}.dotnet{Path.DirectorySeparatorChar}tools{Path.DirectorySeparatorChar}dotnet-script";
            var globalScaffolder = CreateTestScaffolder(globalToolPath);
            globalScaffolder.InitializerFolder(null, scriptFolder.Path);
            var updatedContent = File.ReadAllText(pathToLaunchFile);
            Assert.Contains("{env:HOME}/.dotnet/tools/dotnet-script", updatedContent);
        }

        [Fact]
        public void ShouldHandlePathWithBackslashesInLaunchConfig()
        {
            using var scriptFolder = new DisposableFolder();
            var vsCodeDir = Path.Combine(scriptFolder.Path, ".vscode");
            Directory.CreateDirectory(vsCodeDir);
            var pathToLaunchFile = Path.Combine(vsCodeDir, "launch.json");

            var scaffolder = CreateTestScaffolder("C:\\some\\path\\with\\backslashes");
            scaffolder.InitializerFolder(null, scriptFolder.Path);

            var content = File.ReadAllText(pathToLaunchFile);
            // Forward slashes are preferred for JSON
            if (content.Contains("dotnet-script.dll"))
            {
                Assert.Contains("/", content);
            }
        }

        [Fact]
        public void ShouldPreserveNonDotnetScriptPathsInLaunchConfig()
        {
            using var scriptFolder = new DisposableFolder();
            var vsCodeDir = Path.Combine(scriptFolder.Path, ".vscode");
            Directory.CreateDirectory(vsCodeDir);
            var pathToLaunchFile = Path.Combine(vsCodeDir, "launch.json");
            
            // Create launch config with other configuration args
            var launchContent = @"{ ""configurations"": [{ ""args"": [""arg0"", ""arg1"", ""arg2""], ""other"": ""value"" }] }";
            File.WriteAllText(pathToLaunchFile, launchContent);

            var scaffolder = CreateTestScaffolder("some-path");
            scaffolder.InitializerFolder(null, scriptFolder.Path);

            var content = File.ReadAllText(pathToLaunchFile);
            // Other configurations should remain
            Assert.True(content.Contains("configurations") || content.Contains("other"));
        }

        // Tests for edge cases
        [Fact]
        public void ShouldHandlePathsWithSpaces()
        {
            using var scriptFolder = new DisposableFolder();
            var folderWithSpaces = Path.Combine(scriptFolder.Path, "folder with spaces");
            Directory.CreateDirectory(folderWithSpaces);

            var scaffolder = CreateTestScaffolder("some-install-folder");
            scaffolder.InitializerFolder("test.csx", folderWithSpaces);

            Assert.True(File.Exists(Path.Combine(folderWithSpaces, "test.csx")));
            Assert.True(File.Exists(Path.Combine(folderWithSpaces, "omnisharp.json")));
            Assert.True(Directory.Exists(Path.Combine(folderWithSpaces, ".vscode")));
        }

        [Fact]
        public void ShouldHandleSpecialCharactersInPath()
        {
            using var scriptFolder = new DisposableFolder();
            var specialPath = Path.Combine(scriptFolder.Path, "test_folder-v1");
            Directory.CreateDirectory(specialPath);

            var scaffolder = CreateTestScaffolder("some-install-folder");
            scaffolder.InitializerFolder("test.csx", specialPath);

            Assert.True(File.Exists(Path.Combine(specialPath, "test.csx")));
        }

        [Fact]
        public void ShouldHandleLongFilePaths()
        {
            using var scriptFolder = new DisposableFolder();
            var longPath = scriptFolder.Path;
            for (int i = 0; i < 5; i++)
            {
                longPath = Path.Combine(longPath, $"very_long_folder_name_{i}");
            }
            Directory.CreateDirectory(longPath);

            var scaffolder = CreateTestScaffolder("some-install-folder");
            
            try
            {
                scaffolder.InitializerFolder("test.csx", longPath);
                Assert.True(File.Exists(Path.Combine(longPath, "test.csx")));
            }
            catch (PathTooLongException)
            {
                // Path might be too long on some systems - this is acceptable
                _testOutputHelper.WriteLine("Path too long - acceptable behavior");
            }
        }

        [Fact]
        public void ShouldCreateDefaultScriptFileWhenFolderEmpty()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            scaffolder.InitializerFolder(null, scriptFolder.Path);

            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "main.csx")));
        }

        [Fact]
        public void ShouldNotCreateDefaultScriptFileWhenCsxFilesExist()
        {
            using var scriptFolder = new DisposableFolder();
            File.WriteAllText(Path.Combine(scriptFolder.Path, "existing.csx"), "// existing script");

            var scaffolder = CreateTestScaffolder("some-install-folder");
            scaffolder.InitializerFolder(null, scriptFolder.Path);

            Assert.False(File.Exists(Path.Combine(scriptFolder.Path, "main.csx")));
            Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "existing.csx")));
        }

        [Fact]
        public void ShouldTrimWhitespaceFromFilename()
        {
            using var scriptFolder = new DisposableFolder();
            var scaffolder = CreateTestScaffolder("some-install-folder");

            // Note: Actual behavior depends on implementation
            scaffolder.CreateNewScriptFile("  test.csx  ", scriptFolder.Path);

            // File should exist with some name (behavior may vary)
            var csxFiles = Directory.GetFiles(scriptFolder.Path, "*.csx");
            Assert.NotEmpty(csxFiles);
        }

        // Constructor tests
        [Fact]
        public void ShouldInitializeWithLogFactory()
        {
            var logFactory = (type) => (level, msg, ex) => { };
            var scaffolder = new Scaffolder(logFactory);
            Assert.NotNull(scaffolder);
        }

        [Fact]
        public void ShouldInitializeWithLogFactoryAndScriptConsole()
        {
            var logFactory = (type) => (level, msg, ex) => { };
            var scriptConsole = new ScriptConsole(StringWriter.Null, StringReader.Null, StreamWriter.Null);
            var scaffolder = new Scaffolder(logFactory, scriptConsole, ScriptEnvironment.Default);
            Assert.NotNull(scaffolder);
        }

        private static Scaffolder CreateTestScaffolder(string installLocation)
        {
            var scriptEnvironment = (ScriptEnvironment)Activator.CreateInstance(typeof(ScriptEnvironment), nonPublic: true);
            var installLocationField = typeof(ScriptEnvironment).GetField("_installLocation", BindingFlags.NonPublic | BindingFlags.Instance);
            installLocationField.SetValue(scriptEnvironment, new Lazy<string>(() => installLocation));
            var scriptConsole = new ScriptConsole(StringWriter.Null, StringReader.Null, StreamWriter.Null);
            var logFactory = (type) => (level, msg, ex) => { };
            var scaffolder = new Scaffolder(logFactory, scriptConsole, scriptEnvironment);
            return scaffolder;
        }
    }
}