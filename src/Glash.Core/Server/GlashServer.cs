using Glash.Model;
using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Core.Server
{
    public class GlashServer
    {
        private QpServerOptions qpServerOptions;
        private QpServer qpServer;
        private CommandExecuterManager commandExecuterManager = new CommandExecuterManager();
        private NoticeHandlerManager noticeHandlerManager = new NoticeHandlerManager();
        private Dictionary<string, GlashAgentContext> agentDict = new Dictionary<string, GlashAgentContext>();
        private Dictionary<string, GlashClientContext> clientDict = new Dictionary<string, GlashClientContext>();
        private Dictionary<int, GlashServerTunnelContext> serverTunnelContextDict = new Dictionary<int, GlashServerTunnelContext>();
        private int nextTunnelId = -1;

        public GlashAgentContext[] Agents { get; private set; } = new GlashAgentContext[0];
        public GlashClientContext[] Clients { get; private set; } = new GlashClientContext[0];
        public GlashServerTunnelContext[] Tunnels { get; private set; } = new GlashServerTunnelContext[0];

        public event EventHandler<string> LogPushed;
        public event EventHandler<GlashClientContext> ClientConnected;
        public event EventHandler<GlashAgentContext> AgentConnected;

        public event EventHandler<GlashClientContext> ClientDisconnected;
        public event EventHandler<GlashAgentContext> AgentDisconnected;

        public GlashServer()
        {
            commandExecuterManager.Register(
                new Glash.Agent.Protocol.QpCommands.Register.Request(),
                ExecuteCommand_Agent_Register);
            commandExecuterManager.Register(
                new Glash.Client.Protocol.QpCommands.Register.Request(),
                ExecuteCommand_Client_Register);
            commandExecuterManager.Register(
                new Glash.Client.Protocol.QpCommands.GetAgentList.Request(),
                ExecuteCommand_Client_GetAgentList);
            commandExecuterManager.Register(
                new Glash.Client.Protocol.QpCommands.CreateTunnel.Request(),
                ExecuteCommand_Client_CreateTunnel);
            commandExecuterManager.Register(
                new Glash.Client.Protocol.QpCommands.StartTunnel.Request(),
                ExecuteCommand_Client_StartTunnel);

            noticeHandlerManager.Register<G.D>(OnTunnelDataAviliable);
            noticeHandlerManager.Register<TunnelClosed>(OnTunnelClosed);
        }

        public void Start()
        {
            if (qpServer == null)
                throw new ApplicationException("Must call Init method before start.");
            qpServer.Start();
        }

        public void Stop()
        {
            qpServer?.Stop();
        }

        public void Init(QpServer qpServer = null)
        {
            if (qpServer == null)
                qpServer = qpServerOptions.CreateServer();
            this.qpServer = qpServer;
        }

        public void HandleServerOptions(QpServerOptions qpServerOptions)
        {
            qpServerOptions.InstructionSet = new[]
            {
                Glash.Agent.Protocol.Instruction.Instance,
                Glash.Client.Protocol.Instruction.Instance
            };
            qpServerOptions.RegisterCommandExecuterManager(commandExecuterManager);
            qpServerOptions.RegisterNoticeHandlerManager(noticeHandlerManager);
            this.qpServerOptions = qpServerOptions;
        }

        //Register as Agent
        private Glash.Agent.Protocol.QpCommands.Register.Response ExecuteCommand_Agent_Register(
            QpChannel channel,
            Glash.Agent.Protocol.QpCommands.Register.Request request)
        {
            var key = request.Name;
            GlashAgentContext agent = null;
            lock (agentDict)
            {
                if (agentDict.ContainsKey(key))
                    throw new ApplicationException($"Agent [{key}] already registered.");
                agent = new GlashAgentContext(key, channel);
                agentDict[key] = agent;
                Agents = agentDict.Values.ToArray();
            }
            channel.Tag = agent;
            channel.Disconnected += (s, e) =>
            {
                GlashAgentContext context = null;
                lock (agentDict)
                {
                    if (!agentDict.ContainsKey(key))
                        return;
                    context = agentDict[key];
                    context.Dispose();
                    agentDict.Remove(key);
                    Agents = agentDict.Values.ToArray();
                }
                foreach (var tunnel in Tunnels)
                {
                    if (tunnel.Agent.Name == key)
                        tunnel.OnError(new ApplicationException($"Agent[{key}] disconnected."));
                }
                AgentDisconnected?.Invoke(this, context);
            };
            AgentConnected?.Invoke(this, agent);
            return new Glash.Agent.Protocol.QpCommands.Register.Response();
        }

        //Register as Client
        private Glash.Client.Protocol.QpCommands.Register.Response ExecuteCommand_Client_Register(
            QpChannel channel,
            Glash.Client.Protocol.QpCommands.Register.Request request)
        {
            var key = channel.ChannelName;
            GlashClientContext client = null;
            lock (clientDict)
            {
                if (clientDict.ContainsKey(key))
                    throw new ApplicationException($"Client [{key}] already registered.");
                client = new GlashClientContext(key, channel);
                clientDict[key] = client;
                Clients = clientDict.Values.ToArray();
            }
            channel.Tag = client;
            channel.Disconnected += (s, e) =>
            {
                GlashClientContext context = null;
                lock (clientDict)
                {
                    if (!clientDict.ContainsKey(key))
                        return;
                    context = clientDict[key];
                    clientDict.Remove(key);
                    Clients = clientDict.Values.ToArray();
                }
                foreach (var tunnel in Tunnels)
                {
                    if (tunnel.Client.Name == key)
                        tunnel.OnError(new ApplicationException($"Agent[{key}] disconnected."));
                }
                ClientDisconnected?.Invoke(this, context);
            };
            ClientConnected?.Invoke(this, client);
            return new Glash.Client.Protocol.QpCommands.Register.Response();
        }

        private Glash.Client.Protocol.QpCommands.GetAgentList.Response ExecuteCommand_Client_GetAgentList(
            QpChannel channel,
            Glash.Client.Protocol.QpCommands.GetAgentList.Request request)
        {
            return new Glash.Client.Protocol.QpCommands.GetAgentList.Response()
            {
                Data = Agents.Select(t => t.Name).ToArray()
            };
        }

        private Glash.Client.Protocol.QpCommands.CreateTunnel.Response ExecuteCommand_Client_CreateTunnel(
            QpChannel channel,
            Glash.Client.Protocol.QpCommands.CreateTunnel.Request request)
        {
            var tunnelId = Interlocked.Increment(ref nextTunnelId);
            if (tunnelId > int.MaxValue / 2)
                tunnelId = 0;
            var tunnelInfo = request.Data;
            tunnelInfo.Id = tunnelId;

            GlashClientContext clientContext = null;
            if (!clientDict.TryGetValue(channel.ChannelName, out clientContext))
                throw new ArgumentException($"Client[{channel.ChannelName}] not registered.");
            GlashAgentContext agentContext = null;
            if (!agentDict.TryGetValue(tunnelInfo.Agent, out agentContext))
                throw new ArgumentException($"Agent[{tunnelInfo.Agent}] not registered.");
            agentContext.CreateTunnelAsync(tunnelInfo).Wait();

            var serverTunnelContext = new GlashServerTunnelContext(
                tunnelInfo,
                clientContext,
                agentContext,
                ex =>
                {
                    lock (serverTunnelContextDict)
                    {
                        GlashServerTunnelContext serverTunnelContext;
                        if (!serverTunnelContextDict.TryGetValue(tunnelId, out serverTunnelContext))
                            return;
                        serverTunnelContextDict.Remove(tunnelId);
                        Tunnels = serverTunnelContextDict.Values.ToArray();
                        serverTunnelContext.Dispose();
                    }
                });

            lock (serverTunnelContextDict)
            {
                serverTunnelContextDict[tunnelId] = serverTunnelContext;
                Tunnels = serverTunnelContextDict.Values.ToArray();
            }

            return new Glash.Client.Protocol.QpCommands.CreateTunnel.Response() { Data = tunnelInfo };
        }

        private Glash.Client.Protocol.QpCommands.StartTunnel.Response ExecuteCommand_Client_StartTunnel(
            QpChannel channel,
            Glash.Client.Protocol.QpCommands.StartTunnel.Request request)
        {
            var tunnelId = request.TunnelId;
            GlashServerTunnelContext serverTunnelContext;
            if (!serverTunnelContextDict.TryGetValue(tunnelId, out serverTunnelContext))
                throw new ArgumentException($"Tunnel[{tunnelId}] not exist.");
            serverTunnelContext.StartAgentTunnel();
            return new Glash.Client.Protocol.QpCommands.StartTunnel.Response();
        }

        private void OnTunnelDataAviliable(QpChannel channel, G.D data)
        {
            if (channel.Tag == null)
                return;
            var tunnelId = data.TunnelId;
            GlashServerTunnelContext serverTunnelContext;
            if (!serverTunnelContextDict.TryGetValue(tunnelId, out serverTunnelContext))
                return;
            if (channel.Tag is GlashAgentContext)
            {
                serverTunnelContext.PushDataToClient(data.Data);
            }
            else if (channel.Tag is GlashClientContext)
            {
                serverTunnelContext.PushDataToAgent(data.Data);
            }
        }

        private void OnTunnelClosed(QpChannel channel, Model.TunnelClosed data)
        {
            var tunnelId = data.TunnelId;
            LogPushed?.Invoke(this, $"Tunnel[{tunnelId}] closed.");
            if (channel.Tag == null)
                return;
            GlashServerTunnelContext serverTunnelContext;
            if (!serverTunnelContextDict.TryGetValue(tunnelId, out serverTunnelContext))
                return;
            if (channel.Tag is GlashAgentContext)
            {
                serverTunnelContext.SendTunnelClosedNoticeToClient();
            }
            else if (channel.Tag is GlashClientContext)
            {
                serverTunnelContext.SendTunnelClosedNoticeToAgent();
            }
        }
    }
}
