using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace SJP.ProcessRedux
{
    public class ProcessConfiguration : IProcessConfiguration
    {
        public ProcessConfiguration(string processFileName)
        {
            if (processFileName.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(processFileName));

            _fileName = processFileName;
        }

        public string Arguments { get; set; }

        public NetworkCredential Credentials { get; set; }

        public IDictionary<string, string> EnvironmentVariables { get; } = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public string FileName
        {
            get => _fileName;
            set
            {
                if (value.IsNullOrWhiteSpace())
                    throw new ArgumentNullException(nameof(FileName));

                _fileName = value;
            }
        }

        public string WorkingDirectory { get; set; }

        private string _fileName;
        private readonly IDictionary<string, string> _environmentVariables = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
    }
}
