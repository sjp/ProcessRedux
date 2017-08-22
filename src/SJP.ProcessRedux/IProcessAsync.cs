using System;
using System.IO;
using System.Threading.Tasks;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms. Additionally provides asynchronous methods for interacting with processes.
    /// </summary>
    public interface IProcessAsync : IDisposable
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
        Task KillAsync();

        /// <summary>
        /// Starts the process resource that is described by this component.
        /// </summary>
        /// <returns><c>true</c> if a process resource is started; <c>false</c> if no new process resource is started (for example, if an existing process is reused).</returns>
        bool Start();

        /// <summary>
        /// Instructs the <see cref="IProcessAsync"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>A task that represents the asynchronous wait operation. The value of the task is the exit code of the process when it has closed.</returns>
        Task<int> WaitForExitAsync();
    }
}
