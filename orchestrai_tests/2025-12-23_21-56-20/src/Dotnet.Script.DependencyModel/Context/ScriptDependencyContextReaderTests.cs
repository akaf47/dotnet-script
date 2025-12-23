using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Moq;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.ProjectModel;
using NuGet.Versioning;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.ScriptPackage;
using Dotnet.Script.DependencyModel.Environment;

namespace Dotnet.Script.DependencyModel.Tests.Context
{
    public class ScriptDependencyContextReaderTests
    {
        private readonly Mock<LogFactory> _mockLogFactory;
        private readonly Mock<Logger> _mockLogger;
        private readonly Mock<ScriptFilesDependencyResolver> _mockScriptFilesDependencyResolver;

        public ScriptDependencyContextReaderTests()
        {
            _mockLogFactory = new Mock<LogFactory>();
            _mockLogger = new Mock<Logger>();
            _mockScriptFilesDependencyResolver = new Mock<ScriptFilesDependencyResolver>(new Mock<LogFactory>().Object);

            _mockLogFactory.Setup(lf => lf.CreateLogger<ScriptDependencyContextReader>()).Returns(_mockLogger.Object);
            _mockLogFactory.Setup(lf => lf.CreateLogger<ScriptDependencyContextReader.NuGetLogger>()).Returns(_mockLogger.Object);
        }

        [Fact]
        public void Constructor_With_LogFactory_Only_Should_Create_Default_Resolver()
        {
            // Act
            var reader = new ScriptDependencyContextReader(_mockLogFactory.Object);

            // Assert
            Assert.NotNull(reader);
        }

        [Fact]
        public void Constructor_With_LogFactory_And_Resolver_Should_Use_Provided_Resolver()
        {
            // Act
            var reader = new ScriptDependencyContextReader(_mockLogFactory.Object, _mockScriptFilesDependencyResolver.Object);

            // Assert
            Assert.NotNull(reader);
        }

        [Fact]
        public void ReadDependencyContext_With_Null_Path_Should_Throw_Exception()
        {
            // Arrange
            var reader = new ScriptDependencyContextReader(_mockLogFactory.Object, _mockScriptFilesDependencyResolver.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => reader.ReadDependencyContext(null));
        }

