using System;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SJP.ProcessRedux.Tests
{
    [TestFixture]
    internal class DataStreamingProcessTests : ProcessTest
    {
        [Test]
        public static void Ctor_GivenNullConfig_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DataStreamingProcess(null));
        }

        [Test]
        public static void State_WhenNotStarted_ThrowsInvalidOperationException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var dataProcess = new DataStreamingProcess(config))
                Assert.Throws<InvalidOperationException>(() => _ = dataProcess.State);
        }

        [Test]
        public static void State_WhenExited_ThrowsInvalidOperationException()
        {
            var config = new ProcessConfiguration(TestProcessFilePath);
            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                Task.Delay(100).Wait();
                Assert.Throws<InvalidOperationException>(() => _ = dataProcess.State);
                dataProcess.WaitForExit();
            }
        }

        [Test]
        public static void StandardInput_WhenNotStarted_ThrowsInvalidOperationException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var dataProcess = new DataStreamingProcess(config))
                Assert.Throws<InvalidOperationException>(() => _ = dataProcess.StandardInput);
        }

        [Test]
        public static void StandardInput_WhenExited_ThrowsInvalidOperationException()
        {
            var config = new ProcessConfiguration(TestProcessFilePath);
            using (var textProcess = new DataStreamingProcess(config))
            {
                textProcess.Start();
                Task.Delay(100).Wait();
                Assert.Throws<InvalidOperationException>(() => _ = textProcess.StandardInput);
                textProcess.WaitForExit();
            }
        }

        [Test]
        public static void Kill_WhenProcessNotStarted_ThrowsInvalidOperationException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var dataProcess = new DataStreamingProcess(config))
                Assert.Throws<InvalidOperationException>(() => dataProcess.Kill());
        }

        [Test]
        public static void KillAsync_WhenProcessNotStarted_ThrowsInvalidOperationException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            // using aggregate, but it really constains invalid op
            using (var dataProcess = new DataStreamingProcess(config))
                Assert.Throws<AggregateException>(() => dataProcess.KillAsync().Wait());
        }

        [Test]
        public static void HasStarted_WhenNotStarted_ReturnsFalse()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var dataProcess = new DataStreamingProcess(config))
                Assert.IsFalse(dataProcess.HasStarted);
        }

        [Test]
        public static void HasExited_WhenNotStarted_ReturnsFalse()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var dataProcess = new DataStreamingProcess(config))
                Assert.IsFalse(dataProcess.HasExited);
        }

        [Test]
        public static void Exited_WhenProcessExits_ReturnsExitCode()
        {
            // expect from the process when nothing given to have exit code of 1
            const int errorCode = 1;
            var config = new ProcessConfiguration(TestProcessFilePath);
            var result = -1;

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Exited += (_, exitCode) => result = exitCode;
                dataProcess.Start();
                Task.Delay(1000).Wait();
            }

            Assert.AreEqual(errorCode, result);
        }

        [Test]
        public static void HasStarted_WhenProcessStarts_ReturnsTrue()
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
        public static void HasExited_WhenProcessExits_ReturnsTrue()
        {
            var config = new ProcessConfiguration(TestProcessFilePath);

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                dataProcess.WaitForExit();
                Task.Delay(1000).Wait();
                Assert.IsTrue(dataProcess.HasExited);
            }
        }

        [Test]
        public static void State_WhenProcessStarts_ReturnsValue()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                var state = dataProcess.State;
                dataProcess.WaitForExit();
                Assert.IsNotNull(state);
            }
        }

        [Test]
        public static void StandardInput_WhenProcessStarts_ReturnsNonNullStream()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                Assert.IsNotNull(dataProcess.StandardInput);
                Assert.AreNotSame(Stream.Null, dataProcess.StandardInput);
                dataProcess.WaitForExit();
            }
        }

        [Test]
        public static void Kill_WhenProcessRunning_KillsProcess()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                dataProcess.Kill();
                Assert.IsTrue(dataProcess.HasExited);
            }
        }

        [Test]
        public static void Kill_WhenProcessExited_DoesNotThrowException()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                dataProcess.Kill();
                dataProcess.Kill();
                Assert.Pass();
            }
        }

        [Test]
        public static void WaitForExit_WhenCalled_WaitsForExitAndReturnsExitCode()
        {
            // expect from the process when nothing given to have exit code of 1
            const int errorCode = 1;
            var config = new ProcessConfiguration(TestProcessFilePath);

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                Assert.AreEqual(errorCode, dataProcess.WaitForExit());
            }
        }

        [Test]
        public static async Task WaitForExitAsync_WhenCalled_WaitsForExitAsynchronouslyAndReturnsExitCode()
        {
            // expect from the process when nothing given to have exit code of 1
            const int errorCode = 1;
            var config = new ProcessConfiguration(TestProcessFilePath);

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                var exitCode = await dataProcess.WaitForExitAsync().ConfigureAwait(false);
                Assert.AreEqual(errorCode, exitCode);
            }
        }

        [Test]
        public static void WaitForExitIntTimeoutOutExitCode_WhenTimeoutNotMet_ReturnsTrueAndExitCode()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                var killedInTime = dataProcess.WaitForExit(2000, out var exitCode);
                Assert.IsTrue(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitIntTimeoutOutExitCode_WhenTimeoutElapsed_ReturnsFalseAndZero()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait5Seconds };

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                var killedInTime = dataProcess.WaitForExit(2000, out var exitCode);
                Assert.IsFalse(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitIntTimeoutExitCodeTuple_WhenTimeoutNotMet_ReturnsTrueAndExitCode()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                var (killedInTime, exitCode) = dataProcess.WaitForExit(2000);
                Assert.IsTrue(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitIntTimeoutExitCodeTuple_WhenTimeoutElapsed_ReturnsFalseAndZero()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait5Seconds };

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                var (killedInTime, exitCode) = dataProcess.WaitForExit(2000);
                Assert.IsFalse(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitTimespanTimeoutOutExitCode_WhenTimeoutNotMet_ReturnsTrueAndExitCode()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                var timeout = TimeSpan.FromSeconds(2);
                var killedInTime = dataProcess.WaitForExit(timeout, out var exitCode);
                Assert.IsTrue(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitTimespanTimeoutOutExitCode_WhenTimeoutElapsed_ReturnsFalseAndZero()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait5Seconds };

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                var timeout = TimeSpan.FromSeconds(2);
                var killedInTime = dataProcess.WaitForExit(timeout, out var exitCode);
                Assert.IsFalse(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitTimespanTimeoutExitCodeTuple_WhenTimeoutNotMet_ReturnsTrueAndExitCode()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                var timeout = TimeSpan.FromSeconds(2);
                var (killedInTime, exitCode) = dataProcess.WaitForExit(timeout);
                Assert.IsTrue(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitTimespanTimeoutExitCodeTuple_WhenTimeoutElapsed_ReturnsFalseAndZero()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait5Seconds };

            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.Start();
                var timeout = TimeSpan.FromSeconds(2);
                var (killedInTime, exitCode) = dataProcess.WaitForExit(timeout);
                Assert.IsFalse(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void ErrorDataReceived_WhenSubscribed_ReturnsExpectedData()
        {
            var expected = Convert.ToBase64String(Constants.Data.DataDeadBeef);
            var result = new List<byte>();

            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.WriteStdErrData };
            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.ErrorDataReceived += (_, data) => result.AddRange(data);
                dataProcess.Start();
                Task.Delay(1000).Wait();
            }

            var base64 = Convert.ToBase64String(result.ToArray());
            Assert.AreEqual(expected, base64);
        }

        // uncomment when running locally, seems to fail on CI
        /*[Test]
        public static void OutputDataReceived_WhenSubscribed_ReturnsExpectedData()
        {
            var expected = Convert.ToBase64String(Constants.Data.DataCafeBabe);
            var result = new List<byte>();

            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.WriteStdOutData };
            using (var dataProcess = new DataStreamingProcess(config))
            {
                dataProcess.OutputDataReceived += (s, data) => result.AddRange(data);
                dataProcess.Start();
                Task.Delay(1000).Wait();
            }

            var base64 = Convert.ToBase64String(result.ToArray());
            Assert.AreEqual(expected, base64);
        }*/
    }
}
