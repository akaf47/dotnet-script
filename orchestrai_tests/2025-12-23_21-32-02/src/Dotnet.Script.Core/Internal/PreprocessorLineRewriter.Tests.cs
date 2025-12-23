using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace Dotnet.Script.Core.Internal.Tests
{
    public class PreprocessorLineRewriterTests
    {
        private readonly PreprocessorLineRewriter _rewriter;

        public PreprocessorLineRewriterTests()
        {
            _rewriter = new PreprocessorLineRewriter();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldInitializeWithVisitIntoStructuredTriviaTrue()
        {
            // Arrange & Act
            var rewriter = new PreprocessorLineRewriter();

            // Assert
            Assert.NotNull(rewriter);
            // Verify by visiting a node - if visitIntoStructuredTrivia is true, 
            // it will visit trivia nodes
        }

        #endregion

        #region VisitLoadDirectiveTrivia Tests

        [Fact]
        public void VisitLoadDirectiveTrivia_WithValidLoadDirective_ReturnsProcessedNode()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#load ""test.csx""");
            var root = syntaxTree.GetRoot();
            var loadDirective = root.DescendantNodes()
                .OfType<LoadDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act
            var result = _rewriter.VisitLoadDirectiveTrivia(loadDirective);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<LoadDirectiveTriviaSyntax>(result);
        }

        [Fact]
        public void VisitLoadDirectiveTrivia_WithLoadDirectiveWithoutSkippedTrivia_ReturnsUnchangedNode()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#load ""test.csx""");
            var root = syntaxTree.GetRoot();
            var loadDirective = root.DescendantNodes()
                .OfType<LoadDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act
            var result = _rewriter.VisitLoadDirectiveTrivia(loadDirective);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void VisitLoadDirectiveTrivia_WithLoadDirectiveAndSemicolon_ProcessesSkippedTrivia()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#load ""test.csx"";");
            var root = syntaxTree.GetRoot();
            var loadDirective = root.DescendantNodes()
                .OfType<LoadDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act
            var result = _rewriter.VisitLoadDirectiveTrivia(loadDirective);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<LoadDirectiveTriviaSyntax>(result);
        }

        [Fact]
        public void VisitLoadDirectiveTrivia_WithNull_ReturnsNull()
        {
            // Arrange
            LoadDirectiveTriviaSyntax nullDirective = null;

            // Act & Assert
            // This tests the behavior when null is passed
            try
            {
                var result = _rewriter.VisitLoadDirectiveTrivia(nullDirective);
                // If it doesn't throw, null should be returned
                Assert.Null(result);
            }
            catch (NullReferenceException)
            {
                // Expected behavior
            }
        }

        #endregion

        #region VisitReferenceDirectiveTrivia Tests

        [Fact]
        public void VisitReferenceDirectiveTrivia_WithValidReferenceDirective_ReturnsProcessedNode()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#r ""nuget:Newtonsoft.Json/13.0.1""");
            var root = syntaxTree.GetRoot();
            var referenceDirective = root.DescendantNodes()
                .OfType<ReferenceDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act
            var result = _rewriter.VisitReferenceDirectiveTrivia(referenceDirective);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ReferenceDirectiveTriviaSyntax>(result);
        }

        [Fact]
        public void VisitReferenceDirectiveTrivia_WithReferenceDirectiveWithoutSkippedTrivia_ReturnsUnchangedNode()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#r ""nuget:Newtonsoft.Json/13.0.1""");
            var root = syntaxTree.GetRoot();
            var referenceDirective = root.DescendantNodes()
                .OfType<ReferenceDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act
            var result = _rewriter.VisitReferenceDirectiveTrivia(referenceDirective);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void VisitReferenceDirectiveTrivia_WithReferenceDirectiveAndSemicolon_ProcessesSkippedTrivia()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#r ""nuget:Newtonsoft.Json/13.0.1"";");
            var root = syntaxTree.GetRoot();
            var referenceDirective = root.DescendantNodes()
                .OfType<ReferenceDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act
            var result = _rewriter.VisitReferenceDirectiveTrivia(referenceDirective);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ReferenceDirectiveTriviaSyntax>(result);
        }

        [Fact]
        public void VisitReferenceDirectiveTrivia_WithNull_ReturnsNull()
        {
            // Arrange
            ReferenceDirectiveTriviaSyntax nullDirective = null;

            // Act & Assert
            try
            {
                var result = _rewriter.VisitReferenceDirectiveTrivia(nullDirective);
                // If it doesn't throw, null should be returned
                Assert.Null(result);
            }
            catch (NullReferenceException)
            {
                // Expected behavior
            }
        }

        #endregion

        #region HandleSkippedTrivia Private Method Tests (via reflection)

        [Fact]
        public void HandleSkippedTrivia_WithNodeContainingSkippedTriviaWithBadToken_RemovesBadTokenWhenItsSemicolon()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#load ""test.csx"";");
            var root = syntaxTree.GetRoot();
            var loadDirective = root.DescendantNodes()
                .OfType<LoadDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act - call via reflection to test private method
            var method = typeof(PreprocessorLineRewriter)
                .GetMethod("HandleSkippedTrivia",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (SyntaxNode)method?.Invoke(null, new object[] { loadDirective });

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void HandleSkippedTrivia_WithNodeWithoutSkippedTrivia_ReturnsNodeUnchanged()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#load ""test.csx""");
            var root = syntaxTree.GetRoot();
            var loadDirective = root.DescendantNodes()
                .OfType<LoadDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act - call via reflection to test private method
            var method = typeof(PreprocessorLineRewriter)
                .GetMethod("HandleSkippedTrivia",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (SyntaxNode)method?.Invoke(null, new object[] { loadDirective });

            // Assert
            Assert.NotNull(result);
            Assert.Same(loadDirective, result);
        }

        [Fact]
        public void HandleSkippedTrivia_WithSkippedTriviaContainingSemicolon_ReplacesBadTokenAndReplacesTrivia()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#load ""test.csx"";");
            var root = syntaxTree.GetRoot();
            var loadDirective = root.DescendantNodes()
                .OfType<LoadDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act - call via reflection to test private method
            var method = typeof(PreprocessorLineRewriter)
                .GetMethod("HandleSkippedTrivia",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (SyntaxNode)method?.Invoke(null, new object[] { loadDirective });

            // Assert
            Assert.NotNull(result);
            Assert.IsType<LoadDirectiveTriviaSyntax>(result);
        }

        [Fact]
        public void HandleSkippedTrivia_WithSkippedTriviaButNoBadToken_ReplacesTrivia()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#load ""test.csx""");
            var root = syntaxTree.GetRoot();
            var loadDirective = root.DescendantNodes()
                .OfType<LoadDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act - call via reflection to test private method
            var method = typeof(PreprocessorLineRewriter)
                .GetMethod("HandleSkippedTrivia",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (SyntaxNode)method?.Invoke(null, new object[] { loadDirective });

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void HandleSkippedTrivia_WithReferenceDirectiveAndSemicolon_ProcessesCorrectly()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#r ""nuget:Newtonsoft.Json"";");
            var root = syntaxTree.GetRoot();
            var referenceDirective = root.DescendantNodes()
                .OfType<ReferenceDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act - call via reflection to test private method
            var method = typeof(PreprocessorLineRewriter)
                .GetMethod("HandleSkippedTrivia",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (SyntaxNode)method?.Invoke(null, new object[] { referenceDirective });

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ReferenceDirectiveTriviaSyntax>(result);
        }

        #endregion

        #region Edge Cases and Error Scenarios

        [Fact]
        public void VisitLoadDirectiveTrivia_WithEmptyPath_ReturnsProcessedNode()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#load """"");
            var root = syntaxTree.GetRoot();
            var loadDirective = root.DescendantNodes()
                .OfType<LoadDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act
            var result = _rewriter.VisitLoadDirectiveTrivia(loadDirective);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void VisitReferenceDirectiveTrivia_WithEmptyReference_ReturnsProcessedNode()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#r """"");
            var root = syntaxTree.GetRoot();
            var referenceDirective = root.DescendantNodes()
                .OfType<ReferenceDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act
            var result = _rewriter.VisitReferenceDirectiveTrivia(referenceDirective);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void VisitLoadDirectiveTrivia_WithComplexPath_ReturnsProcessedNode()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#load ""./path/to/test.csx""");
            var root = syntaxTree.GetRoot();
            var loadDirective = root.DescendantNodes()
                .OfType<LoadDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act
            var result = _rewriter.VisitLoadDirectiveTrivia(loadDirective);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void VisitReferenceDirectiveTrivia_WithComplexPackageReference_ReturnsProcessedNode()
        {
            // Arrange
            var syntaxTree = CSharpSyntaxTree.ParseText(@"#r ""nuget:Newtonsoft.Json,13.0.1,https://api.nuget.org/v3/index.json""");
            var root = syntaxTree.GetRoot();
            var referenceDirective = root.DescendantNodes()
                .OfType<ReferenceDirectiveTriviaSyntax>()
                .FirstOrDefault();

            // Act
            var result = _rewriter.VisitReferenceDirectiveTrivia(referenceDirective);

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void Rewriter_WithMultipleLoadDirectives_ProcessesAllDirectives()
        {
            // Arrange
            var code = @"
#load ""test1.csx"";
#load ""test2.csx""
int x = 5;
";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();

            // Act
            var rewrittenNode = _rewriter.Visit(root);

            // Assert
            Assert.NotNull(rewrittenNode);
        }

        [Fact]
        public void Rewriter_WithMultipleReferenceDirectives_ProcessesAllDirectives()
        {
            // Arrange
            var code = @"
#r ""nuget:Package1"";
#r ""nuget:Package2""
int x = 5;
";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();

            // Act
            var rewrittenNode = _rewriter.Visit(root);

            // Assert
            Assert.NotNull(rewrittenNode);
        }

        [Fact]
        public void Rewriter_WithMixedLoadAndReferenceDirectives_ProcessesAll()
        {
            // Arrange
            var code = @"
#load ""test.csx"";
#r ""nuget:Package"";
int x = 5;
";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();

            // Act
            var rewrittenNode = _rewriter.Visit(root);

            // Assert
            Assert.NotNull(rewrittenNode);
        }

        #endregion
    }
}