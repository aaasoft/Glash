using Glash.Blazor.Client.Model;
using Quick.LiteDB.Plus;
using Quick.Utils;

namespace Glash.Blazor.Client;

public class ProfileContextManager
{
    private Dictionary<string, ProfileContext> profileDict;
    public static ProfileContextManager Instance { get; } = new ProfileContextManager();
    private ProfileContextManager()
    {
        profileDict = new Dictionary<string, ProfileContext>();

        var profileModels = ConfigDbContext.CacheContext.Query<Model.Profile>();
        foreach (var model in profileModels)
        {
            profileDict[model.Id] = new ProfileContext(model);
        }
    }

    public bool AutoEnable
    {
        get
        {
            if (bool.TryParse(Config.GetConfig(nameof(AutoEnable)), out var ret))
                return ret;
            return false;
        }
        set
        {
            Config.SetConfig(nameof(AutoEnable), value.ToString());
            if (value)
                beginCheckAutoEnable();
            else
                cts?.Cancel();
        }
    }

    public ProfileContext Get(string value)
    {
        if (profileDict.TryGetValue(value, out var profileContext))
            return profileContext;
        return null;
    }
    public ProfileContext[] GetProfileContexts() => profileDict.Values.ToArray();

    public void Add(Profile model)
    {
        lock (profileDict)
        {
            ConfigDbContext.CacheContext.Add(model);
            profileDict[model.Id] = new ProfileContext(model);
        }
    }

    public void Update(Profile model)
    {
        lock (profileDict)
        {
            ConfigDbContext.CacheContext.Update(model);
        }
    }

    public void Remove(Profile model)
    {
        lock (profileDict)
        {
            ConfigDbContext.CacheContext.Remove(model, true);
            profileDict.Remove(model.Id);
        }
    }

    public ProfileContext GetContext(Profile model)
    {
        lock (profileDict)
        {
            if (profileDict.TryGetValue(model.Id, out var context))
                return context;
        }
        return null;
    }

    private CancellationTokenSource cts;

    public void Start()
    {
        if (AutoEnable)
            beginCheckAutoEnable();
    }

    public void Stop()
    {
        cts?.Cancel();
    }

    private void beginCheckAutoEnable()
    {
        cts?.Cancel();
        cts = new();
        _ = _beginCheckAutoEnable(cts.Token);
    }

    private async Task _beginCheckAutoEnable(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                foreach (var profile in GetProfileContexts())
                {
                    try
                    {
                        if (!profile.Enabled)
                            await profile.Enable();
                    }
                    catch
                    {
                        continue;
                    }
                    foreach (var agent in profile.Agents)
                    {
                        if (!agent.IsLoggedIn)
                            continue;
                        foreach (var proxyRuleContext in profile.GlashClient.ProxyRuleContexts.Where(t => t.Config.Agent == agent.AgentName))
                        {
                            if (proxyRuleContext.Config.Enable)
                                continue;
                            try { profile.GlashClient.EnableProxyRule(proxyRuleContext); }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ExceptionUtils.GetExceptionString(ex));
            }
            //检查一次休息5秒，避免网络拥堵
            await Task.Delay(5000, cancellationToken);
        }
    }
}
