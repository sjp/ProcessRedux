using System;
using System.IO;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms.
    /// </summary>
    public interface IProcess : IDisposable
    {
        /// <summary>
        /// Retrieves the current state of the process.
        /// </summary>
        IProcessState State { get; }

        /// <summary>
        /// Gets a stream used to write the input of the application.
        /// </summary>
        Stream StandardInput { get; }

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        void Kill();

        /// <summary>
        /// Starts the process resource that is described by this component.
        /// </summary>
        /// <returns><c>true</c> if a process resource is started; <c>false</c> if no new process resource is started (for example, if an existing process is reused).</returns>
        bool Start();

        /// <summary>
        /// Instructs the <see cref="IProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>The value that the associated process specified when it terminated.</returns>
        int WaitForExit();

        /// <summary>
        /// Instructs the <see cref="IProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <param name="exitCode">The value that the associated process specified when it terminated. Defaults to <c>0</c> when the timeout expired before the process exited.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
        bool WaitForExit(int milliseconds, out int exitCode);

        /// <summary>
        /// Instructs the <see cref="IProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <returns>A tuple of length two. The component returns <c>true</c> if the associated process has exited; otherwise, <c>false</c>. The second component is the value that the associated process specified when it terminated, defaults to <c>0</c> when the timeout expired before the process exited.</returns>
        (bool exited, int exitCode) WaitForExit(int milliseconds);

        /// <summary>
        /// Instructs the <see cref="IProcess"/> component to wait the specified timespan for the associated process to exit.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the associated process to exit. The maximum is the largest possible value for milliseconds of a 32-bit integer, which represents infinity to the operating system. Any timespan larger than a 32-bit millisecond timeout is assumed to be infinite.</param>
        /// <param name="exitCode">The value that the associated process specified when it terminated. Defaults to <c>0</c> when the timeout expired before the process exited.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
        bool WaitForExit(TimeSpan timeout, out int exitCode);

        /// <summary>
        /// Instructs the <see cref="IProcess"/> component to wait the specified timespan for the associated process to exit.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the associated process to exit. The maximum is the largest possible value for milliseconds of a 32-bit integer, which represents infinity to the operating system. Any timespan larger than a 32-bit millisecond timeout is assumed to be infinite.</param>
        /// <returns>A tuple of length two. The component returns <c>true</c> if the associated process has exited; otherwise, <c>false</c>. The second component is the value that the associated process specified when it terminated, defaults to <c>0</c> when the timeout expired before the process exited.</returns>
        (bool exited, int exitCode) WaitForExit(TimeSpan timeout);

        /// <summary>
        /// Occurs when a process exits. Provides the exit code of the process when it exited.
        /// </summary>
        event EventHandler<int> Exited;
    }
}
