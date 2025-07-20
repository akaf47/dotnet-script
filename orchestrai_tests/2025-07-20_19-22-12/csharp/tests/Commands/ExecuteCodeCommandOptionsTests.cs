using Xunit;
using Dotnet.Script.Core.Commands;

namespace Dotnet.Script.Core.Tests.Commands
{
    public class ExecuteCodeCommandOptionsTests
    {
        [Fact]
        public void Constructor_SetsDefaultValues()
        {
            // Act
            var options = new ExecuteCodeCommandOptions();

            // Assert
            Assert.Null(options.Code);
            Assert.Equal("Release", options.Configuration);
            Assert.False(options.Debug);
            Assert.Empty(options.PackageSources);
        }

        [Fact]
        public void Code_CanBeSetAndRetrieved()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions();
            var code = "Console.WriteLine(\"Test\");";

            // Act
            options.Code = code;

            // Assert
            Assert.Equal(code, options.Code);
        }

        [Fact]
        public void Configuration_CanBeSetAndRetrieved()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions();
            var configuration = "Debug";

            // Act
            options.Configuration = configuration;

            // Assert
            Assert.Equal(configuration, options.Configuration);
        }

        [Fact]
        public void Debug_CanBeSetAndRetrieved()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions();

            // Act
            options.Debug = true;

            // Assert
            Assert.True(options.Debug);
        }

        [Fact]
        public void PackageSources_CanBeSetAndRetrieved()
        {
            // Arrange
            var options = new ExecuteCodeCommandOptions();
            var packageSources = new[] { "https://api.nuget.org/v3/index.json" };

            // Act
            options.PackageSources = packageSources;

            // Assert
            Assert.Equal(packageSources, options.PackageSources);
        }
    }
}