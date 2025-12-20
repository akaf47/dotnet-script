using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using Microsoft.CodeAnalysis;
using Dotnet.Script.DependencyModel.NuGet;
using Dotnet.Script.DependencyModel.ProjectSystem;

[TestFixture]
public class NuGetSourceReferenceResolverTests
{
    private Mock<SourceReferenceResolver> _mockInnerResolver;
    private IDictionary<string, IReadOnlyList<string>> _scriptMap;

    [SetUp]
    public void Setup()
    {
        _mockInnerResolver = new Mock<SourceReferenceResolver>();
        _scriptMap = new Dictionary<string, IReadOnlyList<string>>();
    }

    [Test]
    public void Equals_ComparedWithNull_ReturnsFalse()
    {
        // Arrange
        var resolver = new NuGetSourceReferenceResolver(_mockInnerResolver.Object, _scriptMap);

        // Act
        bool result = resolver.Equals(null);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_ComparedWithSameReference_ReturnsTrue()
    {
        // Arrange
        var resolver = new NuGetSourceReferenceResolver(_mockInnerResolver.Object, _scriptMap);

        // Act
        bool result = resolver.Equals(resolver);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_ComparedWithDifferentType_ReturnsFalse()
    {
        // Arrange
        var resolver = new NuGetSourceReferenceResolver(_mockInnerResolver.Object, _scriptMap);
        var otherObject = new object();

        // Act
        bool result = resolver.Equals(otherObject);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void NormalizePath_NuGetPath_ReturnsSamePath()
    {
        // Arrange
        var resolver = new NuGetSourceReferenceResolver(_mockInnerResolver.Object, _scriptMap);
        string nugetPath = "nuget:package/path";
        string baseFilePath = "/base/path";

        // Act
        string result = resolver.NormalizePath(nugetPath, baseFilePath);

        // Assert
        Assert.AreEqual(nugetPath, result);
    }

    [Test]
    public void NormalizePath_NonNuGetPath_CallsInnerResolver()
    {
        // Arrange
        string path = "/some/path";
        string baseFilePath = "/base/path";
        _mockInnerResolver
            .Setup(x => x.NormalizePath(path, baseFilePath))
            .Returns("/normalized/path");

        var resolver = new NuGetSourceReferenceResolver(_mockInnerResolver.Object, _scriptMap);

        // Act
        string result = resolver.NormalizePath(path, baseFilePath);

        // Assert
        Assert.AreEqual("/normalized/path", result);
        _mockInnerResolver.Verify(x => x.NormalizePath(path, baseFilePath), Times.Once);
    }

    [Test]
    public void ResolveReference_NuGetPackageWithSingleScript_ReturnsScript()
    {
        // Arrange
        _scriptMap["TestPackage"] = new List<string> { "/script/path/test.csx" };
        var resolver = new NuGetSourceReferenceResolver(_mockInnerResolver.Object, _scriptMap);
        string path = "nuget:TestPackage";
        string baseFilePath = "/base/path";

        // Act
        string result = resolver.ResolveReference(path, baseFilePath);

        // Assert
        Assert.AreEqual("/script/path/test.csx", result);
    }

    [Test]
    public void ResolveReference_NuGetPackageWithMultipleScripts_ReturnsOriginalPath()
    {
        // Arrange
        _scriptMap["TestPackage"] = new List<string> 
        { 
            "/script/path1/test1.csx", 
            "/script/path2/test2.csx" 
        };
        var resolver = new NuGetSourceReferenceResolver(_mockInnerResolver.Object, _scriptMap);
        string path = "nuget:TestPackage";
        string baseFilePath = "/base/path";

        // Act
        string result = resolver.ResolveReference(path, baseFilePath);

        // Assert
        Assert.AreEqual(path, result);
    }

    [Test]
    public void ResolveReference_NonNuGetPackage_CallsInnerResolver()
    {
        // Arrange
        string path = "/some/path";
        string baseFilePath = "/base/path";
        _mockInnerResolver
            .Setup(x => x.ResolveReference(path, baseFilePath))
            .Returns("/resolved/path");

        var resolver = new NuGetSourceReferenceResolver(_mockInnerResolver.Object, _scriptMap);

        // Act
        string result = resolver.ResolveReference(path, baseFilePath);

        // Assert
        Assert.AreEqual("/resolved/path", result);
        _mockInnerResolver.Verify(x => x.ResolveReference(path, baseFilePath), Times.Once);
    }

    [Test]
    public void OpenRead_NuGetPackageWithSingleScript_CallsInnerResolverOpenRead()
    {
        // Arrange
        _scriptMap["TestPackage"] = new List<string> { "/script/path/test.csx" };
        _mockInnerResolver
            .Setup(x => x.OpenRead("/script/path/test.csx"))
            .Returns(new MemoryStream());

        var resolver = new NuGetSourceReferenceResolver(_mockInnerResolver.Object, _scriptMap);
        string resolvedPath = "nuget:TestPackage";

        // Act
        Stream result = resolver.OpenRead(resolvedPath);

        // Assert
        Assert.IsNotNull(result);
        _mockInnerResolver.Verify(x => x.OpenRead("/script/path/test.csx"), Times.Once);
    }

    [Test]
    public void OpenRead_NuGetPackageWithMultipleScripts_ReturnsMemoryStreamWithLoadStatements()
    {
        // Arrange
        _scriptMap["TestPackage"] = new List<string> 
        { 
            "/script/path1/test1.csx", 
            "/script/path2/test2.csx" 
        };

        var resolver = new NuGetSourceReferenceResolver(_mockInnerResolver.Object, _scriptMap);
        string resolvedPath = "nuget:TestPackage";

        // Act
        Stream result = resolver.OpenRead(resolvedPath);

        // Assert
        Assert.IsNotNull(result);
        using (var reader = new StreamReader(result))
        {
            var content = reader.ReadToEnd();
            Assert.IsTrue(content.Contains("#load \"/script/path1/test1.csx\""));
            Assert.IsTrue(content.Contains("#load \"/script/path2/test2.csx\""));
        }
    }

    [Test]
    public void OpenRead_NonNuGetPackage_CallsInnerResolverOpenRead()
    {
        // Arrange
        string resolvedPath = "/some/path";
        _mockInnerResolver
            .Setup(x => x.OpenRead(resolvedPath))
            .Returns(new MemoryStream());

        var resolver = new NuGetSourceReferenceResolver(_mockInnerResolver.Object, _scriptMap);

        // Act
        Stream result = resolver.OpenRead(resolvedPath);

        // Assert
        Assert.IsNotNull(result);
        _mockInnerResolver.Verify(x => x.OpenRead(resolvedPath), Times.Once);
    }
}