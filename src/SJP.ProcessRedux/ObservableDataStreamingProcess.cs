using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Represents a process that provides subscriptions to its raw data output.
    /// </summary>
    public class ObservableDataStreamingProcess : IObservableDataStreamingProcess, IProcess
    {
        /// <summary>
        /// Initializes a new <see cref="ObservableDataStreamingProcess"/> instance.
        /// </summary>
        /// <param name="processConfig">Configuration used to determine how to start the process.</param>
        /// <exception cref="ArgumentNullException"><paramref name="processConfig"/> is <c>null</c>.</exception>
        public ObservableDataStreamingProcess(IProcessConfiguration processConfig)
        {
            if (processConfig == null)
                throw new ArgumentNullException(nameof(processConfig));

            _process = new DataStreamingProcess(processConfig);
            Exited += (s, e) => _hasExited = true;
            ErrorData = Observable
                .FromEventPattern<byte[]>(h => _process.ErrorDataReceived += h, h => _process.ErrorDataReceived -= h)
                .Select(x => x.EventArgs);
            OutputData = Observable
                .FromEventPattern<byte[]>(h => _process.OutputDataReceived += h, h => _process.OutputDataReceived -= h)
                .Select(x => x.EventArgs);
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
            remove => _exitedHandler -= value;
        }

        /// <summary>
        /// Provides a subscription for when the application writes to its standard error stream. Provides the data received from the standard error stream.
        /// </summary>
        public IObservable<byte[]> ErrorData { get; }

        /// <summary>
        /// Provides a subscription for when the application writes to its standard output stream. Provides the data received from the standard output stream.
        /// </summary>
        public IObservable<byte[]> OutputData { get; }

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
        public IProcessState State => _process.State;

        /// <summary>
        /// Gets a stream used to write the input of the application. The process must be started for this operation to be valid, see <see cref="HasStarted"/>.
        /// </summary>
        public Stream StandardInput => _process.StandardInput;

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        public void Kill()
        {
            if (_hasExited)
                return;

            _process.Kill();
        }

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        /// <returns>A task representing the asynchronous process kill operation.</returns>
        public Task KillAsync() => _process.KillAsync();

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

            return _hasStarted;
        }

#if NO_SYS_EXCEPTION
        /// <summary>
        /// Instructs the <see cref="ObservableDataStreamingProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>A task that represents the asynchronous wait operation.</returns>
#else
        /// <summary>
        /// Instructs the <see cref="ObservableDataStreamingProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>A task that represents the asynchronous wait operation.</returns>
        /// <exception cref="SystemException">No process information can be obtained. Can also be thrown when the process has not yet started.</exception>
#endif
        public Task<int> WaitForExitAsync() => _process.WaitForExitAsync();

#if NO_SYS_EXCEPTION
        /// <summary>
        /// Instructs the <see cref="ObservableDataStreamingProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>The value that the associated process specified when it terminated.</returns>
#else
        /// <summary>
        /// Instructs the <see cref="ObservableDataStreamingProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>The value that the associated process specified when it terminated.</returns>
        /// <exception cref="SystemException">No process information can be obtained. Can also be thrown when the process has not yet started.</exception>
#endif
        public int WaitForExit() => _process.WaitForExit();

#if NO_SYS_EXCEPTION
        /// <summary>
        /// Instructs the <see cref="ObservableDataStreamingProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <param name="exitCode">The value that the associated process specified when it terminated. Defaults to <c>0</c> when the timeout expired before the process exited.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
#else
        /// <summary>
        /// Instructs the <see cref="ObservableDataStreamingProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <param name="exitCode">The value that the associated process specified when it terminated. Defaults to <c>0</c> when the timeout expired before the process exited.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
        /// <exception cref="SystemException">No process information can be obtained. Can also be thrown when the process has not yet started.</exception>
#endif
        public bool WaitForExit(int milliseconds, out int exitCode) => _process.WaitForExit(milliseconds, out exitCode);

#if NO_SYS_EXCEPTION
        /// <summary>
        /// Instructs the <see cref="ObservableDataStreamingProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <returns>A tuple of length two. The component returns <c>true</c> if the associated process has exited; otherwise, <c>false</c>. The second component is the value that the associated process specified when it terminated, defaults to <c>0</c> when the timeout expired before the process exited.</returns>
