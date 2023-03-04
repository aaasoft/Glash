using Glash.Model;

namespace Glash.Core.Client
{
    public class ProxyPortInfo
    {
        public string AgentName { get; set; }
        public ProtocolType ProtocolType { get; set; }
        public string LocalIPAddress { get; set; }
        public int LocalPort { get; set; }
        public string RemoteHost { get; set; }
        public int RemotePort { get; set; }
        public bool Enable { get; set; }
    }
}
