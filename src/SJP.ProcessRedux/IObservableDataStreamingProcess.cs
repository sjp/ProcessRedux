using System;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms, but do not assume that output is textual.
    /// </summary>
    public interface IObservableDataStreamingProcess : IProcessAsync
    {
        /// <summary>
        /// Provides a subscriptions to events when a process writes data to its redirected standard error stream.
        /// </summary>
        IObservable<byte[]> ErrorData { get; }

        /// <summary>
        /// Provides a subscriptions to events when a process writes data to its redirected standard output stream.
        /// </summary>
        IObservable<byte[]> OutputData { get; }
    }
}