#else
        /// <summary>
        /// Instructs the <see cref="ObservableDataStreamingProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <returns>A tuple of length two. The component returns <c>true</c> if the associated process has exited; otherwise, <c>false</c>. The second component is the value that the associated process specified when it terminated, defaults to <c>0</c> when the timeout expired before the process exited.</returns>
        /// <exception cref="SystemException">No process information can be obtained. Can also be thrown when the process has not yet started.</exception>
#endif
        public (bool exited, int exitCode) WaitForExit(int milliseconds) => _process.WaitForExit(milliseconds);

#if NO_SYS_EXCEPTION
        /// <summary>
        /// Instructs the <see cref="ObservableDataStreamingProcess"/> component to wait the specified timespan for the associated process to exit.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the associated process to exit. The maximum is the largest possible value for milliseconds of a 32-bit integer, which represents infinity to the operating system. Any timespan larger than a 32-bit millisecond timeout is assumed to be infinite.</param>
        /// <param name="exitCode">The value that the associated process specified when it terminated. Defaults to <c>0</c> when the timeout expired before the process exited.</param>
#else
        /// <summary>
        /// Instructs the <see cref="ObservableDataStreamingProcess"/> component to wait the specified timespan for the associated process to exit.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the associated process to exit. The maximum is the largest possible value for milliseconds of a 32-bit integer, which represents infinity to the operating system. Any timespan larger than a 32-bit millisecond timeout is assumed to be infinite.</param>
        /// <param name="exitCode">The value that the associated process specified when it terminated. Defaults to <c>0</c> when the timeout expired before the process exited.</param>
        /// <exception cref="SystemException">No process information can be obtained. Can also be thrown when the process has not yet started.</exception>
#endif
        public bool WaitForExit(TimeSpan timeout, out int exitCode) => _process.WaitForExit(timeout, out exitCode);

#if NO_SYS_EXCEPTION
        /// <summary>
        /// Instructs the <see cref="ObservableDataStreamingProcess"/> component to wait the specified timespan for the associated process to exit.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the associated process to exit. The maximum is the largest possible value for milliseconds of a 32-bit integer, which represents infinity to the operating system. Any timespan larger than a 32-bit millisecond timeout is assumed to be infinite.</param>
        /// <returns>A tuple of length two. The component returns <c>true</c> if the associated process has exited; otherwise, <c>false</c>. The second component is the value that the associated process specified when it terminated, defaults to <c>0</c> when the timeout expired before the process exited.</returns>
#else
        /// <summary>
        /// Instructs the <see cref="ObservableDataStreamingProcess"/> component to wait the specified timespan for the associated process to exit.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the associated process to exit. The maximum is the largest possible value for milliseconds of a 32-bit integer, which represents infinity to the operating system. Any timespan larger than a 32-bit millisecond timeout is assumed to be infinite.</param>
        /// <returns>A tuple of length two. The component returns <c>true</c> if the associated process has exited; otherwise, <c>false</c>. The second component is the value that the associated process specified when it terminated, defaults to <c>0</c> when the timeout expired before the process exited.</returns>
        /// <exception cref="SystemException">No process information can be obtained. Can also be thrown when the process has not yet started.</exception>
#endif
        public (bool exited, int exitCode) WaitForExit(TimeSpan timeout) => _process.WaitForExit(timeout);

        private void OnExitedReceived(object sender, int exitCode) => OnExitedReceived(exitCode);

        /// <summary>
        /// Raises the <see cref="Exited"/> event.
        /// </summary>
        /// <param name="exitCode">The exit code of the process when it exited.</param>
        protected virtual void OnExitedReceived(int exitCode)
        {
            _exitedHandler?.Invoke(this, exitCode);
        }

        /// <summary>
        /// Releases resources used by the current <see cref="ObservableDataStreamingProcess"/> instance.
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Releases resources used by the current <see cref="ObservableDataStreamingProcess"/> instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources are to be disposed. <c>false</c> will not dispose any resources.</param>
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
        private bool _hasStarted;
        private bool _hasExited;
        private EventHandler<int> _exitedHandler;

        private readonly DataStreamingProcess _process;
    }
}
