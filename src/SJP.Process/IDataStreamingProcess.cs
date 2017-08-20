using System;

namespace SJP.Process
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms, where output is provided directly as bytes.
    /// </summary>
    public interface IDataStreamingProcess : IProcess
    {
        /// <summary>
        /// Occurs when an application writes to its redirected <see cref="StandardError"/> stream. Provides the data received from the standard error stream.
        /// </summary>
        event EventHandler<byte[]> ErrorDataReceived;

        /// <summary>
        /// Occurs each time an application writes to its redirected <see cref="StandardOutput"/> stream. Provides the data received from the standard output stream.
        /// </summary>
        event EventHandler<byte[]> OutputDataReceived;
    }
}
