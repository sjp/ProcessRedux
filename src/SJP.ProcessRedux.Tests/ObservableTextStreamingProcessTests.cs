using System;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using System.Text;

namespace SJP.ProcessRedux.Tests
{
    [TestFixture]
    internal class ObservableTextStreamingProcessTests : ProcessTest
    {
        [Test]
        public static void Ctor_GivenNullConfig_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ObservableTextStreamingProcess(null));
        }

        [Test]
        public static void State_WhenNotStarted_ThrowsInvalidOperationException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var textProcess = new ObservableTextStreamingProcess(config))
                Assert.Throws<InvalidOperationException>(() => _ = textProcess.State);
        }

        [Test]
        public static void State_WhenExited_ThrowsInvalidOperationException()
        {
            var config = new ProcessConfiguration(TestProcessFilePath);
            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                Task.Delay(100).Wait();
                Assert.Throws<InvalidOperationException>(() => _ = textProcess.State);
                textProcess.WaitForExit();
            }
        }

        [Test]
        public static void StandardInput_WhenNotStarted_ThrowsInvalidOperationException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var textProcess = new ObservableTextStreamingProcess(config))
                Assert.Throws<InvalidOperationException>(() => _ = textProcess.StandardInput);
        }

        [Test]
        public static void StandardInput_WhenExited_ThrowsInvalidOperationException()
        {
            var config = new ProcessConfiguration(TestProcessFilePath);
            using (var textProcess = new ObservableTextStreamingProcess(config))
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

            using (var textProcess = new ObservableTextStreamingProcess(config))
                Assert.Throws<InvalidOperationException>(() => textProcess.Kill());
        }

        [Test]
        public static void KillAsync_WhenProcessNotStarted_ThrowsInvalidOperationException()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            // using aggregate, but it really constains invalid op
            using (var textProcess = new ObservableTextStreamingProcess(config))
                Assert.Throws<AggregateException>(() => textProcess.KillAsync().Wait());
        }

        [Test]
        public static void HasStarted_WhenNotStarted_ReturnsFalse()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var textProcess = new ObservableTextStreamingProcess(config))
                Assert.IsFalse(textProcess.HasStarted);
        }

        [Test]
        public static void HasExited_WhenNotStarted_ReturnsFalse()
        {
            const string fileName = "notepad.exe";
            var config = new ProcessConfiguration(fileName);

            using (var textProcess = new ObservableTextStreamingProcess(config))
                Assert.IsFalse(textProcess.HasExited);
        }

        [Test]
        public static void Exited_WhenProcessExits_ReturnsExitCode()
        {
            // expect from the process when nothing given to have exit code of 1
            const int errorCode = 1;
            var config = new ProcessConfiguration(TestProcessFilePath);
            var result = -1;

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Exited += (_, exitCode) => result = exitCode;
                textProcess.Start();
                textProcess.WaitForExit();
                Task.Delay(1000).Wait();
            }

            Assert.AreEqual(errorCode, result);
        }

        [Test]
        public static void HasStarted_WhenProcessStarts_ReturnsTrue()
        {
            var config = new ProcessConfiguration(TestProcessFilePath);

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                Assert.IsTrue(textProcess.HasStarted);
                textProcess.WaitForExit();
            }
        }

        [Test]
        public static void HasExited_WhenProcessExits_ReturnsTrue()
        {
            var config = new ProcessConfiguration(TestProcessFilePath);

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                textProcess.WaitForExit();
                Task.Delay(1000).Wait();
                Assert.IsTrue(textProcess.HasExited);
            }
        }

        [Test]
        public static void State_WhenProcessStarts_ReturnsValue()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                var state = textProcess.State;
                textProcess.WaitForExit();
                Assert.IsNotNull(state);
            }
        }

        [Test]
        public static void StandardInput_WhenProcessStarts_ReturnsNonNullStream()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                Assert.IsNotNull(textProcess.StandardInput);
                Assert.AreNotSame(Stream.Null, textProcess.StandardInput);
                textProcess.WaitForExit();
            }
        }

        [Test]
        public static void Kill_WhenProcessRunning_KillsProcess()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                textProcess.Kill();
                Task.Delay(100);
                Assert.IsTrue(textProcess.HasExited);
            }
        }

        [Test]
        public static void Kill_WhenProcessExited_DoesNotThrowException()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                textProcess.Kill();
                textProcess.Kill();
                Assert.Pass();
            }
        }

        [Test]
        public static void WaitForExit_WhenCalled_WaitsForExitAndReturnsExitCode()
        {
            // expect from the process when nothing given to have exit code of 1
            const int errorCode = 1;
            var config = new ProcessConfiguration(TestProcessFilePath);

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                Assert.AreEqual(errorCode, textProcess.WaitForExit());
            }
        }

        [Test]
        public static async Task WaitForExitAsync_WhenCalled_WaitsForExitAsynchronouslyAndReturnsExitCode()
        {
            // expect from the process when nothing given to have exit code of 1
            const int errorCode = 1;
            var config = new ProcessConfiguration(TestProcessFilePath);

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                var exitCode = await textProcess.WaitForExitAsync().ConfigureAwait(false);
                Assert.AreEqual(errorCode, exitCode);
            }
        }

        [Test]
        public static void WaitForExitIntTimeoutOutExitCode_WhenTimeoutNotMet_ReturnsTrueAndExitCode()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                var killedInTime = textProcess.WaitForExit(2000, out var exitCode);
                Assert.IsTrue(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitIntTimeoutOutExitCode_WhenTimeoutElapsed_ReturnsFalseAndZero()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait5Seconds };

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                var killedInTime = textProcess.WaitForExit(2000, out var exitCode);
                Assert.IsFalse(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitIntTimeoutExitCodeTuple_WhenTimeoutNotMet_ReturnsTrueAndExitCode()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                var (killedInTime, exitCode) = textProcess.WaitForExit(2000);
                Assert.IsTrue(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitIntTimeoutExitCodeTuple_WhenTimeoutElapsed_ReturnsFalseAndZero()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait5Seconds };

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                var (killedInTime, exitCode) = textProcess.WaitForExit(2000);
                Assert.IsFalse(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitTimespanTimeoutOutExitCode_WhenTimeoutNotMet_ReturnsTrueAndExitCode()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                var timeout = TimeSpan.FromSeconds(2);
                var killedInTime = textProcess.WaitForExit(timeout, out var exitCode);
                Assert.IsTrue(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitTimespanTimeoutOutExitCode_WhenTimeoutElapsed_ReturnsFalseAndZero()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait5Seconds };

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                var timeout = TimeSpan.FromSeconds(2);
                var killedInTime = textProcess.WaitForExit(timeout, out var exitCode);
                Assert.IsFalse(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitTimespanTimeoutExitCodeTuple_WhenTimeoutNotMet_ReturnsTrueAndExitCode()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait1Second };

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                var timeout = TimeSpan.FromSeconds(2);
                var (killedInTime, exitCode) = textProcess.WaitForExit(timeout);
                Assert.IsTrue(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public static void WaitForExitTimespanTimeoutExitCodeTuple_WhenTimeoutElapsed_ReturnsFalseAndZero()
        {
            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.Wait5Seconds };

            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.Start();
                var timeout = TimeSpan.FromSeconds(2);
                var (killedInTime, exitCode) = textProcess.WaitForExit(timeout);
                Assert.IsFalse(killedInTime);
                Assert.AreEqual(0, exitCode);
            }
        }

        // uncomment when running locally, seems to fail on CI
        /*[Test]
        public static void ErrorLineReceived_WhenSubscribed_ReturnsExpectedData()
        {
            var expected = Constants.Data.StdErrText;
            var result = new StringBuilder();

            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.WriteStdErrText };
            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.ErrorLines.Subscribe(line => result.Append(line));
                textProcess.Start();
                textProcess.WaitForExit();
            }

            Task.Delay(1000).Wait();
            var resultStr = result.ToString();
            Assert.AreEqual(expected, resultStr);
        }*/

        // uncomment when running locally, seems to fail on CI
        /*[Test]
        public static void OutputLineReceived_WhenSubscribed_ReturnsExpectedData()
        {
            var expected = Constants.Data.StdOutText;
            var result = new StringBuilder();

            var config = new ProcessConfiguration(TestProcessFilePath) { Arguments = Constants.Arguments.WriteStdOutText };
            using (var textProcess = new ObservableTextStreamingProcess(config))
            {
                textProcess.OutputLines.Subscribe(line => result.Append(line));
                textProcess.Start();
                textProcess.WaitForExit();
            }

            Task.Delay(1000).Wait();
            var resultStr = result.ToString();
            Assert.AreEqual(expected, resultStr);
        }*/
    }
}
