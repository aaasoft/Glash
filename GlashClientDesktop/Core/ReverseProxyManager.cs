using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Quick.Localize;
using Yarp.ReverseProxy.Configuration;

namespace GlashClientDesktop.Core;

public class ReverseProxyManager : IProxyConfigProvider
{
    public static ReverseProxyManager Instance { get; } = new ReverseProxyManager();
    public int ListenPort { get; set; } = 80;

    public event EventHandler<string> RuleAdded;
    public event EventHandler<string> RuleRemoved;

    private class InMemoryConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }

        public IReadOnlyList<RouteConfig> Routes { get; }

        public IReadOnlyList<ClusterConfig> Clusters { get; }

        public IChangeToken ChangeToken { get; }

        internal void SignalChange()
        {
            _cts.Cancel();
        }
    }

    private Dictionary<string, RouteConfig> routeDict = new Dictionary<string, RouteConfig>();
    private Dictionary<string, ClusterConfig> clusterDict = new Dictionary<string, ClusterConfig>();
    private volatile InMemoryConfig _config;
    public IProxyConfig GetConfig() => _config;

    public ReverseProxyManager()
    {
        _config = new InMemoryConfig([], []);
    }

    private void Update()
    {
        var oldConfig = _config;
        _config = new InMemoryConfig(routeDict.Values.ToArray(), clusterDict.Values.ToArray());
        oldConfig.SignalChange();
    }

    public IReverseProxyBuilder Load(IReverseProxyBuilder builder)
    {
        builder.Services.AddSingleton<IProxyConfigProvider>(this);
        return builder;
    }

    public IReverseProxyBuilder LoadWithoutData(IReverseProxyBuilder builder)
    {
        builder.Services.AddSingleton<IProxyConfigProvider>(this);
        return builder;
    }

    public void AddRule(string domain, string url)
    {
        lock (routeDict)
        {
            if (routeDict.ContainsKey(domain))
                throw new IOException(Locale<ReverseProxyManager>.GetString("Domain[{0}] already exist.", domain));
            routeDict[domain] = new RouteConfig()
            {
                RouteId = "route" + domain,
                ClusterId = "cluster" + domain,
                Match = new RouteMatch { Hosts = [domain], Path = "{**catch-all}" }
            };
            
            clusterDict[domain] = new ClusterConfig()
            {
                ClusterId = "cluster" + domain,
                Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                {
                    { "destination1", new DestinationConfig() { Address = url } }
                }
            };
            Update();
        }
        RuleAdded?.Invoke(this, domain);
    }

    public void RemoveRule(string domainName)
    {
        lock (routeDict)
        {
            if (routeDict.ContainsKey(domainName))
                routeDict.Remove(domainName);
            if (clusterDict.ContainsKey(domainName))
                clusterDict.Remove(domainName);
            Update();
        }
        RuleRemoved?.Invoke(this, domainName);
    }
}
