using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SJP.ProcessRedux
{
    public class StringStreamingProcess : IStringStreamingProcess, IStringStreamingProcessAsync
    {
        public StringStreamingProcess(IProcessConfiguration processConfig, Encoding errorEncoding = null, Encoding outputEncoding = null)
        {
            if (processConfig == null)
                throw new ArgumentNullException(nameof(processConfig));

            var startInfo = processConfig.ToStartInfo();
            startInfo.StandardErrorEncoding = errorEncoding;
            startInfo.StandardOutputEncoding = outputEncoding;

            _process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            Exited += (s, e) => _hasExited = true;
        }

        public event EventHandler<int> Exited
        {
            add
            {
                if (_exitedHandler == null)
                    _process.Exited += OnExitedReceived;
                _exitedHandler += value;
            }
            remove => _exitedHandler -= value;
        }

        public event EventHandler<string> ErrorLineReceived
        {
            add
            {
                if (_errorHandler == null)
                    _process.ErrorDataReceived += OnErrorReceived;
                _errorHandler += value;
            }
            remove
            {
                _errorHandler -= value;
                if (_errorHandler == null)
                    _process.ErrorDataReceived -= OnErrorReceived;
            }
        }

        public event EventHandler<string> OutputLineReceived
        {
            add
            {
                if (_outputHandler == null)
                    _process.OutputDataReceived += OnOutputReceived;
                _outputHandler += value;
            }
            remove
            {
                _outputHandler -= value;
                if (_outputHandler == null)
                    _process.OutputDataReceived -= OnOutputReceived;
            }
        }

        public bool HasExited => _hasExited;

        public bool HasStarted => _hasStarted;

        public IProcessState State
        {
            get
            {
                if (!_hasStarted)
                    throw new ArgumentException($"The process has not yet been started. Cannot determine the current state of a non-running process. Use the { nameof(HasStarted) } property to find out whether a process has been started.", nameof(State));

                var adapter = new ProcessAdapter(_process);
                return new ProcessState(adapter);
            }
        }

        public Stream StandardInput
        {
            get
            {
                if (!_hasStarted)
                    throw new ArgumentException($"The process has not yet been started. Cannot write standard input to a process that has not been started. Use the { nameof(HasStarted) } property to find out whether a process has been started.", nameof(State));

                return _process.StandardInput.BaseStream;
            }
        }

        public void Kill() => _process.Kill();

        public Task KillAsync() => Task.Run(action: _process.Kill);

        public bool Start()
        {
            if (_hasStarted)
                return false;

            _process.Start();
            _hasStarted = true;

            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();

            return _hasStarted;
        }

        public int WaitForExit()
        {
            _process.WaitForExit();
            return _process.ExitCode;
        }

        public Task<int> WaitForExitAsync() => Task.Run(() => WaitForExit());

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

        protected void OnErrorReceived(object sender, DataReceivedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            _errorHandler?.Invoke(this, args.Data);
        }

        protected void OnOutputReceived(object sender, DataReceivedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            _outputHandler?.Invoke(this, args.Data);
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
        private EventHandler<string> _errorHandler;
        private EventHandler<string> _outputHandler;

        private readonly Process _process;
    }
}
