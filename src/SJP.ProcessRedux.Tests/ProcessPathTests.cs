using System;
using System.Linq;
using NUnit.Framework;

namespace SJP.ProcessRedux.Tests
{
    internal static class ProcessPathTests
    {
        [TestFixture, TestPlatform.Windows]
        internal static class WindowsTests
        {
            [Test]
            public static void GetFullPaths_GivenNotepad_ReturnsPathsInWindowsDir()
            {
                const string expected = @"C:\Windows\notepad.exe";

                var allPaths = ProcessPath.GetFullPaths("notepad.exe").ToList();
                Assert.AreEqual(2, allPaths.Count); // windows and system32

                var pathEqual = string.Equals(expected, allPaths[0], StringComparison.OrdinalIgnoreCase);
                Assert.IsTrue(pathEqual);
            }

            [Test]
            public static void GetFullPaths_GivenDotNet_ReturnsOnlyPathInProgramFilesDir()
            {
                const string expected = @"C:\Program Files\dotnet\dotnet.exe";

                var allPaths = ProcessPath.GetFullPaths("dotnet.exe").ToList();
                Assert.AreEqual(1, allPaths.Count);

                var pathEqual = string.Equals(expected, allPaths.Single(), StringComparison.OrdinalIgnoreCase);
                Assert.IsTrue(pathEqual);
            }

            [Test]
            public static void GetFullPaths_GivenDotNetWithoutExtension_ReturnsOnlyPathInProgramFilesDir()
            {
                const string expected = @"C:\Program Files\dotnet\dotnet.exe";

                var allPaths = ProcessPath.GetFullPaths("dotnet").ToList();
                Assert.AreEqual(1, allPaths.Count);

                var pathEqual = string.Equals(expected, allPaths.Single(), StringComparison.OrdinalIgnoreCase);
                Assert.IsTrue(pathEqual);
            }

            [Test]
            public static void GetFullPaths_GivenWordpad_ReturnsOnlyPathInProgramFilesDirViaAppPaths()
            {
                const string expected = @"C:\Program Files\Windows NT\Accessories\WORDPAD.EXE";

                var allPaths = ProcessPath.GetFullPaths("WORDPAD.exe").ToList();
                Assert.AreEqual(1, allPaths.Count);

                var pathEqual = string.Equals(expected, allPaths.Single(), StringComparison.OrdinalIgnoreCase);
                Assert.IsTrue(pathEqual);
            }

            [Test]
            public static void GetFullPath_GivenNotepad_ReturnsPathsInWindowsDir()
            {
                const string expected = @"C:\Windows\notepad.exe";
                var path = ProcessPath.GetFullPath("notepad.exe");

                var pathEqual = string.Equals(expected, path, StringComparison.OrdinalIgnoreCase);
                Assert.IsTrue(pathEqual);
            }

            [Test]
            public static void GetFullPath_GivenDotNet_ReturnsOnlyPathInProgramFilesDir()
            {
                const string expected = @"C:\Program Files\dotnet\dotnet.exe";
                var path = ProcessPath.GetFullPath("dotnet.exe");

                var pathEqual = string.Equals(expected, path, StringComparison.OrdinalIgnoreCase);
                Assert.IsTrue(pathEqual);
            }

            [Test]
            public static void GetFullPath_GivenDotNetWithoutExtension_ReturnsOnlyPathInProgramFilesDir()
            {
                const string expected = @"C:\Program Files\dotnet\dotnet.exe";
                var path = ProcessPath.GetFullPath("dotnet");

                var pathEqual = string.Equals(expected, path, StringComparison.OrdinalIgnoreCase);
                Assert.IsTrue(pathEqual);
            }

            [Test]
            public static void GetFullPath_GivenWordpad_ReturnsOnlyPathInProgramFilesDirViaAppPaths()
            {
                const string expected = @"C:\Program Files\Windows NT\Accessories\WORDPAD.EXE";
                var path = ProcessPath.GetFullPath("WORDPAD.exe");

                var pathEqual = string.Equals(expected, path, StringComparison.OrdinalIgnoreCase);
                Assert.IsTrue(pathEqual);
            }
        }

        [TestFixture, TestPlatform.Osx]
        internal static class OsxTests
        {
            [Test]
            public static void GetFullPaths_GivenSh_ReturnsPathInBinDir()
            {
                const string expected = "/bin/sh";

                var allPaths = ProcessPath.GetFullPaths("sh").ToList();
                Assert.AreEqual(1, allPaths.Count);

                Assert.AreEqual(expected, allPaths.Single());
            }

            [Test]
            public static void GetFullPath_GivenSh_ReturnsPathInBinDir()
            {
                const string expected = "/bin/sh";

                var result = ProcessPath.GetFullPath("sh").ToList();

                Assert.AreEqual(expected, result);
            }
        }

        [TestFixture, TestPlatform.Linux]
        internal static class LinuxTests
        {
            [Test]
            public static void GetFullPaths_GivenSh_ReturnsPathInBinDir()
            {
                const string expected = "/bin/sh";

                var allPaths = ProcessPath.GetFullPaths("sh").ToList();
                Assert.AreEqual(1, allPaths.Count);

                Assert.AreEqual(expected, allPaths.Single());
            }

            [Test]
            public static void GetFullPath_GivenSh_ReturnsPathInBinDir()
            {
                const string expected = "/bin/sh";

                var result = ProcessPath.GetFullPath("sh").ToList();

                Assert.AreEqual(expected, result);
            }
        }
    }
}
