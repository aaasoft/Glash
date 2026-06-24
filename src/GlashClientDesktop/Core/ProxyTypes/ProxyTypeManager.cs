using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace GlashClientDesktop.Core.ProxyTypes
{
    public class ProxyTypeManager
    {
        public static ProxyTypeManager Instance { get; } = new ProxyTypeManager();

        private Dictionary<string, string> formerIdDict = new();
        private Dictionary<string, ProxyTypeInfo> proxyTypeDict = new();

        public ProxyTypeInfo GetProxyTypeInfo(string proxyTypeId)
        {
            if (string.IsNullOrEmpty(proxyTypeId))
                return default;
            proxyTypeDict.TryGetValue(proxyTypeId, out ProxyTypeInfo proxyTypeInfo);
            return proxyTypeInfo;
        }

        public ProxyTypeInfo[] GetProxyTypeInfos() => proxyTypeDict.Values.ToArray();

        public string FixProxyTypeId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return id;
            if (formerIdDict.TryGetValue(id, out var newId))
                return newId;
            return id;
        }

        private void RegisterProxyType<T>(JsonTypeInfo<T> jsonTypeInfo)
            where T : class, IProxyType, new()
        {
            var model = new T();
            var type = typeof(T);
            var id = type.FullName;
            var name = model.Name;

            if (model.FormerIds != null)
                foreach (var formerId in model.FormerIds)
                    formerIdDict[formerId] = id;

            proxyTypeDict[id] = new ProxyTypeInfo(
                id,
                name,
                config =>
                {
                    if (string.IsNullOrEmpty(config))
                        return new T();
                    return JsonSerializer.Deserialize(config, jsonTypeInfo);
                });
        }

        public void Init()
        {
            RegisterProxyType(WebSerializerContext.Default.Web);
            RegisterProxyType(RDPSerializerContext.Default.RDP);
        }
    }
}
