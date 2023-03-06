using Glash.Core.Agent;
using Glash.Core.Client;
using Quick.Protocol;
using System.Net;
using YiQiDong.Agent;
using YiQiDong.Core;
using YiQiDong.Core.Utils;
using YiQiDong.Protocol.V1.Model;

namespace Glash.Agent.ConsoleApp
{
    public class Agent : AbstractAgent
    {
        private CancellationTokenSource cts;
        private GlashAgent glashAgent;

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

            glashAgent = new GlashAgent(Config.ServerUrl, Config.Password, Config.AgentName);
            glashAgent.LogPushed += (sender, e) => AgentContext.Instance.LogInfo(e);
            glashAgent.Disconnected += GlashAgent_Disconnected;
            _ = beginConnect(cts.Token);
        }

        private void GlashAgent_Disconnected(object sender, EventArgs e)
        {
            AgentContext.Instance.LogError($"Agent disconnected from server[{Config.ServerUrl}].");
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
                AgentContext.Instance.LogInfo($"Agent connecting to server[{Config.ServerUrl}]...");
                await glashAgent.ConnectAsync();
                AgentContext.Instance.LogInfo($"Agent connected to server[{Config.ServerUrl}].");
            }
            catch (Exception ex)
            {
                AgentContext.Instance.LogError($"Agent connect to server[{Config.ServerUrl}] error.Reason:" + ExceptionUtils.GetExceptionMessage(ex));
                _ = delayToConnect(token);
                return;
            }
        }

        public override void Stop()
        {
            cts?.Cancel();
            cts = null;
            glashAgent.Dispose();
            glashAgent = null;
            base.Stop();
        }
    }
}
