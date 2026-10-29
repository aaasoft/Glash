using Glash.Client;
using Glash.Client.Protocol.QpModel;
using GlashClientDesktop.Core.ProxyTypes;
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

    private List<ProxyRuleContext> _ProxyRules = new();
    public List<ProxyRuleContext> ProxyRules
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
            GlashClient = new GlashClient(Connection.ServerUrl)
            {
                PreferHighSpeedMode = Connection.PreferHighSpeedMode
            };
            GlashClient.AgentLoginStatusChanged += GlashClient_AgentLoginStatusChanged;
            GlashClient.LogPushed += GlashClient_LogPushed;
            GlashClient.Disconnected += GlashClient_Disconnected;
        }
        catch { }
    }

    private void onRuleAdded(ProxyRuleContext rule, int? index = null)
    {
        if (index == null)
            ProxyRules.Add(rule);
        else
            ProxyRules.Insert(index.Value, rule);

        var proxyTypeInfo = ProxyTypeManager.Instance.GetProxyTypeInfo(rule.Config.ProxyType);
        var proxyType = proxyTypeInfo.CreateInstance(rule.Config.ProxyTypeConfig);
        proxyType.Start(rule);
    }

    private int onRuleDeleted(ProxyRuleContext rule)
    {
        var proxyTypeInfo = ProxyTypeManager.Instance.GetProxyTypeInfo(rule.Config.ProxyType);
        var proxyType = proxyTypeInfo.CreateInstance(rule.Config.ProxyTypeConfig);
        proxyType.Stop();

        var index = ProxyRules.IndexOf(rule);
        ProxyRules.Remove(rule);
        return index;
    }

    public async Task Start()
    {
        try
        {
            await GlashClient.ConnectAsync(Connection.User, Connection.Password);
            //读取规则
            var proxyRuleList = await GlashClient.GetProxyRuleListAsync();
            foreach (var proxyRule in proxyRuleList)
                proxyRule.ProxyType = ProxyTypeManager.Instance.FixProxyTypeId(proxyRule.ProxyType);
            GlashClient.LoadProxyRules(proxyRuleList.ToArray());
            foreach (var rule in GlashClient.ProxyRuleContexts)
                onRuleAdded(rule);

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
            pushLog(Locale<ConnectionContext>.GetString("Connected"));
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
            foreach (var proxyRuleContext in ProxyRules.ToArray())
            {
                GlashClient.UnloadProxyRule(proxyRuleContext);
                onRuleDeleted(proxyRuleContext);
            }
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
        pushLog(Locale<ConnectionContext>.GetString("Disconnected"));
        ConnectedChanged?.Invoke(this, Connected);
    }

    public async Task AddProxyRule(ProxyRuleInfo model)
    {
        model = await GlashClient.SaveProxyRule(model);
        var context = GlashClient.LoadProxyRule(model);
        onRuleAdded(context);
    }

    public async Task EditProxyRule(ProxyRuleInfo model)
    {
        var context = GlashClient.UnloadProxyRule(model.Id);
        var index = onRuleDeleted(context);

        model = await GlashClient.SaveProxyRule(model);
        context = GlashClient.LoadProxyRule(model);
        onRuleAdded(context, index);
    }

    public async Task DeleteProxyRule(ProxyRuleInfo model)
    {
        var context = GlashClient.UnloadProxyRule(model.Id);
        await GlashClient.DeleteProxyRule(model.Id);
        onRuleDeleted(context);
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
