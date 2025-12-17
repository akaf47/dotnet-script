using System;
using System.Reflection;
using Xunit;
using Dotnet.Script.Core;
using System.Runtime.Loader;

public class ScriptAssemblyLoadContextTests
{
    [Fact]
    public void Constructor_Default_ShouldInitializeSuccessfully()
    {
        var context = new ScriptAssemblyLoadContext();
        Assert.NotNull(context);
    }

    [Fact]
    public void IsHomogeneousAssembly_MsCorLib_ShouldReturnTrue()
    {
        var context = new ScriptAssemblyLoadContext();
        var assemblyName = new AssemblyName("mscorlib");
        
        bool result = context.IsHomogeneousAssembly(assemblyName);
        
        Assert.True(result);
    }

    [Fact]
    public void IsHomogeneousAssembly_CodeAnalysisScripting_ShouldReturnTrue()
    {
        var context = new ScriptAssemblyLoadContext();
        var assemblyName = new AssemblyName("Microsoft.CodeAnalysis.Scripting");
        
        bool result = context.IsHomogeneousAssembly(assemblyName);
        
        Assert.True(result);
    }

    [Fact]
    public void IsHomogeneousAssembly_OtherAssembly_ShouldReturnFalse()
    {
        var context = new ScriptAssemblyLoadContext();
        var assemblyName = new AssemblyName("SomeOtherAssembly");
        
        bool result = context.IsHomogeneousAssembly(assemblyName);
        
        Assert.False(result);
    }

    [Fact]
    public void Loading_EventHandlerInvoked_ShouldReturnAssembly()
    {
        var context = new ScriptAssemblyLoadContext();
        Assembly expectedAssembly = null;

        context.Loading += (sender, args) => 
        {
            expectedAssembly = typeof(ScriptAssemblyLoadContextTests).Assembly;
            return expectedAssembly;
        };

        var result = context.InvokeLoading(new AssemblyName("TestAssembly"));
        
        Assert.Equal(expectedAssembly, result);
    }

    [Fact]
    public void LoadingUnmanagedDll_EventHandlerInvoked_ShouldReturnIntPtr()
    {
        var context = new ScriptAssemblyLoadContext();
        IntPtr expectedPtr = new IntPtr(42);

        context.LoadingUnmanagedDll += (sender, args) => 
        {
            return expectedPtr;
        };

        var result = context.InvokeLoadingUnmanagedDll("TestDll.dll");
        
        Assert.Equal(expectedPtr, result);
    }
}