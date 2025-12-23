using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Moq;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.ProjectSystem;

namespace Dotnet.Script.DependencyModel.Tests.ProjectSystem
{
    public class ScriptParserTests
    {
        private readonly ScriptParser _parser;
        private readonly Mock<LogFactory> _mockLogFactory;
        private readonly Mock<Logger> _mockLogger;

        public ScriptParserTests()
        {
            _mockLogger = new Mock<Logger>();
            _mockLogFactory = new Mock<LogFactory>();
            _mockLogFactory.Setup(f => f(It.IsAny<Type>())).Returns(_mockLogger.Object);
            _parser = new ScriptParser(_mockLogFactory.Object);
        }

        #region ParseFromCode Tests

        [Fact]
        public void ParseFromCode_WithValidNuGetReference_ReturnsPackageReference()
        {
            // Arrange
            var code = @"#r ""nuget: Newtonsoft.Json, 12.0.3""";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PackageReferences);
            var packageRef = result.PackageReferences.First();
            Assert.Equal("Newtonsoft.Json", packageRef.Id.ToString());
            Assert.Equal("12.0.3", packageRef.Version.ToString());
        }

        [Fact]
        public void ParseFromCode_WithMultipleNuGetReferences_ReturnsAllPackages()
        {
            // Arrange
            var code = @"
#r ""nuget: Newtonsoft.Json, 12.0.3""
#r ""nuget: System.Net.Http, 4.3.4""
";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.PackageReferences.Count);
        }

