﻿using System;
using System.Collections.Generic;
using System.Net;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Stores configuration that will be applied to the process when it is run.
    /// </summary>
    public class ProcessConfiguration : IProcessConfiguration
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ProcessConfiguration"/>.
        /// </summary>
        /// <param name="processFileName">A path to an executable.</param>
        public ProcessConfiguration(string processFileName)
        {
            if (processFileName.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(processFileName));

            _fileName = processFileName;
        }

        /// <summary>
        /// Gets or sets the set of command-line arguments to use when starting the application.
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Gets or sets credentials for a user to use when starting the process.
        /// </summary>
        public NetworkCredential Credentials { get; set; }

        /// <summary>
        /// Gets search paths for files, directories for temporary files, application-specific options, and other similar information.
        /// </summary>
#if NO_INVARIANT_CULTURE
        public IDictionary<string, string> EnvironmentVariables { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
#else
        public IDictionary<string, string> EnvironmentVariables { get; } = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
#endif

        /// <summary>
        /// Gets or sets the application to start.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the working directory for the process to be started. This <b>must</b> be provided if <see cref="Credentials" /> has been set.
        /// </summary>
        public string WorkingDirectory { get; set; }

        private string _fileName;
    }
}
