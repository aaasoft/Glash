using Glash.Client;
using Glash.Client.Protocol.QpModel;
using GlashClientDesktop.ViewModels;
using Quick.Localize;
using Quick.Utils;
using ReactiveUI;

namespace GlashClientDesktop.Core;

public class ConnectionContext : ReactiveObject, IDisposable
{
    public Model.Connection Connection { get; private set; }
    public GlashClient GlashClient { get; private set; }
    private Dictionary<string, ConnectionAgentProxiesViewModel> agentDict;

    private bool _Connected;
    public bool Connected
    {
        get => _Connected;
        set => this.RaiseAndSetIfChanged(ref _Connected, value);
    }

    private ConnectionAgentProxiesViewModel[] _Agents;
    public ConnectionAgentProxiesViewModel[] Agents
    {
        get => _Agents;
        set => this.RaiseAndSetIfChanged(ref _Agents, value);
    }

    private ProxyRuleContext[] _ProxyRules;
    public ProxyRuleContext[] ProxyRules
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
            //读取规则
            var proxyRuleList = await GlashClient.GetProxyRuleListAsync();
            GlashClient.LoadProxyRules(proxyRuleList.ToArray());
            ProxyRules = GlashClient.ProxyRuleContexts;

            //读取代理端
            var agentList = await GlashClient.GetAgentListAsync();
            Agents = agentList
                .OrderBy(t => t.AgentName)
                .Select(t => new ConnectionAgentProxiesViewModel()
                {
                    ConnectionContext = this,
                    Name = t.AgentName,
                    Connected = t.IsLoggedIn,
                    Rules = ProxyRules.Where(r => r.Config.Agent == t.AgentName).ToArray()
                })
                .ToArray();
            agentDict = Agents.ToDictionary(t => t.Name, t => t);

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
            agentInfo.Connected = data.IsLoggedIn;
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
