using System;
using Xunit;

namespace Dotnet.Script.Core.Internal.Tests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void Contains_WhenSourceIsNull_ReturnsFalse()
        {
            // Arrange
            string source = null;
            string value = "test";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WhenSourceIsNullAndValueIsNull_ReturnsFalse()
        {
            // Arrange
            string source = null;
            string value = null;

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WhenSourceIsNullAndValueIsEmpty_ReturnsFalse()
        {
            // Arrange
            string source = null;
            string value = string.Empty;

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsValueWithOrdinalComparison_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = "World";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceDoesNotContainValueWithOrdinalComparison_ReturnsFalse()
        {
            // Arrange
            string source = "Hello World";
            string value = "xyz";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsValueAtBeginning_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = "Hello";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsValueAtEnd_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = "d";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsValueInMiddle_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = "lo Wo";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceIsEmptyString_ReturnsFalse()
        {
            // Arrange
            string source = string.Empty;
            string value = "test";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WhenSourceIsEmptyAndValueIsEmpty_ReturnsTrue()
        {
            // Arrange
            string source = string.Empty;
            string value = string.Empty;

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenValueIsEmpty_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = string.Empty;

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsValueWithOrdinalIgnoreCaseComparison_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = "hello";

            // Act
            bool result = source.Contains(value, StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceDoesNotContainValueWithOrdinalIgnoreCaseComparison_ReturnsFalse()
        {
            // Arrange
            string source = "Hello World";
            string value = "xyz";

            // Act
            bool result = source.Contains(value, StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsValueWithCurrentCultureComparison_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = "World";

            // Act
            bool result = source.Contains(value, StringComparison.CurrentCulture);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsValueWithCurrentCultureIgnoreCaseComparison_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = "world";

            // Act
            bool result = source.Contains(value, StringComparison.CurrentCultureIgnoreCase);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsValueWithInvariantCultureComparison_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = "World";

            // Act
            bool result = source.Contains(value, StringComparison.InvariantCulture);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsValueWithInvariantCultureIgnoreCaseComparison_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = "world";

            // Act
            bool result = source.Contains(value, StringComparison.InvariantCultureIgnoreCase);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceAndValueAreSame_ReturnsTrue()
        {
            // Arrange
            string source = "test";
            string value = "test";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenValueIsLongerThanSource_ReturnsFalse()
        {
            // Arrange
            string source = "test";
            string value = "testing longer";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsSingleCharacter_ReturnsTrue()
        {
            // Arrange
            string source = "a";
            string value = "a";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsSingleCharacterNotMatching_ReturnsFalse()
        {
            // Arrange
            string source = "a";
            string value = "b";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsSpecialCharacters_ReturnsTrue()
        {
            // Arrange
            string source = "Hello@World!";
            string value = "@World";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsWhitespace_ReturnsTrue()
        {
            // Arrange
            string source = "Hello   World";
            string value = "   ";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsNewlineAndValue_ReturnsTrue()
        {
            // Arrange
            string source = "Hello\nWorld";
            string value = "\n";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenSourceContainsTabAndValue_ReturnsTrue()
        {
            // Arrange
            string source = "Hello\tWorld";
            string value = "\t";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WhenCaseSensitiveAndCaseDoesNotMatch_ReturnsFalse()
        {
            // Arrange
            string source = "HELLO";
            string value = "hello";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WhenCaseSensitiveAndCaseMatches_ReturnsTrue()
        {
            // Arrange
            string source = "hello";
            string value = "hello";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("test", "test", StringComparison.Ordinal, true)]
        [InlineData("test", "es", StringComparison.Ordinal, true)]
        [InlineData("test", "xyz", StringComparison.Ordinal, false)]
        [InlineData("TEST", "test", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("", "", StringComparison.Ordinal, true)]
        [InlineData("abc", "", StringComparison.Ordinal, true)]
        public void Contains_WithVariousInputs_ProducesExpectedResults(string source, string value, StringComparison comparison, bool expected)
        {
            // Act
            bool result = source.Contains(value, comparison);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}