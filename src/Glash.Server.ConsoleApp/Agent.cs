using Glash.Core.Client;
using Glash.Core.Server;
using System.Net;
using YiQiDong.Agent;
using YiQiDong.Core;
using YiQiDong.Core.Utils;
using YiQiDong.Protocol.V1.Model;

namespace Glash.Server.ConsoleApp
{
    public class Agent : AbstractAgent
    {
        public ConfigModel Config { get; private set; }

        public override void Init(ContainerInfo containerInfo)
        {
            base.Init(containerInfo);

            var imageFolder = ImagePathUtils.GetImageFolder(containerInfo.ImageId);
            var containerFolder = ContainerPathUtils.GetContainerFolder(containerInfo.Id);

            AddFunction(new YiQiDong.Core.Functions.AppSettingsConfig(imageFolder, containerFolder, () => ContainerInfo.AutoStart));
        }

        private Glash.Core.Server.GlashServer glashServer;
        public override void Start()
        {
            var appSettingsModel = Quick.Fields.AppSettings.Model.Load();
            Config = appSettingsModel.Convert<ConfigModel>();

            base.Start();

            var qpServerOptions = new Quick.Protocol.Tcp.QpTcpServerOptions()
            {
                Address = IPAddress.Parse(Config.IPAddress),
                Port = Config.Port,
                Password = Config.Password
            };
            glashServer = new GlashServer();
            glashServer.LogPushed += (sender, e) => AgentContext.Instance.LogInfo(e);
            glashServer.HandleServerOptions(qpServerOptions);
            glashServer.AgentConnected += GlashServer_AgentConnected;
            glashServer.AgentDisconnected += GlashServer_AgentDisconnected;
            glashServer.ClientConnected += GlashServer_ClientConnected;
            glashServer.ClientDisconnected += GlashServer_ClientDisconnected;
            glashServer.Init();
            glashServer.Start();
            AgentContext.Instance.LogInfo($"Listening on {Config.IPAddress}:{Config.Port}...");
        }

        private void GlashServer_AgentConnected(object sender, GlashAgentContext e)
        {
            AgentContext.Instance.LogInfo($"Agent connected.Name:{e.Name},Channel:{e.Channel.ChannelName}");
        }

        private void GlashServer_AgentDisconnected(object sender, GlashAgentContext e)
        {
            AgentContext.Instance.LogInfo($"Agent disconnected.Name:{e.Name},Channel:{e.Channel.ChannelName}");
        }

        private void GlashServer_ClientConnected(object sender, GlashClientContext e)
        {
            AgentContext.Instance.LogInfo($"Client connected.Channel:{e.Channel.ChannelName}");
        }

        private void GlashServer_ClientDisconnected(object sender, GlashClientContext e)
        {
            AgentContext.Instance.LogInfo($"Client disconnected.Channel:{e.Channel.ChannelName}");
        }

        public override void Stop()
        {
            glashServer.Stop();
            glashServer = null;
            base.Stop();
        }
    }
}
