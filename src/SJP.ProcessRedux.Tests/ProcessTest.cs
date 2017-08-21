using System.IO;
using System.Reflection;

namespace SJP.ProcessRedux.Tests
{
    public class ProcessTest
    {
        protected static IProcessConfiguration TestProcessConfig => new ProcessConfiguration("dotnet.exe") { Arguments = TestProcessFilePath };

        protected static string TestProcessFilePath => Path.Combine(CurrentDirectory, TestProcessExecutableFile);

        private static string CurrentDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private const string TestProcessExecutableFile = "SJP.ProcessRedux.Tests.ConsoleProcess.dll";
    }
}
