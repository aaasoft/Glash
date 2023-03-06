using Glash.Model;
using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Core.Server
{
    public class GlashAgentContext : IDisposable
    {
        public Model.AgentInfo AgentInfo { get; private set; }
        public QpChannel Channel { get; private set; }
        public GlashAgentContext(Model.AgentInfo agentInfo, QpChannel channel)
        {
            this.AgentInfo = agentInfo;
            this.Channel = channel;
        }

        public void Dispose()
        {

        }

        public async Task CreateTunnelAsync(TunnelInfo tunnelInfo)
        {
            await Channel.SendCommand(new Glash.Agent.Protocol.QpCommands.CreateTunnel.Request()
            {
                Data = tunnelInfo
            });
        }

        public async Task StartTunnelAsync(int tunnelId)
        {
            await Channel.SendCommand(new Glash.Agent.Protocol.QpCommands.StartTunnel.Request()
            {
                TunnelId = tunnelId
            });
        }
    }
}
