using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SJP.Process
{
    internal sealed class AsyncStreamReader
    {
        // Creates a new AsyncStreamReader for the given stream. The
        // character encoding is set by encoding and the buffer size,
        // in number of 16-bit characters, is set by bufferSize.
        public AsyncStreamReader(Stream stream, Action<byte[]> callback)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("Stream must be readable.", nameof(stream));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _stream = stream;
            _callBack = callback;
            _byteBuffer = new byte[DefaultBufferSize];
            _cts = new CancellationTokenSource();
        }

        // User calls BeginRead to start the asynchronous read
        public void BeginRead() => _readToBufferTask = Task.Run(ReadBufferAsync);

        public void CancelOperation() => _cts.Cancel();

        // This is the async callback function. Only one thread could/should call this.
        private async Task ReadBufferAsync()
        {
            while (true)
            {
                try
                {
                    var bytesRead = await _stream.ReadAsync(_byteBuffer, 0, _byteBuffer.Length, _cts.Token).ConfigureAwait(false);
                    if (bytesRead == 0)
                        break;

                    var result = new byte[bytesRead];
                    Array.Copy(_byteBuffer, result, bytesRead);
                    _callBack?.Invoke(result);
                }
                catch (IOException)
                {
                    // We should ideally consume errors from operations getting cancelled
                    // so that we don't crash the unsuspecting parent with an unhandled exc.
                    // This seems to come in 2 forms of exceptions (depending on platform and scenario),
                    // namely OperationCanceledException and IOException (for errorcode that we don't
                    // map explicitly).
                    break; // Treat this as EOF
                }
                catch (OperationCanceledException)
                {
                    // We should consume any OperationCanceledException from child read here
                    // so that we don't crash the parent with an unhandled exc
                    break; // Treat this as EOF
                }
            }
        }

        private readonly CancellationTokenSource _cts;
        private Task _readToBufferTask;

        private readonly Action<byte[]> _callBack;

        private readonly Stream _stream;
        private readonly byte[] _byteBuffer;

        private const int DefaultBufferSize = 1024;
    }
}
