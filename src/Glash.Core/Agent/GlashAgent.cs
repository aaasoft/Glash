using Quick.Protocol;
using Quick.Protocol.Utils;
using System.Net.Sockets;

namespace Glash.Core.Agent
{
    public class GlashAgent : IDisposable
    {
        private QpClient qpClient;
        private string agentName;
        private Dictionary<int, GlashTunnelContext> tunnelContextDict = new Dictionary<int, GlashTunnelContext>();

        public event EventHandler Disconnected;
        public event EventHandler<string> LogPushed;

        public GlashAgent(string url, string password, string agentName)
        {
            var qpClientOptions = QpClientOptions.Parse(new Uri(url));
            qpClientOptions.Password = password;
            qpClientOptions.InstructionSet = new[]
            {
                Glash.Agent.Protocol.Instruction.Instance
            };

            var commandExecuterManager = new CommandExecuterManager();
            commandExecuterManager.Register(
                new Glash.Agent.Protocol.QpCommands.CreateTunnel.Request(),
                executeCommand_CreateTunnel);
            commandExecuterManager.Register(
                new Glash.Agent.Protocol.QpCommands.StartTunnel.Request(),
                executeCommand_StartTunnel);
            qpClientOptions.RegisterCommandExecuterManager(commandExecuterManager);

            var noticeHandlerManager = new NoticeHandlerManager();
            noticeHandlerManager.Register<G.D>(OnTunnelDataAviliable);
            noticeHandlerManager.Register<Model.TunnelClosed>(OnTunnelClosed);
            qpClientOptions.RegisterNoticeHandlerManager(noticeHandlerManager);

            qpClient = qpClientOptions.CreateClient();
            qpClient.Disconnected += QpClient_Disconnected;
            this.agentName = agentName;
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
            await qpClient.SendCommand(new Glash.Agent.Protocol.QpCommands.Register.Request()
            {
                Name = agentName
            });
        }

        public void Dispose()
        {
            qpClient.Disconnect();
        }

        private Glash.Agent.Protocol.QpCommands.CreateTunnel.Response executeCommand_CreateTunnel(
            QpChannel channel,
            Glash.Agent.Protocol.QpCommands.CreateTunnel.Request request)
        {
            var tunnelInfo = request.Data;
            var tunnelId = tunnelInfo.Id;
            switch (tunnelInfo.Type)
            {
                case Model.ProtocolType.TCP:
                    {
                        var tcpClient = new TcpClient();
                        tcpClient.Connect(tunnelInfo.Host, tunnelInfo.Port);
                        var tunnelContext = new GlashTunnelContext(
                            channel, tunnelInfo,
                            tcpClient.GetStream(),
                            ex =>
                            {
                                LogPushed?.Invoke(this, $"Tunnel[{tunnelId}] error.Message:{ExceptionUtils.GetExceptionMessage(ex)}");
                                channel.SendNoticePackage(new Model.TunnelClosed() { TunnelId = tunnelId });
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
                        LogPushed?.Invoke(this, $"Create tunnel[{tunnelId}] to {tunnelInfo.Type}://{tunnelInfo.Host}:{tunnelInfo.Port} success.");
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
            return new Glash.Agent.Protocol.QpCommands.CreateTunnel.Response();
        }

        private Glash.Agent.Protocol.QpCommands.StartTunnel.Response executeCommand_StartTunnel(
            QpChannel channel,
            Glash.Agent.Protocol.QpCommands.StartTunnel.Request request)
        {
            var tunnelId = request.TunnelId;
            GlashTunnelContext tunnelContext;
            if (!tunnelContextDict.TryGetValue(tunnelId, out tunnelContext))
                throw new ApplicationException($"Tunnel[{tunnelId}] not exist.");
            tunnelContext.Start();
            return new Glash.Agent.Protocol.QpCommands.StartTunnel.Response();
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
