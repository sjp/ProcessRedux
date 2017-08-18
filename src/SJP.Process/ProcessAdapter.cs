using System;
using System.Threading.Tasks;
using SysProcess = System.Diagnostics.Process;
using SysStartInfo = System.Diagnostics.ProcessStartInfo;

namespace SJP.Process
{
    public class ProcessAdapter : IProcess
    {
        public ProcessAdapter(SysProcess process)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process));
        }

        public static implicit operator ProcessAdapter(SysProcess process) => new ProcessAdapter(process);

        public event EventHandler Exited;

        public int BasePriority => _process.BasePriority;

        public bool EnableRaisingEvents => _process.EnableRaisingEvents;

        private readonly SysProcess _process;
    }
}
