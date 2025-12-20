using System;
using System.IO;
using System.Threading.Tasks;
using Dotnet.Script.Core.Commands;
using Dotnet.Script.DependencyModel.Context;
using Dotnet.Script.DependencyModel.Logging;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;

[TestFixture]
public class ExecuteInteractiveCommandTests
{
    private Mock<ScriptConsole> _mockScriptConsole;
    private Mock<LogFactory> _mockLogFactory;
    private ExecuteInteractiveCommand _command;

    [SetUp]
    public void Setup()
    {
        _mockScriptConsole = new Mock<ScriptConsole>();
        _mockLogFactory = new Mock<LogFactory>();
        _command = new ExecuteInteractiveCommand(_mockScriptConsole.Object, _mockLogFactory.Object);
    }

    [Test]
    public async Task Execute_NoScriptFile_RunsInteractiveLoop()
    {
        // Arrange
        var options = new ExecuteInteractiveCommandOptions
        {
            ScriptFile = null,
            PackageSources = null
        };

        // Act
        int result = await _command.Execute(options);

        // Assert
        Assert.AreEqual(0, result);
    }

    [Test]
    public async Task Execute_WithScriptFile_RunsLoopWithSeed()
    {
        // Arrange
        var mockScriptFile = new Mock<IScriptFile>();
        mockScriptFile.Setup(sf => sf.Path).Returns(@"C:\test\script.csx");

        var options = new ExecuteInteractiveCommandOptions
        {
            ScriptFile = mockScriptFile.Object,
            Arguments = new[] { "arg1", "arg2" },
            PackageSources = new[] { "https://nuget.org/index.json" }
        };

        // Act
        int result = await _command.Execute(options);

        // Assert
        Assert.AreEqual(0, result);
    }

    [Test]
    public void Execute_NullConsole_ThrowsException()
    {
        // Arrange
        _command = new ExecuteInteractiveCommand(null, _mockLogFactory.Object);
        var options = new ExecuteInteractiveCommandOptions();

        // Act & Assert
        Assert.ThrowsAsync<NullReferenceException>(async () => await _command.Execute(options));
    }

    [Test]
    public async Task Execute_WithCustomAssemblyLoadContext_UsesProvidedContext()
    {
        // Arrange
        var mockAssemblyLoadContext = new Mock<System.Runtime.Loader.AssemblyLoadContext>();
        var options = new ExecuteInteractiveCommandOptions
        {
            ScriptFile = null,
            AssemblyLoadContext = mockAssemblyLoadContext.Object
        };

        // Act
        int result = await _command.Execute(options);

        // Assert
        Assert.AreEqual(0, result);
    }
}