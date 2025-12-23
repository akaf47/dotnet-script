using System;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Xunit;
using Moq;

namespace Dotnet.Script.DependencyModel.NuGet.Tests
{
    public class NuGetMetadataReferenceResolverTests
    {
        private Mock<MetadataReferenceResolver> _mockResolver;
        private NuGetMetadataReferenceResolver _resolver;

        public NuGetMetadataReferenceResolverTests()
        {
            _mockResolver = new Mock<MetadataReferenceResolver>();
            _resolver = new NuGetMetadataReferenceResolver(_mockResolver.Object);
        }

        #region Constructor Tests
        [Fact]
        public void Constructor_WithValidResolver_StoresResolver()
        {
            // Arrange
            var innerResolver = new Mock<MetadataReferenceResolver>().Object;

            // Act
            var resolver = new NuGetMetadataReferenceResolver(innerResolver);

            // Assert
            Assert.NotNull(resolver);
        }

        [Fact]
        public void Constructor_WithNullResolver_StoresNull()
        {
            // Act & Assert - should not throw
            var resolver = new NuGetMetadataReferenceResolver(null);
            Assert.NotNull(resolver);
        }
        #endregion

        #region Equals Tests
        [Fact]
        public void Equals_WithSameObject_ReturnsTrue()
        {
            // Act
            var result = _resolver.Equals(_resolver);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentObject_DelegatesTo_InnerResolver()
        {
            // Arrange
            var otherObj = new object();
            _mockResolver.Setup(r => r.Equals(otherObj)).Returns(true);

            // Act
            var result = _resolver.Equals(otherObj);

            // Assert
            Assert.True(result);
            _mockResolver.Verify(r => r.Equals(otherObj), Times.Once);
        }

        [Fact]
        public void Equals_WithNull_DelegatesTo_InnerResolver()
        {
            // Arrange
            _mockResolver.Setup(r => r.Equals(null)).Returns(false);

            // Act
            var result = _resolver.Equals(null);

            // Assert
            Assert.False(result);
            _mockResolver.Verify(r => r.Equals(null), Times.Once);
        }

        [Fact]
        public void Equals_WithDifferentType_DelegatesTo_InnerResolver()
        {
            // Arrange
            var other = 42;
            _mockResolver.Setup(r => r.Equals(other)).Returns(false);

            // Act
            var result = _resolver.Equals(other);

            // Assert
            Assert.False(result);
        }
        #endregion

        #region GetHashCode Tests
        [Fact]
        public void GetHashCode_ReturnsInnerResolverHashCode()
        {
            // Arrange
            var expectedHashCode = 12345;
            _mockResolver.Setup(r => r.GetHashCode()).Returns(expectedHashCode);

            // Act
            var result = _resolver.GetHashCode();

            // Assert
            Assert.Equal(expectedHashCode, result);
            _mockResolver.Verify(r => r.GetHashCode(), Times.Once);
        }

        [Fact]
        public void GetHashCode_WithDifferentResolvers_ReturnsDifferentHashCodes()
        {
            // Arrange
            var resolver1 = new Mock<MetadataReferenceResolver>();
            var resolver2 = new Mock<MetadataReferenceResolver>();
            resolver1.Setup(r => r.GetHashCode()).Returns(111);
            resolver2.Setup(r => r.GetHashCode()).Returns(222);

            var nugetResolver1 = new NuGetMetadataReferenceResolver(resolver1.Object);
            var nugetResolver2 = new NuGetMetadataReferenceResolver(resolver2.Object);

            // Act
            var hash1 = nugetResolver1.GetHashCode();
            var hash2 = nugetResolver2.GetHashCode();

            // Assert
            Assert.NotEqual(hash1, hash2);
        }
        #endregion

        #region ResolveMissingAssemblies Property Tests
        [Fact]
        public void ResolveMissingAssemblies_ReturnsTrue_WhenInnerResolverReturnsTrue()
        {
            // Arrange
            _mockResolver.Setup(r => r.ResolveMissingAssemblies).Returns(true);

            // Act
            var result = _resolver.ResolveMissingAssemblies;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ResolveMissingAssemblies_ReturnsFalse_WhenInnerResolverReturnsFalse()
        {
            // Arrange
            _mockResolver.Setup(r => r.ResolveMissingAssemblies).Returns(false);

            // Act
            var result = _resolver.ResolveMissingAssemblies;

            // Assert
            Assert.False(result);
        }
        #endregion

        #region ResolveMissingAssembly Tests
        [Fact]
        public void ResolveMissingAssembly_WithValidInputs_DelegatesTo_InnerResolver()
        {
            // Arrange
            var definition = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var identity = new AssemblyIdentity("TestAssembly", new Version(1, 0, 0, 0));
            var mockReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            
            _mockResolver.Setup(r => r.ResolveMissingAssembly(definition, identity))
                .Returns((PortableExecutableReference)mockReference);

            // Act
            var result = _resolver.ResolveMissingAssembly(definition, identity);

            // Assert
            Assert.NotNull(result);
            _mockResolver.Verify(r => r.ResolveMissingAssembly(definition, identity), Times.Once);
        }

        [Fact]
        public void ResolveMissingAssembly_WithNullDefinition_DelegatesTo_InnerResolver()
        {
            // Arrange
            var identity = new AssemblyIdentity("TestAssembly");
            _mockResolver.Setup(r => r.ResolveMissingAssembly(null, identity))
                .Returns((PortableExecutableReference)null);

            // Act
            var result = _resolver.ResolveMissingAssembly(null, identity);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ResolveMissingAssembly_WithNullIdentity_DelegatesTo_InnerResolver()
        {
            // Arrange
            var definition = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            _mockResolver.Setup(r => r.ResolveMissingAssembly(definition, null))
                .Returns((PortableExecutableReference)null);

            // Act
            var result = _resolver.ResolveMissingAssembly(definition, null);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region ResolveReference Tests - NuGet References
        [Fact]
        public void ResolveReference_WithNugetPrefix_LowercaseNuget_ReturnsNuGetAssemblyReference()
        {
            // Arrange
            var reference = "nuget:SomePackage,1.0.0";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            Assert.NotEmpty(result);
            Assert.Single(result);
            Assert.NotNull(result[0]);
            _mockResolver.Verify(r => r.ResolveReference(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MetadataReferenceProperties>()), Times.Never);
        }

        [Fact]
        public void ResolveReference_WithNugetPrefix_UppercaseNuget_ReturnsNuGetAssemblyReference()
        {
            // Arrange
            var reference = "NUGET:SomePackage,1.0.0";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            Assert.NotEmpty(result);
            Assert.Single(result);
            _mockResolver.Verify(r => r.ResolveReference(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MetadataReferenceProperties>()), Times.Never);
        }

        [Fact]
        public void ResolveReference_WithNugetPrefix_MixedcaseNuget_ReturnsNuGetAssemblyReference()
        {
            // Arrange
            var reference = "NuGeT:SomePackage,1.0.0";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public void ResolveReference_WithSdkPrefix_LowercaseSdk_ReturnsNuGetAssemblyReference()
        {
            // Arrange
            var reference = "sdk:Microsoft.NET.Sdk.Web";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            Assert.NotEmpty(result);
            Assert.Single(result);
            _mockResolver.Verify(r => r.ResolveReference(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MetadataReferenceProperties>()), Times.Never);
        }

        [Fact]
        public void ResolveReference_WithSdkPrefix_UppercaseSdk_ReturnsNuGetAssemblyReference()
        {
            // Arrange
            var reference = "SDK:Microsoft.NET.Sdk.Web";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public void ResolveReference_WithSdkPrefix_MixedcaseSdk_ReturnsNuGetAssemblyReference()
        {
            // Arrange
            var reference = "SdK:Microsoft.NET.Sdk.Web";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public void ResolveReference_WithNugetPrefix_NoVersion_ReturnsNuGetAssemblyReference()
        {
            // Arrange
            var reference = "nuget:SomePackage";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public void ResolveReference_WithEmptyString_DelegatesTo_InnerResolver()
        {
            // Arrange
            var reference = "";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;
            _mockResolver.Setup(r => r.ResolveReference(reference, baseFilePath, properties))
                .Returns(ImmutableArray<PortableExecutableReference>.Empty);

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            Assert.Empty(result);
            _mockResolver.Verify(r => r.ResolveReference(reference, baseFilePath, properties), Times.Once);
        }

        [Fact]
        public void ResolveReference_WithFilePath_DelegatesTo_InnerResolver()
        {
            // Arrange
            var reference = "C:\\packages\\myassembly.dll";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;
            var mockResult = ImmutableArray<PortableExecutableReference>.Empty;
            _mockResolver.Setup(r => r.ResolveReference(reference, baseFilePath, properties))
                .Returns(mockResult);

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            Assert.Empty(result);
            _mockResolver.Verify(r => r.ResolveReference(reference, baseFilePath, properties), Times.Once);
        }

        [Fact]
        public void ResolveReference_WithStandardAssemblyName_DelegatesTo_InnerResolver()
        {
            // Arrange
            var reference = "System.Core";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;
            var mockResult = ImmutableArray<PortableExecutableReference>.Empty;
            _mockResolver.Setup(r => r.ResolveReference(reference, baseFilePath, properties))
                .Returns(mockResult);

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            Assert.Empty(result);
            _mockResolver.Verify(r => r.ResolveReference(reference, baseFilePath, properties), Times.Once);
        }

        [Fact]
        public void ResolveReference_ReturnsMetadataFromNuGetResolverAssembly()
        {
            // Arrange
            var reference = "nuget:Newtonsoft.Json,12.0.0";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            Assert.NotEmpty(result);
            var portableExeRef = result[0];
            Assert.NotNull(portableExeRef);
            // The returned reference should be from this assembly
            var assemblyLocation = typeof(NuGetMetadataReferenceResolver).GetTypeInfo().Assembly.Location;
            Assert.Equal(assemblyLocation, portableExeRef.FilePath);
        }

        [Fact]
        public void ResolveReference_WithNullReference_DelegatesTo_InnerResolver()
        {
            // Arrange
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;
            var mockResult = ImmutableArray<PortableExecutableReference>.Empty;
            _mockResolver.Setup(r => r.ResolveReference(null, baseFilePath, properties))
                .Returns(mockResult);

            // Act
            var result = _resolver.ResolveReference(null, baseFilePath, properties);

            // Assert
            Assert.Empty(result);
            _mockResolver.Verify(r => r.ResolveReference(null, baseFilePath, properties), Times.Once);
        }

        [Fact]
        public void ResolveReference_WithNullBaseFilePath_StillResolvesNugetReference()
        {
            // Arrange
            var reference = "nuget:SomePackage,1.0.0";
            var properties = MetadataReferenceProperties.Assembly;

            // Act
            var result = _resolver.ResolveReference(reference, null, properties);

            // Assert
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public void ResolveReference_WithNugetPrefixStartsWith_IsCaseInsensitive()
        {
            // Arrange - test various case combinations
            var testCases = new[]
            {
                "nuget:pkg,1.0",
                "NUGET:pkg,1.0",
                "NuGet:pkg,1.0",
                "nUgEt:pkg,1.0"
            };

            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;

            // Act & Assert
            foreach (var reference in testCases)
            {
                var result = _resolver.ResolveReference(reference, baseFilePath, properties);
                Assert.NotEmpty(result);
                _mockResolver.Verify(r => r.ResolveReference(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MetadataReferenceProperties>()), Times.Never);
            }
        }

        [Fact]
        public void ResolveReference_WithSdkPrefixStartsWith_IsCaseInsensitive()
        {
            // Arrange
            var testCases = new[]
            {
                "sdk:web",
                "SDK:web",
                "Sdk:web",
                "sDk:web"
            };

            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;

            // Act & Assert
            foreach (var reference in testCases)
            {
                var result = _resolver.ResolveReference(reference, baseFilePath, properties);
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void ResolveReference_WithReferenceThatContainsNugetButNotAtStart_DelegatesTo_InnerResolver()
        {
            // Arrange
            var reference = "MyAssembly_nuget";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;
            var mockResult = ImmutableArray<PortableExecutableReference>.Empty;
            _mockResolver.Setup(r => r.ResolveReference(reference, baseFilePath, properties))
                .Returns(mockResult);

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            _mockResolver.Verify(r => r.ResolveReference(reference, baseFilePath, properties), Times.Once);
        }

        [Fact]
        public void ResolveReference_WithReferenceThatContainsSdkButNotAtStart_DelegatesTo_InnerResolver()
        {
            // Arrange
            var reference = "MyAssembly_sdk";
            var baseFilePath = "C:\\scripts\\test.csx";
            var properties = MetadataReferenceProperties.Assembly;
            var mockResult = ImmutableArray<PortableExecutableReference>.Empty;
            _mockResolver.Setup(r => r.ResolveReference(reference, baseFilePath, properties))
                .Returns(mockResult);

            // Act
            var result = _resolver.ResolveReference(reference, baseFilePath, properties);

            // Assert
            _mockResolver.Verify(r => r.ResolveReference(reference, baseFilePath, properties), Times.Once);
        }
        #endregion
    }
}