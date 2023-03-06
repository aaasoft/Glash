using Glash.Core.Agent;
using Glash.Core.Client;
using Quick.Protocol;
using System.Net;
using YiQiDong.Agent;
using YiQiDong.Core;
using YiQiDong.Core.Utils;
using YiQiDong.Protocol.V1.Model;

namespace Glash.Client.ConsoleApp
{
    public class Agent : AbstractAgent
    {
        private CancellationTokenSource cts;
        private GlashClient glashClient;
        public ConfigModel Config { get; private set; }

        public override void Init(ContainerInfo containerInfo)
        {
            base.Init(containerInfo);

            var imageFolder = ImagePathUtils.GetImageFolder(containerInfo.ImageId);
            var containerFolder = ContainerPathUtils.GetContainerFolder(containerInfo.Id);

            AddFunction(new YiQiDong.Core.Functions.AppSettingsConfig(imageFolder, containerFolder, () => ContainerInfo.AutoStart));
        }

        public override void Start()
        {
            var appSettingsModel = Quick.Fields.AppSettings.Model.Load();
            Config = appSettingsModel.Convert<ConfigModel>();

            base.Start();
            cts?.Cancel();
            cts = new CancellationTokenSource();

            glashClient = new GlashClient(Config.ServerUrl, Config.Password);
            glashClient.LogPushed += (sender, e) => AgentContext.Instance.LogInfo(e);
#if DEBUG
            glashClient.AddProxyPortInfo(new ProxyPortInfo()
            {
                Id = Guid.NewGuid().ToString("N"),
                Agent = "TestAgent1",
                LocalIPAddress = "127.0.0.1",
                LocalPort = 19000,
                ProtocolType = Model.ProtocolType.TCP,
                RemoteHost = "www.baidu.com",
                RemotePort = 80,
                Enable = true
            });
#endif
            glashClient.Disconnected += GlashClient_Disconnected;
            _ = beginConnect(cts.Token);
        }

        private void GlashClient_Disconnected(object sender, EventArgs e)
        {
            AgentContext.Instance.LogError($"Client disconnected from server[{Config.ServerUrl}].");
            var currentCts = cts;
            if (currentCts == null)
                return;
            _ = delayToConnect(currentCts.Token);
        }

        private async Task delayToConnect(CancellationToken token)
        {
            try
            {
                await Task.Delay(5000, token);
                _ = beginConnect(token);
            }
            catch { }
        }

        private async Task beginConnect(CancellationToken token)
        {
            try
            {
                AgentContext.Instance.LogInfo($"Client connecting to server[{Config.ServerUrl}]...");
                await glashClient.ConnectAsync();
                AgentContext.Instance.LogInfo($"Client connected to server[{Config.ServerUrl}].");
            }
            catch (Exception ex)
            {
                AgentContext.Instance.LogError($"Client connect to server[{Config.ServerUrl}] error.Reason:" + ExceptionUtils.GetExceptionMessage(ex));
                _ = delayToConnect(token);
                return;
            }
        }

        public override void Stop()
        {
            cts?.Cancel();
            cts = null;
            glashClient.Dispose();
            glashClient = null;
            base.Stop();
        }
    }
}
