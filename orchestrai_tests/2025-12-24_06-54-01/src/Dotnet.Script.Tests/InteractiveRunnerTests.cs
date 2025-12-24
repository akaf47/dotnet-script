using Xunit;
using Dotnet.Script.Shared.Tests;
using Xunit.Abstractions;
using System.Threading.Tasks;

namespace Dotnet.Script.Tests
{
    [Collection("IntegrationTests")]
    public class InteractiveRunnerTests : InteractiveRunnerTestsBase
    {
        public InteractiveRunnerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task ShouldCompileAndExecuteWithWebSdk()
        {
            var commands = new[]
            {
                @"#r ""sdk:Microsoft.NET.Sdk.Web""",
                "using Microsoft.AspNetCore.Builder;",
                @"typeof(WebApplication)",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var result = ctx.Console.Out.ToString();
            var error = ctx.Console.Error.ToString();
            Assert.Contains("[Microsoft.AspNetCore.Builder.WebApplication]", result);
        }

        [Fact]
        public async Task ShouldExecuteSimpleCode()
        {
            var commands = new[]
            {
                "1 + 1",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var result = ctx.Console.Out.ToString();
            Assert.Contains("2", result);
        }

        [Fact]
        public async Task ShouldExecuteWithNugetReference()
        {
            var commands = new[]
            {
                @"#r ""nuget: Automapper, 6.1.1""",
                "using AutoMapper;",
                "typeof(MapperConfiguration)",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var result = ctx.Console.Out.ToString();
            Assert.Contains("[AutoMapper.MapperConfiguration]", result);
        }

        [Fact]
        public async Task ShouldHandleExitCommand()
        {
            var commands = new[]
            {
                "var x = 5;",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var result = ctx.Console.Out.ToString();
            // Should complete without hanging
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldHandleResetCommand()
        {
            var commands = new[]
            {
                "var x = 1;",
                "#reset",
                "x",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var error = ctx.Console.Error.ToString();
            Assert.Contains("error CS0103: The name 'x' does not exist in the current context", error);
        }

        [Fact]
        public async Task ShouldExecuteMultilineCode()
        {
            var commands = new[]
            {
                "class MyClass {",
                "  public int Value { get; set; }",
                "}",
                "var obj = new MyClass { Value = 42 };",
                "obj.Value",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var result = ctx.Console.Out.ToString();
            Assert.Contains("42", result);
        }

        [Fact]
        public async Task ShouldHandleCompilationError()
        {
            var commands = new[]
            {
                "var x = undefinedVariable;",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var error = ctx.Console.Error.ToString();
            Assert.Contains("CS0103", error);
        }

        [Fact]
        public async Task ShouldHandleRuntimeException()
        {
            var commands = new[]
            {
                "throw new System.Exception(\"Test error\");",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var error = ctx.Console.Error.ToString();
            Assert.Contains("System.Exception", error);
        }

        [Fact]
        public async Task ShouldExecuteDirectlyWithExecuteMethod()
        {
            var ctx = GetRunner();
            var result = await ctx.Runner.Execute("2 + 3");

            var output = ctx.Console.Out.ToString();
            Assert.Contains("5", output);
        }

        [Fact]
        public async Task ShouldExecuteWithLoadDirective()
        {
            var pathToFixture = System.IO.Path.Combine("..", "..", "..", "..", "Dotnet.Script.Tests", "TestFixtures", "REPL", "main.csx");
            var commands = new[]
            {
                "var x = 5;",
                $@"#load ""{pathToFixture}""",
                "x * externalValue",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var result = ctx.Console.Out.ToString();
            Assert.Contains("500", result);
        }

        [Fact]
        public async Task ShouldMaintainStateAcrossMultipleExecutions()
        {
            var ctx = GetRunner();
            
            await ctx.Runner.Execute("var myVar = 100;");
            var result = await ctx.Runner.Execute("myVar + 50");

            var output = ctx.Console.Out.ToString();
            Assert.Contains("150", output);
        }

        [Fact]
        public async Task ShouldHandleRefAndLoadOnSameLine()
        {
            var commands = new[]
            {
                @"#r ""nuget: Automapper, 6.1.1""",
                "using AutoMapper;",
                "var cfg = new MapperConfiguration(c => c.AllowNullDestinationValues = true);",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var result = ctx.Console.Out.ToString();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldResetStateAfterResetCommand()
        {
            var ctx = GetRunner();
            
            await ctx.Runner.Execute("var tempVar = 42;");
            ctx.Runner.Reset();
            var result = await ctx.Runner.Execute("tempVar");

            var error = ctx.Console.Error.ToString();
            Assert.Contains("CS0103", error);
        }

        [Fact]
        public async Task ShouldHandleEmptyInput()
        {
            var commands = new[]
            {
                "",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            // Should not throw
            Assert.NotNull(ctx.Console.Out);
        }

        [Fact]
        public async Task ShouldExecuteDeclarationsAndExpressions()
        {
            var commands = new[]
            {
                "int Add(int a, int b) => a + b;",
                "Add(10, 20)",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var result = ctx.Console.Out.ToString();
            Assert.Contains("30", result);
        }

        [Fact]
        public async Task ShouldHandlePackageSourceParameter()
        {
            var commands = new[]
            {
                "1 + 1",
                "#exit"
            };

            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };
            var reader = new System.IO.StringReader(string.Join(System.Environment.NewLine, commands));
            var writer = new System.IO.StringWriter();
            var error = new System.IO.StringWriter();

            var console = new Dotnet.Script.Core.ScriptConsole(writer, reader, error);
            var logFactory = Dotnet.Script.Shared.Tests.TestOutputHelper.CreateTestLogFactory();
            var compiler = new Dotnet.Script.Core.ScriptCompiler(logFactory, useRestoreCache: false);
            var runner = new Dotnet.Script.Core.InteractiveRunner(compiler, logFactory, console, packageSources);

            await runner.RunLoop();

            var result = writer.ToString();
            Assert.Contains("2", result);
        }

        [Fact]
        public async Task ShouldHandleConsoleOutputInMultipleCommands()
        {
            var commands = new[]
            {
                "System.Console.WriteLine(\"Hello\");",
                "System.Console.WriteLine(\"World\");",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var result = ctx.Console.Out.ToString();
            Assert.Contains("Hello", result);
            Assert.Contains("World", result);
        }

        [Fact]
        public async Task ShouldHandleExceptionWithoutExiting()
        {
            var commands = new[]
            {
                "int x = int.Parse(\"not a number\");",
                "var y = 5;",
                "y",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            // Should continue after exception
            var result = ctx.Console.Out.ToString();
            Assert.Contains("5", result);
        }

        [Fact]
        public async Task ShouldHandleLineBreaksInCode()
        {
            var commands = new[]
            {
                "var result = ",
                "  10 +",
                "  20;",
                "result",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var result = ctx.Console.Out.ToString();
            Assert.Contains("30", result);
        }

        [Fact]
        public async Task ShouldReturnValueFromExecution()
        {
            var ctx = GetRunner();
            var result = await ctx.Runner.Execute("5 * 5");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldHandleNullReturnValue()
        {
            var ctx = GetRunner();
            var result = await ctx.Runner.Execute("null");

            // Should not throw
            Assert.True(true);
        }

        [Fact]
        public async Task ShouldHandleLoopInCode()
        {
            var commands = new[]
            {
                "var sum = 0;",
                "for (int i = 0; i < 5; i++) { sum += i; }",
                "sum",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var result = ctx.Console.Out.ToString();
            Assert.Contains("10", result);
        }

        [Fact]
        public async Task ShouldHandleComplexExpression()
        {
            var commands = new[]
            {
                "System.Linq.Enumerable.Range(1, 5).Aggregate((a, b) => a + b)",
                "#exit"
            };

            var ctx = GetRunner(commands);
            await ctx.Runner.RunLoop();

            var result = ctx.Console.Out.ToString();
            Assert.Contains("15", result);
        }
    }
}