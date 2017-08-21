using System;
using NUnit.Framework;

namespace SJP.ProcessRedux.Tests
{
    [TestFixture]
    public class ProcessConfigurationTests
    {
        [Test]
        public void Ctor_GivenNullFileName_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ProcessConfiguration(null));
        }

        [Test]
        public void Ctor_GivenEmptyFileName_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ProcessConfiguration(string.Empty));
        }

        [Test]
        public void Ctor_GivenWhiteSpaceFileName_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ProcessConfiguration("  "));
        }

        [Test]
        public void FileName_GivenNullFileName_ThrowsArgNullException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            Assert.Throws<ArgumentNullException>(() => config.FileName = null);
        }

        [Test]
        public void FileName_GivenEmptyFileName_ThrowsArgNullException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            Assert.Throws<ArgumentNullException>(() => config.FileName = string.Empty);
        }

        [Test]
        public void FileName_GivenWhiteSpaceFileName_ThrowsArgNullException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            Assert.Throws<ArgumentNullException>(() => config.FileName = "     ");
        }

        [Test]
        public void FileName_WhenPassedInCtor_IsSetToCtorArg()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            Assert.AreEqual(fileName, config.FileName);
        }
    }
}
