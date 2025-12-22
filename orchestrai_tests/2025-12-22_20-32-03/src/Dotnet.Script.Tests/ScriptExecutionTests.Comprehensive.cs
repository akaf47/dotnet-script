using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using Dotnet.Script.DependencyModel.Environment;
using Dotnet.Script.Shared.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet.Script.Tests
{
    /// <summary>
    /// Comprehensive test suite for ScriptExecutionTests covering all code paths, edge cases, and error scenarios.
    /// </summary>
    [Collection("IntegrationTests")]
    public class ScriptExecutionTestsComprehensive
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ScriptExecutionTestsComprehensive(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var dllCache = Path.Combine(Path.GetTempPath(), "dotnet-scripts");
            FileUtils.RemoveDirectory(dllCache);
            testOutputHelper.Capture();
        }

        // ========== BASIC EXECUTION TESTS ==========
        
        [Fact]
        public void ShouldExecuteHelloWorld_Success()
        {
            var (output, exitCode) = ScriptTestRunner.Default.ExecuteFixture("HelloWorld", "--no-cache");
            Assert.Contains("Hello World", output);
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ShouldExecuteHelloWorld_WithoutArgs()
        {
            var (output, exitCode) = ScriptTestRunner.Default.ExecuteFixture("HelloWorld");
            Assert.Contains("Hello World", output);
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ShouldExecuteHelloWorld_MultipleExecutions()
        {
            for (int i = 0; i < 3; i++)
            {
                var (output, exitCode) = ScriptTestRunner.Default.ExecuteFixture("HelloWorld", "--no-cache");
                Assert.Contains("Hello World", output);
                Assert.Equal(0, exitCode);
            }
        }

        // ========== NUGET PACKAGE TESTS ==========

        [Fact]
        public void ShouldExecuteScriptWithInlineNugetPackage_Success()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("InlineNugetPackage");
            Assert.Contains("AutoMapper.MapperConfiguration", output);
        }

        [Fact]
        public void ShouldExecuteScriptWithInlineNugetPackage_VerifyOutput()
        {
            var (output, exitCode) = ScriptTestRunner.Default.ExecuteFixture("InlineNugetPackage");
            Assert.Equal(0, exitCode);
            Assert.NotEmpty(output);
            Assert.Contains("AutoMapper", output);
        }

        // ========== NULLABLE CONTEXT TESTS ==========

        [Fact]
        public void ShouldHandleNullableContextAsError_ReturnsExitCode1()
        {
            var (output, exitCode) = ScriptTestRunner.Default.ExecuteFixture("Nullable");
            Assert.Equal(1, exitCode);
        }

        [Fact]
        public void ShouldHandleNullableContextAsError_ContainsErrorCode()
        {
            var (output, exitCode) = ScriptTestRunner.Default.ExecuteFixture("Nullable");
            Assert.Contains("error CS8625", output);
        }

        [Fact]
        public void ShouldHandleNullableContextAsError_OutputNotEmpty()
        {
            var (output, exitCode) = ScriptTestRunner.Default.ExecuteFixture("Nullable");
            Assert.NotEmpty(output);
        }

        [Fact]
        public void ShouldNotHandleDisabledNullableContextAsError_Success()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("NullableDisabled");
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ShouldNotHandleDisabledNullableContextAsError_NoErrors()
        {
            var (output, exitCode) = ScriptTestRunner.Default.ExecuteFixture("NullableDisabled");
            Assert.DoesNotContain("error", output, StringComparison.OrdinalIgnoreCase);
        }

        // ========== EXCEPTION HANDLING TESTS ==========

        [Fact]
        public void ShouldIncludeExceptionLineNumberAndFile_Success()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Exception", "--no-cache");
            Assert.Contains("Exception.csx:line 1", output);
        }

        [Fact]
        public void ShouldIncludeExceptionLineNumberAndFile_Format()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Exception", "--no-cache");
            Assert.Matches(@"Exception\.csx:line \d+", output);
        }

        [Fact]
        public void ShouldReturnExitCodeOneWhenScriptFails()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("Exception");
            Assert.Equal(1, exitCode);
        }

        [Fact]
        public void ShouldReturnStackTraceInformationWhenScriptFails_Contains()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Exception", "--no-cache");
            Assert.Contains("die!", output);
            Assert.Contains("Exception.csx:line 1", output);
        }

        [Fact]
        public void ShouldReturnStackTraceInformationWhenScriptFails_Format()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Exception", "--no-cache");
            Assert.NotEmpty(output);
            var outputLower = output.ToLower();
            Assert.True(outputLower.Contains("exception") || outputLower.Contains("error"));
        }

        // ========== NATIVE LIBRARY TESTS ==========

        [Fact]
        public void ShouldHandlePackageWithNativeLibraries_Success()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("NativeLibrary", "--no-cache");
            Assert.Contains("Connection successful", output);
        }

        [Fact]
        public void ShouldHandlePackageWithNativeLibraries_ExitCode()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("NativeLibrary", "--no-cache");
            Assert.Equal(0, exitCode);
        }

        // ========== COMPILATION ERROR TESTS ==========

        [Fact]
        public void ShouldReturnExitCodeOneWhenScriptFailsToCompile()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("CompilationError");
            Assert.Equal(1, exitCode);
        }

        [Fact]
        public void ShouldReturnExitCodeOneWhenScriptFailsToCompile_OutputNotEmpty()
        {
            var (output, exitCode) = ScriptTestRunner.Default.ExecuteFixture("CompilationError");
            Assert.Equal(1, exitCode);
            Assert.NotEmpty(output);
        }

        [Fact]
        public void ShouldWriteCompilerWarningsToStandardError()
        {
            var result = ScriptTestRunner.Default.ExecuteFixture(fixture: "CompilationWarning", "--no-cache");
            Assert.True(string.IsNullOrWhiteSpace(result.StandardOut) || !result.StandardOut.Contains("CS1998"));
            Assert.Contains("CS1998", result.StandardError, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ShouldWriteCompilerWarningsToStandardError_WarningFormat()
        {
            var result = ScriptTestRunner.Default.ExecuteFixture(fixture: "CompilationWarning", "--no-cache");
            Assert.NotEmpty(result.StandardError);
        }

        // ========== ISSUE HANDLING TESTS ==========

        [Fact]
        public void ShouldHandleIssue129()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Issue129");
            Assert.Contains("Bad HTTP authentication header", output);
        }

        [Fact]
        public void ShouldHandleIssue129_ExitCode()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("Issue129");
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ShouldHandleIssue166()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Issue166", "--no-cache");
            Assert.Contains("Connection successful", output);
        }

        [Fact]
        public void ShouldHandleIssue166_ExitCode()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("Issue166", "--no-cache");
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ShouldHandleIssue181()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Issue181");
            Assert.Contains("42", output);
        }

        [Fact]
        public void ShouldHandleIssue181_ExitCode()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("Issue181");
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ShouldHandleIssue189()
        {
            var (output, _) = ScriptTestRunner.Default.Execute($"\"{Path.Combine(TestPathUtils.GetPathToTestFixtureFolder("Issue189"), "SomeFolder", "Script.csx")}\"");
            Assert.Contains("Newtonsoft.Json.JsonConvert", output);
        }

        [Fact]
        public void ShouldHandleIssue189_WithPath()
        {
            var fixturePath = TestPathUtils.GetPathToTestFixtureFolder("Issue189");
            Assert.True(Directory.Exists(fixturePath));
            var (output, exitCode) = ScriptTestRunner.Default.Execute($"\"{Path.Combine(fixturePath, "SomeFolder", "Script.csx")}\"");
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ShouldHandleIssue198()
        {
            var result = ScriptTestRunner.ExecuteFixtureInProcess("Issue198");
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldHandleIssue204()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Issue204");
            Assert.Contains("System.Net.WebProxy", output);
        }

        [Fact]
        public void ShouldHandleIssue204_ExitCode()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("Issue204");
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ShouldHandleIssue214()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Issue214");
            Assert.Contains("Hello World!", output);
        }

        [Fact]
        public void ShouldHandleIssue318()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Issue318");
            Assert.Contains("Hello World!", output);
        }

        [Fact]
        public void ShouldHandleIssue268()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Issue268");
            Assert.Contains("value:", output);
        }

        [Fact]
        public void ShouldHandleIssue435()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Issue435");
            Assert.Contains("value:Microsoft.Extensions.Configuration.ConfigurationBuilder", output);
        }

        [Fact]
        public void ShouldHandleIssue613()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("Issue613");
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ShouldHandleIssue235()
        {
            string code =
            @"using AgileObjects.AgileMapper;
    namespace TestLibrary
    {
        public class TestClass
        {
            public TestClass()
            {
                IMapper mapper = Mapper.CreateNew();
            }
        }
    }
    ";

            string script =
    @"#r ""nuget: AgileObjects.AgileMapper, 0.25.0""
#r ""testlib/TestLibrary.dll""

    using AgileObjects.AgileMapper;
    using TestLibrary;

    IMapper mapper = Mapper.CreateNew();
    var testClass = new TestClass();
    Console.WriteLine(""Hello World!"");";

            using var disposableFolder = new DisposableFolder();
            var projectFolder = Path.Combine(disposableFolder.Path, "TestLibrary");
            ProcessHelper.RunAndCaptureOutput("dotnet", "new classlib -n TestLibrary", disposableFolder.Path);
            ProcessHelper.RunAndCaptureOutput("dotnet", "add TestLibrary.csproj package AgileObjects.AgileMapper -v 0.25.0", projectFolder);
            File.WriteAllText(Path.Combine(projectFolder, "Class1.cs"), code);
            File.WriteAllText(Path.Combine(projectFolder, "script.csx"), script);
            ProcessHelper.RunAndCaptureOutput("dotnet", "build -c release -o testlib", projectFolder);

            var (output, exitCode) = ScriptTestRunner.Default.Execute(Path.Combine(projectFolder, "script.csx"));

            Assert.Equal(0, exitCode);
            Assert.Contains("Hello World!", output);
        }

        // ========== ARGUMENT PASSING TESTS ==========

        [Fact]
        public void ShouldPassUnknownArgumentToScript()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Arguments", "arg1");
            Assert.Contains("arg1", output);
        }

        [Fact]
        public void ShouldPassUnknownArgumentToScript_MultipleArgs()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Arguments", "arg1", "arg2", "arg3");
            Assert.Contains("arg1", output);
        }

        [Fact]
        public void ShouldPassKnownArgumentToScriptWhenEscapedByDoubleHyphen()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Arguments", "-- -v");
            Assert.Contains("-v", output);
        }

        [Fact]
        public void ShouldPassKnownArgumentToScriptWhenEscapedByDoubleHyphen_Multiple()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Arguments", "-- -v -d -q");
            Assert.Contains("-v", output);
        }

        [Fact]
        public void ShouldNotPassUnEscapedKnownArgumentToScript()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Arguments", "-v");
            Assert.DoesNotContain("-v", output);
        }

        [Fact]
        public void ShouldNotPassUnEscapedKnownArgumentToScript_Multiple()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Arguments", "-v", "-d");
            Assert.DoesNotContain("-v", output);
            Assert.DoesNotContain("-d", output);
        }

        // ========== RETURN VALUE TESTS ==========

        [Fact]
        public void ShouldPropagateReturnValue()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("ReturnValue");
            Assert.Equal(42, exitCode);
        }

        [Fact]
        public void ShouldPropagateReturnValue_ZeroExitCode()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("HelloWorld");
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ShouldPropagateReturnValue_NonZeroExitCode()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("Exception");
            Assert.NotEqual(0, exitCode);
        }

        // ========== CONFIGURATION TESTS ==========

        [Theory]
        [InlineData("release", "false")]
        [InlineData("debug", "true")]
        public void ShouldCompileScriptWithReleaseConfiguration(string configuration, string expected)
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Configuration", $"-c {configuration}");
            Assert.Contains(expected, output, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ShouldCompileScriptWithDebugConfigurationWhenSpecified()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Configuration", "-c debug");
            Assert.Contains("true", output, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ShouldCompileScriptWithDebugConfigurationWhenNotSpecified()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Configuration");
            Assert.Contains("true", output, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ShouldCompileScriptWithReleaseConfigurationExplicitly()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Configuration", "-c release");
            Assert.Contains("false", output, StringComparison.OrdinalIgnoreCase);
        }

        // ========== C# VERSION TESTS ==========

        [Fact]
        public void ShouldHandleCSharp72()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("CSharp72");
            Assert.Contains("hi", output);
        }

        [Fact]
        public void ShouldHandleCSharp72_ExitCode()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("CSharp72");
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ShouldHandleCSharp80()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("CSharp80");
            Assert.Equal(0, exitCode);
        }

        // ========== CODE EVALUATION TESTS ==========

        [Fact]
        public void ShouldEvaluateCode()
        {
            var code = "Console.WriteLine(12345);";
            var (output, _) = ScriptTestRunner.Default.ExecuteCode(code);
            Assert.Contains("12345", output);
        }

        [Fact]
        public void ShouldEvaluateCode_EmptyCode()
        {
            var code = "";
            var (output, exitCode) = ScriptTestRunner.Default.ExecuteCode(code);
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ShouldEvaluateCode_SimpleExpression()
        {
            var code = "Console.WriteLine(1 + 1);";
            var (output, _) = ScriptTestRunner.Default.ExecuteCode(code);
            Assert.Contains("2", output);
        }

        [Fact]
        public void ShouldEvaluateCodeInReleaseMode()
        {
            var code = File.ReadAllText(Path.Combine("..", "..", "..", "TestFixtures", "Configuration", "Configuration.csx"));
            var (output, _) = ScriptTestRunner.Default.ExecuteCodeInReleaseMode(code);
            Assert.Contains("false", output, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ShouldEvaluateCodeInReleaseMode_OutputNotNull()
        {
            var code = "Console.WriteLine(42);";
            var (output, _) = ScriptTestRunner.Default.ExecuteCodeInReleaseMode(code);
            Assert.NotEmpty(output);
        }

        // ========== INLINE NUGET TESTS ==========

        [Fact]
        public void ShouldSupportInlineNugetReferencesinEvaluatedCode()
        {
            var code = @"#r \""nuget: AutoMapper, 6.1.1\"" using AutoMapper; Console.WriteLine(typeof(MapperConfiguration));";
            var (output, _) = ScriptTestRunner.Default.ExecuteCode(code);
            Assert.Contains("AutoMapper.MapperConfiguration", output);
        }

        [Fact]
        public void ShouldSupportInlineNugetReferencesWithTrailingSemicoloninEvaluatedCode()
        {
            var code = @"#r \""nuget: AutoMapper, 6.1.1\""; using AutoMapper; Console.WriteLine(typeof(MapperConfiguration));";
            var (output, _) = ScriptTestRunner.Default.ExecuteCode(code);
            Assert.Contains("AutoMapper.MapperConfiguration", output);
        }

        [Fact]
        public void ShouldSupportInlineNugetReferences_ExitCode()
        {
            var code = @"#r \""nuget: AutoMapper, 6.1.1\""";
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteCode(code);
            Assert.Equal(0, exitCode);
        }

        // ========== REMOTE SCRIPT TESTS ==========

        [Theory]
        [InlineData("https://gist.githubusercontent.com/seesharper/5d6859509ea8364a1fdf66bbf5b7923d/raw/0a32bac2c3ea807f9379a38e251d93e39c8131cb/HelloWorld.csx",
            "Hello World")]
        [InlineData("http://gist.githubusercontent.com/seesharper/5d6859509ea8364a1fdf66bbf5b7923d/raw/0a32bac2c3ea807f9379a38e251d93e39c8131cb/HelloWorld.csx",
            "Hello World")]
        [InlineData("https://github.com/dotnet-script/dotnet-script/files/5035247/hello.csx.gz",
                    "Hello, world!")]
        public void ShouldExecuteRemoteScript(string url, string expectedOutput)
        {
            var result = ScriptTestRunner.Default.Execute(url);
            Assert.Contains(expectedOutput, result.Output);
        }

        [Fact]
        public void ShouldExecuteRemoteScript_HttpsUrl()
        {
            var url = "https://gist.githubusercontent.com/seesharper/5d6859509ea8364a1fdf66bbf5b7923d/raw/0a32bac2c3ea807f9379a38e251d93e39c8131cb/HelloWorld.csx";
            var result = ScriptTestRunner.Default.Execute(url);
            Assert.Equal(0, result.ExitCode);
        }

        [Fact]
        public void ShouldThrowExceptionOnInvalidMediaType()
        {
            var url = "https://github.com/dotnet-script/dotnet-script/archive/0.20.0.zip";
            var (output, _) = ScriptTestRunner.Default.Execute(url);
            Assert.Contains("not supported", output);
        }

        [Fact]
        public void ShouldHandleNonExistingRemoteScript()
        {
            var url = "https://gist.githubusercontent.com/seesharper/5d6859509ea8364a1fdf66bbf5b7923d/raw/0a32bac2c3ea807f9379a38e251d93e39c8131cb/DoesNotExists.csx";
            var (output, _) = ScriptTestRunner.Default.Execute(url);
            Assert.Contains("Not Found", output);
        }

        // ========== PROCESS CLASS TESTS ==========

        [Fact]
        public void ShouldHandleScriptUsingTheProcessClass()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("Process");
            Assert.Contains("Success", output);
        }

        [Fact]
        public void ShouldHandleScriptUsingTheProcessClass_ExitCode()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("Process");
            Assert.Equal(0, exitCode);
        }

        // ========== VERSION RANGE TESTS ==========

        [Fact]
        public void ShouldHandleNuGetVersionRange()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("VersionRange");
            Assert.Contains("AutoMapper.MapperConfiguration", output);
        }

        [Fact]
        public void ShouldHandleNuGetVersionRange_ExitCode()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("VersionRange");
            Assert.Equal(0, exitCode);
        }

        // ========== DEPENDENCY RESOLUTION TESTS ==========

        [Fact]
        public void ShouldThrowMeaningfulErrorMessageWhenDependencyIsNotFound()
        {
            using var libraryFolder = new DisposableFolder();
            ProcessHelper.RunAndCaptureOutput("dotnet", "new classlib -n SampleLibrary", libraryFolder.Path);
            ProcessHelper.RunAndCaptureOutput("dotnet", "pack", libraryFolder.Path);

            using var scriptFolder = new DisposableFolder();
            var code = new StringBuilder();
            code.AppendLine("#r \"nuget:SampleLibrary, 1.0.0\"");
            code.AppendLine("WriteLine(42)");
            var pathToScript = Path.Combine(scriptFolder.Path, "main.csx");
            File.WriteAllText(pathToScript, code.ToString());

            var result = ScriptTestRunner.Default.Execute(pathToScript);
            Assert.Contains("42", result.Output);

            TestPathUtils.RemovePackageFromGlobalNugetCache("SampleLibrary");
            result = ScriptTestRunner.Default.Execute(pathToScript);
            Assert.Contains("Try executing/publishing the script", result.Output);

            result = ScriptTestRunner.Default.Execute($"{pathToScript} --no-cache");
            Assert.Contains("42", result.Output);
        }

        [Fact]
        public void ShouldThrowMeaningfulErrorMessageWhenDependencyIsNotFound_WithNoCacheOption()
        {
            using var libraryFolder = new DisposableFolder();
            ProcessHelper.RunAndCaptureOutput("dotnet", "new classlib -n TestLib", libraryFolder.Path);
            ProcessHelper.RunAndCaptureOutput("dotnet", "pack", libraryFolder.Path);

            using var scriptFolder = new DisposableFolder();
            var code = new StringBuilder();
            code.AppendLine("#r \"nuget:TestLib, 1.0.0\"");
            code.AppendLine("WriteLine(100)");
            var pathToScript = Path.Combine(scriptFolder.Path, "test.csx");
            File.WriteAllText(pathToScript, code.ToString());

            var result = ScriptTestRunner.Default.Execute($"{pathToScript} --no-cache");
            Assert.Equal(0, result.ExitCode);
        }

        // ========== NUGET CONFIG TESTS ==========

        [Fact]
        public void ShouldHandleLocalNuGetConfigWithRelativePath()
        {
            TestPathUtils.RemovePackageFromGlobalNugetCache("NuGetConfigTestLibrary");

            using var packageLibraryFolder = new DisposableFolder();
            CreateTestPackage(packageLibraryFolder.Path);

            string pathToScriptFile = CreateTestScript(packageLibraryFolder.Path);

            var (output, exitCode) = ScriptTestRunner.Default.Execute(pathToScriptFile);
            Assert.Contains("Success", output);
        }

        [Fact]
        public void ShouldHandleLocalNuGetConfigWithRelativePathInParentFolder()
        {
            TestPathUtils.RemovePackageFromGlobalNugetCache("NuGetConfigTestLibrary");

            using var packageLibraryFolder = new DisposableFolder();
            CreateTestPackage(packageLibraryFolder.Path);

            var scriptFolder = Path.Combine(packageLibraryFolder.Path, "ScriptFolder");
            Directory.CreateDirectory(scriptFolder);
            string pathToScriptFile = CreateTestScript(scriptFolder);

            var (output, exitCode) = ScriptTestRunner.Default.Execute(pathToScriptFile);
            Assert.Contains("Success", output);
        }

        [Fact]
        public void ShouldHandleLocalNuGetFileWhenPathContainsSpace()
        {
            TestPathUtils.RemovePackageFromGlobalNugetCache("NuGetConfigTestLibrary");

            using var packageLibraryFolder = new DisposableFolder();
            var packageLibraryFolderPath = Path.Combine(packageLibraryFolder.Path, "library folder");
            Directory.CreateDirectory(packageLibraryFolderPath);

            CreateTestPackage(packageLibraryFolderPath);

            string pathToScriptFile = CreateTestScript(packageLibraryFolderPath);

            var (output, exitCode) = ScriptTestRunner.Default.Execute($"\"{pathToScriptFile}\"");
            Assert.Contains("Success", output);
        }

        // ========== TARGET FRAMEWORK TESTS ==========

        [Fact]
        public void ShouldHandleScriptWithTargetFrameworkInShebang()
        {
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("TargetFrameworkInShebang");
            Assert.Contains("Hello world!", output);
        }

        [Fact]
        public void ShouldHandleScriptWithTargetFrameworkInShebang_ExitCode()
        {
            var (_, exitCode) = ScriptTestRunner.Default.ExecuteFixture("TargetFrameworkInShebang");
            Assert.Equal(0, exitCode);
        }

        // ========== GLOBAL JSON TESTS ==========

        [Fact]
        public void ShouldIgnoreGlobalJsonInScriptFolder()
        {
            var fixture = "InvalidGlobalJson";
            var workingDirectory = Path.GetDirectoryName(TestPathUtils.GetPathToTestFixture(fixture));
            var (output, _) = ScriptTestRunner.Default.ExecuteFixture("InvalidGlobalJson", $"--no-cache", workingDirectory);
            Assert.Contains("Hello world!", output);
        }

        [Fact]
        public void ShouldIgnoreGlobalJsonInScriptFolder_ExitCode()
        {