using System.Net;
using System.Net.Sockets;

namespace Glash.Core.Client
{
    public class ProxyContext
    {
        private GlashClient glashClient;
        private TcpListener tcpListener;
        private CancellationTokenSource cts;
        public ProxyInfo Config { get; private set; }

        public ProxyContext(GlashClient glashClient, ProxyInfo config)
        {
            this.glashClient = glashClient;
            Config = config;
        }

        public void Start()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            switch (Config.Type)
            {
                case Model.TunnelType.TCP:
                    {
                        tcpListener = new TcpListener(IPAddress.Parse(Config.LocalIPAddress), Config.LocalPort);
                        tcpListener.Start();
                        _ = beginAcceptTcpClient(tcpListener, cts.Token);
                        break;
                    }
                case Model.TunnelType.UDP:
                default:
                    throw new NotImplementedException();
            }
        }

        public void Stop()
        {
            cts?.Cancel();

            switch (Config.Type)
            {
                case Model.TunnelType.TCP:
                    {
                        tcpListener.Stop();
                        tcpListener = null;
                        break;
                    }
                case Model.TunnelType.UDP:
                default:
                    throw new NotImplementedException();
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
