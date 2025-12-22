using System;
using System.IO;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class DisposableFolderTests
    {
        [Fact]
        public void ConstructorCreatesDirectory()
        {
            DisposableFolder folder = null;
            try
            {
                folder = new DisposableFolder();
                
                Assert.NotNull(folder.Path);
                Assert.NotEmpty(folder.Path);
                Assert.True(Directory.Exists(folder.Path));
            }
            finally
            {
                folder?.Dispose();
            }
        }

        [Fact]
        public void PathPropertyReturnsValidPath()
        {
            using var folder = new DisposableFolder();
            
            Assert.NotNull(folder.Path);
            Assert.NotEmpty(folder.Path);
            Assert.True(Path.IsPathRooted(folder.Path) || !string.IsNullOrEmpty(folder.Path));
        }

        [Fact]
        public void PathPropertyReturnsConsistentPath()
        {
            using var folder = new DisposableFolder();
            var path1 = folder.Path;
            var path2 = folder.Path;
            
            Assert.Equal(path1, path2);
        }

        [Fact]
        public void DisposalRemovesDirectory()
        {
            var folder = new DisposableFolder();
            var path = folder.Path;
            
            Assert.True(Directory.Exists(path));
            
            folder.Dispose();
            
            Assert.False(Directory.Exists(path));
        }

        [Fact]
        public void DisposalCanBeCalledMultipleTimes()
        {
            var folder = new DisposableFolder();
            var path = folder.Path;
            
            // First disposal
            folder.Dispose();
            Assert.False(Directory.Exists(path));
            
            // Second disposal should not throw
            folder.Dispose();
            Assert.False(Directory.Exists(path));
        }

        [Fact]
        public void CanCreateFilesInFolder()
        {
            using var folder = new DisposableFolder();
            var testFile = Path.Combine(folder.Path, "test.txt");
            
            File.WriteAllText(testFile, "content");
            
            Assert.True(File.Exists(testFile));
            Assert.Equal("content", File.ReadAllText(testFile));
        }

        [Fact]
        public void CanCreateSubdirectories()
        {
            using var folder = new DisposableFolder();
            var subDir = Path.Combine(folder.Path, "subdir");
            
            Directory.CreateDirectory(subDir);
            
            Assert.True(Directory.Exists(subDir));
        }

        [Fact]
        public void DisposalRemovesDirectoryAndContents()
        {
            var folder = new DisposableFolder();
            var path = folder.Path;
            var testFile = Path.Combine(path, "test.txt");
            var subDir = Path.Combine(path, "subdir");
            
            File.WriteAllText(testFile, "content");
            Directory.CreateDirectory(subDir);
            File.WriteAllText(Path.Combine(subDir, "nested.txt"), "nested");
            
            Assert.True(Directory.Exists(path));
            Assert.True(File.Exists(testFile));
            
            folder.Dispose();
            
            Assert.False(Directory.Exists(path));
        }

        [Fact]
        public void UseInUsingStatement()
        {
            string folderPath;
            
            using (var folder = new DisposableFolder())
            {
                folderPath = folder.Path;
                Assert.True(Directory.Exists(folderPath));
            }
            
            Assert.False(Directory.Exists(folderPath));
        }

        [Fact]
        public void MultipleInstancesCreateDifferentPaths()
        {
            using var folder1 = new DisposableFolder();
            using var folder2 = new DisposableFolder();
            
            Assert.NotEqual(folder1.Path, folder2.Path);
            Assert.True(Directory.Exists(folder1.Path));
            Assert.True(Directory.Exists(folder2.Path));
        }
    }
}