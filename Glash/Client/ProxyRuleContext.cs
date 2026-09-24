using Glash.Client.Protocol.QpModel;
using Glash.Core;
using Quick.Utils;
using System.Net;
using System.Net.Sockets;

namespace Glash.Client
{
    public class ProxyRuleContext : PropertyNotifyModel, IDisposable
    {
        private GlashClient glashClient;
        private TcpListener tcpListener;
        private CancellationTokenSource cts;
        public ProxyRuleInfo Config { get; private set; }
        public string LocalEndPoint => $"{Config.LocalIPAddress}:{LocalPort}";
        public string RemoteEndPoint => $"{Config.RemoteHost}:{Config.RemotePort}";
        private int _LocalPort;
        public int LocalPort
        {
            get => _LocalPort;
            private set
            {
                if (EqualityComparer<int>.Default.Equals(_LocalPort, value))
                    return;
                _LocalPort = value;
                RaisePropertyChanged(nameof(LocalPort));
                RaisePropertyChanged(nameof(LocalEndPoint));
            }
        }

        private bool _Working;
        public bool Working
        {
            get => _Working;
            set => RaiseAndSetIfChanged(ref _Working, value);
        }

        private long _UploadSpeed;
        public long UploadSpeed
        {
            get => _UploadSpeed;
            private set => RaiseAndSetIfChanged(ref _UploadSpeed, value);
        }

        private long _DownloadSpeed;
        public long DownloadSpeed
        {
            get => _DownloadSpeed;
            private set => RaiseAndSetIfChanged(ref _DownloadSpeed, value);
        }

        private int _ConnectionCount;
        public int ConnectionCount
        {
            get => _ConnectionCount;
            private set => RaiseAndSetIfChanged(ref _ConnectionCount, value);
        }

        private CancellationTokenSource _speedCts;
        private long _preUploadBytes, _preDownloadBytes;

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

            _speedCts = new CancellationTokenSource();
            _ = beginCalcSpeed(_speedCts.Token);

            if (config.Enable)
                Enable();
        }

        public void Enable()
        {
            cts?.Dispose();

            tcpListener?.Stop();

            cts = new CancellationTokenSource();
            tcpListener = new TcpListener(IPAddress.Parse(Config.LocalIPAddress), Config.LocalPort);
            _ = beginStart(cts.Token);
        }

        public void Disable()
        {
            pushLog("Stoping listen.");
            cts?.Cancel();
            cts?.Dispose();
            cts = null;

            tcpListener?.Stop();
            tcpListener = null;
            LocalPort = Config.LocalPort;
            pushLog("Listen stoped.");
        }


        private async Task delayToStart(CancellationToken token)
        {
            try
            {
                await Task.Delay(5000, token).ConfigureAwait(false);
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
                    var tcpClient = await tcpListener.AcceptTcpClientAsync(token).ConfigureAwait(false);
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
            _speedCts?.Cancel();
            _speedCts?.Dispose();
            Disable();
        }

        /// <summary>
        /// 每秒采样该规则下所有隧道的累计流量，计算上行/下行实时速率（字节/秒）。
        /// </summary>
        private async Task beginCalcSpeed(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                try
                {
                    var (up, down) = glashClient.GetProxyRuleTraffic(Config.Id);
                    var upSpeed = up - _preUploadBytes;
                    if (upSpeed < 0) upSpeed = 0;
                    var downSpeed = down - _preDownloadBytes;
                    if (downSpeed < 0) downSpeed = 0;
                    _preUploadBytes = up;
                    _preDownloadBytes = down;
                    UploadSpeed = upSpeed;
                    DownloadSpeed = downSpeed;
                    ConnectionCount = glashClient.GetProxyRuleConnectionCount(Config.Id);
                }
                catch { }
            }
        }
    }
}
