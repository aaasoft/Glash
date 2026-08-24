using System.Buffers;
using System.Buffers.Text;
using System.Text;
using Glash.Core;
using Quick.Protocol;

namespace Glash.Server
{
    public class GlashServerTunnelContext : IDisposable
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        public TunnelInfo TunnelInfo { get; private set; }
        public GlashClientContext Client { get; private set; }
        public GlashAgentContext Agent { get; private set; }
        public DateTime CreateTime { get; private set; }
        public long UploadBytes { get; private set; }
        public long DownloadBytes { get; private set; }
        public long UploadBytesPerSecond { get; private set; }
        public long DownloadBytesPerSecond { get; private set; }
        private long preUploadBytes, preDownloadBytes;
        private Action<Exception> errorHandler;

        public GlashServerTunnelContext(
            TunnelInfo tunnelInfo,
            GlashClientContext client,
            GlashAgentContext agent,
            Action<Exception> errorHandler)
        {
            TunnelInfo = tunnelInfo;
            Client = client;
            Agent = agent;
            this.errorHandler = errorHandler;
            CreateTime = DateTime.Now;
            cts = new CancellationTokenSource();
            beginCalcSpeed(cts.Token);

            if (tunnelInfo.ClientTunnelPackageType > 0)
                client.Channel.RegisterPackageHandler(tunnelInfo.ClientTunnelPackageType, ClientTunnelPackageHandler);
            if (tunnelInfo.AgentTunnelPackageType > 0)
                agent.Channel.RegisterPackageHandler(tunnelInfo.AgentTunnelPackageType, AgentTunnelPackageHandler);
        }

