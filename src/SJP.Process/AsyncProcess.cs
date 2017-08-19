using System;
using System.Threading.Tasks;
using SysProcess = System.Diagnostics.Process;
using SysStartInfo = System.Diagnostics.ProcessStartInfo;

namespace SJP.Process
{
    public class AsyncProcess
    {
        public AsyncProcess()
        {
            // TODO: add another ctor?
        }

        public event EventHandler<byte[]> OnStandardOutputReceived;

        public event EventHandler<byte[]> OnStandardErrorReceived;

        public event EventHandler<string> OnStandardOutputLineReceived;

        public event EventHandler<string> OnStandardErrorLineReceived;

        public void Start(SysStartInfo startInfo)
        {
            _process.StartInfo = startInfo ?? throw new ArgumentNullException(nameof(startInfo));
            _process.EnableRaisingEvents = true;

            if (OnStandardOutputReceived != null && OnStandardOutputLineReceived != null)
                throw new InvalidOperationException($"Only one of the events { nameof(OnStandardOutputReceived) } and { nameof(OnStandardOutputLineReceived) } may be set");
            if (OnStandardErrorReceived != null && OnStandardErrorLineReceived != null)
                throw new InvalidOperationException($"Only one of the events { nameof(OnStandardOutputReceived) } and { nameof(OnStandardOutputLineReceived) } may be set");

            if (OnStandardOutputReceived != null)
            {
                _outputHandler = (data) => OnStandardOutputReceived?.Invoke(this, data);
                _error = new AsyncStreamReader(_process.StandardOutput.BaseStream, _outputHandler);
            }
            if (OnStandardOutputLineReceived != null)
                _process.OutputDataReceived += (s, e) => OnStandardOutputLineReceived?.Invoke(this, e.Data);

            if (OnStandardErrorReceived != null)
            {
                _errorHandler = (data) => OnStandardErrorReceived?.Invoke(this, data);
                _error = new AsyncStreamReader(_process.StandardError.BaseStream, _errorHandler);
            }
            if (OnStandardOutputLineReceived != null)
                _process.ErrorDataReceived += (s, e) => OnStandardErrorLineReceived?.Invoke(this, e.Data);

            _process.Start();
            _process.WaitForExit();
        }

        public Task<int> StartAsync(SysStartInfo startInfo)
        {
            _process.StartInfo = startInfo ?? throw new ArgumentNullException(nameof(startInfo));
            _process.EnableRaisingEvents = true;

            if (OnStandardOutputReceived != null && OnStandardOutputLineReceived != null)
                throw new InvalidOperationException($"Only one of the events { nameof(OnStandardOutputReceived) } and { nameof(OnStandardOutputLineReceived) } may be set");
            if (OnStandardErrorReceived != null && OnStandardErrorLineReceived != null)
                throw new InvalidOperationException($"Only one of the events { nameof(OnStandardOutputReceived) } and { nameof(OnStandardOutputLineReceived) } may be set");

            var tcs = new TaskCompletionSource<int>();
            _process.Exited += (s, e) =>
            {
                tcs.SetResult(_process.ExitCode);
                _process.Dispose();
            };

            _process.Start();

            if (OnStandardOutputReceived != null)
            {
                _outputHandler = (data) => OnStandardOutputReceived?.Invoke(this, data);
                _output = new AsyncStreamReader(_process.StandardOutput.BaseStream, _outputHandler);
                _output.BeginRead();
            }
            if (OnStandardOutputLineReceived != null)
                _process.OutputDataReceived += (s, e) => OnStandardOutputLineReceived?.Invoke(this, e.Data);

            if (OnStandardErrorReceived != null)
            {
                _errorHandler = (data) => OnStandardErrorReceived?.Invoke(this, data);
                _error = new AsyncStreamReader(_process.StandardError.BaseStream, _errorHandler);
                _error.BeginRead();
            }
            if (OnStandardOutputLineReceived != null)
                _process.ErrorDataReceived += (s, e) => OnStandardErrorLineReceived?.Invoke(this, e.Data);

            return tcs.Task;
        }

        // don't need the following as the underlying process is disposed by the exited event
        // for async
        /*
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _process.Dispose();

            _disposed = true;
        }

        private bool _disposed;
        */

        private Action<byte[]> _outputHandler;
        private Action<byte[]> _errorHandler;

        private AsyncStreamReader _output;
        private AsyncStreamReader _error;

        private readonly SysProcess _process = new SysProcess();
    }
}
