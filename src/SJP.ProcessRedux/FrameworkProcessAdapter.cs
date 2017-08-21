using System;
using System.IO;
using System.Diagnostics;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// A wrapper for a <see cref="Process"/> instance that implements <see cref="IFrameworkProcess"/>.
    /// </summary>
    public sealed class FrameworkProcessAdapter : IFrameworkProcess
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FrameworkProcessAdapter"/>.
        /// </summary>
        /// <param name="process">A <see cref="Process"/> instance that has not yet been started.</param>
        /// <exception cref="ArgumentNullException"><paramref name="process"/> is <c>null</c>.</exception>
        public FrameworkProcessAdapter(Process process)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process));
        }

        /// <summary>
        /// A convenience operator so that <see cref="Process"/> instances can more easily be treated as an <see cref="IFrameworkProcess"/>
        /// </summary>
        /// <param name="process">A <see cref="Process"/> instance that has not yet been started.</param>
        public static implicit operator FrameworkProcessAdapter(Process process) => new FrameworkProcessAdapter(process);

        /// <summary>
        /// Occurs when an application writes to its redirected <see cref="StandardError"/> stream.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> ErrorDataReceived
        {
            add => _process.ErrorDataReceived += value.Invoke;
            remove => _process.ErrorDataReceived -= value.Invoke;
        }

        /// <summary>
        /// Occurs when a process exits.
        /// </summary>
        public event EventHandler Exited
        {
            add => _process.Exited += value.Invoke;
            remove => _process.Exited -= value.Invoke;
        }

        /// <summary>
        /// Occurs each time an application writes a line to its redirected <see cref="StandardOutput"/> stream.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> OutputDataReceived
        {
            add => _process.OutputDataReceived += value.Invoke;
            remove => _process.OutputDataReceived -= value.Invoke;
        }

        /// <summary>
        /// Gets or sets whether the <see cref="Exited"/> event should be raised when the process terminates.
        /// </summary>
        public bool EnableRaisingEvents
        {
            get => _process.EnableRaisingEvents;
            set => _process.EnableRaisingEvents = value;
        }

        /// <summary>
        /// Gets the value that the associated process specified when it terminated.
        /// </summary>
        public int ExitCode => _process.ExitCode;

        /// <summary>
        /// Gets a value indicating whether the associated process has been terminated.
        /// </summary>
        public bool HasExited => _process.HasExited;

        /// <summary>
        /// Gets a value indicating whether the associated process has started.
        /// </summary>
        public bool HasStarted => _hasStarted || GetHasStarted();

        /// <summary>
        /// Gets the unique identifier for the associated process.
        /// </summary>
        public int Id => _process.Id;

        /// <summary>
        /// Gets the name of the computer the associated process is running on.
        /// </summary>
        public string MachineName => _process.MachineName;

        /// <summary>
        /// Gets the amount of nonpaged system memory, in bytes, allocated for the associated process.
        /// </summary>
        public long NonpagedSystemMemorySize => _process.NonpagedSystemMemorySize64;

        /// <summary>
        /// Gets the amount of pageable system memory, in bytes, allocated for the associated process.
        /// </summary>
        public long PagedSystemMemorySize => _process.PagedSystemMemorySize64;

        /// <summary>
        /// Gets the maximum amount of memory in the virtual memory paging file, in bytes, used by the associated process.
        /// </summary>
        public long PeakPagedMemorySize => _process.PeakPagedMemorySize64;

        /// <summary>
        /// Gets the maximum amount of virtual memory, in bytes, used by the associated process.
        /// </summary>
        public long PeakVirtualMemorySize => _process.PeakVirtualMemorySize64;

        /// <summary>
        /// Gets the maximum amount of physical memory, in bytes, used by the associated process.
        /// </summary>
        public long PeakWorkingSet => _process.PeakWorkingSet64;

        /// <summary>
        /// Gets the amount of private memory, in bytes, allocated for the associated process.
        /// </summary>
        public long PrivateMemorySize => _process.PrivateMemorySize64;

        /// <summary>
        /// Gets the privileged processor time for this process.
        /// </summary>
        public TimeSpan PrivilegedProcessorTime => _process.PrivilegedProcessorTime;

        /// <summary>
        /// Gets the name of the process.
        /// </summary>
        public string ProcessName => _process.ProcessName;

        /// <summary>
        /// Gets a stream reader used to read the textual output of the application.
        /// </summary>
        public StreamReader StandardOutput => _process.StandardOutput;

        /// <summary>
        /// Gets a stream writer used to write the input of the application.
        /// </summary>
        public StreamWriter StandardInput => _process.StandardInput;

        /// <summary>
        /// Gets a stream reader used to read the error output of the application.
        /// </summary>
        public StreamReader StandardError => _process.StandardError;

        /// <summary>
        /// Gets or sets the properties used by the <see cref="Start()"/> method of the Process.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <see cref="ProcessStartInfo"/> provided was <c>null</c>.</exception>
        public ProcessStartInfo StartInfo
        {
            get => _process.StartInfo;
            set => _process.StartInfo = value ?? throw new ArgumentNullException(nameof(StartInfo));
        }

        /// <summary>
        /// Gets the time that the associated process was started.
        /// </summary>
        public DateTime StartTime => _process.StartTime;

        /// <summary>
        /// Gets the total processor time for this process.
        /// </summary>
        public TimeSpan TotalProcessorTime => _process.TotalProcessorTime;

        /// <summary>
        /// Gets the user processor time for this process.
        /// </summary>
        public TimeSpan UserProcessorTime => _process.UserProcessorTime;

        /// <summary>
        /// Gets the amount of the virtual memory, in bytes, allocated for the associated process.
        /// </summary>
        public long VirtualMemorySize => _process.VirtualMemorySize64;

        /// <summary>
        /// Gets the amount of physical memory, in bytes, allocated for the associated process.
        /// </summary>
        public long WorkingSet => _process.WorkingSet64;

        /// <summary>
        /// Begins asynchronous read operations on the redirected <see cref="StandardError"/> stream of the application.
        /// </summary>
        public void BeginErrorReadLine() => _process.BeginErrorReadLine();

        /// <summary>
        /// Begins asynchronous read operations on the redirected <see cref="StandardOutput"/> stream of the application.
        /// </summary>
        public void BeginOutputReadLine() => _process.BeginOutputReadLine();

        /// <summary>
        /// Cancels the asynchronous read operation on the redirected <see cref="StandardError"/> stream of an application.
        /// </summary>
        public void CancelErrorRead() => _process.CancelErrorRead();

        /// <summary>
        /// Cancels the asynchronous read operation on the redirected <see cref="StandardOutput"/> stream of an application.
        /// </summary>
        public void CancelOutputRead() => _process.CancelOutputRead();

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        public void Kill() => _process.Kill();

        /// <summary>
        /// Discards any information about the associated process that has been cached inside the process component.
        /// </summary>
        public void Refresh() => _process.Refresh();

        /// <summary>
        /// Starts (or reuses) the process resource that is specified by the <see cref="StartInfo"/> property of this <see cref="FrameworkProcessAdapter"/> component and associates it with the component.
        /// </summary>
        /// <returns><c>true</c> if a process resource is started; <c>false</c> otherwise.</returns>
        public bool Start() => _process.Start();

        /// <summary>
        /// Instructs the <see cref="FrameworkProcessAdapter"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        public void WaitForExit() => _process.WaitForExit();

        /// <summary>
        /// Instructs the <see cref="FrameworkProcessAdapter"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
        public bool WaitForExit(int milliseconds) => _process.WaitForExit(milliseconds);

        private bool GetHasStarted()
        {
            try
            {
                var id = _process.Id;
                _hasStarted = true;
                return _hasStarted;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        /// <summary>
        /// Releases resources used by the current <see cref="FrameworkProcessAdapter"/> instance.
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Releases resources used by the current <see cref="ObservableDataStreamingProcess"/> instance.
        /// </summary>
        /// <param name="disposing"><b>True</b> if managed resources are to be disposed. <b>False</b> will not dispose any resources.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (!disposing)
                return;

            _process.Dispose();
            _disposed = true;
        }

        private bool _hasStarted;
        private bool _disposed;
        private readonly Process _process;
    }
}
