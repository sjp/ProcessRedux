using System;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms, but do not assume that output is textual.
    /// </summary>
    public interface IDataStreamingProcessAsync : IProcessAsync
    {
        /// <summary>
        /// Occurs when an application writes to its redirected <see cref="StandardError"/> stream. Provides the data received from the standard error stream.
        /// </summary>
        event EventHandler<byte[]> ErrorDataReceived;

        /// <summary>
        /// Occurs each time an application writes a line to its redirected <see cref="StandardOutput"/> stream. Provides the data received from the standard output stream.
        /// </summary>
        event EventHandler<byte[]> OutputDataReceived;
    }
}
