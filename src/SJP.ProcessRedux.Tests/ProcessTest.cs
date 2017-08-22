using System.IO;
using System.Reflection;

namespace SJP.ProcessRedux.Tests
{
    [TestPlatform.Windows]
    public class ProcessTest
    {
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
    }
}
