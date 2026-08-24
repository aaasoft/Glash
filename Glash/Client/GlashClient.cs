using Quick.Protocol;
using Glash.Core;
using Glash.Client.Protocol.QpModel;
using Glash.Client.Protocol.QpNotices;
using Quick.Utils;

namespace Glash.Client
{
    public class GlashClient : IDisposable
    {
        private SemaphoreSlim createTunnelLock = new SemaphoreSlim(1, 1);
        private QpClientOptions qpClientOptions;
        private QpClient qpClient;
        private Dictionary<string, ProxyRuleContext> proxyRuleContextDict = new Dictionary<string, ProxyRuleContext>();

        public event EventHandler<AgentLoginStatusChanged> AgentLoginStatusChanged;
        public event EventHandler Disconnected;
        public event EventHandler<string> LogPushed;

        public ProxyRuleContext[] ProxyRuleContexts => proxyRuleContextDict.Values.ToArray();

        public GlashClient(string url, string password = null)
        {
            qpClientOptions = QpClientOptions.Parse(new Uri(url));
            if (!string.IsNullOrEmpty(password))
                qpClientOptions.Password = password;
            qpClientOptions.InstructionSet = new[]
            {
                Client.Protocol.Instruction.Instance
            };
            var noticeHandlerManager = new NoticeHandlerManager();
            noticeHandlerManager.Register<G.D>(OnTunnelDataAviliable);
            noticeHandlerManager.Register<TunnelClosed>(OnTunnelClosed);
            noticeHandlerManager.Register<AgentLoginStatusChanged>(OnAgentLoginStatusChanged);
            qpClientOptions.RegisterNoticeHandlerManager(noticeHandlerManager);
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
            LogPushed?.Invoke(this, $"Disconnected.Message:{ExceptionUtils.GetExceptionMessage(qpClient?.LastException)}");
            closeAllTunnel();
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public async Task ConnectAsync(string user, string password)
        {
            qpClient = qpClientOptions.CreateClient();
            try
            {
                qpClient.Disconnected += QpClient_Disconnected;
                //Connect
                await qpClient.ConnectAsync();
                var answer = CryptoUtils.GetAnswer(qpClient.AuthenticateQuestion, password);
                //Register
                await qpClient.SendCommand(new Protocol.QpCommands.Login.Request()
                {
                    Name = user,
                    Answer = answer
                });
            }
            catch
            {
                qpClient.Disconnected -= QpClient_Disconnected;
                qpClient.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            closeAllTunnel();
            foreach (var proxyContext in proxyRuleContextDict.Values)
                proxyContext.Dispose();
            proxyRuleContextDict.Clear();
            qpClient.Disconnect();
            qpClient.Dispose();
            createTunnelLock.Dispose();
        }

        public async Task EnableProxyRule(string proxyRuleId)
        {
            if (!proxyRuleContextDict.TryGetValue(proxyRuleId, out var context))
                return;
            context.Config.Enable = true;
            await SaveProxyRule(context.Config);
            context.Enable();
        }

        public async Task DisableProxyRule(string proxyRuleId)
        {
            if (!proxyRuleContextDict.TryGetValue(proxyRuleId, out var context))
                return;
            context.Config.Enable = false;
            await SaveProxyRule(context.Config);
            context.Disable();
        }

        public void LoadProxyRule(ProxyRuleInfo config)
        {
            var context = new ProxyRuleContext(this, config);
            proxyRuleContextDict[config.Id] = context;
        }

        public void LoadProxyRules(ProxyRuleInfo[] items)
        {
            foreach (var item in items)
                LoadProxyRule(item);
        }

        public void UnloadProxyRule(ProxyRuleContext proxyRuleContext)
        {
            if (proxyRuleContextDict.ContainsKey(proxyRuleContext.Config.Id))
                proxyRuleContextDict.Remove(proxyRuleContext.Config.Id);
            proxyRuleContext.Dispose();
        }

        public void UnloadProxyRule(string proxyRuleId)
        {
            if (!proxyRuleContextDict.ContainsKey(proxyRuleId))
                return;
            UnloadProxyRule(proxyRuleContextDict[proxyRuleId]);
        }

        private Dictionary<int, GlashTunnelContext> tunnelContextDict = new Dictionary<int, GlashTunnelContext>();

        internal async Task CreateAndStartTunnelAsync(ProxyRuleInfo config, string connectionName, Stream stream)
        {
            try
            {
                await createTunnelLock.WaitAsync();
                byte clientTunnelPackageType = 0;
                try
                {
                    clientTunnelPackageType = qpClient.GetUnusedPackageType();
                }
                catch
                {
                    LogPushed?.Invoke(this, $"Get unused package type error,fallback to legacy mode.");
                }
                //Create Tunnel
                var rep = await qpClient.SendCommand(new Protocol.QpCommands.CreateTunnel.Request()
                {
                    ProxyRuleId = config.Id,
                    ClientTunnelPackageType = clientTunnelPackageType
                });
                var tunnelInfo = rep.Data;
                var tunnelId = tunnelInfo.Id;
                if (tunnelInfo.ClientTunnelPackageType == 0)
                {
                    LogPushed?.Invoke(this, $"Tunnel[{tunnelId}] Server not support tunnel package type,fallback to legacy mode.");
                }
                var tunnelContext = new GlashTunnelContext(
                    qpClient,
                    tunnelId,
                    tunnelInfo.ClientTunnelPackageType,
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
                await qpClient.SendCommand(new Protocol.QpCommands.StartTunnel.Request() { TunnelId = tunnelId });
                tunnelContext.Start();

                LogPushed?.Invoke(this, $"[{connectionName}]: Create tunnel[{tunnelId}] to [{config.Agent}]{config.RemoteHost}:{config.RemotePort} success.");
            }
            catch (Exception ex)
            {
                LogPushed?.Invoke(this, $"[{connectionName}]: Create tunnel to [{config.Agent}]{config.RemoteHost}:{config.RemotePort} failed.Reason:{ExceptionUtils.GetExceptionMessage(ex)}");
                try
                {
                    stream.Close();
                    stream.Dispose();
                }
                catch { }
            }
            finally
            {
                createTunnelLock.Release();
            }
        }

        private async ValueTask OnTunnelDataAviliable(QpChannel channel, G.D data)
        {
            var tunnelId = data.TunnelId;
            if (!tunnelContextDict.ContainsKey(tunnelId))
                return;
            var tunnelContext = tunnelContextDict[tunnelId];
            tunnelContext.PushData(data.Data);
        }

        private async ValueTask OnTunnelClosed(QpChannel channel, TunnelClosed data)
        {
            var tunnelId = data.TunnelId;
            GlashTunnelContext tunnelContext = null;
            lock (tunnelContextDict)
                if (!tunnelContextDict.TryGetValue(tunnelId,out tunnelContext))
                    return;
            tunnelContext.OnError(new ApplicationException("Tunnel closed."));
        }

        private async ValueTask OnAgentLoginStatusChanged(QpChannel channel, AgentLoginStatusChanged data)
        {
            AgentLoginStatusChanged?.Invoke(this, data);
        }

        public async Task<AgentInfo[]> GetAgentListAsync()
        {
            var rep = await qpClient.SendCommand(new Protocol.QpCommands.GetAgentList.Request());
            return rep.Data;
        }

        public async Task<ProxyRuleInfo[]> GetProxyRuleListAsync()
        {
            var rep = await qpClient.SendCommand(new Protocol.QpCommands.GetProxyRuleList.Request());
            return rep.Data;
        }

        public async Task<ProxyRuleInfo> SaveProxyRule(ProxyRuleInfo model)
        {
            var rep = await qpClient.SendCommand(new Glash.Client.Protocol.QpCommands.SaveProxyRule.Request()
            {
                Data = model
            });
            return rep.Data;
        }

        public async Task DeleteProxyRule(string proxyRuleId)
        {
            await qpClient.SendCommand(new Glash.Client.Protocol.QpCommands.DeleteProxyRule.Request()
            {
                ProxyRuleId = proxyRuleId
            });
        }
    }
}