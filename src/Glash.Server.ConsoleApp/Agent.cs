using System.Net;
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

        private Quick.Protocol.Tcp.QpTcpServer qpServer;
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
            qpServer = new Quick.Protocol.Tcp.QpTcpServer(qpServerOptions);
            glashServer = new Glash.Core.Server.GlashServer();
            glashServer.Init(qpServer);
            glashServer.Start();
        }

        public override void Stop()
        {
            glashServer.Stop();
            glashServer = null;
            qpServer.Stop();
            qpServer = null;
            base.Stop();
        }
    }
}
