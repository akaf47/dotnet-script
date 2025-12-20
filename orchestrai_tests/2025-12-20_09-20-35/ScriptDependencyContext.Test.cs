using NUnit.Framework;
using Dotnet.Script.DependencyModel.Context;

[TestFixture]
public class ScriptDependencyContextTests
{
    [Test]
    public void Constructor_WithDependencies_ShouldSetDependenciesProperty()
    {
        // Arrange
        var dependencies = new[]
        {
            new ScriptDependency("Package1", "1.0.0", new[] { "path1" }, new[] { "script1" }),
            new ScriptDependency("Package2", "2.0.0", new[] { "path2" }, new[] { "script2" })
        };

        // Act
        var context = new ScriptDependencyContext(dependencies);

        // Assert
        Assert.AreEqual(dependencies, context.Dependencies);
        Assert.AreEqual(2, context.Dependencies.Length);
    }

    [Test]
    public void Constructor_WithEmptyDependencies_ShouldCreateEmptyArray()
    {
        // Arrange
        var dependencies = Array.Empty<ScriptDependency>();

        // Act
        var context = new ScriptDependencyContext(dependencies);

        // Assert
        Assert.IsNotNull(context.Dependencies);
        Assert.AreEqual(0, context.Dependencies.Length);
    }
}