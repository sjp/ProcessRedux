using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Represents a process that streams raw data output.
    /// </summary>
    public class DataStreamingProcess : IDataStreamingProcess, IDataStreamingProcessAsync
    {
        /// <summary>
        /// Initializes a new <see cref="DataStreamingProcess"/> instance.
        /// </summary>
        /// <param name="processConfig">Configuration used to determine how to start the process.</param>
        /// <exception cref="ArgumentNullException"><paramref name="processConfig"/> is <c>null</c>.</exception>
        public DataStreamingProcess(IProcessConfiguration processConfig)
        {
            if (processConfig == null)
                throw new ArgumentNullException(nameof(processConfig));

            _process.StartInfo = processConfig.ToStartInfo();
            _process.EnableRaisingEvents = true;
            Exited += (s, e) => _hasExited = true;
        }

        /// <summary>
        /// Occurs when the process exits. Returns the exit code of the process when this occurs.
        /// </summary>
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

        /// <summary>
        /// Occurs when an application writes to its standard error stream. Provides the data received from the standard error stream.
        /// </summary>
        public event EventHandler<byte[]> ErrorDataReceived;

        /// <summary>
        /// Occurs when an application writes to its standard output stream. Provides the data received from the standard output stream.
        /// </summary>
        public event EventHandler<byte[]> OutputDataReceived;

        /// <summary>
        /// Gets a value indicating whether the associated process has been terminated.
        /// </summary>
        public bool HasExited => _hasExited;

        /// <summary>
        /// Gets a value indicating whether the associated process has started.
        /// </summary>
        public bool HasStarted => _hasStarted;

        /// <summary>
        /// Retrieves the current state of the process. The process must be started for this operation to be valid, see <see cref="HasStarted"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The process has not started.</exception>
        public IProcessState State
        {
            get
            {
                if (!_hasStarted)
                    throw new InvalidOperationException("The process has not yet been started. Cannot determine the current state of a non-running process.");

                return new ProcessState(_process);
            }
        }

        /// <summary>
        /// Gets a stream used to write the input of the application. The process must be started for this operation to be valid, see <see cref="HasStarted"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The process has not started.</exception>
        public Stream StandardInput
        {
            get
            {
                if (!_hasStarted)
                    throw new InvalidOperationException("The process has not yet been started. Cannot write standard input to a process that has not been started.");

                return _process.StandardInput.BaseStream;
            }
        }

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        public void Kill() => _process.Kill();

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        /// <returns>A task representing the asynchronous process kill operation.</returns>
        public Task KillAsync() => Task.Run(action: _process.Kill);

        /// <summary>
        /// Starts the process resource that is described by this component.
        /// </summary>
        /// <returns><c>true</c> if a process resource is started; <c>false</c> otherwise.</returns>
        public bool Start()
        {
            if (_hasStarted)
                return false;

            _process.Start();
            _hasStarted = true;

            if (ErrorDataReceived != null)
            {
                Action<byte[]> errorHandler = data => ErrorDataReceived?.Invoke(this, data);
                _errorReader = new AsyncStreamReader(_process.StandardError.BaseStream, errorHandler);
                _errorReader.BeginRead();
            }

            if (OutputDataReceived != null)
            {
                Action<byte[]> outputHandler = data => OutputDataReceived?.Invoke(this, data);
                _outputReader = new AsyncStreamReader(_process.StandardOutput.BaseStream, outputHandler);
                _outputReader.BeginRead();
            }

            return _hasStarted;
        }

        /// <summary>
        /// Instructs the <see cref="DataStreamingProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>The value that the associated process specified when it terminated.</returns>
        public int WaitForExit()
        {
            _process.WaitForExit();
            return _process.ExitCode;
        }

        /// <summary>
        /// Instructs the <see cref="DataStreamingProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>A task that represents the asynchronous wait operation.</returns>
        public Task<int> WaitForExitAsync() => Task.Run(() => WaitForExit());

        /// <summary>
        /// Instructs the <see cref="DataStreamingProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <param name="exitCode">The value that the associated process specified when it terminated. Defaults to <c>0</c> when the timeout expired before the process exited.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
        public bool WaitForExit(int milliseconds, out int exitCode)
        {
            var hasExited = _process.WaitForExit(milliseconds);
            exitCode = hasExited ? _process.ExitCode : 0;

            return hasExited;
        }

        /// <summary>
        /// Instructs the <see cref="DataStreamingProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <returns>A tuple of length two. The component returns <c>true</c> if the associated process has exited; otherwise, <c>false</c>. The second component is the value that the associated process specified when it terminated, defaults to <c>0</c> when the timeout expired before the process exited.</returns>
        public (bool exited, int exitCode) WaitForExit(int milliseconds)
        {
            var hasExited = _process.WaitForExit(milliseconds);
            return (hasExited, hasExited ? _process.ExitCode : 0);
        }

        /// <summary>
        /// Instructs the <see cref="DataStreamingProcess"/> component to wait the specified timespan for the associated process to exit.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the associated process to exit. The maximum is the largest possible value for milliseconds of a 32-bit integer, which represents infinity to the operating system. Any timespan larger than a 32-bit millisecond timeout is assumed to be infinite.</param>
        /// <param name="exitCode">The value that the associated process specified when it terminated. Defaults to <c>0</c> when the timeout expired before the process exited.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
        public bool WaitForExit(TimeSpan timeout, out int exitCode)
        {
            var milliseconds = timeout.TotalMilliseconds.Clamp(0, int.MaxValue);
            var intMs = (int)milliseconds;

            return WaitForExit(intMs, out exitCode);
        }

        /// <summary>
        /// Instructs the <see cref="DataStreamingProcess"/> component to wait the specified timespan for the associated process to exit.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the associated process to exit. The maximum is the largest possible value for milliseconds of a 32-bit integer, which represents infinity to the operating system. Any timespan larger than a 32-bit millisecond timeout is assumed to be infinite.</param>
        /// <returns>A tuple of length two. The component returns <c>true</c> if the associated process has exited; otherwise, <c>false</c>. The second component is the value that the associated process specified when it terminated, defaults to <c>0</c> when the timeout expired before the process exited.</returns>
        public (bool exited, int exitCode) WaitForExit(TimeSpan timeout)
        {
            var milliseconds = timeout.TotalMilliseconds.Clamp(0, int.MaxValue);
            var intMs = (int)milliseconds;

            return WaitForExit(intMs);
        }

        private void OnExitedReceived(object sender, EventArgs args) => OnExitedReceived(_process.ExitCode);

        /// <summary>
        /// Raises the <see cref="Exited"/> event.
        /// </summary>
        /// <param name="exitCode">The exit code of the process when it exited.</param>
        protected virtual void OnExitedReceived(int exitCode)
        {
            _exitedHandler?.Invoke(this, exitCode);
        }

        /// <summary>
        /// Releases resources used by the current <see cref="DataStreamingProcess"/> instance.
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Releases resources used by the current <see cref="DataStreamingProcess"/> instance.
        /// </summary>
        /// <param name="disposing"><b>True</b> if managed resources are to be disposed. <b>False</b> will not dispose any resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (!disposing)
                return;

            _errorReader?.CancelOperation();
            _outputReader?.CancelOperation();
            _process.Dispose();
            _disposed = true;
        }

        private bool _disposed;
        private bool _hasExited;
        private bool _hasStarted;

        private EventHandler<int> _exitedHandler;

        private AsyncStreamReader _errorReader;
        private AsyncStreamReader _outputReader;

        private readonly IFrameworkProcess _process = new FrameworkProcessAdapter(new Process());
    }
}
