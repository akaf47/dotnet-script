using Xunit;
using Dotnet.Script.DependencyModel.Compilation;
using Dotnet.Script.DependencyModel.ProjectSystem;
using System.Collections.Generic;

namespace Dotnet.Script.DependencyModel.Tests.Compilation
{
    /// <summary>
    /// Tests for ICompilationReferenceReader interface.
    /// Note: This is an interface with only one method signature, so tests verify
    /// that implementations conform to the contract.
    /// </summary>
    public class ICompilationReferenceReaderTests
    {
        [Fact]
        public void ICompilationReferenceReader_InterfaceExists()
        {
            // Verify the interface is public
            Assert.True(typeof(ICompilationReferenceReader).IsPublic);
            Assert.True(typeof(ICompilationReferenceReader).IsInterface);
        }

        [Fact]
        public void ICompilationReferenceReader_HasReadMethod()
        {
            // Verify the Read method exists
            var readMethod = typeof(ICompilationReferenceReader).GetMethod("Read");
            Assert.NotNull(readMethod);
            Assert.True(readMethod.IsPublic);
        }

        [Fact]
        public void ICompilationReferenceReader_ReadMethodAcceptsProjectFileInfo()
        {
            // Verify method signature: IEnumerable<CompilationReference> Read(ProjectFileInfo projectFile)
            var readMethod = typeof(ICompilationReferenceReader).GetMethod("Read");
            var parameters = readMethod.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("projectFile", parameters[0].Name);
            Assert.Equal(typeof(ProjectFileInfo), parameters[0].ParameterType);
        }

        [Fact]
        public void ICompilationReferenceReader_ReadMethodReturnsEnumerableOfCompilationReference()
        {
            // Verify return type is IEnumerable<CompilationReference>
            var readMethod = typeof(ICompilationReferenceReader).GetMethod("Read");
            var returnType = readMethod.ReturnType;
            Assert.True(returnType.IsGenericType);
            Assert.Equal(typeof(IEnumerable<CompilationReference>), returnType);
        }
    }
}