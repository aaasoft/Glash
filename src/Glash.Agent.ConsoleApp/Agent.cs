using Glash.Core.Agent;
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
        public ConfigModel Config { get; private set; }

        public override void Init(ContainerInfo containerInfo)
        {
            base.Init(containerInfo);

            var imageFolder = ImagePathUtils.GetImageFolder(containerInfo.ImageId);
            var containerFolder = ContainerPathUtils.GetContainerFolder(containerInfo.Id);

            AddFunction(new YiQiDong.Core.Functions.AppSettingsConfig(imageFolder, containerFolder, () => ContainerInfo.AutoStart));
        }

        private Glash.Core.Agent.GlashAgent glashAgent;
        public override void Start()
        {
            var appSettingsModel = Quick.Fields.AppSettings.Model.Load();
            Config = appSettingsModel.Convert<ConfigModel>();

            base.Start();

            var qpClientOptions = QpClientOptions.Parse(new Uri(Config.ServerUrl));
            qpClientOptions.Password = Config.Password;
            qpClientOptions.InstructionSet = new[]
            {
                Glash.Agent.Protocol.Instruction.Instance
            };

            AgentContext.Instance.LogError($"Agent connecting to server[{Config.ServerUrl}]...");
            glashAgent = new Glash.Core.Agent.GlashAgent(qpClientOptions, Config.AgentName);
            glashAgent.StartAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    AgentContext.Instance.LogError($"Agent connect to server[{Config.ServerUrl}] error.Reason:" + ExceptionUtils.GetExceptionMessage(t.Exception.InnerException));
                    return;
                }
                AgentContext.Instance.LogInfo($"Agent connected to server[{Config.ServerUrl}].");
            });
        }

        public override void Stop()
        {
            glashAgent.Stop();
            glashAgent = null;
            base.Stop();
        }
    }
}
