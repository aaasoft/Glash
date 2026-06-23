using Glash.Client.Protocol.QpModel;
using Glash.Core;
using Quick.Utils;
using System.Net;
using System.Net.Sockets;

namespace Glash.Client
{
    public class ProxyRuleContext : PropertyNotifyModel,IDisposable
    {
        private GlashClient glashClient;
        private TcpListener tcpListener;
        private CancellationTokenSource cts;
        public ProxyRuleInfo Config { get; private set; }
        public int LocalPort { get; private set; }

        private bool _Working;
        public bool Working
        {
            get => _Working;
            set => RaiseAndSetIfChanged(ref _Working, value);
        }

        public static int MaxLogLines = 100;
        private Queue<string> logQueue = new();
        public string[] Logs
        {
            get
            {
                lock (logQueue)
                    return logQueue.ToArray();
            }
        }
        private void pushLog(string line)
        {
            line = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {line}";
            lock (logQueue)
            {
                logQueue.Enqueue(line);
                while (true)
                {
                    var currentCount = logQueue.Count;
                    if (currentCount == 0 || currentCount <= MaxLogLines)
                        break;
                    logQueue.Dequeue();
                }
            }
        }

        public ProxyRuleContext(GlashClient glashClient, ProxyRuleInfo config)
        {
            this.glashClient = glashClient;
            Config = config;
            LocalPort = config.LocalPort;

            if (config.Enable)
                Enable();
        }

        public void Enable()
        {
            cts?.Cancel();
            tcpListener?.Stop();

            cts = new CancellationTokenSource();
            tcpListener = new TcpListener(IPAddress.Parse(Config.LocalIPAddress), Config.LocalPort);
            _ = beginStart(cts.Token);
        }

        public void Disable()
        {
            pushLog("Stoping listen.");
            cts?.Cancel();

            tcpListener?.Stop();
            tcpListener = null;
            LocalPort = Config.LocalPort;
            pushLog("Listen stoped.");
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
                pushLog($"Start listen {Config.LocalIPAddress}:{Config.LocalPort}...");
                tcpListener.Start();
                LocalPort = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
                pushLog($"Listening {Config.LocalIPAddress}:{LocalPort}.");
                _ = beginAcceptTcpClient(tcpListener, token);
                Working = true;
            }
            catch (Exception ex)
            {
                pushLog($"Listen {Config.LocalIPAddress}:{LocalPort} error.Reason: {ExceptionUtils.GetExceptionMessage(ex)}");
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
            Disable();
        }
    }
}
