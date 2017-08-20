using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security;

namespace SJP.ProcessRedux
{
    internal static class NumericExtensions
    {
        public static double Clamp(this double number, double minValue, double maxValue)
        {
            if (number < minValue)
                return minValue;
            if (number > maxValue)
                return maxValue;

            return number;
        }
    }

    internal static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string input) => string.IsNullOrWhiteSpace(input);
    }

    internal static class ProcessConfigurationExtensions
    {
        public static ProcessStartInfo ToStartInfo(this IProcessConfiguration processConfig)
        {
            if (processConfig == null)
                throw new ArgumentNullException(nameof(processConfig));

            var startInfo = new ProcessStartInfo
            {
                Arguments = processConfig.Arguments,
                CreateNoWindow = true,
                FileName = processConfig.FileName,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = processConfig.WorkingDirectory
            };

#if HAS_PROCESS_CREDENTIALS
            startInfo.Domain = processConfig.Credentials?.Domain;
            startInfo.UserName = processConfig.Credentials?.UserName;
#if HAS_PROCESS_CREDENTIALS_PASSWORDINCLEARTEXT
            startInfo.PasswordInClearText = processConfig.Credentials?.Password;
#endif
#endif

#if HAS_ENVVARS
            var configVars = processConfig.EnvironmentVariables ?? new Dictionary<string, string>();
            if (configVars.Count > 0)
            {
                foreach (var kv in configVars)
                    startInfo.EnvironmentVariables[kv.Key] = kv.Value;
            }
#endif

            return startInfo;
        }
    }

    internal static class ProcessStartInfoExtensions
    {
        public static IProcessConfiguration ToProcessConfiguration(this ProcessStartInfo startInfo)
        {
            if (startInfo == null)
                throw new ArgumentNullException(nameof(startInfo));
            if (startInfo.FileName.IsNullOrWhiteSpace())
                throw new ArgumentException($"The { nameof(ProcessStartInfo) } object must contain a file name.", nameof(startInfo));

            NetworkCredential credentials = null;
#if HAS_PROCESS_CREDENTIALS_PASSWORDINCLEARTEXT
            var hasCredentials = !startInfo.Domain.IsNullOrWhiteSpace()
                || !startInfo.UserName.IsNullOrWhiteSpace()
                || !startInfo.PasswordInClearText.IsNullOrWhiteSpace();
            if (hasCredentials)
                credentials = new NetworkCredential(startInfo.UserName, startInfo.PasswordInClearText, startInfo.Domain);
#elif HAS_PROCESS_CREDENTIALS
            var hasCredentials = !startInfo.Domain.IsNullOrWhiteSpace()
                || !startInfo.UserName.IsNullOrWhiteSpace()
                || startInfo.Password != null;
            if (hasCredentials)
                credentials = new NetworkCredential(startInfo.UserName, startInfo.Password, startInfo.Domain);
#endif

            var processConfig = new ProcessConfiguration(startInfo.FileName)
            {
                Arguments = startInfo.Arguments,
                Credentials = credentials,
                WorkingDirectory = startInfo.WorkingDirectory
            };

#if HAS_ENVVARS
            if (startInfo.EnvironmentVariables != null)
            {
                foreach (DictionaryEntry entry in startInfo.EnvironmentVariables)
                    processConfig.EnvironmentVariables[entry.Key.ToString()] = entry.Value.ToString();
            }
#endif

            return processConfig;
        }
    }
}
