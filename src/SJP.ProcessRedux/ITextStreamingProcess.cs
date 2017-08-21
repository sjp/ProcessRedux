using System;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms, and assumes that redirected output is textual.
    /// </summary>
    public interface ITextStreamingProcess : IProcess
    {
        /// <summary>
        /// Occurs when an application writes a line to its redirected standard error stream. Provides the data received from the standard error stream.
        /// </summary>
        event EventHandler<string> ErrorLineReceived;

        /// <summary>
        /// Occurs each time an application writes a line to its redirected standard output stream. Provides the data received from the standard output stream.
        /// </summary>
        event EventHandler<string> OutputLineReceived;
    }
}
