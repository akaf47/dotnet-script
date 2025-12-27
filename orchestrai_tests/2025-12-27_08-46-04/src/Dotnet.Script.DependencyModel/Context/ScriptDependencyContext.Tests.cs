using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Dotnet.Script.DependencyModel.Context;

namespace Dotnet.Script.DependencyModel.Tests.Context
{
    public class ScriptDependencyContextTests
    {
        private readonly Mock<IScriptDependencyContext> _mockContext;

        public ScriptDependencyContextTests()
        {
            _mockContext = new Mock<IScriptDependencyContext>();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldInitializeWithValidParameters()
        {
            // Arrange
            var targetFramework = "net6.0";
            var projectFilePath = "/path/to/project.csproj";

            // Act
            var context = new ScriptDependencyContext(targetFramework, projectFilePath);

            // Assert
            Assert.NotNull(context);
            Assert.Equal(targetFramework, context.TargetFramework);
            Assert.Equal(projectFilePath, context.ProjectFilePath);
        }

        [Fact]
        public void Constructor_WithNullTargetFramework_ShouldThrowArgumentNullException()
        {
            // Arrange
            string targetFramework = null;
            var projectFilePath = "/path/to/project.csproj";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new ScriptDependencyContext(targetFramework, projectFilePath));
        }

        [Fact]
        public void Constructor_WithNullProjectFilePath_ShouldThrowArgumentNullException()
        {
            // Arrange
            var targetFramework = "net6.0";
            string projectFilePath = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new ScriptDependencyContext(targetFramework, projectFilePath));
        }

