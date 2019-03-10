using System;
using System.IO;

namespace SJP.ProcessRedux.ConsoleTest
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            FFmpeg.Convert();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    public static class FFmpeg
    {
        public static void Convert()
        {
            const string flacInputPath = @"C:\Users\sjp\Downloads\05. End Of Days.flac";
            const string mp3OutputPath = @"C:\Users\sjp\Downloads\05. End Of Days.mp3";
            const string ffmpegPath = @"C:\Users\sjp\Downloads\ffmpeg-20170815-62dfa2b-win64-static\bin\ffmpeg.exe";

            if (File.Exists(mp3OutputPath))
                File.Delete(mp3OutputPath);

            var processConfig = new ProcessConfiguration(ffmpegPath) { Arguments = $"-i \"{ flacInputPath }\" -b:a 192k -f mp3 -" };
            using (var writer = new BinaryWriter(File.OpenWrite(mp3OutputPath)))
            using (var process = new DataStreamingProcess(processConfig))
            {
                process.OutputDataReceived += (_, data) => writer.Write(data);
                process.Start();
                _ = process.WaitForExit();
            }
        }
    }
}
