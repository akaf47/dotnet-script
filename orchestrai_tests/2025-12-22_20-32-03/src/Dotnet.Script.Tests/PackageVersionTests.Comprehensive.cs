using Dotnet.Script.DependencyModel.ProjectSystem;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class PackageVersionComprehensiveTests
    {
        [Fact]
        public void ConstructorInitializesWithValidVersion()
        {
            var version = new PackageVersion("1.2.3");
            
            Assert.NotNull(version);
        }

        [Fact]
        public void ConstructorHandlesNullVersion()
        {
            // Should handle null or throw
            try
            {
                var version = new PackageVersion(null);
                Assert.NotNull(version);
            }
            catch (System.ArgumentNullException)
            {
                // Expected
            }
        }

        [Fact]
        public void ConstructorHandlesEmptyVersion()
        {
            var version = new PackageVersion("");
            
            Assert.NotNull(version);
        }

        [Fact]
        public void IsPinnedReturnsTrueForSemanticVersion()
        {
            var version = new PackageVersion("1.2.3");
            
            Assert.True(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsTrueForFourPartVersion()
        {
            var version = new PackageVersion("1.2.3.4");
            
            Assert.True(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsTrueForVersionWithPrerelease()
        {
            var version = new PackageVersion("1.2.3-beta1");
            
            Assert.True(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsTrueForVersionWithBuild()
        {
            var version = new PackageVersion("1.2.3-beta");
            
            Assert.True(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsTrueForComplexPrereleaseVersion()
        {
            var version = new PackageVersion("2.0.0-preview3.20122.2");
            
            Assert.True(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsTrueForCiBuildVersion()
        {
            var version = new PackageVersion("1.0.0-ci-20180920T1656");
            
            Assert.True(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsTrueForBracketedSingleVersion()
        {
            var version = new PackageVersion("[1.2]");
            
            Assert.True(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsTrueForBracketedThreePartVersion()
        {
            var version = new PackageVersion("[1.2.3]");
            
            Assert.True(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsTrueForBracketedPrereleaseVersion()
        {
            var version = new PackageVersion("[1.2.3-beta1]");
            
            Assert.True(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsFalseForTwoPartVersion()
        {
            var version = new PackageVersion("1.0");
            
            Assert.False(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsFalseForOpenLowerBound()
        {
            var version = new PackageVersion("(1.0,)");
            
            Assert.False(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsFalseForOpenUpperBound()
        {
            var version = new PackageVersion("(,1.0]");
            
            Assert.False(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsFalseForClosedRange()
        {
            var version = new PackageVersion("[1.0,2.0]");
            
            Assert.False(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsFalseForOpenRange()
        {
            var version = new PackageVersion("(1.0,2.0)");
            
            Assert.False(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsFalseForMixedRange()
        {
            var version = new PackageVersion("[1.0,2.0)");
            
            Assert.False(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsFalseForSingleParenthesis()
        {
            var version = new PackageVersion("(1.0)");
            
            Assert.False(version.IsPinned);
        }

        [Fact]
        public void IsPinnedReturnsFalseForEmptyVersion()
        {
            var version = new PackageVersion("");
            
            Assert.False(version.IsPinned);
        }

        [Fact]
        public void EqualityWorksForIdenticalVersions()
        {
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.3");
            
            Assert.Equal(version1, version2);
        }

        [Fact]
        public void EqualityIsCaseInsensitive()
        {
            var version1 = new PackageVersion("1.2.3-BETA1");
            var version2 = new PackageVersion("1.2.3-beta1");
            
            Assert.Equal(version1, version2);
        }

        [Fact]
        public void InequalityWorksForDifferentVersions()
        {
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.4");
            
            Assert.NotEqual(version1, version2);
        }

        [Fact]
        public void InequalityWorksForDifferentPrerelease()
        {
            var version1 = new PackageVersion("1.2.3-alpha");
            var version2 = new PackageVersion("1.2.3-beta");
            
            Assert.NotEqual(version1, version2);
        }

        [Fact]
        public void EqualityHandlesNullComparison()
        {
            var version = new PackageVersion("1.2.3");
            
            Assert.NotEqual(version, null);
            Assert.False(version.Equals(null));
        }

        [Fact]
        public void GetHashCodeConsistentForEqualVersions()
        {
            var version1 = new PackageVersion("1.2.3-beta1");
            var version2 = new PackageVersion("1.2.3-BETA1");
            
            Assert.Equal(version1.GetHashCode(), version2.GetHashCode());
        }

        [Fact]
        public void GetHashCodeDifferentForDifferentVersions()
        {
            var version1 = new PackageVersion("1.2.3");
            var version2 = new PackageVersion("1.2.4");
            
            // Hash codes should typically be different for different inputs
            // (though not guaranteed, it's extremely likely)
            Assert.NotEqual(version1.GetHashCode(), version2.GetHashCode());
        }

        [Fact]
        public void ToStringReturnsVersionString()
        {
            var version = new PackageVersion("1.2.3");
            var versionString = version.ToString();
            
            Assert.NotNull(versionString);
            Assert.Contains("1.2.3", versionString);
        }

        [Fact]
        public void IsPinnedHandlesVersionWithWildcard()
        {
            var version = new PackageVersion("1.2.*");
            
            // Wildcard versions are typically not pinned
            Assert.False(version.IsPinned);
        }

        [Fact]
        public void IsPinnedHandlesVersionWithLeadingZeros()
        {
            var version = new PackageVersion("01.02.03");
            
            Assert.True(version.IsPinned);
        }

        [Fact]
        public void IsPinnedHandlesVersionWithLongPrerelease()
        {
            var version = new PackageVersion("1.2.3-alpha.beta.gamma");
            
            Assert.True(version.IsPinned);
        }
    }
}