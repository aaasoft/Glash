using Glash.Client;
using Glash.Client.Protocol.QpModel;
using Quick.Localize;
using Quick.Utils;

namespace Glash.Blazor.Client.Core;

public class ConnectionContext : IDisposable
{

    public Model.Connection Connection { get; private set; }
    public GlashClient GlashClient { get; private set; }
    public AgentInfo[] Agents { get; private set; }
    public ProxyRuleInfo[] ProxyRules { get; private set; }
    private Dictionary<string, AgentInfo> agentDict;

    public bool Connected { get; private set; }

    public EventHandler<bool> ConnectedChanged;
    public EventHandler<AgentInfo> AgentLoginStatusChanged;
    public EventHandler<string[]> LogChanged;

    private CancellationTokenSource cts;

    public ConnectionContext(Model.Connection connection)
    {
        Connection = connection;
        cts = new CancellationTokenSource();

        try
        {
            GlashClient = new GlashClient(Connection.ServerUrl);
            GlashClient.AgentLoginStatusChanged += GlashClient_AgentLoginStatusChanged;
            GlashClient.LogPushed += GlashClient_LogPushed;
            GlashClient.Disconnected += GlashClient_Disconnected;

            _ = beginConnect(cts.Token);
        }
        catch { }
    }


    private async Task delayToConnect(CancellationToken token)
    {
        try
        {
            await Task.Delay(5000, token);
            _ = beginConnect(token);
        }
        catch { }
    }

    private async Task beginConnect(CancellationToken token)
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
            pushLog(Locale<ConnectionContext>.GetString("Connected"));
            ConnectedChanged?.Invoke(this, Connected);
        }
        catch (Exception ex)
        {
            pushLog(ExceptionUtils.GetExceptionMessage(ex));
            _ = delayToConnect(token);
            return;
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
        pushLog(Locale<ConnectionContext>.GetString("Disconnected"));
        ConnectedChanged?.Invoke(this, Connected);

        var currentCts = cts;
        if (currentCts == null)
            return;
        _ = delayToConnect(currentCts.Token);
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
        cts?.Cancel();
        cts = null;
        if (GlashClient != null)
        {
            foreach (var proxyRuleContext in GlashClient.ProxyRuleContexts)
                GlashClient.UnloadProxyRule(proxyRuleContext);
            GlashClient.AgentLoginStatusChanged -= GlashClient_AgentLoginStatusChanged;
            GlashClient.LogPushed -= GlashClient_LogPushed;
            GlashClient.Disconnected -= GlashClient_Disconnected;
            GlashClient.Dispose();
            GlashClient = null;
        }
    }
}
