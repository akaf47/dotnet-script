using Xunit;
using Dotnet.Script.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Script.Core.Tests.Internal
{
    public class EnumerableExtensionsTests
    {
        #region ForEach Tests

        [Fact]
        public void ForEach_WithValidEnumerableAndAction_ShouldExecuteActionForEachElement()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5 };
            var results = new List<int>();
            var action = new Action<int>(x => results.Add(x));

            // Act
            items.ForEach(action);

            // Assert
            Assert.Equal(items, results);
        }

        [Fact]
        public void ForEach_WithEmptyEnumerable_ShouldNotExecuteAction()
        {
            // Arrange
            var items = new int[] { };
            var callCount = 0;
            var action = new Action<int>(x => callCount++);

            // Act
            items.ForEach(action);

            // Assert
            Assert.Equal(0, callCount);
        }

        [Fact]
        public void ForEach_WithNullEnumerable_ShouldThrowArgumentNullException()
        {
            // Arrange
            IEnumerable<int> items = null;
            var action = new Action<int>(x => { });

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => items.ForEach(action));
        }

        [Fact]
        public void ForEach_WithNullAction_ShouldThrowArgumentNullException()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };
            Action<int> action = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => items.ForEach(action));
        }

        [Fact]
        public void ForEach_WithSingleElement_ShouldExecuteActionOnce()
        {
            // Arrange
            var items = new[] { 42 };
            var results = new List<int>();

            // Act
            items.ForEach(x => results.Add(x));

            // Assert
            Assert.Single(results);
            Assert.Equal(42, results[0]);
        }

        [Fact]
        public void ForEach_WithLargeCollection_ShouldExecuteActionForAll()
        {
            // Arrange
            var items = Enumerable.Range(0, 10000).ToList();
            var callCount = 0;

            // Act
            items.ForEach(x => callCount++);

            // Assert
            Assert.Equal(10000, callCount);
        }

        [Fact]
        public void ForEach_WhenActionThrowsException_ShouldPropagateException()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };
            var action = new Action<int>(x =>
            {
                if (x == 2)
                    throw new InvalidOperationException("Test exception");
            });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => items.ForEach(action));
        }

        #endregion

        #region FirstOrEmpty Tests

        [Fact]
        public void FirstOrEmpty_WithPopulatedEnumerable_ShouldReturnFirstElement()
        {
            // Arrange
            var items = new[] { "first", "second", "third" };

            // Act
            var result = items.FirstOrEmpty();

            // Assert
            Assert.Equal("first", result);
        }

        [Fact]
        public void FirstOrEmpty_WithEmptyEnumerable_ShouldReturnEmptyString()
        {
            // Arrange
            var items = new string[] { };

            // Act
            var result = items.FirstOrEmpty();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FirstOrEmpty_WithNullEnumerable_ShouldThrowArgumentNullException()
        {
            // Arrange
            IEnumerable<string> items = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => items.FirstOrEmpty());
        }

        [Fact]
        public void FirstOrEmpty_WithNullElementInEnumerable_ShouldReturnNull()
        {
            // Arrange
            var items = new string[] { null, "second" };

            // Act
            var result = items.FirstOrEmpty();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void FirstOrEmpty_WithWhitespaceStringAsFirst_ShouldReturnWhitespace()
        {
            // Arrange
            var items = new[] { "   ", "second" };

            // Act
            var result = items.FirstOrEmpty();

            // Assert
            Assert.Equal("   ", result);
        }

        #endregion

        #region ToStringList Tests

        [Fact]
        public void ToStringList_WithEnumerableOfStrings_ShouldConvertToList()
        {
            // Arrange
            var items = new[] { "one", "two", "three" };

            // Act
            var result = items.ToStringList();

            // Assert
            Assert.IsType<List<string>>(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(items, result);
        }

        [Fact]
        public void ToStringList_WithEmptyEnumerable_ShouldReturnEmptyList()
        {
            // Arrange
            var items = new string[] { };

            // Act
            var result = items.ToStringList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ToStringList_WithNullEnumerable_ShouldThrowArgumentNullException()
        {
            // Arrange
            IEnumerable<string> items = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => items.ToStringList());
        }

        [Fact]
        public void ToStringList_WithSingleElement_ShouldReturnListWithOneElement()
        {
            // Arrange
            var items = new[] { "single" };

            // Act
            var result = items.ToStringList();

            // Assert
            Assert.Single(result);
            Assert.Equal("single", result[0]);
        }

        [Fact]
        public void ToStringList_WithNullElements_ShouldIncludeNullsInList()
        {
            // Arrange
            var items = new string[] { "one", null, "three" };

            // Act
            var result = items.ToStringList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Null(result[1]);
        }

        #endregion

        #region FirstOrDefaultIfEmpty Tests

        [Fact]
        public void FirstOrDefaultIfEmpty_WithPopulatedEnumerable_ShouldReturnFirstElement()
        {
            // Arrange
            var items = new[] { 10, 20, 30 };

            // Act
            var result = items.FirstOrDefaultIfEmpty(0);

            // Assert
            Assert.Equal(10, result);
        }

        [Fact]
        public void FirstOrDefaultIfEmpty_WithEmptyEnumerable_ShouldReturnDefaultValue()
        {
            // Arrange
            var items = new int[] { };

            // Act
            var result = items.FirstOrDefaultIfEmpty(42);

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public void FirstOrDefaultIfEmpty_WithNullEnumerable_ShouldThrowArgumentNullException()
        {
            // Arrange
            IEnumerable<int> items = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => items.FirstOrDefaultIfEmpty(0));
        }

        [Fact]
        public void FirstOrDefaultIfEmpty_WithNullAsDefaultValue_ShouldReturnNullWhenEmpty()
        {
            // Arrange
            var items = new string[] { };

            // Act
            var result = items.FirstOrDefaultIfEmpty(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void FirstOrDefaultIfEmpty_WithZeroAsDefaultValue_ShouldReturnZeroWhenEmpty()
        {
            // Arrange
            var items = new int[] { };

            // Act
            var result = items.FirstOrDefaultIfEmpty(0);

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region Contains Tests

        [Fact]
        public void Contains_WithPresentElement_ShouldReturnTrue()
        {
            // Arrange
            var items = new[] { "apple", "banana", "cherry" };

            // Act
            var result = items.Contains("banana");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithAbsentElement_ShouldReturnFalse()
        {
            // Arrange
            var items = new[] { "apple", "banana", "cherry" };

            // Act
            var result = items.Contains("mango");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WithEmptyEnumerable_ShouldReturnFalse()
        {
            // Arrange
            var items = new string[] { };

            // Act
            var result = items.Contains("anything");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WithNullEnumerable_ShouldThrowArgumentNullException()
        {
            // Arrange
            IEnumerable<string> items = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => items.Contains("value"));
        }

        [Fact]
        public void Contains_WithNullValue_ShouldFindNullInEnumerable()
        {
            // Arrange
            var items = new string[] { "apple", null, "cherry" };

            // Act
            var result = items.Contains(null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithFirstElement_ShouldReturnTrue()
        {
            // Arrange
            var items = new[] { "first", "second", "third" };

            // Act
            var result = items.Contains("first");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithLastElement_ShouldReturnTrue()
        {
            // Arrange
            var items = new[] { "first", "second", "last" };

            // Act
            var result = items.Contains("last");

            // Assert
            Assert.True(result);
        }

        #endregion

        #region AnyIfEmpty Tests

        [Fact]
        public void AnyIfEmpty_WithNonEmptyEnumerable_ShouldReturnTrue()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act
            var result = items.AnyIfEmpty();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AnyIfEmpty_WithEmptyEnumerable_ShouldReturnFalse()
        {
            // Arrange
            var items = new int[] { };

            // Act
            var result = items.AnyIfEmpty();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AnyIfEmpty_WithNullEnumerable_ShouldThrowArgumentNullException()
        {
            // Arrange
            IEnumerable<int> items = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => items.AnyIfEmpty());
        }

        [Fact]
        public void AnyIfEmpty_WithSingleElement_ShouldReturnTrue()
        {
            // Arrange
            var items = new[] { 42 };

            // Act
            var result = items.AnyIfEmpty();

            // Assert
            Assert.True(result);
        }

        #endregion

        #region IsEmpty Tests

        [Fact]
        public void IsEmpty_WithEmptyEnumerable_ShouldReturnTrue()
        {
            // Arrange
            var items = new int[] { };

            // Act
            var result = items.IsEmpty();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEmpty_WithNonEmptyEnumerable_ShouldReturnFalse()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act
            var result = items.IsEmpty();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsEmpty_WithNullEnumerable_ShouldThrowArgumentNullException()
        {
            // Arrange
            IEnumerable<int> items = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => items.IsEmpty());
        }

        [Fact]
        public void IsEmpty_WithSingleElement_ShouldReturnFalse()
        {
            // Arrange
            var items = new[] { 99 };

            // Act
            var result = items.IsEmpty();

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}