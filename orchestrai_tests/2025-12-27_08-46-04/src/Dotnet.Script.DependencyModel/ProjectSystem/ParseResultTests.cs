using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Dotnet.Script.DependencyModel.ProjectSystem;

namespace Dotnet.Script.DependencyModel.Tests.ProjectSystem
{
    public class ParseResultTests
    {
        [Fact]
        public void Constructor_WithValidParameters_InitializesAllProperties()
        {
            // Arrange
            var includeFiles = new[] { "file1.cs", "file2.cs" };
            var references = new[] { "ref1.dll", "ref2.dll" };
            var packageReferences = new[] { "pkg1", "pkg2" };
            var frameworkReferences = new[] { "framework1", "framework2" };
            var preprocessorSymbols = new[] { "DEBUG", "RELEASE" };

            // Act
            var result = new ParseResult(
                includeFiles,
                references,
                packageReferences,
                frameworkReferences,
                preprocessorSymbols
            );

            // Assert
            Assert.Equal(includeFiles, result.IncludeFiles);
            Assert.Equal(references, result.References);
            Assert.Equal(packageReferences, result.PackageReferences);
            Assert.Equal(frameworkReferences, result.FrameworkReferences);
            Assert.Equal(preprocessorSymbols, result.PreprocessorSymbols);
        }

        [Fact]
        public void Constructor_WithEmptyCollections_InitializesWithEmptyCollections()
        {
            // Arrange
            var emptyArray = Array.Empty<string>();

            // Act
            var result = new ParseResult(
                emptyArray,
                emptyArray,
                emptyArray,
                emptyArray,
                emptyArray
            );

            // Assert
            Assert.Empty(result.IncludeFiles);
            Assert.Empty(result.References);
            Assert.Empty(result.PackageReferences);
            Assert.Empty(result.FrameworkReferences);
            Assert.Empty(result.PreprocessorSymbols);
        }

        [Fact]
        public void Constructor_WithNullCollections_InitializesWithNull()
        {
            // Act
            var result = new ParseResult(null, null, null, null, null);

            // Assert
            Assert.Null(result.IncludeFiles);
            Assert.Null(result.References);
            Assert.Null(result.PackageReferences);
            Assert.Null(result.FrameworkReferences);
            Assert.Null(result.PreprocessorSymbols);
        }

        [Fact]
        public void IncludeFiles_Property_ReturnsCorrectValue()
        {
            // Arrange
            var includeFiles = new[] { "file1.cs", "file2.cs", "file3.cs" };
            var result = new ParseResult(
                includeFiles,
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()
            );

            // Act & Assert
            Assert.Equal(includeFiles, result.IncludeFiles);
            Assert.Equal(3, result.IncludeFiles.Count());
        }

        [Fact]
        public void References_Property_ReturnsCorrectValue()
        {
            // Arrange
            var references = new[] { "System.dll", "System.Core.dll" };
            var result = new ParseResult(
                Array.Empty<string>(),
                references,
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()
            );

            // Act & Assert
            Assert.Equal(references, result.References);
            Assert.Equal(2, result.References.Count());
        }

        [Fact]
        public void PackageReferences_Property_ReturnsCorrectValue()
        {
            // Arrange
            var packageReferences = new[] { "Newtonsoft.Json", "AutoMapper" };
            var result = new ParseResult(
                Array.Empty<string>(),
                Array.Empty<string>(),
                packageReferences,
                Array.Empty<string>(),
                Array.Empty<string>()
            );

            // Act & Assert
            Assert.Equal(packageReferences, result.PackageReferences);
        }

        [Fact]
        public void FrameworkReferences_Property_ReturnsCorrectValue()
        {
            // Arrange
            var frameworkReferences = new[] { "System", "System.Net" };
            var result = new ParseResult(
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                frameworkReferences,
                Array.Empty<string>()
            );

            // Act & Assert
            Assert.Equal(frameworkReferences, result.FrameworkReferences);
        }

        [Fact]
        public void PreprocessorSymbols_Property_ReturnsCorrectValue()
        {
            // Arrange
            var preprocessorSymbols = new[] { "DEBUG", "TRACE", "NET_5_0" };
            var result = new ParseResult(
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                preprocessorSymbols
            );

            // Act & Assert
            Assert.Equal(preprocessorSymbols, result.PreprocessorSymbols);
        }

        [Fact]
        public void Constructor_WithSingleElementCollections_StoresCorrectly()
        {
            // Arrange
            var singleFile = new[] { "single.cs" };
            var singleRef = new[] { "single.dll" };
            var singlePkg = new[] { "SinglePackage" };
            var singleFwk = new[] { "SingleFramework" };
            var singleSymbol = new[] { "SYMBOL" };

            // Act
            var result = new ParseResult(singleFile, singleRef, singlePkg, singleFwk, singleSymbol);

            // Assert
            Assert.Single(result.IncludeFiles);
            Assert.Single(result.References);
            Assert.Single(result.PackageReferences);
            Assert.Single(result.FrameworkReferences);
            Assert.Single(result.PreprocessorSymbols);
        }

