using NUnit.Framework;
using Scalar.Common;
using Scalar.Common.Git;
using Scalar.Tests.Should;

namespace Scalar.UnitTests.Common
{
    [TestFixture]
    public class GitVersionTests
    {
        [TestCase]
        public void GetFeatureFlags_VfsGitVersion_ReturnsGvfsProtocolSupported()
        {
            var version = new GitVersion(2, 28, 0, "vfs", 1, 0);
            GitFeatureFlags features = version.GetFeatures();
            features.HasFlag(GitFeatureFlags.GvfsProtocol).ShouldBeTrue();
        }

        [TestCase]
        public void GetFeatureFlags_NormalGitVersion_ReturnsGvfsProtocolNotSupported()
        {
            var gitGitVersion = new GitVersion(2, 28, 0);
            GitFeatureFlags gitGitFeatures = gitGitVersion.GetFeatures();
            gitGitFeatures.HasFlag(GitFeatureFlags.GvfsProtocol).ShouldBeFalse();

            var winGitVersion = new GitVersion(2, 28, 0, "windows", 1, 1);
            GitFeatureFlags winGitFeatures = winGitVersion.GetFeatures();
            winGitFeatures.HasFlag(GitFeatureFlags.GvfsProtocol).ShouldBeFalse();
        }

        [TestCase]
        public void TryParseInstallerName()
        {
            this.ParseAndValidateInstallerVersion("Git-1.2.3.scalar.4.5.gb16030b-64-bit" + ScalarPlatform.Instance.Constants.InstallerExtension);
            this.ParseAndValidateInstallerVersion("git-1.2.3.scalar.4.5.gb16030b-64-bit" + ScalarPlatform.Instance.Constants.InstallerExtension);
            this.ParseAndValidateInstallerVersion("Git-1.2.3.scalar.4.5.gb16030b-64-bit" + ScalarPlatform.Instance.Constants.InstallerExtension);
        }

        [TestCase]
        public void Version_Data_Null_Returns_False()
        {
            GitVersion version;
            bool success = GitVersion.TryParseVersion(null, out version);
            success.ShouldEqual(false);
        }

        [TestCase]
        public void Version_Data_Empty_Returns_False()
        {
            GitVersion version;
            bool success = GitVersion.TryParseVersion(string.Empty, out version);
            success.ShouldEqual(false);
        }

        [TestCase]
        public void Version_Data_Not_Enough_Numbers_Returns_False()
        {
            GitVersion version;
            bool success = GitVersion.TryParseVersion("2.0", out version);
            success.ShouldEqual(false);
        }

        [TestCase]
        public void Version_Data_Too_Many_Numbers_Returns_True()
        {
            GitVersion version;
            bool success = GitVersion.TryParseVersion("2.0.1.test.1.4.3.6", out version);
            success.ShouldEqual(true);
        }

        [TestCase]
        public void Version_Data_Valid_Returns_True()
        {
            GitVersion version;
            bool success = GitVersion.TryParseVersion("2.0.1.test.1.2", out version);
            success.ShouldEqual(true);
        }

        [TestCase]
        public void Compare_Different_Platforms_Returns_False()
        {
            GitVersion version1 = new GitVersion(1, 2, 3, "test", 4, 1);
            GitVersion version2 = new GitVersion(1, 2, 3, "test1", 4, 1);

            version1.IsLessThan(version2).ShouldEqual(false);
            version1.IsEqualTo(version2).ShouldEqual(false);
        }

        [TestCase]
        public void Compare_Version_Equal()
        {
            GitVersion version1 = new GitVersion(1, 2, 3, "test", 4, 1);
            GitVersion version2 = new GitVersion(1, 2, 3, "test", 4, 1);

            version1.IsLessThan(version2).ShouldEqual(false);
            version1.IsEqualTo(version2).ShouldEqual(true);
        }

        [TestCase]
        public void Compare_Version_Major_Less()
        {
            GitVersion version1 = new GitVersion(0, 2, 3, "test", 4, 1);
            GitVersion version2 = new GitVersion(1, 2, 3, "test", 4, 1);

            version1.IsLessThan(version2).ShouldEqual(true);
            version1.IsEqualTo(version2).ShouldEqual(false);
        }

        [TestCase]
        public void Compare_Version_Major_Greater()
        {
            GitVersion version1 = new GitVersion(2, 2, 3, "test", 4, 1);
            GitVersion version2 = new GitVersion(1, 2, 3, "test", 4, 1);

            version1.IsLessThan(version2).ShouldEqual(false);
            version1.IsEqualTo(version2).ShouldEqual(false);
        }

