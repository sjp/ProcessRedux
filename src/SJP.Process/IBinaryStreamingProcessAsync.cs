using System;
using System.IO;
using System.Threading.Tasks;
using SysStartInfo = System.Diagnostics.ProcessStartInfo;

namespace SJP.Process
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms, but do not assume that output is textual.
    /// </summary>
    public interface IBinaryStreamingProcessAsync
    {
        /// <summary>
        /// Gets the value that the associated process specified when it terminated.
        /// </summary>
        int ExitCode { get; }

        /// <summary>
        /// Gets a value indicating whether the associated process has been terminated.
        /// </summary>
        bool HasExited { get; }

        /// <summary>
        /// Gets the unique identifier for the associated process.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the name of the computer the associated process is running on.
        /// </summary>
        string MachineName { get; }

        /// <summary>
        /// Gets the amount of nonpaged system memory, in bytes, allocated for the associated process.
        /// </summary>
        long NonpagedSystemMemorySize { get; }

        /// <summary>
        /// Gets the amount of pageable system memory, in bytes, allocated for the associated process.
        /// </summary>
        long PagedSystemMemorySize { get; }

        /// <summary>
        /// Gets the maximum amount of memory in the virtual memory paging file, in bytes, used by the associated process.
        /// </summary>
        long PeakPagedMemorySize { get; }

        /// <summary>
        /// Gets the maximum amount of virtual memory, in bytes, used by the associated process.
        /// </summary>
        long PeakVirtualMemorySize { get; }

        /// <summary>
        /// Gets the maximum amount of physical memory, in bytes, used by the associated process.
        /// </summary>
        long PeakWorkingSet { get; }

        /// <summary>
        /// Gets the amount of private memory, in bytes, allocated for the associated process.
        /// </summary>
        long PrivateMemorySize { get; }

        /// <summary>
        /// Gets the privileged processor time for this process.
        /// </summary>
        TimeSpan PrivilegedProcessorTime { get; }

        /// <summary>
        /// Gets the name of the process.
        /// </summary>
        string ProcessName { get; }

        /// <summary>
        /// Gets a stream used to write the input of the application.
        /// </summary>
        Stream StandardInput { get; }

        /// <summary>
        /// Gets or sets the properties to pass to the <see cref="Start()"/> method of the Process.
        /// </summary>
        SysStartInfo StartInfo { get; set; }

        /// <summary>
        /// Gets the time that the associated process was started.
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// Gets the total processor time for this process.
        /// </summary>
        TimeSpan TotalProcessorTime { get; }

        /// <summary>
        /// Gets the user processor time for this process.
        /// </summary>
        TimeSpan UserProcessorTime { get; }

        /// <summary>
        /// Gets the amount of the virtual memory, in bytes, allocated for the associated process.
        /// </summary>
        long VirtualMemorySize { get; }

        /// <summary>
        /// Gets the amount of physical memory, in bytes, allocated for the associated process.
        /// </summary>
        long WorkingSet { get; }

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        void Kill();

        /// <summary>
        /// Discards any information about the associated process that has been cached inside the process component.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Starts (or reuses) the process resource that is specified by the <see cref="StartInfo"/> property of this <see cref="IBinaryStreamingProcessAsync"/> component and associates it with the component.
        /// </summary>
        /// <returns><c>true</c> if a process resource is started; <c>false</c> if no new process resource is started (for example, if an existing process is reused).</returns>
        bool Start();

        /// <summary>
        /// Instructs the <see cref="IBinaryStreamingProcessAsync"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>A task that represents the asynchronous wait operation. The value of the task is the exit code of the process when it has closed.</returns>
        Task<int> WaitForExitAsync();

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
