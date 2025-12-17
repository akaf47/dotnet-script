using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.DependencyModel.Runtime;
using Microsoft.CodeAnalysis.Scripting;
using Moq;

public class ScriptRunnerTests
{
    private Mock<ScriptCompiler> _mockScriptCompiler;
    private Mock<LogFactory> _mockLogFactory;
    private Mock<ScriptConsole> _mockScriptConsole;

    public ScriptRunnerTests()
    {
        _mockScriptCompiler = new Mock<ScriptCompiler>();
        _mockLogFactory = new Mock<LogFactory>();
        _mockScriptConsole = new Mock<ScriptConsole>();
    }

    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Act
        var runner = CreateScriptRunner();

        // Assert
        Assert.NotNull(runner);
    }

    [Fact]
    public async Task Execute_WithDllPath_ShouldRunScriptSuccessfully()
    {
        // Arrange
        var runner = CreateScriptRunner();
        var dllPath = "/test/script.dll";
        var args = new[] { "arg1", "arg2" };
        
        var mockAssembly = CreateMockAssembly();
        var mockLoadPal = CreateMockAssemblyLoadPal(mockAssembly);
        var mockRuntimeDeps = CreateMockRuntimeDependencies();

        SetupMockScriptCompiler(dllPath, mockRuntimeDeps);

        // Act
        var result = await runner.Execute<string>(dllPath, args);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Execute_WithScriptContext_ShouldRunScriptSuccessfully()
    {
        // Arrange
        var runner = CreateScriptRunner();
        var context = CreateMockScriptContext();
        
        var mockCompilationContext = CreateMockCompilationContext<string>();

        _mockScriptCompiler
            .Setup(x => x.CreateCompilationContext<string, CommandLineScriptGlobals>(context))
            .Returns(mockCompilationContext);

        // Act
        var result = await runner.Execute<string>(context);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Execute_WithCompilationContext_ShouldRunScriptSuccessfully()
    {
        // Arrange
        var runner = CreateScriptRunner();
        var mockCompilationContext = CreateMockCompilationContext<string>();
        var mockHost = new CommandLineScriptGlobals(null, null);

        // Act
        var result = await runner.Execute(mockCompilationContext, mockHost);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void ResolveAssembly_ExistingAssembly_ShouldReturnLoadedAssembly()
    {
        // Arrange
        var runner = CreateScriptRunner();
        var mockLoadPal = new Mock<AssemblyLoadPal>();
        var assemblyName = new AssemblyName("TestAssembly");
        var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>
        {
            { "TestAssembly", new RuntimeAssembly { Path = "/test/assembly.dll" } }
        };

        mockLoadPal
            .Setup(x => x.LoadFrom("/test/assembly.dll"))
            .Returns(Assembly.GetExecutingAssembly());

        // Act
        var result = runner.ResolveAssembly(mockLoadPal.Object, assemblyName, runtimeDepsMap);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void ResolveAssembly_NonExistingAssembly_ShouldReturnNull()
    {
        // Arrange
        var runner = CreateScriptRunner();
        var mockLoadPal = new Mock<AssemblyLoadPal>();
        var assemblyName = new AssemblyName("UnknownAssembly");
        var runtimeDepsMap = new Dictionary<string, RuntimeAssembly>();

        // Act
        var result = runner.ResolveAssembly(mockLoadPal.Object, assemblyName, runtimeDepsMap);

        // Assert
        Assert.Null(result);
    }

    private ScriptRunner CreateScriptRunner()
    {
        return new ScriptRunner(_mockScriptCompiler.Object, _mockLogFactory.Object, _mockScriptConsole.Object);
    }

    private void SetupMockScriptCompiler(string dllPath, IEnumerable<RuntimeDependency> runtimeDeps)
    {
        _mockScriptCompiler
            .Setup(x => x.RuntimeDependencyResolver.GetDependenciesForLibrary(dllPath))
            .Returns(runtimeDeps);

        _mockScriptCompiler
            .Setup(x => x.CreateScriptDependenciesMap(It.IsAny<IEnumerable<RuntimeDependency>>()))
            .Returns(new Dictionary<string, RuntimeAssembly>());
    }

    private Assembly CreateMockAssembly()
    {
        var mockType = new Mock<Type>();
        mockType
            .Setup(x => x.GetMethod("<Factory>", BindingFlags.Static | BindingFlags.Public))
            .Returns(CreateMockFactoryMethod());

        var mockAssembly = new Mock<Assembly>();
        mockAssembly
            .Setup(x => x.GetType("Submission#0"))
            .Returns(mockType.Object);

        return mockAssembly.Object;
    }

    private MethodInfo CreateMockFactoryMethod()
    {
        var mockMethod = new Mock<MethodInfo>();
        mockMethod
            .Setup(x => x.Invoke(null, It.IsAny<object[]>()))
            .Returns(Task.FromResult("Test Result"));

        return mockMethod.Object;
    }

    private AssemblyLoadPal CreateMockAssemblyLoadPal(Assembly assembly)
    {
        var mockLoadPal = new Mock<AssemblyLoadPal>();
        mockLoadPal
            .Setup(x => x.LoadFrom(It.IsAny<string>()))
            .Returns(assembly);

        return mockLoadPal.Object;
    }

    private IEnumerable<RuntimeDependency> CreateMockRuntimeDependencies()
    {
        return new[]
        {
            new RuntimeDependency 
            { 
                Name = "TestDependency", 
                Path = "/test/dependency.dll" 
            }
        };
    }

    private ScriptContext CreateMockScriptContext()
    {
        return new ScriptContext(
            Microsoft.CodeAnalysis.Text.SourceText.From("test code"), 
            "/test/dir", 
            new[] { "arg1" }
        );
    }

    private ScriptCompilationContext<T> CreateMockCompilationContext<T>()
    {
        var mockScript = new Mock<Script<T>>();
        mockScript
            .Setup(x => x.RunAsync(It.IsAny<object>(), It.IsAny<Func<Exception, bool>>()))
            .ReturnsAsync(CreateMockScriptState<T>());

        var mockContext = new Mock<ScriptCompilationContext<T>>();
        mockContext
            .SetupGet(x => x.Script)
            .Returns(mockScript.Object);

        return mockContext.Object;
    }

    private ScriptState<T> CreateMockScriptState<T>()
    {
        var mockState = new Mock<ScriptState<T>>();
        mockState
            .SetupGet(x => x.ReturnValue)
            .Returns(default(T));
        mockState
            .SetupGet(x => x.Exception)
            .Returns((Exception)null);

        return mockState.Object;
    }
}