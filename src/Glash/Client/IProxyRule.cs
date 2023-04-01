namespace Glash.Client
{
    public interface IProxyRule
    {
        string Id { get; set; }
        string Name { get; set; }
        string Agent { get; set; }
        string LocalIPAddress { get; set; }
        int LocalPort { get; set; }
        string RemoteHost { get; set; }
        int RemotePort { get; set; }
        bool Enable { get; set; }
    }
}
