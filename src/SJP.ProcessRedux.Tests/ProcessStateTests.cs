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
    }
}
