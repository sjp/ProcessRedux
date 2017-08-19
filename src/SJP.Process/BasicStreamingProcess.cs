using System;
using System.IO;
using SysProcess = System.Diagnostics.Process;
using SysStartInfo = System.Diagnostics.ProcessStartInfo;

namespace SJP.Process
{
    public class BasicStreamingProcess : IBasicStreamingProcess, IDisposable
    {
        public BasicStreamingProcess(SysStartInfo startInfo)
        {
            if (startInfo == null)
                throw new ArgumentNullException(nameof(startInfo));

            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            _process.StartInfo = startInfo;
            _process.EnableRaisingEvents = true;
            Exited += (s, e) => _hasExited = true;
        }

        public event EventHandler<byte[]> ErrorDataReceived;
        public event EventHandler<byte[]> OutputDataReceived;

        public event EventHandler<int> Exited
        {
            add
            {
                if (_exitedHandler == null)
                    _process.Exited += OnExitedReceived;
                _exitedHandler += value;
            }
            remove
            {
                _exitedHandler -= value;
            }
        }

        public bool HasExited => _hasExited;

        public bool HasStarted => _hasStarted;

        public IProcessState State
        {
            get
            {
                if (!_hasStarted)
                    throw new ArgumentException("The process has not yet been started. Cannot determine the current state of a non-running process.", nameof(State));

                var adapter = new ProcessAdapter(_process);
                return new ProcessState(adapter);
            }
        }

        public Stream StandardInput
        {
            get
            {
                if (!_hasStarted)
                    throw new ArgumentException("The process has not yet been started. Cannot write standard input to a process that has not been started.", nameof(State));

                return _process.StandardInput.BaseStream;
            }
        }

        public void Kill() => _process.Kill();

        public bool Start()
        {
            if (_hasStarted)
                return false;

            _process.Start();
            _hasStarted = true;

            Action<byte[]> errorHandler = data => ErrorDataReceived?.Invoke(this, data);
            var error = new AsyncStreamReader(_process.StandardError.BaseStream, errorHandler);
            error.BeginRead();

            Action<byte[]> outputHandler = data => OutputDataReceived?.Invoke(this, data);
            var output = new AsyncStreamReader(_process.StandardOutput.BaseStream, outputHandler);
            output.BeginRead();

            return _hasStarted;
        }

        public int WaitForExit()
        {
            _process.WaitForExit();
            return _process.ExitCode;
        }

        public bool WaitForExit(int milliseconds, out int exitCode)
        {
            var hasExited = _process.WaitForExit(milliseconds);
            exitCode = hasExited ? _process.ExitCode : 0;

            return hasExited;
        }

        public (bool exited, int exitCode) WaitForExit(int milliseconds)
        {
            var hasExited = _process.WaitForExit(milliseconds);
            return (hasExited, hasExited ? _process.ExitCode : 0);
        }

        public bool WaitForExit(TimeSpan timeout, out int exitCode)
        {
            var milliseconds = timeout.TotalMilliseconds.Clamp(0, int.MaxValue);
            var intMs = (int)milliseconds;

            return WaitForExit(intMs, out exitCode);
        }

        public (bool exited, int exitCode) WaitForExit(TimeSpan timeout)
        {
            var milliseconds = timeout.TotalMilliseconds.Clamp(0, int.MaxValue);
            var intMs = (int)milliseconds;

            return WaitForExit(intMs);
        }

        protected void OnExitedReceived(object sender, EventArgs args)
        {
            _exitedHandler?.Invoke(this, _process.ExitCode);
        }

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
        private bool _hasExited;
        private bool _hasStarted;

        private EventHandler<int> _exitedHandler;

        private readonly SysProcess _process = new SysProcess();
    }
}
