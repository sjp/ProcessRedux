using System;

namespace SJP.Process
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms, but do not assume that output is textual.
    /// </summary>
    public interface IObservableDataStreamingProcess : IProcessAsync
    {
        /// <summary>
        /// Provides a subscriptions to events when a process writes data to its redirected <see cref="StandardError"/> stream.
        /// </summary>
        IObservable<byte[]> ErrorData { get; }

        /// <summary>
        /// Provides a subscriptions to events when a process writes data to its redirected <see cref="StandardOutput"/> stream.
        /// </summary>
        IObservable<byte[]> OutputData { get; }
    }
}
