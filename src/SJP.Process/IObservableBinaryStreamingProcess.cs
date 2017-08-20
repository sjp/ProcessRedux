using System;
using System.IO;
using System.Threading.Tasks;
using SysStartInfo = System.Diagnostics.ProcessStartInfo;

namespace SJP.Process
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms, but do not assume that output is textual.
    /// </summary>
    public interface IObservableBinaryStreamingProcess
    {
        /// <summary>
        /// Instructs the <see cref="IObservableBinaryStreamingProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>A task that represents the asynchronous wait operation. The value of the task is the exit code of the process when it has closed.</returns>
        Task<int> WaitForExitAsync();

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
