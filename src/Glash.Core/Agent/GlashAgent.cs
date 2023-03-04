using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Core.Agent
{
    public class GlashAgent
    {
        private QpClientOptions qpClientOptions;
        private QpClient qpClient;
        private string agentName;

        public GlashAgent(QpClientOptions qpClientOptions, string agentName)
        {
            this.qpClientOptions = qpClientOptions;
            this.qpClient = qpClientOptions.CreateClient();
            this.agentName = agentName;
        }

        public async Task StartAsync()
        {
            //Connect
            await qpClient.ConnectAsync();
            //Register
            await qpClient.SendCommand(new Glash.Agent.Protocol.QpCommands.Register.Request()
            {
                Name = agentName
            });
        }

        public void Stop()
        {
            qpClient.Disconnect();
        }
    }
}
