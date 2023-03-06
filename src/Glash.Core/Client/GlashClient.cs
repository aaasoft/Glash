using Glash.Model;
using Quick.Protocol;
using Quick.Protocol.Utils;
using System.Net.Sockets;

namespace Glash.Core.Client
{
    public class GlashClient : IDisposable
    {
        private QpClient qpClient;
        public event EventHandler Disconnected;
        public event EventHandler<string> LogPushed;

        public GlashClient(string url, string password)
        {
            var qpClientOptions = QpClientOptions.Parse(new Uri(url));
            qpClientOptions.Password = password;
            qpClientOptions.InstructionSet = new[]
            {
                Glash.Agent.Protocol.Instruction.Instance
            };
            var noticeHandlerManager = new NoticeHandlerManager();
            noticeHandlerManager.Register<G.D>(OnTunnelDataAviliable);
            noticeHandlerManager.Register<TunnelClosed>(OnTunnelClosed);
            qpClientOptions.RegisterNoticeHandlerManager(noticeHandlerManager);
            qpClient = qpClientOptions.CreateClient();
            qpClient.Disconnected += QpClient_Disconnected;
        }

        private void QpClient_Disconnected(object sender, EventArgs e)
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public async Task ConnectAsync()
        {
            //Connect
            await qpClient.ConnectAsync();
            //Register
            await qpClient.SendCommand(new Glash.Client.Protocol.QpCommands.Register.Request());
        }

        public void Dispose()
        {
            qpClient.Disconnect();
        }

        private Dictionary<string, ProxyContext> proxyPortContextDict = new Dictionary<string, ProxyContext>();

        public void EnableProxyPortInfo(ProxyContext context)
        {
            try
            {
                context.Start();
                context.Config.Enable = true;
                LogPushed?.Invoke(this, $"{context.Config} enabled.");
            }
            catch (Exception ex)
            {
                context.Config.Enable = false;
                LogPushed?.Invoke(this, $"{context.Config} enable failed.Reason:{ExceptionUtils.GetExceptionMessage(ex)}");
            }
        }

        public void EnableProxyPortInfo(string configId)
        {
            if (!proxyPortContextDict.ContainsKey(configId))
                return;
            var context = proxyPortContextDict[configId];
            EnableProxyPortInfo(context);
        }

        public void DisableProxyPortInfo(ProxyContext context)
        {
            try
            {
                context.Stop();
                context.Config.Enable = false;
                LogPushed?.Invoke(this, $"{context.Config} disabled.");
            }
            catch (Exception ex)
            {
                LogPushed?.Invoke(this, $"{context.Config} disable failed.Reason:{ExceptionUtils.GetExceptionMessage(ex)}");
            }
        }

        public void DisableProxyPortInfo(string configId)
        {
            if (!proxyPortContextDict.ContainsKey(configId))
                return;
            var context = proxyPortContextDict[configId];
            DisableProxyPortInfo(context);
        }

        public void AddProxyPortInfo(ProxyInfo config)
        {
            var context = new ProxyContext(this, config);
            proxyPortContextDict[config.Name] = context;
            if (config.Enable)
                EnableProxyPortInfo(context);
        }

        public void AddProxyPortInfos(ProxyInfo[] items)
        {
            foreach (var item in items)
                AddProxyPortInfo(item);
        }

        public void RemoveProxyPortInfo(string configId)
        {
            if (!proxyPortContextDict.ContainsKey(configId))
                return;
            var context = proxyPortContextDict[configId];
            proxyPortContextDict.Remove(configId);
            DisableProxyPortInfo(context);
        }

        private Dictionary<int, GlashTunnelContext> tunnelContextDict = new Dictionary<int, GlashTunnelContext>();

        public async Task CreateAndStartTunnelAsync(ProxyInfo config, string connectionName, Stream stream)
        {
            try
            {
                //Create Tunnel
                var rep = await qpClient.SendCommand(new Glash.Client.Protocol.QpCommands.CreateTunnel.Request()
                {
                    Data = new Model.TunnelInfo()
                    {
                        Agent = config.Agent,
                        Type = config.Type,
                        Host = config.RemoteHost,
                        Port = config.RemotePort
                    }
                });
                var tunnelId = rep.Data.Id;
                var tunnelContext = new GlashTunnelContext(
                    qpClient,
                    rep.Data,
                    stream,
                    ex =>
                    {
                        LogPushed?.Invoke(this, $"Tunnel[{tunnelId}] error.Message:{ExceptionUtils.GetExceptionMessage(ex)}");
                        qpClient.SendNoticePackage(new TunnelClosed() { TunnelId = tunnelId });

                        GlashTunnelContext tunnelContext = null;
                        lock (tunnelContextDict)
                        {
                            if (!tunnelContextDict.ContainsKey(tunnelId))
                                return;
                            tunnelContext = tunnelContextDict[tunnelId];
                        }
                        tunnelContext.Dispose();                        
                    });
                lock (tunnelContextDict)
                    tunnelContextDict[tunnelId] = tunnelContext;

                //Start Tunnel
                await qpClient.SendCommand(new Glash.Client.Protocol.QpCommands.StartTunnel.Request() { TunnelId = tunnelId });
                tunnelContext.Start();

                LogPushed?.Invoke(this, $"[{connectionName}]: Create tunnel[{tunnelId}] to {config.Type}://{config.Agent}/{config.RemoteHost}/{config.RemotePort} success.");
            }
            catch (Exception ex)
            {
                LogPushed?.Invoke(this, $"[{connectionName}]: Create tunnel to {config.Type}://{config.Agent}/{config.RemoteHost}/{config.RemotePort} failed.Reason:{ExceptionUtils.GetExceptionMessage(ex)}");
                try
                {
                    stream.Close();
                    stream.Dispose();
                }
                catch { }
            }
        }

        private void OnTunnelDataAviliable(QpChannel channel, G.D data)
        {
            var tunnelId = data.TunnelId;
            if (!tunnelContextDict.ContainsKey(tunnelId))
                return;
            var tunnelContext = tunnelContextDict[tunnelId];
            tunnelContext.PushData(data.Data);
        }

        private void OnTunnelClosed(QpChannel channel, Model.TunnelClosed data)
        {
            var tunnelId = data.TunnelId;
            GlashTunnelContext tunnelContext = null;
            lock (tunnelContextDict)
            {
                if (!tunnelContextDict.ContainsKey(tunnelId))
                    return;
                tunnelContext = tunnelContextDict[tunnelId];
                tunnelContextDict.Remove(tunnelId);
            }
            tunnelContext.Dispose();
            LogPushed?.Invoke(this, $"Tunnel[{tunnelId}] closed.");
        }
    }
}