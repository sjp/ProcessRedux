using System;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Represents the current state of the process.
    /// </summary>
    public class ProcessState : IProcessState
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ProcessState"/>.
        /// </summary>
        /// <param name="process">A system framework process to retrieve state from. The process must have started for state to be retrieved.</param>
        public ProcessState(IFrameworkProcess process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));
            if (!process.HasStarted)
                throw new ArgumentException("The given process has not yet been started. Cannot determine the current state of a non-running process.", nameof(process));

            process.Refresh();
            ExitCode = process.HasExited ? process.ExitCode : 0;
            HasExited = process.HasExited;
            Id = process.Id;
            MachineName = process.MachineName;
            NonpagedSystemMemorySize = process.NonpagedSystemMemorySize;
            PagedSystemMemorySize = process.PagedSystemMemorySize;
            PeakPagedMemorySize = process.PeakPagedMemorySize;
            PeakVirtualMemorySize = process.PeakVirtualMemorySize;
            PeakWorkingSet = process.PeakWorkingSet;
            PrivateMemorySize = process.PrivateMemorySize;
            PrivilegedProcessorTime = process.PrivilegedProcessorTime;
            ProcessName = process.ProcessName;
            TotalProcessorTime = process.TotalProcessorTime;
            UserProcessorTime = process.UserProcessorTime;
            VirtualMemorySize = process.VirtualMemorySize;
            WorkingSet = process.WorkingSet;
        }

        /// <summary>
        /// Gets the value that the associated process specified when it terminated.
        /// </summary>
        public int ExitCode { get; }

        /// <summary>
        /// Gets a value indicating whether the associated process has been terminated.
        /// </summary>
        public bool HasExited { get; }

        /// <summary>
        /// Gets the unique identifier for the associated process.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the name of the computer the associated process is running on.
        /// </summary>
        public string MachineName { get; }

        /// <summary>
        /// Gets the amount of nonpaged system memory, in bytes, allocated for the associated process.
        /// </summary>
        public long NonpagedSystemMemorySize { get; }

        /// <summary>
        /// Gets the amount of pageable system memory, in bytes, allocated for the associated process.
        /// </summary>
        public long PagedSystemMemorySize { get; }

        /// <summary>
        /// Gets the maximum amount of memory in the virtual memory paging file, in bytes, used by the associated process.
        /// </summary>
        public long PeakPagedMemorySize { get; }

        /// <summary>
        /// Gets the maximum amount of virtual memory, in bytes, used by the associated process.
        /// </summary>
        public long PeakVirtualMemorySize { get; }

        /// <summary>
        /// Gets the maximum amount of physical memory, in bytes, used by the associated process.
        /// </summary>
        public long PeakWorkingSet { get; }

        /// <summary>
        /// Gets the amount of private memory, in bytes, allocated for the associated process.
        /// </summary>
        public long PrivateMemorySize { get; }

        /// <summary>
        /// Gets the privileged processor time for this process.
        /// </summary>
        public TimeSpan PrivilegedProcessorTime { get; }

        /// <summary>
        /// Gets the name of the process.
        /// </summary>
        public string ProcessName { get; }

        /// <summary>
        /// Gets the total processor time for this process.
        /// </summary>
        public TimeSpan TotalProcessorTime { get; }

        /// <summary>
        /// Gets the user processor time for this process.
        /// </summary>
        public TimeSpan UserProcessorTime { get; }

        /// <summary>
        /// Gets the amount of the virtual memory, in bytes, allocated for the associated process.
        /// </summary>
        public long VirtualMemorySize { get; }

        /// <summary>
        /// Gets the amount of physical memory, in bytes, allocated for the associated process.
        /// </summary>
        public long WorkingSet { get; }
    }
}
