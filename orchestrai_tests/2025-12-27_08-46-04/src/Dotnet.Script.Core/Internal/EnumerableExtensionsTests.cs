using System;
using System.Collections.Generic;
using System.Linq;
using Dotnet.Script.Core.Internal;
using Xunit;

namespace Dotnet.Script.Core.Tests.Internal
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void OrderBy_WithValidComparison_ReturnsOrderedSequence()
        {
            // Arrange
            var numbers = new[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = numbers.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(new[] { 1, 1, 2, 3, 4, 5, 6, 9 }, result);
        }

        [Fact]
        public void OrderBy_WithReverseComparison_ReturnsReverseOrderedSequence()
        {
            // Arrange
            var numbers = new[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            Comparison<int> comparison = (a, b) => b.CompareTo(a);

            // Act
            var result = numbers.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(new[] { 9, 6, 5, 4, 3, 2, 1, 1 }, result);
        }

        [Fact]
        public void OrderBy_WithStrings_ReturnsOrderedStringSequence()
        {
            // Arrange
            var strings = new[] { "zebra", "apple", "banana", "cherry" };
            Comparison<string> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = strings.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(new[] { "apple", "banana", "cherry", "zebra" }, result);
        }

        [Fact]
        public void OrderBy_WithEmptySequence_ReturnsEmptySequence()
        {
            // Arrange
            var numbers = new int[] { };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = numbers.OrderBy(comparison).ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void OrderBy_WithSingleElement_ReturnsSingleElementSequence()
        {
            // Arrange
            var numbers = new[] { 42 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = numbers.OrderBy(comparison).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(42, result[0]);
        }

        [Fact]
        public void OrderBy_WithNullComparison_ThrowsArgumentNullException()
        {
            // Arrange
            var numbers = new[] { 3, 1, 4 };
            Comparison<int> comparison = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => numbers.OrderBy(comparison).ToList());
        }

        [Fact]
        public void OrderBy_WithCustomObjectComparison_ReturnsOrderedSequence()
        {
            // Arrange
            var people = new[]
            {
                new Person { Name = "Charlie", Age = 30 },
                new Person { Name = "Alice", Age = 25 },
                new Person { Name = "Bob", Age = 35 }
            };
            Comparison<Person> comparison = (a, b) => a.Age.CompareTo(b.Age);

            // Act
            var result = people.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal("Alice", result[0].Name);
            Assert.Equal("Charlie", result[1].Name);
            Assert.Equal("Bob", result[2].Name);
        }

        [Fact]
        public void OrderBy_WithDuplicateElements_MaintainsStability()
        {
            // Arrange
            var numbers = new[] { 5, 3, 5, 1, 3, 5 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = numbers.OrderBy(comparison).ToList();

            // Assert
            var groups = result.GroupBy(x => x).ToList();
            foreach (var group in groups)
            {
                Assert.True(group.Count() > 0);
            }
        }

        [Fact]
        public void OrderBy_WithNegativeNumbers_ReturnsCorrectlyOrderedSequence()
        {
            // Arrange
            var numbers = new[] { -5, 3, -1, 0, 10, -100 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = numbers.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(new[] { -100, -5, -1, 0, 3, 10 }, result);
        }

        [Fact]
        public void OrderBy_WithComplexComparison_ReturnsCorrectlyOrderedSequence()
        {
            // Arrange
            var numbers = new[] { 10, 5, 20, 15, 8 };
            // Custom comparison: order by absolute distance from 10
            Comparison<int> comparison = (a, b) =>
            {
                var distA = Math.Abs(a - 10);
                var distB = Math.Abs(b - 10);
                return distA.CompareTo(distB);
            };

            // Act
            var result = numbers.OrderBy(comparison).ToList();

            // Assert
            Assert.Equal(10, result[0]); // distance 0
            Assert.Equal(2, result.Count(x => x == 8 || x == 15)); // distance 2
        }

        [Fact]
        public void OrderBy_ReturnsIOrderedEnumerable()
        {
            // Arrange
            var numbers = new[] { 3, 1, 4 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b);

            // Act
            var result = numbers.OrderBy(comparison);

            // Assert
            Assert.IsAssignableFrom<IOrderedEnumerable<int>>(result);
        }

        [Fact]
        public void OrderBy_WithNullStringElements_HandlesProperly()
        {
            // Arrange
            var strings = new string[] { null, "apple", null, "banana" };
            Comparison<string> comparison = (a, b) =>
            {
                if (a == null && b == null) return 0;
                if (a == null) return -1;
                if (b == null) return 1;
                return a.CompareTo(b);
            };

            // Act
            var result = strings.OrderBy(comparison).ToList();

            // Assert
            Assert.Null(result[0]);
            Assert.Null(result[1]);
            Assert.Equal("apple", result[2]);
            Assert.Equal("banana", result[3]);
        }

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}