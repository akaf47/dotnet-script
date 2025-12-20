using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Dotnet.Script.DependencyModel.ProjectSystem;

[TestFixture]
public class ScriptFilesResolverTests
{
    private ScriptFilesResolver _resolver;

    [SetUp]
    public void Setup()
    {
        _resolver = new ScriptFilesResolver();
    }

    [Test]
    public void GetScriptFiles_WithSingleFile_ReturnsFile()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        
        try 
        {
            File.WriteAllText(tempFile, string.Empty);

            // Act
            var result = _resolver.GetScriptFiles(tempFile);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Contains(tempFile));
        }
        finally 
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public void GetScriptFilesFromCode_WithLoadDirectives_ResolvesReferencedScripts()
    {
        // Arrange
        var baseDir = Path.GetTempPath();
        var script1 = Path.Combine(baseDir, "script1.csx");
        var script2 = Path.Combine(baseDir, "script2.csx");
        
        try 
        {
            File.WriteAllText(script1, @"#load ""script2.csx""");
            File.WriteAllText(script2, string.Empty);

            // Act
            var result = _resolver.GetScriptFilesFromCode(File.ReadAllText(script1));

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(script1));
            Assert.IsTrue(result.Contains(script2));
        }
        finally 
        {
            File.Delete(script1);
            File.Delete(script2);
        }
    }

    [Test]
    public void GetLoadDirectives_WithNugetDirective_SkipsNugetReferences()
    {
        // Arrange
        string content = @"#load ""nuget:SomePackage""
                           #load ""localscript.csx""";

        // Act
        var result = (string[])typeof(ScriptFilesResolver)
            .GetMethod("GetLoadDirectives", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Invoke(null, new object[] { content });

        // Assert
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual("localscript.csx", result[0]);
    }

    [Test]
    public void GetScriptFiles_WithRecursiveLoadDirectives_ResolvesAllScripts()
    {
        // Arrange
        var baseDir = Path.GetTempPath();
        var script1 = Path.Combine(baseDir, "script1.csx");
        var script2 = Path.Combine(baseDir, "script2.csx");
        var script3 = Path.Combine(baseDir, "script3.csx");
        
        try 
        {
            File.WriteAllText(script1, @"#load ""script2.csx""");
            File.WriteAllText(script2, @"#load ""script3.csx""");
            File.WriteAllText(script3, string.Empty);

            // Act
            var result = _resolver.GetScriptFiles(script1);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Contains(script1));
            Assert.IsTrue(result.Contains(script2));
            Assert.IsTrue(result.Contains(script3));
        }
        finally 
        {
            File.Delete(script1);
            File.Delete(script2);
            File.Delete(script3);
        }
    }
}