        [Fact]
        public void Constructor_WithLargeCollections_HandlesCorrectly()
        {
            // Arrange
            var largeCollection = Enumerable.Range(1, 1000).Select(i => $"item_{i}").ToArray();

            // Act
            var result = new ParseResult(
                largeCollection,
                largeCollection,
                largeCollection,
                largeCollection,
                largeCollection
            );

            // Assert
            Assert.Equal(1000, result.IncludeFiles.Count());
            Assert.Equal(1000, result.References.Count());
            Assert.Equal(1000, result.PackageReferences.Count());
            Assert.Equal(1000, result.FrameworkReferences.Count());
            Assert.Equal(1000, result.PreprocessorSymbols.Count());
        }

        [Fact]
        public void Constructor_WithMixedNullAndEmptyCollections_HandlesCorrectly()
        {
            // Arrange
            var emptyArray = Array.Empty<string>();

            // Act
            var result = new ParseResult(
                emptyArray,
                null,
                emptyArray,
                null,
                emptyArray
            );

            // Assert
            Assert.NotNull(result.IncludeFiles);
            Assert.Null(result.References);
            Assert.NotNull(result.PackageReferences);
            Assert.Null(result.FrameworkReferences);
            Assert.NotNull(result.PreprocessorSymbols);
        }

        [Fact]
        public void Constructor_WithSpecialCharactersInNames_StoresCorrectly()
        {
            // Arrange
            var filesWithSpecialChars = new[] { "file-name.cs", "file_name.cs", "file.name.cs" };
            var refsWithSpecialChars = new[] { "Ref-1.dll", "Ref_2.dll" };
            var pkgsWithSpecialChars = new[] { "Package.Name", "Package-Name" };
            var fwkWithSpecialChars = new[] { "System.Net.Http", "System.IO.FileSystem" };
            var symbolsWithSpecialChars = new[] { "DEBUG", "NET_5_0_OR_GREATER" };

            // Act
            var result = new ParseResult(
                filesWithSpecialChars,
                refsWithSpecialChars,
                pkgsWithSpecialChars,
                fwkWithSpecialChars,
                symbolsWithSpecialChars
            );

            // Assert
            Assert.Equal(filesWithSpecialChars, result.IncludeFiles);
            Assert.Equal(refsWithSpecialChars, result.References);
            Assert.Equal(pkgsWithSpecialChars, result.PackageReferences);
            Assert.Equal(fwkWithSpecialChars, result.FrameworkReferences);
            Assert.Equal(symbolsWithSpecialChars, result.PreprocessorSymbols);
        }

        [Fact]
        public void Constructor_PreservesEnumerableOrder()
        {
            // Arrange
            var orderedFiles = new[] { "z.cs", "a.cs", "m.cs" };
            var orderedRefs = new[] { "z.dll", "a.dll", "m.dll" };

            // Act
            var result = new ParseResult(
                orderedFiles,
                orderedRefs,
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()
            );

            // Assert
            Assert.Equal(orderedFiles, result.IncludeFiles);
            Assert.Equal(orderedRefs, result.References);
            Assert.Equal("z.cs", result.IncludeFiles.First());
            Assert.Equal("m.cs", result.IncludeFiles.Last());
        }

        [Fact]
        public void Constructor_WithDuplicateEntries_StoresAllDuplicates()
        {
            // Arrange
            var filesWithDuplicates = new[] { "file.cs", "file.cs", "file.cs" };

            // Act
            var result = new ParseResult(
                filesWithDuplicates,
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()
            );

            // Assert
            Assert.Equal(3, result.IncludeFiles.Count());
        }

        [Fact]
        public void Properties_AreReadable_AndReturnSameInstance()
        {
            // Arrange
            var files = new[] { "file.cs" };
            var refs = new[] { "ref.dll" };
            var pkgs = new[] { "pkg" };
            var fwks = new[] { "fwk" };
            var symbols = new[] { "SYM" };

            var result = new ParseResult(files, refs, pkgs, fwks, symbols);

            // Act
            var files1 = result.IncludeFiles;
            var files2 = result.IncludeFiles;

            // Assert
            Assert.Same(files1, files2);
        }

        [Fact]
        public void Constructor_WithEmptyStrings_StoresEmptyStrings()
        {
            // Arrange
            var emptyStrings = new[] { "", "" };

            // Act
            var result = new ParseResult(
                emptyStrings,
                emptyStrings,
                emptyStrings,
                emptyStrings,
                emptyStrings
            );

            // Assert
            Assert.Equal(2, result.IncludeFiles.Count());
            Assert.All(result.IncludeFiles, item => Assert.Empty(item));
        }

        [Fact]
        public void Constructor_WithWhitespaceStrings_StoresWhitespaceStrings()
        {
            // Arrange
            var whitespaceStrings = new[] { "   ", "\t", "\n" };

            // Act
            var result = new ParseResult(
                whitespaceStrings,
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()
            );

            // Assert
            Assert.Equal(3, result.IncludeFiles.Count());
        }
    }
}