using System;
using System.Collections.Generic;
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
        private static IEnumerable<string> AppPaths
        {
            get
            {
                // see:
                // https://blogs.msdn.microsoft.com/oldnewthing/20110725-00/?p=10073
                // https://helgeklein.com/blog/2010/08/how-the-app-paths-registry-key-makes-windows-both-faster-and-safer/

                var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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
                            result.Add(appPathValue);

                        subKey.Dispose();
                    }

                    appPathKey.Dispose();
                }

                return result;
            }
        }

        private static bool IsWindows => _isWindowsLoader.Value;

#if NETFX
        private readonly static Lazy<bool> _isWindowsLoader = new Lazy<bool>(() => Environment.OSVersion.Platform == PlatformID.Win32NT);
#else
        private readonly static Lazy<bool> _isWindowsLoader = new Lazy<bool>(() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
#endif
    }
}
