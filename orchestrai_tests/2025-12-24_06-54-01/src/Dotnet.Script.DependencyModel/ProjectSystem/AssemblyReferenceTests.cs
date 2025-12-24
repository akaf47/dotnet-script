using System;
using Xunit;
using Dotnet.Script.DependencyModel.ProjectSystem;
using Moq;
using Dotnet.Script.DependencyModel.Environment;

namespace Dotnet.Script.DependencyModel.Tests.ProjectSystem
{
    public class AssemblyReferenceTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidPath_ShouldInitializeAssemblyPath()
        {
            // Arrange
            var testPath = "/path/to/assembly.dll";

            // Act
            var reference = new AssemblyReference(testPath);

            // Assert
            Assert.Equal(testPath, reference.AssemblyPath);
        }

        [Fact]
        public void Constructor_WithNullPath_ShouldInitializeWithNull()
        {
            // Arrange
            string testPath = null;

            // Act
            var reference = new AssemblyReference(testPath);

            // Assert
            Assert.Null(reference.AssemblyPath);
        }

        [Fact]
        public void Constructor_WithEmptyPath_ShouldInitializeWithEmptyString()
        {
            // Arrange
            var testPath = string.Empty;

            // Act
            var reference = new AssemblyReference(testPath);

            // Assert
            Assert.Empty(reference.AssemblyPath);
        }

        [Fact]
        public void Constructor_WithWindowsPath_ShouldInitializeCorrectly()
        {
            // Arrange
            var testPath = @"C:\Program Files\Assembly.dll";

            // Act
            var reference = new AssemblyReference(testPath);

            // Assert
            Assert.Equal(testPath, reference.AssemblyPath);
        }

        [Fact]
        public void Constructor_WithUnixPath_ShouldInitializeCorrectly()
        {
            // Arrange
            var testPath = "/usr/local/assembly.dll";

            // Act
            var reference = new AssemblyReference(testPath);

            // Assert
            Assert.Equal(testPath, reference.AssemblyPath);
        }

        #endregion

        #region Equals(AssemblyReference) Tests

