using System;
using System.Collections.Generic;
using Xunit;
using Dotnet.Script.Core;

namespace Dotnet.Script.Core.Tests
{
    public class CSharpObjectFormatterExtensionsTests
    {
        [Fact]
        public void FormatObject_WithNull_ShouldReturnNullString()
        {
            // Arrange
            object obj = null;

            // Act
            var result = obj.FormatObject();

            // Assert
            Assert.Equal("null", result);
        }

        [Theory]
        [InlineData(42, "42")]
        [InlineData("hello", "\"hello\"")]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        public void FormatObject_WithPrimitiveTypes_ShouldReturnFormattedString(object input, string expected)
        {
            // Act
            var result = input.FormatObject();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatObject_WithArray_ShouldReturnFormattedArray()
        {
            // Arrange
            var array = new[] { 1, 2, 3 };

            // Act
            var result = array.FormatObject();

            // Assert
            Assert.Contains("1", result);
            Assert.Contains("2", result);
            Assert.Contains("3", result);
        }

        [Fact]
        public void FormatObject_WithList_ShouldReturnFormattedList()
        {
            // Arrange
            var list = new List<string> { "a", "b", "c" };

            // Act
            var result = list.FormatObject();

            // Assert
            Assert.Contains("a", result);
            Assert.Contains("b", result);
            Assert.Contains("c", result);
        }

        [Fact]
        public void FormatObject_WithDictionary_ShouldReturnFormattedDictionary()
        {
            // Arrange
            var dict = new Dictionary<string, int> { { "key1", 1 }, { "key2", 2 } };

            // Act
            var result = dict.FormatObject();

            // Assert
            Assert.Contains("key1", result);
            Assert.Contains("key2", result);
        }

        [Fact]
        public void FormatObject_WithCustomObject_ShouldReturnFormattedObject()
        {
            // Arrange
            var obj = new { Name = "Test", Value = 42 };

            // Act
            var result = obj.FormatObject();

            // Assert
            Assert.Contains("Name", result);
            Assert.Contains("Test", result);
            Assert.Contains("Value", result);
            Assert.Contains("42", result);
        }

        [Fact]
        public void FormatObject_WithNestedObject_ShouldReturnFormattedNestedObject()
        {
            // Arrange
            var obj = new 
            { 
                Name = "Parent", 
                Child = new { Name = "Child", Value = 10 } 
            };

            // Act
            var result = obj.FormatObject();

            // Assert
            Assert.Contains("Parent", result);
            Assert.Contains("Child", result);
            Assert.Contains("10", result);
        }

        [Fact]
        public void FormatObject_WithCircularReference_ShouldHandleGracefully()
        {
            // Arrange
            var obj1 = new TestClass { Name = "Object1" };
            var obj2 = new TestClass { Name = "Object2" };
            obj1.Reference = obj2;
            obj2.Reference = obj1;

            // Act & Assert
            // Should not throw StackOverflowException
            var result = obj1.FormatObject();
            Assert.NotNull(result);
        }

        private class TestClass
        {
            public string Name { get; set; }
            public TestClass Reference { get; set; }
        }
    }
}