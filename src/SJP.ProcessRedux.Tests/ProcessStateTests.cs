using System;
using NUnit.Framework;
using Moq;

namespace SJP.ProcessRedux.Tests
{
    [TestFixture]
    public class ProcessStateTests
    {
        [Test]
        public void Ctor_GivenNullFrameworkProcess_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ProcessState(null));
        }

        [Test]
        public void Ctor_GivenUnstartedProcess_ThrowsArgNullException()
        {
            var process = new Mock<IFrameworkProcess>();
            process.SetupGet(p => p.HasStarted).Returns(false);

            Assert.Throws<ArgumentException>(() => new ProcessState(process.Object));
        }

        public void ExitCode_GivenUnfinishedProcess_ReturnsZero()
        {
            var process = new Mock<IFrameworkProcess>();
            process.SetupGet(p => p.HasExited).Returns(false);

            var state = new ProcessState(process.Object);

            Assert.AreEqual(0, state.ExitCode);
        }

        public void ExitCode_GivenFinishedProcess_ReturnsExitCode()
        {
            const int exitCode = 123;
            var process = new Mock<IFrameworkProcess>();
            process.SetupGet(p => p.HasExited).Returns(true);
            process.SetupGet(p => p.ExitCode).Returns(exitCode);

            var state = new ProcessState(process.Object);

            Assert.AreEqual(exitCode, state.ExitCode);
        }
    }
}
