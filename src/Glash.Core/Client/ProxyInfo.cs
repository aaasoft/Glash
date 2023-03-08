using Glash.Model;

namespace Glash.Core.Client
{
    public class ProxyInfo
    {
        public string Name { get; set; }
        public string Agent { get; set; }
        public TunnelType Type { get; set; } = TunnelType.TCP;
        public string LocalIPAddress { get; set; }
        public int LocalPort { get; set; }
        public string RemoteHost { get; set; }
        public int RemotePort { get; set; }
        public bool Enable { get; set; }

        public override string ToString()
        {
            return $"Proxy[{Name}]";
        }
    }
}
