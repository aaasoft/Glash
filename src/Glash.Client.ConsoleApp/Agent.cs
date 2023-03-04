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
        public ConfigModel Config { get; private set; }

        public override void Init(ContainerInfo containerInfo)
        {
            base.Init(containerInfo);

            var imageFolder = ImagePathUtils.GetImageFolder(containerInfo.ImageId);
            var containerFolder = ContainerPathUtils.GetContainerFolder(containerInfo.Id);

            AddFunction(new YiQiDong.Core.Functions.AppSettingsConfig(imageFolder, containerFolder, () => ContainerInfo.AutoStart));
        }

        private Quick.Protocol.QpClient qpClient;
        private Glash.Core.Client.GlashClient glashClient;
        public override void Start()
        {
            var appSettingsModel = Quick.Fields.AppSettings.Model.Load();
            Config = appSettingsModel.Convert<ConfigModel>();

            base.Start();

            var qpClientOptions = Quick.Protocol.QpClientOptions.Parse(new Uri(Config.ServerUrl));
            qpClientOptions.Password = Config.Password;
            qpClientOptions.InstructionSet = new[]
{
                Glash.Client.Protocol.Instruction.Instance
            };
            qpClient = qpClientOptions.CreateClient();
            qpClient.ConnectAsync().Wait();
            glashClient = new Glash.Core.Client.GlashClient(qpClient);
            glashClient.StartAsync().Wait();
            AgentContext.Instance.LogInfo("Client connected to server.");
        }

        public override void Stop()
        {
            glashClient.Stop();
            glashClient = null;
            qpClient.Disconnect();
            qpClient = null;
            base.Stop();
        }
    }
}
