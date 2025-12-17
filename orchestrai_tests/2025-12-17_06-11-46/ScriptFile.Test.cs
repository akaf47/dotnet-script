using System;
using System.IO;
using Xunit;
using Moq;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptFileTests
    {
        [Fact]
        public void Constructor_ShouldSetPathAndIsRemote()
        {
            var scriptFile = new ScriptFile("http://example.com/script.cs");

            Assert.Equal("http://example.com/script.cs", scriptFile.Path);
            Assert.True(scriptFile.IsRemote);
            Assert.Null(scriptFile.Directory);
        }

        [Fact]
        public void Constructor_ShouldSetLocalFileProperties()
        {
            var tempFile = Path.GetTempFileName();
            try 
            {
                var scriptFile = new ScriptFile(tempFile);

                Assert.Equal(tempFile, scriptFile.Path);
                Assert.False(scriptFile.IsRemote);
                Assert.Equal(Path.GetDirectoryName(tempFile), scriptFile.Directory);
            }
            finally 
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void Constructor_ShouldThrowWhenLocalFileNotFound()
        {
            var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".cs");

            Assert.Throws<Exception>(() => new ScriptFile(nonExistentPath));
        }

        [Fact]
        public void HasValue_ShouldReturnTrueForNonEmptyPath()
        {
            var scriptFile = new ScriptFile("test.cs");
            Assert.True(scriptFile.HasValue);
        }

        [Fact]
        public void HasValue_ShouldReturnFalseForEmptyPath()
        {
            var scriptFile = new ScriptFile("");
            Assert.False(scriptFile.HasValue);
        }

        [Fact]
        public void IsHttpUri_ShouldDetectHttpAndHttpsUris()
        {
            var httpUri = "http://example.com";
            var httpsUri = "https://example.com";
            var nonUri = "local/path";

            var scriptFileHttp = new ScriptFile(httpUri);
            var scriptFileHttps = new ScriptFile(httpsUri);
            var scriptFileLocal = new ScriptFile(nonUri);

            Assert.True(scriptFileHttp.IsRemote);
            Assert.True(scriptFileHttps.IsRemote);
            Assert.False(scriptFileLocal.IsRemote);
        }
    }
}