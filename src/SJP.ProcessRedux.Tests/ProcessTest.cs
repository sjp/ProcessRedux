using System.IO;
using System.Reflection;

namespace SJP.ProcessRedux.Tests
{
    [TestPlatform.Windows]
    internal class ProcessTest
    {
        protected ProcessTest() { }

        static ProcessTest()
        {
            ExtractToCurrentDirectoryIfMissing();
        }

        protected static IProcessConfiguration TestProcessConfig => new ProcessConfiguration(TestProcessFilePath);

        protected static string TestProcessFilePath => Path.Combine(CurrentDirectory, TestProcessExecutableFile);

        private static string CurrentDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static void ExtractToCurrentDirectoryIfMissing()
        {
            if (File.Exists(TestProcessFilePath))
                return;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName().Name + "." + TestProcessExecutableFile;

            using (var resource = assembly.GetManifestResourceStream(resourceName))
            using (var writer = File.OpenWrite(TestProcessFilePath))
                resource.CopyTo(writer);
        }

        private const string TestProcessExecutableFile = "SJP.ProcessRedux.Tests.ConsoleProcess.exe";

        protected static class Constants
        {
            public static class Arguments
            {
                public static string ReadStdInData => "stdinData";

                public static string WriteStdOutData => "stdoutData";

                public static string WriteStdErrData => "stderrData";

                public static string ReadStdInText => "stdinText";

                public static string WriteStdOutText => "stdoutText";

                public static string WriteStdErrText => "stderrText";

                public static string Wait1Second => "wait1s";

                public static string Wait5Seconds => "wait5s";
            }

            public static class Data
            {
                // use for stdout
                public static byte[] DataCafeBabe => new byte[] { 0xCA, 0xFE, 0xBA, 0xBE };

                // use for stderr
                public static byte[] DataDeadBeef => new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };

                public static string StdInText => "This is standard output text.";

                public static string StdOutText => "This is standard output text.";

                public static string StdErrText => "This is standard error text.";
            }
        }
    }
}