        private void beginCalcSpeed(CancellationToken cancellationToken)
        {
            Task.Delay(1000, cancellationToken).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;
                try
                {
                    var currentUploadBytes = UploadBytes;
                    var currentDownloadBytes = DownloadBytes;

                    UploadBytesPerSecond = currentUploadBytes - preUploadBytes;
                    if (UploadBytesPerSecond < 0)
                        UploadBytesPerSecond = 0;
                    preUploadBytes = currentUploadBytes;

                    DownloadBytesPerSecond = currentDownloadBytes - preDownloadBytes;
                    if (DownloadBytesPerSecond < 0)
                        DownloadBytesPerSecond = 0;
                    preDownloadBytes = currentDownloadBytes;
                }
                catch { }
                beginCalcSpeed(cancellationToken);
            });
        }

        public void OnError(Exception ex)
        {
            errorHandler?.Invoke(ex);
        }

        private async ValueTask ClientTunnelPackageHandler(QpChannel channel, byte packageType, ReadOnlySequence<byte> bodyBuffer)
        {
            if (TunnelInfo.AgentTunnelPackageType > 0)
            {
                await _PushDataToAgent(bodyBuffer);
            }
            else
            {
                var ret = Convert.ToInt32(bodyBuffer.Length);
                var buffer = ArrayPool<byte>.Shared.Rent(ret);
                bodyBuffer.CopyTo(buffer.AsSpan(0, ret));
                var base64Str = Convert.ToBase64String(buffer, 0, ret);
                ArrayPool<byte>.Shared.Return(buffer);
                await _PushDataToAgent(base64Str);
            }
        }

        private async ValueTask AgentTunnelPackageHandler(QpChannel channel, byte packageType, ReadOnlySequence<byte> bodyBuffer)
        {
            if (TunnelInfo.ClientTunnelPackageType > 0)
            {
                await _PushDataToClient(bodyBuffer);
            }
            else
            {
                var ret = Convert.ToInt32(bodyBuffer.Length);
                var buffer = ArrayPool<byte>.Shared.Rent(ret);
                bodyBuffer.CopyTo(buffer.AsSpan(0, ret));
                var base64Str = Convert.ToBase64String(buffer, 0, ret);
                ArrayPool<byte>.Shared.Return(buffer);
                await _PushDataToClient(base64Str);
            }
        }

        private async Task PushBase64Data(QpChannel channel, string data)
        {
            try
            {
                await channel.SendNoticePackage(new G.D()
                {
                    TunnelId = TunnelInfo.Id,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        private async Task _PushDataToClient(string data)
        {
            await PushBase64Data(Client.Channel, data);
            DownloadBytes += data.Length;
        }

        private async Task _PushDataToClient(ReadOnlySequence<byte> data)
        {
            var ret = Convert.ToInt32(data.Length);
            await Client.Channel.SendPackage(TunnelInfo.ClientTunnelPackageType, async writer =>
            {
                var span = writer.GetSpan(ret);
                data.CopyTo(span);
                writer.Advance(ret);
                await writer.FlushAsync();
                return ret;
            });
            DownloadBytes += data.Length;
        }

        public async Task PushDataToClient(string data)
        {
            if (TunnelInfo.ClientTunnelPackageType > 0)
            {
                var bufferLength = Encoding.UTF8.GetByteCount(data);
                var buffer = ArrayPool<byte>.Shared.Rent(bufferLength);
                Encoding.UTF8.GetBytes(data, buffer.AsSpan(0, bufferLength));
                var ret = Base64.DecodeFromUtf8InPlace(buffer.AsSpan(0, bufferLength), out var bytesWritten);
                if (ret != OperationStatus.Done)
                    throw new IOException($"Error when convert base64 string to byte array,reason: {ret}");
                await _PushDataToClient(new ReadOnlySequence<byte>(buffer, 0, bytesWritten));
            }
            else
            {
                await _PushDataToClient(data);
            }
        }


        private async Task _PushDataToAgent(string data)
        {
            await PushBase64Data(Agent.Channel, data);
            UploadBytes += data.Length;
        }

        private async Task _PushDataToAgent(ReadOnlySequence<byte> data)
        {
            var ret = Convert.ToInt32(data.Length);
            await Agent.Channel.SendPackage(TunnelInfo.AgentTunnelPackageType, async writer =>
            {
                var span = writer.GetSpan(ret);
                data.CopyTo(span);
                writer.Advance(ret);
                await writer.FlushAsync();
                return ret;
            });
            UploadBytes += data.Length;
        }

        public async Task PushDataToAgent(string data)
        {
            if (TunnelInfo.AgentTunnelPackageType > 0)
            {
                var bufferLength = Encoding.UTF8.GetByteCount(data);
                var buffer = ArrayPool<byte>.Shared.Rent(bufferLength);
                Encoding.UTF8.GetBytes(data, buffer.AsSpan(0, bufferLength));
                var ret = Base64.DecodeFromUtf8InPlace(buffer.AsSpan(0, bufferLength), out var bytesWritten);
                if (ret != OperationStatus.Done)
                    throw new IOException($"Error when convert base64 string to byte array,reason: {ret}");
                await _PushDataToAgent(new ReadOnlySequence<byte>(buffer, 0, bytesWritten));
            }
            else
            {
                await _PushDataToAgent(data);
            }
        }

        public Task SendTunnelClosedNotice(QpChannel channel) => channel.SendNoticePackage(new TunnelClosed() { TunnelId = TunnelInfo.Id });
        public Task SendTunnelClosedNoticeToClient() => SendTunnelClosedNotice(Client.Channel);
        public Task SendTunnelClosedNoticeToAgent() => SendTunnelClosedNotice(Agent.Channel);
        public Task StartAgentTunnel() => Agent.StartTunnelAsync(TunnelInfo.Id);

        public void Dispose()
        {
            if (TunnelInfo.ClientTunnelPackageType > 0)
                Client.Channel.UnregisterPackageHandler(TunnelInfo.ClientTunnelPackageType);
            if (TunnelInfo.AgentTunnelPackageType > 0)
                Agent.Channel.UnregisterPackageHandler(TunnelInfo.AgentTunnelPackageType);
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }
    }
}
