using System.Collections.Generic;
using System.Net;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Defines properties used to configure a process.
    /// </summary>
    public interface IProcessConfiguration
    {
        /// <summary>
        /// Gets or sets the set of command-line arguments to use when starting the application.
        /// </summary>
        string Arguments { get; set; }

        /// <summary>
        /// Gets or sets credentials for a user to use when starting the process.
        /// </summary>
        NetworkCredential Credentials { get; set; }

        /// <summary>
        /// Gets search paths for files, directories for temporary files, application-specific options, and other similar information.
        /// </summary>
        IDictionary<string, string> EnvironmentVariables { get; }

        /// <summary>
        /// Gets or sets the application to start.
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Gets or sets the working directory for the process to be started. This <b>must</b> be provided if <see cref="Credentials"/> has been set.
        /// </summary>
        string WorkingDirectory { get; set; }
    }
}
