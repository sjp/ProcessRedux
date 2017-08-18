using System;
using System.Diagnostics;
using System.IO;

namespace SJP.Process.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            FFmpeg.Convert();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    public static class FFmpeg
    {
        public static void Convert()
        {
            var inputPath = @"C:\Users\sjp\Downloads\05. End Of Days.flac";
            var outputPath = @"C:\Users\sjp\Downloads\05. End Of Days.mp3";
            var processPath = @"C:\Users\sjp\Downloads\ffmpeg-20170815-62dfa2b-win64-static\bin\ffmpeg.exe";

            if (File.Exists(outputPath))
                File.Delete(outputPath);

            using (var writer = new BinaryWriter(File.OpenWrite(outputPath)))
            {
                var process = new AsyncProcess();
                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = processPath,
                    Arguments = $"-i \"{ inputPath }\" -b:a 192k -f mp3 -",
                    RedirectStandardOutput = true
                };

                process.OnStandardOutputReceived += (s, data) =>
                {
                    writer.Write(data);
                };
                var result = process.StartAsync(startInfo);
                var exitCode = result.Result;
            }
        }
    }
