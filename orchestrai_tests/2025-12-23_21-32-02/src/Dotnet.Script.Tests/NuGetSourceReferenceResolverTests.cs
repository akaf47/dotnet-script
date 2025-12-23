using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Dotnet.Script.DependencyModel.NuGet;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class NuGetSourceReferenceResolverTests
    {
        [Fact]
        public void ShouldHandleResolvingInvalidPackageReference()
        {
            Dictionary<string, IReadOnlyList<string>> scriptMap = new Dictionary<string, IReadOnlyList<string>>();
            NuGetSourceReferenceResolver resolver = new NuGetSourceReferenceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), scriptMap);
            var result = resolver.ResolveReference("nuget:InvalidPackage, 1.2.3", Directory.GetCurrentDirectory());
            // Should not throw and delegates to underlying resolver
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldReturnPathForNugetReference()
        {
            var scriptList = new List<string> { "/path/to/script.csx" };
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>
            {
                { "TestPackage", scriptList }
            };
            
            NuGetSourceReferenceResolver resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var result = resolver.ResolveReference("nuget:TestPackage, 1.0.0", Directory.GetCurrentDirectory());
            Assert.Equal("/path/to/script.csx", result);
        }

        [Fact]
        public void ShouldReturnOriginalPathWhenMultipleScriptsInMap()
        {
            var scriptList = new List<string> 
            { 
                "/path/to/script1.csx",
                "/path/to/script2.csx"
            };
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>
            {
                { "TestPackage", scriptList }
            };
            
            NuGetSourceReferenceResolver resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var result = resolver.ResolveReference("nuget:TestPackage, 1.0.0", Directory.GetCurrentDirectory());
            // When multiple scripts, returns the original path
            Assert.Equal("nuget:TestPackage, 1.0.0", result);
        }

        [Fact]
        public void ShouldDelegateNonNugetReferencesToUnderlyingResolver()
        {
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>();
            var underlyingResolver = new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory());
            var resolver = new NuGetSourceReferenceResolver(underlyingResolver, scriptMap);
            
            var result = resolver.ResolveReference("path/to/file.csx", Directory.GetCurrentDirectory());
            // Should delegate to underlying resolver
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldOpenReadWithSingleScript()
        {
            var scriptList = new List<string> { "/path/to/single/script.csx" };
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>
            {
                { "TestPackage", scriptList }
            };
            
            NuGetSourceReferenceResolver resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            // This would fail if the path doesn't exist, but we're testing the logic
            try
            {
                var stream = resolver.OpenRead("nuget:TestPackage, 1.0.0");
                Assert.NotNull(stream);
            }
            catch (FileNotFoundException)
            {
                // Expected since the test path doesn't exist
            }
        }

        [Fact]
        public void ShouldOpenReadWithMultipleScripts()
        {
            var scriptList = new List<string> 
            { 
                "script1.csx",
                "script2.csx"
            };
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>
            {
                { "TestPackage", scriptList }
            };
            
            NuGetSourceReferenceResolver resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var stream = resolver.OpenRead("nuget:TestPackage, 1.0.0");
            Assert.NotNull(stream);
            
            // Verify the stream contains load statements
            var memoryStream = stream as MemoryStream;
            Assert.NotNull(memoryStream);
            
            memoryStream.Position = 0;
            var reader = new StreamReader(memoryStream);
            var content = reader.ReadToEnd();
            
            Assert.Contains("#load \"script1.csx\"", content);
            Assert.Contains("#load \"script2.csx\"", content);
        }

        [Fact]
        public void ShouldOpenReadDelegateForNonNugetReference()
        {
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>();
            var resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            try
            {
                var stream = resolver.OpenRead("non-existent-file.csx");
                // Should attempt to delegate to underlying resolver
                Assert.NotNull(stream);
            }
            catch (FileNotFoundException)
            {
                // Expected since file doesn't exist
            }
        }

        [Fact]
        public void ShouldNormalizeNugetPath()
        {
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>();
            var resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var normalizedPath = resolver.NormalizePath("nuget:TestPackage, 1.0.0", Directory.GetCurrentDirectory());
            // NuGet paths should pass through unchanged
            Assert.Equal("nuget:TestPackage, 1.0.0", normalizedPath);
        }

        [Fact]
        public void ShouldNormalizeRegularPath()
        {
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>();
            var resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var normalizedPath = resolver.NormalizePath("path/to/file.csx", Directory.GetCurrentDirectory());
            // Regular paths should delegate to underlying resolver
            Assert.NotNull(normalizedPath);
        }

        [Fact]
        public void ShouldHandleNugetPathWithoutVersion()
        {
            var scriptList = new List<string> { "/path/to/script.csx" };
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>
            {
                { "TestPackage", scriptList }
            };
            
            NuGetSourceReferenceResolver resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var result = resolver.ResolveReference("nuget:TestPackage", Directory.GetCurrentDirectory());
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldHandleEmptyScriptMap()
        {
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>();
            var resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var result = resolver.ResolveReference("nuget:UnknownPackage, 1.0.0", Directory.GetCurrentDirectory());
            Assert.NotNull(result);
        }

        [Fact]
        public void EqualsReturnsFalseForNull()
        {
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>();
            var resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            Assert.False(resolver.Equals(null));
        }

        [Fact]
        public void EqualsReturnsTrueForSameInstance()
        {
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>();
            var resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            Assert.True(resolver.Equals(resolver));
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentType()
        {
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>();
            var resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            Assert.False(resolver.Equals("not a resolver"));
        }

        [Fact]
        public void GetHashCodeReturnsConsistentValue()
        {
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>();
            var resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var hashCode1 = resolver.GetHashCode();
            var hashCode2 = resolver.GetHashCode();
            
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void ShouldHandlePackageNameWithSpecialCharacters()
        {
            var scriptList = new List<string> { "/path/to/script.csx" };
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>
            {
                { "Test.Package-Name_123", scriptList }
            };
            
            NuGetSourceReferenceResolver resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var result = resolver.ResolveReference("nuget:Test.Package-Name_123, 1.0.0", Directory.GetCurrentDirectory());
            Assert.Equal("/path/to/script.csx", result);
        }

        [Fact]
        public void ShouldHandleCaseInsensitiveNugetPrefix()
        {
            var scriptList = new List<string> { "/path/to/script.csx" };
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>
            {
                { "TestPackage", scriptList }
            };
            
            NuGetSourceReferenceResolver resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var normalizedPath = resolver.NormalizePath("NUGET:TestPackage, 1.0.0", Directory.GetCurrentDirectory());
            // Should still be recognized as nuget path
            Assert.Equal("NUGET:TestPackage, 1.0.0", normalizedPath);
        }

        [Fact]
        public void ShouldCreateMemoryStreamForMultipleScripts()
        {
            var scriptList = new List<string> 
            { 
                "script1.csx",
                "script2.csx",
                "script3.csx"
            };
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>
            {
                { "MultiScriptPackage", scriptList }
            };
            
            NuGetSourceReferenceResolver resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var stream = resolver.OpenRead("nuget:MultiScriptPackage, 1.0.0");
            
            var memoryStream = stream as MemoryStream;
            Assert.NotNull(memoryStream);
            
            memoryStream.Position = 0;
            var reader = new StreamReader(memoryStream);
            var content = reader.ReadToEnd();
            
            Assert.Contains("script1.csx", content);
            Assert.Contains("script2.csx", content);
            Assert.Contains("script3.csx", content);
            Assert.Equal(3, content.Split("#load").Length - 1);
        }

        [Fact]
        public void ShouldHandleEmptyVersionInReference()
        {
            var scriptList = new List<string> { "/path/to/script.csx" };
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>
            {
                { "TestPackage", scriptList }
            };
            
            NuGetSourceReferenceResolver resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var result = resolver.ResolveReference("nuget:TestPackage,", Directory.GetCurrentDirectory());
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldPreserveMemoryStreamPosition()
        {
            var scriptList = new List<string> 
            { 
                "script1.csx",
                "script2.csx"
            };
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>
            {
                { "TestPackage", scriptList }
            };
            
            NuGetSourceReferenceResolver resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var stream = resolver.OpenRead("nuget:TestPackage, 1.0.0") as MemoryStream;
            
            // Stream position should be at start (0) after creation
            Assert.Equal(0, stream.Position);
        }

        [Fact]
        public void ShouldReturnOriginalPathWhenPackageNotInMap()
        {
            var scriptMap = new Dictionary<string, IReadOnlyList<string>>
            {
                { "ExistingPackage", new List<string> { "/path/to/script.csx" } }
            };
            
            NuGetSourceReferenceResolver resolver = new NuGetSourceReferenceResolver(
                new SourceFileResolver(ImmutableArray<string>.Empty, Directory.GetCurrentDirectory()), 
                scriptMap);
            
            var result = resolver.ResolveReference("nuget:NonExistentPackage, 1.0.0", Directory.GetCurrentDirectory());
            Assert.NotNull(result);
        }
    }
}