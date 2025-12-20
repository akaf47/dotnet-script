using System;
using System.Linq;
using Dotnet.Script.DependencyModel.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace Dotnet.Script.Core.Tests
{
    [TestFixture]
    public class ScriptContextTests
    {
        [Test]
        public void Constructor_WithAllParameters_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var code = SourceText.From("test script");
            var workingDirectory = "/test/directory";
            var args = new[] { "arg1", "arg2" };
            var filePath = "/test/script.csx";
            var optimizationLevel = OptimizationLevel.Release;
            var scriptMode = ScriptMode.Eval;
            var packageSources = new[] { "https://nuget.org/index.json" };

            // Act
            var context = new ScriptContext(
                code, 
                workingDirectory, 
                args, 
                filePath, 
                optimizationLevel, 
                scriptMode, 
                packageSources
            );

            // Assert
            Assert.AreEqual(code, context.Code);
            Assert.AreEqual(workingDirectory, context.WorkingDirectory);
            Assert.AreEqual(2, context.Args.Count);
            Assert.AreEqual("arg1", context.Args[0]);
            Assert.AreEqual("arg2", context.Args[1]);
            Assert.AreEqual(filePath, context.FilePath);
            Assert.AreEqual(optimizationLevel, context.OptimizationLevel);
            Assert.AreEqual(ScriptMode.Script, context.ScriptMode); // Note: FilePath changes ScriptMode
            Assert.AreEqual(1, context.PackageSources.Length);
            Assert.AreEqual("https://nuget.org/index.json", context.PackageSources[0]);
        }

        [Test]
        public void Constructor_WithNullArgs_ShouldInitializeEmptyArgs()
        {
            // Arrange
            var code = SourceText.From("test script");
            var workingDirectory = "/test/directory";

            // Act
            var context = new ScriptContext(code, workingDirectory, null);

            // Assert
            Assert.IsNotNull(context.Args);
            Assert.AreEqual(0, context.Args.Count);
        }

        [Test]
        public void Constructor_WithNullPackageSources_ShouldInitializeEmptyPackageSources()
        {
            // Arrange
            var code = SourceText.From("test script");
            var workingDirectory = "/test/directory";

            // Act
            var context = new ScriptContext(code, workingDirectory, null);

            // Assert
            Assert.IsNotNull(context.PackageSources);
            Assert.AreEqual(0, context.PackageSources.Length);
        }

        [Test]
        public void Constructor_WithNoFilePath_ShouldUseProvidedScriptMode()
        {
            // Arrange
            var code = SourceText.From("test script");
            var workingDirectory = "/test/directory";
            var scriptMode = ScriptMode.Eval;

            // Act
            var context = new ScriptContext(
                code, 
                workingDirectory, 
                null, 
                null, 
                OptimizationLevel.Debug, 
                scriptMode
            );

            // Assert
            Assert.AreEqual(scriptMode, context.ScriptMode);
        }
    }
}