using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Dotnet.Script.DependencyModel.Logging;
using Moq;

[TestFixture]
public class ScriptParserTests
{
    private Mock<LogFactory> _mockLogFactory;
    private Mock<Logger> _mockLogger;
    private ScriptParser _scriptParser;

    [SetUp]
    public void Setup()
    {
        _mockLogFactory = new Mock<LogFactory>();
        _mockLogger = new Mock<Logger>();
        _mockLogFactory.Setup(x => x.CreateLogger<ScriptParser>()).Returns(_mockLogger.Object);
        _scriptParser = new ScriptParser(_mockLogFactory.Object);
    }

    [Test]
    public void ParseFromCode_WithPackageReferences_ReturnsCorrectParseResult()
    {
        // Arrange
        string code = @"#r ""nuget:Newtonsoft.Json,12.0.3""
                        #r ""nuget:System.Text.Json,5.0.0""
                        #load ""nuget:Microsoft.Extensions.Logging,5.0.0""";

        // Act
        var result = _scriptParser.ParseFromCode(code);

        // Assert
        Assert.AreEqual(3, result.PackageReferences.Count);
        Assert.IsTrue(result.PackageReferences.Contains(new PackageReference("Newtonsoft.Json", "12.0.3")));
        Assert.IsTrue(result.PackageReferences.Contains(new PackageReference("System.Text.Json", "5.0.0")));
        Assert.IsTrue(result.PackageReferences.Contains(new PackageReference("Microsoft.Extensions.Logging", "5.0.0")));
        Assert.AreEqual(string.Empty, result.Sdk);
    }

    [Test]
    public void ParseFromCode_WithNoPackageReferences_ReturnsEmptyResult()
    {
        // Arrange
        string code = "// No references";

        // Act
        var result = _scriptParser.ParseFromCode(code);

        // Assert
        Assert.AreEqual(0, result.PackageReferences.Count);
        Assert.AreEqual(string.Empty, result.Sdk);
    }

    [Test]
    public void ParseFromCode_WithSdkReference_ReturnsSdk()
    {
        // Arrange
        string code = @"#r ""Microsoft.NET.Sdk.Web""";

        // Act
        var result = _scriptParser.ParseFromCode(code);

        // Assert
        Assert.AreEqual("Microsoft.NET.Sdk.Web", result.Sdk);
    }

    [Test]
    public void ParseFromCode_WithUnsupportedSdk_ThrowsNotSupportedException()
    {
        // Arrange
        string code = @"#r ""Unsupported.Sdk""";

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => _scriptParser.ParseFromCode(code));
    }

    [Test]
    public void ParseFromFiles_WithMultipleFiles_ReturnsAllPackageReferences()
    {
        // Arrange
        var tempFile1 = Path.GetTempFileName();
        var tempFile2 = Path.GetTempFileName();
        
        try 
        {
            File.WriteAllText(tempFile1, @"#r ""nuget:Newtonsoft.Json,12.0.3""");
            File.WriteAllText(tempFile2, @"#r ""nuget:System.Text.Json,5.0.0""
                                           #load ""nuget:Microsoft.Extensions.Logging,5.0.0""
                                           #r ""Microsoft.NET.Sdk.Web""");

            // Act
            var result = _scriptParser.ParseFromFiles(new[] { tempFile1, tempFile2 });

            // Assert
            Assert.AreEqual(3, result.PackageReferences.Count);
            Assert.IsTrue(result.PackageReferences.Contains(new PackageReference("Newtonsoft.Json", "12.0.3")));
            Assert.IsTrue(result.PackageReferences.Contains(new PackageReference("System.Text.Json", "5.0.0")));
            Assert.IsTrue(result.PackageReferences.Contains(new PackageReference("Microsoft.Extensions.Logging", "5.0.0")));
            Assert.AreEqual("Microsoft.NET.Sdk.Web", result.Sdk);
        }
        finally 
        {
            File.Delete(tempFile1);
            File.Delete(tempFile2);
        }
    }

    [Test]
    public void ParseFromFiles_WithEmptyFiles_ReturnsEmptyResult()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        
        try 
        {
            File.WriteAllText(tempFile, string.Empty);

            // Act
            var result = _scriptParser.ParseFromFiles(new[] { tempFile });

            // Assert
            Assert.AreEqual(0, result.PackageReferences.Count);
            Assert.AreEqual(string.Empty, result.Sdk);
        }
        finally 
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public void ParseFromFiles_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), "non_existent_file.csx");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => _scriptParser.ParseFromFiles(new[] { nonExistentFile }));
    }

    [Test]
    public void ParseFromCode_WithMixedDirectives_CorrectlyParsesAllReferences()
    {
        // Arrange
        string code = @"#r ""nuget:Newtonsoft.Json,12.0.3""
                        #load ""nuget:Microsoft.Extensions.Logging,5.0.0""
                        #r ""nuget:System.Text.Json,5.0.0""";

        // Act
        var result = _scriptParser.ParseFromCode(code);

        // Assert
        Assert.AreEqual(3, result.PackageReferences.Count);
        Assert.IsTrue(result.PackageReferences.Contains(new PackageReference("Newtonsoft.Json", "12.0.3")));
        Assert.IsTrue(result.PackageReferences.Contains(new PackageReference("System.Text.Json", "5.0.0")));
        Assert.IsTrue(result.PackageReferences.Contains(new PackageReference("Microsoft.Extensions.Logging", "5.0.0")));
    }
}