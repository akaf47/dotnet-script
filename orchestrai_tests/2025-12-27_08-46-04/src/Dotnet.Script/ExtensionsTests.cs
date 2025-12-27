using System;
using McMaster.Extensions.CommandLineUtils;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class ExtensionsTests
    {
        /// <summary>
        /// Test that ValueEquals returns true when option has a value and strings match with specified comparison
        /// </summary>
        [Fact]
        public void ValueEquals_WithMatchingValue_OrdinalComparison_ReturnsTrue()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("TestValue");
            var comparisonValue = "TestValue";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test that ValueEquals returns true when option has a value and strings match case-insensitively
        /// </summary>
        [Fact]
        public void ValueEquals_WithMatchingValueIgnoreCase_ReturnsTrue()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("testvalue");
            var comparisonValue = "TESTVALUE";
            var comparison = StringComparison.OrdinalIgnoreCase;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test that ValueEquals returns false when option has a value but strings don't match
        /// </summary>
        [Fact]
        public void ValueEquals_WithNonMatchingValue_ReturnsFalse()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("FirstValue");
            var comparisonValue = "SecondValue";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test that ValueEquals returns false when option has no value
        /// </summary>
        [Fact]
        public void ValueEquals_WithoutValue_ReturnsFalse()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            var comparisonValue = "TestValue";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test that ValueEquals throws ArgumentNullException when option is null
        /// </summary>
        [Fact]
        public void ValueEquals_WithNullOption_ThrowsArgumentNullException()
        {
            // Arrange
            CommandOption option = null;
            var comparisonValue = "TestValue";
            var comparison = StringComparison.Ordinal;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => option.ValueEquals(comparisonValue, comparison));
            Assert.Equal("option", exception.ParamName);
        }

        /// <summary>
        /// Test that ValueEquals handles null comparison value correctly
        /// </summary>
        [Fact]
        public void ValueEquals_WithNullComparisonValue_ReturnsFalse()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("TestValue");
            string comparisonValue = null;
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test that ValueEquals handles empty string values
        /// </summary>
        [Fact]
        public void ValueEquals_WithEmptyStringValue_CanMatch()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("");
            var comparisonValue = "";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test that ValueEquals with empty option value doesn't match non-empty comparison
        /// </summary>
        [Fact]
        public void ValueEquals_WithEmptyOptionValueAndNonEmptyComparison_ReturnsFalse()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("");
            var comparisonValue = "TestValue";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test that ValueEquals uses the specified StringComparison correctly
        /// </summary>
        [Fact]
        public void ValueEquals_WithCurrentCultureComparison_RespectsComparison()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("TestValue");
            var comparisonValue = "testvalue";
            var comparison = StringComparison.CurrentCultureIgnoreCase;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test that ValueEquals returns false when option value differs in case with Ordinal comparison
        /// </summary>
        [Fact]
        public void ValueEquals_WithCaseMismatchAndOrdinalComparison_ReturnsFalse()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("TestValue");
            var comparisonValue = "testvalue";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test that ValueEquals works with very long strings
        /// </summary>
        [Fact]
        public void ValueEquals_WithLongString_Works()
        {
            // Arrange
            var longValue = new string('a', 10000);
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add(longValue);
            var comparisonValue = longValue;
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test that ValueEquals works with special characters
        /// </summary>
        [Fact]
        public void ValueEquals_WithSpecialCharacters_Works()
        {
            // Arrange
            var specialValue = "test!@#$%^&*()_+-=[]{}|;:',.<>?";
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add(specialValue);
            var comparisonValue = specialValue;
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test that ValueEquals works with unicode characters
        /// </summary>
        [Fact]
        public void ValueEquals_WithUnicodeCharacters_Works()
        {
            // Arrange
            var unicodeValue = "café ñoño 日本語";
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add(unicodeValue);
            var comparisonValue = unicodeValue;
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test that ValueEquals works with whitespace in values
        /// </summary>
        [Fact]
        public void ValueEquals_WithWhitespace_MatchesExactly()
        {
            // Arrange
            var valueWithWhitespace = "  test value  ";
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add(valueWithWhitespace);
            var comparisonValue = "  test value  ";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test that ValueEquals differentiates whitespace variations
        /// </summary>
        [Fact]
        public void ValueEquals_WithDifferentWhitespace_ReturnsFalse()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("  test value  ");
            var comparisonValue = " test value ";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test that ValueEquals works with InvariantCulture comparison
        /// </summary>
        [Fact]
        public void ValueEquals_WithInvariantCultureComparison_Works()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("TestValue");
            var comparisonValue = "TestValue";
            var comparison = StringComparison.InvariantCulture;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test that ValueEquals works with InvariantCultureIgnoreCase comparison
        /// </summary>
        [Fact]
        public void ValueEquals_WithInvariantCultureIgnoreCaseComparison_Works()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("testvalue");
            var comparisonValue = "TESTVALUE";
            var comparison = StringComparison.InvariantCultureIgnoreCase;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test that ValueEquals works with CurrentCulture comparison
        /// </summary>
        [Fact]
        public void ValueEquals_WithCurrentCultureComparison_Works()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("TestValue");
            var comparisonValue = "TestValue";
            var comparison = StringComparison.CurrentCulture;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test edge case with option having multiple values - should use first value
        /// </summary>
        [Fact]
        public void ValueEquals_WithMultipleValues_UsesFirstValue()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.MultipleValue);
            option.Values.Add("FirstValue");
            option.Values.Add("SecondValue");
            var comparisonValue = "FirstValue";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test that HasValue() is called correctly for option without value
        /// </summary>
        [Fact]
        public void ValueEquals_VerifiesHasValueBeforeAccessing_WithFlagOption()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.NoValue);
            var comparisonValue = "TestValue";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test that extension method returns correct type (bool)
        /// </summary>
        [Fact]
        public void ValueEquals_ReturnsBooleanValue()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("TestValue");
            var comparisonValue = "TestValue";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.IsType<bool>(result);
            Assert.True(result);
        }

        /// <summary>
        /// Test with both null option and null value to ensure proper null handling
        /// </summary>
        [Fact]
        public void ValueEquals_WithNullOptionAndNullValue_ThrowsForNullOption()
        {
            // Arrange
            CommandOption option = null;
            string comparisonValue = null;
            var comparison = StringComparison.Ordinal;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => option.ValueEquals(comparisonValue, comparison));
            Assert.Equal("option", exception.ParamName);
        }

        /// <summary>
        /// Test with numeric-like strings
        /// </summary>
        [Fact]
        public void ValueEquals_WithNumericStrings_Works()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("12345");
            var comparisonValue = "12345";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test with numeric-like strings that don't match
        /// </summary>
        [Fact]
        public void ValueEquals_WithDifferentNumericStrings_ReturnsFalse()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("12345");
            var comparisonValue = "54321";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test with newline characters in string
        /// </summary>
        [Fact]
        public void ValueEquals_WithNewlineCharacters_Works()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("test\nvalue");
            var comparisonValue = "test\nvalue";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test with tab characters in string
        /// </summary>
        [Fact]
        public void ValueEquals_WithTabCharacters_Works()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("test\tvalue");
            var comparisonValue = "test\tvalue";
            var comparison = StringComparison.Ordinal;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test case sensitivity with OrdinalIgnoreCase comparison
        /// </summary>
        [Fact]
        public void ValueEquals_MixedCaseWithOrdinalIgnoreCase_ReturnsTrue()
        {
            // Arrange
            var option = new CommandOption("-t|--test", CommandOptionType.SingleValue);
            option.Values.Add("tEsT");
            var comparisonValue = "TeSt";
            var comparison = StringComparison.OrdinalIgnoreCase;

            // Act
            var result = option.ValueEquals(comparisonValue, comparison);

            // Assert
            Assert.True(result);
        }
    }
}