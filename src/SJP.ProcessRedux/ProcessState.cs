using System;

namespace SJP.ProcessRedux
{
    public class ProcessState : IProcessState
    {
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

        public int ExitCode { get; }

        public bool HasExited { get; }

        public int Id { get; }

        public string MachineName { get; }

        public long NonpagedSystemMemorySize { get; }

        public long PagedSystemMemorySize { get; }

        public long PeakPagedMemorySize { get; }

        public long PeakVirtualMemorySize { get; }

        public long PeakWorkingSet { get; }

        public long PrivateMemorySize { get; }

        public TimeSpan PrivilegedProcessorTime { get; }

        public string ProcessName { get; }

        public TimeSpan TotalProcessorTime { get; }

        public TimeSpan UserProcessorTime { get; }

        public long VirtualMemorySize { get; }

        public long WorkingSet { get; }
    }
}
