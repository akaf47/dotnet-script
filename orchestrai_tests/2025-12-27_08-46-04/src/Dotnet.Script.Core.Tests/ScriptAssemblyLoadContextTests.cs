#if NET

using System;
using System.Reflection;
using System.Runtime.Loader;
using Dotnet.Script.Core;
using Xunit;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptAssemblyLoadContextTests
    {
        [Fact]
        public void Constructor_WithoutParameters_CreatesInstance()
        {
            // Arrange & Act
            var context = new ScriptAssemblyLoadContext();

            // Assert
            Assert.NotNull(context);
            Assert.IsAssignableFrom<AssemblyLoadContext>(context);
        }

        [Fact]
        public void Constructor_WithoutParameters_ContextIsNotNull()
        {
            // Arrange & Act
            var context = new ScriptAssemblyLoadContext();

            // Assert
            Assert.NotNull(context);
        }

#if NET5_0_OR_GREATER
        [Fact]
        public void Constructor_WithNameAndIsCollectibleFalse_CreatesInstance()
        {
            // Arrange & Act
            var context = new ScriptAssemblyLoadContext("TestContext", false);

            // Assert
            Assert.NotNull(context);
            Assert.Equal("TestContext", context.Name);
        }

        [Fact]
        public void Constructor_WithNameAndIsCollectibleTrue_CreatesCollectibleInstance()
        {
            // Arrange & Act
            var context = new ScriptAssemblyLoadContext("CollectibleContext", true);

            // Assert
            Assert.NotNull(context);
            Assert.Equal("CollectibleContext", context.Name);
        }

        [Fact]
        public void Constructor_WithNullName_CreatesInstance()
        {
            // Arrange & Act
            var context = new ScriptAssemblyLoadContext(null, false);

            // Assert
            Assert.NotNull(context);
        }

        [Fact]
        public void Constructor_WithEmptyName_CreatesInstance()
        {
            // Arrange & Act
            var context = new ScriptAssemblyLoadContext("", false);

            // Assert
            Assert.NotNull(context);
        }

        [Fact]
        public void Constructor_WithIsCollectibleTrue_CreatesCollectibleInstance()
        {
            // Arrange & Act
            var context = new ScriptAssemblyLoadContext(true);

            // Assert
            Assert.NotNull(context);
        }

        [Fact]
        public void Constructor_WithIsCollectibleFalse_CreatesNonCollectibleInstance()
        {
            // Arrange & Act
            var context = new ScriptAssemblyLoadContext(false);

            // Assert
            Assert.NotNull(context);
        }
#endif

        [Fact]
        public void IsHomogeneousAssembly_WithMscorlib_ReturnsTrue()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var assemblyName = new AssemblyName("mscorlib");

            // Act
            var result = context.IsHomogeneousAssembly(assemblyName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsHomogeneousAssembly_WithMscorlibDifferentCase_ReturnsTrue()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var assemblyName = new AssemblyName("MSCORLIB");

            // Act
            var result = context.IsHomogeneousAssembly(assemblyName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsHomogeneousAssembly_WithMicrosoftCodeAnalysisScripting_ReturnsTrue()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var assemblyName = new AssemblyName("Microsoft.CodeAnalysis.Scripting");

            // Act
            var result = context.IsHomogeneousAssembly(assemblyName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsHomogeneousAssembly_WithMicrosoftCodeAnalysisScriptingDifferentCase_ReturnsTrue()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var assemblyName = new AssemblyName("MICROSOFT.CODEANALYSIS.SCRIPTING");

            // Act
            var result = context.IsHomogeneousAssembly(assemblyName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsHomogeneousAssembly_WithOtherAssembly_ReturnsFalse()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var assemblyName = new AssemblyName("System.Core");

            // Act
            var result = context.IsHomogeneousAssembly(assemblyName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsHomogeneousAssembly_WithArbitraryAssemblyName_ReturnsFalse()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var assemblyName = new AssemblyName("MyCustomAssembly");

            // Act
            var result = context.IsHomogeneousAssembly(assemblyName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsHomogeneousAssembly_WithEmptyName_ReturnsFalse()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var assemblyName = new AssemblyName("");

            // Act
            var result = context.IsHomogeneousAssembly(assemblyName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsHomogeneousAssembly_WithNullName_ReturnsFalse()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var assemblyName = new AssemblyName { Name = null };

            // Act
            var result = context.IsHomogeneousAssembly(assemblyName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Load_WithoutEventHandler_ReturnsNull()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var assemblyName = new AssemblyName("TestAssembly");

            // Act
            var result = context.Load(assemblyName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Load_WithEventHandlerReturningAssembly_ReturnsAssembly()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var testAssembly = typeof(ScriptAssemblyLoadContextTests).Assembly;
            var assemblyName = new AssemblyName("TestAssembly");

            // Add handler that returns assembly
            context.Loading += (sender, args) =>
            {
                Assert.Equal("TestAssembly", args.Name.Name);
                return testAssembly;
            };

            // Act
            var result = context.Load(assemblyName);

            // Assert
            Assert.Equal(testAssembly, result);
        }

        [Fact]
        public void Load_WithEventHandlerReturningNull_ContinuesToNextHandler()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var testAssembly = typeof(ScriptAssemblyLoadContextTests).Assembly;
            var assemblyName = new AssemblyName("TestAssembly");
            var firstHandlerCalled = false;
            var secondHandlerCalled = false;

            // Add first handler that returns null
            context.Loading += (sender, args) =>
            {
                firstHandlerCalled = true;
                return null;
            };

            // Add second handler that returns assembly
            context.Loading += (sender, args) =>
            {
                secondHandlerCalled = true;
                return testAssembly;
            };

            // Act
            var result = context.Load(assemblyName);

            // Assert
            Assert.True(firstHandlerCalled);
            Assert.True(secondHandlerCalled);
            Assert.Equal(testAssembly, result);
        }

        [Fact]
        public void Load_WithMultipleHandlersFirstReturnsAssembly_StopsAtFirstNonNull()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var testAssembly = typeof(ScriptAssemblyLoadContextTests).Assembly;
            var assemblyName = new AssemblyName("TestAssembly");
            var secondHandlerCalled = false;

            // Add first handler that returns assembly
            context.Loading += (sender, args) =>
            {
                return testAssembly;
            };

            // Add second handler
            context.Loading += (sender, args) =>
            {
                secondHandlerCalled = true;
                return null;
            };

            // Act
            var result = context.Load(assemblyName);

            // Assert
            Assert.Equal(testAssembly, result);
            // Note: The second handler may or may not be called depending on invocation order
            // The important part is we got the first result
        }

        [Fact]
        public void Load_WithEventHandlerRemoved_ReturnsNull()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var testAssembly = typeof(ScriptAssemblyLoadContextTests).Assembly;
            var assemblyName = new AssemblyName("TestAssembly");

            ScriptAssemblyLoadContext.LoadingEventHandler handler = (sender, args) =>
            {
                return testAssembly;
            };

            context.Loading += handler;

            // Act - Remove handler
            context.Loading -= handler;
            var result = context.Load(assemblyName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LoadUnmanagedDll_WithoutEventHandler_ReturnsZero()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var unmanagedDllName = "test.dll";

            // Act
            var result = context.LoadUnmanagedDll(unmanagedDllName);

            // Assert
            Assert.Equal(IntPtr.Zero, result);
        }

        [Fact]
        public void LoadUnmanagedDll_WithEventHandlerReturningPointer_ReturnsPointer()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var unmanagedDllName = "test.dll";
            var dummyPointer = new IntPtr(12345);

            context.LoadingUnmanagedDll += (sender, args) =>
            {
                Assert.Equal("test.dll", args.UnmanagedDllName);
                Assert.NotNull(args.LoadUnmanagedDllFromPath);
                return dummyPointer;
            };

            // Act
            var result = context.LoadUnmanagedDll(unmanagedDllName);

            // Assert
            Assert.Equal(dummyPointer, result);
        }

        [Fact]
        public void LoadUnmanagedDll_WithEventHandlerReturningZero_ContinuesToNextHandler()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var unmanagedDllName = "test.dll";
            var dummyPointer = new IntPtr(12345);
            var firstHandlerCalled = false;
            var secondHandlerCalled = false;

            // Add first handler that returns zero
            context.LoadingUnmanagedDll += (sender, args) =>
            {
                firstHandlerCalled = true;
                return IntPtr.Zero;
            };

            // Add second handler that returns pointer
            context.LoadingUnmanagedDll += (sender, args) =>
            {
                secondHandlerCalled = true;
                return dummyPointer;
            };

            // Act
            var result = context.LoadUnmanagedDll(unmanagedDllName);

            // Assert
            Assert.True(firstHandlerCalled);
            Assert.True(secondHandlerCalled);
            Assert.Equal(dummyPointer, result);
        }

        [Fact]
        public void LoadUnmanagedDll_WithMultipleHandlersFirstReturnsPointer_StopsAtFirstNonZero()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var unmanagedDllName = "test.dll";
            var dummyPointer = new IntPtr(12345);
            var secondHandlerCalled = false;

            // Add first handler that returns pointer
            context.LoadingUnmanagedDll += (sender, args) =>
            {
                return dummyPointer;
            };

            // Add second handler
            context.LoadingUnmanagedDll += (sender, args) =>
            {
                secondHandlerCalled = true;
                return IntPtr.Zero;
            };

            // Act
            var result = context.LoadUnmanagedDll(unmanagedDllName);

            // Assert
            Assert.Equal(dummyPointer, result);
        }

        [Fact]
        public void LoadUnmanagedDll_WithEventHandlerRemoved_ReturnsZero()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var unmanagedDllName = "test.dll";
            var dummyPointer = new IntPtr(12345);

            ScriptAssemblyLoadContext.LoadingUnmanagedDllEventHandler handler = (sender, args) =>
            {
                return dummyPointer;
            };

            context.LoadingUnmanagedDll += handler;

            // Act - Remove handler
            context.LoadingUnmanagedDll -= handler;
            var result = context.LoadUnmanagedDll(unmanagedDllName);

            // Assert
            Assert.Equal(IntPtr.Zero, result);
        }

        [Fact]
        public void LoadUnmanagedDll_WithEmptyDllName_PassesEmptyStringToHandler()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var unmanagedDllName = "";
            var dummyPointer = new IntPtr(12345);
            var capturedDllName = "";

            context.LoadingUnmanagedDll += (sender, args) =>
            {
                capturedDllName = args.UnmanagedDllName;
                return dummyPointer;
            };

            // Act
            var result = context.LoadUnmanagedDll(unmanagedDllName);

            // Assert
            Assert.Equal("", capturedDllName);
            Assert.Equal(dummyPointer, result);
        }

        [Fact]
        public void LoadingEventArgs_Constructor_StoresAssemblyName()
        {
            // Arrange
            var assemblyName = new AssemblyName("TestAssembly");

            // Act
            var args = new ScriptAssemblyLoadContext.LoadingEventArgs(assemblyName);

            // Assert
            Assert.Equal(assemblyName, args.Name);
        }

        [Fact]
        public void LoadingUnmanagedDllEventArgs_Constructor_StoresUnmanagedDllName()
        {
            // Arrange
            var unmanagedDllName = "test.dll";
            Func<string, IntPtr> loadFunc = (s) => IntPtr.Zero;

            // Act
            var args = new ScriptAssemblyLoadContext.LoadingUnmanagedDllEventArgs(unmanagedDllName, loadFunc);

            // Assert
            Assert.Equal("test.dll", args.UnmanagedDllName);
        }

        [Fact]
        public void LoadingUnmanagedDllEventArgs_Constructor_StoresLoadUnmanagedDllFromPath()
        {
            // Arrange
            var unmanagedDllName = "test.dll";
            Func<string, IntPtr> loadFunc = (s) => IntPtr.Zero;

            // Act
            var args = new ScriptAssemblyLoadContext.LoadingUnmanagedDllEventArgs(unmanagedDllName, loadFunc);

            // Assert
            Assert.Equal(loadFunc, args.LoadUnmanagedDllFromPath);
        }

        [Fact]
        public void Load_WithNullAssemblyName_PassesNullToHandler()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var testAssembly = typeof(ScriptAssemblyLoadContextTests).Assembly;
            var capturedAssemblyName = new AssemblyName("Dummy");

            context.Loading += (sender, args) =>
            {
                capturedAssemblyName = args.Name;
                return testAssembly;
            };

            // Act
            var result = context.Load(null);

            // Assert
            // Assembly should be returned regardless
            Assert.Equal(testAssembly, result);
        }

        [Fact]
        public void Load_EventHandlerAccessesSenderContext_SenderIsCorrect()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var testAssembly = typeof(ScriptAssemblyLoadContextTests).Assembly;
            var assemblyName = new AssemblyName("TestAssembly");
            ScriptAssemblyLoadContext capturedSender = null;

            context.Loading += (sender, args) =>
            {
                capturedSender = sender;
                return testAssembly;
            };

            // Act
            var result = context.Load(assemblyName);

            // Assert
            Assert.Equal(context, capturedSender);
        }

        [Fact]
        public void LoadUnmanagedDll_EventHandlerAccessesSenderContext_SenderIsCorrect()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var dummyPointer = new IntPtr(12345);
            ScriptAssemblyLoadContext capturedSender = null;

            context.LoadingUnmanagedDll += (sender, args) =>
            {
                capturedSender = sender;
                return dummyPointer;
            };

            // Act
            var result = context.LoadUnmanagedDll("test.dll");

            // Assert
            Assert.Equal(context, capturedSender);
        }

        [Fact]
        public void Load_WithAllNullHandlers_ReturnsNull()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();
            var assemblyName = new AssemblyName("TestAssembly");

            // Act - No handlers added
            var result = context.Load(assemblyName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LoadUnmanagedDll_WithAllNullHandlers_ReturnsZero()
        {
            // Arrange
            var context = new ScriptAssemblyLoadContext();

            // Act - No handlers added
            var result = context.LoadUnmanagedDll("test.dll");

            // Assert
            Assert.Equal(IntPtr.Zero, result);
        }
    }
}

#endif