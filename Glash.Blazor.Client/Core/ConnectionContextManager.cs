using Glash.Blazor.Client.Model;
using Quick.LiteDB.Plus;

namespace Glash.Blazor.Client.Core;

public class ConnectionContextManager
{
    private Dictionary<string, ConnectionContext> connectionDict;
    public static ConnectionContextManager Instance { get; } = new ConnectionContextManager();
    private ConnectionContextManager()
    {
        connectionDict = new Dictionary<string, ConnectionContext>();

        var connectionModels = ConfigDbContext.CacheContext.Query<Model.Connection>();
        foreach (var model in connectionModels)
        {
            connectionDict[model.Id] = new ConnectionContext(model);
        }
    }

    public ConnectionContext Get(string value)
    {
        if (connectionDict.TryGetValue(value, out var connectionContext))
            return connectionContext;
        return null;
    }
    public ConnectionContext[] GetConnectionContexts() => connectionDict.Values.ToArray();

    public void Add(Connection model)
    {
        lock (connectionDict)
        {
            ConfigDbContext.CacheContext.Add(model);
            connectionDict[model.Id] = new ConnectionContext(model);
        }
    }

    public void Update(Connection model)
    {
        lock (connectionDict)
        {
            var modelContext = GetContext(model);
            modelContext.Dispose();
            ConfigDbContext.CacheContext.Update(model);
            connectionDict[model.Id] = new ConnectionContext(model);
        }
    }

    public void Remove(Connection model)
    {
        lock (connectionDict)
        {
            ConfigDbContext.CacheContext.Remove(model, true);
            connectionDict.Remove(model.Id);
        }
    }

    public ConnectionContext GetContext(Connection model)
    {
        lock (connectionDict)
        {
            if (connectionDict.TryGetValue(model.Id, out var context))
                return context;
        }
        return null;
    }
}
