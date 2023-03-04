using Quick.Protocol;

namespace Glash.Core.Client
{
    public class GlashClient
    {
        private QpClient qpClient;

        public GlashClient(QpClient qpClient)
        {
            this.qpClient = qpClient;
        }

        public async Task StartAsync()
        {
            //Register
            await qpClient.SendCommand(new Glash.Client.Protocol.QpCommands.Register.Request());
        }

        public void Stop()
        {

        }
    }
}
