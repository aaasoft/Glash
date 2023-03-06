using Glash.Model;
using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Core.Server
{
    public class GlashServerTunnelContext
    {
        private TunnelInfo tunnelInfo;
        private GlashClientContext client;
        private GlashAgentContext agent;
        private Action<Exception> errorHandler;

        public GlashServerTunnelContext(
            TunnelInfo tunnelInfo,
            GlashClientContext client,
            GlashAgentContext agent,
            Action<Exception> errorHandler)
        {
            this.tunnelInfo = tunnelInfo;
            this.client = client;
            this.agent = agent;
            this.errorHandler = errorHandler;
        }

        private void OnError(Exception ex)
        {
            errorHandler?.Invoke(ex);
        }

        private void PushData(QpChannel channel, byte[] data)
        {
            try
            {
                channel.SendNoticePackage(new G.D()
                {
                    TunnelId = tunnelInfo.Id,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        public void PushDataToClient(byte[] data)
        {
            PushData(client.Channel, data);
        }

        public void PushDataToAgent(byte[] data)
        {
            PushData(agent.Channel, data);
        }

        public void SendTunnelClosedNotice(QpChannel channel)
        {
            channel.SendNoticePackage(new Model.TunnelClosed() { TunnelId = tunnelInfo.Id });
        }
        
        public void SendTunnelClosedNoticeToClient()
        {
            SendTunnelClosedNotice(client.Channel);
        }

        public void SendTunnelClosedNoticeToAgent()
        {
            SendTunnelClosedNotice(agent.Channel);
        }

        public void StartAgentTunnel()
        {
            agent.StartTunnelAsync(tunnelInfo.Id).Wait();
        }
    }
}
