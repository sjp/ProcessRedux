using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SJP.ProcessRedux.Tests.ConsoleProcess
{
    // this not intended to be run directly, only run when embedded into a test assembly
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args == null || args.Length == 0)
                return 1;

            if (args.Any(a => a == Constants.Arguments.Wait1Second))
                Thread.Sleep(1000);

            if (args.Any(a => a == Constants.Arguments.Wait5Seconds))
                Thread.Sleep(5000);

            if (args.Any(a => a == Constants.Arguments.ReadStdInText))
            {
                var line = Console.ReadLine();
                return line == Constants.Data.StdInText
                    ? ExitSuccess
                    : ExitFailure;
            }

            if (args.Any(a => a == Constants.Arguments.ReadStdInData))
            {
                byte[] bytes;
                using (var binReader = new BinaryReader(Console.OpenStandardInput()))
                    bytes = binReader.ReadBytes(4);

                return bytes.SequenceEqual(Constants.Data.DataCafeBabe)
                    ? ExitSuccess
                    : ExitFailure;
            }

            var result = ExitSuccess;
            if (args.Any(a => a == Constants.Arguments.WriteStdOutData))
            {
                using (var binWriter = new BinaryWriter(Console.OpenStandardOutput()))
                    binWriter.Write(Constants.Data.DataCafeBabe);
            }

            if (args.Any(a => a == Constants.Arguments.WriteStdErrData))
            {
                using (var binWriter = new BinaryWriter(Console.OpenStandardError()))
                    binWriter.Write(Constants.Data.DataDeadBeef);
                result = ExitFailure;
            }

            if (args.Any(a => a == Constants.Arguments.WriteStdOutText))
            {
                Console.Error.WriteLine(Constants.Data.StdOutText);
            }

            if (args.Any(a => a == Constants.Arguments.WriteStdErrText))
            {
                Console.Error.WriteLine(Constants.Data.StdErrText);
                result = ExitFailure;
            }

            return result;
        }

        private const int ExitSuccess = 0;
        private const int ExitFailure = 1;
    }

    public static class Constants
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
