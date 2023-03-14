using Glash.Model;
using Quick.Protocol;
using Quick.Protocol.Utils;

namespace Glash.Core.Client
{
    public class GlashClient : IDisposable
    {
        private QpClient qpClient;
        private Dictionary<string, ProxyContext> proxyContextDict = new Dictionary<string, ProxyContext>();

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

        private void closeAllTunnel()
        {
            GlashTunnelContext[] tunnels = null;
            lock (tunnelContextDict)
            {
                tunnels = tunnelContextDict.Values.ToArray();
                tunnelContextDict.Clear();
            }
            foreach (var tunnel in tunnels)
                tunnel.Dispose();
        }

        private void QpClient_Disconnected(object sender, EventArgs e)
        {
            closeAllTunnel();
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
            closeAllTunnel();
            foreach (var proxyContext in proxyContextDict.Values)
                proxyContext.Stop();
            proxyContextDict.Clear();
            qpClient.Disconnect();
        }

        public void EnableProxyInfo(ProxyContext context)
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
                throw;
            }
        }

        public void EnableProxyInfo(string proxyId)
        {
            if (!proxyContextDict.ContainsKey(proxyId))
                return;
            var context = proxyContextDict[proxyId];
            EnableProxyInfo(context);
        }

        public void DisableProxyInfo(ProxyContext context)
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
                throw;
            }
        }

        public void DisableProxyPortInfo(string configId)
        {
            if (!proxyContextDict.ContainsKey(configId))
                return;
            var context = proxyContextDict[configId];
            DisableProxyInfo(context);
        }

        public void AddProxyPortInfo(ProxyInfo config)
        {
            var context = new ProxyContext(this, config);
            proxyContextDict[config.Name] = context;
            if (config.Enable)
                try
                {
                    EnableProxyInfo(context);
                }
                catch { }
        }

        public void AddProxyPortInfos(ProxyInfo[] items)
        {
            foreach (var item in items)
                AddProxyPortInfo(item);
        }

        public void RemoveProxyPortInfo(string proxyName)
        {
            if (!proxyContextDict.ContainsKey(proxyName))
                return;
            var context = proxyContextDict[proxyName];
            proxyContextDict.Remove(proxyName);
            DisableProxyInfo(context);
        }

        private Dictionary<int, GlashTunnelContext> tunnelContextDict = new Dictionary<int, GlashTunnelContext>();

        internal async Task CreateAndStartTunnelAsync(ProxyInfo config, string connectionName, Stream stream)
        {
            try
            {
                //Create Tunnel
                var rep = await qpClient.SendCommand(new Glash.Client.Protocol.QpCommands.CreateTunnel.Request()
                {
                    Data = new Model.TunnelInfo()
                    {
                        Agent = config.Agent,
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
                        LogPushed?.Invoke(this, $"Tunnel[{tunnelId}] closed.");
                    });
                lock (tunnelContextDict)
                    tunnelContextDict[tunnelId] = tunnelContext;

                //Start Tunnel
                await qpClient.SendCommand(new Glash.Client.Protocol.QpCommands.StartTunnel.Request() { TunnelId = tunnelId });
                tunnelContext.Start();

                LogPushed?.Invoke(this, $"[{connectionName}]: Create tunnel[{tunnelId}] to tcp://{config.Agent}/{config.RemoteHost}/{config.RemotePort} success.");
            }
            catch (Exception ex)
            {
                LogPushed?.Invoke(this, $"[{connectionName}]: Create tunnel to tcp://{config.Agent}/{config.RemoteHost}/{config.RemotePort} failed.Reason:{ExceptionUtils.GetExceptionMessage(ex)}");
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

        public async Task<string[]> GetAgentListAsync()
        {
            var rep = await qpClient.SendCommand(new Glash.Client.Protocol.QpCommands.GetAgentList.Request());
            return rep.Data;
        }
    }
}