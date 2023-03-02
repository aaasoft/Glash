using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Server
{
    public class GlashServer
    {
        private QpServer qpServer;
        private CommandExecuterManager commandExecuterManager = new CommandExecuterManager();

        public GlashServer()
        {

        }

        public GlashServer(QpServer qpServer)
        {
            this.qpServer = qpServer;

        }

        public void Start()
        {
            qpServer.Start();
        }

        public void Stop()
        {
            qpServer.Stop();
        }

        public void HandleServerOptions(QpServerOptions serverOptions)
        {
            serverOptions.RegisterCommandExecuterManager(commandExecuterManager);
            serverOptions.InstructionSet = new[]
            {
                Agent.Protocol.Instruction.Instance,
                Client.Protocol.Instruction.Instance
            };
        }
    }
}
