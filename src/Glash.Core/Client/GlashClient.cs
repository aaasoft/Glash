using Quick.Protocol;

namespace Glash.Core.Client
{
    public class GlashClient : IDisposable
    {
        private QpClient qpClient;
        public event EventHandler Disconnected;

        public GlashClient(string url, string password)
        {
            var qpClientOptions = QpClientOptions.Parse(new Uri(url));
            qpClientOptions.Password = password;
            qpClientOptions.InstructionSet = new[]
            {
                Glash.Agent.Protocol.Instruction.Instance
            };
            qpClient = qpClientOptions.CreateClient();
            qpClient.Disconnected += QpClient_Disconnected;
        }

        private void QpClient_Disconnected(object sender, EventArgs e)
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public async Task ConnectAsync()
        {
            //Connect
            await qpClient.ConnectAsync();
            //Register
            await qpClient.SendCommand(new Glash.Client.Protocol.QpCommands.Register.Request());
        }

        public void Dispose()
        {
            qpClient.Disconnect();
        }
    }
}