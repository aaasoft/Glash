using Glash.Client;
using Glash.Client.Protocol.QpModel;
using Quick.Localize;
using Quick.Utils;
using ReactiveUI;

namespace GlashClientDesktop.Core;

public class ConnectionContext : ReactiveObject, IDisposable
{
    public Model.Connection Connection { get; private set; }
    public GlashClient GlashClient { get; private set; }
    private Dictionary<string, AgentInfo> agentDict;

    private bool _Connected;
    public bool Connected
    {
        get => _Connected;
        set => this.RaiseAndSetIfChanged(ref _Connected, value);
    }
    
    private AgentInfo[] _Agents;
    public AgentInfo[] Agents
    {
        get => _Agents;
        set => this.RaiseAndSetIfChanged(ref _Agents, value);
    }

    private ProxyRuleInfo[] _ProxyRules;
    public ProxyRuleInfo[] ProxyRules
    {
        get => _ProxyRules;
        set => this.RaiseAndSetIfChanged(ref _ProxyRules, value);
    }

    public EventHandler<bool> ConnectedChanged;
    public EventHandler<AgentInfo> AgentLoginStatusChanged;
    public EventHandler<string[]> LogChanged;

    public ConnectionContext(Model.Connection connection)
    {
        Connection = connection;
        try
        {
            GlashClient = new GlashClient(Connection.ServerUrl);
            GlashClient.AgentLoginStatusChanged += GlashClient_AgentLoginStatusChanged;
            GlashClient.LogPushed += GlashClient_LogPushed;
            GlashClient.Disconnected += GlashClient_Disconnected;
        }
        catch { }
    }

    public async Task Start()
    {
        try
        {
            await GlashClient.ConnectAsync(Connection.User, Connection.Password);
            var agentList = await GlashClient.GetAgentListAsync();
            Agents = agentList
                .OrderBy(t => t.AgentName)
                .OrderByDescending(t => t.IsLoggedIn)
                .ToArray();
            agentDict = Agents.ToDictionary(t => t.AgentName, t => t);

            var proxyRuleList = await GlashClient.GetProxyRuleListAsync();
            ProxyRules = proxyRuleList
                .OrderBy(t => t.Name)
                .ToArray();
            GlashClient.LoadProxyRules(ProxyRules);

            Connected = true;
            pushLog(Locale.GetString("Connected"));
            ConnectedChanged?.Invoke(this, Connected);
        }
        catch (Exception ex)
        {
            pushLog(ExceptionUtils.GetExceptionMessage(ex));
            throw;
        }
    }

    public void Stop()
    {
        if (GlashClient != null)
        {
            foreach (var proxyRuleContext in GlashClient.ProxyRuleContexts)
                GlashClient.UnloadProxyRule(proxyRuleContext);
            GlashClient.Dispose();
        }
    }


    public static int MaxLogLines = 100;
    private Queue<string> logQueue = new();
    public string[] Logs
    {
        get
        {
            lock (logQueue)
                return logQueue.ToArray();
        }
    }
    private void pushLog(string line)
    {
        line = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {line}";
        lock (logQueue)
        {
            logQueue.Enqueue(line);
            while (true)
            {
                var currentCount = logQueue.Count;
                if (currentCount == 0 || currentCount <= MaxLogLines)
                    break;
                logQueue.Dequeue();
            }
        }
    }

    private void GlashClient_AgentLoginStatusChanged(
        object sender,
        Glash.Client.Protocol.QpNotices.AgentLoginStatusChanged e)
    {
        Task.Run(() =>
        {
            var data = e.Data;
            if (!agentDict.ContainsKey(data.AgentName))
                return;
            var agentInfo = agentDict[data.AgentName];
            agentInfo.IsLoggedIn = data.IsLoggedIn;
            AgentLoginStatusChanged?.Invoke(this, data);
        });
    }

    private void GlashClient_LogPushed(object sender, string e)
    {
        pushLog(e);
        LogChanged?.Invoke(this, Logs);
    }

    private void GlashClient_Disconnected(object sender, EventArgs e)
    {
        foreach (var proxyRuleContext in GlashClient.ProxyRuleContexts)
            GlashClient.UnloadProxyRule(proxyRuleContext);
        Connected = false;
        pushLog(Locale.GetString("Disconnected"));
        ConnectedChanged?.Invoke(this, Connected);
    }

    public async Task AddProxyRule(ProxyRuleInfo model)
    {
        model = await GlashClient.SaveProxyRule(model);
        GlashClient.LoadProxyRule(model);
    }

    public async Task DuplicateProxyRule(ProxyRuleInfo newModel)
    {
        newModel = await GlashClient.SaveProxyRule(newModel);
        GlashClient.LoadProxyRule(newModel);
    }

    public async Task EditProxyRule(ProxyRuleInfo model)
    {
        GlashClient.UnloadProxyRule(model.Id);
        model = await GlashClient.SaveProxyRule(model);
        GlashClient.LoadProxyRule(model);
    }

    public async Task DeleteProxyRule(ProxyRuleInfo model)
    {
        GlashClient.UnloadProxyRule(model.Id);
        await GlashClient.DeleteProxyRule(model.Id);
    }

    public void Dispose()
    {
        if (GlashClient != null)
        {
            GlashClient.AgentLoginStatusChanged -= GlashClient_AgentLoginStatusChanged;
            GlashClient.LogPushed -= GlashClient_LogPushed;
            GlashClient.Disconnected -= GlashClient_Disconnected;
            Stop();
            GlashClient = null;
        }
    }
}
