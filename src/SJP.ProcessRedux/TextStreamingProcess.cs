using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SJP.ProcessRedux
{
    /// <summary>
    /// Represents a process that streams textual output.
    /// </summary>
    public class TextStreamingProcess : ITextStreamingProcess, ITextStreamingProcessAsync
    {
        /// <summary>
        /// Initializes a new <see cref="TextStreamingProcess"/> instance.
        /// </summary>
        /// <param name="processConfig">Configuration used to determine how to start the process.</param>
        /// <param name="errorEncoding">The encoding to use when processing textual error output. The default, <c>null</c>, indicates that the default encoding should be used.</param>
        /// <param name="outputEncoding">The encoding to use when processing textual standard output. The default, <c>null</c>, indicates that the default encoding should be used.</param>
        /// <exception cref="ArgumentNullException"><paramref name="processConfig"/> is <c>null</c>.</exception>
        public TextStreamingProcess(IProcessConfiguration processConfig, Encoding errorEncoding = null, Encoding outputEncoding = null)
        {
            if (processConfig == null)
                throw new ArgumentNullException(nameof(processConfig));

            var startInfo = processConfig.ToStartInfo();
            startInfo.StandardErrorEncoding = errorEncoding;
            startInfo.StandardOutputEncoding = outputEncoding;

            var process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };
            _process = new FrameworkProcessAdapter(process);

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
            remove => _exitedHandler -= value;
        }

        /// <summary>
        ///  Occurs each time an application writes a line to its standard error stream. Provides the line received from the standard error stream.
        /// </summary>
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

        /// <summary>
        /// Occurs each time an application writes a line to its standard output stream. Provides the line received from the standard output stream.
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether the associated process has been terminated.
        /// </summary>
        public bool HasExited => _hasExited;

        /// <summary>
        /// Gets a value indicating whether the associated process has started.
        /// </summary>
        public bool HasStarted => _hasStarted;

        /// <summary>
        /// Retrieves the current state of the process. The process must be running for this operation to be valid, see <see cref="HasStarted"/> and <see cref="HasExited"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The process has not started, or has already exited.</exception>
        public IProcessState State
        {
            get
            {
                if (!_hasStarted)
                    throw new InvalidOperationException("The process has not yet been started. Cannot determine the current state of a non-running process.");
                if (_hasExited)
                    throw new InvalidOperationException("The process has exited. Cannot determine the current state of a non-running process.");

                try
                {
                    return new ProcessState(_process);
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("The process has exited. Cannot determine the current state of a non-running process.");
                }
            }
        }

        /// <summary>
        /// Gets a stream used to write the input of the application. The process must be running for this operation to be valid, see <see cref="HasStarted"/> and <see cref="HasExited"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The process has not started, or has already exited.</exception>
        public Stream StandardInput
        {
            get
            {
                if (!_hasStarted)
                    throw new InvalidOperationException($"The process has not yet been started. Cannot write standard input to a process that has not been started. Use the { nameof(HasStarted) } property to find out whether a process has been started.");
                if (_hasExited)
                    throw new InvalidOperationException($"The process has exited. Cannot write standard input to a process that has exited. Use the { nameof(HasExited) } property to find out whether a process has exited.");

                return _process.StandardInput.BaseStream;
            }
        }

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        public void Kill()
        {
            if (_hasExited)
                return;

            _process.Kill();
            _hasExited = true;
        }

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        /// <returns>A task representing the asynchronous process kill operation.</returns>
        public Task KillAsync() => Task.Run(action: Kill);

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

            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();

            return _hasStarted;
        }

#if NO_SYS_EXCEPTION
                /// <summary>
        /// Instructs the <see cref="TextStreamingProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>The value that the associated process specified when it terminated.</returns>
#else
        /// <summary>
        /// Instructs the <see cref="TextStreamingProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>The value that the associated process specified when it terminated.</returns>
        /// <exception cref="SystemException">No process information can be obtained. Can also be thrown when the process has not yet started.</exception>
#endif
        public int WaitForExit()
        {
            _process.WaitForExit();
            return _process.ExitCode;
        }

#if NO_SYS_EXCEPTION
                /// <summary>
        /// Instructs the <see cref="TextStreamingProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>A task that represents the asynchronous wait operation.</returns>
#else
        /// <summary>
        /// Instructs the <see cref="TextStreamingProcess"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <returns>A task that represents the asynchronous wait operation.</returns>
        /// <exception cref="SystemException">No process information can be obtained. Can also be thrown when the process has not yet started.</exception>
#endif
        public Task<int> WaitForExitAsync() => Task.Run(() => WaitForExit());

#if NO_SYS_EXCEPTION
                /// <summary>
        /// Instructs the <see cref="TextStreamingProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <param name="exitCode">The value that the associated process specified when it terminated. Defaults to <c>0</c> when the timeout expired before the process exited.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
#else
        /// <summary>
        /// Instructs the <see cref="TextStreamingProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <param name="exitCode">The value that the associated process specified when it terminated. Defaults to <c>0</c> when the timeout expired before the process exited.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
        /// <exception cref="SystemException">No process information can be obtained. Can also be thrown when the process has not yet started.</exception>
#endif
        public bool WaitForExit(int milliseconds, out int exitCode)
        {
            var hasExited = _process.WaitForExit(milliseconds);
            exitCode = hasExited ? _process.ExitCode : 0;

            return hasExited;
        }