        [Fact]
        public void Constructor_WithEmptyTargetFramework_ShouldThrowArgumentException()
        {
            // Arrange
            var targetFramework = string.Empty;
            var projectFilePath = "/path/to/project.csproj";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new ScriptDependencyContext(targetFramework, projectFilePath));
        }

        [Fact]
        public void Constructor_WithEmptyProjectFilePath_ShouldThrowArgumentException()
        {
            // Arrange
            var targetFramework = "net6.0";
            var projectFilePath = string.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new ScriptDependencyContext(targetFramework, projectFilePath));
        }

        [Fact]
        public void Constructor_WithWhitespaceTargetFramework_ShouldThrowArgumentException()
        {
            // Arrange
            var targetFramework = "   ";
            var projectFilePath = "/path/to/project.csproj";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new ScriptDependencyContext(targetFramework, projectFilePath));
        }

        [Fact]
        public void Constructor_WithWhitespaceProjectFilePath_ShouldThrowArgumentException()
        {
            // Arrange
            var targetFramework = "net6.0";
            var projectFilePath = "   ";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new ScriptDependencyContext(targetFramework, projectFilePath));
        }

        #endregion

        #region Properties Tests

        [Fact]
        public void TargetFramework_ShouldReturnCorrectValue()
        {
            // Arrange
            var targetFramework = "net5.0";
            var context = new ScriptDependencyContext(targetFramework, "/path/to/project.csproj");

            // Act
            var result = context.TargetFramework;

            // Assert
            Assert.Equal(targetFramework, result);
        }

        [Fact]
        public void ProjectFilePath_ShouldReturnCorrectValue()
        {
            // Arrange
            var projectFilePath = "/custom/path/my.csproj";
            var context = new ScriptDependencyContext("net6.0", projectFilePath);

            // Act
            var result = context.ProjectFilePath;

            // Assert
            Assert.Equal(projectFilePath, result);
        }

        [Theory]
        [InlineData("net5.0")]
        [InlineData("net6.0")]
        [InlineData("net7.0")]
        [InlineData("netstandard2.1")]
        [InlineData("netcoreapp3.1")]
        public void Constructor_WithVariousTargetFrameworks_ShouldSucceed(string targetFramework)
        {
            // Arrange & Act
            var context = new ScriptDependencyContext(targetFramework, "/path/project.csproj");

            // Assert
            Assert.Equal(targetFramework, context.TargetFramework);
        }

        [Theory]
        [InlineData("/absolute/path/project.csproj")]
        [InlineData("relative/path/project.csproj")]
        [InlineData("C:\\Windows\\project.csproj")]
        [InlineData("./project.csproj")]
        public void Constructor_WithVariousProjectPaths_ShouldSucceed(string projectPath)
        {
            // Arrange & Act
            var context = new ScriptDependencyContext("net6.0", projectPath);

            // Assert
            Assert.Equal(projectPath, context.ProjectFilePath);
        }

        #endregion

        #region Method Tests - GetDependencies

        [Fact]
        public void GetDependencies_ShouldReturnEmptyListWhenNoDependencies()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act
            var dependencies = context.GetDependencies();

            // Assert
            Assert.NotNull(dependencies);
            Assert.Empty(dependencies);
        }

        [Fact]
        public void GetDependencies_ShouldReturnAllAddedDependencies()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            var dep1 = "Newtonsoft.Json/13.0.0";
            var dep2 = "System.Collections.Immutable/5.0.0";

            context.AddDependency(dep1);
            context.AddDependency(dep2);

            // Act
            var dependencies = context.GetDependencies();

            // Assert
            Assert.NotNull(dependencies);
            Assert.Contains(dep1, dependencies);
            Assert.Contains(dep2, dependencies);
            Assert.Equal(2, dependencies.Count());
        }

        [Fact]
        public void GetDependencies_ShouldReturnImmutableCollection()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            context.AddDependency("Newtonsoft.Json/13.0.0");

            // Act
            var dependencies1 = context.GetDependencies();
            var dependencies2 = context.GetDependencies();

            // Assert
            Assert.NotSame(dependencies1, dependencies2);
        }

        #endregion

        #region Method Tests - AddDependency

        [Fact]
        public void AddDependency_WithValidDependency_ShouldSucceed()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            var dependency = "Newtonsoft.Json/13.0.0";

            // Act
            context.AddDependency(dependency);
            var dependencies = context.GetDependencies();

            // Assert
            Assert.Contains(dependency, dependencies);
        }

        [Fact]
        public void AddDependency_WithNullDependency_ShouldThrowArgumentNullException()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => context.AddDependency(null));
        }

        [Fact]
        public void AddDependency_WithEmptyDependency_ShouldThrowArgumentException()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => context.AddDependency(string.Empty));
        }

        [Fact]
        public void AddDependency_WithWhitespaceDependency_ShouldThrowArgumentException()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => context.AddDependency("   "));
        }

        [Fact]
        public void AddDependency_WithDuplicateDependency_ShouldNotAddDuplicate()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            var dependency = "Newtonsoft.Json/13.0.0";

            // Act
            context.AddDependency(dependency);
            context.AddDependency(dependency);
            var dependencies = context.GetDependencies();

            // Assert
            Assert.Single(dependencies.Where(d => d == dependency));
        }

        [Fact]
        public void AddDependency_MultipleDependencies_ShouldAddAll()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            var dependencies = new[] 
            { 
                "Newtonsoft.Json/13.0.0",
                "System.Collections.Immutable/5.0.0",
                "Microsoft.Extensions.Logging/6.0.0"
            };

            // Act
            foreach (var dep in dependencies)
            {
                context.AddDependency(dep);
            }
            var result = context.GetDependencies();

            // Assert
            Assert.Equal(3, result.Count());
            foreach (var dep in dependencies)
            {
                Assert.Contains(dep, result);
            }
        }

        #endregion

        #region Method Tests - RemoveDependency

        [Fact]
        public void RemoveDependency_WithExistingDependency_ShouldRemove()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            var dependency = "Newtonsoft.Json/13.0.0";
            context.AddDependency(dependency);

            // Act
            context.RemoveDependency(dependency);
            var dependencies = context.GetDependencies();

            // Assert
            Assert.DoesNotContain(dependency, dependencies);
        }

        [Fact]
        public void RemoveDependency_WithNonExistentDependency_ShouldNotThrow()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            var dependency = "NonExistent/1.0.0";

            // Act & Assert (should not throw)
            context.RemoveDependency(dependency);
            Assert.Empty(context.GetDependencies());
        }

        [Fact]
        public void RemoveDependency_WithNullDependency_ShouldThrowArgumentNullException()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => context.RemoveDependency(null));
        }

        [Fact]
        public void RemoveDependency_WithEmptyDependency_ShouldThrowArgumentException()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => context.RemoveDependency(string.Empty));
        }

        [Fact]
        public void RemoveDependency_ShouldOnlyRemoveSpecificDependency()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            var dep1 = "Newtonsoft.Json/13.0.0";
            var dep2 = "System.Collections.Immutable/5.0.0";
            context.AddDependency(dep1);
            context.AddDependency(dep2);

            // Act
            context.RemoveDependency(dep1);
            var dependencies = context.GetDependencies();

            // Assert
            Assert.DoesNotContain(dep1, dependencies);
            Assert.Contains(dep2, dependencies);
            Assert.Single(dependencies);
        }

        #endregion

        #region Method Tests - ClearDependencies

        [Fact]
        public void ClearDependencies_WithExistingDependencies_ShouldRemoveAll()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            context.AddDependency("Dep1/1.0.0");
            context.AddDependency("Dep2/1.0.0");
            context.AddDependency("Dep3/1.0.0");

            // Act
            context.ClearDependencies();
            var dependencies = context.GetDependencies();

            // Assert
            Assert.Empty(dependencies);
        }

        [Fact]
        public void ClearDependencies_WithNoDependencies_ShouldSucceed()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act & Assert (should not throw)
            context.ClearDependencies();
            Assert.Empty(context.GetDependencies());
        }

        [Fact]
        public void ClearDependencies_ShouldAllowAddingDependenciesAfterClear()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            context.AddDependency("Dep1/1.0.0");
            context.ClearDependencies();

            // Act
            context.AddDependency("NewDep/1.0.0");
            var dependencies = context.GetDependencies();

            // Assert
            Assert.Single(dependencies);
            Assert.Contains("NewDep/1.0.0", dependencies);
        }

        #endregion

        #region Method Tests - ContainsDependency

        [Fact]
        public void ContainsDependency_WithExistingDependency_ShouldReturnTrue()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            var dependency = "Newtonsoft.Json/13.0.0";
            context.AddDependency(dependency);

            // Act
            var result = context.ContainsDependency(dependency);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ContainsDependency_WithNonExistentDependency_ShouldReturnFalse()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act
            var result = context.ContainsDependency("NonExistent/1.0.0");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ContainsDependency_WithNullDependency_ShouldThrowArgumentNullException()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => context.ContainsDependency(null));
        }

        [Fact]
        public void ContainsDependency_ShouldBeCaseSensitive()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            context.AddDependency("Newtonsoft.Json/13.0.0");

            // Act
            var resultExact = context.ContainsDependency("Newtonsoft.Json/13.0.0");
            var resultDifferentCase = context.ContainsDependency("newtonsoft.json/13.0.0");

            // Assert
            Assert.True(resultExact);
            Assert.False(resultDifferentCase);
        }

        #endregion

        #region Method Tests - GetDependencyCount

        [Fact]
        public void GetDependencyCount_WithNoDependencies_ShouldReturnZero()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act
            var count = context.GetDependencyCount();

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void GetDependencyCount_WithAddedDependencies_ShouldReturnCorrectCount()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            context.AddDependency("Dep1/1.0.0");
            context.AddDependency("Dep2/1.0.0");
            context.AddDependency("Dep3/1.0.0");

            // Act
            var count = context.GetDependencyCount();

            // Assert
            Assert.Equal(3, count);
        }

        [Fact]
        public void GetDependencyCount_AfterRemovingDependency_ShouldReturnUpdatedCount()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            context.AddDependency("Dep1/1.0.0");
            context.AddDependency("Dep2/1.0.0");

            // Act
            context.RemoveDependency("Dep1/1.0.0");
            var count = context.GetDependencyCount();

            // Assert
            Assert.Equal(1, count);
        }

        #endregion

        #region Method Tests - ToString

        [Fact]
        public void ToString_ShouldReturnValidString()
        {
            // Arrange
            var targetFramework = "net6.0";
            var projectPath = "/path/to/project.csproj";
            var context = new ScriptDependencyContext(targetFramework, projectPath);

            // Act
            var result = context.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(targetFramework, result);
            Assert.Contains(projectPath, result);
        }

        [Fact]
        public void ToString_WithDependencies_ShouldIncludeDependencyInfo()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            context.AddDependency("Newtonsoft.Json/13.0.0");

            // Act
            var result = context.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        #endregion

        #region Method Tests - Equality

        [Fact]
        public void Equals_WithSameValues_ShouldReturnTrue()
        {
            // Arrange
            var context1 = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            var context2 = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act
            var result = context1.Equals(context2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentTargetFramework_ShouldReturnFalse()
        {
            // Arrange
            var context1 = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            var context2 = new ScriptDependencyContext("net5.0", "/path/to/project.csproj");

            // Act
            var result = context1.Equals(context2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithDifferentProjectPath_ShouldReturnFalse()
        {
            // Arrange
            var context1 = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            var context2 = new ScriptDependencyContext("net6.0", "/different/path/project.csproj");

            // Act
            var result = context1.Equals(context2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act
            var result = context.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithDifferentType_ShouldReturnFalse()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            var other = "not a context";

            // Act
            var result = context.Equals(other);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void WorkflowScenario_AddRemoveMultipleDependencies()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");

            // Act - Add dependencies
            context.AddDependency("Dep1/1.0.0");
            context.AddDependency("Dep2/1.0.0");
            context.AddDependency("Dep3/1.0.0");
            Assert.Equal(3, context.GetDependencyCount());

            // Act - Remove one dependency
            context.RemoveDependency("Dep2/1.0.0");
            Assert.Equal(2, context.GetDependencyCount());

            // Assert
            Assert.True(context.ContainsDependency("Dep1/1.0.0"));
            Assert.False(context.ContainsDependency("Dep2/1.0.0"));
            Assert.True(context.ContainsDependency("Dep3/1.0.0"));
        }

        [Fact]
        public void WorkflowScenario_ClearAndReaddDependencies()
        {
            // Arrange
            var context = new ScriptDependencyContext("net6.0", "/path/to/project.csproj");
            context.AddDependency("Dep1/1.0.0");

            // Act
            context.ClearDependencies();
            Assert.Empty(context.GetDependencies());

            context.AddDependency("NewDep/2.0.0");

            // Assert
            Assert.Single(context.GetDependencies());
            Assert.True(context.ContainsDependency("NewDep/2.0.0"));
        }

        #endregion
    }
}