        [Fact]
        public void EqualsAssemblyReference_WithSamePath_ShouldReturnTrue()
        {
            // Arrange
            var path = "/path/to/assembly.dll";
            var reference1 = new AssemblyReference(path);
            var reference2 = new AssemblyReference(path);

            // Act
            var result = reference1.Equals(reference2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualsAssemblyReference_WithDifferentPaths_ShouldReturnFalse()
        {
            // Arrange
            var reference1 = new AssemblyReference("/path/to/assembly1.dll");
            var reference2 = new AssemblyReference("/path/to/assembly2.dll");

            // Act
            var result = reference1.Equals(reference2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsAssemblyReference_WithNullOther_ShouldReturnFalse()
        {
            // Arrange
            var reference1 = new AssemblyReference("/path/to/assembly.dll");
            AssemblyReference reference2 = null;

            // Act
            var result = reference1.Equals(reference2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsAssemblyReference_WithSameReference_ShouldReturnTrue()
        {
            // Arrange
            var reference = new AssemblyReference("/path/to/assembly.dll");

            // Act
            var result = reference.Equals(reference);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualsAssemblyReference_WithCaseDifferenceOnWindows_ShouldReturnTrueOnWindows()
        {
            // Arrange - This test validates OS-specific behavior
            var reference1 = new AssemblyReference(@"C:\Path\Assembly.dll");
            var reference2 = new AssemblyReference(@"C:\path\assembly.dll");

            // Act
            var result = reference1.Equals(reference2);

            // Assert - On Windows: case-insensitive comparison (True), On Unix: case-sensitive (False)
            if (ScriptEnvironment.Default.IsWindows)
            {
                Assert.True(result);
            }
            else
            {
                Assert.False(result);
            }
        }

        [Fact]
        public void EqualsAssemblyReference_WithBothNull_ShouldReturnFalse()
        {
            // Arrange
            var reference1 = new AssemblyReference(null);
            var reference2 = new AssemblyReference(null);

            // Act
            var result = reference1.Equals(reference2);

            // Assert
            Assert.False(result); // null.Equals(null) via PathComparer returns false
        }

        #endregion

        #region Equals(object) Tests

        [Fact]
        public void EqualsObject_WithSameAssemblyReference_ShouldReturnTrue()
        {
            // Arrange
            var path = "/path/to/assembly.dll";
            var reference1 = new AssemblyReference(path);
            object reference2 = new AssemblyReference(path);

            // Act
            var result = reference1.Equals(reference2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualsObject_WithDifferentAssemblyReferences_ShouldReturnFalse()
        {
            // Arrange
            var reference1 = new AssemblyReference("/path/to/assembly1.dll");
            object reference2 = new AssemblyReference("/path/to/assembly2.dll");

            // Act
            var result = reference1.Equals(reference2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsObject_WithNonAssemblyReferenceObject_ShouldReturnFalse()
        {
            // Arrange
            var reference1 = new AssemblyReference("/path/to/assembly.dll");
            object reference2 = "not an AssemblyReference";

            // Act
            var result = reference1.Equals(reference2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsObject_WithNullObject_ShouldReturnFalse()
        {
            // Arrange
            var reference1 = new AssemblyReference("/path/to/assembly.dll");
            object reference2 = null;

            // Act
            var result = reference1.Equals(reference2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsObject_WithIntegerObject_ShouldReturnFalse()
        {
            // Arrange
            var reference1 = new AssemblyReference("/path/to/assembly.dll");
            object reference2 = 42;

            // Act
            var result = reference1.Equals(reference2);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetHashCode Tests

        [Fact]
        public void GetHashCode_WithSamePath_ShouldReturnSameHashCode()
        {
            // Arrange
            var path = "/path/to/assembly.dll";
            var reference1 = new AssemblyReference(path);
            var reference2 = new AssemblyReference(path);

            // Act
            var hash1 = reference1.GetHashCode();
            var hash2 = reference2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_WithDifferentPaths_ShouldReturnDifferentHashCodes()
        {
            // Arrange
            var reference1 = new AssemblyReference("/path/to/assembly1.dll");
            var reference2 = new AssemblyReference("/path/to/assembly2.dll");

            // Act
            var hash1 = reference1.GetHashCode();
            var hash2 = reference2.GetHashCode();

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_WithCaseDifferenceOnWindows_ShouldReturnSameHashCodeOnWindows()
        {
            // Arrange
            var reference1 = new AssemblyReference(@"C:\Path\Assembly.dll");
            var reference2 = new AssemblyReference(@"C:\path\assembly.dll");

            // Act
            var hash1 = reference1.GetHashCode();
            var hash2 = reference2.GetHashCode();

            // Assert - On Windows: same hash, On Unix: different hash
            if (ScriptEnvironment.Default.IsWindows)
            {
                Assert.Equal(hash1, hash2);
            }
            else
            {
                Assert.NotEqual(hash1, hash2);
            }
        }

        [Fact]
        public void GetHashCode_WithNullPath_ShouldNotThrow()
        {
            // Arrange
            var reference = new AssemblyReference(null);

            // Act
            var hash = reference.GetHashCode();

            // Assert - Should not throw, just return a hash code
            Assert.IsType<int>(hash);
        }

        [Fact]
        public void GetHashCode_WithEmptyPath_ShouldReturnValidHashCode()
        {
            // Arrange
            var reference = new AssemblyReference(string.Empty);

            // Act
            var hash = reference.GetHashCode();

            // Assert
            Assert.IsType<int>(hash);
        }

        [Fact]
        public void GetHashCode_IsConsistent()
        {
            // Arrange
            var reference = new AssemblyReference("/path/to/assembly.dll");

            // Act
            var hash1 = reference.GetHashCode();
            var hash2 = reference.GetHashCode();
            var hash3 = reference.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
            Assert.Equal(hash2, hash3);
        }

        #endregion

        #region HashSet/Dictionary Integration Tests

        [Fact]
        public void ShouldWorkInHashSet_WithDuplicates()
        {
            // Arrange
            var path = "/path/to/assembly.dll";
            var reference1 = new AssemblyReference(path);
            var reference2 = new AssemblyReference(path);
            var hashSet = new System.Collections.Generic.HashSet<AssemblyReference>();

            // Act
            hashSet.Add(reference1);
            hashSet.Add(reference2);

            // Assert
            Assert.Single(hashSet);
        }

        [Fact]
        public void ShouldWorkInHashSet_WithDifferentPaths()
        {
            // Arrange
            var reference1 = new AssemblyReference("/path/to/assembly1.dll");
            var reference2 = new AssemblyReference("/path/to/assembly2.dll");
            var hashSet = new System.Collections.Generic.HashSet<AssemblyReference>();

            // Act
            hashSet.Add(reference1);
            hashSet.Add(reference2);

            // Assert
            Assert.Equal(2, hashSet.Count);
        }

        [Fact]
        public void ShouldWorkInDictionary_AsKey()
        {
            // Arrange
            var path = "/path/to/assembly.dll";
            var reference = new AssemblyReference(path);
            var dictionary = new System.Collections.Generic.Dictionary<AssemblyReference, string>();

            // Act
            dictionary.Add(reference, "value");

            // Assert
            Assert.True(dictionary.ContainsKey(new AssemblyReference(path)));
            Assert.Equal("value", dictionary[new AssemblyReference(path)]);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void AssemblyPath_WithSpecialCharacters_ShouldPreserve()
        {
            // Arrange
            var testPath = "/path/with spaces/assembly (2.0).dll";

            // Act
            var reference = new AssemblyReference(testPath);

            // Assert
            Assert.Equal(testPath, reference.AssemblyPath);
        }

        [Fact]
        public void AssemblyPath_WithUnicodeCharacters_ShouldPreserve()
        {
            // Arrange
            var testPath = "/path/with/café/assembly.dll";

            // Act
            var reference = new AssemblyReference(testPath);

            // Assert
            Assert.Equal(testPath, reference.AssemblyPath);
        }

        [Fact]
        public void AssemblyPath_WithVeryLongPath_ShouldWork()
        {
            // Arrange
            var testPath = "/" + string.Join("/", System.Linq.Enumerable.Repeat("verylongfoldername", 50)) + "/assembly.dll";

            // Act
            var reference = new AssemblyReference(testPath);

            // Assert
            Assert.Equal(testPath, reference.AssemblyPath);
        }

        #endregion
    }
}