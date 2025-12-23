using Xunit;
using Dotnet.Script.DependencyModel.Context;

namespace Dotnet.Script.DependencyModel.Tests.Context
{
    public class ScriptModeTests
    {
        [Fact]
        public void ScriptMode_Should_Have_Script_Value()
        {
            // Act
            var scriptMode = ScriptMode.Script;

            // Assert
            Assert.Equal(0, (int)scriptMode);
            Assert.Equal(ScriptMode.Script, scriptMode);
        }

        [Fact]
        public void ScriptMode_Should_Have_Eval_Value()
        {
            // Act
            var evalMode = ScriptMode.Eval;

            // Assert
            Assert.Equal(1, (int)evalMode);
            Assert.Equal(ScriptMode.Eval, evalMode);
        }

        [Fact]
        public void ScriptMode_Should_Have_REPL_Value()
        {
            // Act
            var replMode = ScriptMode.REPL;

            // Assert
            Assert.Equal(2, (int)replMode);
            Assert.Equal(ScriptMode.REPL, replMode);
        }

        [Fact]
        public void ScriptMode_Script_And_Eval_Should_Not_Be_Equal()
        {
            // Arrange
            var scriptMode = ScriptMode.Script;
            var evalMode = ScriptMode.Eval;

            // Act & Assert
            Assert.NotEqual(scriptMode, evalMode);
        }

        [Fact]
        public void ScriptMode_Eval_And_REPL_Should_Not_Be_Equal()
        {
            // Arrange
            var evalMode = ScriptMode.Eval;
            var replMode = ScriptMode.REPL;

            // Act & Assert
            Assert.NotEqual(evalMode, replMode);
        }

        [Fact]
        public void ScriptMode_Script_And_REPL_Should_Not_Be_Equal()
        {
            // Arrange
            var scriptMode = ScriptMode.Script;
            var replMode = ScriptMode.REPL;

            // Act & Assert
            Assert.NotEqual(scriptMode, replMode);
        }

        [Theory]
        [InlineData(ScriptMode.Script)]
        [InlineData(ScriptMode.Eval)]
        [InlineData(ScriptMode.REPL)]
        public void ScriptMode_Should_Support_All_Enum_Values(ScriptMode mode)
        {
            // Act & Assert
            Assert.NotNull(mode);
            Assert.True(mode >= ScriptMode.Script && mode <= ScriptMode.REPL);
        }

        [Fact]
        public void ScriptMode_Should_Be_Castable_To_Int()
        {
            // Act
            int scriptValue = (int)ScriptMode.Script;
            int evalValue = (int)ScriptMode.Eval;
            int replValue = (int)ScriptMode.REPL;

            // Assert
            Assert.Equal(0, scriptValue);
            Assert.Equal(1, evalValue);
            Assert.Equal(2, replValue);
        }

        [Fact]
        public void ScriptMode_Should_Be_Comparable()
        {
            // Act & Assert
            Assert.True(ScriptMode.Script < ScriptMode.Eval);
            Assert.True(ScriptMode.Eval < ScriptMode.REPL);
            Assert.True(ScriptMode.Script < ScriptMode.REPL);
        }

        [Fact]
        public void ScriptMode_ToString_Should_Return_Enum_Name()
        {
            // Act
            string scriptStr = ScriptMode.Script.ToString();
            string evalStr = ScriptMode.Eval.ToString();
            string replStr = ScriptMode.REPL.ToString();

            // Assert
            Assert.Equal("Script", scriptStr);
            Assert.Equal("Eval", evalStr);
            Assert.Equal("REPL", replStr);
        }
    }
}