namespace Glash.Core.Client
{
    public class ServerInfo
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Password { get; set; }
        public List<IProxyRule> ProxyList { get; set; } = new List<IProxyRule>();

        public override string ToString()
        {
            return $"Server[{Name}]";
        }
    }
}
