using System.Buffers.Text;
using System.Text;
using Quick.Protocol;

namespace Glash.Core
{
    public class GlashTunnelContext : IDisposable
    {
        private CancellationTokenSource cts;
        private byte[] readBuffer = new byte[4 * 1024];
        private byte[] writeBuffer = new byte[8 * 1024];
        private QpChannel channel;
        private TunnelInfo tunnelInfo;
        private Stream stream;
        private Action<Exception> errorHandler;

        public GlashTunnelContext(QpChannel channel, TunnelInfo tunnelInfo, Stream stream, Action<Exception> errorHandler)
        {
            this.channel = channel;
            this.tunnelInfo = tunnelInfo;
            this.stream = stream;
            this.errorHandler = errorHandler;
        }

        private async Task beginRead(CancellationToken token)
        {
            try
            {
                var task = stream?.ReadAsync(readBuffer, 0, readBuffer.Length, token);
                if (task == null)
                    return;
                var ret = await task;
                if (ret <= 0)
                    throw new IOException("Read count: " + ret);
                await channel.SendNoticePackage(new G.D()
                {
                    TunnelId = tunnelInfo.Id,
                    Data = Convert.ToBase64String(readBuffer, 0, ret)
                });
                _ = beginRead(token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        public void PushData(string data)
        {
            var strBytesLength = Encoding.UTF8.GetByteCount(data);
            if (strBytesLength > writeBuffer.Length)
                writeBuffer = new byte[strBytesLength];
            strBytesLength = Encoding.UTF8.GetBytes(data, writeBuffer);
            var ret = Base64.DecodeFromUtf8InPlace(writeBuffer.AsSpan(0, strBytesLength), out var dataBytesLength);
            if (ret != System.Buffers.OperationStatus.Done)
                throw new IOException($"Error when convert base64 string to byte array,reason: {ret}");
            try
            {
                stream?.Write(writeBuffer, 0, dataBytesLength);
                stream?.Flush();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        private void OnError(Exception ex)
        {
            errorHandler?.Invoke(ex);
        }

        public void Start()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = new CancellationTokenSource();
            _ = beginRead(cts.Token);
        }

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            try
            {
                stream?.Dispose();
                stream = null;
            }
            catch { }
        }
    }
}
