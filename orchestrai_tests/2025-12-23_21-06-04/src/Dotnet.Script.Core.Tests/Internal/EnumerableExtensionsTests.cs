using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Dotnet.Script.Core.Internal;

namespace Dotnet.Script.Core.Tests.Internal
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void OrderBy_WithValidComparison_ShouldReturnOrderedEnumerable()
        {
            // Arrange
            var source = new[] { 3, 1, 2 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IOrderedEnumerable<int>>(result);
        }

        [Fact]
        public void OrderBy_WithAscendingComparison_ShouldReturnSortedAscending()
        {
            // Arrange
            var source = new[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(new[] { 1, 1, 2, 3, 4, 5, 6, 9 }, result);
        }

        [Fact]
        public void OrderBy_WithDescendingComparison_ShouldReturnSortedDescending()
        {
            // Arrange
            var source = new[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            Comparison<int> comparison = (a, b) => b.CompareTo(a);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(new[] { 9, 6, 5, 4, 3, 2, 1, 1 }, result);
        }

        [Fact]
        public void OrderBy_WithEmptySource_ShouldReturnEmptyEnumerable()
        {
            // Arrange
            var source = new int[] { };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void OrderBy_WithSingleElement_ShouldReturnSingleElement()
        {
            // Arrange
            var source = new[] { 42 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(42, result[0]);
        }

        [Fact]
        public void OrderBy_WithAlreadySortedSource_ShouldMaintainOrder()
        {
            // Arrange
            var source = new[] { 1, 2, 3, 4, 5 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
        }

        [Fact]
        public void OrderBy_WithReverseSortedSource_ShouldSort()
        {
            // Arrange
            var source = new[] { 5, 4, 3, 2, 1 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
        }

        [Fact]
        public void OrderBy_WithDuplicateElements_ShouldPreserveAll()
        {
            // Arrange
            var source = new[] { 1, 3, 1, 2, 1, 3, 2 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(new[] { 1, 1, 1, 2, 2, 3, 3 }, result);
        }

        [Fact]
        public void OrderBy_WithNegativeNumbers_ShouldSortCorrectly()
        {
            // Arrange
            var source = new[] { -5, 3, -1, 10, -3, 0 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(new[] { -5, -3, -1, 0, 3, 10 }, result);
        }

        [Fact]
        public void OrderBy_WithStrings_ShouldSortAlphabetically()
        {
            // Arrange
            var source = new[] { "delta", "alpha", "charlie", "bravo" };
            Comparison<string> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(new[] { "alpha", "bravo", "charlie", "delta" }, result);
        }

        [Fact]
        public void OrderBy_WithCustomObjects_ShouldSortByComparison()
        {
            // Arrange
            var source = new[]
            {
                new TestObject { Value = 3, Name = "C" },
                new TestObject { Value = 1, Name = "A" },
                new TestObject { Value = 2, Name = "B" }
            };
            Comparison<TestObject> comparison = (a, b) => a.Value.CompareTo(b.Value);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(1, result[0].Value);
            Assert.Equal(2, result[1].Value);
            Assert.Equal(3, result[2].Value);
        }

        [Fact]
        public void OrderBy_WithCustomObjects_ShouldSortByNameComparison()
        {
            // Arrange
            var source = new[]
            {
                new TestObject { Value = 3, Name = "Charlie" },
                new TestObject { Value = 1, Name = "Alice" },
                new TestObject { Value = 2, Name = "Bob" }
            };
            Comparison<TestObject> comparison = (a, b) => a.Name.CompareTo(b.Name);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal("Alice", result[0].Name);
            Assert.Equal("Bob", result[1].Name);
            Assert.Equal("Charlie", result[2].Name);
        }

        [Fact]
        public void OrderBy_WithNullComparison_ShouldThrowArgumentNullException()
        {
            // Arrange
            var source = new[] { 3, 1, 2 };
            Comparison<int> comparison = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => source.OrderBy(comparison).ToList());
        }

        [Fact]
        public void OrderBy_WithNullSource_ShouldThrowNullReferenceException()
        {
            // Arrange
            IEnumerable<int> source = null;
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => source.OrderBy(comparison));
        }

        [Fact]
        public void OrderBy_ShouldReturnIOrderedEnumerableInterface()
        {
            // Arrange
            var source = new[] { 3, 1, 2 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison);

            // Assert
            Assert.IsAssignableFrom<IOrderedEnumerable<int>>(result);
        }

        [Fact]
        public void OrderBy_ShouldBeEnumerable()
        {
            // Arrange
            var source = new[] { 3, 1, 2 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison);

            // Assert
            Assert.IsAssignableFrom<IEnumerable<int>>(result);
        }

        [Fact]
        public void OrderBy_WithLargeDataset_ShouldSortCorrectly()
        {
            // Arrange
            var random = new Random(42);
            var source = Enumerable.Range(1, 1000).OrderBy(_ => random.Next()).ToArray();
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(1000, result.Count);
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i] <= result[i + 1]);
            }
        }

        [Fact]
        public void OrderBy_WithZeroComparison_ShouldPreserveBothElements()
        {
            // Arrange
            var source = new[] { 1, 2, 3 };
            // Comparison that always returns 0 (equal)
            Comparison<int> comparison = (a, b) => 0;

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(1, result);
            Assert.Contains(2, result);
            Assert.Contains(3, result);
        }

        [Fact]
        public void OrderBy_WithComplexComparisonLogic_ShouldApplyCorrectly()
        {
            // Arrange
            var source = new[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            // Sort by even first, then by value
            Comparison<int> comparison = (a, b) =>
            {
                int aEven = (a % 2 == 0) ? 0 : 1;
                int bEven = (b % 2 == 0) ? 0 : 1;
                int evenCompare = aEven.CompareTo(bEven);
                if (evenCompare != 0) return evenCompare;
                return a.CompareTo(b);
            };

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            // Even numbers should come first: 2, 4, 6
            // Odd numbers should come second: 1, 1, 3, 5, 9
            var evens = result.Where(x => x % 2 == 0).ToList();
            var odds = result.Where(x => x % 2 != 0).ToList();

            Assert.Equal(3, evens.Count);
            Assert.Equal(5, odds.Count);
            Assert.True(result.IndexOf(2) < result.IndexOf(1));
        }

        [Fact]
        public void OrderBy_ShouldNotModifyOriginalSource()
        {
            // Arrange
            var source = new[] { 3, 1, 2 };
            var originalSource = new[] { 3, 1, 2 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(originalSource, source);
        }

        [Fact]
        public void OrderBy_WithStringsCaseSensitive_ShouldSortByCase()
        {
            // Arrange
            var source = new[] { "Zebra", "apple", "Banana", "apricot" };
            Comparison<string> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            // Case-sensitive sort puts uppercase letters before lowercase
            Assert.Equal("Banana", result[0]);
            Assert.Equal("Zebra", result[1]);
            Assert.Equal("apple", result[2]);
            Assert.Equal("apricot", result[3]);
        }

        [Fact]
        public void OrderBy_WithMixedPositiveNegativeZero_ShouldSortCorrectly()
        {
            // Arrange
            var source = new[] { 0, -5, 5, -1, 1, -10, 10 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(new[] { -10, -5, -1, 0, 1, 5, 10 }, result);
        }

        [Fact]
        public void OrderBy_ReturnValueShouldBeConsumableMultipleTimes()
        {
            // Arrange
            var source = new[] { 3, 1, 2 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = source.OrderBy(comparison);
            var firstConsumption = result.ToList();
            var secondConsumption = result.ToList();

            // Assert
            Assert.Equal(firstConsumption, secondConsumption);
            Assert.Equal(new[] { 1, 2, 3 }, firstConsumption);
        }

        private class TestObject
        {
            public int Value { get; set; }
            public string Name { get; set; }
        }
    }
}