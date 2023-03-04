using Quick.Protocol;

namespace Glash.Core.Agent
{
    public class GlashAgent : IDisposable
    {
        private QpClient qpClient;
        private string agentName;
        public event EventHandler Disconnected;

        public GlashAgent(string url, string password, string agentName)
        {
            var qpClientOptions = QpClientOptions.Parse(new Uri(url));
            qpClientOptions.Password = password;
            qpClientOptions.InstructionSet = new[]
            {
                Glash.Agent.Protocol.Instruction.Instance
            };
            qpClient = qpClientOptions.CreateClient();
            qpClient.Disconnected += QpClient_Disconnected;
            this.agentName = agentName;
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
            await qpClient.SendCommand(new Glash.Agent.Protocol.QpCommands.Register.Request()
            {
                Name = agentName
            });
        }

        public void Dispose()
        {
            qpClient.Disconnect();
        }
    }
}
