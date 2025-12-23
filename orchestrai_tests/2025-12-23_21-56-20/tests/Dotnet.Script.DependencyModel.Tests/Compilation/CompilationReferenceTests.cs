using Xunit;
using Dotnet.Script.DependencyModel.Compilation;

namespace Dotnet.Script.DependencyModel.Tests.Compilation
{
    public class CompilationReferenceTests
    {
        [Fact]
        public void Constructor_WithValidPath_SetsPathProperty()
        {
            // Arrange
            const string expectedPath = "/usr/local/lib/assembly.dll";

            // Act
            var reference = new CompilationReference(expectedPath);

            // Assert
            Assert.NotNull(reference);
            Assert.Equal(expectedPath, reference.Path);
        }

        [Fact]
        public void Constructor_WithEmptyString_SetsPathProperty()
        {
            // Arrange
            const string expectedPath = "";

            // Act
            var reference = new CompilationReference(expectedPath);

            // Assert
            Assert.NotNull(reference);
            Assert.Equal(expectedPath, reference.Path);
        }

        [Fact]
        public void Constructor_WithNullPath_SetsPathToNull()
        {
            // Arrange
            const string expectedPath = null;

            // Act
            var reference = new CompilationReference(expectedPath);

            // Assert
            Assert.NotNull(reference);
            Assert.Null(reference.Path);
        }

        [Fact]
        public void Path_IsReadOnly()
        {
            // Arrange
            const string path = "/path/to/assembly.dll";
            var reference = new CompilationReference(path);

            // Act & Assert
            Assert.Equal(path, reference.Path);
            // Verify that Path property has no setter by attempting to access it
            var propertyInfo = typeof(CompilationReference).GetProperty("Path");
            Assert.NotNull(propertyInfo);
            Assert.Null(propertyInfo.SetMethod);
        }

        [Fact]
        public void Constructor_WithLongPath_SetsPathProperty()
        {
            // Arrange
            const string expectedPath = "/very/long/path/to/deep/nested/directory/structure/assembly.dll";

            // Act
            var reference = new CompilationReference(expectedPath);

            // Assert
            Assert.Equal(expectedPath, reference.Path);
        }

        [Fact]
        public void Constructor_WithSpecialCharactersInPath_SetsPathProperty()
        {
            // Arrange
            const string expectedPath = "/path/with spaces/and-dashes/assembly (1).dll";

            // Act
            var reference = new CompilationReference(expectedPath);

            // Assert
            Assert.Equal(expectedPath, reference.Path);
        }

        [Fact]
        public void Constructor_WithWindowsPath_SetsPathProperty()
        {
            // Arrange
            const string expectedPath = "C:\\Users\\name\\AppData\\Local\\assembly.dll";

            // Act
            var reference = new CompilationReference(expectedPath);

            // Assert
            Assert.Equal(expectedPath, reference.Path);
        }

        [Fact]
        public void Constructor_WithRelativePath_SetsPathProperty()
        {
            // Arrange
            const string expectedPath = "../relative/path/assembly.dll";

            // Act
            var reference = new CompilationReference(expectedPath);

            // Assert
            Assert.Equal(expectedPath, reference.Path);
        }

        [Fact]
        public void MultipleInstances_HaveDifferentPaths()
        {
            // Arrange
            const string path1 = "/path/to/assembly1.dll";
            const string path2 = "/path/to/assembly2.dll";

            // Act
            var reference1 = new CompilationReference(path1);
            var reference2 = new CompilationReference(path2);

            // Assert
            Assert.NotEqual(reference1.Path, reference2.Path);
            Assert.Equal(path1, reference1.Path);
            Assert.Equal(path2, reference2.Path);
        }
    }
}