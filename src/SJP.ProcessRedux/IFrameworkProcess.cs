using System;
using System.IO;
using System.Diagnostics;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Defines properties, methods and events that are common to processes on all platforms, as implemented by <see cref="Process"/>. Includes one extra property, <see cref="HasStarted"/>.
    /// </summary>
    public interface IFrameworkProcess : IDisposable
    {
        /// <summary>
        /// Gets or sets whether the <see cref="Exited"/> event should be raised when the process terminates.
        /// </summary>
        bool EnableRaisingEvents { get; set; }

        /// <summary>
        /// Gets the value that the associated process specified when it terminated.
        /// </summary>
        /// <exception cref="InvalidOperationException">The process has not exited.</exception>
        int ExitCode { get; }

        /// <summary>
        /// Gets a value indicating whether the associated process has been terminated.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is no process associated with the object.</exception>
        bool HasExited { get; }

        /// <summary>
        /// Gets a value indicating whether the associated process has started.
        /// </summary>
        bool HasStarted { get; }

        /// <summary>
        /// Gets the unique identifier for the associated process.
        /// </summary>
        /// <exception cref="InvalidOperationException">The process's Id property has not been set.</exception>
        int Id { get; }

        /// <summary>
        /// Gets the name of the computer the associated process is running on.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is no process associated with this process object.</exception>
        string MachineName { get; }

        /// <summary>
        /// Gets the amount of paged memory, in bytes, allocated for the associated process.
        /// </summary>
        long PagedMemorySize { get; }

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
        /// <exception cref="InvalidOperationException">The process does not have an identifier, or no process is associated with the process. Can also be thrown if the process has exited.</exception>
        string ProcessName { get; }

        /// <summary>
        /// Gets a stream reader used to read the textual output of the application.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="StandardOutput"/> stream has been opened for asynchronous read operations with <see cref="BeginOutputReadLine"/>.</exception>
        StreamReader StandardOutput { get; }

        /// <summary>
        /// Gets a stream writer used to write the input of the application.
        /// </summary>
        StreamWriter StandardInput { get; }

        /// <summary>
        /// Gets a stream reader used to read the error output of the application.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="StandardError"/> stream has been opened for asynchronous read operations with <see cref="BeginErrorReadLine"/>.</exception>
        StreamReader StandardError { get; }

        /// <summary>
        /// Gets or sets the properties to pass to the <see cref="Start()"/> method of the Process.
        /// </summary>
        ProcessStartInfo StartInfo { get; set; }

        /// <summary>
        /// Gets the time that the associated process was started.
        /// </summary>
        /// <exception cref="InvalidOperationException">The process has exited or has not yet started.</exception>
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
        /// Begins asynchronous read operations on the redirected <see cref="StandardError"/> stream of the application.
        /// </summary>
        /// <exception cref="InvalidOperationException">An asynchronous read operation is already in progress on the <see cref="StandardError"/> stream. Can also be thrown when the <see cref="StandardError"/> stream has been used by a synchronous read operation.</exception>
        void BeginErrorReadLine();

        /// <summary>
        /// Begins asynchronous read operations on the redirected <see cref="StandardOutput"/> stream of the application.
        /// </summary>
        /// <exception cref="InvalidOperationException">An asynchronous read operation is already in progress on the <see cref="StandardOutput"/> stream. Can also be thrown when the <see cref="StandardOutput"/> stream has been used by a synchronous read operation.</exception>
        void BeginOutputReadLine();

        /// <summary>
        /// Cancels the asynchronous read operation on the redirected <see cref="StandardError"/> stream of an application.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="StandardError"/> stream is not enabled for asynchronous read operations.</exception>
        void CancelErrorRead();

        /// <summary>
        /// Cancels the asynchronous read operation on the redirected <see cref="StandardOutput"/> stream of an application.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="StandardOutput"/> stream is not enabled for asynchronous read operations.</exception>
        void CancelOutputRead();

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        /// <exception cref="InvalidOperationException">The process has already exited or the process has not yet started.</exception>
        void Kill();

        /// <summary>
        /// Discards any information about the associated process that has been cached inside the process component.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Starts (or reuses) the process resource that is specified by the <see cref="StartInfo"/> property of this <see cref="IFrameworkProcess"/> component and associates it with the component.
        /// </summary>
        /// <returns><c>true</c> if a process resource is started; <c>false</c> if no new process resource is started (for example, if an existing process is reused).</returns>
        /// <exception cref="InvalidOperationException">No file name was specified in the process component's <see cref="StartInfo"/>.</exception>
        bool Start();

#if NO_SYS_EXCEPTION
        /// <summary>
        /// Instructs the <see cref="IFrameworkProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
#else
        /// <summary>
        /// Instructs the <see cref="IFrameworkProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <exception cref="SystemException">No process <see cref="Id"/> has been set, and a handle from which the <see cref="Id"/> property can be determined does not exist. Can also be thrown when the process has not yet started.</exception>
#endif
        void WaitForExit();

#if NO_SYS_EXCEPTION
        /// <summary>
        /// Instructs the <see cref="IFrameworkProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
#else
        /// <summary>
        /// Instructs the <see cref="IFrameworkProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
        /// <exception cref="SystemException">No process <see cref="Id"/> has been set, and a handle from which the <see cref="Id"/> property can be determined does not exist. Can also be thrown when the process has not yet started.</exception>
#endif
        bool WaitForExit(int milliseconds);

        /// <summary>
        /// Occurs when an application writes to its redirected <see cref="StandardError"/> stream.
        /// </summary>
        event EventHandler<DataReceivedEventArgs> ErrorDataReceived;

        /// <summary>
        /// Occurs when a process exits.
        /// </summary>
        event EventHandler Exited;

        /// <summary>
        /// Occurs each time an application writes a line to its redirected <see cref="StandardOutput"/> stream.
        /// </summary>
        event EventHandler<DataReceivedEventArgs> OutputDataReceived;
    }
}
