using System;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms, where output is provided directly as bytes.
    /// </summary>
    public interface IDataStreamingProcess : IProcess
    {
        /// <summary>
        /// Occurs when an application writes to its redirected standard error stream. Provides the data received from the standard error stream.
        /// </summary>
        event EventHandler<byte[]> ErrorDataReceived;

        /// <summary>
        /// Occurs each time an application writes to its redirected standard output stream. Provides the data received from the standard output stream.
        /// </summary>
        event EventHandler<byte[]> OutputDataReceived;
    }
}
