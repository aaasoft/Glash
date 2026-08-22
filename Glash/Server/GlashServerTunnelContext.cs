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
            this.TunnelInfo = tunnelInfo;
            this.Client = client;
            this.Agent = agent;
            this.errorHandler = errorHandler;
            CreateTime = DateTime.Now;
            cts = new CancellationTokenSource();
            beginCalcSpeed(cts.Token);
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

        private async Task PushData(QpChannel channel, string data)
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

        public async Task PushDataToClient(string data)
        {
            await PushData(Client.Channel, data);
            DownloadBytes += data.Length;
        }

        public async Task PushDataToAgent(string data)
        {
            await PushData(Agent.Channel, data);
            UploadBytes += data.Length;
        }

        public Task SendTunnelClosedNotice(QpChannel channel) => channel.SendNoticePackage(new TunnelClosed() { TunnelId = TunnelInfo.Id });
        public Task SendTunnelClosedNoticeToClient() => SendTunnelClosedNotice(Client.Channel);
        public Task SendTunnelClosedNoticeToAgent() => SendTunnelClosedNotice(Agent.Channel);
        public Task StartAgentTunnel() => Agent.StartTunnelAsync(TunnelInfo.Id);

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }
    }
}
