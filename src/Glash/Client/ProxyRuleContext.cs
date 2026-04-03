using Glash.Client.Protocol.QpModel;
using System.Net;
using System.Net.Sockets;

namespace Glash.Client
{
    public class ProxyRuleContext : IDisposable
    {
        private GlashClient glashClient;
        private TcpListener tcpListener;
        private CancellationTokenSource cts;
        public ProxyRuleInfo Config { get; private set; }
        public int LocalPort { get; private set; }

        public bool Working { get; private set; }

        public ProxyRuleContext(GlashClient glashClient, ProxyRuleInfo config)
        {
            this.glashClient = glashClient;
            Config = config;
            LocalPort = config.LocalPort;

            cts?.Cancel();
            cts = new CancellationTokenSource();

            if (config.Enable)
            {
                tcpListener = new TcpListener(IPAddress.Parse(Config.LocalIPAddress), Config.LocalPort);
                _ = beginStart(cts.Token);
            }
        }


        private async Task delayToStart(CancellationToken token)
        {
            try
            {
                await Task.Delay(5000, token);
                _ = beginStart(token);
            }
            catch { }
        }

        private async Task beginStart(CancellationToken token)
        {
            try
            {
                tcpListener.Start();
                LocalPort = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
                _ = beginAcceptTcpClient(tcpListener, token);
                Working = true;
            }
            catch
            {
                _ = delayToStart(token);
            }
        }

        private async Task beginAcceptTcpClient(TcpListener tcpListener, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var tcpClient = await tcpListener.AcceptTcpClientAsync(token);
                    var connectionName = $"TCP:{tcpClient.Client.RemoteEndPoint}";
                    //Create and Start Tunnel
                    _ = glashClient.CreateAndStartTunnelAsync(Config, connectionName, tcpClient.GetStream());
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        public void Dispose()
        {
            cts?.Cancel();

            tcpListener?.Stop();
            tcpListener = null;
            LocalPort = Config.LocalPort;
        }
    }
}
