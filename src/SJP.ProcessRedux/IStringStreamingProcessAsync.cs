﻿using System;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms, and assumes that input and output is textual.
    /// </summary>
    public interface IStringStreamingProcessAsync : IProcessAsync
    {
        /// <summary>
        /// Occurs when an application writes a line to its redirected <see cref="StandardError"/> stream. Provides the data received from the standard error stream.
        /// </summary>
        event EventHandler<string> ErrorLineReceived;

        /// <summary>
        /// Occurs each time an application writes a line to its redirected <see cref="StandardOutput"/> stream. Provides the data received from the standard output stream.
        /// </summary>
        event EventHandler<string> OutputLineReceived;
    }
}