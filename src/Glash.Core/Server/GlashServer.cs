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
        private Dictionary<string, GlashAgentContext> agentDict = new Dictionary<string, GlashAgentContext>();
        private Dictionary<string, GlashClientContext> clientDict = new Dictionary<string, GlashClientContext>();

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
            this.qpServerOptions = qpServerOptions;
        }

        //Register as Agent
        private Glash.Agent.Protocol.QpCommands.Register.Response ExecuteCommand_Agent_Register(
            QpChannel channel,
            Glash.Agent.Protocol.QpCommands.Register.Request request)
        {
            lock (agentDict)
            {
                var key = request.Name;
                if (agentDict.ContainsKey(key))
                    throw new ApplicationException($"Agent [{key}] already registered.");
                var context = new GlashAgentContext(new Model.AgentInfo()
                {
                    Name = key,
                    ConnectionInfo = channel.ChannelName
                }, channel);
                agentDict[key] = context;
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
                    }
                    AgentDisconnected?.Invoke(this, context);
                };
                AgentConnected?.Invoke(this, context);
            }
            return new Glash.Agent.Protocol.QpCommands.Register.Response();
        }

        //Register as Client
        private Glash.Client.Protocol.QpCommands.Register.Response ExecuteCommand_Client_Register(
            QpChannel channel,
            Glash.Client.Protocol.QpCommands.Register.Request request)
        {
            lock (clientDict)
            {
                var key = channel.ChannelName;
                if (clientDict.ContainsKey(key))
                    throw new ApplicationException($"Client [{key}] already registered.");
                var context = new GlashClientContext(channel);
                clientDict[key] = context;
                channel.Disconnected += (s, e) =>
                {
                    GlashClientContext context = null;
                    lock (clientDict)
                    {
                        if (!clientDict.ContainsKey(key))
                            return;
                        context = clientDict[key];
                        context.Dispose();
                        clientDict.Remove(key);
                    }
                    ClientDisconnected?.Invoke(this, context);
                };
                ClientConnected?.Invoke(this, context);
            }
            return new Glash.Client.Protocol.QpCommands.Register.Response();
        }

        private Glash.Client.Protocol.QpCommands.GetAgentList.Response ExecuteCommand_Client_GetAgentList(
            QpChannel channel,
            Glash.Client.Protocol.QpCommands.GetAgentList.Request request)
        {
            return new Glash.Client.Protocol.QpCommands.GetAgentList.Response();
        }

        private Glash.Client.Protocol.QpCommands.CreateTunnel.Response ExecuteCommand_Client_CreateTunnel(
            QpChannel channel,
            Glash.Client.Protocol.QpCommands.CreateTunnel.Request request)
        {
            return new Glash.Client.Protocol.QpCommands.CreateTunnel.Response();
        }
    }
}
