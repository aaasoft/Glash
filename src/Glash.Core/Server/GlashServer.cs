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
        private QpServer qpServer;
        private CommandExecuterManager commandExecuterManager = new CommandExecuterManager();

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

        public void Init(QpServer qpServer)
        {
            this.qpServer = qpServer;
        }

        public void HandleServerOptions(QpServerOptions serverOptions)
        {
            serverOptions.RegisterCommandExecuterManager(commandExecuterManager);
            serverOptions.InstructionSet = new[]
            {
                Glash.Agent.Protocol.Instruction.Instance,
                Glash.Client.Protocol.Instruction.Instance
            };
        }

        //Register as Agent
        private Glash.Agent.Protocol.QpCommands.Register.Response ExecuteCommand_Agent_Register(
            QpChannel channel,
            Glash.Agent.Protocol.QpCommands.Register.Request request)
        {
            return new Glash.Agent.Protocol.QpCommands.Register.Response();
        }

        //Register as Client
        private Glash.Client.Protocol.QpCommands.Register.Response ExecuteCommand_Client_Register(
            QpChannel channel,
            Glash.Client.Protocol.QpCommands.Register.Request request)
        {
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
