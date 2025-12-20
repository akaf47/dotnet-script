using NUnit.Framework;
using Moq;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Dotnet.Script.DependencyModel.NuGet;
using Dotnet.Script.DependencyModel.ProjectSystem;

[TestFixture]
public class NuGetSourceReferenceResolverTests
{
    private Mock<SourceReferenceResolver> _mockBaseResolver;
    private Dictionary<string, IReadOnlyList<string>> _scriptMap;
    private NuGetSourceReferenceResolver _resolver;

    [SetUp]
    public void Setup()
    {
        _mockBaseResolver = new Mock<SourceReferenceResolver>();
        _scriptMap = new Dictionary<string, IReadOnlyList<string>>();
        _resolver = new NuGetSourceReferenceResolver(_mockBaseResolver.Object, _scriptMap);
    }

    [Test]
    public void Equals_WithSameObject_ShouldReturnTrue()
    {
        // Arrange & Act
        var result = _resolver.Equals(_resolver);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange & Act
        var result = _resolver.Equals(null);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void NormalizePath_WithNuGetPrefix_ShouldReturnOriginalPath()
    {
        // Arrange
        var path = "nuget:SomePackage";
        var basePath = "/base/path";

        // Act
        var result = _resolver.NormalizePath(path, basePath);

        // Assert
        Assert.AreEqual(path, result);
    }

    [Test]
    public void NormalizePath_WithoutNuGetPrefix_ShouldUseBaseResolver()
    {
        // Arrange
        var path = "/some/path";
        var basePath = "/base/path";
        _mockBaseResolver.Setup(x => x.NormalizePath(path, basePath)).Returns("normalized/path");

        // Act
        var result = _resolver.NormalizePath(path, basePath);

        // Assert
        Assert.AreEqual("normalized/path", result);
    }

    [Test]
    public void ResolveReference_WithSingleScriptForPackage_ShouldReturnScript()
    {
        // Arrange
        var path = "nuget:SomePackage";
        var basePath = "/base/path";
        _scriptMap["SomePackage"] = new[] { "/script/path" };

        // Act
        var result = _resolver.ResolveReference(path, basePath);

        // Assert
        Assert.AreEqual("/script/path", result);
    }

    [Test]
    public void ResolveReference_WithMultipleScripts_ShouldReturnOriginalPath()
    {
        // Arrange
        var path = "nuget:SomePackage";
        var basePath = "/base/path";
        _scriptMap["SomePackage"] = new[] { "/script1", "/script2" };

        // Act
        var result = _resolver.ResolveReference(path, basePath);

        // Assert
        Assert.AreEqual(path, result);
    }

    [Test]
    public void OpenRead_WithSingleScript_ShouldUseBaseResolverStream()
    {
        // Arrange
        var resolvedPath = "nuget:SomePackage";
        _scriptMap["SomePackage"] = new[] { "/script/path" };
        var mockStream = new MemoryStream();
        _mockBaseResolver.Setup(x => x.OpenRead(resolvedPath)).Returns(mockStream);

        // Act
        var result = _resolver.OpenRead(resolvedPath);

        // Assert
        Assert.AreEqual(mockStream, result);
    }

    [Test]
    public void OpenRead_WithMultipleScripts_ShouldReturnMemoryStreamWithLoadStatements()
    {
        // Arrange
        var resolvedPath = "nuget:SomePackage";
        _scriptMap["SomePackage"] = new[] { "/script1", "/script2" };

        // Act
        var result = _resolver.OpenRead(resolvedPath);

        // Assert
        Assert.IsInstanceOf<MemoryStream>(result);
        using (var reader = new StreamReader(result))
        {
            var content = reader.ReadToEnd();
            Assert.IsTrue(content.Contains("#load \"/script1\""));
            Assert.IsTrue(content.Contains("#load \"/script2\""));
        }
    }
}