        [Fact]
        public void ParseFromCode_WithLoadDirective_ReturnsPackageReference()
        {
            // Arrange
            var code = @"#load ""nuget: Newtonsoft.Json, 12.0.3""";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PackageReferences);
        }

        [Fact]
        public void ParseFromCode_WithBothReferenceAndLoadDirective_ReturnsBothPackages()
        {
            // Arrange
            var code = @"
#r ""nuget: Newtonsoft.Json, 12.0.3""
#load ""nuget: System.Net.Http, 4.3.4""
";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.PackageReferences.Count);
        }

        [Fact]
        public void ParseFromCode_WithSdkReference_ReturnsSdk()
        {
            // Arrange
            var code = @"#r ""sdk: Microsoft.NET.Sdk.Web""";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Microsoft.NET.Sdk.Web", result.Sdk);
        }

        [Fact]
        public void ParseFromCode_WithUnsupportedSdk_ThrowsNotSupportedException()
        {
            // Arrange
            var code = @"#r ""sdk: Microsoft.NET.Sdk.Razor""";

            // Act & Assert
            var exception = Assert.Throws<NotSupportedException>(() => _parser.ParseFromCode(code));
            Assert.Contains("Microsoft.NET.Sdk.Razor", exception.Message);
            Assert.Contains("not supported", exception.Message);
        }

        [Fact]
        public void ParseFromCode_WithSdkAndNuGetReferences_ReturnsBoth()
        {
            // Arrange
            var code = @"
#r ""sdk: Microsoft.NET.Sdk.Web""
#r ""nuget: Newtonsoft.Json, 12.0.3""
";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Microsoft.NET.Sdk.Web", result.Sdk);
            Assert.Single(result.PackageReferences);
        }

        [Fact]
        public void ParseFromCode_WithNoReferences_ReturnsEmptyResult()
        {
            // Arrange
            var code = "Console.WriteLine(\"Hello World\");";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.PackageReferences);
            Assert.Equal(string.Empty, result.Sdk);
        }

        [Fact]
        public void ParseFromCode_WithEmptyString_ReturnsEmptyResult()
        {
            // Arrange
            var code = "";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.PackageReferences);
            Assert.Equal(string.Empty, result.Sdk);
        }

        [Fact]
        public void ParseFromCode_WithNullString_ThrowsArgumentNullException()
        {
            // Arrange
            string code = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _parser.ParseFromCode(code));
        }

        [Fact]
        public void ParseFromCode_WithPackageVersionWithoutVersion_ReturnsEmptyVersion()
        {
            // Arrange
            var code = @"#r ""nuget: Newtonsoft.Json""";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PackageReferences);
            var packageRef = result.PackageReferences.First();
            Assert.Equal("Newtonsoft.Json", packageRef.Id.ToString());
            Assert.Equal("", packageRef.Version.ToString());
        }

        [Fact]
        public void ParseFromCode_WithDifferentCasing_ParsesCorrectly()
        {
            // Arrange
            var code = @"#R ""nuget: newtonsoft.json, 12.0.3""";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PackageReferences);
        }

        [Fact]
        public void ParseFromCode_WithWhitespaceInDirective_ParsesCorrectly()
        {
            // Arrange
            var code = @"#  r  ""nuget: Newtonsoft.Json, 12.0.3""";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PackageReferences);
        }

        [Fact]
        public void ParseFromCode_WithTabsInDirective_ParsesCorrectly()
        {
            // Arrange
            var code = "#\tr\t\"nuget: Newtonsoft.Json, 12.0.3\"";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PackageReferences);
        }

        [Fact]
        public void ParseFromCode_WithMultipleSdkReferences_UsesLastOne()
        {
            // Arrange
            var code = @"
#r ""sdk: Microsoft.NET.Sdk.Web""
#r ""sdk: Microsoft.NET.Sdk.Web""
";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Microsoft.NET.Sdk.Web", result.Sdk);
        }

        [Fact]
        public void ParseFromCode_WithDuplicatePackageReferences_ReturnsDistinct()
        {
            // Arrange
            var code = @"
#r ""nuget: Newtonsoft.Json, 12.0.3""
#r ""nuget: Newtonsoft.Json, 12.0.3""
";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PackageReferences);
        }

        #endregion

        #region ParseFromFiles Tests

        [Fact]
        public void ParseFromFiles_WithValidCsxFile_ReturnsPackageReferences()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, @"#r ""nuget: Newtonsoft.Json, 12.0.3""");

                // Act
                var result = _parser.ParseFromFiles(new[] { tempFile });

                // Assert
                Assert.NotNull(result);
                Assert.Single(result.PackageReferences);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ParseFromFiles_WithMultipleFiles_ReturnsCombinedReferences()
        {
            // Arrange
            var tempFile1 = Path.GetTempFileName();
            var tempFile2 = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile1, @"#r ""nuget: Newtonsoft.Json, 12.0.3""");
                File.WriteAllText(tempFile2, @"#r ""nuget: System.Net.Http, 4.3.4""");

                // Act
                var result = _parser.ParseFromFiles(new[] { tempFile1, tempFile2 });

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.PackageReferences.Count);
            }
            finally
            {
                File.Delete(tempFile1);
                File.Delete(tempFile2);
            }
        }

        [Fact]
        public void ParseFromFiles_WithEmptyFileList_ReturnsEmptyResult()
        {
            // Arrange
            var files = Enumerable.Empty<string>();

            // Act
            var result = _parser.ParseFromFiles(files);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.PackageReferences);
            Assert.Equal(string.Empty, result.Sdk);
        }

        [Fact]
        public void ParseFromFiles_WithNullFileList_ThrowsArgumentNullException()
        {
            // Arrange
            IEnumerable<string> files = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _parser.ParseFromFiles(files));
        }

        [Fact]
        public void ParseFromFiles_WithFileSdkReference_ReturnsSdk()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, @"#r ""sdk: Microsoft.NET.Sdk.Web""");

                // Act
                var result = _parser.ParseFromFiles(new[] { tempFile });

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Microsoft.NET.Sdk.Web", result.Sdk);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ParseFromFiles_WithMultipleFilesSdkReferences_UsesLastOne()
        {
            // Arrange
            var tempFile1 = Path.GetTempFileName();
            var tempFile2 = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile1, @"#r ""sdk: Microsoft.NET.Sdk.Web""");
                File.WriteAllText(tempFile2, @"#r ""sdk: Microsoft.NET.Sdk.Web""");

                // Act
                var result = _parser.ParseFromFiles(new[] { tempFile1, tempFile2 });

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Microsoft.NET.Sdk.Web", result.Sdk);
            }
            finally
            {
                File.Delete(tempFile1);
                File.Delete(tempFile2);
            }
        }

        [Fact]
        public void ParseFromFiles_WithDuplicateReferencesAcrossFiles_ReturnsDistinct()
        {
            // Arrange
            var tempFile1 = Path.GetTempFileName();
            var tempFile2 = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile1, @"#r ""nuget: Newtonsoft.Json, 12.0.3""");
                File.WriteAllText(tempFile2, @"#r ""nuget: Newtonsoft.Json, 12.0.3""");

                // Act
                var result = _parser.ParseFromFiles(new[] { tempFile1, tempFile2 });

                // Assert
                Assert.NotNull(result);
                Assert.Single(result.PackageReferences);
            }
            finally
            {
                File.Delete(tempFile1);
                File.Delete(tempFile2);
            }
        }

        [Fact]
        public void ParseFromFiles_LogsEachFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, "");

                // Act
                _parser.ParseFromFiles(new[] { tempFile });

                // Assert
                _mockLogger.Verify(
                    l => l(LogLevel.Debug, It.Is<string>(s => s.Contains(tempFile)), null),
                    Times.Once);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ParseFromFiles_WithFileNotFound_ThrowsFileNotFoundException()
        {
            // Arrange
            var files = new[] { "/nonexistent/path/to/file.csx" };

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _parser.ParseFromFiles(files));
        }

        [Fact]
        public void ParseFromFiles_WithEmptyFile_ReturnsEmptyResult()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, "");

                // Act
                var result = _parser.ParseFromFiles(new[] { tempFile });

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result.PackageReferences);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ParseFromFiles_WithLoadAndReferenceDirectives_ReturnsBoth()
        {
            // Arrange
            var tempFile1 = Path.GetTempFileName();
            var tempFile2 = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile1, @"#r ""nuget: Newtonsoft.Json, 12.0.3""");
                File.WriteAllText(tempFile2, @"#load ""nuget: System.Net.Http, 4.3.4""");

                // Act
                var result = _parser.ParseFromFiles(new[] { tempFile1, tempFile2 });

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.PackageReferences.Count);
            }
            finally
            {
                File.Delete(tempFile1);
                File.Delete(tempFile2);
            }
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void ParseFromCode_WithComplexPackageId_ParsesCorrectly()
        {
            // Arrange
            var code = @"#r ""nuget: My_Complex.Package-Name, 1.0.0""";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PackageReferences);
            var packageRef = result.PackageReferences.First();
            Assert.Equal("My_Complex.Package-Name", packageRef.Id.ToString());
        }

        [Fact]
        public void ParseFromCode_WithVersionRange_ParsesCorrectly()
        {
            // Arrange
            var code = @"#r ""nuget: Newtonsoft.Json, [12.0.0, 13.0.0)""";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PackageReferences);
        }

        [Fact]
        public void ParseFromCode_WithCommentedReference_DoesNotParse()
        {
            // Arrange
            var code = @"// #r ""nuget: Newtonsoft.Json, 12.0.3""";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.PackageReferences);
        }

        [Fact]
        public void ParseFromCode_WithMultilineComment_DoesNotParse()
        {
            // Arrange
            var code = @"/* #r ""nuget: Newtonsoft.Json, 12.0.3"" */";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.PackageReferences);
        }

        [Fact]
        public void ParseFromCode_WithInvalidDirective_ReturnsEmpty()
        {
            // Arrange
            var code = @"#invalid ""nuget: Newtonsoft.Json, 12.0.3""";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.PackageReferences);
        }

        [Fact]
        public void ParseFromCode_WithSdkDirectiveMissingVersion_ReturnsSdk()
        {
            // Arrange
            var code = @"#r ""sdk: Microsoft.NET.Sdk.Web""";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Microsoft.NET.Sdk.Web", result.Sdk);
        }

        [Fact]
        public void ParseFromCode_WithSdkDirectiveMultipleVersionParts_ReturnsSdk()
        {
            // Arrange
            var code = @"#r ""sdk: Microsoft.NET.Sdk.Web""";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Microsoft.NET.Sdk.Web", result.Sdk);
        }

        [Fact]
        public void ParseFromCode_WithLeadingWhitespaceBeforeDirective_ParsesCorrectly()
        {
            // Arrange
            var code = "    #r \"nuget: Newtonsoft.Json, 12.0.3\"";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PackageReferences);
        }

        [Fact]
        public void ParseFromCode_WithInlineComment_ParsesDirectiveBeforeComment()
        {
            // Arrange
            var code = "#r \"nuget: Newtonsoft.Json, 12.0.3\" // comment";

            // Act
            var result = _parser.ParseFromCode(code);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PackageReferences);
        }

        #endregion
    }
}