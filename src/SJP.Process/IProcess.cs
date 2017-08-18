using System;
using System.Threading.Tasks;
using SysProcess = System.Diagnostics.Process;
using SysStartInfo = System.Diagnostics.ProcessStartInfo;

namespace SJP.Process
{
    public interface IProcess
    {
        /// <summary>
        /// Gets the base priority of the associated process.
        /// </summary>
        int BasePriority { get; }

        /// <summary>
        /// Gets or sets whether the <see cref="Exited"/> event should be raised when the process terminates.
        /// </summary>
        bool EnableRaisingEvents { get; }

        /// <summary>
        /// Occurs when a process exits.
        /// </summary>
        event EventHandler Exited;
    }
}
