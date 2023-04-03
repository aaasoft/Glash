namespace Glash.Server
{
    public interface IClientManager
    {
        bool Login(LoginInfo loginInfo);
        Client.Protocol.QpModel.AgentInfo[] GetClientRelateAgents(string clientName);
        public bool IsClientRelateAgent(string clientName, string agnetName);
    }
}
