using Glash.Model;
using NJsonSchema.Annotations;
using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Core.Server
{
    public class GlashServerTunnelContext : IDisposable
    {
        public TunnelInfo TunnelInfo { get; private set; }
        public GlashClientContext Client { get; private set; }
        public GlashAgentContext Agent { get; private set; }
        public DateTime CreateTime { get; private set; }
        public long UploadBytes { get; private set; }
        public long DownloadBytes { get; private set; }

        private Action<Exception> errorHandler;

        public GlashServerTunnelContext(
            TunnelInfo tunnelInfo,
            GlashClientContext client,
            GlashAgentContext agent,
            Action<Exception> errorHandler)
        {
            this.TunnelInfo = tunnelInfo;
            this.Client = client;
            this.Agent = agent;
            this.errorHandler = errorHandler;
            CreateTime = DateTime.Now;
        }

        public void OnError(Exception ex)
        {
            errorHandler?.Invoke(ex);
        }

        private void PushData(QpChannel channel, byte[] data)
        {
            try
            {
                channel.SendNoticePackage(new G.D()
                {
                    TunnelId = TunnelInfo.Id,
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
            PushData(Client.Channel, data);
            DownloadBytes = data.Length;
        }

        public void PushDataToAgent(byte[] data)
        {
            PushData(Agent.Channel, data);
            UploadBytes += data.Length;
        }

        public void SendTunnelClosedNotice(QpChannel channel)
        {
            channel.SendNoticePackage(new Model.TunnelClosed() { TunnelId = TunnelInfo.Id });
        }

        public void SendTunnelClosedNoticeToClient()
        {
            SendTunnelClosedNotice(Client.Channel);
        }

        public void SendTunnelClosedNoticeToAgent()
        {
            SendTunnelClosedNotice(Agent.Channel);
        }

        public void StartAgentTunnel()
        {
            Agent.StartTunnelAsync(TunnelInfo.Id).Wait();
        }

        public void Dispose()
        {

        }
    }
}
