using System.Net;
using System.Net.Sockets;

namespace Glash.Core.Client
{
    public class ProxyPortContext
    {
        private GlashClient glashClient;
        private TcpListener tcpListener;
        private CancellationTokenSource cts;
        public ProxyPortInfo Config { get; private set; }

        public ProxyPortContext(GlashClient glashClient, ProxyPortInfo config)
        {
            this.glashClient = glashClient;
            Config = config;
        }

        public void Start()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            switch (Config.ProtocolType)
            {
                case Model.ProtocolType.TCP:
                    {
                        tcpListener = new TcpListener(IPAddress.Parse(Config.LocalIPAddress), Config.LocalPort);
                        tcpListener.Start();
                        _ = beginAcceptTcpClient(tcpListener, cts.Token);
                        break;
                    }
                case Model.ProtocolType.UDP:
                    {
                        break;
                    }
            }
        }

        public void Stop()
        {
            cts?.Cancel();

            switch (Config.ProtocolType)
            {
                case Model.ProtocolType.TCP:
                    {
                        tcpListener.Stop();
                        tcpListener = null;
                        break;
                    }
                case Model.ProtocolType.UDP:
                    {
                        break;
                    }
            }
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