        [TestCase]
        public void Compare_Version_Minor_Less()
        {
            GitVersion version1 = new GitVersion(1, 1, 3, "test", 4, 1);
            GitVersion version2 = new GitVersion(1, 2, 3, "test", 4, 1);

            version1.IsLessThan(version2).ShouldEqual(true);
            version1.IsEqualTo(version2).ShouldEqual(false);
        }

        [TestCase]
        public void Compare_Version_Minor_Greater()
        {
            GitVersion version1 = new GitVersion(1, 3, 3, "test", 4, 1);
            GitVersion version2 = new GitVersion(1, 2, 3, "test", 4, 1);

            version1.IsLessThan(version2).ShouldEqual(false);
            version1.IsEqualTo(version2).ShouldEqual(false);
        }

        [TestCase]
        public void Compare_Version_Build_Less()
        {
            GitVersion version1 = new GitVersion(1, 2, 2, "test", 4, 1);
            GitVersion version2 = new GitVersion(1, 2, 3, "test", 4, 1);

            version1.IsLessThan(version2).ShouldEqual(true);
            version1.IsEqualTo(version2).ShouldEqual(false);
        }

        [TestCase]
        public void Compare_Version_Build_Greater()
        {
            GitVersion version1 = new GitVersion(1, 2, 4, "test", 4, 1);
            GitVersion version2 = new GitVersion(1, 2, 3, "test", 4, 1);

            version1.IsLessThan(version2).ShouldEqual(false);
            version1.IsEqualTo(version2).ShouldEqual(false);
        }

        [TestCase]
        public void Compare_Version_Revision_Less()
        {
            GitVersion version1 = new GitVersion(1, 2, 3, "test", 3, 1);
            GitVersion version2 = new GitVersion(1, 2, 3, "test", 4, 1);

            version1.IsLessThan(version2).ShouldEqual(true);
            version1.IsEqualTo(version2).ShouldEqual(false);
        }

        [TestCase]
        public void Compare_Version_Revision_Greater()
        {
            GitVersion version1 = new GitVersion(1, 2, 3, "test", 5, 1);
            GitVersion version2 = new GitVersion(1, 2, 3, "test", 4, 1);

            version1.IsLessThan(version2).ShouldEqual(false);
            version1.IsEqualTo(version2).ShouldEqual(false);
        }

        [TestCase]
        public void Compare_Version_MinorRevision_Less()
        {
            GitVersion version1 = new GitVersion(1, 2, 3, "test", 4, 1);
            GitVersion version2 = new GitVersion(1, 2, 3, "test", 4, 2);

            version1.IsLessThan(version2).ShouldEqual(true);
            version1.IsEqualTo(version2).ShouldEqual(false);
        }

        [TestCase]
        public void Compare_Version_MinorRevision_Greater()
        {
            GitVersion version1 = new GitVersion(1, 2, 3, "test", 4, 2);
            GitVersion version2 = new GitVersion(1, 2, 3, "test", 4, 1);

            version1.IsLessThan(version2).ShouldEqual(false);
            version1.IsEqualTo(version2).ShouldEqual(false);
        }

        [TestCase]
        public void Allow_Blank_Minor_Revision()
        {
            GitVersion version;
            GitVersion.TryParseVersion("1.2.3.test.4", out version).ShouldEqual(true);

            version.Major.ShouldEqual(1);
            version.Minor.ShouldEqual(2);
            version.Build.ShouldEqual(3);
            version.Platform.ShouldEqual("test");
            version.Revision.ShouldEqual(4);
            version.MinorRevision.ShouldEqual(0);
        }

        [TestCase]
        public void Allow_Invalid_Minor_Revision()
        {
            GitVersion version;
            GitVersion.TryParseVersion("1.2.3.test.4.notint", out version).ShouldEqual(true);

            version.Major.ShouldEqual(1);
            version.Minor.ShouldEqual(2);
            version.Build.ShouldEqual(3);
            version.Platform.ShouldEqual("test");
            version.Revision.ShouldEqual(4);
            version.MinorRevision.ShouldEqual(0);
        }

        private void ParseAndValidateInstallerVersion(string installerName)
        {
            GitVersion version;
            bool success = GitVersion.TryParseInstallerName(installerName, ScalarPlatform.Instance.Constants.InstallerExtension, out version);
            success.ShouldBeTrue();

            version.Major.ShouldEqual(1);
            version.Minor.ShouldEqual(2);
            version.Build.ShouldEqual(3);
            version.Platform.ShouldEqual("scalar");
            version.Revision.ShouldEqual(4);
            version.MinorRevision.ShouldEqual(5);
        }
    }
}
