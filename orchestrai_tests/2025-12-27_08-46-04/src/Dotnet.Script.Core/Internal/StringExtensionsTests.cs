using System;
using Xunit;

namespace Dotnet.Script.Core.Tests.Internal
{
    public class StringExtensionsTests
    {
        [Fact]
        public void Contains_WithStringFoundUsingOrdinal_ReturnsTrue()
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
        public void Contains_WithStringNotFoundUsingOrdinal_ReturnsFalse()
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
        public void Contains_WithEmptySource_ReturnsFalse()
        {
            // Arrange
            string source = "";
            string value = "test";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WithEmptyValue_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = "";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithBothEmpty_ReturnsTrue()
        {
            // Arrange
            string source = "";
            string value = "";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithNullSource_ReturnsFalse()
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
        public void Contains_WithNullValue_ThrowsArgumentNullException()
        {
            // Arrange
            string source = "Hello World";
            string value = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => source.Contains(value, StringComparison.Ordinal));
        }

        [Fact]
        public void Contains_WithCaseInsensitiveComparison_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = "world";

            // Act
            bool result = source.Contains(value, StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithCaseSensitiveComparison_ReturnsFalse()
        {
            // Arrange
            string source = "Hello World";
            string value = "world";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WithSubstringAtBeginning_ReturnsTrue()
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
        public void Contains_WithSubstringAtEnd_ReturnsTrue()
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
        public void Contains_WithSubstringInMiddle_ReturnsTrue()
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
        public void Contains_WithEntireStringAsValue_ReturnsTrue()
        {
            // Arrange
            string source = "Hello World";
            string value = "Hello World";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithValueLongerThanSource_ReturnsFalse()
        {
            // Arrange
            string source = "Hi";
            string value = "Hello World";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WithCurrentCultureComparison_ReturnsCorrectResult()
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
        public void Contains_WithCurrentCultureIgnoreCaseComparison_ReturnsCorrectResult()
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
        public void Contains_WithInvariantCultureComparison_ReturnsCorrectResult()
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
        public void Contains_WithInvariantCultureIgnoreCaseComparison_ReturnsCorrectResult()
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
        public void Contains_WithSpecialCharacters_ReturnsTrue()
        {
            // Arrange
            string source = "Hello@World#123";
            string value = "@World";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithNumbers_ReturnsTrue()
        {
            // Arrange
            string source = "Hello123World";
            string value = "123";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithWhitespace_ReturnsCorrectResult()
        {
            // Arrange
            string source = "Hello World";
            string value = " ";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithNewlineCharacters_ReturnsCorrectResult()
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
        public void Contains_WithTabCharacters_ReturnsCorrectResult()
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
        public void Contains_WithUnicodeCharacters_ReturnsTrue()
        {
            // Arrange
            string source = "Hëllö Wørld";
            string value = "ëll";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithDifferentUnicodeCharacters_ReturnsFalse()
        {
            // Arrange
            string source = "Hëllö Wørld";
            string value = "éll"; // different accent

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_NullSourceWithNullValue_ThrowsArgumentNullException()
        {
            // Arrange
            string source = null;
            string value = null;

            // Act & Assert
            // The null coalescing on source handles null, but IndexOf will be called with null value
            Assert.Throws<ArgumentNullException>(() => source.Contains(value, StringComparison.Ordinal));
        }

        [Fact]
        public void Contains_WithRepeatingPattern_ReturnsTrue()
        {
            // Arrange
            string source = "aaaaaaaaab";
            string value = "aaa";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithRepeatingPatternNotFound_ReturnsFalse()
        {
            // Arrange
            string source = "aaaaaaaaab";
            string value = "aaab";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_IndexOfReturnsNegativeOne_ReturnsFalse()
        {
            // Arrange
            string source = "Hello";
            string value = "xyz";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_IndexOfReturnsZero_ReturnsTrue()
        {
            // Arrange
            string source = "Hello";
            string value = "H";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_IndexOfReturnsPositiveNumber_ReturnsTrue()
        {
            // Arrange
            string source = "Hello";
            string value = "ll";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithSingleCharacterSource_ReturnsCorrectResult()
        {
            // Arrange
            string source = "A";
            string value = "A";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithSingleCharacterSourceNotFound_ReturnsFalse()
        {
            // Arrange
            string source = "A";
            string value = "B";

            // Act
            bool result = source.Contains(value, StringComparison.Ordinal);

            // Assert
            Assert.False(result);
        }
    }
}