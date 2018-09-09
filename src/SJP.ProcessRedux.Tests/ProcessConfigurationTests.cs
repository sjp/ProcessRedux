using System;
using NUnit.Framework;

namespace SJP.ProcessRedux.Tests
{
    [TestFixture]
    internal static class ProcessConfigurationTests
    {
        [Test]
        public static void Ctor_GivenNullFileName_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ProcessConfiguration(null));
        }

        [Test]
        public static void Ctor_GivenEmptyFileName_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ProcessConfiguration(string.Empty));
        }

        [Test]
        public static void Ctor_GivenWhiteSpaceFileName_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ProcessConfiguration("  "));
        }

        [Test]
        public static void FileName_GivenNullFileName_ThrowsArgNullException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            Assert.Throws<ArgumentNullException>(() => config.FileName = null);
        }

        [Test]
        public static void FileName_GivenEmptyFileName_ThrowsArgNullException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            Assert.Throws<ArgumentNullException>(() => config.FileName = string.Empty);
        }

        [Test]
        public static void FileName_GivenWhiteSpaceFileName_ThrowsArgNullException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            Assert.Throws<ArgumentNullException>(() => config.FileName = "     ");
        }

        [Test]
        public static void FileName_WhenPassedInCtor_IsSetToCtorArg()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            Assert.AreEqual(fileName, config.FileName);
        }
    }
}
