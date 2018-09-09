using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Methods for working with process paths. Primarily used to find fully qualified paths to executables.
    /// </summary>
    public static class ProcessPath
    {
        /// <summary>
        /// Retrieves the path to the first executable that will be run when given only its filename. Useful for running system or global processes, e.g. ls, wget, cmd.exe.
        /// </summary>
        /// <param name="executableFileName">The file name of the executable.</param>
        /// <returns>A full path to an executable, <c>null</c> if a path could not be found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="executableFileName"/> is <c>null</c>, empty or whitespace.</exception>
        public static string GetFullPath(string executableFileName)
        {
            if (executableFileName.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(executableFileName));

            return GetFullPaths(executableFileName).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves all full paths to executables that can be started given only its filename. Useful for running system or global processes, e.g. ls, wget, cmd.exe.
        /// </summary>
        /// <param name="executableFileName">The file name of the executable.</param>
        /// <returns>A collection of paths to executables.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="executableFileName"/> is <c>null</c>, empty or whitespace.</exception>
        public static IEnumerable<string> GetFullPaths(string executableFileName)
        {
            if (executableFileName.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(executableFileName));

            if (IsWindows)
                return GetFullPathsWindows(executableFileName);

            if (IsOsx)
                return GetFullPathsOsx(executableFileName);

            return GetFullPathsLinux(executableFileName);
        }

        // use case insensitive comparison for windows
        private static IEnumerable<string> GetFullPathsWindows(string fileName)
        {
            var extensions = Path.HasExtension(fileName)
                ? new[] { Path.GetExtension(fileName) }
                : PathExtensions;

            var fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
            var searchFileNames = new HashSet<string>(extensions.Select(ext => fileNameNoExt + ext), StringComparer.OrdinalIgnoreCase);

            var uniqueCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var result = WindowsSearchDirs
                .SelectMany(Directory.EnumerateFiles)
                .Where(name => searchFileNames.Contains(Path.GetFileName(name)) && uniqueCheck.Add(name))
                .ToList();

            var appPaths = AppPaths;
            if (appPaths.ContainsKey(fileName) && !uniqueCheck.Contains(appPaths[fileName]))
                result.Add(appPaths[fileName]);

            return result;
        }

        // use case insensitive file matching on osx
        private static IEnumerable<string> GetFullPathsOsx(string fileName)
        {
            var uniqueCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var matchingFiles = PathDirs
                .SelectMany(Directory.EnumerateFiles)
                .Where(name => string.Equals(fileName, Path.GetFileName(name), StringComparison.OrdinalIgnoreCase))
                .ToList();

            var result = new List<string>();
            foreach (var matchingFile in matchingFiles)
            {
                if (uniqueCheck.Add(matchingFile))
                    result.Add(matchingFile);
            }

            return result;
        }

        private static IEnumerable<string> GetFullPathsLinux(string fileName)
        {
            return PathDirs
                .SelectMany(Directory.EnumerateFiles)
                .Where(name => fileName == Path.GetFileName(name))
                .Distinct()
                .ToList();
        }

        private static IEnumerable<string> WindowsSearchDirs
        {
            get
            {
                var searchDirs = new List<string>() { Directory.GetCurrentDirectory() };

                var winDir = Environment.GetEnvironmentVariable("windir");
                if (winDir.IsNullOrWhiteSpace() || !Directory.Exists(winDir))
                    winDir = Environment.GetEnvironmentVariable("SystemRoot");
                if (winDir.IsNullOrWhiteSpace() || !Directory.Exists(winDir))
                {
                    searchDirs.AddRange(PathDirs);
                    return searchDirs;
                }
                searchDirs.Add(winDir);

                var sys32Dir = Path.Combine(winDir, "system32");
                if (sys32Dir.IsNullOrWhiteSpace() || !Directory.Exists(sys32Dir))
                {
                    searchDirs.AddRange(PathDirs);
                    return searchDirs;
                }
                searchDirs.Add(sys32Dir);

                var uniqueCheck = new HashSet<string>(searchDirs, StringComparer.OrdinalIgnoreCase);

                foreach (var dir in PathDirs)
                {
                    if (uniqueCheck.Add(dir))
                        searchDirs.Add(dir);
                }

                return searchDirs;
            }
        }

        private static IEnumerable<string> PathDirs
        {
            get
            {
                var pathEnvVar = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                return pathEnvVar.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Trim())
                    .Where(d => !d.IsNullOrWhiteSpace())
                    .Select(Environment.ExpandEnvironmentVariables)
                    .Where(Directory.Exists)
                    .ToList();
            }
        }

        private static IEnumerable<string> PathExtensions
        {
            get
            {
                var pathExtEnvVar = Environment.GetEnvironmentVariable("PATHEXT") ?? string.Empty;
                return pathExtEnvVar.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(ext => ext.Trim())
                    .Where(ext => !ext.IsNullOrWhiteSpace())
                    .ToList();
            }
        }

        private static IReadOnlyDictionary<string, string> AppPaths
        {
            get
            {
                // see:
                // https://blogs.msdn.microsoft.com/oldnewthing/20110725-00/?p=10073
                // https://helgeklein.com/blog/2010/08/how-the-app-paths-registry-key-makes-windows-both-faster-and-safer/

                var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                using (var hklm = Registry.LocalMachine)
                {
                    var appPathKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths");
                    if (appPathKey == null)
                        return result;

                    foreach (var subKeyName in appPathKey.GetSubKeyNames())
                    {
                        var subKey = appPathKey.OpenSubKey(subKeyName);
                        if (subKey == null)
                            continue;

                        var appPathValue = Convert.ToString(subKey.GetValue(string.Empty)); // "" is the (Default) value
                        if (!string.IsNullOrWhiteSpace(appPathValue))
                        {
                            // wordpad and others have quotes around the value, unwrap
                            if (appPathValue[0] == '"' && appPathValue[appPathValue.Length - 1] == '"')
                                appPathValue = appPathValue.Substring(1, appPathValue.Length - 2);

                            appPathValue = Environment.ExpandEnvironmentVariables(appPathValue);
                            if (File.Exists(appPathValue) && !result.ContainsKey(subKeyName)) // keep only the first value
                                result[subKeyName] = appPathValue;
                        }

                        subKey.Dispose();
                    }

                    appPathKey.Dispose();
                }

                return result;
            }
        }

        private static bool IsWindows { get; } = Environment.OSVersion.Platform == PlatformID.Win32NT;

        private static bool IsOsx { get; } = Environment.OSVersion.Platform == PlatformID.MacOSX;
    }
}
