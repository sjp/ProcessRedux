using System;
using NUnit.Framework;
using Moq;

namespace SJP.ProcessRedux.Tests
{
    [TestFixture]
    public class DataStreamingProcessTests : ProcessTest
    {
        [Test]
        public void Ctor_GivenNullConfig_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DataStreamingProcess(null));
        }

        [Test]
        public void State_WhenNotStarted_ThrowsInvalidOperationException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var dataProcess = new DataStreamingProcess(config))
                Assert.Throws<InvalidOperationException>(() => { var x = dataProcess.State; });
        }

        [Test]
        public void StandardInput_WhenNotStarted_ThrowsInvalidOperationException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var dataProcess = new DataStreamingProcess(config))
                Assert.Throws<InvalidOperationException>(() => { var x = dataProcess.StandardInput; });
        }

        [Test]
        public void HasStarted_WhenNotStarted_ReturnsFalse()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var dataProcess = new DataStreamingProcess(config))
                Assert.IsFalse(dataProcess.HasStarted);
        }

        [Test]
        public void HasExited_WhenNotStarted_ReturnsFalse()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var dataProcess = new DataStreamingProcess(config))
                Assert.IsFalse(dataProcess.HasExited);
        }

        /*
        [Test]
        public void Exited_WhenProcessExits_ReturnsExitCode()
        {
            // expect from the process when nothing given to have exit code of 1
            const int errorCode = 1;
            var config = new ProcessConfiguration(TestProcessFilePath);
            var result = -1;

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Exited += (s, exitCode) => result = exitCode;
                dataProcess.Start();
                dataProcess.WaitForExit();
            }

            Assert.AreEqual(errorCode, result);
        }

        [Test]
        public void HasStarted_WhenProcessStarts_ReturnsTrue()
        {
            var config = new ProcessConfiguration(TestProcessFilePath);

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                Assert.IsTrue(dataProcess.HasStarted);
                dataProcess.WaitForExit();
            }
        }

        [Test]
        public void HasExited_WhenProcessExits_ReturnsTrue()
        {
            var config = new ProcessConfiguration(TestProcessFilePath);

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                dataProcess.WaitForExit();
                Assert.IsTrue(dataProcess.HasExited);
            }
        }*/
    }
}