        [Fact]
        public void ReadDependencyContext_With_Non_Existent_File_Should_Throw_InvalidOperationException()
        {
            // Arrange
            var reader = new ScriptDependencyContextReader(_mockLogFactory.Object, _mockScriptFilesDependencyResolver.Object);
            var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "project.assets.json");

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => reader.ReadDependencyContext(nonExistentPath));
            Assert.Contains("Unable to read lockfile", exception.Message);
        }

        [Fact]
        public void ReadDependencyContext_With_Invalid_Json_Should_Throw_Exception()
        {
            // Arrange
            var reader = new ScriptDependencyContextReader(_mockLogFactory.Object, _mockScriptFilesDependencyResolver.Object);
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, "invalid json content {");

                // Act & Assert
                Assert.Throws<Exception>(() => reader.ReadDependencyContext(tempFile));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ReadDependencyContext_With_Less_Than_Two_Targets_Should_Throw_InvalidOperationException()
        {
            // Arrange
            var reader = new ScriptDependencyContextReader(_mockLogFactory.Object, _mockScriptFilesDependencyResolver.Object);
            var tempFile = CreateAssetFileWithTargetCount(1);
            try
            {
                // Act & Assert
                var exception = Assert.Throws<InvalidOperationException>(() => reader.ReadDependencyContext(tempFile));
                Assert.Contains("does not contain a runtime target", exception.Message);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ReadDependencyContext_With_Valid_Assets_File_Should_Return_Context()
        {
            // Arrange
            var reader = new ScriptDependencyContextReader(_mockLogFactory.Object, _mockScriptFilesDependencyResolver.Object);
            var tempFile = CreateValidAssetFile();
            try
            {
                // Act
                var context = reader.ReadDependencyContext(tempFile);

                // Assert
                Assert.NotNull(context);
                Assert.NotNull(context.Dependencies);
                Assert.True(context.Dependencies.Length >= 0);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ReadDependencyContext_Should_Include_NetCore_App_Dependency_When_Version_Three_Or_Higher()
        {
            // Arrange
            var reader = new ScriptDependencyContextReader(_mockLogFactory.Object, _mockScriptFilesDependencyResolver.Object);
            var tempFile = CreateValidAssetFile();
            try
            {
                // Act
                var context = reader.ReadDependencyContext(tempFile);

                // Assert
                Assert.NotNull(context);
                // If running on .NET Core 3.0 or higher, Microsoft.NETCore.App should be included
                if (ScriptEnvironment.Default.NetCoreVersion.Major >= 3)
                {
                    var netCoreAppDep = context.Dependencies.FirstOrDefault(d => d.Name == "Microsoft.NETCore.App");
                    Assert.NotNull(netCoreAppDep);
                }
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void IsAssembly_With_Valid_Assembly_Should_Return_True()
        {
            // Arrange - Use a known assembly file
            var assemblyPath = typeof(object).Assembly.Location;

            // Act
            var result = InvokeIsAssemblyMethod(assemblyPath);

            // Assert
            Assert.True((bool)result);
        }

        [Fact]
        public void IsAssembly_With_Invalid_File_Should_Return_False()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, "This is not an assembly");

                // Act
                var result = InvokeIsAssemblyMethod(tempFile);

                // Assert
                Assert.False((bool)result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void IsAssembly_With_Non_Existent_File_Should_Return_False()
        {
            // Arrange
            var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".dll");

            // Act
            var result = InvokeIsAssemblyMethod(nonExistentPath);

            // Assert
            Assert.False((bool)result);
        }

        [Fact]
        public void HasAspNetCoreFrameworkReference_With_No_AspNetCore_Reference_Should_Return_False()
        {
            // Arrange
            var reader = new ScriptDependencyContextReader(_mockLogFactory.Object, _mockScriptFilesDependencyResolver.Object);
            var tempFile = CreateValidAssetFile();
            try
            {
                // Act
                var result = InvokeHasAspNetCoreFrameworkReference(reader, tempFile);

                // Assert
                Assert.False((bool)result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void NuGetLogger_Log_With_Debug_Level_Should_Call_Debug()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var mockLogger = new Mock<Logger>();
            mockLogFactory.Setup(lf => lf.CreateLogger<ScriptDependencyContextReader.NuGetLogger>()).Returns(mockLogger.Object);

            var mockMessage = new Mock<ILogMessage>();
            mockMessage.Setup(m => m.Level).Returns(NuGet.Common.LogLevel.Debug);
            mockMessage.Setup(m => m.Message).Returns("Debug message");

            // Act
            InvokeNuGetLoggerLog(mockLogFactory.Object, mockMessage.Object);

            // Assert
            mockLogger.Verify(l => l.Debug(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void NuGetLogger_Log_With_Verbose_Level_Should_Call_Trace()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var mockLogger = new Mock<Logger>();
            mockLogFactory.Setup(lf => lf.CreateLogger<ScriptDependencyContextReader.NuGetLogger>()).Returns(mockLogger.Object);

            var mockMessage = new Mock<ILogMessage>();
            mockMessage.Setup(m => m.Level).Returns(NuGet.Common.LogLevel.Verbose);
            mockMessage.Setup(m => m.Message).Returns("Verbose message");

            // Act
            InvokeNuGetLoggerLog(mockLogFactory.Object, mockMessage.Object);

            // Assert
            mockLogger.Verify(l => l.Trace(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void NuGetLogger_Log_With_Information_Level_Should_Call_Info()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var mockLogger = new Mock<Logger>();
            mockLogFactory.Setup(lf => lf.CreateLogger<ScriptDependencyContextReader.NuGetLogger>()).Returns(mockLogger.Object);

            var mockMessage = new Mock<ILogMessage>();
            mockMessage.Setup(m => m.Level).Returns(NuGet.Common.LogLevel.Information);
            mockMessage.Setup(m => m.Message).Returns("Info message");

            // Act
            InvokeNuGetLoggerLog(mockLogFactory.Object, mockMessage.Object);

            // Assert
            mockLogger.Verify(l => l.Info(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void NuGetLogger_Log_With_Error_Level_Should_Call_Error()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var mockLogger = new Mock<Logger>();
            mockLogFactory.Setup(lf => lf.CreateLogger<ScriptDependencyContextReader.NuGetLogger>()).Returns(mockLogger.Object);

            var mockMessage = new Mock<ILogMessage>();
            mockMessage.Setup(m => m.Level).Returns(NuGet.Common.LogLevel.Error);
            mockMessage.Setup(m => m.Message).Returns("Error message");

            // Act
            InvokeNuGetLoggerLog(mockLogFactory.Object, mockMessage.Object);

            // Assert
            mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void NuGetLogger_Log_With_Minimal_Level_Should_Call_Info()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var mockLogger = new Mock<Logger>();
            mockLogFactory.Setup(lf => lf.CreateLogger<ScriptDependencyContextReader.NuGetLogger>()).Returns(mockLogger.Object);

            var mockMessage = new Mock<ILogMessage>();
            mockMessage.Setup(m => m.Level).Returns(NuGet.Common.LogLevel.Minimal);
            mockMessage.Setup(m => m.Message).Returns("Minimal message");

            // Act
            InvokeNuGetLoggerLog(mockLogFactory.Object, mockMessage.Object);

            // Assert
            mockLogger.Verify(l => l.Info(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void NuGetLogger_LogAsync_Should_Call_Log()
        {
            // Arrange
            var mockLogFactory = new Mock<LogFactory>();
            var mockLogger = new Mock<Logger>();
            mockLogFactory.Setup(lf => lf.CreateLogger<ScriptDependencyContextReader.NuGetLogger>()).Returns(mockLogger.Object);

            var mockMessage = new Mock<ILogMessage>();
            mockMessage.Setup(m => m.Level).Returns(NuGet.Common.LogLevel.Information);
            mockMessage.Setup(m => m.Message).Returns("Test message");

            // Act
            var result = InvokeNuGetLoggerLogAsync(mockLogFactory.Object, mockMessage.Object);

            // Assert
            Assert.NotNull(result);
            mockLogger.Verify(l => l.Info(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetAspNetCoreRuntimeInfo_With_Invalid_Version_Should_Throw_InvalidOperationException()
        {
            // Arrange
            var invalidPath = Path.Combine(Path.GetTempPath(), "invalid_version");

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                InvokeGetAspNetCoreRuntimeInfo(invalidPath));
            Assert.Contains("Unable to parse netcore app version", exception.Message);
        }

        [Fact]
        public void GetAspNetCoreRuntimeInfo_With_Valid_Version_But_No_AspNetCore_Should_Throw_InvalidOperationException()
        {
            // Arrange
            var validVersionPath = Path.Combine(Path.GetTempPath(), "8.0.0");
            Directory.CreateDirectory(validVersionPath);
            
            try
            {
                // Act & Assert
                var exception = Assert.Throws<InvalidOperationException>(() => 
                    InvokeGetAspNetCoreRuntimeInfo(validVersionPath));
                Assert.Contains("Failed to resolve the path to 'Microsoft.AspNetCore.App'", exception.Message);
            }
            finally
            {
                if (Directory.Exists(validVersionPath))
                    Directory.Delete(validVersionPath, true);
            }
        }

        // Helper methods
        private string CreateValidAssetFile()
        {
            var tempFile = Path.GetTempFileName();
            var json = @"{
  ""project"": {
    ""frameworks"": {
      ""netcoreapp3.1"": {}
    }
  },
  ""packageFolders"": [
    {
      ""path"": """ + Path.GetTempPath() + @"""
    }
  ],
  ""targets"": {
    ""netcoreapp3.1"": {},
    ""netcoreapp3.1/win-x64"": {
      ""TestPackage/1.0.0"": {
        ""type"": ""package"",
        ""dependencies"": {},
        ""runtime"": {},
        ""native"": {},
        ""compile"": {},
        ""contentFiles"": {}
      }
    }
  }
}";
            File.WriteAllText(tempFile, json);
            return tempFile;
        }

        private string CreateAssetFileWithTargetCount(int targetCount)
        {
            var tempFile = Path.GetTempFileName();
            var targetsJson = "";
            for (int i = 0; i < targetCount; i++)
            {
                targetsJson += $@"""target{i}"": {{}},";
            }
            if (targetsJson.EndsWith(","))
                targetsJson = targetsJson.Substring(0, targetsJson.Length - 1);

            var json = @"{
  ""project"": {
    ""frameworks"": {
      ""netcoreapp3.1"": {}
    }
  },
  ""packageFolders"": [
    {
      ""path"": """ + Path.GetTempPath() + @"""
    }
  ],
  ""targets"": {
    " + targetsJson + @"
  }
}";
            File.WriteAllText(tempFile, json);
            return tempFile;
        }

        private object InvokeIsAssemblyMethod(string filePath)
        {
            var method = typeof(ScriptDependencyContextReader).GetMethod("IsAssembly", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return method?.Invoke(null, new object[] { filePath });
        }

        private object InvokeHasAspNetCoreFrameworkReference(ScriptDependencyContextReader reader, string path)
        {
            var method = typeof(ScriptDependencyContextReader).GetMethod("HasAspNetCoreFrameworkReference", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return method?.Invoke(reader, new object[] { path });
        }

        private void InvokeNuGetLoggerLog(LogFactory logFactory, ILogMessage message)
        {
            var nugetLoggerType = typeof(ScriptDependencyContextReader).GetNestedType("NuGetLogger",
                System.Reflection.BindingFlags.NonPublic);
            var logMethod = nugetLoggerType?.GetMethod("Log", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var logger = Activator.CreateInstance(nugetLoggerType, logFactory);
            logMethod?.Invoke(logger, new object[] { message });
        }

        private object InvokeNuGetLoggerLogAsync(LogFactory logFactory, ILogMessage message)
        {
            var nugetLoggerType = typeof(ScriptDependencyContextReader).GetNestedType("NuGetLogger",
                System.Reflection.BindingFlags.NonPublic);
            var logAsyncMethod = nugetLoggerType?.GetMethod("LogAsync", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var logger = Activator.CreateInstance(nugetLoggerType, logFactory);
            return logAsyncMethod?.Invoke(logger, new object[] { message });
        }

        private object InvokeGetAspNetCoreRuntimeInfo(string netcoreAppRuntimeAssemblyLocation)
        {
            var method = typeof(ScriptDependencyContextReader).GetMethod("GetAspNetCoreRuntimeInfo",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return method?.Invoke(null, new object[] { netcoreAppRuntimeAssemblyLocation });
        }
    }
}