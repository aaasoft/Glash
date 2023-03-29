using System.Net;
using System.Net.Sockets;

namespace Glash.Core.Client
{
    public class ProxyRuleContext
    {
        private GlashClient glashClient;
        private TcpListener tcpListener;
        private CancellationTokenSource cts;
        public IProxyRule Config { get; private set; }

        public ProxyRuleContext(GlashClient glashClient, IProxyRule config)
        {
            this.glashClient = glashClient;
            Config = config;
        }

        public void Start()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            tcpListener = new TcpListener(IPAddress.Parse(Config.LocalIPAddress), Config.LocalPort);
            tcpListener.Start();
            _ = beginAcceptTcpClient(tcpListener, cts.Token);
        }

        public void Stop()
        {
            cts?.Cancel();

            tcpListener?.Stop();
            tcpListener = null;
        }

        private async Task beginAcceptTcpClient(TcpListener tcpListener, CancellationToken token)
        {
            try
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                var connectionName = $"TCP:{tcpClient.Client.RemoteEndPoint}";
                //Create and Start Tunnel
                _ = glashClient.CreateAndStartTunnelAsync(Config, connectionName, tcpClient.GetStream());
            }
            catch (TaskCanceledException)
            {
                return;
            }
            _ = beginAcceptTcpClient(tcpListener, token);
        }
    }
}
