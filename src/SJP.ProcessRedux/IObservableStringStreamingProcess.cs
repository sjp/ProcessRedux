using System;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms, and assumes that input and output is textual.
    /// </summary>
    public interface IObservableStringStreamingProcess : IProcessAsync
    {
        /// <summary>
        /// Provides a subscriptions to events when a process writes a line to its redirected <see cref="StandardError"/> stream.
        /// </summary>
        IObservable<string> ErrorLines { get; }

        /// <summary>
        /// Provides a subscriptions to events when a process writes a line to its redirected <see cref="StandardOutput"/> stream.
        /// </summary>
        IObservable<string> OutputLines { get; }
    }
}
