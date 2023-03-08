namespace Glash.Core.Client
{
    public class ServerInfo
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Password { get; set; }
        public List<ProxyInfo> ProxyList { get; set; } = new List<ProxyInfo>();

        public override string ToString()
        {
            return $"Server[{Name}]";
        }
    }
}
