using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace SJP.ProcessRedux
{
    public class ObservableStringStreamingProcess : IObservableStringStreamingProcess, IProcess
    {
        public ObservableStringStreamingProcess(IProcessConfiguration processConfig)
        {
            if (processConfig == null)
                throw new ArgumentNullException(nameof(processConfig));

            _process = new StringStreamingProcess(processConfig);
            ErrorLines = Observable
                .FromEventPattern<string>(h => _process.ErrorLineReceived += h, h => _process.ErrorLineReceived -= h)
                .Select(x => x.EventArgs);
            OutputLines = Observable
                .FromEventPattern<string>(h => _process.OutputLineReceived += h, h => _process.OutputLineReceived -= h)
                .Select(x => x.EventArgs);
        }

        public event EventHandler<int> Exited
        {
            add => _process.Exited += value;
            remove => _process.Exited -= value;
        }

        public IObservable<string> ErrorLines { get; }

        public IObservable<string> OutputLines { get; }

        public IProcessState State => _process.State;

        public Stream StandardInput => _process.StandardInput;

        public void Kill() => _process.Kill();

        public Task KillAsync() => _process.KillAsync();

        public bool Start() => _process.Start();

        public int WaitForExit() => _process.WaitForExit();

        public Task<int> WaitForExitAsync() => _process.WaitForExitAsync();

        public bool WaitForExit(int milliseconds, out int exitCode) => _process.WaitForExit(milliseconds, out exitCode);

        public (bool exited, int exitCode) WaitForExit(int milliseconds) => _process.WaitForExit(milliseconds);

        public bool WaitForExit(TimeSpan timeout, out int exitCode) => _process.WaitForExit(timeout, out exitCode);

        public (bool exited, int exitCode) WaitForExit(TimeSpan timeout) => _process.WaitForExit(timeout);

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (!disposing)
                return;

            _process.Dispose();
            _disposed = true;
        }

        private bool _disposed;
        private readonly StringStreamingProcess _process;
    }
}
