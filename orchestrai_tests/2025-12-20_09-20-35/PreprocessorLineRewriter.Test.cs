using NUnit.Framework;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Dotnet.Script;

[TestFixture]
public class PreprocessorLineRewriterTests
{
    private PreprocessorLineRewriter _rewriter;

    [SetUp]
    public void Setup()
    {
        _rewriter = new PreprocessorLineRewriter();
    }

    [Test]
    public void VisitLoadDirectiveTrivia_WithSkippedTrivia_HandlesSkippedTokens()
    {
        // Arrange
        var syntaxTree = CSharpSyntaxTree.ParseText(@"#load ""script.csx"";");
        var loadDirective = syntaxTree.GetRoot().DescendantNodes().OfType<LoadDirectiveTriviaSyntax>().First();

        // Act
        var rewrittenNode = _rewriter.VisitLoadDirectiveTrivia(loadDirective);

        // Assert
        Assert.IsNotNull(rewrittenNode);
        Assert.IsInstanceOf<LoadDirectiveTriviaSyntax>(rewrittenNode);
    }

    [Test]
    public void VisitReferenceDirectiveTrivia_WithSkippedTrivia_HandlesSkippedTokens()
    {
        // Arrange
        var syntaxTree = CSharpSyntaxTree.ParseText(@"#r ""nuget:SomePackage"";");
        var referenceDirective = syntaxTree.GetRoot().DescendantNodes().OfType<ReferenceDirectiveTriviaSyntax>().First();

        // Act
        var rewrittenNode = _rewriter.VisitReferenceDirectiveTrivia(referenceDirective);

        // Assert
        Assert.IsNotNull(rewrittenNode);
        Assert.IsInstanceOf<ReferenceDirectiveTriviaSyntax>(rewrittenNode);
    }

    [Test]
    public void HandleSkippedTrivia_WithBadToken_RemovesBadToken()
    {
        // Arrange
        var syntaxTree = CSharpSyntaxTree.ParseText(@"#load ""script.csx"";");
        var loadDirective = syntaxTree.GetRoot().DescendantNodes().OfType<LoadDirectiveTriviaSyntax>().First();

        // Use reflection to call private method
        var method = typeof(PreprocessorLineRewriter).GetMethod("HandleSkippedTrivia", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        // Act
        var rewrittenNode = (SyntaxNode)method.Invoke(null, new object[] { loadDirective });

        // Assert
        Assert.IsNotNull(rewrittenNode);
        Assert.IsInstanceOf<LoadDirectiveTriviaSyntax>(rewrittenNode);
    }
}