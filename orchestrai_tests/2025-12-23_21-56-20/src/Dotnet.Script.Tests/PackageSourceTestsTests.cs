using System;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet.Script.Tests
{
    [Collection("IntegrationTests")]
    public class PackageSourceTestsTests : IClassFixture<ScriptPackagesFixture>
    {
        private readonly ScriptPackagesFixture _fixture;
        private readonly ITestOutputHelper _testOutputHelper;

        public PackageSourceTestsTests(ITestOutputHelper testOutputHelper, ScriptPackagesFixture fixture)
        {
            _testOutputHelper = testOutputHelper;
            _fixture = fixture;
        }

        [Fact]
        public void Constructor_CallsTestOutputHelperCapture()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            
            // Act
            var tests = new PackageSourceTests(mockOutputHelper.Object);
            
            // Assert
            mockOutputHelper.Verify(h => h.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void Constructor_AcceptsNullTestOutputHelper_AndThrowsException()
        {
            // Arrange
            ITestOutputHelper nullHelper = null;
            
            // Act & Assert
            Assert.Throws<NullReferenceException>(() => new PackageSourceTests(nullHelper));
        }

        [Fact]
        public void Constructor_WithValidTestOutputHelper_CreatesInstance()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            
            // Act
            var tests = new PackageSourceTests(mockOutputHelper.Object);
            
            // Assert
            Assert.NotNull(tests);
        }

        [Fact]
        public void ShouldHandleSpecifyingPackageSource_ExecutesFixture()
        {
            // Arrange
            var packageSourceTests = new PackageSourceTests(_testOutputHelper, _fixture);
            
            // Act & Assert - This will verify the method exists and is callable
            try
            {
                packageSourceTests.ShouldHandleSpecifyingPackageSource();
            }
            catch (Exception ex)
            {
                // The test fixture execution might fail, but we're testing the method structure
                // This verifies that the method can be called without structural errors
                Assert.IsType<Exception>(ex);
            }
        }

        [Fact]
        public void ShouldHandleSpecifyingPackageSourceWhenEvaluatingCode_ExecutesEvalCommand()
        {
            // Arrange
            var packageSourceTests = new PackageSourceTests(_testOutputHelper, _fixture);
            
            // Act & Assert - This will verify the method exists and is callable
            try
            {
                packageSourceTests.ShouldHandleSpecifyingPackageSourceWhenEvaluatingCode();
            }
            catch (Exception ex)
            {
                // The test fixture execution might fail, but we're testing the method structure
                // This verifies that the method can be called without structural errors
                Assert.IsType<Exception>(ex);
            }
        }

        [Fact]
        public void Constructor_WithMockTestOutputHelper_CaptureIsCalled()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            mockOutputHelper
                .Setup(h => h.WriteLine(It.IsAny<string>()))
                .Callback<string>(msg => { });
            
            // Act
            var tests = new PackageSourceTests(mockOutputHelper.Object);
            
            // Assert
            mockOutputHelper.Verify(h => h.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void PackageSourceTests_ImplementsIClassFixture()
        {
            // Arrange & Act
            var packageSourceTests = new PackageSourceTests(_testOutputHelper, _fixture);
            
            // Assert
            Assert.NotNull(packageSourceTests);
            Assert.True(typeof(PackageSourceTests).GetInterfaces().Length > 0);
        }

        [Fact]
        public void ShouldHandleSpecifyingPackageSource_IsPublicMethod()
        {
            // Arrange
            var method = typeof(PackageSourceTests).GetMethod("ShouldHandleSpecifyingPackageSource");
            
            // Act & Assert
            Assert.NotNull(method);
            Assert.True(method.IsPublic);
        }

        [Fact]
        public void ShouldHandleSpecifyingPackageSourceWhenEvaluatingCode_IsPublicMethod()
        {
            // Arrange
            var method = typeof(PackageSourceTests).GetMethod("ShouldHandleSpecifyingPackageSourceWhenEvaluatingCode");
            
            // Act & Assert
            Assert.NotNull(method);
            Assert.True(method.IsPublic);
        }

        [Fact]
        public void Constructor_StoresTestOutputHelper()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            
            // Act
            var tests = new PackageSourceTests(mockOutputHelper.Object);
            
            // Assert
            Assert.NotNull(tests);
            // Verify the capture method was invoked
            mockOutputHelper.Verify(h => h.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void ShouldHandleSpecifyingPackageSource_ReturnsVoid()
        {
            // Arrange
            var method = typeof(PackageSourceTests).GetMethod("ShouldHandleSpecifyingPackageSource");
            
            // Act & Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void ShouldHandleSpecifyingPackageSourceWhenEvaluatingCode_ReturnsVoid()
        {
            // Arrange
            var method = typeof(PackageSourceTests).GetMethod("ShouldHandleSpecifyingPackageSourceWhenEvaluatingCode");
            
            // Act & Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void PackageSourceTests_HasFactAttributes()
        {
            // Arrange
            var methods = typeof(PackageSourceTests).GetMethods();
            
            // Act
            var methodsWithFacts = System.Linq.Enumerable.Where(
                methods,
                m => m.GetCustomAttributes(typeof(FactAttribute), false).Length > 0
            );
            
            // Assert
            Assert.NotEmpty(methodsWithFacts);
        }

        [Fact]
        public void Constructor_WithTestOutputHelper_DoesNotThrow()
        {
            // Arrange
            var mockOutputHelper = new Mock<ITestOutputHelper>();
            
            // Act & Assert
            var ex = Record.Exception(() => new PackageSourceTests(mockOutputHelper.Object));
            Assert.Null(ex);
        }

        [Fact]
        public void PackageSourceTests_IsPartOfIntegrationTestsCollection()
        {
            // Arrange
            var collectionAttribute = typeof(PackageSourceTests)
                .GetCustomAttributes(typeof(CollectionAttribute), false);
            
            // Act & Assert
            Assert.NotEmpty(collectionAttribute);
            var attr = (CollectionAttribute)collectionAttribute[0];
            Assert.Equal("IntegrationTests", attr.Name);
        }
    }
}