#if NO_SYS_EXCEPTION
                /// <summary>
        /// Instructs the <see cref="TextStreamingProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <returns>A tuple of length two. The component returns <c>true</c> if the associated process has exited; otherwise, <c>false</c>. The second component is the value that the associated process specified when it terminated, defaults to <c>0</c> when the timeout expired before the process exited.</returns>
#else
        /// <summary>
        /// Instructs the <see cref="TextStreamingProcess"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <returns>A tuple of length two. The component returns <c>true</c> if the associated process has exited; otherwise, <c>false</c>. The second component is the value that the associated process specified when it terminated, defaults to <c>0</c> when the timeout expired before the process exited.</returns>
        /// <exception cref="SystemException">No process information can be obtained. Can also be thrown when the process has not yet started.</exception>
#endif
        public (bool exited, int exitCode) WaitForExit(int milliseconds)
        {
            var hasExited = _process.WaitForExit(milliseconds);
            return (hasExited, hasExited ? _process.ExitCode : 0);
        }

#if NO_SYS_EXCEPTION
                /// <summary>
        /// Instructs the <see cref="TextStreamingProcess"/> component to wait the specified timespan for the associated process to exit.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the associated process to exit. The maximum is the largest possible value for milliseconds of a 32-bit integer, which represents infinity to the operating system. Any timespan larger than a 32-bit millisecond timeout is assumed to be infinite.</param>
        /// <param name="exitCode">The value that the associated process specified when it terminated. Defaults to <c>0</c> when the timeout expired before the process exited.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
#else
        /// <summary>
        /// Instructs the <see cref="TextStreamingProcess"/> component to wait the specified timespan for the associated process to exit.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the associated process to exit. The maximum is the largest possible value for milliseconds of a 32-bit integer, which represents infinity to the operating system. Any timespan larger than a 32-bit millisecond timeout is assumed to be infinite.</param>
        /// <param name="exitCode">The value that the associated process specified when it terminated. Defaults to <c>0</c> when the timeout expired before the process exited.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
        /// <exception cref="SystemException">No process information can be obtained. Can also be thrown when the process has not yet started.</exception>
#endif
        public bool WaitForExit(TimeSpan timeout, out int exitCode)
        {
            var milliseconds = timeout.TotalMilliseconds.Clamp(0, int.MaxValue);
            var intMs = (int)milliseconds;

            return WaitForExit(intMs, out exitCode);
        }

#if NO_SYS_EXCEPTION
        /// <summary>
        /// Instructs the <see cref="TextStreamingProcess"/> component to wait the specified timespan for the associated process to exit.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the associated process to exit. The maximum is the largest possible value for milliseconds of a 32-bit integer, which represents infinity to the operating system. Any timespan larger than a 32-bit millisecond timeout is assumed to be infinite.</param>
        /// <returns>A tuple of length two. The component returns <c>true</c> if the associated process has exited; otherwise, <c>false</c>. The second component is the value that the associated process specified when it terminated, defaults to <c>0</c> when the timeout expired before the process exited.</returns>
#else
        /// <summary>
        /// Instructs the <see cref="TextStreamingProcess"/> component to wait the specified timespan for the associated process to exit.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the associated process to exit. The maximum is the largest possible value for milliseconds of a 32-bit integer, which represents infinity to the operating system. Any timespan larger than a 32-bit millisecond timeout is assumed to be infinite.</param>
        /// <returns>A tuple of length two. The component returns <c>true</c> if the associated process has exited; otherwise, <c>false</c>. The second component is the value that the associated process specified when it terminated, defaults to <c>0</c> when the timeout expired before the process exited.</returns>
        /// <exception cref="SystemException">No process information can be obtained. Can also be thrown when the process has not yet started.</exception>
#endif
        public (bool exited, int exitCode) WaitForExit(TimeSpan timeout)
        {
            var milliseconds = timeout.TotalMilliseconds.Clamp(0, int.MaxValue);
            var intMs = (int)milliseconds;

            return WaitForExit(intMs);
        }

        private void OnExitedReceived(object sender, EventArgs args) => OnExitedReceived(_process.ExitCode);

        private void OnErrorReceived(object sender, DataReceivedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            OnErrorReceived(args.Data);
        }

        private void OnOutputReceived(object sender, DataReceivedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            OnOutputReceived(args.Data);
        }

        /// <summary>
        /// Raises the <see cref="Exited"/> event.
        /// </summary>
        /// <param name="exitCode">The exit code of the process when it exited.</param>
        protected virtual void OnExitedReceived(int exitCode)
        {
            _exitedHandler?.Invoke(this, exitCode);
        }

        /// <summary>
        /// Raises the <see cref="ErrorLineReceived"/> event.
        /// </summary>
        /// <param name="line">A line of output from standard error.</param>
        protected virtual void OnErrorReceived(string line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));

            _errorHandler?.Invoke(this, line);
        }

        /// <summary>
        /// Raises the <see cref="OutputLineReceived"/> event.
        /// </summary>
        /// <param name="line">A line of output from standard output.</param>
        protected virtual void OnOutputReceived(string line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));

            _outputHandler?.Invoke(this, line);
        }

        /// <summary>
        /// Releases resources used by the current <see cref="TextStreamingProcess"/> instance.
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Releases resources used by the current <see cref="TextStreamingProcess"/> instance.
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
        private bool _hasExited;
        private bool _hasStarted;

        private EventHandler<int> _exitedHandler;
        private EventHandler<string> _errorHandler;
        private EventHandler<string> _outputHandler;

        private readonly IFrameworkProcess _process;
    }
}
