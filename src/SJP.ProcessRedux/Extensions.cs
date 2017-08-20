using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

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
        public static bool IsNullOrEmpty(this string input) => string.IsNullOrEmpty(input);

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
                Domain = processConfig.Credentials?.Domain,
                FileName = processConfig.FileName,
                Password = processConfig.Credentials?.SecurePassword,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UserName = processConfig.Credentials?.UserName,
                UseShellExecute = false,
                WorkingDirectory = processConfig.WorkingDirectory
            };

            var configVars = processConfig.EnvironmentVariables ?? new Dictionary<string, string>();
            if (configVars.Count > 0)
            {
                foreach (var kv in configVars)
                    startInfo.EnvironmentVariables[kv.Key] = kv.Value;
            }

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
            var hasCredentials = !startInfo.Domain.IsNullOrWhiteSpace()
                || !startInfo.UserName.IsNullOrWhiteSpace()
                || startInfo.Password != null
                || !startInfo.PasswordInClearText.IsNullOrWhiteSpace();
            if (hasCredentials)
            {
                credentials = startInfo.Password != null
                    ? new NetworkCredential(startInfo.UserName, startInfo.Password, startInfo.Domain)
                    : new NetworkCredential(startInfo.UserName, startInfo.PasswordInClearText, startInfo.Domain);
            }

            var processConfig = new ProcessConfiguration(startInfo.FileName)
            {
                Arguments = startInfo.Arguments,
                Credentials = credentials,
                WorkingDirectory = startInfo.WorkingDirectory
            };

            if (startInfo.EnvironmentVariables != null)
            {
                foreach (DictionaryEntry entry in startInfo.EnvironmentVariables)
                    processConfig.EnvironmentVariables[entry.Key.ToString()] = entry.Value.ToString();
            }

            return processConfig;
        }
    }
}
